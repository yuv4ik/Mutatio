using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using Mutatio.Extensions;
using Newtonsoft.Json;

namespace Mutatio
{
    public class ProjectWithProjectJson : IProjectTemplate
    {
        readonly DotNetProject proj;

        public ProjectWithProjectJson(DotNetProject proj)
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
            // Backup project.json
            var projectJsonFilePath = proj.GetProjectJsonFilePath();
            FileService.CopyFile(projectJsonFilePath, $"{backupFolderPath}/project.json");
            // Backup project.lock.json
            if(File.Exists(proj.GetProjectLockJsonFilePath()))
            {
                var projectLockJsonFilePath = proj.GetProjectLockJsonFilePath();
                FileService.CopyFile(projectLockJsonFilePath, $"{backupFolderPath}/project.lock.json");
            }
            // Backup AssemblyInfo.x
            FileService.CopyDirectory(proj.GetPropertiesDirPath(), $"{backupFolderPath}/Properties");
        }

        public void CleanUpOldFormatFiles()
        {
            FileService.DeleteFile(proj.GetProjectJsonFilePath());
            if (File.Exists(proj.GetProjectLockJsonFilePath()))
                FileService.DeleteFile(proj.GetProjectLockJsonFilePath());
            FileService.DeleteDirectory(proj.GetPropertiesDirPath());
            FileService.DeleteFile(proj.GetProjFilePath());
        }

        public IEnumerable<(string name, string version)> GetPackages()
        {
            var projJsonContents = File.ReadAllText(proj.GetProjectJsonFilePath(), new UTF8Encoding(true));
            var projJson = JsonConvert.DeserializeObject<ProjJson>(projJsonContents);
            return projJson.Dependencies.Select(d => (d.Key, d.Value));
        }

        public override string ToString() => "project with project.json";

        class ProjJson
        {
            [JsonProperty("dependencies")]
            public Dictionary<string, string> Dependencies { get; set; }
        }
	}
}
