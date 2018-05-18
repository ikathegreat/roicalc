using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class HourlyPerson : BaseModel
    {
        public string Name { get; set; }
        public EmployeeKind EmployeeKind { get; set; }
        public double HourlyWage { get; set; }
    }
}
