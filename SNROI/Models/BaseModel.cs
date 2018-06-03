using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SNROI.Models
{

    public abstract class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void FirePropertyChanged([CallerMemberName] string aPropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
    }

    public static class Constants
    {
        public const string ReportTemplateDirectoryName = "Templates";
        public const string AppSettingsDirectoryName = "AppSettings";
        public const string ImagesDirectoryName = "Images";
    }
}
