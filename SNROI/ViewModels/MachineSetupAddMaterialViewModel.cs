using GalaSoft.MvvmLight.CommandWpf;
using SNROI.Models;

namespace SNROI.ViewModels
{
    public class MachineSetupAddMaterialViewModel
    {
        public Material Material { get; set; }

        private RelayCommand addNewMaterialCommand;

        public RelayCommand AddNewMaterialCommand => 
            addNewMaterialCommand ?? (addNewMaterialCommand = new RelayCommand(FireAddNewMachineMaterialToList));

        public delegate void AddNewMachineMaterialToListEvent(Material material, MachineSetupAddMaterialViewModel machineSetupAddMaterialViewModel);
        public event AddNewMachineMaterialToListEvent AddNewMachineMaterialToList;
        public void FireAddNewMachineMaterialToList()
        {
            AddNewMachineMaterialToList?.Invoke(Material, this);
        }

    }
}
