using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class LoggedInUserInfos
    {
        public long userType_eV_ID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Surname { get; set; }
        public string Fathername { get; set; }
        public string Email { get; set; }

        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
    }
}