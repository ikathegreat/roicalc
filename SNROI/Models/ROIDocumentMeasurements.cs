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
            MaterialMinutesSavedPerUnit = 1;
            MaterialTimeUnits = TimeUnitsEnums.Day;
            MachineMinutesSavedPerUnit = 1;
            MachineTimeUnits = TimeUnitsEnums.Day;
            ProgrammingMinutesSavedPerUnit = 1;
            ProgrammingTimeUnits = TimeUnitsEnums.Day;

        }

        public double MaterialMinutesSavedPerUnit { get; set; }
        public double MachineMinutesSavedPerUnit { get; set; }
        public double ProgrammingMinutesSavedPerUnit { get; set; }
        public TimeUnitsEnums MaterialTimeUnits { get; set; }
        public TimeUnitsEnums MachineTimeUnits { get; set; }
        public TimeUnitsEnums ProgrammingTimeUnits { get; set; }
    }
}
