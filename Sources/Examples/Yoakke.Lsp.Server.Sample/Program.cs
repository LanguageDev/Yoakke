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
        public TextDocumentSyncKind SyncKind => TextDocumentSyncKind.Incremental;

        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; } = new DocumentFilter[]
        {
            new DocumentFilter
            {
                Pattern = "**/*.txt",
            },
        };

        public void DidChange(DidChangeTextDocumentParams changeParams)
        {
            Console.Error.WriteLine("CHANGE");
            Console.Error.WriteLine($"  Uri: {changeParams.TextDocument.Uri.Value}");
            foreach (var change in changeParams.ContentChanges)
            {
                var ch = (TextDocumentContentChangeEvent.Incremental)change;
                Console.Error.WriteLine($"  From: {ch.Range.Start.Line}, {ch.Range.Start.Character}");
                Console.Error.WriteLine($"  Text: {ch.Text}");
            }
        }

        public void DidClose(DidCloseTextDocumentParams closeParams)
        {
            Console.Error.WriteLine("CLOSE");
            Console.Error.WriteLine($"  Uri: {closeParams.TextDocument.Uri.Value}");
        }

        public void DidOpen(DidOpenTextDocumentParams openParams)
        {
            Console.Error.WriteLine("OPEN");
            Console.Error.WriteLine($"  Uri: {openParams.TextDocument.Uri.Value}");
        }

        public void DidSave(DidSaveTextDocumentParams saveParams)
        {
            Console.Error.WriteLine("SAVE");
            Console.Error.WriteLine($"  Uri: {saveParams.TextDocument.Uri.Value}");
        }
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
                        .UseNameAndVersion("Test LS", "0.0.1-alpha")
                        .UseInputAndOutputStream(Console.OpenStandardInput(), Console.OpenStandardOutput())
                        .UseHandler<ITextDocumentSyncHandler, SyncHandler>();
                });
    }
}
