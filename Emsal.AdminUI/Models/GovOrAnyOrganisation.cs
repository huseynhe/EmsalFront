using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class GovOrAnyOrganisation
    {
        public string UserName { get; set; }
        public string OrganisationName { get; set; }
        public string Email { get; set; }

        public long Id { get; set; }

        public string FullAddress { get; set; }

        public string ManagerName { get; set; }
        public string ManagerSurname { get; set; }
        public string ManagerFatherName { get; set; }
    }
}