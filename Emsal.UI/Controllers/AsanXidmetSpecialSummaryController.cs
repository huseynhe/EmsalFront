using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.asan)]

    public class AsanXidmetSpecialSummaryController : Controller
    {
        //
        // GET: /GovernmentOrganisation/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        private BaseInput binput;
        SpecialSummaryViewModel modelUser;

        public ActionResult Index(long? UserId)
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.LoggedInUser);
            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();
            return View(modelUser);
        }

        public ActionResult PotentialSellers(int? userId)
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("SpecialLogin");
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


            if (modelUser.FutureUser != null || UserFromEmail.Count != 0 || PersonFromPin.Count != 0 || OrgFromVoen.Count != 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public ActionResult AddPotentialClient()
        {
            User modelUser = new User();
            BaseOutput enumCatOut = srv.WS_GetEnumCategorysByName(binput, "UserType", out modelUser.EnumCategory);

            BaseOutput userTypesOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);

            modelUser.UserTypeList = modelUser.EnumValueArray.ToList();

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult AddPotentialClient(User form)
        {
            if (CheckExistence(form))
            {
                binput = new BaseInput();

                User modelUser = new User();


                modelUser.FutureUser = new tblUser();
                modelUser.UserRole = new tblUserRole();

                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);

                modelUser.UserRoleType = "Potensial İstehsalçı";

                modelUser.Password = form.Password;
                modelUser.Pin = form.Pin;

                modelUser.UserType = form.UserType;
                modelUser.Voen = form.Voen;
                return AddAddressInfo(modelUser.FutureUser, "producerPerson", modelUser.Pin, modelUser.Voen);
            }
            else
            {
                TempData["UserAsanPotSignUpError"] = "info";
                return RedirectToAction("AddPotentialClient");
            }
        }

        public ActionResult AddPotentialSeller()
        {
            User modelUser = new User();
            BaseOutput enumCatOut = srv.WS_GetEnumCategorysByName(binput, "UserType", out modelUser.EnumCategory);

            BaseOutput userTypesOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);

            modelUser.UserTypeList = modelUser.EnumValueArray.ToList();

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult AddPotentialSeller(User form)
        {
            if (CheckExistence(form))
            {
                binput = new BaseInput();

                User modelUser = new User();


                modelUser.FutureUser = new tblUser();
                modelUser.UserRole = new tblUserRole();

                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);

                modelUser.UserRoleType = "Potensial Satıcı";

                modelUser.Password = form.Password;
                return AddAddressInfo(modelUser.FutureUser, "sellerPerson", form.Pin, form.Voen);
            }
            else
            {
                TempData["UserAsanSelSignUpError"] = "info";
                return RedirectToAction("AddPotentialSeller");
            }
        }

        public ActionResult AddPotentialClientAndSeller()
        {
            User modelUser = new User();

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult AddPotentialClientAndSeller(User form)
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

                modelUser.UserRoleType = "Potensial Satıcı, Potensial Müştəri";

                modelUser.Password = form.Password;

                return AddAddressInfo(modelUser.FutureUser, "producerPerson,sellerPerson", modelUser.Pin, form.Voen);
            }
            else
            {
                return View();
            }

        }


        public ActionResult AddAddressInfo(tblUser FutureUser, string UserRolesStr, string pin, string voen)
        {
            Session["arrONum"] = null;

            User modelUser = new User();

            modelUser.Password = FutureUser.Password;

            modelUser.UserName = FutureUser.Username;

            modelUser.Email = FutureUser.Email;

            modelUser.FutureUserRole = UserRolesStr;

            modelUser.Pin = pin;
            modelUser.Voen = voen;
            return View("AddAddressInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddAddressInfo(User form)
        {

            binput = new BaseInput();
            User modelUser = new User();

            modelUser.FutureAddress = new tblAddress();

            BaseOutput userrOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);

            modelUser.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();
            modelUser.FutureAddress.fullAddress = form.FullAddress;

            modelUser.UserName = form.UserName;
            modelUser.AddressId = (long)modelUser.FutureAddress.adminUnit_Id;
            modelUser.Pin = form.Pin;
            modelUser.Email = form.Email;

            if (form.Voen == null)
            {
                return AddPersonInfo(modelUser.UserName, modelUser.AddressId, form.Password, modelUser.Email, modelUser.Pin, modelUser.FutureAddress.fullAddress, form.FutureUserRole);
            }
            else
            {
                return AddOrganisationInfo(modelUser.UserName, modelUser.AddressId, form.Password, modelUser.Email, modelUser.Pin, modelUser.FutureAddress.fullAddress, form.FutureUserRole);
            }
        }

        public ActionResult AddPersonInfo(string userName, long AddressId, string password, string email, string Pin, string fullAddress, string futureUserRole)
        {
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

            modelUser.AddressId = AddressId;
            modelUser.UserName = userName;
            modelUser.Password = password;
            modelUser.Pin = Pin;

            modelUser.FullAddress = fullAddress;

            return View("AddPersonInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddPersonInfo(User form)
        {
            binput = new BaseInput();

            User modelUser = new User();
            modelUser.FuturePerson = new tblPerson();
            modelUser.FutureUser = new tblUser();
            modelUser.UserRole = new tblUserRole();
            modelUser.FutureAddress = new tblAddress();
            //adding future user
            modelUser.FutureUser.Username = form.UserName;
            modelUser.FutureUser.Email = form.Email;
            modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
            modelUser.FutureUser.Status = 1;

            BaseOutput personEnumOut = srv.WS_GetEnumValueByName(binput, "fizikişexs", out modelUser.EnumValue);
            modelUser.FutureUser.userType_eV_ID = modelUser.EnumValue.Id;

            modelUser.FutureUser.userType_eV_IDSpecified = true;
            modelUser.FutureUser.StatusSpecified = true;

            //created user
            FormsIdentity identity = (FormsIdentity)User.Identity;
            BaseOutput adminOut = srv.WS_GetUserById(binput, Convert.ToInt32(identity.Ticket.UserData), true, out modelUser.CreatedUser);
            binput.userName = modelUser.CreatedUser.Username;

            modelUser.FutureUser.createdUser = modelUser.CreatedUser.Username;
            BaseOutput userOut = srv.WS_AddUser(binput, modelUser.FutureUser, out modelUser.FutureUser);

            //adding user role
            string[] userRoles = form.FutureUserRole.Split(',');

            foreach (var item in userRoles)
            {
                BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, item, out modelUser.Role);

                modelUser.UserRole.RoleId = modelUser.Role.Id;
                modelUser.UserRole.RoleIdSpecified = true;
                modelUser.UserRole.UserId = modelUser.FutureUser.Id;
                modelUser.UserRole.UserIdSpecified = true;
                modelUser.UserRole.Status = 1;
                modelUser.UserRole.StatusSpecified = true;
                BaseOutput addUserProducerRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);

            }

            //adding address

            modelUser.FutureAddress.fullAddress = form.FullAddress;
            modelUser.FutureAddress.Status = 1;
            modelUser.FutureAddress.StatusSpecified = true;
            modelUser.FutureAddress.user_Id = modelUser.FutureUser.Id;
            modelUser.FutureAddress.user_IdSpecified = true;
            modelUser.FutureAddress.user_type_eV_IdSpecified = true;

           
            modelUser.FutureAddress.adminUnit_Id = form.AddressId;
            modelUser.FutureAddress.adminUnit_IdSpecified = true;

            BaseOutput addressOut = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);

            //adding person
            modelUser.FuturePerson.UserId = modelUser.FutureUser.Id;
            modelUser.FuturePerson.UserIdSpecified = true;
            modelUser.FuturePerson.Name = form.Name;
            modelUser.FuturePerson.FatherName = form.FatherName;
            modelUser.FuturePerson.Surname = form.Surname;
            modelUser.FuturePerson.PinNumber = form.Pin;
            modelUser.FuturePerson.gender = form.Gender;
            modelUser.FuturePerson.Status = 1;

            modelUser.FuturePerson.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
            modelUser.FuturePerson.address_Id = modelUser.FutureAddress.Id;
            modelUser.FuturePerson.address_IdSpecified = true;
            modelUser.FuturePerson.birtdaySpecified = true;
            modelUser.FuturePerson.StatusSpecified = true;

            if(form.Education!="Təhsil Seçin")
            {
                BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
                modelUser.FuturePerson.educationLevel_eV_Id = modelUser.EnumValue.Id;
                modelUser.FuturePerson.educationLevel_eV_IdSpecified = true;
            }
            if(form.Job != "İş seçin")
            {
                BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
                modelUser.FuturePerson.job_eV_Id = modelUser.EnumValue.Id;
                modelUser.FuturePerson.job_eV_IdSpecified = true;
            }

            BaseOutput personOut = srv.WS_AddPerson(binput, modelUser.FuturePerson, out modelUser.FuturePerson);
            modelUser.Password = form.Password;
            modelUser.FutureUser.Username = form.UserName;
            return View("Contract", modelUser);
        }


        public ActionResult AddOrganisationInfo(string userName, long AddressId, string password, string email, string Pin, string fullAddress, string futureUserRole)
        {
            ModelState.Clear();
            Session["arrONum"] = null;

            User modelUser = new Models.User();

            modelUser.AddressId = AddressId;
            modelUser.UserName = userName;
            modelUser.Password = password;
            modelUser.Pin = Pin;

            modelUser.FullAddress = fullAddress;
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
            return View("AddOrganisationInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddOrganisationInfo(User form)
        {
            
            binput = new BaseInput();

            User modelUser = new User();
            modelUser.ForeignOrganisation = new tblForeign_Organization();
            modelUser.FutureAddress = new tblAddress();
            modelUser.FutureUser = new tblUser();
            modelUser.UserRole = new tblUserRole();

            modelUser.FutureUser.Username = form.UserName;
            modelUser.FutureUser.Email = form.Email;
            modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
            modelUser.FutureUser.Status = 1;

            BaseOutput personEnumOut = srv.WS_GetEnumValueByName(binput, "legalPerson", out modelUser.EnumValue);

            modelUser.FutureUser.userType_eV_ID = modelUser.EnumValue.Id;
            modelUser.FutureUser.userType_eV_IDSpecified = true;
            modelUser.FutureUser.StatusSpecified = true;

            //add created user

            FormsIdentity identity = (FormsIdentity)User.Identity;
            BaseOutput adminOut = srv.WS_GetUserById(binput, Convert.ToInt32(identity.Ticket.UserData), true, out modelUser.CreatedUser);
            binput.userName = modelUser.CreatedUser.Username;
            modelUser.FutureUser.createdUser = modelUser.CreatedUser.Username;

            BaseOutput userOut = srv.WS_AddUser(binput, modelUser.FutureUser, out modelUser.FutureUser);

            string[] userRoles = form.FutureUserRole.Split(',');

            foreach (var item in userRoles)
            {
                BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, item, out modelUser.Role);

                modelUser.UserRole.RoleId = modelUser.Role.Id;
                modelUser.UserRole.RoleIdSpecified = true;
                modelUser.UserRole.UserId = modelUser.FutureUser.Id;
                modelUser.UserRole.UserIdSpecified = true;
                modelUser.UserRole.Status = 1;
                modelUser.UserRole.StatusSpecified = true;
                BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);

            }

            modelUser.FutureAddress.fullAddress = form.FullAddress;
            modelUser.FutureAddress.Status = 1;
            modelUser.FutureAddress.StatusSpecified = true;
            modelUser.FutureAddress.user_Id = modelUser.FutureUser.Id;
            modelUser.FutureAddress.user_IdSpecified = true;
            modelUser.FutureAddress.user_type_eV_IdSpecified = true;

            modelUser.FutureAddress.adminUnit_Id = modelUser.AddressId;
            modelUser.FutureAddress.adminUnit_IdSpecified = true;
            BaseOutput address = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);

            //add manager

            modelUser.Manager = new tblPerson();
            modelUser.Manager.Name = form.ManagerName;

            modelUser.Manager.PinNumber = form.Pin;
            modelUser.Manager.FatherName = form.FatherName;
            modelUser.Manager.Surname = form.UserName;
            modelUser.Manager.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
            modelUser.Manager.birtdaySpecified = true;
            modelUser.Manager.gender = form.Gender;

            if(form.Education != "Təhsil Seçin")
            {
                BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
                modelUser.Manager.educationLevel_eV_Id = modelUser.EnumValue.Id;
                modelUser.Manager.educationLevel_eV_IdSpecified = true;
            }
            
            if(form.Job != "İş seçin")
            {
                BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
                modelUser.Manager.job_eV_Id = modelUser.EnumValue.Id;
                modelUser.Manager.job_eV_IdSpecified = true;
            }

            modelUser.Manager.Status = 1;
            modelUser.Manager.StatusSpecified = true;

            BaseOutput managerOut = srv.WS_AddPerson(binput, modelUser.Manager, out modelUser.Manager);


            //add foreign organisation
            modelUser.ForeignOrganisation.name = form.Name;
            modelUser.ForeignOrganisation.Status = 1;

            modelUser.ForeignOrganisation.address_Id = modelUser.FutureAddress.Id;
            modelUser.ForeignOrganisation.address_IdSpecified = true;

            modelUser.ForeignOrganisation.userId = modelUser.FutureUser.Id;
            modelUser.ForeignOrganisation.userIdSpecified = true;

            modelUser.ForeignOrganisation.voen = form.Voen;
            modelUser.ForeignOrganisation.manager_Id = modelUser.Manager.Id;

            modelUser.ForeignOrganisation.manager_IdSpecified = true;

            BaseOutput foreignOrganisationOut = srv.WS_AddForeign_Organization(binput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);


            return RedirectToAction("Index", "AsanXidmetSpecialSummary");
        }


        public long ConvertStringYearMonthDayFormatToTimestamp(User form)
        {
            string[] dates = Regex.Split(form.Birthday, @"\.");
            int year = Convert.ToInt32(dates[2]);
            int month = Convert.ToInt32(dates[1]);
            int day = Convert.ToInt32(dates[0]);
            DateTime dTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long birthday = (long)(dTime - sTime).TotalSeconds;

            return birthday;
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

      

        public string CheckPassword(
        int? userId,
        string password,
        bool IdSpecified = true)
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();

            modelUser.User = new tblUser();

            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }
            modelUser.User.Id = (long)userId;

            modelUser.User.IdSpecified = true;
            modelUser.User.StatusSpecified = true;


            BaseOutput userGet = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
            if (password != null)
            {
                bool verify = BCrypt.Net.BCrypt.Verify(password, modelUser.User.Password);

                if (verify)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            else
            {
                return "false";
            }
        }

        public ActionResult ChangePassword(
   string userName,
   string name,
   string surname,
   int? userId,
   string email,
   string password,
   int? userType = null,
   bool IdSpecified = true
    )
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();



            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }

            try
            {
                BaseOutput userOutput = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
                modelUser.User.Username = modelUser.User.Username;
                modelUser.User.Id = modelUser.User.Id;
                modelUser.User.Email = modelUser.User.Email;
                modelUser.User.Password = BCrypt.Net.BCrypt.HashPassword(password, 5);
                modelUser.User.IdSpecified = true;
                BaseOutput pout = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }

        }

        public ActionResult ReceivedMessages(int? page, long? userId)
        {
            binput = new BaseInput();
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            modelUser = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.LoggedInUser);

            BaseOutput bout = srv.WS_GetComMessagesyByToUserId(binput, (long)userId, true, out modelUser.ComMessageArray);


            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();
            modelUser.PagingReceivedMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            modelUser.UserList = new List<tblUser>();

            foreach (var message in modelUser.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.fromUserID, true, out modelUser.User);
                modelUser.UserList.Add(modelUser.User);
            }
            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)userId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();


            BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.UserRoleArray);

            return View(modelUser);
        }

        public ActionResult SentMessages(int? page, long? userId)
        {
            binput = new BaseInput();
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            modelUser = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.LoggedInUser);

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserId(binput, (long)userId, true, out modelUser.ComMessageArray);

            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();
            modelUser.PagingSentMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            modelUser.UserList = new List<tblUser>();

            foreach (var message in modelUser.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.toUserID, true, out modelUser.User);
                modelUser.UserList.Add(modelUser.User);
            }
            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)userId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();

            BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.UserRoleArray);

            return View(modelUser);
        }
    }
}
