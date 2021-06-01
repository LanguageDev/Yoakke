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
            var stream = FullDuplexStream.Splice(Console.OpenStandardInput(), Console.OpenStandardOutput());
            var jsonRpc = new JsonRpc(stream);
            jsonRpc.AddLocalRpcMethod("initialize", (Func<InitializeParams, int>)((a) =>
            {
                int x = 0;
                return 0;
            }));
            jsonRpc.StartListening();
            await jsonRpc.Completion;
        }
    }
}
