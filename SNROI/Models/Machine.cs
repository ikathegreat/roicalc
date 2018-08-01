using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using SNROI.Enums;

namespace SNROI.Models
{
    public class Machine : ObservableObject, ICloneable
    {

        private string name = "Machine";
        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private MachineKind machineKind = MachineKind.Plasma;
        public MachineKind MachineKind
        {
            get { return machineKind; }
            set { Set(ref machineKind, value); }
        }

        private double costPerHourToRun = 1;
        public double CostPerHourToRun
        {
            get { return costPerHourToRun; }
            set { Set(ref costPerHourToRun, value); }
        }

        private double monthlyUtilizationPercentage = 50;
        public double MonthlyUtilizationPercentage
        {
            get { return monthlyUtilizationPercentage; }
            set { Set(ref monthlyUtilizationPercentage, value); }
        }

        private ObservableCollection<MachineMaterial> machineMaterials = new ObservableCollection<MachineMaterial>();
        public ObservableCollection<MachineMaterial> MachineMaterials
        {
            get { return machineMaterials; }
            set { Set(ref machineMaterials, value); }
        }

        public object Clone()
        {
            var machine = new Machine
            {
                Name = this.Name,
                MachineKind = this.MachineKind,
                CostPerHourToRun = this.CostPerHourToRun,
                MonthlyUtilizationPercentage = this.MonthlyUtilizationPercentage,
                MachineMaterials = this.MachineMaterials
            };
            return machine;
        }
    }
}
