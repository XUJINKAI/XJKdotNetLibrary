using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace XJK.FileSystem
{
    public static class FileWatcher
    {
        public static FileSystemWatcher WatchFileContent(string FilePattern)
        {
            var dir = Path.GetDirectoryName(FilePattern);
            if (!Directory.Exists(dir)) throw new DirectoryNotFoundException(dir);
            var file = Path.GetFileName(FilePattern);
            return new FileSystemWatcher(dir, file)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite,
            };
        }
    }
}
