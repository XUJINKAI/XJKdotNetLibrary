using System;
using System.IO;
using XJK.Logger;

namespace XJK.FileSystem
{
    public static class FS
    {
        public static bool IsValidPath(string path, out string FullPath)
        {
            try
            {
                FullPath = Path.GetFullPath(path);
                return true;
            }
            catch
            {
                FullPath = null;
                return false;
            }
        }

        public static string CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        public static bool PathEquals(string path1, string path2)
        {
            return Path.GetFullPath(path1).Equals(Path.GetFullPath(path2));
        }

        public static void CopyWholeFolder(string SourcePath, string DestinationPath, bool overWrite)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), overWrite);
            }
        }

        public static bool TruncateFile(string filepath, int truncateSize, int thresholdSize = 0)
        {
            var info = new FileInfo(filepath);
            int judgeSize = thresholdSize > 0 ? thresholdSize : truncateSize;
            if (info.Length > judgeSize)
            {
                string data = ReadAllText(filepath);
                int idx = data.Length - truncateSize;
                idx = Math.Min(data.Length, idx);
                WriteAllText(filepath, data.Substring(idx));
                return true;
            }
            return false;
        }

        public static string ReadAllText(string path)
        {
            Log.DEBUG($"ReadAllText {path}");
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var textReader = new StreamReader(fileStream))
                {
                    return textReader.ReadToEnd();
                }
            }
        }

        public static void WriteAllText(string path, string content)
        {
            Log.DEBUG($"WriteAllText {path}");
            File.WriteAllText(path, content);
            // 写入容易错乱
            //using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            //{
            //    using (var textWriter = new StreamWriter(fileStream))
            //    {
            //        textWriter.Write(content);
            //    }
            //}
        }

        public static void AppendText(string path, string content)
        {
            Log.DEBUG($"AppendText {path}");
            using (var fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (var textWriter = new StreamWriter(fileStream))
                {
                    textWriter.Write(content);
                }
            }
        }
    }

    public static class FileSystemExtension
    {
        public static void WriteToAll(this string text, string path)
        {
            FS.WriteAllText(path, text);
        }

        public static void WriteToAppend(this string text, string path)
        {
            FS.AppendText(path, text);
        }
    }
}
