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
            var jsonRpc = JsonRpc.Attach(stream, new Server());
            await jsonRpc.Completion;
        }
    }

    class Server
    {
        [JsonRpcMethod("initialize", UseSingleObjectParameterDeserialization = true)]
        public int Initialize(InitializeParams initializeParams)
        {
            var x = 0;
            return 0;
        }
    }
}
