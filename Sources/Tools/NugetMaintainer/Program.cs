// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Yoakke.NugetMaintainer;

internal static class Program
{
    private class Options
    {
        [Option("local-root", Required = true, HelpText = "The root for local packages produced.")]
        public string LocalRoot { get; set; } = string.Empty;

        [Option("prefix", Required = true, HelpText = "The prefix of package names.")]
        public string Prefix { get; set; } = string.Empty;

        [Option("owner", Required = true, HelpText = "The owner of the packages.")]
        public string Owner { get; set; } = string.Empty;

        [Option("api-key", Required = true, HelpText = "The NuGet API key.")]
        public string ApiKey { get; set; } = string.Empty;

        [Option("nightly-suffix", Required = true, HelpText = "Suffix for nightly packages.")]
        public string NightlySuffix { get; set; } = string.Empty;

        [Option("timestamp", Required = true, HelpText = "The timestamp used for the currently built nightly packages.")]
        public string Timestamp { get; set; } = string.Empty;

        [Option("timestamp-format", Required = true, HelpText = "The timestamp date-time format.")]
        public string TimestampFormat { get; set; } = string.Empty;

        [Option("lookbehind-days", Required = false, HelpText = "How many days to look behind for versions. Set to -1 for infinite.", Default = -1)]
        public int LookbehindDays { get; set; }

        [Option("delay-between-requests", Required = false, HelpText = "The number of seconds to wait between requests.", Default = 60)]
        public int DelayBetweenRequests { get; set; }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static SourceCacheContext sourceCache;
    private static SourceRepository sourceRepository;
    private static PackageSearchResource searchResource;
    private static PackageUpdateResource updateResource;
    private static FindPackageByIdResource findByIdResource;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private static async Task Main(string[] args) =>
        await Parser.Default.ParseArguments<Options>(args)
            .WithParsedAsync(Run);

    private static async Task Run(Options options)
    {
        // Set up NuGet stuff
        sourceCache = new();
        sourceRepository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
        updateResource = await sourceRepository.GetResourceAsync<PackageUpdateResource>();
        findByIdResource = await sourceRepository.GetResourceAsync<FindPackageByIdResource>();

        // Search for the nightly package IDs that were just built
        var localPackageIds = GetLocalNightlyPackageIds(options);

        // Search for the remotely published packages
        var remotePackageIds = await GetRemotePackageIds(options);

        // If there are differences, that means we removed/renmed something
        // In those cases, unist/deprecate all versions
        var toCompletelyUnlist = remotePackageIds.Except(localPackageIds).ToList();
        foreach (var packageId in toCompletelyUnlist)
        {
            await DeleteAllVersionsOfPackage(options, packageId);
        }

        // Now for each package that exists on remote, we want to unlish the nightly versions
        // These are packages that are both present locally and on remote
        var toUnlistNightly = localPackageIds.Intersect(remotePackageIds).ToList();
        foreach (var packageId in toUnlistNightly)
        {
            await DeleteAllNightlyVersionsOfPackage(options, packageId);
        }
    }

    private static async Task DeleteAllNightlyVersionsOfPackage(Options options, string id)
    {
        var nightlyVersions = await GetNightlyPackageVersions(options, id);
        await DeleteVersionsOfPackage(options, id, nightlyVersions);
    }

    private static async Task DeleteAllVersionsOfPackage(Options options, string id)
    {
        var allVersions = await GetAllPackageVersions(id);
        await DeleteVersionsOfPackage(options, id, allVersions);
    }

    private static async Task DeleteVersionsOfPackage(Options options, string id, IReadOnlyList<string> versions)
    {
        var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;

        Console.WriteLine($"Unlisting versions for {id} (wait time between request: {options.DelayBetweenRequests} seconds):");
        foreach (var version in versions)
        {
            Console.WriteLine($"  {version}");
            await Task.Delay(options.DelayBetweenRequests * 1000);
            await updateResource.Delete(
                id,
                version,
                getApiKey: packageSource => options.ApiKey,
                confirm: packageSource => true,
                noServiceEndpoint: false,
                logger);
        }
    }

    private static async Task<IReadOnlyList<string>> GetNightlyPackageVersions(Options options, string id) =>
        (await GetAllPackageVersions(id))
            .Where(v => v.EndsWith(options.NightlySuffix))
            .Where(v =>
            {
              // Now we take lookbehind into account
              if (options.LookbehindDays == -1) return true;

              // We gotta count days back
              var versionDatePart = v[..^options.NightlySuffix.Length];
              // If we fail date-parsing, assume no list
              if (!DateTime.TryParseExact(
                  versionDatePart,
                  options.TimestampFormat,
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.AssumeUniversal,
                  out var date)) return false;

              // Only list, if in the given range
              return date >= DateTime.Now.AddDays(-options.LookbehindDays);
            })
            .ToList();

    private static async Task<IReadOnlyList<string>> GetAllPackageVersions(string id)
    {
        var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;

        var versions = await findByIdResource.GetAllVersionsAsync(
            id,
            sourceCache,
            logger,
            cancellationToken);

        return versions.Select(v => v.OriginalVersion).ToList();
    }

    private static async Task<IReadOnlyList<string>> GetRemotePackageIds(Options options)
    {
        var logger = NullLogger.Instance;
        var cancellationToken = CancellationToken.None;
        var searchFilter = new SearchFilter(includePrerelease: true);

        // We search for all existing packages
        var publishedPackages = await searchResource.SearchAsync(
            $"{options.Prefix} owner:{options.Owner}",
            searchFilter,
            skip: 0,
            take: 1000,
            logger,
            cancellationToken);

        return publishedPackages.Select(p => p.Identity.Id).ToList();
    }

    private static IReadOnlyList<string> GetLocalNightlyPackageIds(Options options) =>
        GetLocalNightlyPackagePaths(options)
            .Select(GetPackageId)
            .ToList();

    private static IReadOnlyList<string> GetLocalNightlyPackagePaths(Options options) =>
        Directory.GetFiles(
            options.LocalRoot,
            $"*{options.Timestamp}-{options.NightlySuffix}.nupkg",
            SearchOption.AllDirectories);

    private static string GetPackageId(string packagePath)
    {
        using var inputStream = new FileStream(packagePath, FileMode.Open);
        using var reader = new PackageArchiveReader(inputStream);
        var nuspec = reader.NuspecReader;
        return nuspec.GetId();
    }
}
