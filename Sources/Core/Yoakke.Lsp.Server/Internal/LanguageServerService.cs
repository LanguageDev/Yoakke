// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamJsonRpc;
using StreamJsonRpc.Protocol;
using Yoakke.Lsp.Model;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.Capabilities.Client;
using Yoakke.Lsp.Model.Capabilities.Server;
using Yoakke.Lsp.Model.General;
using Yoakke.Lsp.Model.TextSynchronization;
using Yoakke.Lsp.Server.Handlers;
using Yoakke.Lsp.Server.Hosting;

namespace Yoakke.Lsp.Server.Internal
{
    internal class LanguageServerService : IHostedService, IDisposable
    {
        private static readonly JsonRpcTargetOptions JsonRpcTargetOptions = new()
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

        // TODO: Interlocked anywhere? This is not thread-safe

        public string? Name { get; set; }

        public string? Version { get; set; }

        public int ExitCode { get; private set; }

        private readonly JsonRpc jsonRpc;
        private ServerState state;
        private readonly ILanguageClient languageClient;
        private readonly ITextDocumentSyncHandler? textDocumentSyncHandler;
        private ClientCapabilities clientCapabilities;

        public LanguageServerService(IServiceProvider serviceProvider, ILanguageServerBuilder builder)
        {
            this.jsonRpc = serviceProvider.GetRequiredService<JsonRpc>();
            this.state = ServerState.NotStarted;
            this.jsonRpc.AddLocalRpcTarget(this, JsonRpcTargetOptions);
            this.clientCapabilities = new ClientCapabilities { };

            // Name and version
            this.Name = builder.GetSetting("name");
            this.Version = builder.GetSetting("version");

            // Dependencies
            this.languageClient = serviceProvider.GetRequiredService<ILanguageClient>();
            this.textDocumentSyncHandler = serviceProvider.GetService<ITextDocumentSyncHandler>();
        }

        // Lifecycle ///////////////////////////////////////////////////////////

        public void Dispose() => this.jsonRpc.Dispose();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.SetServerState(ServerState.WaitingForInitializeRequest);
            this.jsonRpc.StartListening();
            return Task.CompletedTask;
        }

        // TODO: Cancellation?
        public Task StopAsync(CancellationToken cancellationToken) => this.jsonRpc.Completion;

        // Messages ////////////////////////////////////////////////////////////

        // General /////////////////////

        [JsonRpcMethod("initialize", UseSingleObjectParameterDeserialization = true)]
        private InitializeResult Initialize(InitializeParams initializeParams)
        {
            this.AssertState(ServerState.WaitingForInitializeRequest, JsonRpcErrorCode.InvalidRequest);
            // We are waiting for an initialize request and just received one, answer with the proper result
            this.clientCapabilities = initializeParams.Capabilities;
            var result = new InitializeResult
            {
                ServerInfo = new InitializeResult.ServerInformation
                {
                    Name = this.Name ?? "Language Server",
                    Version = this.Version ?? "0.0.1",
                },
                Capabilities = new ServerCapabilities
                {
                    TextDocumentSync = this.GetStaticTextDocumentSyncCapability(),
                },
            };
            // Increment server state
            this.SetServerState(ServerState.WaitingForInitializedNotification);
            return result;
        }

        [JsonRpcMethod("initialized", UseSingleObjectParameterDeserialization = true)]
        private void Initialized(InitializedParams initializedParams)
        {
            this.AssertState(ServerState.WaitingForInitializedNotification, JsonRpcErrorCode.InvalidRequest);
            // Client told us that they got our initialize response
            // Dynamically register capabilities that we need
            if (this.textDocumentSyncHandler is not null && this.RegisterTextDocumentSyncDynamically())
            {
                this.languageClient.RegisterHandler(this.textDocumentSyncHandler);
            }
            // Increment server state
            // We can go into normal operation
            this.SetServerState(ServerState.Operating);
        }

        [JsonRpcMethod("shutdown")]
        private object? Shutdown()
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            // Client has requested a shutdown
            // TODO: Any cleanup?
            // Increment server state
            this.SetServerState(ServerState.ShutdownRequested);
            return null;
        }

        [JsonRpcMethod("exit")]
        private void Exit()
        {
            // The client has notified to terminate process, set appropriate exit code
            this.ExitCode = (this.state == ServerState.ShutdownRequested) ? 0 : 1;
            // Increment server state
            this.SetServerState(ServerState.Exited);
            // Kill the connection
            if (this.jsonRpc != null) this.jsonRpc.Dispose();
        }

        // Text synchronization ////////

        [JsonRpcMethod("textDocument/didOpen", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidOpen(DidOpenTextDocumentParams didOpenParams)
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            this.textDocumentSyncHandler.DidOpen(didOpenParams);
        }

        [JsonRpcMethod("textDocument/didChange", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidChange(DidChangeTextDocumentParams didChangeParams)
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            this.textDocumentSyncHandler.DidChange(didChangeParams);
        }

        // TODO: Implement if can
        [JsonRpcMethod("textDocument/willSave", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_WillSave(WillSaveTextDocumentParams willSaveParams)
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            // this.textDocumentSyncHandler.WillSave(willSaveParams);
        }

        // TODO: Implement if can
        [JsonRpcMethod("textDocument/willSaveWaitUntil", UseSingleObjectParameterDeserialization = true)]
        private IReadOnlyList<TextEdit>? TextDocument_WillSaveWaitUntil(WillSaveTextDocumentParams willSaveParams)
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            return null;
            // return this.textDocumentSyncHandler.WillSaveWaitUntil(willSaveParams);
        }

        [JsonRpcMethod("textDocument/didSave", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidSave(DidSaveTextDocumentParams didSaveParams)
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            this.textDocumentSyncHandler.DidSave(didSaveParams);
        }

        [JsonRpcMethod("textDocument/didClose", UseSingleObjectParameterDeserialization = true)]
        private void TextDocument_DidClose(DidCloseTextDocumentParams didCloseParams)
        {
            this.AssertState(ServerState.Operating, JsonRpcErrorCode.InvalidRequest);
            this.textDocumentSyncHandler.DidClose(didCloseParams);
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
            this.AssertState(expectedState, (int)errorCode);

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

        private Either<TextDocumentSyncOptions, TextDocumentSyncKind>? GetStaticTextDocumentSyncCapability() =>
            this.RegisterTextDocumentSyncDynamically()
                ? (Either<TextDocumentSyncOptions, TextDocumentSyncKind>?)null
                : new TextDocumentSyncOptions
                {
                    OpenClose = true,
                    Change = this.textDocumentSyncHandler.SyncKind,
                };

        private bool RegisterTextDocumentSyncDynamically() =>
            this.clientCapabilities?.TextDocument?.Synchronization?.DynamicRegistration == true;
    }
}
