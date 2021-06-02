using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lsp.Model.General;

namespace Yoakke.Lsp
{
    // TODO: Message ordering guarantees?

    /// <summary>
    /// Implements basic LSP server logic.
    /// </summary>
    public class LanguageServer
    {
        // These should come in strictly increasing order
        private enum ServerState
        {
            NotStarted,
            WaitingForInitializeRequest,
            WaitingForInitializedNotification,
            Operating,
            ShutdownRequested,
            Exited,
        }

        /// <summary>
        /// The name of this language server.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// The version of this language server.
        /// </summary>
        public string Version { get; init; }
        /// <summary>
        /// The exit code of the language server.
        /// </summary>
        public int ExitCode { get; private set; }

        private JsonRpc? rpcServer;
        private Stream? rpcStream;
        // TODO: Interlocked? This is not thread-safe
        private ServerState state = ServerState.NotStarted;

        /// <summary>
        /// Initializes a new <see cref="LanguageServer"/>.
        /// </summary>
        /// <param name="name">The name of the language server.</param>
        /// <param name="version">The version of the language server.</param>
        public LanguageServer(string name = "My Language Server", string version = "0.0.1")
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Runs the RPC server.
        /// </summary>
        /// <returns>A task that completes when the Language Server is terminated and it's value is the exit code
        /// of the language server.</returns>
        public async Task<int> RunAsync()
        {
            rpcServer = BuildRpcServer();
            rpcServer.StartListening();
            await rpcServer.Completion;
            return ExitCode;
        }

        // Builder /////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the communication stream for this server.
        /// </summary>
        /// <param name="stream">The stream to communicate through with the client.</param>
        /// <returns>This instance, so subsequent calls can be chained.</returns>
        public LanguageServer WithStream(Stream stream)
        {
            this.rpcStream = stream;
            return this;
        }

        // Messages ////////////////////////////////////////////////////////////

        [JsonRpcMethod("initialize", UseSingleObjectParameterDeserialization = true)]
        private InitializeResult Initialize(InitializeParams initializeParams)
        {
            AssertState(ServerState.WaitingForInitializeRequest, JsonRpcErrorCode.InvalidRequest);
            // We are waiting for an initialize request and just received one, answer with the proper result
            var result = new InitializeResult
            {
                ServerInfo = new InitializeResult.ServerInformation
                {
                    Name = Name,
                    Version = Version,
                },
                // TODO
            };
            // Increment server state
            SetServerState(ServerState.WaitingForInitializedNotification);
            return result;
        }

        [JsonRpcMethod("initialized", UseSingleObjectParameterDeserialization = true)]
        private void Initialized(InitializedParams initializedParams)
        {
            AssertState(ServerState.WaitingForInitializedNotification, JsonRpcErrorCode.InvalidRequest);
            // Client told us that they got our initialize response
            // TODO: Dynamically register capabilities
            // Increment server state
            // We can go into normal operation
            SetServerState(ServerState.Operating);
        }

        [JsonRpcMethod("shutdown")]
        private object? Shutdown()
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            // Client has requested a shutdown
            // TODO: Any cleanup?
            // Increment server state
            SetServerState(ServerState.ShutdownRequested);
            return null;
        }

        [JsonRpcMethod("exit")]
        private void Exit()
        {
            // The client has notified to terminate process, set appropriate exit code
            ExitCode = (state == ServerState.ShutdownRequested) ? 0 : 1;
            // Increment server state
            SetServerState(ServerState.Exited);
            // Kill the connection
            if (rpcServer != null) rpcServer.Dispose();
        }

        // Setup ///////////////////////////////////////////////////////////////

        private JsonRpc BuildRpcServer()
        {
            if (rpcStream == null) throw new InvalidOperationException("No stream specified!");
            var result = new JsonRpc(rpcStream);
            result.AddLocalRpcTarget(this, new JsonRpcTargetOptions { AllowNonPublicInvocation = true });
            return result;
        }

        // State modification //////////////////////////////////////////////////

        private void SetServerState(ServerState toState)
        {
            if ((int)this.state > (int)toState)
            {
                throw new LocalRpcException($"server tried to go backwards in it's state progression")
                {
                    ErrorCode = (int)JsonRpcErrorCode.InternalError,
                };
            }
        }

        private void AssertState(ServerState expectedState, JsonRpcErrorCode errorCode) =>
            AssertState(expectedState, (int)errorCode);

        private void AssertState(ServerState expectedState, int errorCode)
        {
            if ((int)this.state != (int)expectedState)
            {
                throw new LocalRpcException($"server was not in the expected {expectedState} state for the request")
                {
                    ErrorCode = errorCode,
                };
            }
        }
    }
}
