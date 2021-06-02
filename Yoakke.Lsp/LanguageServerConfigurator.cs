using Microsoft.Extensions.DependencyInjection;
using Nerdbank.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lsp
{
    /// <summary>
    /// Provides configuration for the <see cref="LanguageServer"/>.
    /// </summary>
    public class LanguageServerConfigurator
    {
        // Name and version
        internal string? Name { get; private set; }
        internal string? Version { get; private set; }
        // IO streams
        private Stream? duplexStream;
        private Stream? inputStream;
        private Stream? outputStream;
        // Services
        private IServiceCollection serviceCollection;

        internal LanguageServerConfigurator() 
        {
            serviceCollection = new ServiceCollection();
        }

        /// <summary>
        /// Creates a new language server from the current configuration.
        /// </summary>
        /// <returns>The created language server.</returns>
        public LanguageServer CreateServer() => new LanguageServer(this);

        /// <summary>
        /// Sets the name of the language server.
        /// </summary>
        /// <param name="name">The new name to assign to the language server.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithName(string name)
        {
            this.Name = name;
            return this;
        }

        /// <summary>
        /// Sets the version of the language server.
        /// </summary>
        /// <param name="version">The new version to assign to the language server.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithVersion(string version)
        {
            this.Version = version;
            return this;
        }

        /// <summary>
        /// Sets the name and the version of the language server.
        /// </summary>
        /// <param name="name">The new name to assign to the language server.</param>
        /// <param name="version">The new version to assign to the language server.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithNameAndVersion(string name, string version) =>
            WithName(name).WithVersion(version);

        /// <summary>
        /// Sets the given stream as the duplex communication stream. This means the stream will be used for 
        /// both input and output.
        /// 
        /// This will remove any input or output stream specified previously.
        /// </summary>
        /// <param name="duplexStream">The duplex stream to set.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithDuplexStream(Stream duplexStream)
        {
            this.duplexStream = duplexStream;
            this.inputStream = null;
            this.outputStream = null;
            return this;
        }

        /// <summary>
        /// Sets the given stream as the input communication stream.
        /// 
        /// This will remove any duplex stream specified previously.
        /// </summary>
        /// <param name="inputStream">The input stream to set.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithInputStream(Stream inputStream)
        {
            this.inputStream = inputStream;
            this.duplexStream = null;
            return this;
        }

        /// <summary>
        /// Sets the given stream as the output communication stream.
        /// 
        /// This will remove any duplex stream specified previously.
        /// </summary>
        /// <param name="outputStream">The output stream to set.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithOutputStream(Stream outputStream)
        {
            this.outputStream = outputStream;
            this.duplexStream = null;
            return this;
        }

        /// <summary>
        /// Sets the given streams as the input- and output communication streams.
        /// 
        /// This will remove any duplex stream specified previously.
        /// </summary>
        /// <param name="inputStream">The input stream to set.</param>
        /// <param name="outputStream">The output stream to set.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator WithInputAndOutputStream(Stream inputStream, Stream outputStream) =>
            WithInputStream(inputStream).WithOutputStream(outputStream);

        /// <summary>
        /// Configures custom services in the DI container.
        /// </summary>
        /// <param name="configure">The configuration method.</param>
        /// <returns>This instance to chain method calls.</returns>
        public LanguageServerConfigurator ConfigureServices(Action<IServiceCollection> configure)
        {
            configure(serviceCollection);
            return this;
        }

        internal Stream GetCommunicationStream()
        {
            if (duplexStream is not null) return duplexStream;
            if (inputStream is null) throw new InvalidOperationException("no input stream specified");
            if (outputStream is null) throw new InvalidOperationException("no output stream specified");
            return FullDuplexStream.Splice(inputStream, outputStream);
        }
    }
}
