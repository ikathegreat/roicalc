using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SNROI.Tools;

namespace SNROI.Test
{
    [TestClass]
    public class FileSystemToolsUnitTest
    {
        [TestMethod]
        public void GetNextAvailableFilename()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            const string fixedFileName = "file.txt";
            var firstFilePath = Path.Combine(tempDirectory, fixedFileName);

            //No directory
            Assert.AreEqual(FileSystemTools.GetNextAvailableFilename(firstFilePath), null);

            Directory.CreateDirectory(tempDirectory);

            //No file
            Assert.AreEqual(FileSystemTools.GetNextAvailableFilename(firstFilePath), firstFilePath);

            //First file
            WriteRandomTextToFile(firstFilePath);
            var filePath = Path.Combine(tempDirectory, "file2.txt");
            Assert.AreEqual(FileSystemTools.GetNextAvailableFilename(firstFilePath), filePath);

            //Second file
            WriteRandomTextToFile(filePath);
            filePath = Path.Combine(tempDirectory, "file3.txt");
            Assert.AreEqual(FileSystemTools.GetNextAvailableFilename(firstFilePath), filePath);

            //Clean up
            var di = new DirectoryInfo(tempDirectory);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            Directory.Delete(tempDirectory);

        }

        private static void WriteRandomTextToFile(string filePath)
        {
            using (var fs = File.Create(filePath))
            {
                var info = new UTF8Encoding(true).GetBytes(GetRandomString());
                fs.Write(info, 0, info.Length);
            }
        }

        private static string GetRandomString()
        {
            return Path.GetRandomFileName().Replace(".", "");
        }
    }
}
