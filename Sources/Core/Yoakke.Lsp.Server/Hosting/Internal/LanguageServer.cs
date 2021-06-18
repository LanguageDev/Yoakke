// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Yoakke.Lsp.Server.Internal;

namespace Yoakke.Lsp.Server.Hosting.Internal
{
    internal class LanguageServer : ILanguageServer
    {
        private readonly IHost host;

        public LanguageServer(IHost host)
        {
            this.host = host;
        }

        public void Dispose() => this.host.Dispose();

        public void Start() => this.host.Start();

        public Task StartAsync(CancellationToken cancellationToken) => this.host.StartAsync(cancellationToken);

        public async Task<int> StopAsync(CancellationToken cancellationToken)
        {
            var lspService = (LanguageServerService?)this.host.Services.GetService(typeof(LanguageServerService))!;
            await lspService.StopAsync(cancellationToken);
            return lspService.ExitCode;
        }
    }
}
