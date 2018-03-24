using System;
using System.IO;
using System.Text;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using Mutatio.Extensions;

namespace Mutatio
{
    public class ConvertProjectToNetStandardHandler : CommandHandler
    {
        ProjectOperations ProjectOperations => IdeApp.ProjectOperations;

        protected override void Update(CommandInfo info)
        {
            info.Enabled =
                IsWorkspaceOpen()
                && ProjectIsNotBuildingOrRunning()
                && UserSelectedItemIsProject()
                && UserSelectedProjectIsPortableLibrary()
                && UserSelectedProjectIsCSharpProject()
                && UserSelectedProjectIsNotNetStandard20();
        }

        protected async override void Run()
        {
            using (var monitor = IdeApp.Workbench.ProgressMonitors.GetToolOutputProgressMonitor(false))
            {
                monitor.BeginTask(1);

                try
                {
                    var proj = ProjectOperations.CurrentSelectedItem as DotNetProject;

                    monitor.Log.WriteLine($"Converting {proj.Name}{proj.GetProjFileExtension()} to NET Standard 2.0 format ..");
                    monitor.Log.WriteLine();

                    var projFilePath = proj.GetProjFilePath();

                    monitor.Log.WriteLine($"Generating new {proj.GetProjFileExtension()}");
                    monitor.Log.WriteLine();

                    var netStandardProjContent = new NetStandardProjFileGenerator(proj).GenerateProjForNetStandard();

                    monitor.Log.WriteLine($"Creating backup");
                    monitor.Log.WriteLine();

                    BackupOldFormatFiles(proj);

                    monitor.Log.WriteLine($"Deleting old files");
                    monitor.Log.WriteLine();

                    CleanUpOldFormatFiles(proj);

                    // Create a new .xproj
                    File.WriteAllText($"{projFilePath}", netStandardProjContent, Encoding.UTF8);

                    // TODO: Programmatically reload the project instead of re-opening.
                    monitor.Log.WriteLine($"Re-opening the project");
                    monitor.Log.WriteLine();

                    await IdeApp.Workspace.Close(true);
                    await IdeApp.Workspace.OpenWorkspaceItem(proj.ParentSolution.FileName);

                    monitor.ReportSuccess("Convertion succeed.");
                }
                catch (Exception ex)
                {
                    monitor.ReportError($"Convertion failed. Please create an issue on github: {Consts.ProjectUrl}", ex);
                }
                finally
                {
                    monitor.EndTask();
                }
            }
        }

        void BackupOldFormatFiles(DotNetProject proj)
        {
            var backupFolderPath = $"{proj.BaseDirectory.FullPath.ParentDirectory}/mutatio_backup";
            // Create backup directory
            FileService.CreateDirectory(backupFolderPath);
            // Backup current .xproj
            var projFilePath = proj.GetProjFilePath();
            FileService.CopyFile(projFilePath, $"{backupFolderPath}/{proj.Name}.{proj.GetProjFileExtension()}");
            // Backup packages.config
            var packagesConfigFilePath = proj.GetPackagesFilePath();
            FileService.CopyFile(packagesConfigFilePath, $"{backupFolderPath}/packages.config");

            // Backup AssemblyInfo.x
            FileService.CopyDirectory(proj.GetPropertiesDirPath(), $"{backupFolderPath}/Properties");
        }

        void CleanUpOldFormatFiles(DotNetProject proj)
        {
            FileService.DeleteFile(proj.GetPackagesFilePath());
            // TODO: In F# there is no /Properties
            FileService.DeleteDirectory(proj.GetPropertiesDirPath());
            FileService.DeleteFile(proj.GetProjFilePath());
        }

        // Shoud be enabled only when the workspace is opened
        bool IsWorkspaceOpen() => IdeApp.Workspace.IsOpen;

        bool ProjectIsNotBuildingOrRunning()
        {
            var isBuild = ProjectOperations.IsBuilding(ProjectOperations.CurrentSelectedSolution);
            var isRun = ProjectOperations.IsRunning(ProjectOperations.CurrentSelectedSolution);

            return !isBuild && !isRun && IdeApp.ProjectOperations.CurrentBuildOperation.IsCompleted
                         && IdeApp.ProjectOperations.CurrentRunOperation.IsCompleted;
        }

        bool UserSelectedItemIsProject() => ProjectOperations.CurrentSelectedItem is DotNetProject;
        bool UserSelectedProjectIsCSharpProject() => (ProjectOperations.CurrentSelectedItem as DotNetProject).IsCSharpProject();
        bool UserSelectedProjectIsNotNetStandard20() => !(ProjectOperations.CurrentSelectedItem as DotNetProject).TargetFramework.Name.Equals(".NETStandard 2.0", StringComparison.OrdinalIgnoreCase);
        bool UserSelectedProjectIsPortableLibrary() => (ProjectOperations.CurrentSelectedItem as DotNetProject).IsPortableLibrary;
    }
}