using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class ContactViewModel : UserRoles
    {
        [Display(Name = "Ad, soyad, ata adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string nameSurnameFathername { get; set; }


        [Display(Name = "E-poçt")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string mail { get; set; }

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string phone { get; set; }

        [Display(Name = "Müraciətin məzmunu")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string appealBody { get; set; }
    }
}
