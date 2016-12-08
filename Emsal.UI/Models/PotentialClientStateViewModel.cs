﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emsal.WebInt.EmsalSrv;
using System.ComponentModel.DataAnnotations;
using Emsal.Utility.CustomObjects;

namespace Emsal.UI.Models
{
    public class PotentialClientStateViewModel : UserRoles
    {
        //public string messageSuccess = "Yadda saxlanıldı.";
        //public int fileSize = 2097152;
        //public string fileDirectory = @"C:\inetpub\emsalfiles";
        //public List<string> fileTypes = new List<string> { "image/jpeg", "image/png", "application/pdf" };

        public string messageSuccess = FileExtension.messageSuccess;
        public int fileSize = FileExtension.fileSize;
        public string fileDirectory = FileExtension.fileDirectoryExternal;
        public List<string> fileTypes = FileExtension.fileMimeTypes;

        public tblUser User;

        public tblUserRole UserRole;
        public IList<tblUserRole> UserRoleList { get; set; }
        public tblUserRole[] UserRoleArray;

        public tblPerson Person;

        public tblPotential_Production PotentialProduction;
        public IList<tblPotential_Production> PotentialProductionList { get; set; }
        public IList<tblPotential_Production> SelectedPotentialProductionList { get; set; }
        public tblPotential_Production[] PotentialProductionArray;


        public tblOffer_Production Offer_Production;
        public IList<tblOffer_Production> Offer_ProductionList { get; set; }
        public tblOffer_Production[] Offer_ProductionArray;


        public ProductionDetail ProductionDetail;
        public IList<ProductionDetail> ProductionDetailList { get; set; }
        public ProductionDetail[] ProductionDetailArray;

        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public tblProductCatalog[] ProductCatalogArray;

        public UserInfo UserInfo;
        public IList<UserInfo> UserInfoList { get; set; }
        public UserInfo[] UserInfoArray;

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

        public PagedList.IPagedList<ProductionDetail> PagingDetail { get; set; }
        public PagedList.IPagedList<tblPotential_Production> Paging { get; set; }
        public PagedList.IPagedList<UserInfo> PagingUserInfo { get; set; }


        public bool isPDF { get; set; }
        public long userId { get; set; }

        public long totalSize = 0;

        public long Id { get; set; }
        public long UserId { get; set; }

        [Display(Name = "Bildiriş")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string message { get; set; }


        [Display(Name = "Başlıq")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string title { get; set; }

        [Display(Name = "Əlavə qeyd")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
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

        public int productId { get; set; }

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

        [Display(Name = "Seçilmiş məhsulları təsdiqləyin")]
        public bool confirmList { get; set; }


        public int isMain { get; set; }
        public string stateStatusEV { get; set; }

        [Display(Name = "Səbəb")]
        public long stateStatusEVId { get; set; }
        public string productName { get; set; }
        public string userInfo { get; set; }


        public string nameSurnameFathername { get; set; }
        public string pin { get; set; }
        public string fullAddress { get; set; }
        public int itemCount { get; set; }

    }
}