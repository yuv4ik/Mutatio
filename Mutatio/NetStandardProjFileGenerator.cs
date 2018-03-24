using System.Linq;
using System.Text;
using MonoDevelop.Projects;
using Mutatio.Extensions;

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

        readonly DotNetProject project;

        public NetStandardProjFileGenerator(DotNetProject project)
        {
            this.project = project;
        }

        public string GenerateProjForNetStandard()
        {
            var packagesFileParser = new PackagesFileParser();
            var packages = packagesFileParser.GetPackages(project.GetPackagesFilePath());

            var packagesText = new StringBuilder(packages.Count());
            foreach (var package in packages)
                packagesText.AppendLine($"<PackageReference Include=\"{package.name}\" Version=\"{package.version}\"/>");

            return string.Format(netStandard20ProjFormat, packagesText);
        }
    }
}