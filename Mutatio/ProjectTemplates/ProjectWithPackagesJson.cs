using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using Mutatio.Extensions;

namespace Mutatio
{
    public class ProjectWithPackagesJson : IProjectTemplate
    {
        readonly DotNetProject proj;

        public ProjectWithPackagesJson(DotNetProject proj)
        {
            this.proj = proj;
        }

        public void BackupOldFormatFiles()
        {
            var backupFolderPath = $"{proj.BaseDirectory.FullPath.ParentDirectory}/mutatio_backup";
            // Create backup directory
            FileService.CreateDirectory(backupFolderPath);
            // Backup current .xproj
            var projFilePath = proj.GetProjFilePath();
            FileService.CopyFile(projFilePath, $"{backupFolderPath}/{proj.Name}.{proj.GetProjFileExtension()}");
            // Backup packages.config
            var packagesConfigFilePath = proj.GetPackagesJsonFilePath();
            FileService.CopyFile(packagesConfigFilePath, $"{backupFolderPath}/packages.config");

            // Backup AssemblyInfo.x
            FileService.CopyDirectory(proj.GetPropertiesDirPath(), $"{backupFolderPath}/Properties");
        }

        public void CleanUpOldFormatFiles()
        {
            FileService.DeleteFile(proj.GetPackagesJsonFilePath());
            FileService.DeleteDirectory(proj.GetPropertiesDirPath());
            FileService.DeleteFile(proj.GetProjFilePath());
        }

        public IEnumerable<(string name, string version)> GetPackages()
        {
            var doc = XDocument.Load(proj.GetPackagesJsonFilePath());
            return doc.Elements("packages").Descendants().Select(p => (p.Attribute("id").Value, p.Attribute("version").Value));
        }

        public override string ToString() => "project with packages.json";
	}
}
