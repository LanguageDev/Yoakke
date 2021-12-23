// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerdbank.Streams;

namespace Yoakke.Lsp.Server.Hosting.Internal;

/// <summary>
/// A simple <see cref="ILanguageServerBuilder"/> implementation.
/// </summary>
internal class LanguageServerBuilder : ILanguageServerBuilder
{
  private readonly IHostBuilder hostBuilder;
  private readonly LanguageServerBuilderContext builderContext;
  private readonly Dictionary<string, string?> settings;
  private Stream? duplexStream;
  private Stream? inputStream;
  private Stream? outputStream;

  /// <summary>
  /// Initializes a new instance of the <see cref="LanguageServerBuilder"/> class.
  /// </summary>
  /// <param name="hostBuilder">The <see cref="IHostBuilder"/> to wrap.</param>
  public LanguageServerBuilder(IHostBuilder hostBuilder)
  {
    this.hostBuilder = hostBuilder;
    this.builderContext = new();
    this.settings = new();
  }

  /// <inheritdoc/>
  public ILanguageServer Build() => new LanguageServer(this.hostBuilder.Build());

  /// <inheritdoc/>
  public ILanguageServerBuilder ConfigureServices(Action<LanguageServerBuilderContext, IServiceCollection> configureServices)
  {
    this.hostBuilder.ConfigureServices((_, serviceCollection) => configureServices(this.builderContext, serviceCollection));
    return this;
  }

  /// <inheritdoc/>
  public ILanguageServerBuilder UseInputStream(Stream stream)
  {
    if (!stream.CanRead) throw new ArgumentException("the stream does not support reading", nameof(stream));
    this.inputStream = stream;
    this.duplexStream = null;
    return this;
  }

  /// <inheritdoc/>
  public ILanguageServerBuilder UseOutputStream(Stream stream)
  {
    if (!stream.CanWrite) throw new ArgumentException("the stream does not support writing", nameof(stream));
    this.outputStream = stream;
    this.duplexStream = null;
    return this;
  }

  /// <inheritdoc/>
  public ILanguageServerBuilder UseDuplexStream(Stream stream)
  {
    if (!stream.CanRead || !stream.CanWrite) throw new ArgumentException("the stream does not support both reading and writing", nameof(stream));
    this.duplexStream = stream;
    this.inputStream = null;
    this.outputStream = null;
    return this;
  }

  /// <inheritdoc/>
  public ILanguageServerBuilder UseSetting(string key, string? value)
  {
    this.settings[key] = value;
    return this;
  }

  /// <inheritdoc/>
  public string? GetSetting(string key) => this.settings.TryGetValue(key, out var value)
      ? value
      : null;

  /// <summary>
  /// Constructs a duplex communication <see cref="Stream"/> from the current configuration.
  /// </summary>
  /// <returns>The constructed duplex <see cref="Stream"/>.</returns>
  internal Stream GetCommunicationStream()
  {
    if (this.duplexStream is not null) return this.duplexStream;
    if (this.inputStream is null) throw new InvalidOperationException("no input stream specified");
    if (this.outputStream is null) throw new InvalidOperationException("no output stream specified");
    return FullDuplexStream.Splice(this.inputStream, this.outputStream);
  }
}
