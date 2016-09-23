using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Emsal.Utility.CustomObjects;

namespace Emsal.UI.Controllers
{
    public class SignUpController : Controller
    {
        //
        // GET: /SignUp/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private BaseInput binput;
        UserViewModel modelUser;

        public long ConvertStringYearMonthDayFormatToTimestamp(User form)
        {
            Regex regex = new Regex(@"\.");
            string[] dates = regex.Split(form.Birthday);
            int year = Convert.ToInt32(dates[2]);
            int month = Convert.ToInt32(dates[1]);
            int day = Convert.ToInt32(dates[0]);
            DateTime dTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long birthday = (long)(dTime - sTime).TotalSeconds;

            return birthday;
        }

        public bool CheckUsername(User form)
        {
            binput = new BaseInput();
            User modelUser = new User();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);

            if(modelUser.FutureUser != null)
            {
                return false;
            }
            return true;
        }

        public bool CheckPin(User form)
        {
            binput = new BaseInput();
            User modelUser = new User();

            BaseOutput personsOut = srv.WS_GetPersons(binput, out modelUser.PersonArray);
            List<tblPerson> PersonFromPin = form.Pin == null ? new List<tblPerson>() : modelUser.PersonArray.Where(x => x.PinNumber == form.Pin).ToList();

            if(PersonFromPin.Count != 0)
            {
                return false;
            }
            return true;
        }

        public bool CheckVoen(User form)
        {
            binput = new BaseInput();
            User modelUser = new User();


            BaseOutput orgOut = srv.WS_GetForeign_Organizations(binput, out modelUser.ForeignOrganisationArray);
            List<tblForeign_Organization> OrgFromVoen = form.Voen == null ? new List<tblForeign_Organization>() : modelUser.ForeignOrganisationArray.Where(x => x.voen == form.Voen).ToList();

            if(OrgFromVoen.Count != 0)
            {
                return false;
            }
            return true;
        }

        public bool CheckExistence(User form)
        {
            binput = new BaseInput();
            User modelUser = new User();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);

            BaseOutput checkEdxistenseOut = srv.WS_GetUsers(binput, out modelUser.UserArray);
            List<tblUser> UserFromEmail = modelUser.UserArray.Where(x => x.Email == form.Email).ToList();

            BaseOutput personsOut = srv.WS_GetPersons(binput, out modelUser.PersonArray);
            List<tblPerson> PersonFromPin = form.Pin == null ? new List<tblPerson>() : modelUser.PersonArray.Where(x => x.PinNumber == form.Pin).ToList();

            BaseOutput orgOut = srv.WS_GetForeign_Organizations(binput, out modelUser.ForeignOrganisationArray);
            List<tblForeign_Organization> OrgFromVoen = form.Voen == null ? new List<tblForeign_Organization>() : modelUser.ForeignOrganisationArray.Where(x => x.voen == form.Voen).ToList();

            //normal ucun olmalidir 
            //if (modelUser.FutureUser != null || UserFromEmail.Count != 0 || PersonFromPin.Count != 0 || OrgFromVoen.Count != 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            if (modelUser.FutureUser != null || UserFromEmail.Count != 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public ActionResult Index(string uid, int? type)
        {
            //ModelState.Clear();
            TempData.Clear();
            Session["arrNum"] = null;

            User modelUser = new User();
            BaseOutput enumCatOut = srv.WS_GetEnumCategorysByName(binput, "UserType", out modelUser.EnumCategory);

            BaseOutput userTypesOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);

            modelUser.UserTypeList = modelUser.EnumValueArray.ToList();

