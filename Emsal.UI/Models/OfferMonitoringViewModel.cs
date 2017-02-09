using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emsal.WebInt.EmsalSrv;
using System.ComponentModel.DataAnnotations;
using Emsal.Utility.CustomObjects;
using System.Web.Mvc;

namespace Emsal.UI.Models
{
    public class OfferMonitoringViewModel : UserRoles
    {
        //public string messageSuccess = "Yadda saxlanıldı.";
        //public int fileSize = 2097152;
        //public string fileDirectory = @"C:\inetpub\emsalfiles";
        //public List<string> fileTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };

        public string messageSuccess = FileExtension.messageSuccess;
        public int fileSize = FileExtension.fileSize;
        public string fileDirectory = FileExtension.fileDirectoryExternal;
        public string fileDirectoryContract = FileExtension.fileDirectoryExternalContract;
        public List<string> fileTypes = FileExtension.fileMimeTypes;
        public string fileAcceptTypes = FileExtension.fileTypes;
        public string tempFileDirectory = HttpContext.Current.Server.MapPath(FileExtension.fileDirectoryTempFile);
        public string tempFileDirectoryFV = FileExtension.tempFileDirectoryFV;

        public List<string> fileTypesPDF = FileExtension.fileMimeTypesPDF;

        public tblUser User;

        public tblOffer_Production OfferProduction;
        public IList<tblOffer_Production> OfferProductionList { get; set; }
        public tblOffer_Production[] OfferProductionArray;

        public ProductCatalogDetail ProductCatalogDetail;
        public IList<ProductCatalogDetail> ProductCatalogDetailList { get; set; }
        public ProductCatalogDetail[] ProductCatalogDetailArray;

        public ProductionDetail ProductionDetail;
        public IList<ProductionDetail> ProductionDetailList { get; set; }
        public ProductionDetail[] ProductionDetailArray;

        public tblDemand_Production DemandProduction;
        public IList<tblDemand_Production> DemandProductionList { get; set; }
        public IList<tblDemand_Production> SelectedDemandProductionList { get; set; }
        public tblDemand_Production[] DemandProductionArray;


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

        public OfferProductionDetailSearch OfferProductionDetailSearch;

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

        public tblContract Contract;
        public IList<tblContract> ContractList { get; set; }
        public tblContract[] ContractArray;

        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;

        public tblProductAddress ProductAddress;
        public IList<tblProductAddress> ProductAddressList { get; set; }
        public tblProductAddress[] ProductAddressArray;

        public GetDemandProductionDetailistForEValueIdSearch GetDemandProductionDetailistForEValueIdSearch;
        public IList<GetDemandProductionDetailistForEValueIdSearch> GetDemandProductionDetailistForEValueIdSearchList { get; set; }
        public GetDemandProductionDetailistForEValueIdSearch[] GetDemandProductionDetailistForEValueIdSearchArray;

        public tblPerson Person;
        public IList<tblPerson> PersonList { get; set; }
        public tblPerson[] PersonArray;

        public PersonDetail PersonDetail;
        public IList<PersonDetail> PersonDetailList { get; set; }
        public PersonDetail[] PersonDetailArray;

        public tblForeign_Organization Foreign_Organization;
        public IList<tblForeign_Organization> Foreign_OrganizationList { get; set; }
        public tblForeign_Organization[] Foreign_OrganizationArray;

        public tblProduction_Calendar ProductionCalendar;
        public IList<tblProduction_Calendar> ProductionCalendarList { get; set; }
        public tblProduction_Calendar[] ProductionCalendarArray;

        public tblComMessage ComMessage;
        public IList<tblComMessage> ComMessageList { get; set; }
        public tblComMessage[] ComMessageArray;


        public tblConfirmationMessage ConfirmationMessage;
        public IList<tblConfirmationMessage> ConfirmationMessageList { get; set; }
        public tblConfirmationMessage[] ConfirmationMessageArray;

        public tblComMessageAttachment ComMessageAttachment;
        public IList<tblComMessageAttachment> ComMessageAttachmentList { get; set; }
        public tblComMessageAttachment[] ComMessageAttachmentArray;

        public DemanProductionGroup DemanProductionGroup { get; set; }
        public DemanProductionGroup[] DemanProductionGroupArray;
        public IList<DemanProductionGroup> DemanProductionGroupList { get; set; }

        public DemandOfferProductsSearch DemandOfferProductsSearch { get; set; }

        public PagedList.IPagedList<ProductionDetail> Paging { get; set; }
        public PagedList.IPagedList<tblPerson> PagingPerson { get; set; }
        public PagedList.IPagedList<long> PagingT { get; set; }

        public bool isPDF { get; set; }
        public long userId { get; set; }

        public string finVoen { get; set; }

        public long totalSize = 0;

        public string actionName { get; set; }

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

        [Display(Name = "Qiyməti (bir ölçü vahidinə düşən) (AZN-lə)")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        //[Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public string price { get; set; }

        [Display(Name = "Miqdarı (Həcmi)")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public string size { get; set; }

        public long productId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int[] enumCat { get; set; }

        public int[] enumVal { get; set; }

        [Display(Name = "Ünvan")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string descAddress { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası məcburidir.")]
        public int addressId { get; set; }

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
        public string monitoringStatusEV { get; set; }

        [Display(Name = "Səbəb")]
        public long monitoringStatusEVId { get; set; }
        public string productName { get; set; }
        public string userInfo { get; set; }
        public long userType { get; set; }
        public long itemCount = 0;
        public bool itemCountB = true;

        public string nameSurnameFathername { get; set; }
        public string icraci { get; set; }
        public string pin { get; set; }
        public IList<HttpPostedFileBase> attachfiles { get; set; }
        public string FCType { get; set; }
        public string fname { get; set; }
        public bool isApprov { get; set; }
        public bool isSeller { get; set; }
    }
}