using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class CultureCurrencyPair : BaseModel
    {
        public string Country { get; set; }
        public string CultureCode { get; set; }
    }
}
