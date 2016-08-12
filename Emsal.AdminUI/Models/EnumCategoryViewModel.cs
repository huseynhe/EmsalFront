using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class EnumCategoryViewModel : UserInfoViewModel
    {
        public tblEnumCategory EnumCategory;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }

   
        public tblEnumCategory[] EnumCategoryArray;

        public PagedList.IPagedList<tblEnumCategory> Paging
        {
            get;
            set;
        }


        public long Id { get; set; }
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Məcburi xana.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Məcburi xana.")]
        [Display(Name = "Qeyd")]
        public string description { get; set; }
        [Display(Name = "Status")]
        public bool Status { get; set; }
        [Display(Name = "Produkt üçün")]
        public bool isProductDescibe { get; set; }


        public long LastUpdatedStatus { get; set; }
        public string createdUser { get; set; }
        public long createdDate { get; set; }
        public string updatedUser { get; set; }
        public long updatedDate { get; set; }
    }
}