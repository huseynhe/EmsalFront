using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class ProductPriceViewModel : UserInfoViewModel
    {
        public tblProductPrice ProductPrice;
        public tblProductPrice ProductPriceOUT;
        public tblProductPrice[] ProductPriceArray;
        public List<tblProductPrice> ProductPriceList { get; set; }


        public PagedList.IPagedList<ProductPriceDetail> Paging { get; set; }

        public tblProductCatalog ProductCatalog;
        public tblProductCatalog[] ProductCatalogArray;
        public List<tblProductCatalog> ProductCatalogList { get; set; }

        public ProductPriceDetail ProductPriceDetail;
        public ProductPriceDetail[] ProductPriceDetailArray;
        public ProductPriceDetail[] ProductPriceDetailArrayNP;
        public List<ProductPriceDetail> ProductPriceDetailList { get; set; }
        public PagedList.IPagedList<ProductPriceDetail> ProductPriceDetailListPaging { get; set; }

        public tblEnumCategory EnumCategoryYear;
        public tblEnumCategory EnumCategoryRub;
        public tblEnumCategory EnumCategory;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public tblEnumValue EnumValue;
        public tblEnumValue EnumValueYear;
        public tblEnumValue EnumValueRub;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public IList<tblEnumValue> EnumValueListYear { get; set; }
        public IList<tblEnumValue> EnumValueListRub { get; set; }
        public tblEnumValue[] EnumValueArray;
        public tblEnumValue[] EnumValueArrayYear;
        public tblEnumValue[] EnumValueArrayRub;

        public int approv { get; set; }
        [Display(Name = "İl")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public long year { get; set; }
        [Display(Name = "Rüb")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public long rub { get; set; }

        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string tst { get; set; }

        [Display(Name = "Qiymət")]
        public string[] price { get; set; }

        public long[] prodId { get; set; }


        [Display(Name = "Məhsul")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string product { get; set; }
        [Display(Name = "Qiyməti")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string unitPrice { get; set; }

        public int NPCount { get; set; }
        public string pname { get; set; }
    }
}