using System.IO;

namespace Boerman.Core.Helpers
{
    public static class DirectoryHelpers
    {
        public static void CreatePathIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
