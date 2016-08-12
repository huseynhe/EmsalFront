using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class SelectedProduct : UserRoles
    {
        public long Id { get; set; }
        public string productName { get; set; }
        public string price { get; set; }
        public string size { get; set; }
        public string measurement { get; set; }
        public string qrafik { get; set; }
        public string month { get; set; }
        public string region { get; set; }
        public string note { get; set; }
    }
}