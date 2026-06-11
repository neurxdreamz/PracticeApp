using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic
{
    public static class DataBaseConfig
    {
        public static string GetConnectionString()
        {
            return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=деталь (для практики).accdb;Persist Security Info=False;";
        }
    }
}
