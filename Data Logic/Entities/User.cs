using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Logic.Entities
{
    public class User
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string UserName { get; set; }

        public Role RoleName { get; set; }
    }
}
