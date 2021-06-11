// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.TextSynchronization;
using Yoakke.Lsp.Server.Handlers;
using Yoakke.Lsp.Server.Hosting;

namespace Yoakke.Lsp.Sample
{
    internal class SyncHandler : ITextDocumentSyncHandler
    {
        public bool SendOpenClose => true;

        public TextDocumentSyncKind SyncKind => TextDocumentSyncKind.Incremental;

        public void DidChange(DidChangeTextDocumentParams changeParams) => throw new NotImplementedException();

        public void DidClose(DidCloseTextDocumentParams closeParams) => throw new NotImplementedException();

        public void DidOpen(DidOpenTextDocumentParams openParams) => throw new NotImplementedException();

        public void DidSave(DidSaveTextDocumentParams saveParams) => throw new NotImplementedException();

        public void WillSave(WillSaveTextDocumentParams saveParams) => throw new NotImplementedException();

        public IReadOnlyList<TextEdit>? WillSaveWaitUntil(WillSaveTextDocumentParams saveParams) => throw new NotImplementedException();
    }

    internal class Program
    {
        private static async Task Main(string[] args) =>
            await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureLanguageServer(langServerBuilder =>
                {
                    langServerBuilder
                        .UseNameAndVersion("Test LS", "0.0.1")
                        .UseInputAndOutputStream(Console.OpenStandardInput(), Console.OpenStandardOutput())
                        .UseHandler<ITextDocumentSyncHandler, SyncHandler>();
                });
    }
}
