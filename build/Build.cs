using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.OctoVersion;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Providers;

using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    readonly BuildSettingsProvider BuildSettingsProvider = new();

    [Solution]
    readonly Solution Solution;

    [Parameter][Secret] readonly string NuGetApiKey;
    [Parameter] readonly string NuGetSource;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";
    AbsolutePath BuildOutputDirectory => ArtifactsDirectory / "build";
    AbsolutePath NugetDirectory => ArtifactsDirectory / "nuget";

    string Branch => GitRepository.FromLocalDirectory(RootDirectory.ToString()).Branch;

    // The Required Attribute will automatically throw an exception if the
    // OctoVersionInfo parameter is not set due to an error or misconfiguration in Nuke.
    // 'Framework = "net6.0"' is only required for net6.0 apps.
    [Required]
    [OctoVersion(UpdateBuildNumber = true, BranchMember = nameof(Branch), Framework = "net8.0")]
    readonly OctoVersionInfo OctoVersion;

    public static int Main()
    {
        return Execute<Build>(x => x.Pack);
    }

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Clean => _ => _
                         .Before(Restore)
                         .Executes(() =>
                         {
                             SourceDirectory.GlobDirectories("*/bin", "*/obj").ForEach(x => x.DeleteDirectory());
                             OutputDirectory.CreateOrCleanDirectory();
                         });

    Target Restore => _ => _
                           .DependsOn(Clean)
                          .Executes(() =>
                          {
                              DotNetTasks.DotNetRestore(settings => settings.SetProjectFile(Solution));
                          });

    Target Compile => _ => _
                           .DependsOn(Restore)
                           .Executes(() =>
                           {
                               // Load the Build Settings
                               BuildSettingsProvider.LoadBuildSettings(RootDirectory / ".buildSettings.yml");

                               // Build the entire solution
                               Console.WriteLine("Building the full solution");
                               DotNetBuild(settings => settings.SetProjectFile(Solution)
                                                               .EnableNoRestore()
                                                               .SetConfiguration(Configuration));

                               // Build the individual packages to be published
                               foreach (var currentPackage in BuildSettingsProvider.BuildSettingsData?.NugetPackages!)
                               {
                                   // Find the Package
                                   var currentProject = Solution.AllProjects
                                                                .FirstOrDefault(project => project.Name == currentPackage.Name);
                                   if (currentProject == null)
                                   {
                                       throw new Exception($"Project {currentPackage.Name} not found in solution");
                                   }

                                   // Determine the Package Version
                                   var packageVersion = BuildSettingsProvider.GetPackageVersion(currentPackage.Name, Branch);

                                   Console.WriteLine($"Output directory: {BuildOutputDirectory / currentProject.Name}");

                                   // Build the Package
                                   Console.WriteLine($"Building {currentProject}");
                                   DotNetBuild(settings => settings.SetProjectFile(currentProject)
                                                                   .EnableNoRestore()
                                                                   .SetOutputDirectory(BuildOutputDirectory / currentProject.Name)
                                                                   .SetAssemblyVersion(currentPackage.Version)
                                                                   .SetFileVersion(currentPackage.Version)
                                                                   .SetInformationalVersion(packageVersion)
                                                                   .SetConfiguration(Configuration));
                               }
                           });

    Target Test => _ => _
                        .DependsOn(Compile)
                        .Executes(() =>
                        {
                            var testProjects = Solution.AllProjects
                                                       .Where(project => project.Name.Contains("UnitTests"))
                                                       .Select(project => project.Path);

                            foreach (var currentProject in testProjects)
                            {
                                Console.WriteLine($"Test Project: {currentProject}");
                                DotNetTest(settings => settings.SetProjectFile(currentProject)
                                                               .SetConfiguration(Configuration)
                                                               .EnableNoRestore()
                                                               .EnableNoBuild());
                            }
                        });

    Target Pack => _ => _
                        .DependsOn(Test)
                        .Executes(() =>
                        {
                            foreach (var currentPackage in BuildSettingsProvider.BuildSettingsData?.NugetPackages!)
                            {
                                // Find the Package
                                var currentProject = Solution.AllProjects
                                                             .FirstOrDefault(project => project.Name == currentPackage.Name);
                                if (currentProject == null)
                                {
                                    throw new Exception($"Project {currentPackage.Name} not found in solution");
                                }

                                // Determine the Package Version
                                var packageVersion = BuildSettingsProvider.GetPackageVersion(currentPackage.Name, Branch);
                                Console.WriteLine($"Package Version: {packageVersion}");

                                // Package the Package
                                Console.WriteLine($"Building {currentProject}");
                                DotNetPack(s => s.SetProject(currentProject)
                                                 .SetOutputDirectory(NugetDirectory)
                                                 .SetVersion(packageVersion)
                                                 .SetIncludeSymbols(true)
                                                 .SetConfiguration(Configuration)
                                                 .EnableNoRestore()
                                                 .EnableNoBuild());
                            }
                        });

    Target Publish => _ => _
                        .DependsOn(Pack)
                        .Requires(() => NuGetApiKey)
                        .Requires(() => NuGetSource)
                        .Executes(() =>
                        {
                            Console.WriteLine("Pushing NuGet packages");
                            var filesToPush = NugetDirectory.GlobFiles("*.nupkg");

                            foreach (var currentPackage in filesToPush)
                            {
                                // if (!currentPackage.Name.Contains("Extensions"))
                                // {
                                //     continue;
                                // }

                                // Exclude the Symbols Packages
                                if (currentPackage.Name.Contains(".symbols."))
                                {
                                    Console.WriteLine($"Skipping Symbols package: {currentPackage}");
                                    continue;
                                }

                                // Push the package
                                Console.WriteLine($"Pushing {currentPackage}");
                                DotNetNuGetPush(settings => settings.SetTargetPath(currentPackage)
                                                                    .SetSource(NuGetSource)
                                                                    .SetApiKey(NuGetApiKey));
                            }
                        });

}
