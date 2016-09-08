using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Emsal.UI.Models
{
    public class SpecialSummaryViewModel:UserRoles
    {
        public PagedList.IPagedList<SpecialSummaryPotentialAndOffer> PagingConfirmedOffer
        {
            get;
            set;
        }
        public PagedList.IPagedList<SpecialSummaryPotentialAndOffer> PagingOffer
        {
            get;
            set;
        }
        public PagedList.IPagedList<SpecialSummaryPotentialAndOffer> PagingOffAirOffer
        {
            get;
            set;
        }
        public PagedList.IPagedList<SpecialSummaryPotentialAndOffer> PagingRejectedOffer
        {
            get;
            set;
        }

        public PagedList.IPagedList<SpecialSummaryPotentialAndOffer> PagingConfirmedPotential
        {
            get;
            set;
        }

        public PagedList.IPagedList<tblComMessage> PagingReceivedMessages
        {
            get;
            set;
        }
        public PagedList.IPagedList<tblComMessage> PagingSentMessages
        {
            get;
            set;
        }
        public PagedList.IPagedList<GovernmentOrganisationDemand> PagingConfirmedDemand
        {
            get;
            set;
        }
        public PagedList.IPagedList<GovernmentOrganisationDemand> PagingDemand
        {
            get;
            set;
        }
        public PagedList.IPagedList<GovernmentOrganisationDemand> PagingOffAirDemand
        {
            get;
            set;
        }
        public PagedList.IPagedList<GovernmentOrganisationDemand> PagingRejectedDemand
        {
            get;
            set;
        }

        public PagedList.IPagedList<tblComMessage> PagingPrivateMessages
        {
            get;
            set;
        }
        
        public tblUser User;
        public IList<tblUser> UserList { get; set; }
        public tblUser[] UserArray;

        public tblOffer_Production OfferProduction;
        public IList<tblOffer_Production> OfferProductionList { get; set; }
        public tblOffer_Production[] OfferProductionArray;


        public tblPotential_Production PotentialProduction;
        public IList<tblPotential_Production> PotentialProductionList { get; set; }
        public IList<tblPotential_Production> SelectedPotentialProductionList { get; set; }
        public tblPotential_Production[] PotentialProductionArray;



        public tblProductCatalog ProductCatalog;
        public IList<tblProductCatalog> ProductCatalogList { get; set; }
        public tblProductCatalog[] ProductCatalogArray;


        public tblProductionControl ProductionControl;
        public IList<tblProductionControl> ProductionControlList { get; set; }
        public tblProductionControl[] ProductionControlArray;


        public tblEnumCategory EnumCategory;
        public IList<tblEnumCategory> EnumCategoryList { get; set; }
        public tblEnumCategory[] EnumCategoryArray;


        public tblEnumValue EnumValue;
        public IList<tblEnumValue> EnumValueList { get; set; }
        public tblEnumValue[] EnumValueArray;





        public tblComMessage ComMessage;
        public long ComMessageId { get; set; }

        public tblComMessage[] ComMessageArray;

        public IList<tblComMessage> ComMessageList { get; set; }
        public tblComMessage[] NotReadComMessageArray;
        
        public tblUserRole UserRole;
        public tblUserRole[] UserRoleArray;


        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;


        public int arrNum = 0;
        public int arrPNum = 0;

        public string NameSurname { get; set; }


        public tblPerson Person;

        public int MessageCount { get; set; }

        public tblUser LoggedInUser;

        public LoggedInUserInfos LoggedInUserInfos { get; set; }


        public tblDemand_Production DemandProduction;
        public IList<tblDemand_Production> DemandProductionList { get; set; }
        public IList<tblDemand_Production> SelectedDemandProductionList { get; set; }
        public tblDemand_Production[] DemandProductionArray;


        //ferid
        public MonthsModel modelMonths { get; set; }
        public List<MonthsModel> modelMonthsList { get; set; }


        public tblProductAddress ProductAddress;
        public IList<tblProductAddress> ProductAddressList { get; set; }
        public IList<tblProductAddress> ProductAddressListFE { get; set; }
        public IList<tblProductAddress>[] ProductAddressListFEA { get; set; }
        public tblProductAddress[] ProductAddressArray;


        public tblProduction_Calendar ProductionCalendar;
        public IList<tblProduction_Calendar> ProductionCalendarList { get; set; }
        public tblProduction_Calendar[] ProductionCalendarArray;


        public tblRole Role;

        [Required(ErrorMessage = "Mesaj daxil edilməmişdir")]
        [Display(Name = "Mesaj")]
        public string Message { get; set; }



        public string Type { get; set; }

        public List<tblProduct_Document> ProductDocumentList;
        public tblProduct_Document[] ProductDocumentArray;
        public tblProduct_Document ProductDocument;


        public tblUser CommunicatedUser;


        public tblForeign_Organization Organisation;
        public tblForeign_Organization[] OrganisationArray;
        public List<tblForeign_Organization> OrganisationList { get; set; }

        public tblForeign_Organization ParentOrganisation;



        public long ParentOrganisationId { get; set; }



        //for the form
        [Required(ErrorMessage = "İstifadəçi adı daxil edilməmişdir")]
        [Display(Name = "İstifadəçi Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vöen daxil edilməmişdir")]
        [Display(Name = "Vöen")]
        public string Voen { get; set; }

        [Required(ErrorMessage = "Şifrə daxil edilməmişdir")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Şifrə minimum 8 xarakterdən ibarət olmalıdır")]
        [Display(Name = "Şifrə")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Elektron Poçt Ünvanı daxil edilməmişdir")]
        [Display(Name = "Elektron Poçt Ünvanı")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Məsul Şəxsin Elektron Poçt ünvanı daxil edilməmişdir")]
        [Display(Name = "Məsul Şəxsin Elektron Poçt Ünvanı")]
        public string ManagerEmail { get; set; }


        public int addressId { get; set; }


        [Required(ErrorMessage = "Adres daxil edilməmişdir")]
        [Display(Name = "Adres")]
        public string FullAddress { get; set; }

        public tblAddress Address;
        public tblAddress[] AddressArray;

        public int[] adId { get; set; }

        public tblPerson Manager;

        [Required(ErrorMessage = "Menecer adı daxil edilməmişdir")]
        [Display(Name = "Məsul Şəxsin Adı")]
        public string ManagerName { get; set; }

        [Required(ErrorMessage = "Fin daxil edilməmişdir")]
        [Display(Name = "Fin")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "Ata adı daxil edilməmişdir")]
        [Display(Name = "Ata Adı")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "Cinsi daxil edilməmişdir")]
        [Display(Name = "Cinsi")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Təhsil məlumatları daxil edilməmişdir")]
        [Display(Name = "Təhsil")]
        public string Education { get; set; }

        [Required(ErrorMessage = "İş məlumatları daxil edilməmişdir")]
        [Display(Name = "İş")]
        public string Job { get; set; }

        [Required(ErrorMessage = "Ad daxil edilməmişdir")]
        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad daxil edilməmişdir")]
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Doğum tarixi daxil edilməmişdir")]
        [Display(Name = "Doğum Tarixi")]
        public string Birthday { get; set; }


        public List<tblEnumValue> EducationList { get; set; }
        public List<tblEnumValue> JobList { get; set; }

        [Display(Name = "Məsul Şəxsin ev telefonu")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 10 rəqəmdən ibarət olmalıdır")]
        public string ManagerHomePhone { get; set; }


        [Required(ErrorMessage = "Məsul Şəxsin iş telefonu daxil edilməmişdir")]
        [Display(Name = "Məsul Şəxsin iş telefonu")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır")]
        public string ManagerWorkPhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır")]
        [Display(Name = "Məsul Şəxsin mobil telefonu")]
        public string ManagerMobilePhone { get; set; }


        public long ManagerEducation { get; set; }
        public long ManagerJob { get; set; }

        public tblCommunication ComunicationInformations;
        public tblCommunication[] CommunicationInformationsArray;
        public List<tblCommunication> CommunicationInformationsList { get; set; }


        //id of the organisation
        public long OrganisationId { get; set; }

        public long CommunicatedUserId { get; set; }

        public SpecialSummaryPotentialAndOffer SpOffer { get; set; }
        public SpecialSummaryPotentialAndOffer[] SpOfferArray { get; set; }
        public List<SpecialSummaryPotentialAndOffer> SpOfferList { get; set; }

        public GovernmentOrganisationDemand OrgDemand { get; set; }
        public GovernmentOrganisationDemand[] OrgDemandArray { get; set; }
        public List<GovernmentOrganisationDemand> OrgDemandList { get; set; }

        public string mobilePhonePrefix { get; set; }

        public string WorkPhonePrefix { get; set; }

        public List<tblEnumValue> MobilePhonePrefixList { get; set; }

        public List<tblEnumValue> WorkPhonePrefixList { get; set; }
        public long AdminUnitId { get; set; }
        public List<long> GivenAdminUnitIds { get; set; }
        public IList<tblPRM_AdminUnit>[] PRMAdminUnitArrayPa { get; set; }
        public string descAddress { get; set; }
        public List<tblEnumValue> UserTypeList { get; set; }

        public string finvoen { get; set; }
        public int? finvoenType { get; set; }

        [Required(ErrorMessage = "İş telefonu daxil edilməmişdir")]
        [Display(Name = "İş telefonu")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır")]
        public string WorkPhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır")]
        [Display(Name = "Mobil telefonu")]
        public string MobilePhone { get; set; }

        public string FutureUserRole { get; set; }
        public tblForeign_Organization ForeignOrganisation;
        public tblForeign_Organization[] ForeignOrganisationArray;
        public List<tblForeign_Organization> ForeignOrganisationList { get; set; }

    }
}