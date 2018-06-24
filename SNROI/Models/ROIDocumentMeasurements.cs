using SNROI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class ROIDocumentMeasurements
    {
        public ROIDocumentMeasurements()
        {
            ProgrammingMinutesSavedPerUnit = 1;
            TimeUnitsEnums = TimeUnitsEnums.Day;

        }

        public int ProgrammingMinutesSavedPerUnit { get; set; }
        public TimeUnitsEnums TimeUnitsEnums { get; set; }
    }
}
