using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class Material : BaseModel
    {
        public string Name { get; set; }

        public double CostPerWeightUnit { get; set; }
        public double TotalMonthlyPurchasedWeight { get; set; }
        public double TotalMonthlyPurchasedCost { get; set; }
    }
}
