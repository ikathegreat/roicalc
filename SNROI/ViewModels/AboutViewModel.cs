using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace SNROI.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {

        public string Version
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }
        public string UpdateDate
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return new FileInfo(assembly.Location).LastWriteTime.ToShortDateString();
            }
        }
    }
}
