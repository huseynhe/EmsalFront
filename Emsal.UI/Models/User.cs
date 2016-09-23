using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emsal.UI.Models
{
    public class User : UserRoles
    {
        [Required(ErrorMessage = "İstifadəçi adı daxil edilməmişdir")]
        [Display(Name = "İstifadəçi Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ad daxil edilməmişdir")]
        [Display(Name = "Adı")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Menecer adı daxil edilməmişdir")]
        [Display(Name = "Menecer adı")]
        public string ManagerName { get; set; }


        [Required(ErrorMessage = "Soyad daxil edilməmişdir")]
        [Display(Name = "Soyadı")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Ata adı daxil edilməmişdir")]
        [Display(Name = "Ata Adı")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "Cinsi daxil edilməmişdir")]
        [Display(Name = "Cinsi")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Doğum tarixi daxil edilməmişdir")]
        [Display(Name = "Doğum Tarixi")]
        public string Birthday { get; set; }


        [Required(ErrorMessage = "Fin daxil edilməmişdir")]
        [Display(Name = "Fin")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "Vöen daxil edilməmişdir")]
        [Display(Name = "Vöen")]
        public string Voen { get; set; }



        public string UserRoleType { get; set; }


        public string UserRoleTypeClient { get; set; }

        public string UserRoleTypeSeller { get; set; }

        [Required(ErrorMessage = "Email daxil edilməmişdir")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifrə daxil edilməmişdir")]
        [Display(Name = "Şifrə")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Ölke daxil edilməmişdir")]
        [Display(Name = "Ölke")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Şeher daxil edilməmişdir")]
        [Display(Name = "Şeher")]
        public string City { get; set; }


        [Required(ErrorMessage = "Şeher daxil edilməmişdir")]
        [Display(Name = "Şeher")]
        public string ManagerCity { get; set; }

        [Required(ErrorMessage = "Küçe daxil edilməmişdir")]
        [Display(Name = "Küçe")]
        public string Throughfare { get; set; }

        [Required(ErrorMessage = "Küçe daxil edilməmişdir")]
        [Display(Name = "Küçe")]
        public string ManagerThroughfare { get; set; }

        [Required(ErrorMessage = "Kənd daxil edilməmişdir")]
        [Display(Name = "Kənd")]
        public string Village { get; set; }

        [Required(ErrorMessage = "Kənd daxil edilməmişdir")]
        [Display(Name = "Kənd")]
        public string ManagerVillage { get; set; }

        [Required(ErrorMessage = "Adres daxil edilməmişdir")]
        [Display(Name = "Adres")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Menecerin Adresi daxil edilməmişdir")]
        [Display(Name = "Menecerin Adresi")]
        public string ManagerFullAddress { get; set; }


        [Required(ErrorMessage = "Təhsil məlumatları daxil edilməmişdir")]
        [Display(Name = "Təhsil")]
        public string Education { get; set; }

        [Required(ErrorMessage = "İş məlumatları daxil edilməmişdir")]
        [Display(Name = "İş")]
        public string Job { get; set; }

        public tblUser FutureUser;
        public tblPerson FuturePerson;
        public tblAddress FutureAddress;
        public tblAddress ManagerFutureAddress;
        public tblForeign_Organization ForeignOrganisation;
        public tblRole Role;
        public tblPerson Manager;
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
        public List<tblEnumValue> UserTypeList { get; set; }
        public long UserRoleId { get; set; }
        public long Status { get; set; }
        public long AddressId { get; set; }

        public long CityId { get; set; }

        public long VillageId { get; set; }

        public long ThroughfareId { get; set; }

        public string FutureUserRole { get; set; }

        public string UserType { get; set; }

        public int[] adId { get; set; }
        public int lastAdminUnitId { get; set; }

        public tblPRM_Thoroughfare ThroughfarePrm;

        public string uid { get; set; }
        public int? finvoenType { get; set; }

        public tblUser CreatedUser;

        public tblUser[] UserArray;
        public IEnumerable<tblUser> UserList { get; set; }

        [Display(Name = "ID")]
        public string ID { get; set; }

        public PagedList.IPagedList<CreatedUser> PagingCreatedUsers
        {
            get;
            set;
        }

        public CreatedUser Createduser;

        public CreatedUser[] CreatedUserArray { get; set; }

        public List<CreatedUser> CreatedUserList { get; set; }



        public tblPerson[] PersonArray;
        public List<tblPerson> PersonList { get; set; }

        public tblForeign_Organization[] ForeignOrganisationArray;
        public List<tblForeign_Organization> ForeignOrganisationList { get; set; }

        public long AdminUnitId { get; set; }

        [Required(ErrorMessage = "İş telefonu daxil edilməmişdir")]
        [Display(Name = "İş telefonu")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır")]
        public string WorkPhone { get; set; }

        [Required(ErrorMessage = "Mobil telefonu daxil edilməmişdir")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Telefon nömrəsi 7 rəqəmdən ibarət olmalıdır")]
        [Display(Name = "Mobil telefonu")]
        public string MobilePhone { get; set; }

        public List<tblEnumValue> MobilePhonePrefixList { get; set; }

        public List<tblEnumValue> WorkPhonePrefixList { get; set; }
        public string mobilePhonePrefix { get; set; }
        public string workPhonePrefix { get; set; }

        public tblCommunication ComunicationInformations;
        public tblCommunication[] CommunicationInformationsArray;
        public List<tblCommunication> CommunicationInformationsList { get; set; }

    }
}