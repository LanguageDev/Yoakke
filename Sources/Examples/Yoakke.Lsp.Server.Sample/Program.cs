// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Yoakke.Lsp.Model.Basic;
using Yoakke.Lsp.Model.Diagnostics;
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

        private readonly ILanguageClient client;

        public SyncHandler(ILanguageClient client)
        {
            this.client = client;
        }

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

            Console.Error.WriteLine("AAAAAA");
            this.client.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = changeParams.TextDocument.Uri,
                Diagnostics = new Diagnostic[]
                {
                    new Diagnostic
                    {
                        Code = "E001",
                        Message = "Hello diagnosics",
                        Range = new Model.Basic.Range
                        {
                            Start = new Position
                            {
                                Line = 0,
                                Character = 0,
                            },
                            End = new Position
                            {
                                Line = 0,
                                Character = 1,
                            },
                        },
                        Severity = DiagnosticSeverity.Error,
                    },
                },
            });
            Console.Error.WriteLine("BBBBBBB");
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
                        .UseNameAndVersion("Test LS", "0.0.1-alpha2")
                        .UseInputAndOutputStream(Console.OpenStandardInput(), Console.OpenStandardOutput())
                        .UseHandler<ITextDocumentSyncHandler, SyncHandler>();
                });
    }
}
