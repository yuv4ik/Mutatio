using MonoDevelop.Projects;

namespace Mutatio.Extensions
{
    public static class DotNetProjectExtenions
    {
        public static string GetCsProjFilePath(this DotNetProject proj) => $"{proj.BaseDirectory.FullPath}/{proj.Name}.csproj";
        public static string GetPackagesFilePath(this DotNetProject proj) => $"{proj.BaseDirectory.FullPath}/packages.config";
        public static string GetPropertiesDirPath(this DotNetProject proj) => $"{proj.BaseDirectory.FullPath}/Properties";
    }
}
