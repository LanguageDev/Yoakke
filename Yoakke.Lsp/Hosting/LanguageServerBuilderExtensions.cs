using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Yoakke.Lsp.Hosting
{
    /// <summary>
    /// Extensions for an <see cref="ILanguageServerBuilder"/>.
    /// </summary>
    public static class LanguageServerBuilderExtensions
    {
        /// <summary>
        /// Sets the name of the language server.
        /// </summary>
        /// <param name="builder">The language server builder.</param>
        /// <param name="name">The new name to assign to the language server.</param>
        /// <returns>>The language server builder to chain method calls.</returns>
        public static ILanguageServerBuilder UseName(this ILanguageServerBuilder builder, string name) =>
            builder.UseSetting("name", name);

        /// <summary>
        /// Sets the version of the language server.
        /// </summary>
        /// <param name="builder">The language server builder.</param>
        /// <param name="version">The new version to assign to the language server.</param>
        /// <returns>>The language server builder to chain method calls.</returns>
        public static ILanguageServerBuilder UseVersion(this ILanguageServerBuilder builder, string version) =>
            builder.UseSetting("version", version);

        /// <summary>
        /// Sets the name and the version of the language server.
        /// </summary>
        /// <param name="builder">The language server builder.</param>
        /// <param name="name">The new name to assign to the language server.</param>
        /// <param name="version">The new version to assign to the language server.</param>
        /// <returns>>The language server builder to chain method calls.</returns>
        public static ILanguageServerBuilder UseNameAndVersion(this ILanguageServerBuilder builder, string name, string version) =>
            builder.UseName(name).UseVersion(version);

        /// <summary>
        /// Sets the given streams as the input- and output communication streams.
        /// </summary>
        /// <param name="builder">The language server builder.</param>
        /// <param name="inputStream">The input stream to set.</param>
        /// <param name="outputStream">The output stream to set.</param>
        /// <returns>The language server builder to chain method calls.</returns>
        public static ILanguageServerBuilder UseInputAndOutputStream(
            this ILanguageServerBuilder builder,
            Stream inputStream,
            Stream outputStream) =>
            builder.UseInputStream(inputStream).UseOutputStream(outputStream);

        /// <summary>
        /// Configures services for the language server with a delegate.
        /// </summary>
        /// <param name="builder">The language server builder.</param>
        /// <param name="configureServices">The delegate to configure with.</param>
        /// <returns>The language server builder to chain method calls.</returns>
        public static ILanguageServerBuilder ConfigureServices(
            this ILanguageServerBuilder builder,
            Action<IServiceCollection> configureServices) =>
            builder.ConfigureServices((_, serviceCollection) => configureServices(serviceCollection));

        // Handler registration ////////////////////////////////////////////////

        // TODO
    }
}
