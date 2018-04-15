using MonoDevelop.Projects;
using Mutatio.Extensions;

namespace Mutatio
{
    public static class ProjectTemplateFactory
    {
        public static IProjectTemplate GetTemplate(DotNetProject project)
        {
            if (project.ContainsProjectJson())
                return new ProjectWithProjectJson(project);
            else
                return new ProjectWithPackagesJson(project);
        }
    }
}
