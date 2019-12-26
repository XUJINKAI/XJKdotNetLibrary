using System.IO;

namespace XJK.FileSystem
{
    public static class FileWatcher
    {
        public static FileSystemWatcher WatchFileContent(string FilePattern)
        {
            GetWatcherArguments(FilePattern, out var dir, out var file);
            return new FileSystemWatcher(dir, file)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite,
            };
        }

        public static void GetWatcherArguments(string FilePattern, out string Dir, out string File)
        {
            Dir = Path.GetDirectoryName(FilePattern);
            if (!Directory.Exists(Dir)) throw new DirectoryNotFoundException(Dir);
            File = Path.GetFileName(FilePattern);
        }
    }
}
