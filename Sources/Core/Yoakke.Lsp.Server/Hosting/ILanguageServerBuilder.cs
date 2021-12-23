// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace Yoakke.Lsp.Server.Hosting;

/// <summary>
/// A builder for <see cref="ILanguageServer"/>.
/// </summary>
public interface ILanguageServerBuilder
{
  /// <summary>
  /// Builds the language server.
  /// </summary>
  /// <returns>The built language server.</returns>
  public ILanguageServer Build();

  /// <summary>
  /// Configures services for the language server with a delegate.
  /// </summary>
  /// <param name="configureServices">The delegate to configure with.</param>
  /// <returns>This instance to chain calls.</returns>
  public ILanguageServerBuilder ConfigureServices(Action<LanguageServerBuilderContext, IServiceCollection> configureServices);

  /// <summary>
  /// Specifies to use the given stream as an input stream.
  /// </summary>
  /// <param name="stream">The stream to use as input.</param>
  /// <returns>This instance to chain calls.</returns>
  public ILanguageServerBuilder UseInputStream(Stream stream);

  /// <summary>
  /// Specifies to use the given stream as an output stream.
  /// </summary>
  /// <param name="stream">The stream to use as output.</param>
  /// <returns>This instance to chain calls.</returns>
  public ILanguageServerBuilder UseOutputStream(Stream stream);

  /// <summary>
  /// Specifies to use the given stream as IO.
  /// </summary>
  /// <param name="stream">The stream to use as both input and output.</param>
  /// <returns>This instance to chain calls.</returns>
  public ILanguageServerBuilder UseDuplexStream(Stream stream);

  /// <summary>
  /// Specifies to use a given key-value pair in the settings.
  /// </summary>
  /// <param name="key">The key of the configuration.</param>
  /// <param name="value">The value to assign.</param>
  /// <returns>This instance to chain calls.</returns>
  public ILanguageServerBuilder UseSetting(string key, string? value);

  /// <summary>
  /// Retrieves a setting with the given key.
  /// </summary>
  /// <param name="key">The key of the setting.</param>
  /// <returns>The current value of the setting.</returns>
  public string? GetSetting(string key);
}
