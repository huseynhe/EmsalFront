using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class SpecialSummaryPotentialAndOffer
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ParentName { get; set; }
        public long ProductEndDate { get; set; }
        public double ProductQuantity { get; set; }
        public string QuantityType { get; set; }
        public double ProductTotalPrice { get; set; }
        public string ProductProfilePicture { get; set; }
    }
}