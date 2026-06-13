using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Entities
{
    public class Detail
    {
        
        public int IdRecord { get; set; }
        public string DetailName { get; set; }
        public int BatchVolume { get; set; }

      
        public int SectorId { get; set; }
        public int WorkerId { get; set; }
        public int ShiftId { get; set; }

        
        public string SectorName { get; set; }
        public string WorkerFullName { get; set; }
        public int ShiftNumber { get; set; }
    }
}
