using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerdbank.Streams;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;
using Yoakke.Lsp.Api.Handlers;
using Yoakke.Lsp.Hosting;
using Yoakke.Lsp.Model.General;

namespace Yoakke.Lsp.Sample
{
    class Program
    {
        static async Task Main(string[] args) => 
            await CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLanguageServer(langServerBuilder =>
                {
                    langServerBuilder
                        .UseNameAndVersion("Test LS", "0.0.1")
                        .UseInputAndOutputStream(Console.OpenStandardInput(), Console.OpenStandardOutput());
                });
    }
}
