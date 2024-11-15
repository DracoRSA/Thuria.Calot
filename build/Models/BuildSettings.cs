using System.Collections.Generic;

namespace Models
{
    public class BuildSettings
    {
        public string Configuration { get; set; }
        public List<NugetPackageData> NugetPackages { get; set; }
    }
}
