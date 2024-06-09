using System.IO;
using VisGist.Data;

namespace VisGist.Helpers
{
    internal static class File
    {
        internal static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                System.IO.File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        internal static void EnsureUserSyntaxFiles()
        {
            // Ensure User Syntax Definitions are created
            if (!Directory.Exists(Constants.UserSyntaxDirectory))
            {
                Directory.CreateDirectory(Constants.UserSyntaxDirectory);
                Helpers.File.CopyFilesRecursively(Constants.DefaultSyntaxDirectory, Constants.UserSyntaxDirectory);
            }
        }
    }
}