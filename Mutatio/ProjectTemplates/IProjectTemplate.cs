using System.Collections.Generic;

namespace Mutatio
{
    public interface IProjectTemplate
    {
        void BackupOldFormatFiles();
        void CleanUpOldFormatFiles();
        IEnumerable<(string name, string version)> GetPackages();
    }
}
