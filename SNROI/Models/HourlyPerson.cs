using GalaSoft.MvvmLight;
using SNROI.Enums;

namespace SNROI.Models
{
    public class HourlyPerson : ObservableObject
    {
        public string Name { get; set; }

        /// <summary>
        /// Type of employee, role, position
        /// </summary>
        public EmployeeKind EmployeeKind { get; set; } = EmployeeKind.Programmer;

        /// <summary>
        /// Monetary cost per hour to employ this person
        /// </summary>
        public double HourlyWage { get; set; } = 1.00;
    }
}
