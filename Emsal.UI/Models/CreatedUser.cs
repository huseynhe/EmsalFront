using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class CreatedUser : UserRoles
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string FatherName { get; set; }
        public string Role { get; set; }

        public string UserType { get; set; }
    }
}