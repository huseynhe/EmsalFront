using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class DemandCalendar
    {
        public string day { get; set; }
        public string month { get; set; }
        public string ocklock { get; set; }
        public string quantity { get; set; }

        public string price { get; set; }

        public string shipmetType { get; set; }
    }
}