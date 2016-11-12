using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class ProductCatalogViewModel
    {
        public string prodAltName;
        public string prodName;


        public tblUser User;
        public tblPerson Person;

        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public IList<tblProductCatalog> ProductCatalogListPC { get; set; }
        public tblProductCatalog[] ProductCatalogArray;
        public tblProductCatalog[] ProductCatalogArrayPC;

        public ProductionCalendarDetail LProductionCalendarDetail;
        public IList<ProductionCalendarDetail> LProductionCalendarDetailList { get; set; }
        public ProductionCalendarDetail[] LProductionCalendarDetailArray;

        public tblAnnouncement Announcement;
        public IList<tblAnnouncement> AnnouncementList { get; set; }
        public tblAnnouncement[] AnnouncementArray;

        public UserInfo UserInfo;
        public UserInfo UserInfoS;
        public IList<UserInfo> UserInfoList { get; set; }
        public IList<UserInfo> UserInfoListS { get; set; }
        public IList<UserInfo> UserInfoListP { get; set; }
        public UserInfo[] UserInfoArray;

        public tblProductCatalogControl ProductCatalogControl;
        public IList<tblProductCatalogControl> ProductCatalogControlList { get; set; }
        public tblProductCatalogControl[] ProductCatalogControlArray;

        public tblOffer_Production OfferProduction;
        public IList<tblOffer_Production> OfferProductionList { get; set; }
        public tblOffer_Production[] OfferProductionArray;

        public tblDemand_Production DemandProduction;
        public IList<tblDemand_Production> DemandProductionList { get; set; }
        public IList<tblDemand_Production> SelectedDemandProductionList { get; set; }
        public tblDemand_Production[] DemandProductionArray;

        public AnnouncementDetail AnnouncementDetail;
        public IList<AnnouncementDetail> AnnouncementDetailList { get; set; }
        public AnnouncementDetail[] AnnouncementDetailArray;

        public ProductionDetail ProductionDetail;
        public IList<ProductionDetail> ProductionDetailList { get; set; }
        public ProductionDetail[] ProductionDetailArray;

        public tblPotential_Production PotentialProduction;
        public IList<tblPotential_Production> PotentialProductionList { get; set; }
        public IList<tblPotential_Production> SelectedPotentialProductionList { get; set; }
        public tblPotential_Production[] PotentialProductionArray;

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

        public tblProductionControl ProductionControl;
        public IList<tblProductionControl> ProductionControlList { get; set; }
        public tblProductionControl[] ProductionControlArray;

        public tblProduction_Document ProductionDocument;
        public IList<tblProduction_Document> ProductionDocumentList { get; set; }
        public tblProduction_Document[] ProductionDocumentArray;

        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;

        public string fullAddress { get; set; }

        public tblProductAddress ProductAddress;
        public IList<tblProductAddress> ProductAddressList { get; set; }
        public tblProductAddress[] ProductAddressArray;


        public tblProduction_Calendar ProductionCalendar;
        public IList<tblProduction_Calendar> ProductionCalendarList { get; set; }
        public tblProduction_Calendar[] ProductionCalendarArray;

        public SelectedProduct SelectedProduct;
        public IList<SelectedProduct> SelectedProductList { get; set; }
        public SelectedProduct[] SelectedProductArray;

        public Announced Announced;
        public IList<Announced> AnnouncedList { get; set; }
        public IList<Announced> AnnouncedListY { get; set; }
        public Announced[] AnnouncedArray;

        public PagedList.IPagedList<AnnouncementDetail> Paging { get; set; }
        public PagedList.IPagedList<ProductionDetail> PagingProduction { get; set; }

        public PagedList.IPagedList<UserInfo> PagingUserInfo { get; set; }

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


        public int noPaged { get; set; }

        public DateTime createdDate { get; set; }


        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public int itemCount { get; set; }
        public int addressId { get; set; }
        public long rId { get; set; }
        public long rIdau { get; set; }
        public string sort { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string products { get; set; }
        public string pName { get; set; }

        public long arrNum = 0;
        public long pId = 0;
        public long productId = 0;
        public long pIdau = 0;
    }
}
