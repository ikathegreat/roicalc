using GalaSoft.MvvmLight;

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
    public class Machine : ObservableObject
    {

        public string Name { get; set; }
        public MachineKind MachineKind { get; set; }
        public string CostPerHourToRun { get; set; }
    }
}
