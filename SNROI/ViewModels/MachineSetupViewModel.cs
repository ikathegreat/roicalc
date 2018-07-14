using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Machine machine = new Machine();
        public Machine Machine
        {
            get => machine;
            set
            {
                machine = value;
                if (machine == null)
                    return;
                MachineMaterialItemListCollection.Clear();
                foreach (var machineMachineMaterial in machine.MachineMaterials)
                {
                    var machineMaterialItemViewModel = new MachineMaterialItemViewModel() { MachineMaterial = machineMachineMaterial };
                    machineMaterialItemViewModel.MachineMaterialChanged += MachineMaterialItemViewModel_MachineMaterialChanged;
                    machineMaterialItemViewModel.RemoveMachineMaterial += MachineMaterialItemViewModel_RemoveMachineMaterial;
                    MachineMaterialItemListCollection.Add(machineMaterialItemViewModel);
                }
            }
        }

        private void MachineMaterialItemViewModel_RemoveMachineMaterial(MachineMaterial machineMaterial)
        {
            MachineMaterialItemListCollection.Remove(MachineMaterialItemListCollection.Single(s => s.MachineMaterial == machineMaterial));
            //Machine.MachineMaterials.Remove(machineMaterial);
        }

        private void MachineMaterialItemViewModel_MachineMaterialChanged(MachineMaterial machineMaterial)
        {
            //get new remainder
            var newRemainder = 100 - machineMaterial.PercentOfTotalMachineCapacity;

            foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
            {
                //Update new % based on ratio
                machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity
                    = machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity / 100 *
                      newRemainder;
            }

        }

        private RelayCommand okCommand;

        public RelayCommand OKCommand
        {
            get
            {
                return okCommand
                       ?? (okCommand = new RelayCommand(
                           () =>
                           {
                               Machine.MachineMaterials.Clear();
                               foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
                               {
                                   Machine.MachineMaterials.Add(machineMaterialItemViewModel.MachineMaterial);
                               }
                           }));
            }
        }
        private RelayCommand addMachineCommand;

        public RelayCommand AddTestMaterialCommand
        {
            get
            {
                return addMachineCommand
                       ?? (addMachineCommand = new RelayCommand(
                           () =>
                           {
                               //get new remainder
                               var newRemainder = 100 - ((double)100 / (MachineMaterialItemListCollection.Count + 1));
                               var isCurrentPercentSumEqualTo100 =
                                   Math.Abs(MachineMaterialItemListCollection.Sum(x =>
                                                x.MachineMaterial.PercentOfTotalMachineCapacity) - 100) < 1;
                               if (isCurrentPercentSumEqualTo100)
                               {
                                   foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
                                   {
                                       //Update new % based on ratio
                                       machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity
                                           = machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity / 100 *
                                             newRemainder;
                                   }
                               }
                               else
                               {

                               }
                               MachineMaterialItemListCollection.Add(new MachineMaterialItemViewModel()
                               {
                                   MachineMaterial = new MachineMaterial()
                                   {
                                       Name = $"Test {MachineMaterialItemListCollection.Count + 1}",
                                       CostPerWeightUnit = 4.0,
                                       PercentOfTotalMachineCapacity = isCurrentPercentSumEqualTo100 ? (double)100 / (MachineMaterialItemListCollection.Count + 1)
                                           : 100 - MachineMaterialItemListCollection.Sum(x => x.MachineMaterial.PercentOfTotalMachineCapacity),
                                       TotalMonthlyPurchasedWeight = 3
                                   }
                               });
                           }));
            }
        }

        public ObservableCollection<MachineMaterialItemViewModel> MachineMaterialItemListCollection { get; set; } = new ObservableCollection<MachineMaterialItemViewModel>();
    }
}
