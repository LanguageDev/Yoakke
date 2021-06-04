using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Hosting
{
    /// <summary>
    /// Extension methods for an <see cref="ILanguageServer"/>.
    /// </summary>
    public static class LanguageServerExtensions
    {
        /// <summary>
        /// Runs a language server and blocks the calling thread until the host is shut down.
        /// </summary>
        /// <param name="languageServer">The language server to run.</param>
        /// <returns>The exit code of the language server.</returns>
        public static int Run(this ILanguageServer languageServer) => languageServer.RunAsync().Result;

        /// <summary>
        /// Runs a language server and returns a task that only completes when the language server is shut down.
        /// </summary>
        /// <param name="languageServer">The language server to run.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the execution.</param>
        /// <returns>A task that completes when the language server is shut down. Contains the exit code of the language server.</returns>
        public static async Task<int> RunAsync(this ILanguageServer languageServer, CancellationToken cancellationToken = default)
        {
            await languageServer.StartAsync(cancellationToken);
            return await languageServer.StopAsync(cancellationToken);
        }
    }
}
