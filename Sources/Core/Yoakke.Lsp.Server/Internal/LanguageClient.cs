// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamJsonRpc;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions;
using Yoakke.Lsp.Model.Client;
using Yoakke.Lsp.Model.Diagnostics;
using Yoakke.Lsp.Server.Handlers;

namespace Yoakke.Lsp.Server.Internal
{
    internal class LanguageClient : ILanguageClient
    {
        private readonly JsonRpc jsonRpc;

        /// <summary>
        /// We batch unregistrations under a single key, so we store multiple unregistration keys
        /// for a single registration key.
        /// For example text document synchronization can register up to 5 method handlers, but we do not care about
        /// the ID of each registration, hence a common deregistration key.
        /// </summary>
        private readonly Dictionary<string, string[]> unregisterKeys;

        public LanguageClient(JsonRpc jsonRpc)
        {
            this.jsonRpc = jsonRpc;
        }

        /// <inheritdoc/>
        public string RegisterHandler(ITextDocumentSyncHandler handler)
        {
            var deregistrationKey = GenerateGuid();
            var registrations = new List<Registration>();
            var documentSelector = handler.DocumentSelector;

            // TODO: Shouldn't we be able to dynamically register for just one method in case it's not supported statically
            // by the client?

            // Register for 'textDocument/didOpen'
            registrations.Add(new Registration
            {
                Id = GenerateGuid(),
                Method = "textDocument/didOpen",
                RegisterOptions = new TextDocumentRegistrationOptions
                {
                    DocumentSelector = documentSelector,
                },
            });
            // Register for 'textDocument/didChange'
            registrations.Add(new Registration
            {
                Id = GenerateGuid(),
                Method = "textDocument/didChange",
                RegisterOptions = new TextDocumentChangeRegistrationOptions
                {
                    DocumentSelector = documentSelector,
                    SyncKind = handler.SyncKind,
                },
            });
            // Register for 'textDocument/didSave'
            registrations.Add(new Registration
            {
                Id = GenerateGuid(),
                Method = "textDocument/didSave",
                RegisterOptions = new TextDocumentSaveRegistrationOptions
                {
                    DocumentSelector = documentSelector,
                    // NOTE: We might want to control this?
                    IncludeText = false,
                },
            });
            // Register for 'textDocument/didClose'
            registrations.Add(new Registration
            {
                Id = GenerateGuid(),
                Method = "textDocument/didClose",
                RegisterOptions = new TextDocumentRegistrationOptions
                {
                    DocumentSelector = documentSelector,
                },
            });

            this.RegisterCapability(new RegistrationParams
            {
                Registrations = registrations,
            }).Wait();

            var registrationKeys = registrations.Select(r => r.Id).ToArray();
            this.unregisterKeys.Add(deregistrationKey, registrationKeys);
            return deregistrationKey;
        }

        /// <inheritdoc/>
        public void PublishDiagnostics(PublishDiagnosticsParams diagnosticsParams) =>
            this.jsonRpc.NotifyWithParameterObjectAsync("textDocument/publishDiagnostics", diagnosticsParams).Wait();

        private Task RegisterCapability(RegistrationParams @params) =>
            this.jsonRpc.InvokeWithParameterObjectAsync("client/registerCapability", @params);

        private static string GenerateGuid() => Guid.NewGuid().ToString();
    }
}
