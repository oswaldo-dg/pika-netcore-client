using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.NetCore.Client
{
    public enum UserType
    {
        Usuer = 0, Admin = 1
    }

    public class DomainAccess
    {
        public string Domain { get; set; }
        public string OrgUnit { get; set; }
        public string DomainId { get; set; }
        public string OrgUnitId { get; set; }
        public UserType Type { get; set; }
    }
}
