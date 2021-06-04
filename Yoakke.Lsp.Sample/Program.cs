using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Yoakke.Lsp.Hosting;

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
