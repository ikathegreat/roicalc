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
        private readonly ObservableCollection<Material> materialsList;

        public MachineSetupViewModel(ObservableCollection<Material> aMaterialList)
        {
            materialsList = aMaterialList;
            foreach (var material in materialsList)
            {
                AddMaterialToPossibleMaterialsToAddList(material);
            }
        }

        private void AddMaterialToPossibleMaterialsToAddList(Material material)
        {
            var machineSetupAddMaterialViewModel = new MachineSetupAddMaterialViewModel() { Material = material };
            machineSetupAddMaterialViewModel.AddNewMachineMaterialToList += MachineSetupAddMaterialViewModel_AddNewMachineMaterialToList;
            MachineSetupAddMaterialViewModelList.Add(machineSetupAddMaterialViewModel);
        }

        private void MachineSetupAddMaterialViewModel_AddNewMachineMaterialToList(Material material, MachineSetupAddMaterialViewModel machineSetupAddMaterialViewModel)
        {
            var newMachineMaterialItemViewModel = new MachineMaterialItemViewModel()
            {
                MachineMaterial = new MachineMaterial()
                {
                    Name = material.Name,
                    CostPerWeightUnit = material.CostPerWeightUnit,
                    PercentOfTotalMachineCapacity = 0,
                    TotalMonthlyPurchasedWeight = material.TotalMonthlyPurchasedWeight
                }
            };

            newMachineMaterialItemViewModel.RemoveMachineMaterial += MachineMaterialItemViewModel_RemoveMachineMaterial;
            newMachineMaterialItemViewModel.MachineMaterialChanged += MachineMaterialItemViewModel_MachineMaterialChanged;
            MachineSetupAddMaterialViewModelList.Remove(machineSetupAddMaterialViewModel); //Only can be added once
            MachineMaterialItemListCollection.Add(newMachineMaterialItemViewModel);
            RebalanceMaterialPercents();

            RaisePropertyChanged(nameof(IsAddNewMaterialEnabled));
        }

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


                    for (var i = 0; i < MachineSetupAddMaterialViewModelList.Count; i++)
                    {
                        var curMaterial = MachineSetupAddMaterialViewModelList[i].Material;
                        //Poor man's object equality
                        if ((curMaterial.Name != machineMachineMaterial.Name) ||
                            (Math.Abs(curMaterial.CostPerWeightUnit - machineMachineMaterial.CostPerWeightUnit) > 0.001) ||
                            (Math.Abs(curMaterial.TotalMonthlyPurchasedWeight - machineMachineMaterial.TotalMonthlyPurchasedWeight) > 0.001))
                            continue;

                        MachineSetupAddMaterialViewModelList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void MachineMaterialItemViewModel_RemoveMachineMaterial(MachineMaterial machineMaterial)
        {
            if (!DialogService.Instance.ShowMessageQuestion($"Remove {machineMaterial.Name}?", "Remove Material"))
                return;
            MachineMaterialItemListCollection.Remove(MachineMaterialItemListCollection.Single(s => s.MachineMaterial == machineMaterial));

            AddMaterialToPossibleMaterialsToAddList(machineMaterial);
            RaisePropertyChanged(nameof(IsAddNewMaterialEnabled));

            RebalanceMaterialPercents();
        }

        private void RebalanceMaterialPercents()
        {
            if (MachineMaterialItemListCollection.Count == 1)
            {
                var machineMaterialItemViewModel = MachineMaterialItemListCollection.FirstOrDefault();
                machineMaterialItemViewModel.PercentOfTotalMachineCapacity = 100;
                machineMaterialItemViewModel.RaisePropertyChanged(nameof(machineMaterialItemViewModel.PercentOfTotalMachineCapacity));
                return;
            }
            foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
            {
                machineMaterialItemViewModel.PercentOfTotalMachineCapacity
                    = (double)100 / (MachineMaterialItemListCollection.Count);
                machineMaterialItemViewModel.RaisePropertyChanged(nameof(machineMaterialItemViewModel.PercentOfTotalMachineCapacity));
            }
        }

        private void MachineMaterialItemViewModel_MachineMaterialChanged(MachineMaterial machineMaterial)
        {
            if (MachineMaterialItemListCollection.Count == 1)
            {
                MachineMaterialItemListCollection.FirstOrDefault().UpdatePercentOfTotalMachineCapacity(100);
            }

            //get new remainder
            var newRemainder = 100 - machineMaterial.PercentOfTotalMachineCapacity;

            foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
            {

                if (MachineMaterialItemListCollection.Count < 2)
                    continue;

                if (machineMaterialItemViewModel.MachineMaterial.Equals(machineMaterial))
                {
                    machineMaterialItemViewModel.RaisePropertyChanged(
                        nameof(machineMaterialItemViewModel.PercentOfTotalMachineCapacity));
                    continue;
                }

                if (Math.Abs(newRemainder) < 0.001)
                {
                    machineMaterialItemViewModel.UpdatePercentOfTotalMachineCapacity(0);
                }
                else
                {
                    //Update new % based on ratio
                    machineMaterialItemViewModel.UpdatePercentOfTotalMachineCapacity(newRemainder / (MachineMaterialItemListCollection.Count - 1));
                }

                //machineMaterialItemViewModel.PercentOfTotalMachineCapacity
                //    = newRemainder / (MachineMaterialItemListCollection.Count - 1);

                machineMaterialItemViewModel.RaisePropertyChanged(
                    nameof(machineMaterialItemViewModel.PercentOfTotalMachineCapacity));
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

        public bool IsAddNewMaterialEnabled => MachineSetupAddMaterialViewModelList.Count > 0;

        public ObservableCollection<MachineSetupAddMaterialViewModel> MachineSetupAddMaterialViewModelList { get; set; } = new ObservableCollection<MachineSetupAddMaterialViewModel>();

        //private RelayCommand addMachineCommand;


        //public RelayCommand AddTestMaterialCommand
        //{
        //    get
        //    {
        //        return addMachineCommand
        //               ?? (addMachineCommand = new RelayCommand(
        //                   () =>
        //                   {
        //                       //get new remainder
        //                       var newRemainder = 100 - ((double)100 / (MachineMaterialItemListCollection.Count + 1));
        //                       var isCurrentPercentSumEqualTo100 =
        //                           Math.Abs(MachineMaterialItemListCollection.Sum(x =>
        //                                        x.MachineMaterial.PercentOfTotalMachineCapacity) - 100) < 1;
        //                       if (isCurrentPercentSumEqualTo100)
        //                       {
        //                           foreach (var machineMaterialItemViewModel in MachineMaterialItemListCollection)
        //                           {
        //                               //Update new % based on ratio
        //                               machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity
        //                                   = machineMaterialItemViewModel.MachineMaterial.PercentOfTotalMachineCapacity / 100 *
        //                                     newRemainder;
        //                           }
        //                       }
        //                       else
        //                       {

        //                       }

        //                       var newMachineMaterialItemViewModel = new MachineMaterialItemViewModel()
        //                       {
        //                           MachineMaterial = new MachineMaterial()
        //                           {
        //                               Name = $"Test {MachineMaterialItemListCollection.Count + 1}",
        //                               CostPerWeightUnit = 4.0,
        //                               PercentOfTotalMachineCapacity = isCurrentPercentSumEqualTo100 ? (double)100 / (MachineMaterialItemListCollection.Count + 1)
        //                                   : 100 - MachineMaterialItemListCollection.Sum(x => x.MachineMaterial.PercentOfTotalMachineCapacity),
        //                               TotalMonthlyPurchasedWeight = 3
        //                           }
        //                       };
        //                       newMachineMaterialItemViewModel.RemoveMachineMaterial +=
        //                           MachineMaterialItemViewModel_RemoveMachineMaterial;
        //                       MachineMaterialItemListCollection.Add(newMachineMaterialItemViewModel);
        //                   }));
        //    }
        //}

        public ObservableCollection<MachineMaterialItemViewModel> MachineMaterialItemListCollection { get; set; } = new ObservableCollection<MachineMaterialItemViewModel>();
    }
}
