using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutatio
{
    public class NetStandardProjFileGenerator
    {
        string netStandard20ProjFormat => @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    {0}
  </ItemGroup>

</Project>";
        
        public string GenerateProjForNetStandard(Func<IEnumerable<(string name, string version)>> getPackages)
        {
            var packages = getPackages();

            var packagesText = new StringBuilder(packages.Count());
            foreach (var package in packages)
                packagesText.AppendLine($"<PackageReference Include=\"{package.name}\" Version=\"{package.version}\"/>");

            return string.Format(netStandard20ProjFormat, packagesText);
        }
    }
}