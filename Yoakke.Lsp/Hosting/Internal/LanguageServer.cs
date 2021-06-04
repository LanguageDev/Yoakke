using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Yoakke.Lsp.Internal;

namespace Yoakke.Lsp.Hosting.Internal
{
    internal class LanguageServer : ILanguageServer
    {
        private readonly IHost host;

        public LanguageServer(IHost host)
        {
            this.host = host;
        }

        public void Dispose() => host.Dispose();

        public void Start() => host.Start();
        public Task StartAsync(CancellationToken cancellationToken) => host.StartAsync(cancellationToken);

        public async Task<int> StopAsync(CancellationToken cancellationToken)
        {
            var lspService = (LspService?)host.Services.GetService(typeof(LspService));
            Debug.Assert(lspService is not null);
            await lspService.StopAsync(cancellationToken);
            return lspService.ExitCode;
        }
    }
}
