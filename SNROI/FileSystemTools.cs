using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI
{
    public static class FileSystemTools
    {
        /// <summary>
        /// https://codereview.stackexchange.com/a/32327
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetNextAvailableFilename(string filename)
        {
            if (!File.Exists(filename)) return filename;

            string alternateFilename;
            var fileNameIndex = 1;
            do
            {
                fileNameIndex += 1;
                alternateFilename = CreateNumberedFilename(filename, fileNameIndex);
            } while (File.Exists(alternateFilename));

            return alternateFilename;
        }

        private static string CreateNumberedFilename(string filename, int number)
        {
            var plainName = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);
            return Path.Combine(Path.GetDirectoryName(filename), plainName + number + extension);
        }
    }
}
