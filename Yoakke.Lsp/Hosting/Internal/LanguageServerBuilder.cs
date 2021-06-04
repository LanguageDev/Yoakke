using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerdbank.Streams;
using System;
using System.Collections.Generic;
using System.IO;

namespace Yoakke.Lsp.Hosting.Internal
{
    internal class LanguageServerBuilder : ILanguageServerBuilder
    {
        private readonly IHostBuilder hostBuilder;
        private readonly LanguageServerBuilderContext builderContext;
        private readonly Dictionary<string, string?> settings;
        private Stream? duplexStream;
        private Stream? inputStream;
        private Stream? outputStream;

        public LanguageServerBuilder(IHostBuilder hostBuilder)
        {
            this.hostBuilder = hostBuilder;
            this.builderContext = new();
            this.settings = new();
        }

        public ILanguageServer Build() => new LanguageServer(hostBuilder.Build());

        public ILanguageServerBuilder ConfigureServices(Action<LanguageServerBuilderContext, IServiceCollection> configureServices)
        {
            hostBuilder.ConfigureServices((_, serviceCollection) => configureServices(builderContext, serviceCollection));
            return this;
        }

        public ILanguageServerBuilder UseInputStream(Stream stream)
        {
            this.inputStream = stream;
            this.duplexStream = null;
            return this;
        }

        public ILanguageServerBuilder UseOutputStream(Stream stream)
        {
            this.outputStream = stream;
            this.duplexStream = null;
            return this;
        }

        public ILanguageServerBuilder UseDuplexStream(Stream stream)
        {
            this.duplexStream = stream;
            this.inputStream = null;
            this.outputStream = null;
            return this;
        }

        public ILanguageServerBuilder UseSetting(string key, string? value)
        {
            settings[key] = value;
            return this;
        }

        public string? GetSetting(string key) => settings[key];

        internal Stream GetCommunicationStream()
        {
            if (duplexStream is not null) return duplexStream;
            if (inputStream is null) throw new InvalidOperationException("no input stream specified");
            if (outputStream is null) throw new InvalidOperationException("no output stream specified");
            return FullDuplexStream.Splice(inputStream, outputStream);
        }
    }
}
