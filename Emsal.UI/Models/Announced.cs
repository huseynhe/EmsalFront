using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class Announced : UserRoles
    {
        public long anId { get; set; }
        public long anPId { get; set; }
        public string anProductName { get; set; }
        public string anName { get; set; }
        public string anDescription { get; set; }
        public string anDate { get; set; }
        public decimal anSize { get; set; }
        public string anMeasurement { get; set; }
        public string Value { get; set; }
    }
}