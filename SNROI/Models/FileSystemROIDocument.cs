using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;

namespace SNROI.Models
{
    public class FileSystemROIDocument : BaseModel
    {
        public FileSystemROIDocument(string aFilePath)
        {
            FilePath = aFilePath;
        }

        public string ROIDocumentName => string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileNameWithoutExtension(FilePath);
        //public string CompanyName
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(FilePath))
        //            return string.Empty;
        //        if (!File.Exists(FilePath))
        //            return string.Empty;
        //        var file = ShellFile.FromFilePath(FilePath);
        //        var companyName = file.Properties.System.Company.Value;
        //        return companyName;
        //    }
        //}
        public DateTime DateModified => string.IsNullOrEmpty(FilePath) ? DateTime.MinValue : new FileInfo(FilePath).LastWriteTime;

        public DateTime DateCreated => string.IsNullOrEmpty(FilePath) ? DateTime.MinValue : new FileInfo(FilePath).CreationTime;

        public string FileSize
        {
            get
            {
                long fileSizeLength = 0;
                if (!string.IsNullOrEmpty(FilePath))
                {
                    if (File.Exists(FilePath))
                    {
                        try
                        {
                            fileSizeLength = (new FileInfo(FilePath)).Length;
                        }
                        catch { }
                    }
                }
                return BytesToFormattedString(fileSizeLength);
            }
        }
        public string FilePath { get; set; }

        private static string BytesToFormattedString(long value)
        {
            if (value < 0) { return "-" + BytesToFormattedString(-value); }
            if (value == 0) { return "0.0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return $"{adjustedSize:n0} {SizeSuffixes[mag]}";
        }

        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    }
}
