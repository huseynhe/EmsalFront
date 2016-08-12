using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emsal.WebInt.EmsalSrv;

namespace Emsal.AdminUI.Models
{
    public class UserViewModel
    {
        public tblUser User;
        public tblPerson Person;
        public tblEnumCategory EnumCategory;
        public tblComMessage ComMessage;
        public tblEnumValue EnumValue;
        public tblRole Role;
        public tblUserRole UserRole;

        public string EducationLevel { get; set; }
        public string Job { get; set; }

        public string RoleType { get; set; }
        public List<string> RoleTypes { get; set; }
        public int EducationId { get; set; }
        public int JobId { get; set; }

        public string newPassword { get; set; }

        public virtual string PasswordHash { get; set; }



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
    }
}