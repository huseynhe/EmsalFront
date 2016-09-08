using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class PotentialClientViewModel : UserRoles
    {
        public string messageSuccess = "Yadda saxlanıldı.";
        public int fileSize = 2097152;
        public string fileDirectory = @"D:\fls";
        public List<string> fileTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };

        public long arrNum = 0;
        public long arrNumFU = 0;
        public long arrPNum = 0;

        public tblUser User;
        public tblForeign_Organization ForeignOrganization;
        public tblUserRole tblUserRole;

        public tblPotential_Production PotentialProduction;
        public IList<tblPotential_Production> PotentialProductionList { get; set; }
        public IList<tblPotential_Production> SelectedPotentialProductionList { get; set; }
        public tblPotential_Production[] PotentialProductionArray;

        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public IList<tblProductCatalog> ProductCatalogListPC { get; set; }
        public IList<tblProductCatalog> ProductCatalogListFE { get; set; }
        public IList<tblProductCatalog>[] ProductCatalogListFEA { get; set; }
        public tblProductCatalog[] ProductCatalogArrayFE;
        public tblProductCatalog[] ProductCatalogArray;
        public tblProductCatalog[] ProductCatalogArrayPC;

        public tblEnumCategory EnumCategory;
        public tblEnumCategory EnumCategorySS;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;

        public tblEnumValue EnumValue;
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

        public tblAddress Address;
        public IList<tblAddress> AddressList { get; set; }
        public tblAddress[] AddressArray;

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
        public IList<tblProductAddress> ProductAddressListFE { get; set; }
        public IList<tblProductAddress>[] ProductAddressListFEA { get; set; }
        public tblProductAddress[] ProductAddressArray;


        public tblProduction_Calendar ProductionCalendar;
        public IList<tblProduction_Calendar> ProductionCalendarList { get; set; }
        public tblProduction_Calendar[] ProductionCalendarArray;

        public ProductionDetail ProductionDetail;
        public IList<ProductionDetail> ProductionDetailList { get; set; }
        public ProductionDetail[] ProductionDetailArray;

        public SelectedProduct SelectedProduct;
        public IList<SelectedProduct> SelectedProductList { get; set; }
        public SelectedProduct[] SelectedProductArray;



        public tblPerson Person;
        public tblPerson Personr;
        public IList<tblPerson> PersonList { get; set; }
        public tblPerson[] PersonArray;

        public string[] selectedMonth { get; set; }

        public bool isPDF { get; set; }
        public long userId { get; set; }

        public long totalSize = 0;

        public long Id { get; set; }
        public long productionCalendarId { get; set; }
        public long productAddressId { get; set; }
        public long[] productAddressIds { get; set; }

        public long[] productionControlEVIds { get; set; }

        [Display(Name = "Əlavə qeyd")]
        //[Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string description { get; set; }

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
        public long[] productIds { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int[] enumCat { get; set; }
        public long[] pcId { get; set; }

        public int[] enumVal { get; set; }
        public int[] adId { get; set; }
        public int[] prodId { get; set; }

        [Display(Name = "Ünvan")]
        //[Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string descAddress { get; set; }

        [Display(Name = "Ünvan")]
        //[Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string descAddressFU { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası məcburidir.")]
        public int addressId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası məcburidir.")]
        public int addressIdFU { get; set; }

        [Display(Name = "Sənəd növləri")]
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası minimum {1} olmalıdır.")]
        public int documentTypes { get; set; }

        [Display(Name = "Daşınma qrafiki")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public long shippingSchedule { get; set; } 
        [Display(Name = "Ay")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string checkedMonth { get; set; }

        [Display(Name = "Potensial istehsal məhsullarını təsdiqlə")]
        public bool confirmList { get; set; }

        [Display(Name = "Başlama tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime? startDate { get; set; }


        [Display(Name = "Bitmə tarixi")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public virtual DateTime? endDate { get; set; }




        [Display(Name = "FİN")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string FIN { get; set; }

        [Display(Name = "Fiziki şəxsin adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string name { get; set; }

        [Display(Name = "Soyadı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string surname { get; set; }

        [Display(Name = "Ata adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string fathername { get; set; }





        [Display(Name = "VÖEN")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string VOEN { get; set; }


        [Display(Name = "Hüquqi şəxsin adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string legalLame { get; set; }


        [Display(Name = "Hüquqi şəxsin rəhbərinin adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string legalPName { get; set; }

        [Display(Name = "Soyadı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string legalPSurname { get; set; }

        [Display(Name = "Ata adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string legalPFathername { get; set; }
    }
}
