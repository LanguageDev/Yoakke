using Microsoft.Extensions.Hosting;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;
using Yoakke.Lsp.Model.General;

namespace Yoakke.Lsp.Server.Internal
{
    internal class LspService : IHostedService, IDisposable
    {
        private static readonly JsonRpcTargetOptions JsonRpcTargetOptions = new JsonRpcTargetOptions 
        { 
            AllowNonPublicInvocation = true 
        };

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

        private JsonRpc jsonRpc;

        // TODO: Interlocked anywhere? This is not thread-safe

        // TODO. Get these as configuration instead?
        public string? Name { get; set; }
        public string? Version { get; set; }

        public int ExitCode { get; private set; }
        
        private ServerState state;

        public LspService(JsonRpc jsonRpc)
        {
            this.jsonRpc = jsonRpc;
            this.state = ServerState.NotStarted;
            this.jsonRpc.AddLocalRpcTarget(this, JsonRpcTargetOptions);
        }

        // Lifecycle ///////////////////////////////////////////////////////////

        public void Dispose()
        {
            jsonRpc.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            SetServerState(ServerState.WaitingForInitializeRequest);
            jsonRpc.StartListening();
            return Task.CompletedTask;
        }

        // TODO: Cancellation?
        public Task StopAsync(CancellationToken cancellationToken) => jsonRpc.Completion;

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
                    Name = Name ?? "Language Server",
                    Version = Version ?? "0.0.1",
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
            if (jsonRpc != null) jsonRpc.Dispose();
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
            if (this.state != expectedState)
            {
                throw new LocalRpcException($"server was not in the expected {expectedState} state for the request")
                {
                    ErrorCode = errorCode,
                };
            }
        }
    }
}
