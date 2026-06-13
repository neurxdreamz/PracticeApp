using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Entities
{
    public class Worker
    {
        public int IdWorker { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }
        public int Grade { get; set; }
        public decimal TariffRate { get; set; }
    }
}
