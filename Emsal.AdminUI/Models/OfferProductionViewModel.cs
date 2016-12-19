using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class OfferProductionViewModel : UserInfoViewModel
    {
        //public string messageSuccess = "Yadda saxlanıldı.";
        //public int fileSize = 2097152;
        //public string fileDirectory = @"C:\inetpub\emsalfiles";
        //public List<string> fileTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };

        public string messageSuccess = FileExtension.messageSuccess;
        public int fileSize = FileExtension.fileSize;
        public string fileDirectory = FileExtension.fileDirectoryExternal;
        public List<string> fileTypes = FileExtension.fileMimeTypes;


        public tblOffer_Production OfferProduction;
        public IList<tblOffer_Production> OfferProductionList { get; set; }
        public tblOffer_Production[] OfferProductionArray;

        public ProductionDetail ProductionDetail;
        public IList<ProductionDetail> ProductionDetailList { get; set; }
        public ProductionDetail[] ProductionDetailArray;

        public OfferProductionDetail OfferProductionDetail;
        public IList<OfferProductionDetail> OfferProductionDetailList { get; set; }
        public OfferProductionDetail[] OfferProductionDetailArray;

        public tblDemand_Production DemandProduction;
        public IList<tblDemand_Production> DemandProductionList { get; set; }
        public IList<tblDemand_Production> SelectedDemandProductionList { get; set; }
        public tblDemand_Production[] DemandProductionArray;

        public ProductCatalogDetail[] ProductCatalogDetailArray;
        public IList<ProductCatalogDetail> ProductCatalogDetailList { get; set; }

        public tblPotential_Production PotentialProduction;
        public IList<tblPotential_Production> PotentialProductionList { get; set; }
        public IList<tblPotential_Production> SelectedPotentialProductionList { get; set; }
        public tblPotential_Production[] PotentialProductionArray;

        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public tblProductCatalog[] ProductCatalogArray;

        public tblEnumCategory EnumCategory;
        public tblEnumCategory EnumCategorySS;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public tblEnumValue EnumValue;
        public tblEnumValue EnumValueST;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public tblEnumValue[] EnumValueArray;
        public IList<tblEnumValue> EnumValueMonthList { get; set; }
        public IList<tblEnumValue> EnumValueShippingScheduleList { get; set; }
        public IList<tblEnumValue> EnumValueDocumentTypeList { get; set; }

        public tblProductCatalogControl ProductCatalogControl;
        public IList<tblProductCatalogControl> ProductCatalogControlList { get; set; }
        public IList<tblProductCatalogControl> ProductCatalogControlDocumentTypeList { get; set; }
        public tblProductCatalogControl[] ProductCatalogControlArray;

        public tblProductionControl ProductionControl;
        public IList<tblProductionControl> ProductionControlList { get; set; }
        public tblProductionControl[] ProductionControlArray;

        public tblProduction_Document ProductionDocument;
        public IList<tblProduction_Document> ProductionDocumentList { get; set; }
        public tblProduction_Document[] ProductionDocumentArray;

        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;

        public tblProductAddress ProductAddress;
        public IList<tblProductAddress> ProductAddressList { get; set; }
        public tblProductAddress[] ProductAddressArray;


        public tblProduction_Calendar ProductionCalendar;
        public IList<tblProduction_Calendar> ProductionCalendarList { get; set; }
        public tblProduction_Calendar[] ProductionCalendarArray;

        public tblComMessage ComMessage;
        public IList<tblComMessage> ComMessageList { get; set; }
        public tblComMessage[] ComMessageArray;


        public tblConfirmationMessage ConfirmationMessage;
        public IList<tblConfirmationMessage> ConfirmationMessageList { get; set; }
        public tblConfirmationMessage[] ConfirmationMessageArray;

        public OfferProductionExcell OfferProductionExcell;
        public IList<OfferProductionExcell> OfferProductionExcellList { get; set; }

        public PagedList.IPagedList<OfferProductionDetail> OfferPaging { get; set; }
        public PagedList.IPagedList<ProductionDetail> Paging { get; set; }
        public PagedList.IPagedList<long> PagingT { get; set; }

        public bool isPDF { get; set; }
        public long userId { get; set; }

        public long itemCount = 0;
        public bool itemCountB = true;

        public string[] auArrName{ get; set; }

        public long totalSize = 0;


        public long Id { get; set; }

        [Display(Name = "İmtinanın səbəbi")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string title { get; set; }

        [Display(Name = "Əlavə qeyd")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string description { get; set; }


        [Display(Name = "Bildiriş")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string message { get; set; }

        [Display(Name = "Qiyməti (AZN-lə)")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        //[Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public string price { get; set; }

        [Display(Name = "Miqdarı (Həcmi)")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public string size { get; set; }

        public long productId { get; set; }

        public long userType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int[] enumCat { get; set; }

        public int[] enumVal { get; set; }

        [Display(Name = "Ünvan")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string descAddress { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası məcburidir.")]
        public long addressId { get; set; }

        public string startDate { get; set; }
        public string endDate { get; set; }
        public string forma { get; set; }
        public string eheader { get; set; }

        [Display(Name = "Sənəd növləri")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int documentTypes { get; set; }

        [Display(Name = "Çatdırılma qrafiki")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public long shippingSchedule { get; set; }
        public int[] checkedMonth { get; set; }

        [Display(Name = "Seçilmiş məhsulların imzalanmış faylını əlavə et")]
        public bool confirmList { get; set; }


        public int isMain { get; set; }
        public string statusEV { get; set; }
        public string productName { get; set; }
        public string fullAddress { get; set; }
        public string userInfo { get; set; }

        public decimal currentPagePrice { get; set; }
        public decimal allPagePrice { get; set; }
        public string actionName { get; set; }
    }

    public class OfferProductionExcell
    {
        public string productName { get; set; }
           public string typeDescription { get; set; }
           public string quantity { get; set; }
           public string unitPrice { get; set; }
           public string fullAddress { get; set; }
           public string personNameAddress { get; set; }
    }
}
