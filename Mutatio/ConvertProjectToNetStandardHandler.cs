using System;
using System.IO;
using System.Text;
using MonoDevelop.Components.Commands;
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
                    // We don't want to miss any log messages after reloading the solution
                    var logSummary = new StringBuilder();
                    var proj = ProjectOperations.CurrentSelectedItem as DotNetProject;

                    logSummary.AppendLine($"Converting {proj.Name}{proj.GetProjFileExtension()} to NET Standard 2.0 format ..");

                    var projFilePath = proj.GetProjFilePath();

                    logSummary.AppendLine($"Generating new {proj.GetProjFileExtension()}");

                    var projectTemplate = ProjectTemplateFactory.GetTemplate(proj);
                    logSummary.AppendLine($"Project template detected: {projectTemplate}");
                    var netStandardProjContent = new NetStandardProjFileGenerator().GenerateProjForNetStandard(projectTemplate.GetPackages);

                    logSummary.AppendLine("Creating backup");

                    projectTemplate.BackupOldFormatFiles();

                    logSummary.AppendLine("Deleting old files");

                    projectTemplate.CleanUpOldFormatFiles();

                    // Create a new .xproj
                    File.WriteAllText($"{projFilePath}", netStandardProjContent, Encoding.UTF8);

                    // TODO: Programmatically reload the project instead of re-opening.
                    logSummary.AppendLine("Re-opening the project");

                    await IdeApp.Workspace.Close(true);
                    await IdeApp.Workspace.OpenWorkspaceItem(proj.ParentSolution.FileName);

                    monitor.Log.WriteLine(logSummary);

                    monitor.Log.WriteLine("Please note that .NET Standard 2.0 is supported only from Xamarin.Forms 2.4+.");
                    monitor.Log.WriteLine("More information can be found here: https://docs.microsoft.com/en-us/xamarin/xamarin-forms/internals/net-standard");
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