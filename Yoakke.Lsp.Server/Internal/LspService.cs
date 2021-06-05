using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yoakke.Lsp.Model;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.Capabilities.Server;
using Yoakke.Lsp.Model.General;
using Yoakke.Lsp.Model.TextSynchronization;
using Yoakke.Lsp.Server.Handlers;

namespace Yoakke.Lsp.Server.Internal
{
    internal class LspService : IHostedService, IDisposable
    {
        private static readonly JsonRpcTargetOptions JsonRpcTargetOptions = new JsonRpcTargetOptions
        {
            AllowNonPublicInvocation = true,
            UseSingleObjectParameterDeserialization = true,
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
        private ITextDocumentSyncHandler textDocumentSyncHandler;

        public LspService(JsonRpc jsonRpc, IServiceProvider serviceProvider)
        {
            this.jsonRpc = jsonRpc;
            this.state = ServerState.NotStarted;
            this.jsonRpc.AddLocalRpcTarget(this, JsonRpcTargetOptions);
            // Dependencies
            textDocumentSyncHandler = serviceProvider.GetRequiredService<ITextDocumentSyncHandler>();
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

        // General /////////////////////

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
                Capabilities = new ServerCapabilities
                {
                    TextDocumentSync = GetTextDocumentSyncCapability(),
                }
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

        // Text synchronization ////////

        [JsonRpcMethod("textDocument/didOpen", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidOpen(DidOpenTextDocumentParams didOpenParams)
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            textDocumentSyncHandler.DidOpen(didOpenParams);
        }

        [JsonRpcMethod("textDocument/didChange", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidChange(DidChangeTextDocumentParams didChangeParams)
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            textDocumentSyncHandler.DidChange(didChangeParams);
        }

        [JsonRpcMethod("textDocument/willSave", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_WillSave(WillSaveTextDocumentParams willSaveParams)
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            textDocumentSyncHandler.WillSave(willSaveParams);
        }

        [JsonRpcMethod("textDocument/willSaveWaitUntil", UseSingleObjectParameterDeserialization = true)]
        private IReadOnlyList<TextEdit>? TextDocument_WillSaveWaitUntil(WillSaveTextDocumentParams willSaveParams)
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            return textDocumentSyncHandler.WillSaveWaitUntil(willSaveParams);
        }

        [JsonRpcMethod("textDocument/didSave", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidSave(DidSaveTextDocumentParams didSaveParams)
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            textDocumentSyncHandler.DidSave(didSaveParams);
        }

        [JsonRpcMethod("textDocument/didClose", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidClose(DidCloseTextDocumentParams didCloseParams)
        {
            AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            textDocumentSyncHandler.DidClose(didCloseParams);
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

        // Capability construction

        private Either<TextDocumentSyncOptions, TextDocumentSyncKind> GetTextDocumentSyncCapability() => 
            textDocumentSyncHandler.SendOpenClose
                ? new TextDocumentSyncOptions
                {
                    OpenClose = true,
                    Change = textDocumentSyncHandler.SyncKind,
                }
                : textDocumentSyncHandler.SyncKind;
    }
}
