using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class MachineMaterial : Material, IComparable
    {
        private double percentOfTotalMachineCapacity = 100;

        public double PercentOfTotalMachineCapacity
        {
            get => percentOfTotalMachineCapacity;
            set
            {
                percentOfTotalMachineCapacity = value;
                RaisePropertyChanged(nameof(PercentOfTotalMachineCapacity));
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is MachineMaterial machine)
            {
                return ((machine.Name == this.Name)
                        && (Math.Abs(machine.CostPerWeightUnit - this.CostPerWeightUnit) < 0)
                        && (Math.Abs(machine.TotalMonthlyPurchasedWeight - this.TotalMonthlyPurchasedWeight) < 0)
                        && (Math.Abs(machine.PercentOfTotalMachineCapacity - this.PercentOfTotalMachineCapacity) < 0)
                        ) ? 0 : -1;
            }
            else
            {
                return -1;
            }
        }
    }
}
