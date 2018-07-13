using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class MachineMaterial : Material
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
    }
}
