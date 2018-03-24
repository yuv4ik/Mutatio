using System;
using MonoDevelop.Projects;

namespace Mutatio.Extensions
{
    public static class DotNetProjectExtenions
    {
        public static string GetPackagesFilePath(this DotNetProject proj) => $"{proj.BaseDirectory.FullPath}/packages.config";
        public static string GetPropertiesDirPath(this DotNetProject proj) => $"{proj.BaseDirectory.FullPath}/Properties";

        public static string GetProjFilePath(this DotNetProject proj) => proj.FileName.FullPath;
        public static string GetProjFileExtension(this DotNetProject proj) => proj.FileName.Extension;

        public static string GetAssemblyInfoFilePath(this DotNetProject proj)
        {
            switch(proj.LanguageName)
            {
                case "C#":
                    return $"{proj.BaseDirectory.FullPath}/Properties/AssemblyInfo.cs";
                case "F#":
                    return $"{proj.BaseDirectory.FullPath}/AssemblyInfo.fs";
                case "VBNET":
                    return $"{proj.BaseDirectory.FullPath}/AssemblyInfo.vb";
                default:
                    throw new NotSupportedException($"{proj.LanguageName} is currently unsupported. Please create an issue on github: {Consts.ProjectUrl}");
            }
        }

        public static bool IsCSharpProject(this DotNetProject proj) => proj.LanguageName.Equals("c#", StringComparison.OrdinalIgnoreCase);
    }
}
