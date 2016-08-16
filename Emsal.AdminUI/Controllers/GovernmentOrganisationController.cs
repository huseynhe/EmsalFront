using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Text.RegularExpressions;
using Emsal.AdminUI.Infrastructure;
using System.Web.Security;

namespace Emsal.AdminUI.Controllers
{
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class GovernmentOrganisationController : Controller
    {
        //
        // GET: /AddGovernmentOrganisation/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        private BaseInput binput;
        Organisation modelUser;
        public ActionResult Index(int? page, long? UserId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            Organisation modelUser = new Organisation();
            //get roles by name gelecek bura
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            BaseOutput bouput = srv.WS_GetGovernmentOrganisations(binput, 12, true, out modelUser.UserArray);
            modelUser.UserList = modelUser.UserArray.ToList();

            modelUser.Paging = modelUser.UserList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndex", modelUser)
                : View(modelUser);
        }
        public ActionResult AddGovOrganisation(long?UserId)
        {
            modelUser = new Organisation();

            BaseOutput roles = srv.WS_GetRoles(binput, out modelUser.UserRoleArray);

            modelUser.UserRoleList = modelUser.UserRoleArray.ToList();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            return View(modelUser);
        }


        //child organisations part
        public ActionResult ChildOrganisations(long? UserId, long? orgId, long? orgUserId)
        {
            binput = new BaseInput();
            modelUser = new Organisation();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            if(orgUserId != null)
            {
                BaseOutput ForeignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, (long)orgUserId, true, out modelUser.ParentOrganisation);
            }
            if(orgId != null)
            {
                BaseOutput ForeignOrganissationOut = srv.WS_GetForeign_OrganizationById(binput, (long)orgId, true, out modelUser.ParentOrganisation);

            }
            long organisationId = (long)modelUser.ParentOrganisation.Id;


            BaseOutput organisationByParentId = srv.WS_GetForeign_OrganisationsByParentId(binput, (long)organisationId, true, out modelUser.ForeignOrganisationArray);



            return View(modelUser);
        }

        public JsonResult GetOrganisations(long? orgId, long? UserId)
        {
            binput = new BaseInput();
            modelUser = new Organisation();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.User);

            //BaseOutput ForeignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, (long)orgId, true, out modelUser.ParentOrganisation);
            long organisationId = (long)orgId;

            BaseOutput organisationByParentId = srv.WS_GetForeign_OrganisationsByParentId(binput, (long)organisationId, true, out modelUser.ForeignOrganisationArray);
            return Json(modelUser.ForeignOrganisationArray, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AddChildOrganisation(long? UserId, long? parentId)
        {
            binput = new BaseInput();

            Session["arrONum"] = null;
            modelUser = new Organisation();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            modelUser.ParentOrganisationId = (long)parentId;


            BaseOutput edcationCatOut = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelUser.EnumCategory);
            BaseOutput eductationOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.EducationList = modelUser.EnumValueArray.ToList();

            BaseOutput jobCatOut = srv.WS_GetEnumCategorysByName(binput, "İş-Təşkilat", out modelUser.EnumCategory);
            BaseOutput jobbOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.JobList = modelUser.EnumValueArray.ToList();

            return View("AddChildOrganisation", modelUser);
        }

        [HttpPost]
        public ActionResult AddChildOrganisation(long? UserId, Organisation form)
        {
            if (CheckExistence(form))
            {
                binput = new BaseInput();
                modelUser = new Organisation();

                modelUser.User = new tblUser();
                modelUser.UserRole = new tblUserRole();
                modelUser.FutureAddress = new tblAddress();
                modelUser.ForeignOrganisation = new tblForeign_Organization();

                modelUser.User.Username = form.UserName;
                modelUser.User.Email = form.Email;
                modelUser.User.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
                modelUser.User.Status = 1;

                BaseOutput personEnumOut = srv.WS_GetEnumValueByName(binput, "legalPerson", out modelUser.EnumValue);
                modelUser.User.userType_eV_ID = modelUser.EnumValue.Id;

                modelUser.User.userType_eV_IDSpecified = true;
                modelUser.User.StatusSpecified = true;


                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

                binput.userName = modelUser.Admin.Username;
                BaseOutput userOut = srv.WS_AddUser(binput, modelUser.User, out modelUser.User);


                //give roles to user
                BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);

                modelUser.UserRole.RoleId = modelUser.Role.Id;
                modelUser.UserRole.RoleIdSpecified = true;
                modelUser.UserRole.UserId = modelUser.User.Id;
                modelUser.UserRole.UserIdSpecified = true;
                modelUser.UserRole.Status = 1;
                modelUser.UserRole.StatusSpecified = true;
                BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);


                //add address informations
                modelUser.FutureAddress.fullAddress = form.FullAddress;
                modelUser.FutureAddress.Status = 1;
                modelUser.FutureAddress.StatusSpecified = true;
                modelUser.FutureAddress.user_Id = modelUser.User.Id;
                modelUser.FutureAddress.user_IdSpecified = true;
                modelUser.FutureAddress.user_type_eV_IdSpecified = true;
                modelUser.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();

