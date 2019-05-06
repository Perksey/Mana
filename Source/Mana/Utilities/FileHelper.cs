using System;
using System.IO;
using System.Threading;

namespace Mana.Utilities
{
    public static class FileHelper
    {
        /// <summary>
        /// Gets a value that indicates whether the given file can be opened for exclusive access.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A value that indicates whether the given file can be opened for exclusive access.</returns>
        public static bool CanOpenExclusively(string path)
        {
            try
            {
                using (Stream stream = File.OpenRead(path))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Pauses execution until the given file can be opened with exclusive access.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="interval">The duration, in milliseconds, that the method will sleep between checks.</param>
        public static void WaitForFile(string path, int interval = 10)
        {
            while (!CanOpenExclusively(path))
            {
                Thread.Sleep(interval);
            }
        }
    }
}