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
                    MachineMaterialItemListCollection.Add(new MachineMaterialItemViewModel() { MachineMaterial = machineMachineMaterial });
                }
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
                               //Doo Stuff
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

                               foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
                               {
                                   //Update new % based on ratio
                                   machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity
                                       = machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity / 100 *
                                         newRemainder;
                               }
                               MachineMaterialItemListCollection.Add(new MachineMaterialItemViewModel()
                               {
                                   MachineMaterial = new MachineMaterial()
                                   {
                                       Name = $"Test {MachineMaterialItemListCollection.Count + 1}",
                                       CostPerWeightUnit = 4.0,
                                       PercentOfTotalMachineCapacity = (double)100 / (MachineMaterialItemListCollection.Count + 1),
                                       TotalMonthlyPurchasedWeight = 3
                                   }
                               });
                           }));
            }
        }

        public ObservableCollection<MachineMaterialItemViewModel> MachineMaterialItemListCollection { get; set; } = new ObservableCollection<MachineMaterialItemViewModel>();
    }
}
