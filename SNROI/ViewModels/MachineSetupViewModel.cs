using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;
using SNROI.ViewModels.Utilities;

namespace SNROI.ViewModels
{
    public class MachineSetupViewModel : ViewModelBase
    {
        public Machine Machine { get; set; } = new Machine();

        private RelayCommand okCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return okCommand
                       ?? (okCommand = new RelayCommand(
                           () =>
                           {
                               //Doo Stuff
                           }));
            }
        }
    }
}
