using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace HDS.FileHandler
{
    public static class FileUtil
    {
        /// <summary>
        /// Creates a <see cref="FileSystemWatcher"/> that does not
        /// watch subdirectories.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static FileSystemWatcher WatchFolder(string dirPath,
            string filter)
        {
            ValidateDirectoryExists(dirPath);
            if (string.IsNullOrWhiteSpace(filter))
                filter = "*.*";

            var watcher = new FileSystemWatcher
            {
                Path = dirPath,
                Filter = filter,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };
            return watcher;
        }

        /// <summary>
        /// Attempts to open a stream of the file. 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileLocked(FileInfo file)
        {
            ValidateFileExists(file.FullName);

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open,
                         FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        /// <summary>
        /// Sleeps the thread by 1/10s until the file is released or
        /// the max seconds.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="maxSeconds"></param>
        public static void WaitForFileToUnlock(FileInfo file, int maxSeconds)
        {
            ValidateFileExists(file.FullName);

            var iSleepCount = 0;
            while (FileUtil.IsFileLocked(file))
            {
                Thread.Sleep(100);
                iSleepCount++;
                if (iSleepCount >= maxSeconds * 10)
                {
                    throw new IOException(string.Format("File '{0}' was locked for more than '{1}' seconds.",
                        file.FullName, maxSeconds));
                }
            }
        }

        /// <summary>
        /// Read a file contents.
        /// </summary>
        /// <param name="filePath">Full path to file</param>
        /// <param name="encoding">Encoding <see cref="System.Text.Encoding"/></param>
        /// <param name="maxSecondsWaitForUnlock">Checks every 1/10s for file to unlock. 0 for skip wait</param>
        /// <returns></returns>
        public static IEnumerable<string> ReadFile(string filePath, Encoding encoding, int maxSecondsWaitForUnlock)
        {
            ValidateFileExists(filePath);
            if (maxSecondsWaitForUnlock > 0)
                WaitForFileToUnlock(new FileInfo(filePath), maxSecondsWaitForUnlock);
            return File.ReadAllLines(filePath, encoding);
        }

        /// <summary>
        /// Writes a text file to the destination. Will attempt to create destination directory if it doesn't exist.
        /// </summary>
        /// <param name="destinationFile"></param>
        /// <param name="fileContents"></param>
        /// <param name="encoding"></param>
        public static void WriteFile(string destinationFile, IEnumerable<string> fileContents, Encoding encoding)
        {
            ValidateDirectoryExists(Path.GetDirectoryName(destinationFile));
            File.WriteAllLines(FileNameIncrementer(destinationFile), fileContents, encoding);
        }

        /// <summary>
        /// Appends a _## to the file name until it finds a unique name.
        /// It will increase the numbers used as needed, so log_102993.txt could happen :)
        /// It will not attempt to create the directory if it does not exist, but in that case
        /// will return the incoming filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileNameIncrementer(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            var result = filePath;
            var dir = Path.GetDirectoryName(filePath) ?? string.Empty;
            var ext = Path.GetExtension(filePath) ?? string.Empty;
            var fmt = "00";

            var iCount = 1;
            while (File.Exists(result))
            {
                var fileName = Path.GetFileNameWithoutExtension(result);

                //Check for existing _## format
                if (fileName.LastIndexOf('_') > 0)
                {
                    var position = fileName.LastIndexOf('_');
                    int endsInNumber;

                    if (int.TryParse(fileName.Substring(position + 1, fileName.Length - position - 1), out endsInNumber))
                    {
                        fileName = fileName.Substring(0, position);
                    }
                }

                result = Path.Combine(dir,
                                     fileName + "_" + iCount.ToString(fmt) + ext);
                iCount++;

                //Change fmt to 000 if between 100 and 1000, e.g.
                if (iCount > 99)
                    fmt = new string('0', (int)Math.Floor(Math.Log10(iCount) + 1));
            }
            return result;
        }

        /// <summary>
        /// Moves a file from one folder to another. Will attempt to create the destination directory if it doesn't exist.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="maxSecondsWaitForUnlock"></param>
        /// <exception cref="System.IO.IOException">Throws an IO exception</exception>
        public static void MoveFile(string filePath, string destinationDirectory, int maxSecondsWaitForUnlock)
        {
            ValidateFileExists(filePath);
            ValidateDirectoryExists(destinationDirectory);

            var fi = new FileInfo(filePath);
            if (maxSecondsWaitForUnlock > 0)
                WaitForFileToUnlock(fi, maxSecondsWaitForUnlock);

            // ReSharper disable AssignNullToNotNullAttribute
            fi.MoveTo(FileNameIncrementer(Path.Combine(destinationDirectory, Path.GetFileName(filePath))));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Verifys that a file exists
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="System.IO.IOException">Throws an IO exception</exception>
        public static void ValidateFileExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) ||
                !File.Exists(filePath))
            {
                throw new IOException("File not found");
            }
        }

        /// <summary>
        /// Check if a directory exits, if not, attempt to create it.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <exception cref="System.IO.IOException">Throws an IO exception</exception>
        public static void ValidateDirectoryExists(string dirPath)
        {
            if (string.IsNullOrWhiteSpace(dirPath))
            {
                throw new IOException("Invalid directory");
            }

            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception ex)
                {
                    throw new IOException("Directory did not exist; failed to create it.", ex);
                }
            }
        }
    }
}