            modelUser.finvoenType = type;
            modelUser.uid = uid;


            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelUser.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.MobilePhonePrefixList = modelUser.EnumValueArray.ToList();

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult Index(User form)
        {
            if (CheckExistence(form))
            {
                binput = new BaseInput();

                User modelUser = new User();


                modelUser.FutureUser = new tblUser();
                modelUser.FuturePerson = new tblPerson();
                modelUser.FutureAddress = new tblAddress();
                modelUser.UserRole = new tblUserRole();

                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);

                modelUser.Password = form.Password;

                BaseOutput adminunits = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == 0).ToList();
                if (form.Pin != null && form.uid != null)
                {
                    BaseOutput personOut = srv.WS_GetPersonByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.FuturePerson);
                    if (form.Pin != modelUser.FuturePerson.PinNumber)
                    {
                        TempData["pinIncorrect"] = "info";
                        return Redirect("http://localhost:56557/SignUp?uid=" + form.uid + "&type=1");

                    }
                }
                if (form.Voen != null && form.uid != null)
                {
                    BaseOutput personOut = srv.WS_GetForeign_OrganizationByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.ForeignOrganisation);
                    if (form.Voen != modelUser.ForeignOrganisation.voen)
                    {
                        TempData["voenIncorrect"] = "info";
                        return Redirect("http://localhost:56557/SignUp?uid=" + form.uid + "&type=2");
                    }
                }
                modelUser.Voen = form.Voen;
                modelUser.Pin = form.Pin;
                modelUser.finvoenType = form.finvoenType;
                modelUser.uid = form.uid;

                modelUser.MobilePhone = form.MobilePhone;
                modelUser.mobilePhonePrefix = form.mobilePhonePrefix;


                if (modelUser.uid != null)
                {
                    return AddAddressInfo(modelUser.FutureUser, "producerPerson", modelUser);
                }

                return AddAddressInfo(modelUser.FutureUser, "sellerPerson", modelUser);


            }
            else
            {
                if (!CheckUsername(form))
                {
                    TempData["UserNameExists"] = "info";
                }
                //else if (!CheckPin(form))
                //{
                //    TempData["YouaresignedUp"] = "info";
                //}
                //else if (!CheckVoen(form))
                //{
                //    TempData["YouaresignedUp"] = "info";
                //}
              
                return RedirectToAction("Index");
            }
        }

        public ActionResult AddAddressInfo(tblUser FutureUser, string userTpe, User UserInfo)
        {
            Session["arrONum"] = null;

            User modelUser = new User();

            modelUser.Password = FutureUser.Password;

            modelUser.UserName = FutureUser.Username;

            modelUser.Email = FutureUser.Email;

            modelUser.MobilePhone = UserInfo.MobilePhone;
            modelUser.mobilePhonePrefix = UserInfo.mobilePhonePrefix;

            modelUser.FutureUserRole = userTpe;

            modelUser.Pin = UserInfo.Pin;
            modelUser.Voen = UserInfo.Voen;
            modelUser.uid = UserInfo.uid;
            modelUser.finvoenType = UserInfo.finvoenType;

            if (modelUser.uid != null)
            {
                if (modelUser.finvoenType == 1)
                {
                    BaseOutput personOUt = srv.WS_GetPersonByUserId(binput, Convert.ToInt64(modelUser.uid), true, out modelUser.FuturePerson);
                    BaseOutput addressOut = srv.WS_GetAddressById(binput, (long)modelUser.FuturePerson.address_Id, true, out modelUser.FutureAddress);
                    modelUser.AdminUnitId = (long)modelUser.FutureAddress.adminUnit_Id;
                }
                if (modelUser.finvoenType == 2)
                {
                    BaseOutput orgOut = srv.WS_GetForeign_OrganizationByVoen(binput, modelUser.Voen, out modelUser.ForeignOrganisation);
                    BaseOutput addressOut = srv.WS_GetAddressById(binput, (long)modelUser.ForeignOrganisation.address_Id, true, out modelUser.FutureAddress);
                    modelUser.AdminUnitId = (long)modelUser.FutureAddress.adminUnit_Id;
                }
            }

            return View("AddAddressInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddAddressInfo(User form)
        {
            binput = new BaseInput();
            User modelUser = new User();

            modelUser.FutureAddress = new tblAddress();

            BaseOutput userrOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);


            //BaseOutput galf = srv.WS_GetAdminUnitListForID(binput, form.lastAdminUnitId, true, out modelUser.PRMAdminUnitArray);
            //modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();
            //modelUser.FutureAddress.fullAddress = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Name));

            modelUser.FutureAddress.adminUnit_Id = form.lastAdminUnitId;


            modelUser.UserName = form.UserName;
            modelUser.AddressId = (long)modelUser.FutureAddress.adminUnit_Id;
            modelUser.Pin = form.Pin;
            modelUser.Voen = form.Voen;
            modelUser.Email = form.Email;
            modelUser.Password = form.Password;
            modelUser.FutureUserRole = form.FutureUserRole;
            modelUser.finvoenType = form.finvoenType;
            modelUser.uid = form.uid;
            if (modelUser.uid == null)
                modelUser.uid = "0";
            modelUser.MobilePhone = form.MobilePhone;
            modelUser.mobilePhonePrefix = form.mobilePhonePrefix;

            if (form.Voen == null)
            {
                return AddPersonInfo(modelUser.UserName, modelUser);
            }
            else
            {
                return AddOrganisationInfo(modelUser, form.FutureUserRole);
            }
        }


        public ActionResult AddPersonInfo(string userName, User UserInfo)
        {
            ModelState.Clear();

            User modelUser = new Models.User();

            BaseOutput enumcatEd = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelUser.EnumCategory);
            if (modelUser.EnumCategory == null)
            {
                modelUser.EnumCategory = new tblEnumCategory();
            }
            BaseOutput enumvalEd = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);

            modelUser.EducationList = modelUser.EnumValueArray.ToList();

            BaseOutput enumcatJob = srv.WS_GetEnumCategorysByName(binput, "İş", out modelUser.EnumCategory);
            if (modelUser.EnumCategory == null)
            {
                modelUser.EnumCategory = new tblEnumCategory();
            }
            BaseOutput enumvalJob = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.JobList = modelUser.EnumValueArray.ToList();

            if (UserInfo.uid != null)
            {
                BaseOutput finvoenPersonOUt = srv.WS_GetPersonByUserId(binput, Convert.ToInt64(UserInfo.uid), true, out modelUser.FuturePerson);
                BaseOutput finvoenUserOut = srv.WS_GetUserById(binput, (long)modelUser.FuturePerson.UserId, true, out modelUser.FutureUser);
                modelUser.Name = modelUser.FuturePerson == null ? null : modelUser.FuturePerson.Name;
                modelUser.FatherName = modelUser.FuturePerson == null ? null : modelUser.FuturePerson.FatherName;
                modelUser.Surname = modelUser.FuturePerson == null ? null : modelUser.FuturePerson.Surname;
            }


            modelUser.AddressId = (long)UserInfo.FutureAddress.adminUnit_Id;
            modelUser.UserName = userName;
            modelUser.Password = UserInfo.Password;
            modelUser.Pin = UserInfo.Pin;
            modelUser.finvoenType = UserInfo.finvoenType;
            modelUser.uid = UserInfo.uid;
            modelUser.FullAddress = UserInfo.FullAddress;
            modelUser.Email = UserInfo.Email;
            modelUser.MobilePhone = UserInfo.MobilePhone;
            modelUser.mobilePhonePrefix = UserInfo.mobilePhonePrefix;

            return View("AddPersonInfo", modelUser);
        }
        [HttpPost]
        public ActionResult AddPersonInfo(User form)
        {
            binput = new BaseInput();

            User modelUser = new User();
            string userRoles;
            modelUser.UserRole = new tblUserRole();
            modelUser.ThroughfarePrm = new tblPRM_Thoroughfare();
            if (form.uid != null)
            {
                BaseOutput personWithFinOut = srv.WS_GetPersonByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.FuturePerson);
                BaseOutput userWithFinVoenOut = srv.WS_GetUserById(binput, (long)modelUser.FuturePerson.UserId, true, out modelUser.FutureUser);

                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);

                BaseOutput updateUser = srv.WS_UpdateUser(binput, modelUser.FutureUser, out modelUser.FutureUser);
                userRoles = "producerPerson";
            }

            else
            {
                modelUser.FutureUser = new tblUser();
                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
                modelUser.FutureUser.Status = 1;

                BaseOutput personEnumOut = srv.WS_GetEnumValueByName(binput, "fizikişexs", out modelUser.EnumValue);

                modelUser.FutureUser.userType_eV_ID = modelUser.EnumValue.Id;
                modelUser.FutureUser.userType_eV_IDSpecified = true;
                modelUser.FutureUser.StatusSpecified = true;
                BaseOutput userOut = srv.WS_AddUser(binput, modelUser.FutureUser, out modelUser.FutureUser);
                userRoles = "sellerPerson";

            }


            string[] userRolesArr = userRoles.Split(',');

            foreach (var item in userRolesArr)
            {
                BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, item, out modelUser.Role);

                modelUser.UserRole.RoleId = modelUser.Role == null ? 0 : modelUser.Role.Id;
                modelUser.UserRole.RoleIdSpecified = true;
                modelUser.UserRole.UserId = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
                modelUser.UserRole.UserIdSpecified = true;
                modelUser.UserRole.Status = 1;
                modelUser.UserRole.StatusSpecified = true;
                if(form.uid == null)
                {
                    BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);
                }
            }


            //BaseOutput address = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);

            if (form.uid != null)
            {
                BaseOutput personWitFinOut = srv.WS_GetPersonByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.FuturePerson);

                //update address
                BaseOutput addressout = srv.WS_GetAddressById(binput, (long)modelUser.FuturePerson.address_Id, true, out modelUser.FutureAddress);

                modelUser.FutureAddress.thoroughfare_Id = modelUser.ThroughfarePrm == null ? 0 : modelUser.ThroughfarePrm.Id;
                modelUser.FutureAddress.thoroughfare_IdSpecified = true;

                //modelUser.FutureAddress.fullAddress = form.FullAddress;
                //modelUser.FutureAddress.adminUnit_Id = form.AddressId;


                BaseOutput galf = srv.WS_GetAdminUnitListForID(binput, form.AddressId, true, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();
                modelUser.FutureAddress.fullAddress = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Name));

                modelUser.FutureAddress.adminUnit_Id = form.AddressId;

                modelUser.FutureAddress.Status = 1;
                modelUser.FutureAddress.StatusSpecified = true;
                modelUser.FutureAddress.user_Id = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
                modelUser.FutureAddress.user_IdSpecified = true;
                modelUser.FutureAddress.user_type_eV_IdSpecified = true;

                modelUser.FutureAddress.adminUnit_IdSpecified = true;

                BaseOutput updateAddress = srv.WS_UpdateAddress(binput, modelUser.FutureAddress);

                modelUser.FuturePerson.gender = form.Gender;

                modelUser.FuturePerson.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
                modelUser.FuturePerson.address_Id = modelUser.FutureAddress == null ? 0 : modelUser.FutureAddress.Id;
                modelUser.FuturePerson.address_IdSpecified = true;
                modelUser.FuturePerson.birtdaySpecified = true;

                if (form.Education != "Təhsil Seçin")
                {
                    BaseOutput educationWithFinEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
                    modelUser.FuturePerson.educationLevel_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                    modelUser.FuturePerson.educationLevel_eV_IdSpecified = true;

                }

                if (form.Job != "İş seçin")
                {
                    BaseOutput jobWithFinEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
                    modelUser.FuturePerson.job_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                    modelUser.FuturePerson.job_eV_IdSpecified = true;

                }

                BaseOutput updateWithFinPerson = srv.WS_UpdatePerson(binput, modelUser.FuturePerson, out modelUser.FuturePerson);
            }
            else
            {
                modelUser.FutureAddress = new tblAddress();

                modelUser.FutureAddress.thoroughfare_Id = modelUser.ThroughfarePrm == null ? 0 : modelUser.ThroughfarePrm.Id;
                modelUser.FutureAddress.thoroughfare_IdSpecified = true;
                modelUser.FutureAddress.fullAddress = form.FullAddress;
                modelUser.FutureAddress.Status = 1;
                modelUser.FutureAddress.StatusSpecified = true;
                modelUser.FutureAddress.user_Id = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
                modelUser.FutureAddress.user_IdSpecified = true;
                modelUser.FutureAddress.user_type_eV_IdSpecified = true;

                modelUser.FutureAddress.adminUnit_Id = form.AddressId;
                modelUser.FutureAddress.adminUnit_IdSpecified = true;

                BaseOutput address = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);


                modelUser.FuturePerson = new tblPerson();
                modelUser.FuturePerson.UserId = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
                modelUser.FuturePerson.UserIdSpecified = true;
                modelUser.FuturePerson.Name = form.Name;
                modelUser.FuturePerson.FatherName = form.FatherName;
                modelUser.FuturePerson.Surname = form.Surname;
                modelUser.FuturePerson.PinNumber = form.Pin;
                modelUser.FuturePerson.gender = form.Gender;
                modelUser.FuturePerson.Status = 1;

                modelUser.FuturePerson.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
                modelUser.FuturePerson.address_Id = modelUser.FutureAddress == null ? 0 : modelUser.FutureAddress.Id;
                modelUser.FuturePerson.address_IdSpecified = true;
                modelUser.FuturePerson.birtdaySpecified = true;
                modelUser.FuturePerson.StatusSpecified = true;

                if (form.Education != "Təhsil Seçin")
                {
                    BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
                    modelUser.FuturePerson.educationLevel_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                    modelUser.FuturePerson.educationLevel_eV_IdSpecified = true;
                }

                if (form.Job != "İş seçin")
                {
                    BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
                    modelUser.FuturePerson.job_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                    modelUser.FuturePerson.job_eV_IdSpecified = true;
                }


                BaseOutput personOut = srv.WS_AddPerson(binput, modelUser.FuturePerson, out modelUser.FuturePerson);

            }


            if (form.MobilePhone != "")
            {
                modelUser.ComunicationInformations = new tblCommunication();
                BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelUser.EnumValue);
                modelUser.ComunicationInformations.comType = (int)modelUser.EnumValue.Id;
                modelUser.ComunicationInformations.comTypeSpecified = true;
                modelUser.ComunicationInformations.communication = form.mobilePhonePrefix + form.MobilePhone;
                modelUser.ComunicationInformations.description = form.mobilePhonePrefix + form.MobilePhone;
                modelUser.ComunicationInformations.PersonId = modelUser.FuturePerson.Id;
                modelUser.ComunicationInformations.PersonIdSpecified = true;
                modelUser.ComunicationInformations.priorty = 1;
                modelUser.ComunicationInformations.priortySpecified = true;
                BaseOutput comunicationnOUtt = srv.WS_AddCommunication(binput, modelUser.ComunicationInformations, out modelUser.ComunicationInformations);
            }

            TempData["personSignUp"] = "info";
            return SendUserInfos(modelUser.FutureUser.Username, form.Password, modelUser.FutureUser.Email);
        }


        public ActionResult AddOrganisationInfo(User UserInfo, string futureUserRole)
        {
            Session["arrONum"] = null;
            User modelUser = new Models.User();

            modelUser.AddressId = UserInfo.AddressId;
            modelUser.UserName = UserInfo.UserName;
            modelUser.Password = UserInfo.Password;
            modelUser.Pin = UserInfo.Pin;
            modelUser.uid = UserInfo.uid;
            modelUser.finvoenType = UserInfo.finvoenType;
            modelUser.FullAddress = UserInfo.FullAddress;
            modelUser.Email = UserInfo.Email;
            modelUser.MobilePhone = UserInfo.MobilePhone;
            modelUser.mobilePhonePrefix = UserInfo.mobilePhonePrefix;

            BaseOutput adminunits = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
            modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == 0).ToList();

            BaseOutput enumcatEd = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelUser.EnumCategory);

            if (modelUser.EnumCategory == null)
            {
                modelUser.EnumCategory = new tblEnumCategory();
            }
            BaseOutput enumvalEd = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);

            modelUser.EducationList = modelUser.EnumValueArray.ToList();

            BaseOutput enumcatJob = srv.WS_GetEnumCategorysByName(binput, "İş-Təşkilat", out modelUser.EnumCategory);
            if (modelUser.EnumCategory == null)
            {
                modelUser.EnumCategory = new tblEnumCategory();
            }
            BaseOutput enumvalJob = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.JobList = modelUser.EnumValueArray.ToList();

            if (UserInfo.uid != null)
            {
                BaseOutput finvoenOrgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, Convert.ToInt64(UserInfo.uid), true, out modelUser.ForeignOrganisation);
                BaseOutput finvoenUserOut = srv.WS_GetUserById(binput, (long)modelUser.ForeignOrganisation.userId, true, out modelUser.FutureUser);
                var managerId = modelUser.ForeignOrganisation.manager_Id == null ? 0 : modelUser.ForeignOrganisation.manager_Id;
                BaseOutput finvoenManagerOut = srv.WS_GetPersonById(binput, (long)managerId, true, out modelUser.FuturePerson);
                modelUser.ManagerName = modelUser.FuturePerson == null ? null : modelUser.FuturePerson.Name;
                modelUser.FatherName = modelUser.FuturePerson == null ? null : modelUser.FuturePerson.FatherName;
                modelUser.Surname = modelUser.FuturePerson == null ? null : modelUser.FuturePerson.Surname;
                modelUser.Name = modelUser.ForeignOrganisation == null ? null : modelUser.ForeignOrganisation.name;
            }

            return View("AddOrganisationInfo", modelUser);
        }


        [HttpPost]
        public ActionResult AddOrganisationInfo(User form)
        {
            binput = new BaseInput();
            User modelUser = new User();
            modelUser.FutureAddress = new tblAddress();

            string orgRoles;
            if (form.uid != null)
            {
                BaseOutput orgOut = srv.WS_GetForeign_OrganizationByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.ForeignOrganisation);
                BaseOutput userWithFinOut = srv.WS_GetUserById(binput, (long)modelUser.ForeignOrganisation.userId, true, out modelUser.FutureUser);
                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);

                BaseOutput updateUser = srv.WS_UpdateUser(binput, modelUser.FutureUser, out modelUser.FutureUser);
                orgRoles = "producerPerson";
            }

            else
            {
                modelUser.FutureUser = new tblUser();
                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
                modelUser.FutureUser.Status = 1;

                BaseOutput pgovEnumOut = srv.WS_GetEnumValueByName(binput, "legalPerson", out modelUser.EnumValue);

                modelUser.FutureUser.userType_eV_ID = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                modelUser.FutureUser.userType_eV_IDSpecified = true;
                modelUser.FutureUser.StatusSpecified = true;
                BaseOutput userOut = srv.WS_AddUser(binput, modelUser.FutureUser, out modelUser.FutureUser);
                orgRoles = "sellerPerson";
            }
            modelUser.ForeignOrganisation = new tblForeign_Organization();
            modelUser.UserRole = new tblUserRole();
            modelUser.ThroughfarePrm = new tblPRM_Thoroughfare();

            string[] userRoles = orgRoles.Split(',');

            foreach (var item in userRoles)
            {
                BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, item, out modelUser.Role);

                modelUser.UserRole.RoleId = modelUser.Role.Id;
                modelUser.UserRole.RoleIdSpecified = true;
                modelUser.UserRole.UserId = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
                modelUser.UserRole.UserIdSpecified = true;
                modelUser.UserRole.Status = 1;
                modelUser.UserRole.StatusSpecified = true;
                BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);
            }

            //add address of orgaisation

            modelUser.FutureAddress.fullAddress = form.FullAddress;
            modelUser.FutureAddress.Status = 1;
            modelUser.FutureAddress.StatusSpecified = true;
            modelUser.FutureAddress.user_Id = modelUser.FutureUser.Id;
            modelUser.FutureAddress.user_IdSpecified = true;
            modelUser.FutureAddress.user_type_eV_IdSpecified = true;

            modelUser.FutureAddress.adminUnit_Id = form.AddressId;

            modelUser.FutureAddress.adminUnit_IdSpecified = true;
            BaseOutput address = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);

            //add manager
            if (form.uid != null)
            {
                BaseOutput orgOut = srv.WS_GetForeign_OrganizationByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.ForeignOrganisation);
                var managerId = modelUser.ForeignOrganisation.manager_Id == null ? 0 : modelUser.ForeignOrganisation.manager_Id;
                BaseOutput personOrgOut = srv.WS_GetPersonById(binput, (long)managerId, true, out modelUser.Manager);
                modelUser.Manager.UserId = modelUser.FutureUser.Id;
                modelUser.Manager.UserIdSpecified = true;
                BaseOutput updateManager = srv.WS_UpdatePerson(binput, modelUser.Manager, out modelUser.Manager);
            }
            else
            {
                modelUser.Manager = new tblPerson();
                modelUser.Manager.Name = form.ManagerName;

                modelUser.Manager.PinNumber = form.Pin;
                modelUser.Manager.FatherName = form.FatherName;
                modelUser.Manager.Surname = form.UserName;
                modelUser.Manager.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
                modelUser.Manager.birtdaySpecified = true;
                modelUser.Manager.gender = form.Gender;

                BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
                modelUser.Manager.educationLevel_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                modelUser.Manager.educationLevel_eV_IdSpecified = true;
                BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
                modelUser.Manager.job_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
                modelUser.Manager.job_eV_IdSpecified = true;

                modelUser.Manager.Status = 1;
                modelUser.Manager.StatusSpecified = true;

                BaseOutput managerOut = srv.WS_AddPerson(binput, modelUser.Manager, out modelUser.Manager);
            }


            //add foreign organisation

            if (form.uid != null)
            {
                BaseOutput orgOut = srv.WS_GetForeign_OrganizationByUserId(binput, Convert.ToInt64(form.uid), true, out modelUser.ForeignOrganisation);
                modelUser.ForeignOrganisation.name = form.Name;
                modelUser.ForeignOrganisation.address_Id = modelUser.FutureAddress == null ? 0 : modelUser.FutureAddress.Id;

                BaseOutput updateFOrOrg = srv.WS_UpdateForeign_Organization(binput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);
            }
            else
            {
                modelUser.ForeignOrganisation.name = form.Name;
                modelUser.ForeignOrganisation.Status = 1;

                modelUser.ForeignOrganisation.address_Id = modelUser.FutureAddress == null ? 0 : modelUser.FutureAddress.Id;
                modelUser.ForeignOrganisation.address_IdSpecified = true;

                modelUser.ForeignOrganisation.userId = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
                modelUser.ForeignOrganisation.userIdSpecified = true;

                modelUser.ForeignOrganisation.voen = form.Voen;
                modelUser.ForeignOrganisation.manager_Id = modelUser.Manager == null ? 0 : modelUser.Manager.Id;

                modelUser.ForeignOrganisation.manager_IdSpecified = true;

                BaseOutput foreignOrganisationOut = srv.WS_AddForeign_Organization(binput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);
            }


            if (form.MobilePhone != null)
            {
                modelUser.ComunicationInformations = new tblCommunication();
                BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelUser.EnumValue);
                modelUser.ComunicationInformations.comType = (int)modelUser.EnumValue.Id;
                modelUser.ComunicationInformations.comTypeSpecified = true;
                modelUser.ComunicationInformations.communication = form.mobilePhonePrefix + form.MobilePhone;
                modelUser.ComunicationInformations.description = form.mobilePhonePrefix + form.MobilePhone;
                modelUser.ComunicationInformations.PersonId = modelUser.Manager.Id;
                modelUser.ComunicationInformations.PersonIdSpecified = true;
                modelUser.ComunicationInformations.priorty = 1;
                modelUser.ComunicationInformations.priortySpecified = true;
                BaseOutput comunicationnOUtt = srv.WS_AddCommunication(binput, modelUser.ComunicationInformations, out modelUser.ComunicationInformations);
            }

            return SendUserInfos(modelUser.FutureUser.Username, form.Password, modelUser.FutureUser.Email);
        }

        public ActionResult SendUserInfos(string userName, string password, string email)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("ferid.heziyev@gmail.com", "emsal.az");
                if (String.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".com"))
                {
                    email = "ferid.heziyev@gmail.com";
                }
                msg.To.Add(email);
                //string fromPassword = "e1701895";
                msg.Subject = "Üzvlüyü tesdiqle";

                msg.Body = "<p>İstifadəçi adınız:" + userName + "</p>" +
                           "<p>Şifrəniz:" + password + "</p>" +
                           "<p>http://localhost:56557/Login</p>";

                msg.IsBodyHtml = true;

                Mail.SendMail(msg);                   

                return RedirectToAction("Index", "Login");
            }
            catch (Exception err)
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult GeneratePDF(string Username, string password)
        {
            binput = new BaseInput();

            User modelUser = new User();

            List<string> userTypesList = new List<string>();

            srv.WS_GetUserByUserName(binput, Username, out modelUser.FutureUser);
            srv.WS_GetPersonByUserId(binput, modelUser.FutureUser.Id, true, out modelUser.FuturePerson);

            srv.WS_GetUserRolesByUserId(binput, modelUser.FutureUser.Id, true, out modelUser.UserRolesArray);

            if (modelUser.UserRolesArray.Length > 1)
            {
                foreach (var item in modelUser.UserRolesArray)
                {
                    srv.WS_GetRoleById(binput, item.RoleId, true, out modelUser.Role);

                    userTypesList.Add(modelUser.Role.Name);
                }
                modelUser.UserRoleType = string.Join(",", userTypesList);
            }

            else
            {
                srv.WS_GetRoleById(binput, modelUser.UserRolesArray[0].RoleId, true, out modelUser.Role);
                modelUser.UserRoleType = modelUser.Role.Name;
            }


            modelUser.Password = password;
            return new Rotativa.ViewAsPdf("Contract", modelUser);
        }


        public JsonResult GetThroughfaresByAdminUnitId(long Id)
        {
            binput = new BaseInput();

            User modelUser = new User();

            srv.WS_GetPRM_ThoroughfaresByAdminUnitId(binput, (int)Id, true, out modelUser.PRMThroughfareArray);

            modelUser.PRMThroughfareList = modelUser.PRMThroughfareArray.ToList();
            return Json(modelUser.PRMThroughfareList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AdminUnit(long? adminUnitId, int pId = 0)
        {
            binput = new BaseInput();

            SpecialSummaryViewModel modelUser = new SpecialSummaryViewModel();

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);

     
            modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            if (adminUnitId != null && adminUnitId != 0)
            {

                List<long> ids = new List<long>();


                BaseOutput adminUnittOut = srv.WS_GetPRM_AdminUnitById(binput, (long)adminUnitId, true, out modelUser.PRMAdminUnit);
                ids.Add(modelUser.PRMAdminUnit.Id);

                while (modelUser.PRMAdminUnit.ParentID != 0)
                {
                    srv.WS_GetPRM_AdminUnitById(binput, (long)modelUser.PRMAdminUnit.ParentID, true, out modelUser.PRMAdminUnit);
                    ids.Add(modelUser.PRMAdminUnit.Id);
                }

                modelUser.GivenAdminUnitIds = new List<long>();

                modelUser.GivenAdminUnitIds.Add(0);

                for (int i = ids.Count - 1; i >= 0; i--)
                {
                    modelUser.GivenAdminUnitIds.Add(ids[i]);
                }

                modelUser.PRMAdminUnitArrayPa = new IList<tblPRM_AdminUnit>[modelUser.GivenAdminUnitIds.Count() - 1];

                int s = 0;
                foreach (long itm in modelUser.GivenAdminUnitIds)
                {
                    BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(binput, (int)itm, true, out modelUser.PRMAdminUnitArray);
                    modelUser.PRMAdminUnitArrayPa[s] = modelUser.PRMAdminUnitArray.ToList();

                    s = s + 1;
                    if (s == modelUser.GivenAdminUnitIds.Count() - 1)
                    {
                        break;
                    }
                }

            }


            if (adminUnitId > 0)
            {
                BaseOutput gpa = srv.WS_GetAddressById(binput, (long)adminUnitId, true, out modelUser.Address);
                modelUser.Address = new tblAddress();

                     BaseOutput galf = srv.WS_GetAdminUnitListForID(binput, (long)adminUnitId, true, out modelUser.PRMAdminUnitArray);
                    modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();
                    modelUser.Address.fullAddress = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Id));


                    modelUser.Address.fullAddress = "0," + modelUser.Address.fullAddress;
                    modelUser.addressIds = modelUser.Address.fullAddress.Split(',').Select(long.Parse).ToArray();

                    modelUser.PRMAdminUnitArrayPa = new IList<tblPRM_AdminUnit>[modelUser.addressIds.Count()];
                    int s = 0;
                    foreach (long itm in modelUser.addressIds)
                    {
                        BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(binput, (int)itm, true, out modelUser.PRMAdminUnitArray);
                        modelUser.PRMAdminUnitArrayPa[s] = modelUser.PRMAdminUnitArray.ToList();
                        s = s + 1;
                    }

                    if (modelUser.PRMAdminUnitArrayPa[s - 1].Count() > 0)
                    {
                        modelUser.Address.fullAddress = modelUser.Address.fullAddress + ",0";
                        modelUser.addressIds = modelUser.Address.fullAddress.Split(',').Select(long.Parse).ToArray();
                    }
            }

            if (Session["arrONum"] == null)
            {
                Session["arrONum"] = modelUser.arrNum;
            }
            else
            {
                modelUser.arrNum = (int)Session["arrONum"] + 1;
                Session["arrONum"] = modelUser.arrNum;
            }

            return View(modelUser);
        }
    }
}
