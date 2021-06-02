using Nerdbank.Streams;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;
using Yoakke.Lsp.Model.General;

namespace Yoakke.Lsp.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var languageServer = LanguageServer.Configure()
                .WithNameAndVersion("Test Language Server", "0.0.1-pre-alpha")
                .WithInputAndOutputStream(Console.OpenStandardInput(), Console.OpenStandardOutput())
                .CreateServer();

            var exitCode = await languageServer.RunAsync();
            Console.WriteLine($"Language server exited with code {exitCode}");
        }
    }
}
