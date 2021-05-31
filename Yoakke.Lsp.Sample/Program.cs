using Nerdbank.Streams;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stream = FullDuplexStream.Splice(Console.OpenStandardInput(), Console.OpenStandardOutput());
            var jsonRpc = new JsonRpc(stream);
            jsonRpc.AddLocalRpcMethod("add", (Func<object, int>)((a) =>
            {
                int x = 0;
                return 0;
            }));
            jsonRpc.StartListening();
            await jsonRpc.Completion;
        }
    }
}
