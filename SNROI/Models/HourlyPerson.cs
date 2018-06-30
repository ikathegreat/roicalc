using GalaSoft.MvvmLight;

namespace SNROI.Models
{
    public enum EmployeeKind
    {
        Programmer,
        Operator,
        Sales,
        Quoting,
        Manager,
        Shipping,
        Other
    }
    public class HourlyPerson : ObservableObject
    {
        public string Name { get; set; }
        public EmployeeKind EmployeeKind { get; set; }
        public double HourlyWage { get; set; }
    }
}
