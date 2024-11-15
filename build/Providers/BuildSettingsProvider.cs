using System;
using System.IO;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

using YamlDotNet.Serialization;

using Models;

namespace Providers;

public class BuildSettingsProvider
{
    [NotNull] public BuildSettings BuildSettingsData;

    public void LoadBuildSettings([NotNull] string buildSettingsFilename)
    {
        if (buildSettingsFilename == null) throw new ArgumentNullException(nameof(buildSettingsFilename));
        if (BuildSettingsData != null) 
        {
            return;
        }

        if (!File.Exists(buildSettingsFilename))
        {
            throw new Exception($"Build Settings file not found [{buildSettingsFilename}]");
        }

        var deserializer = new DeserializerBuilder().Build();
        var yaml         = File.ReadAllText(buildSettingsFilename);

        BuildSettingsData = deserializer.Deserialize<BuildSettings>(yaml);
    }

    public string GetPackageVersion(string currentPackageName, string currentBranch)
    {
        if (BuildSettingsData == null)
        {
            throw new Exception("Build Settings not loaded");
        }

        var package = BuildSettingsData.NugetPackages?.FirstOrDefault(p => p.Name == currentPackageName);
        if (package == null)
        {
            throw new Exception($"Package {currentBranch} not found in Build Settings");
        }

        var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "0";

        return !currentBranch.ToLowerInvariant().Contains("master") &&
               !currentBranch.ToLowerInvariant().Contains("release") &&
               !currentBranch.ToLowerInvariant().Contains("hotfix")
                   ? $"{package.Version}.{buildNumber}-beta"
                   : $"{package.Version}.{buildNumber}";
    }
}
