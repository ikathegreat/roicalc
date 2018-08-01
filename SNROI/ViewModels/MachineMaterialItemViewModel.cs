using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;

namespace SNROI.ViewModels
{
    public class MachineMaterialItemViewModel : ViewModelBase
    {
        public MachineMaterial MachineMaterial { get; set; } = new MachineMaterial();

        public double PercentOfTotalMachineCapacity
        {
            get => MachineMaterial.PercentOfTotalMachineCapacity;

            set
            {
                MachineMaterial.PercentOfTotalMachineCapacity = value;
                FireMachineMaterialChanged(MachineMaterial);
            }
        }
        



        public void UpdatePercentOfTotalMachineCapacity(double value)
        {
            MachineMaterial.PercentOfTotalMachineCapacity = value;
        }

        public delegate void MachineMaterialChangedDelegate(MachineMaterial machineMaterial);

        public event MachineMaterialChangedDelegate MachineMaterialChanged;

        public void FireMachineMaterialChanged(MachineMaterial aMachineMaterial)
        {
            MachineMaterialChanged?.Invoke(aMachineMaterial);
        }

        private RelayCommand removeMaterialCommand;
        private double percentOfTotalMachineCapacity;

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
