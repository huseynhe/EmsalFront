using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class ProductCatalogViewModel : UserInfoViewModel
    {
        //public string messageSuccess = "Yadda saxlanıldı.";
        //public int fileSize = 2097152;
        //public string fileDirectorySV = @"/Content/profileImage/";
        //public string fileDirectory = HttpContext.Current.Server.MapPath("~/Content/profileImage/");
        //public List<string> fileTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };

        public string messageSuccess = FileExtension.messageSuccess;
        public int fileSize = FileExtension.fileSize;
        public string fileDirectorySV = FileExtension.fileDirectorySV;
        public string fileDirectory = HttpContext.Current.Server.MapPath(FileExtension.fileDirectoryProfileImage);
        public List<string> fileTypes = FileExtension.fileMimeTypes;

        public tblUser User;

        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public tblProductCatalog[] ProductCatalogArray;


        public tblEnumCategory EnumCategory;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public tblProduct_Document ProductDocument;
        public tblProduct_Document ProductDocumentFile;
        public IList<tblProduct_Document> ProductDocumentList { get; set; }
        public tblProduct_Document[] ProductDocumentArray;
        public tblProduct_Document[] ProductDocumentArrayFile;

        public tblEnumValue EnumValue;
        public tblEnumValue EnumValueFP;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public IList<tblEnumValue> EnumValueEDVList { get; set; }
        public tblEnumValue[] EnumValueArray;

        public tblProductCatalogControl ProductCatalogControl;
        public IList<tblProductCatalogControl> ProductCatalogControlList { get; set; }
        public tblProductCatalogControl[] ProductCatalogControlArray;

        [Display(Name = "Məhsulun adı")]
        [Required(ErrorMessage = "Məcburi xana.")]
        public string productName { get; set; }

        [Display(Name = "Aid olduğu məhsul")]
        [Required(ErrorMessage = "Məcburi xana.")]
        public string productParentName { get; set; }

        [Required(ErrorMessage = "Məcburi xana.")]
        [Display(Name = "Qeyd")]
        public string description { get; set; }


        [Display(Name = "Status")]
        public bool Status { get; set; }

    }
}