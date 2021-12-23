// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamJsonRpc;
using Yoakke.Lsp.Server.Handlers;
using Yoakke.Lsp.Server.Hosting.Internal;
using Yoakke.Lsp.Server.Internal;

namespace Yoakke.Lsp.Server.Hosting;

/// <summary>
/// Extension functionality for <see cref="IHostBuilder"/>s.
/// </summary>
public static class HostBuilderExtensions
{
  /// <summary>
  /// Adds and configures a Language Server.
  /// </summary>
  /// <param name="hostBuilder">The host builder.</param>
  /// <param name="configure">The configuration delegate.</param>
  /// <returns>The host builder to chain method calls.</returns>
  public static IHostBuilder ConfigureLanguageServer(
      this IHostBuilder hostBuilder,
      Action<ILanguageServerBuilder> configure)
  {
    var builder = new LanguageServerBuilder(hostBuilder);
    hostBuilder.ConfigureServices((_, services) =>
    {
      services.AddSingleton(_ => new JsonRpc(builder.GetCommunicationStream()));
      services.AddSingleton<ILanguageClient, LanguageClient>();
      services.AddHostedService(serviceProvider => new LanguageServerService(serviceProvider, builder));
      var provider = services.BuildServiceProvider();
    });
    // Invoke user configure
    configure(builder);
    // TODO: Inject settings into LspService (name, version, ...)?
    return hostBuilder;
  }
}
