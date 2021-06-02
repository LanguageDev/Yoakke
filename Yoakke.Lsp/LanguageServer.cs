using StreamJsonRpc;
using StreamJsonRpc.Protocol;
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
        /// <summary>
        /// Creates a configurator for a language server.
        /// </summary>
        /// <returns>The created configurator.</returns>
        public static LanguageServerConfigurator Configure() => new LanguageServerConfigurator();

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
        public string Name => configurator.Name ?? "Language Server";
        /// <summary>
        /// The version of this language server.
        /// </summary>
        public string Version => configurator.Version ?? "0.0.1";

        /// <summary>
        /// The exit code of the language server.
        /// </summary>
        public int ExitCode { get; private set; }

        private LanguageServerConfigurator configurator;
        // TODO: Interlocked? This is not thread-safe
        private ServerState state;
        private JsonRpc rpcServer;

        internal LanguageServer(LanguageServerConfigurator configurator)
        {
            this.configurator = configurator;
            state = ServerState.NotStarted;
            rpcServer = new JsonRpc(configurator.GetCommunicationStream());
            rpcServer.AddLocalRpcTarget(this, new JsonRpcTargetOptions { AllowNonPublicInvocation = true });
        }

        /// <summary>
        /// Runs the RPC server.
        /// </summary>
        /// <returns>A task that completes when the Language Server is terminated and it's value is the exit code
        /// of the language server.</returns>
        public async Task<int> RunAsync()
        {
            rpcServer.StartListening();
            await rpcServer.Completion;
            return ExitCode;
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
            this.state = toState;
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
