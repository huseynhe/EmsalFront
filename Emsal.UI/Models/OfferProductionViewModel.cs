using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class OfferProductionViewModel : UserRoles
    {
        public string messageSuccess = "Yadda saxlanıldı.";
        public int fileSize = 2097152;
        public string fileDirectory = @"D:\fls";
        public List<string> fileTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };

        public int arrNum = 0;
        public int arrPNum = 0;
        public string fullAddressId = "";

        public tblUser User;
        public tblPerson Person;

        public tblUserRole UserRole;
        public tblUserRole[] UserRoleArray;

        public tblOffer_Production OfferProduction;
        public IList<tblOffer_Production> OfferProductionList { get; set; }
        public tblOffer_Production[] OfferProductionArray;

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
        public IList<tblProductCatalog> ProductCatalogListPC { get; set; }
        public tblProductCatalog[] ProductCatalogArray;
        public tblProductCatalog[] ProductCatalogArrayPC;
        public IList<tblProductCatalog>[] ProductCatalogListFEA { get; set; }

        public tblEnumCategory EnumCategory;
        public tblEnumCategory EnumCategorySS;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public tblEnumValue EnumValue;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public tblEnumValue[] EnumValueArray;
        public IList<tblEnumValue> EnumValueMonthList { get; set; }
        public IList<tblEnumValue> EnumValueMonthListFV { get; set; }
        public IList<tblEnumValue> EnumValueMonthListFVL { get; set; }
        public IList<tblEnumValue> EnumValueMonthListFVE { get; set; }
        public IList<tblEnumValue> EnumValueShippingScheduleList { get; set; }
        public IList<tblEnumValue> EnumValueDocumentTypeList { get; set; }

        public tblProductCatalogControl ProductCatalogControl;
        public IList<tblProductCatalogControl> ProductCatalogControlList { get; set; }
        public IList<tblProductCatalogControl> ProductCatalogControlDocumentTypeList { get; set; }
        public tblProductCatalogControl[] ProductCatalogControlArray;

        public tblProductionControl ProductionControl;
        public IList<tblProductionControl> ProductionControlList { get; set; }
        public tblProductionControl[] ProductionControlArray;


        public tblProductionCalendar LProductionCalendar;
        public IList<tblProductionCalendar> LProductionCalendarList { get; set; }
        public tblProductionCalendar[] LProductionCalendarArray;

        public ProductionCalendarDetail LProductionCalendarDetail;
        public IList<ProductionCalendarDetail> LProductionCalendarDetailList { get; set; }
        public ProductionCalendarDetail[] LProductionCalendarDetailArray;

        public tblProduction_Document ProductionDocument;
        public IList<tblProduction_Document> ProductionDocumentList { get; set; }
        public tblProduction_Document[] ProductionDocumentArray;

        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public IList<tblPRM_AdminUnit> PRMAdminUnitListFA { get; set; }
        public IList<tblPRM_AdminUnit>[] PRMAdminUnitArrayFA { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;

        public tblProductAddress ProductAddress;
        public IList<tblProductAddress> ProductAddressList { get; set; }
        public tblProductAddress[] ProductAddressArray;


        public tblProduction_Calendar ProductionCalendar;
        public IList<tblProduction_Calendar> ProductionCalendarList { get; set; }
        public tblProduction_Calendar[] ProductionCalendarArray;

        public SelectedProduct SelectedProduct;
        public IList<SelectedProduct> SelectedProductList { get; set; }
        public SelectedProduct[] SelectedProductArray;

        public bool isPDF { get; set; }
        public long userId { get; set; }
        public long ppId { get; set; }

        public long totalSize = 0;

        public long Id { get; set; }
        public long opId { get; set; }
        public long productionCalendarId { get; set; }
        public long productAddressId { get; set; }
        public long[] productAddressIds { get; set; }


        //[Display(Name = "Başlıq")]
        //[Required(ErrorMessage = "{0} xanası məcburidir.")]
        //public string title { get; set; }

        [Display(Name = "Əlavə qeyd")]
        //[Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string description { get; set; }

        [Display(Name = "Qiyməti (bir ölçü vahidinə düşən) (AZN-lə)")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        //[Range(typeof(decimal), "1", "79228162514264337593543950335")]
        public string[] price { get; set; }

        [Display(Name = "Miqdarı (Həcmi)")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public string[] size { get; set; }

public string pprice { get; set; }
public string psize { get; set; }

public int productId { get; set; }
        public long[] productIds { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int[] enumCat { get; set; }

        public int[] enumVal { get; set; }
        public int[] hour { get; set; }
        public int[] day { get; set; }


        public long[] pcId { get; set; }

        [Display(Name = "Ünvan")]
        //[Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string descAddress { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası məcburidir.")]
        public int addressId { get; set; }

        [Display(Name = "Sənəd növləri")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int documentTypes { get; set; }

        [Display(Name = "Daşınma qrafiki")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public long shippingSchedule { get; set; }
        [Display(Name = "Ay")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string checkedMonth { get; set; }

        [Display(Name = "Seçilmiş məhsulların imzalanmış faylını əlavə et")]
        public bool confirmList { get; set; }

         [Display(Name = "Başlama tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime? startDate { get; set; }


         [Display(Name = "Bitmə tarixi")]
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
         [Required(ErrorMessage = "{0} xanası məcburidir.")]
         public virtual DateTime? endDate { get; set; }


        [Display(Name = "Başlama tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual int startDateYear { get; set; }

        [Display(Name = "Başlama tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual string startDateMonth { get; set; }


        [Display(Name = "Bitmə tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual int endDateYear { get; set; }

        [Display(Name = "Bitmə tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual string endDateMonth { get; set; }


        public string ProductName { get; set; }
        [Display(Name = "Məhsulun Miqdarı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string Quantity { get; set; }
        public long QuantityType { get; set; }
        public string Title { get; set; }

        public long ProductType { get; set; }

        public string Address { get; set; }
        public string QuantityTypeStr { get; set; }

        public int[] month { get; set; }
        public int[] year { get; set; }
        public int[] howMany { get; set; }

        public long[] productionControlEVIds { get; set; }


        public string ProductParentName { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string UnitPrice { get; set; }
    }
}
