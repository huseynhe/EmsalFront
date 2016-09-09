using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class GovernmentOrganisationDemand
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ParentName { get; set; }
        public string ShipmentPlace { get; set; }
        public string ShipmentPeriod { get; set; }
        public double ProductQuantity { get; set; }
        public string QuantityType { get; set; }
        public double ProductTotalPrice { get; set; }
        public string ProductProfilePicture { get; set; }

        public DemandCalendar DemandCalendar { get; set; }
        public List<DemandCalendar> DemandCalendarList { get; set; }
    }
}