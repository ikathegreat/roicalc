using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public enum MachineKind
    {
        Laser,
        Plasma,
        Waterjet,
        Punch,
        Combo,
        Router,
        Knife,
        Other
    }
    public class Machine : BaseModel
    {

        public string Name { get; set; }
        public MachineKind MachineKind { get; set; }
        public string CostPerHourToRun { get; set; }
    }
}
