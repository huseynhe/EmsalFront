using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class MonthsModel : UserRoles
    {
        public int productionId { get; set; }
        public string monthName { get; set; }
        public Guid id = Guid.NewGuid();
    }
}