                modelUser.FutureAddress.adminUnit_IdSpecified = true;
                BaseOutput address = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);


                //add manager

                modelUser.Manager = new tblPerson();
                modelUser.Manager.Name = form.ManagerName;

                modelUser.Manager.PinNumber = form.Pin;
                modelUser.Manager.FatherName = form.FatherName;
                modelUser.Manager.Surname = form.Surname;
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

                //add manager communication informations
                if (form.ManagerEmail != null)
                {
                    modelUser.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "email", out modelUser.EnumValue);
                    modelUser.ComunicationInformations.comType = (int)modelUser.EnumValue.Id;
                    modelUser.ComunicationInformations.comTypeSpecified = true;
                    modelUser.ComunicationInformations.communication = form.ManagerEmail;
                    modelUser.ComunicationInformations.description = form.ManagerEmail;
                    modelUser.ComunicationInformations.PersonId = modelUser.Manager.Id;
                    modelUser.ComunicationInformations.PersonIdSpecified = true;
                    modelUser.ComunicationInformations.priorty = 1;
                    modelUser.ComunicationInformations.priortySpecified = true;

                    BaseOutput comunicationOUtt = srv.WS_AddCommunication(binput, modelUser.ComunicationInformations, out modelUser.ComunicationInformations);
                }


                if (form.ManagerMobilePhone != null)
                {
                    modelUser.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelUser.EnumValue);
                    modelUser.ComunicationInformations.comType = (int)modelUser.EnumValue.Id;
                    modelUser.ComunicationInformations.comTypeSpecified = true;
                    modelUser.ComunicationInformations.communication = form.ManagerMobilePhone;
                    modelUser.ComunicationInformations.description = form.ManagerMobilePhone;
                    modelUser.ComunicationInformations.PersonId = modelUser.Manager.Id;
                    modelUser.ComunicationInformations.PersonIdSpecified = true;
                    modelUser.ComunicationInformations.priorty = 2;
                    modelUser.ComunicationInformations.priortySpecified = true;
                    BaseOutput comunicationOUt = srv.WS_AddCommunication(binput, modelUser.ComunicationInformations, out modelUser.ComunicationInformations);

                }

                if (form.ManagerHomePhone != null)
                {
                    modelUser.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "homePhone", out modelUser.EnumValue);
                    modelUser.ComunicationInformations.comType = (int)modelUser.EnumValue.Id;
                    modelUser.ComunicationInformations.comTypeSpecified = true;
                    modelUser.ComunicationInformations.communication = form.ManagerHomePhone;
                    modelUser.ComunicationInformations.description = form.ManagerHomePhone;
                    modelUser.ComunicationInformations.PersonId = modelUser.Manager.Id;
                    modelUser.ComunicationInformations.PersonIdSpecified = true;
                    modelUser.ComunicationInformations.priorty = 2;
                    modelUser.ComunicationInformations.priortySpecified = true;
                    BaseOutput comunicatioOUtt = srv.WS_AddCommunication(binput, modelUser.ComunicationInformations, out modelUser.ComunicationInformations);

                }

                if (form.ManagerWorkPhone != null)
                {
                    modelUser.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelUser.EnumValue);
                    modelUser.ComunicationInformations.comType = (int)modelUser.EnumValue.Id;
                    modelUser.ComunicationInformations.comTypeSpecified = true;
                    modelUser.ComunicationInformations.communication = form.ManagerWorkPhone;
                    modelUser.ComunicationInformations.description = form.ManagerWorkPhone;
                    modelUser.ComunicationInformations.PersonId = modelUser.Manager.Id;
                    modelUser.ComunicationInformations.PersonIdSpecified = true;
                    modelUser.ComunicationInformations.priorty = 1;
                    modelUser.ComunicationInformations.priortySpecified = true;
                    BaseOutput comunicationnOUtt = srv.WS_AddCommunication(binput, modelUser.ComunicationInformations, out modelUser.ComunicationInformations);

                }
                //add foreign organisation
                modelUser.ForeignOrganisation.name = form.Name;
                modelUser.ForeignOrganisation.Status = 1;

                modelUser.ForeignOrganisation.address_Id = modelUser.FutureAddress.Id;
                modelUser.ForeignOrganisation.address_IdSpecified = true;

                modelUser.ForeignOrganisation.userId = modelUser.User.Id;
                modelUser.ForeignOrganisation.userIdSpecified = true;

                modelUser.ForeignOrganisation.voen = form.Voen;
                modelUser.ForeignOrganisation.manager_Id = modelUser.Manager.Id;

                modelUser.ForeignOrganisation.manager_IdSpecified = true;

                modelUser.ForeignOrganisation.parent_Id = form.ParentOrganisationId;
                modelUser.ForeignOrganisation.parent_IdSpecified = true;

                BaseOutput foreignOrganisationOut = srv.WS_AddForeign_Organization(binput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);


                return RedirectToAction("Index");
            }
            else
            {
                TempData["ChildOrganisationExists"] = "Bu adda istifadəçi sistemdə mövcuddur.";
                return RedirectToAction("AddChildOrganisation", new { parentId = form.ParentOrganisationId });
            }

        }

        public ActionResult EditChildOrganisation(long? UserId, long? Id)
        {
            binput = new BaseInput();

            Session["arrONum"] = null;
            modelUser = new Organisation();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.User);

            modelUser.OrganisationId = (long)Id;
            //get the orgaisation  that is going to be updated
            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, (long)Id, true, out modelUser.ForeignOrganisation);
            modelUser.Voen = modelUser.ForeignOrganisation.voen;
            modelUser.Name = modelUser.ForeignOrganisation.name;

            //get the user registered to the organisation
            BaseOutput orgOut = srv.WS_GetUserById(binput, (long)modelUser.ForeignOrganisation.userId, true, out modelUser.User);
            modelUser.UserName = modelUser.User.Username;
            //modelUser.Password = modelUser.User.Password;
            modelUser.Email = modelUser.User.Email;

            //get the address of the organisation
            //this is biased. we do not know if an organisation can have more than one address.***********************
            BaseOutput addressOut = srv.WS_GetAddressById(binput, (long)modelUser.ForeignOrganisation.address_Id, true, out modelUser.FutureAddress);
            modelUser.FullAddress = modelUser.FutureAddress.fullAddress;
            //*********************************************************


            //get the address hierarchy this is also biased *************************
            //BaseOutput adminUnitOut = srv.WS_GetPRM_AdminUnitById(binput, (long)modelUser.AddressArray.FirstOrDefault().adminUnit_Id, true, out modelUser.PRMAdminUnit);
            //BaseOutput addressesOut = srv.WS_GETPRM_AdminUnitsByChildId(binput, modelUser.PRMAdminUnit, out modelUser.PRMAdminUnitArray);

            BaseOutput edcationCatOut = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelUser.EnumCategory);
            BaseOutput eductationOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.EducationList = modelUser.EnumValueArray.ToList();


            BaseOutput jobCatOut = srv.WS_GetEnumCategorysByName(binput, "İş-Təşkilat", out modelUser.EnumCategory);
            BaseOutput jobbOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.JobList = modelUser.EnumValueArray.ToList();

            //get manager infos
            BaseOutput ManagerOut = srv.WS_GetPersonById(binput, (long)modelUser.ForeignOrganisation.manager_Id, true, out modelUser.Manager);
            modelUser.ManagerName = modelUser.Manager.Name;
            modelUser.Surname = modelUser.Manager.Surname;
            modelUser.FatherName = modelUser.Manager.FatherName;
            modelUser.Pin = modelUser.Manager.PinNumber;

            //get communication informations of the manager
            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelUser.CommunicationInformationsArray);
            modelUser.CommunicationInformationsList = modelUser.CommunicationInformationsArray.Where(x => x.PersonId == modelUser.Manager.Id).ToList();


            //get emailtype enum value
            BaseOutput emailEnumOut = srv.WS_GetEnumValueByName(binput, "email", out modelUser.EnumValue);
            modelUser.ManagerEmail = modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? null : modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;

            //get HomePhone type enum value
            BaseOutput homePhoneOut = srv.WS_GetEnumValueByName(binput, "homePhone", out modelUser.EnumValue);
            modelUser.ManagerHomePhone = modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? null : modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;

            //get MobilePhone type enum value
            BaseOutput mobilePhoneOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelUser.EnumValue);
            modelUser.ManagerMobilePhone = modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? null : modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;

            //get WorkPhone type enum value
            BaseOutput workPhoneOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelUser.EnumValue);
            modelUser.ManagerWorkPhone = modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? null : modelUser.CommunicationInformationsList.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;


            //get the gender
            modelUser.Gender = modelUser.Manager.gender;
            modelUser.ManagerEducation = (long)modelUser.Manager.educationLevel_eV_Id;
            modelUser.ManagerJob = (long)modelUser.Manager.job_eV_Id;

            //birthday not finished
            //modelUser.Birthday = modelUser.Manager.birtday;

            BaseOutput adminnOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            return View("EditChildOrganisation", modelUser);
        }

        [HttpPost]
        public ActionResult EditChildOrganisation(long? UserId, Organisation form, long OrganisationId)
        {
            binput = new BaseInput();
            modelUser = new Organisation();

            modelUser.User = new tblUser();
            modelUser.FutureAddress = new tblAddress();
            modelUser.ForeignOrganisation = new tblForeign_Organization();

            //update user

            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, OrganisationId, true, out modelUser.ForeignOrganisation);
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)modelUser.ForeignOrganisation.userId, true, out modelUser.User);

            modelUser.User.Username = form.UserName;
            modelUser.User.Email = form.Email;
            modelUser.User.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);


            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            binput.userName = modelUser.Admin.Username;
            BaseOutput userUpdate = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);

            //update address
            BaseOutput addressOUT = srv.WS_GetAddressById(binput, (long)modelUser.ForeignOrganisation.address_Id, true, out modelUser.FutureAddress);
            modelUser.FutureAddress.fullAddress = form.FullAddress;
            modelUser.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();
            BaseOutput address = srv.WS_UpdateAddress(binput, modelUser.FutureAddress);


            //update manager
            BaseOutput managerOut = srv.WS_GetPersonById(binput, (long)modelUser.ForeignOrganisation.manager_Id, true, out modelUser.Manager);

            modelUser.Manager.Name = form.ManagerName;
            modelUser.Manager.PinNumber = form.Pin;
            modelUser.Manager.FatherName = form.FatherName;
            modelUser.Manager.Surname = form.Surname;
            modelUser.Manager.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
            modelUser.Manager.gender = form.Gender;

            BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
            modelUser.Manager.educationLevel_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;

            BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
            modelUser.Manager.job_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;


            BaseOutput updatemanager = srv.WS_UpdatePerson(binput, modelUser.Manager, out modelUser.Manager);

            //update manager communication informations

            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelUser.CommunicationInformationsArray);
            modelUser.CommunicationInformationsList = modelUser.CommunicationInformationsArray.Where(x => x.PersonId == modelUser.Manager.Id).ToList();

            foreach (var item in modelUser.CommunicationInformationsList)
            {
                BaseOutput emailEnumOut = srv.WS_GetEnumValueByName(binput, "email", out modelUser.EnumValue);
                if (item.comType == modelUser.EnumValue.Id)
                {
                    item.communication = form.ManagerEmail;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelUser.ComunicationInformations);
                }

                BaseOutput homePhoneOut = srv.WS_GetEnumValueByName(binput, "homePhone", out modelUser.EnumValue);
                if (item.comType == modelUser.EnumValue.Id)
                {
                    item.communication = form.ManagerHomePhone;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelUser.ComunicationInformations);
                }

                BaseOutput mobilePhoneOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelUser.EnumValue);
                if (item.comType == modelUser.EnumValue.Id)
                {
                    item.communication = form.ManagerMobilePhone;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelUser.ComunicationInformations);
                }
                BaseOutput workPhoneOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelUser.EnumValue);
                if (item.comType == modelUser.EnumValue.Id)
                {
                    item.communication = form.ManagerWorkPhone;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelUser.ComunicationInformations);
                }
            }
            //update address
            BaseOutput addressOut = srv.WS_GetAddressById(binput, (long)modelUser.ForeignOrganisation.address_Id, true, out modelUser.FutureAddress);
            modelUser.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();


            //update foreign organisation
            modelUser.ForeignOrganisation.name = form.Name;
            modelUser.ForeignOrganisation.voen = form.Voen;

            BaseOutput updateOrganisation = srv.WS_UpdateForeign_Organization(binput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);


            return RedirectToAction("Index", "GovernmentOrganisation");
        }

        public ActionResult OrganisationInfo(long? UserId, long? Id)
        {
            binput = new BaseInput();
            modelUser = new Organisation();

            //get the organisation
            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, (long)Id, true, out modelUser.ForeignOrganisation);
            modelUser.Name = modelUser.ForeignOrganisation.name;

            //get the manager
            BaseOutput managerOut = srv.WS_GetPersonById(binput, (long)modelUser.ForeignOrganisation.manager_Id, true, out modelUser.Manager);
            modelUser.ManagerName = modelUser.Manager.Name;
            modelUser.Surname = modelUser.Manager.Surname;
            //get manager infos
            BaseOutput managerInfosOut = srv.WS_GetCommunicationByPersonId(binput, modelUser.Manager.Id, true, out modelUser.CommunicationInformationsArray);

            BaseOutput emailEnumOut = srv.WS_GetEnumValueByName(binput, "email", out modelUser.EnumValue);
            modelUser.ManagerEmail = modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;

            BaseOutput homePhoneOut = srv.WS_GetEnumValueByName(binput, "homePhone", out modelUser.EnumValue);
            modelUser.ManagerHomePhone = modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;

            BaseOutput mobilePhoneOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelUser.EnumValue);
            modelUser.ManagerMobilePhone = modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;

            BaseOutput workPhoneOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelUser.EnumValue);
            modelUser.ManagerWorkPhone = modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelUser.CommunicationInformationsArray.Where(x => x.comType == modelUser.EnumValue.Id).FirstOrDefault().communication;


            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            return View(modelUser);
        }



        public ActionResult DeleteChildOrganisation(long? UserId, long? Id, long? parentId)
        {
            binput = new BaseInput();
            modelUser = new Organisation();

            BaseOutput UserOut = srv.WS_GetUserById(binput, (long)Id, true, out modelUser.User);
            BaseOutput foreignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, modelUser.User.Id, true, out modelUser.ForeignOrganisation);
            BaseOutput addressesOut = srv.WS_GetAddressesByUserId(binput, modelUser.User.Id, true, out modelUser.FutureAddressArray);
            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRolesArray);
            BaseOutput managerOut = srv.WS_GetPersonById(binput, (long)modelUser.ForeignOrganisation.manager_Id, true, out modelUser.Manager);

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            binput.userName = modelUser.Admin.Username;

            modelUser.User.updatedUser = binput.userName;

            BaseOutput deleteChildOrganisationUser = srv.WS_DeleteUser(binput, modelUser.User);
            BaseOutput deleteChildOrganisation = srv.WS_DeleteForeign_Organization(binput, modelUser.ForeignOrganisation);

            //delete all the addreses of the child organisation
            foreach (var item in modelUser.FutureAddressArray)
            {
                srv.WS_DeleteAddress(binput, item);
            }

            //delete all the roles of the child organisation
            foreach (var item in modelUser.UserRolesArray)
            {
                BaseOutput deletedRole = srv.WS_DeleteUserRole(binput, item);
            }

            //delete manager of the child organisation
            BaseOutput deleteManager = srv.WS_DeletePerson(binput, modelUser.Manager);


            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult AddGovOrganisation(Organisation form)
        {
            if (CheckExistence(form))
            {
                binput = new BaseInput();

                Organisation modelUser = new Organisation();

                modelUser.FutureUser = new tblUser();
                modelUser.FutureAddress = new tblAddress();
                modelUser.UserRole = new tblUserRole();

                modelUser.FutureUser.Username = form.UserName;
                modelUser.FutureUser.Email = form.Email;
                modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
                modelUser.Voen = form.Voen;
                modelUser.FutureUser.Status = 1;

                modelUser.Password = form.Password;

                BaseOutput adminunits = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == 0).ToList();

                return AddAddressInfo(modelUser.FutureUser, modelUser.Voen, null);
            }
            else
            {
                TempData["CustomError"] = "Bu adda istifadəçi sistemdə mövcuddur.";
                return RedirectToAction("AddGovOrganisation");
            }
           
        }

        public ActionResult AddAddressInfo(tblUser FutureUser, string Voen, long? UserId)
        {
            Session["arrONum"] = null;
            Organisation modelUser = new Organisation();

            BaseOutput adminunits = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
            modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == 0).ToList();

           
            modelUser.Email = FutureUser.Email;

            modelUser.Password = FutureUser.Password;
            modelUser.Voen = Voen;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return View("AddAddressInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddAddressInfo(Organisation form)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            
            modelUser.AddressId = form.adId.LastOrDefault();    
            modelUser.UserName = form.UserName;
            modelUser.Voen = form.Voen;
            return AddGovernmentOrganisationInfo(modelUser.UserName, form.Password, form.Email, modelUser.AddressId,form.FullAddress, modelUser.Voen,null);
        }

        public ActionResult AddGovernmentOrganisationInfo(string userName, string password, string email, long AddressId, string fullAddress, string Voen, long? UserId )
        {
            ModelState.Clear();
            Session["arrONum"] = null;
            Organisation modelUser = new Models.Organisation();

            modelUser.UserName = userName;
            modelUser.Password = password;
            modelUser.Email = email;
            modelUser.FullAddress = fullAddress;
            modelUser.UserName = userName;
            modelUser.Password = password;
            modelUser.AddressId = AddressId;
            modelUser.Voen = Voen;
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

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return View("AddGovernmentOrganisationInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddGovernmentOrganisationInfo(Organisation form)
        {
            binput = new BaseInput();

            Organisation modelUser = new Organisation();
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
            BaseOutput userOut = srv.WS_AddUser(binput, modelUser.FutureUser, out modelUser.FutureUser);

            
           //give roles to user
            BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);

            modelUser.UserRole.RoleId = modelUser.Role.Id;
            modelUser.UserRole.RoleIdSpecified = true;
            modelUser.UserRole.UserId = modelUser.FutureUser.Id;
            modelUser.UserRole.UserIdSpecified = true;
            modelUser.UserRole.Status = 1;
            modelUser.UserRole.StatusSpecified = true;
            BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);

            //add address informations
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

            modelUser.Manager = new tblPerson();
            modelUser.Manager.Name = form.ManagerName;

            modelUser.Manager.PinNumber = form.Pin;
            modelUser.Manager.FatherName = form.FatherName;
            modelUser.Manager.Surname = form.UserName;
            modelUser.Manager.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
            modelUser.Manager.birtdaySpecified = true;
            modelUser.Manager.gender = form.Gender;

            BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
            modelUser.Manager.educationLevel_eV_Id = modelUser.EnumValue == null ? 0: modelUser.EnumValue.Id;
            modelUser.Manager.educationLevel_eV_IdSpecified = true;

            BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
            modelUser.Manager.job_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
            modelUser.Manager.job_eV_IdSpecified = true;

            modelUser.Manager.Status = 1;
            modelUser.Manager.StatusSpecified = true;

            //modelUser.Manager.address_Id = modelUser.ManagerFutureAddress.Id;
            modelUser.Manager.address_IdSpecified = true;

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
            TempData["GovSuccess"] = "info";

            return RedirectToAction("Index", "GovernmentOrganisation");
        }

        //public JsonResult GetAdminUnitsByParentId(long Id)
        //{
        //    binput = new BaseInput();

        //    Organisation modelUser = new Organisation();

        //    srv.WS_GetAdminUnitsByParentId(binput, (int)Id, true, out modelUser.PRMAdminUnitArray);

        //    modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();
        //    return Json(modelUser.PRMAdminUnitList, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetThroughfaresByAdminUnitId(long Id)
        {
            binput = new BaseInput();

            Organisation modelUser = new Organisation();

            srv.WS_GetPRM_ThoroughfaresByAdminUnitId(binput, (int)Id, true, out modelUser.PRMThroughfareArray);

            modelUser.PRMThroughfareList = modelUser.PRMThroughfareArray.ToList();
            return Json(modelUser.PRMThroughfareList, JsonRequestBehavior.AllowGet);
        }

        public void SendUserInfos(string userName, string password, string email)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("ferid.heziyev@gmail.com", "emsal.az");
            if (String.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".com") )
            {
                email = "ferid.heziyev@gmail.com";
            }
            msg.To.Add(email);
            string fromPassword = "e1701895";
            msg.Subject = "Üzvlüyü tesdiqle";

            msg.Body = "<p>İstifadəçi adınız:" + userName + "</p>" +
                       "<p>Şifrəniz:" + password + "</p>" +
                       "<p>http://localhost:56557/Login</p>";

            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
            smtp.Timeout = 20000;

            //smtp.Send(msg);
        }

        public ActionResult Delete(long Id)
        {
            //if route is org go to organisation action if route is gov go to index action
            string route = "org";
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput UserOut = srv.WS_GetUserById(binput, Id, true, out modelUser.User);
            BaseOutput foreignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, modelUser.User.Id, true, out modelUser.ForeignOrganisation);
            BaseOutput addressesOut = srv.WS_GetAddressesByUserId(binput, modelUser.User.Id, true, out modelUser.FutureAddressArray);
            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRolesArray);
            BaseOutput govOrgTypeRoleOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);

            foreach (var item in modelUser.FutureAddressArray)
            {
                BaseOutput deleteAddress = srv.WS_DeleteAddress(binput, item);
            }
            foreach (var item in modelUser.UserRolesArray)
            {
                if(item.RoleId == modelUser.Role.Id)
                {
                    route = "gov";
                }
                BaseOutput deleteUserRole = srv.WS_DeleteUserRole(binput, item);

            }
            BaseOutput deleteForeignOrganisation = srv.WS_DeleteForeign_Organization(binput, modelUser.ForeignOrganisation);
            BaseOutput deleteUser = srv.WS_DeleteUser(binput, modelUser.User);
            
            
            if(route == "gov")
            {
                return RedirectToAction("Index", "GovernmentOrganisation");
            }
           else if(route == "org")
            {
                return RedirectToAction("Organisations", "GovernmentOrganisation");
            }
            else
            {
                return View();
            }
        }

        public ActionResult DeleteIndividual(long Id)
        {
            //if route is org go to organisation action if route is gov go to index action
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput UserOut = srv.WS_GetUserById(binput, Id, true, out modelUser.User);
            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelUser.User.Id, true, out modelUser.Person);
            BaseOutput addressesOut = srv.WS_GetAddressesByUserId(binput, modelUser.User.Id, true, out modelUser.FutureAddressArray);
            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRolesArray);

            foreach (var item in modelUser.FutureAddressArray)
            {
                BaseOutput deleteAddress = srv.WS_DeleteAddress(binput, item);
            }
            foreach (var item in modelUser.UserRolesArray)
            {
                BaseOutput deleteUserRole = srv.WS_DeleteUserRole(binput, item);
            }
            BaseOutput deletePerson = srv.WS_DeletePerson(binput, modelUser.Person);
            BaseOutput deleteUser = srv.WS_DeleteUser(binput, modelUser.User);

            if (IsPerson((long)modelUser.User.userType_eV_ID))
            {
                return RedirectToAction("Individuals", "GovernmentOrganisation");
            }
            else if(IsASC((long)modelUser.User.userType_eV_ID))
            {
                return RedirectToAction("ASCUsers", "GovernmentOrganisation");
            }
            else
            {
                return RedirectToAction("KTNUsers", "GovernmentOrganisation");
            }
        }

        public ActionResult Edit(long Id, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            BaseOutput userOut = srv.WS_GetUserById(binput, Id, true, out modelUser.User);
            modelUser.UserName = modelUser.User == null ? null : modelUser.User.Username;
            modelUser.Email = modelUser.User == null ? null : modelUser.User.Email;
            modelUser.Password = modelUser.User == null ? null : modelUser.User.Password;
            
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            BaseOutput ascsOut = srv.WS_GetPRM_ASCBranches(binput, out modelUser.ASCBranchArray);
            BaseOutput ktnOut = srv.WS_GetPRM_KTNBranches(binput, out modelUser.KTNBranchArray);

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult Edit(Organisation form, long Id)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput userOut = srv.WS_GetUserById(binput, Id, true, out modelUser.User);

            modelUser.User.Username = form.UserName;
            modelUser.User.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
            modelUser.User.Email = form.Email;
            modelUser.User.ASC_ID = form.ASCId;
            modelUser.User.KTN_ID = form.KTNId;
            BaseOutput updateUserOut = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);

            BaseOutput individualTypeOut = srv.WS_GetEnumValueByName(binput, "fizikişexs", out modelUser.EnumValue);
            long fiziki = modelUser.EnumValue.Id;    
            BaseOutput ascOut = srv.WS_GetEnumValueByName(binput, "ASC", out modelUser.EnumValue);
            long asc = modelUser.EnumValue.Id;
            if (IsPerson((long)modelUser.User.userType_eV_ID) || IsASC((long)modelUser.User.userType_eV_ID) || IsKTN((long)modelUser.User.userType_eV_ID))
            {
                return RedirectToAction("EditPerson", "GovernmentOrganisation", new { userId = modelUser.User.Id });
            }
            else
            {
                return RedirectToAction("EditOrganisation", "GovernmentOrganisation", new { userId = modelUser.User.Id });
            }

        }

        public ActionResult EditPerson(long userId, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            modelUser.User = new tblUser();

            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, userId, true, out modelUser.Person);
            if (modelUser.Person == null)
            {
                TempData["personNotExist"] = "infonotexist";
                return RedirectToAction("Individuals");
            }

            modelUser.User.Id = userId;
            modelUser.Name = modelUser.Person == null ? null : modelUser.Person.Name;
            modelUser.Surname = modelUser.Person == null ? null : modelUser.Person.Surname;
            modelUser.Gender = modelUser.Person == null ? null : modelUser.Person.gender;
            modelUser.FatherName = modelUser.Person == null ? null :modelUser.Person.FatherName;
            modelUser.Pin = modelUser.Person.PinNumber;

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return View(modelUser);
        }

        [HttpPost]
        public ActionResult EditPerson(Organisation form, long userId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, userId, true, out modelUser.Person);

            modelUser.Person.Name = form.Name;
            modelUser.Person.Surname = form.Surname;
            modelUser.Person.FatherName = form.FatherName;
            modelUser.Person.gender = form.Gender;
            modelUser.Person.PinNumber = form.Pin;

            BaseOutput updatePersonOut = srv.WS_UpdatePerson(binput, modelUser.Person, out modelUser.Person);

            return RedirectToAction("EditAddress", "GovernmentOrganisation", new { id = modelUser.Person.address_Id, userId = userId });
        }

        public ActionResult EditOrganisation(long userId, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            modelUser.User = new tblUser();
            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, userId, true, out modelUser.ForeignOrganisation);

            if(modelUser.ForeignOrganisation == null)
            {
                TempData["foreignOrganisationNotExist"] = "infonotexist";
                return RedirectToAction("Organisations");
            }

            modelUser.Name = modelUser.ForeignOrganisation == null ? null : modelUser.ForeignOrganisation.name;
            modelUser.Voen = modelUser.ForeignOrganisation == null ? null: modelUser.ForeignOrganisation.voen;

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            modelUser.User.Id = userId;
            return View(modelUser);
        }
        
        [HttpPost]
        public ActionResult EditOrganisation(Organisation form, long userId, long organisationId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, organisationId, true, out modelUser.ForeignOrganisation);

            modelUser.ForeignOrganisation.name = form.Name;
            modelUser.ForeignOrganisation.voen = form.Voen;

            BaseOutput updateOrganisationOut = srv.WS_UpdateForeign_Organization(binput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);

            return RedirectToAction("EditAddress", "GovernmentOrganisation", new { id = modelUser.ForeignOrganisation.address_Id, userId = userId, AdminId = 1});
        }


        public ActionResult EditAddress(long id, long userId, long?AdminId)
        {
            Session["arrONum"] = null;

            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            modelUser.User = new tblUser();
            BaseOutput addressOut = srv.WS_GetAddressById(binput, id, true, out modelUser.FutureAddress);

            modelUser.User.Id = userId;

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    AdminId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)AdminId, true, out modelUser.Admin);
            return View(modelUser);
        }

        public bool IsASC(long userType)
        {
            modelUser = new Organisation();

            BaseOutput userTypePut = srv.WS_GetEnumValueByName(binput, "ASC", out modelUser.EnumValue);
            if(userType == modelUser.EnumValue.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsPerson(long userType)
        {
            modelUser = new Organisation();
            BaseOutput userTypePut = srv.WS_GetEnumValueByName(binput, "fizikişexs", out modelUser.EnumValue);
            if (userType == modelUser.EnumValue.Id)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsKTN(long userType)
        {
            modelUser = new Organisation();
            BaseOutput userTypePut = srv.WS_GetEnumValueByName(binput, "KTN", out modelUser.EnumValue);
            if (userType == modelUser.EnumValue.Id)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [HttpPost]
        public ActionResult EditAddress(Organisation form, long addressId, long userId)
        {
            Session["arrONum"] = null;

            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            bool ifGov = false;
            BaseOutput addressOut = srv.WS_GetAddressById(binput, addressId, true, out modelUser.FutureAddress);

            modelUser.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();


            BaseOutput updateAddressOut = srv.WS_UpdateAddress(binput, modelUser.FutureAddress);

            BaseOutput userOut = srv.WS_GetUserById(binput, userId, true, out modelUser.User);

            BaseOutput userTypePut = srv.WS_GetEnumValueByName(binput, "fizikişexs", out modelUser.EnumValue);
            
            BaseOutput roleOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);
            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRolesArray);

            foreach (var item in modelUser.UserRolesArray)
            {
                if(item.RoleId == modelUser.Role.Id)
                {
                    ifGov = true;
                }
            }    
            
            if (IsPerson((long)modelUser.User.userType_eV_ID))
            {
                return RedirectToAction("Individuals", "GovernmentOrganisation");
            }
            else if (IsASC((long)modelUser.User.userType_eV_ID)){
                return RedirectToAction("ASCUsers", "GovernmentOrganisation");
            }
            else if (IsKTN((long)modelUser.User.userType_eV_ID))
            {
                return RedirectToAction("KTNUsers", "GovernmentOrganisation");
            }
            else if (!ifGov)
            {
                return RedirectToAction("Organisations", "GovernmentOrganisation");
            }
            else
            {
                return RedirectToAction("Index", "GovernmentOrganisation");
            }
        }


        public long ConvertStringYearMonthDayFormatToTimestamp(Organisation form)
        {
            string[] dates = Regex.Split(form.Birthday, @"\.");
            int year = dates.Length == 0 ? 0 : Convert.ToInt32(dates[2]);
            int month = Convert.ToInt32(dates[1]);
            int day = Convert.ToInt32(dates[0]);
            DateTime dTime = new DateTime(year, month, day );
            DateTime sTime = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            long birthday = (long)(dTime - sTime).TotalSeconds;

            return birthday;
        }

        public ActionResult Individuals(int? page, long?AdminId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            Organisation modelUser = new Organisation();
            //get roles by name gelecek bura
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    AdminId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)AdminId, true, out modelUser.Admin);

            BaseOutput enumPersonOut = srv.WS_GetEnumValueByName(binput, "fizikişexs", out modelUser.EnumValue);
            BaseOutput bouput = srv.WS_GetUsersByUserType(binput, modelUser.EnumValue.Id, true, out modelUser.UserArray);

            modelUser.UserList = modelUser.UserArray.Where(x=>x.Username != null).ToList();


            modelUser.Paging = modelUser.UserList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndividuals", modelUser)
                : View(modelUser);
        }
        public ActionResult Organisations(int? page, long?AdminId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            Organisation modelUser = new Organisation();
            //get roles by name gelecek bura

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    AdminId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)AdminId, true, out modelUser.Admin);
            BaseOutput organisationEnumOut = srv.WS_GetEnumValueByName(binput, "legalPerson", out modelUser.EnumValue);
            BaseOutput govRoleOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);


            BaseOutput userOut = srv.WS_GetOrganisationTypeUsers(binput, modelUser.Role.Id, true, modelUser.EnumValue.Id, true, out modelUser.UserArray);

            modelUser.UserList = modelUser.UserArray.Where(x=>x.Username!=null).ToList();


            modelUser.Paging = modelUser.UserList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialOrganisation", modelUser)
                : View(modelUser);
        }

        public ActionResult ASCUsers(int? page, long? AdminId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            Organisation modelUser = new Organisation();
            //get roles by name gelecek bura
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    AdminId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)AdminId, true, out modelUser.Admin);
            BaseOutput ascenumOUt = srv.WS_GetEnumValueByName(binput, "ASC", out modelUser.EnumValue);
            BaseOutput ASCUsersOut = srv.WS_GetUsersByUserType(binput, modelUser.EnumValue.Id, true, out modelUser.UserArray);

            //getting all asc users
            modelUser.UserList = modelUser.UserArray.ToList();


            modelUser.Paging = modelUser.UserList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialASCUsers", modelUser)
                : View(modelUser);
        }

        public ActionResult KTNUsers(int? page, long? AdminId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            Organisation modelUser = new Organisation();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    AdminId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)AdminId, true, out modelUser.Admin);
            BaseOutput ascenumOUt = srv.WS_GetEnumValueByName(binput, "KTN", out modelUser.EnumValue);
            BaseOutput ASCUsersOut = srv.WS_GetUsersByUserType(binput, modelUser.EnumValue.Id, true, out modelUser.UserArray);

            //getting all asc users
            modelUser.UserList = modelUser.UserArray.ToList();


            modelUser.Paging = modelUser.UserList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialKTNUsers", modelUser)
                : View(modelUser);
        }
        public ActionResult AddBRANCHUser(long? UserId, string type)
        {
                Organisation modelUser = new Organisation();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

                BaseOutput ascsOut = srv.WS_GetPRM_ASCBranches(binput, out modelUser.ASCBranchArray);
                BaseOutput ktnOut = srv.WS_GetPRM_KTNBranches(binput, out modelUser.KTNBranchArray);

                modelUser.userType = type;
                
                return View(modelUser);
        }


        [HttpPost]
        public ActionResult AddBRANCHUser(Organisation form)
        {
            if (CheckExistence(form))
            {
                modelUser = new Organisation();

                modelUser.UserName = form.UserName;
                modelUser.Password = form.Password;
                modelUser.Pin = form.Pin;
                modelUser.Email = form.Email;
                modelUser.ASCId = form.ASCId;
                modelUser.KTNId = form.KTNId;
                return AddBRANCHPersonInfo(null, modelUser);
            }
            else
            {
                TempData["CustomError"] = "Bu adda istifadəçi sistemdə mövcuddur.";
                return RedirectToAction("AddBRANCHUser");
            }
            
        }
        public ActionResult AddBRANCHPersonInfo(long? UserId, Organisation ModelUserInfo)
        {
            ModelState.Clear();

            Organisation modelUser = new Organisation();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

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

            modelUser.UserName = ModelUserInfo.UserName;
            modelUser.Email = ModelUserInfo.Email;
            modelUser.Password = ModelUserInfo.Password;
            modelUser.Pin = ModelUserInfo.Pin;
            modelUser.ASCId = ModelUserInfo.ASCId;
            modelUser.KTNId = ModelUserInfo.KTNId;
            return View("AddBRANCHPersonInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddBRANCHPersonInfo(Organisation form)
        {
            modelUser = new Organisation();
            modelUser.UserName = form.UserName;
            modelUser.Password = form.Password;
            modelUser.Email = form.Email;
            modelUser.Pin = form.Pin;
            modelUser.Name = form.Name;
            modelUser.Surname = form.Surname;
            modelUser.Birthday = form.Birthday;
            modelUser.FatherName = form.FatherName;
            modelUser.ASCId = form.ASCId;
            modelUser.KTNId = form.KTNId;
            modelUser.Education = form.Education;
            modelUser.Job = form.Job;
            modelUser.Gender = form.Gender;
            return AddBRANCHAddressInfo(null, modelUser);
        }

        public ActionResult AddBRANCHAddressInfo(long? UserId, Organisation ModelUserInfo)
        {
            Session["arrONum"] = null;

            Organisation modelUser = new Organisation();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            modelUser.UserName = ModelUserInfo.UserName;
            modelUser.Email = ModelUserInfo.Email;
            modelUser.Password = ModelUserInfo.Password;
            modelUser.Pin = ModelUserInfo.Pin;
            modelUser.Name = ModelUserInfo.Name;
            modelUser.Surname = ModelUserInfo.Surname;
            modelUser.Birthday = ModelUserInfo.Birthday;
            modelUser.FatherName = ModelUserInfo.FatherName;
            modelUser.ASCId = ModelUserInfo.ASCId;
            modelUser.KTNId = ModelUserInfo.KTNId;
            modelUser.Education = ModelUserInfo.Education;
            modelUser.Job = ModelUserInfo.Job;
            modelUser.Gender = ModelUserInfo.Gender;
            return View("AddBRANCHAddressInfo", modelUser);
        }

        [HttpPost]
        public ActionResult AddBRANCHAddressInfo(Organisation form)
        {
            binput = new BaseInput();
            modelUser = new Organisation();
            modelUser.FutureUser = new tblUser();
            modelUser.FutureAddress = new tblAddress();
            modelUser.Person = new tblPerson();
            modelUser.UserRole = new tblUserRole();

            modelUser.FutureUser.ASC_ID = form.ASCId;
            modelUser.FutureUser.ASC_IDSpecified = true;
            modelUser.FutureUser.KTN_ID = form.KTNId;
            modelUser.FutureUser.KTN_IDSpecified = true;
            modelUser.FutureUser.Email = form.Email;
            modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password);

            modelUser.FutureUser.Username = form.UserName;

            //get the ASC user type enum value id
            if(modelUser.FutureUser.KTN_ID == 0)
            {
                BaseOutput ascOut = srv.WS_GetEnumValueByName(binput, "ASC", out modelUser.EnumValue);
                BaseOutput ascRoleOut = srv.WS_GetRoleByName(binput, "ascUser", out modelUser.Role);
            }
            else
            {
                BaseOutput ktnOut = srv.WS_GetEnumValueByName(binput, "KTN", out modelUser.EnumValue);
                BaseOutput ktnRoleOut = srv.WS_GetRoleByName(binput, "ktnUser", out modelUser.Role);
            }
            modelUser.FutureUser.userType_eV_ID = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
            modelUser.FutureUser.userType_eV_IDSpecified = true;

            BaseOutput addUser = srv.WS_AddUser(binput, modelUser.FutureUser, out modelUser.FutureUser);
            //add user role
            modelUser.UserRole.RoleId = modelUser.Role == null ? 0: modelUser.Role.Id;
            modelUser.UserRole.RoleIdSpecified = true;
            modelUser.UserRole.UserId = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
            modelUser.UserRole.UserIdSpecified = true;

            BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);

            //add address
            modelUser.FutureAddress.adminUnit_Id = form.adId.Length == 0 ? 0 : form.adId.LastOrDefault();
            modelUser.FutureAddress.fullAddress = form.FullAddress;
            modelUser.FutureAddress.user_Id = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
            modelUser.FutureAddress.adminUnit_IdSpecified = true;
            modelUser.FutureAddress.user_IdSpecified = true;

            BaseOutput addAddress = srv.WS_AddAddress(binput, modelUser.FutureAddress, out modelUser.FutureAddress);

            //add person
            modelUser.Person.Name = form.Name;
            modelUser.Person.Surname = form.Surname;
            modelUser.Person.FatherName = form.FatherName;
            modelUser.Person.address_Id = modelUser.FutureAddress == null ? 0 : modelUser.FutureAddress.Id;
            modelUser.Person.address_IdSpecified = true;
            modelUser.Person.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
            modelUser.Person.birtdaySpecified = true;
            modelUser.Person.gender = form.Gender;

            BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelUser.EnumValue);
            modelUser.Person.educationLevel_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
            modelUser.Person.educationLevel_eV_IdSpecified = true;

            BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelUser.EnumValue);
            modelUser.Person.job_eV_Id = modelUser.EnumValue == null ? 0 : modelUser.EnumValue.Id;
            modelUser.Person.job_eV_IdSpecified = true;

            modelUser.Person.UserId = modelUser.FutureUser == null ? 0 : modelUser.FutureUser.Id;
            modelUser.Person.UserIdSpecified = true;
            modelUser.Person.PinNumber = form.Pin;

            BaseOutput addPerson = srv.WS_AddPerson(binput, modelUser.Person, out modelUser.Person);

            
            if (IsASC((long)modelUser.FutureUser.userType_eV_ID))
            {
                TempData["ascUser"] = "info";
                return RedirectToAction("ASCUsers");
            }
            else
            {
                TempData["ktnUser"] = "info";
                return RedirectToAction("KTNUsers");
            }
        }

        public ActionResult AdminUnit(int pId = 0)
        {
            binput = new BaseInput();

            Organisation modelUser = new Organisation();

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
            modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelOfferProduction.PRMAdminUnitArray);
            //modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
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

        public ActionResult BranchResponsibility(int pId = 1)
        {
            binput = new BaseInput();

            Organisation modelUser = new Organisation();

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
            modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelOfferProduction.PRMAdminUnitArray);
            //modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
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
        public bool CheckExistence(Organisation form)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);

            tblUser UserFromUsername = modelUser.FutureUser;


            BaseOutput checkEdxistenseOut = srv.WS_GetUsers(binput, out modelUser.UserArray);
            List<tblUser> UserFromEmail = modelUser.UserArray.Where(x => x.Email == form.Email).ToList();

            BaseOutput orgOut = srv.WS_GetForeign_Organizations(binput, out modelUser.ForeignOrganisationArray);
            List<tblForeign_Organization> OrgFromVoen = form.Voen == null ? new List<tblForeign_Organization>() : modelUser.ForeignOrganisationArray.Where(x => x.voen == form.Voen).ToList();


            if (modelUser.FutureUser != null || UserFromEmail.Count != 0 || OrgFromVoen.Count != 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


    }
}
