using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emsal.WebInt.EmsalSrv;
using System.ComponentModel.DataAnnotations;

namespace Emsal.UI.Models
{
    public class UserViewModel : UserRoles
    {
        public tblUser User;
        public tblPerson Person;
        public tblEnumCategory EnumCategory;
        public tblComMessage ComMessage;
        public tblEnumValue EnumValue;
        public tblRole Role;
        public tblUserRole UserRole;

        public tblAddress Address;
        public IList<tblAddress> AddressList { get; set; }
        public tblAddress[] AddressArray;

        public tblPRM_AdminUnit PRMAdminUnit;
        public IList<tblPRM_AdminUnit> PRMAdminUnitList { get; set; }
        public tblPRM_AdminUnit[] PRMAdminUnitArray;
        public string EducationLevel { get; set; }
        public string Job { get; set; }

        public string RoleType { get; set; }
        public List<string> RoleTypes { get; set; }
        public int EducationId { get; set; }
        public int JobId { get; set; }

        public string newPassword { get; set; }

        public virtual string PasswordHash { get; set; }


        public AuthLogin authLogin;

        public tblEnumValue EducationEnumValue;
        public tblEnumValue JobEnumValue;
        public IList<tblEnumValue> EnumValueEducationList { get; set; }
        public IList<tblEnumValue> EnumValueJobList { get; set; }
        public IList<tblComMessage> ComMessageList { get; set; }
        public IList<tblUser> UserList { get; set; }
        public List<tblUserRole> UserRoleList { get; set; }


        public tblEnumValue[] EnumValueEducationArray;
        public tblEnumValue[] EnumValueJobArray;
        public tblComMessage[] ComMessageArray;
        public tblUserRole[] UserRoleArray;

        //auth
        public tblAuthenticatedPart AuthenticatedPart;

        public tblPrivilegedRole[] PrivilegedRolesArray;
        public List<tblPrivilegedRole> PrivilegedRolesList { get; set; }
        //

        public int arrNum = 0;
        public int arrPNum = 0;

        public string descAddress { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "{0} xanası məcburidir.")]
        public int addressId { get; set; }

        [Required(ErrorMessage = "Tam Adres daxil edilməmişdir")]
        [Display(Name = "Tam Adres")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Küçe daxil edilməmişdir")]
        [Display(Name = "Küçe")]
        public string Throughfare { get; set; }
        public int messageCount { get; set; }


        [Required(ErrorMessage = "Mesaj daxil edilməmişdir")]
        [Display(Name = "Mesaj")]
        public string Message { get; set; }

        public long ComMessageId { get; set; }

        public int MyProperty { get; set; }
        public string NameSurname { get; set; }

        public tblUser LoggedInUser;

        [Display(Name = "Ad")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string Name { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string Surname { get; set; }

        [Display(Name = "Ata adı")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string FatherName { get; set; }

        [Display(Name = "Başlama tarixi")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string profilePicture { get; set; }

        [Display(Name = "Cinsi")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        public string gender { get; set; }

        [Display(Name = "Doğum tarixi")]
        [Required(ErrorMessage = "{0} xanası məcburidir.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public string birtday { get; set; }



        public string createdUser { get; set; }

    }
}