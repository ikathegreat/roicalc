using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SNROI.Models
{

    public abstract class BaseModel : INotifyPropertyChanged
    {
        //private bool _isDirty;

        public event PropertyChangedEventHandler PropertyChanged;

        public void FirePropertyChanged([CallerMemberName] string aPropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
            //IsDirty = true;
        }

        //Todo: IsDirty is present on the UI, unintended
        //[XmlIgnore]
        //public bool IsDirty
        //{
        //    get => _isDirty;
        //    set
        //    {
        //        _isDirty = value;
        //    }
        //}
    }

    public static class Constants
    {
        public const string ReportTemplateDirectoryName = "Templates";
        public const string AppSettingsDirectoryName = "AppSettings";
        public const string ImagesDirectoryName = "Images";
    }
}
