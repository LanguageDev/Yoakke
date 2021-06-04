using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yoakke.Lsp.Hosting
{
    /// <summary>
    /// Represents a configurable Language Server.
    /// </summary>
    public interface ILanguageServer : IDisposable
    {
        /// <summary>
        /// Starts communication with the language client.
        /// </summary>
        public void Start();
        /// <summary>
        /// Starts communication with the language client.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to abort start with.</param>
        /// <returns>A task that completes when the Language Server starts.</returns>
        public Task StartAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Stops the language server gracefully.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to signal a non-graceful stop.</param>
        /// <returns>A task that completes when the language server stops. Carries the return code.</returns>
        public Task<int> StopAsync(CancellationToken cancellationToken);
    }
}
