using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class Individual
    {
        public string Username { get; set; }
        public string Surname { get; set; }
        public string Fathername { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public long Id { get; set; }
    }
}