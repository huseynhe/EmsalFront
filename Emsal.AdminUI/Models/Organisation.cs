using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.AdminUI.Models
{
    public class Organisation
    {
        [Required(ErrorMessage = "İstifadəçi adı daxil edilməmişdir")]
        [Display(Name = "İstifadəçi Adı")]
        public string UserName { get; set; }

        public PagedList.IPagedList<tblUser> Paging
        {
            get;
            set;
        }

        public PagedList.IPagedList<long> PagingOrganisation
        {
            get;
            set;
        }


        public PagedList.IPagedList<tblAuthenticatedPart> PagingParts
        {
            get;
            set;
        }
        public PagedList.IPagedList<tblPRM_ASCBranch> PagingASC
        {
            get;
            set;
        }
        public PagedList.IPagedList<tblPRM_KTNBranch> PagingKTN
        {
            get;
            set;
        }
        public PagedList.IPagedList<long> PagingIndividual
        {
            get;
            set;
        }

        public UserDetails[] userDetailArray;
        public UserDetailSearch search { get; set; }
        public int pageCount { get; set; }
        public int pageNumber { get; set; }

        public string UserRoleType { get; set; }

        public string UserRoleTypeClient { get; set; }

        public string UserRoleTypeSeller { get; set; }

        [Required(ErrorMessage = "Elektron poçt daxil edilməmişdir")]
        [Display(Name = "Elektron poçt")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Təşkilat adı daxil edilməmişdir")]
        [Display(Name = "Təşkilat adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Şifrə daxil edilməmişdir")]
        [Display(Name = "Şifrə")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Ölke daxil edilməmişdir")]
        [Display(Name = "Ölke")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Şeher daxil edilməmişdir")]
        [Display(Name = "Şeher")]
        public string City { get; set; }

        [Required(ErrorMessage = "Küçe daxil edilməmişdir")]
        [Display(Name = "Küçe")]
        public string Throughfare { get; set; }

        [Required(ErrorMessage = "Kənd daxil edilməmişdir")]
        [Display(Name = "Kənd")]
        public string Village { get; set; }

        [Required(ErrorMessage = "Adres daxil edilməmişdir")]
        [Display(Name = "Adres")]
        public string FullAddress { get; set; }

        public tblUser User;
        public tblUser FutureUser;
        public tblUser[] UserArray;
        public IList<tblUser> UserList { get; set; }
        public tblForeign_Organization ForeignOrganisation;

        public tblAddress FutureAddress;
        public tblAddress[] FutureAddressArray;
        public List<tblAddress> FutureAddressList { get; set; }

        public tblRole Role;
        public tblPRM_AdminUnit PRMAdminUnit;
        public tblPRM_AdminUnit[] PRMAdminUnitArray;
        public List<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_Thoroughfare PRMThroughfare;
        public tblPRM_Thoroughfare[] PRMThroughfareArray;
        public List<tblPRM_Thoroughfare> PRMThroughfareList;
        public tblEnumValue EnumValue;
        public tblEnumValue[] EnumValueArray;
        public List<tblEnumValue> EnumValueList { get; set; }
        public tblEnumCategory EnumCategory;
        public List<tblEnumCategory> EnumCategoryList { get; set; }
        public tblRole[] UserRoleArray;
        public List<tblRole> UserRoleList { get; set; }
        public tblUserRole UserRole;
        public tblUserRole[] UserRolesArray;
        public List<tblEnumValue> EducationList { get; set; }
        public List<tblEnumValue> JobList { get; set; }
        public List<tblEnumValue> MobilePhonePrefixList { get; set; }

        public List<tblEnumValue> WorkPhonePrefixList { get; set; }

        public long UserRoleId { get; set; }
        public long Status { get; set; }
        public long AddressId { get; set; }

        public long ThroughfareId { get; set; }

        public long VillageId { get; set; }

        public long CityId { get; set; }
        public tblPerson Manager;

        [Required(ErrorMessage = "Məsul şəxsin adı daxil edilməmişdir")]
        [Display(Name = "Məsul şəxsin adı")]
        public string ManagerName { get; set; }

        [Required(ErrorMessage = "Soyad daxil edilməmişdir")]
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }


        [Required(ErrorMessage = "Fin daxil edilməmişdir")]
        [Display(Name = "Fin")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "Voen daxil edilməmişdir")]
        [Display(Name = "Voen")]
        public string Voen { get; set; }

        [Required(ErrorMessage = "Ata adı daxil edilməmişdir")]
        [Display(Name = "Ata Adı")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "Doğum tarixi daxil edilməmişdir")]
        [Display(Name = "Doğum Tarixi")]
        public string Birthday { get; set; }

        [Required(ErrorMessage = "Cinsi daxil edilməmişdir")]
        [Display(Name = "Cinsi")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Təhsil məlumatları daxil edilməmişdir")]
        [Display(Name = "Təhsil")]
        public string Education { get; set; }

        [Required(ErrorMessage = "İş məlumatları daxil edilməmişdir")]
        [Display(Name = "İş")]
        public string Job { get; set; }

        public tblPerson Person;


        public tblAuthenticatedPart AuthenticatedPart;

        public tblAuthenticatedPart[] AuthenticatedPartArray;

        public IList<tblAuthenticatedPart> AuthenticatedPartList { get; set; }

        public List<tblRole> PageRoles { get; set; }
        public IList<tblRole> UserRoles { get; set; }
        public tblPrivilegedRole PrivilegedRole;
        public tblPrivilegedRole[] PrivilegedRolesArray;
        public List<tblPrivilegedRole> PrivilegedRolesList { get; set; }

        public tblUser Admin;
        public int[] adId { get; set; }

        public List<string> ktn { get; set; }

        public string[] ktnar { get; set; }


        public List<string> asc { get; set; }

        public string[] ascar { get; set; }

        public tblPRM_Thoroughfare ThroughfarePrm;

        public int arrNum = 0;
        public int arrPNum = 0;


        public tblPRM_ASCBranch ASCBranch;
        public tblPRM_ASCBranch[] ASCBranchArray;
        public List<tblPRM_ASCBranch> ASCBranchList { get; set; }

        public tblPRM_KTNBranch KTNBranch;
        public tblPRM_KTNBranch[] KTNBranchArray;
        public List<tblPRM_KTNBranch> KTNBranchList { get; set; }

        [Display(Name = "ID")]
        public string ID { get; set; }

        public tblUser UserINFO;

        public long ASCId { get; set; }
        public long KTNId { get; set; }

        public tblBranchResponsibility branchResponsibility;
        public tblBranchResponsibility[] branchRespArray;

        [Required(ErrorMessage = "İstifadeçi növü seçilmemişdir")]
        [Display(Name = "İstifadeçi növü")]
        public string userType { get; set; }

        public tblPerson[] PersonArray;
        public List<tblPerson> PersonList { get; set; }

        public tblForeign_Organization[] ForeignOrganisationArray;
        public List<tblForeign_Organization> ForeignOrganisationList { get; set; }


        [Display(Name = "Məsul Şəxsin ev telefonu")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 10 rəqəmdən ibarət olmalıdır və tərkibində hərf olmamaldır")]
        public string ManagerHomePhone { get; set; }


        [Required(ErrorMessage = "Məsul Şəxsin iş telefonu daxil edilməmişdir")]
        [Display(Name = "Məsul Şəxsin iş telefonu")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır və tərkibində hərf olmamaldır")]
        public string ManagerWorkPhone { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Mobil nömrəsi 7 rəqəmdən ibarət olmalıdır.Tərkibində hərf olmamaldır")]
        [Display(Name = "Məsul Şəxsin mobil telefonu")]
        public string ManagerMobilePhone { get; set; }


        public long ManagerEducation { get; set; }
        public long ManagerJob { get; set; }

        public long ParentOrganisationId { get; set; }

        [Required(ErrorMessage = "Məsul Şəxsin Elektron Poçt ünvanı daxil edilməmişdir")]
        [Display(Name = "Məsul Şəxsin Elektron Poçt Ünvanı")]
        public string ManagerEmail { get; set; }

        public tblCommunication ComunicationInformations;
        public tblCommunication[] CommunicationInformationsArray;
        public List<tblCommunication> CommunicationInformationsList { get; set; }

        public tblForeign_Organization ParentOrganisation;

        //id of the organisation
        public long OrganisationId { get; set; }


        public GovOrAnyOrganisation GovernmentOrganisation { get; set; }
        public GovOrAnyOrganisation[] GovernmentOrganisationArray { get; set; }
        public List<GovOrAnyOrganisation> GovernmentOrganisationList { get; set; }


        public Individual Individual { get; set; }
        public Individual[] IndividualArray { get; set; }
        public List<Individual> IndividualList { get; set; }


        public long AdminUnitId { get; set; }


        public List<long> GivenAdminUnitIds { get; set; }
        public long[] productAddressIds { get; set; }

        public IList<tblPRM_AdminUnit>[] PRMAdminUnitArrayPa { get; set; }

        [Display(Name = "Tam Ünvan")]
        public string descAddress { get; set; }

        public long RedirectToParent { get; set; }


        public string mobilePhonePrefix { get; set; }

        public string WorkPhonePrefix { get; set; }
        public List<string> NullCommunicationsList { get; set;}


        public bool NotChild { get; set; }


        public long[] branchesIdArr { get; set; }
        public string actionName { get; set; }

        public string SName { get; set; }
        public string SSurname { get; set; }
        public string SFName { get; set; }
        public string SEMail { get; set; }
        public string SFullAddress { get; set; }
        public string SUserName { get; set; }
        public string SOrganizationName { get; set; }
    }


}