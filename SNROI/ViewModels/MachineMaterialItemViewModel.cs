using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
