using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
            var lspService = (LspService?)this.host.Services.GetService(typeof(LspService));
            Debug.Assert(lspService is not null);
            await lspService.StopAsync(cancellationToken);
            return lspService.ExitCode;
        }
    }
}
