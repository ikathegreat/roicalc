using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;

namespace SNROI.ViewModels
{
    public class MachineMaterialItemViewModel
    {
        private MachineMaterial machineMaterial = new MachineMaterial();

        public MachineMaterial MachineMaterial
        {
            get => machineMaterial;
            set
            {
                machineMaterial = value;
                FireMachineMaterialChanged(machineMaterial);
            }
        }

        public delegate void MachineMaterialChangedDelegate(MachineMaterial machineMaterial);

        public event MachineMaterialChangedDelegate MachineMaterialChanged;

        public void FireMachineMaterialChanged(MachineMaterial aMachineMaterial)
        {
            MachineMaterialChanged?.Invoke(aMachineMaterial);
        }

        private RelayCommand removeMaterialCommand;

        public RelayCommand RemoveMaterialCommand
        {
            get
            {
                return removeMaterialCommand
                       ?? (removeMaterialCommand = new RelayCommand(() => { FireRemoveMachineMaterial(MachineMaterial); }));
            }
        }
        public delegate void RemoveMachineMaterialDelegate(MachineMaterial machineMaterial);

        public event RemoveMachineMaterialDelegate RemoveMachineMaterial;

        public void FireRemoveMachineMaterial(MachineMaterial aMachineMaterial)
        {
            RemoveMachineMaterial?.Invoke(aMachineMaterial);
        }
    }
}
