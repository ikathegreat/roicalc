using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using SNROI.Enums;

namespace SNROI.Models
{
    public class Machine : ObservableObject, ICloneable
    {

        public string Name { get; set; } = "Machine";

        /// <summary>
        /// Type of machine 
        /// </summary>
        public MachineKind MachineKind { get; set; } = MachineKind.Plasma;

        /// <summary>
        /// Monetary cost per hour to run the machine
        /// </summary>
        public double CostPerHourToRun { get; set; } = 1;

        /// <summary>
        /// Percent of uptime per month
        /// </summary>
        public double MonthlyUtilizationPercentage { get; set; } = 50;

        public ObservableCollection<MachineMaterial> MachineMaterials { get; set; } = new ObservableCollection<MachineMaterial>();

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
