using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class EnumValueViewModel : UserInfoViewModel
    {
        public tblEnumValue EnumValue;
        public IList<tblEnumValue> EnumValueList { get; set; }   
        public tblEnumValue[] EnumValueArray;

        public PagedList.IPagedList<tblEnumValue> Paging
        {
            get;
            set;
        }


        public tblEnumCategory EnumCategory;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public long Id { get; set; }
        public int enumCategoryId { get; set; }
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Məcburi xana.")]

        public string name { get; set; }
        [Required(ErrorMessage = "Məcburi xana.")]
        [Display(Name = "Qeyd")]
        public string description { get; set; }
        [Display(Name = "Status")]
        public bool Status { get; set; }
    }
}