﻿using NuGet;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
//using NuGet.Protocol.Core.v2;
using NuGet.Common;
using ServiceStack.Text;
using Subroute.Core.Data;
using Subroute.Core.Exceptions;
using Subroute.Core.Models.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using static NuGet.Protocol.Core.Types.Repository;

namespace Subroute.Core.Nuget
{
    public class NugetService : INugetService
    {
        //private readonly IPackageRepository _PackageRepository = null;

        public static string[] TargetFrameworks = new[] { ".NETFramework,Version=v4.5", ".NETFramework,Version=v4.0" };

        public NugetService()
        {
            //_PackageRepository = PackageRepositoryFactory.Default.CreateRepository(Settings.NugetPackageUri);
        }

        public string DownloadPackage(NugetPackage package)
        {
            //// First we'll determine if the package has already been download. We only need to download the package
            //// once. Then every single subroute that references this package will already have it.
            //var folder = $"{package.Id}.{package.Version}";
            //var path = Path.Combine(Settings.NugetPackageDirectory, folder);

            //if (Directory.Exists(path))
            //    return path;

            //// Attempt to locate package by ID and Version in the nuget package repository.
            //var sourcePackage = _PackageRepository.FindPackage(package.Id, SemanticVersion.Parse(package.Version));

            //// When no package was found, it could be a network error or the package was unlisted. Throw exception.
            //if (sourcePackage == null)
            //    throw new NotFoundException($"Unable to locate package. Package ID: {package.Id}, Version: {package.Version}.");

            //// When the package is located, extract its contents into the standard local directory format.
            //sourcePackage.ExtractContents(new PhysicalFileSystem(Settings.NugetPackageDirectory), folder);

            //// To simplify getting the package details, we'll return the mapped package details.
            //return path;
            return null;
        }

        public NugetPackage[] ResolveDependencies(Dependency dependency)
        {
            return null;
            //// Find the actual nuget package from the gallery to get its dependencies.
            //var package = _PackageRepository.FindPackage(dependency.Id, SemanticVersion.Parse(dependency.Version));

            //if (package == null)
            //    throw new NotFoundException($"Unable to locate package. Package ID: {dependency.Id}, Version: {dependency.Version}.");

            //// Recursively locate any additional nuget dependencies for this package that their
            //// packages, and combine into a single output array.
            //return new[] { NugetPackage.Map(package) }
            //    .Concat(ResolveDependencies(package))
            //    .ToArray();
        }

        //private NugetPackage[] ResolveDependencies(IPackage package)
        //{
        //    return null;
        //    //var walker = new DependentsWalker(_PackageRepository, new FrameworkName(".NETFramework", Version.Parse("4.6")));
        //    //walker.DependencyVersion = DependencyVersion.Highest;
        //    //walker.SkipPackageTargetCheck = true;
        //    //var dependencies = walker.GetDependents(package);
            
        //    ////// Find nuget only dependencies.
        //    ////var dependencies = package.DependencySets
        //    ////    .Where(ds => ds.TargetFramework == null && !ds.SupportedFrameworks.Any())
        //    ////    .SelectMany(ds => ds.Dependencies)
        //    ////    .ToArray();

        //    ////if (!dependencies.Any())
        //    ////    return new NugetPackage[0];

        //    ////// Find actual packages in nuget gallery by IVersionSpec.
        //    ////var foundDependencies = dependencies
        //    ////    .Select(p => _PackageRepository.FindPackage(p.Id, p.VersionSpec, true, true))
        //    ////    .ToArray();

        //    ////// Determine which packages we could not locate.
        //    ////var missing = dependencies.Where(d => !foundDependencies.Any(fd => fd.Id == d.Id)).ToArray();

        //    ////if (missing.Any())
        //    ////    throw new NotFoundException($"Unable to locate dependent packages: {string.Join(", ", missing.Select(m => m.Id))}");

        //    //// Determine if any of these dependencies have other nuget dependencies.
        //    //return dependencies
        //    //    .Select(NugetPackage.Map)
        //    //    //.Concat(foundDependencies.SelectMany(fd => ResolveDependencies(fd)))
        //    //    .ToArray();
        //}

        public async Task<PagedCollection<NugetPackage>> SearchPackagesAsync(string keyword, int? skip = null, int? take = null)
        {
            // Lets apply a guard-rail to ensure the client doesn't request too much data.
            if (take.GetValueOrDefault() > 100 || !take.HasValue)
                take = 100;

            Logger logger = new Logger();
            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            //providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
            NuGet.Configuration.PackageSource packageSource = new NuGet.Configuration.PackageSource("https://api.nuget.org/v3/index.json");
            SourceRepository sourceRepository = new SourceRepository(packageSource, providers);
            PackageSearchResource searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();
            IEnumerable<IPackageSearchMetadata> searchMetadata = await searchResource.SearchAsync(keyword, new SearchFilter(false, SearchFilterType.IsLatestVersion), skip.GetValueOrDefault(), take.GetValueOrDefault(), logger, CancellationToken.None);
            searchMetadata.Dump();

            return new PagedCollection<NugetPackage>
            {
                Results = new NugetPackage[0]
            };
        }

        //public PagedCollection<NugetPackage> SearchPackages(string keyword, int? skip = null, int? take = null)
        //{
        //    // Lets apply a guard-rail to ensure the client doesn't request too much data.
        //    if (take.GetValueOrDefault() > 100 || !take.HasValue)
        //        take = 100;

        //    var core = new RepositoryFactory().GetCoreV3("", FeedType.HttpV3);
        //    core.

        //    // Create source query to filter packages by keyword and target framework (include pre-releases).
        //    var query = _PackageRepository.Search(keyword, TargetFrameworks, true);

        //    // We'll use the PagedCollection class to return critical paging data back to the client.
        //    // Before we apply paging we'll need to get a total count back from the package store.
        //    // We will also apply the rest of the paging data for return to the client. In the event
        //    // that we don't have a page size (take), we'll use the total count since everything
        //    // will be returned.
        //    var result = new PagedCollection<NugetPackage>()
        //    {
        //        TotalCount = query.Count(),
        //        Skip = skip.GetValueOrDefault(),
        //        Take = take.GetValueOrDefault()
        //    };

        //    // Materialize the data from the nuget package repository.
        //    var packages = query
        //        .Skip(skip.GetValueOrDefault())
        //        .Take(take.GetValueOrDefault())
        //        .ToArray();

        //    // Make the actual request to the package store to get the current page of packages.
        //    // Skip and take will always be applied since we don't want to return an unbounded result set.
        //    // We'll be materializing the data, then projecting the data as a new type that is serializable.
        //    result.Results = packages.Select(NugetPackage.Map).ToArray();
            
        //    return result;
        //}
    }

    public class Logger : NuGet.Common.ILogger
    {
        public void LogDebug(string data) => $"DEBUG: {data}".Dump();
        public void LogVerbose(string data) => $"VERBOSE: {data}".Dump();
        public void LogInformation(string data) => $"INFORMATION: {data}".Dump();
        public void LogMinimal(string data) => $"MINIMAL: {data}".Dump();
        public void LogWarning(string data) => $"WARNING: {data}".Dump();
        public void LogError(string data) => $"ERROR: {data}".Dump();
        public void LogSummary(string data) => $"SUMMARY: {data}".Dump();
        public void LogInformationSummary(string data) => $"SUMMARY: {data}".Dump();
        public void LogErrorSummary(string data) => $"ERROR: {data}".Dump();
    }
}
