using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.Utility.UtilityObjects;
using Emsal.WebInt.EmsalSrv;
using Emsal.WebInt.IAMAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Emsal.Utility.CustomObjects;
using System.Net.Mail;

namespace Emsal.UI.Controllers
{
    public class SignUpOPController : Controller
    {
        //
        // GET: /SignUpOP/

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        Emsal.WebInt.TaxesSRV.BasicHttpBinding_ITaxesIntegrationService taxessrv = Emsal.WebInt.EmsalService.taxesService;

        private static string fullAddressId = "";
        private static string addressDesc = "";
        //private static string fin = "";
        //private static string sVoen = "";
        private static string orgRoles = "";
        //private static long sUid  = 0;
        //private static long potId = 0;
        private static string sType = null;
        private BaseInput baseInput;
        private UserViewModel modelUser;
        tblPRM_AdminUnit tblAdminUnit;
        Emsal.WebInt.TaxesSRV.VOENDATA taxesService = null;



        public ActionResult Index(long uid = 0, string type = null)
        {
            try
            {
                UserViewModel model = new UserViewModel();

                baseInput = new BaseInput();
                modelUser = new UserViewModel();

                long userId = 0;
                //sUid = potId = uid;
                sType = type;

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelUser.User);

                modelUser.Person = new tblPerson();

                //BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, auid, true, out modelUser.PRMAdminUnitArray);
                //modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();

                //fullAddressId = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Id));

                BaseOutput adminUnit = srv.WS_GetAdminUnitsByParentId(baseInput, 0, true, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.OrderBy( x=> x.Name ).ToList();


                BaseOutput mobileN = srv.WS_GetEnumCategorysByName(baseInput, "mobilePhonePrefix", out modelUser.EnumCategory);
                BaseOutput mobileİd = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueMobilePhone = modelUser.EnumValueArray.ToList();

                BaseOutput educationN = srv.WS_GetEnumCategorysByName(baseInput, "Tehsil", out modelUser.EnumCategory);
                BaseOutput educationId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueEducationList = modelUser.EnumValueArray.ToList();

                BaseOutput jobN = srv.WS_GetEnumCategorysByName(baseInput, "İş", out modelUser.EnumCategory);
                BaseOutput jobId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueJobList = modelUser.EnumValueArray.ToList();

                long i = 0;
                if (type == "1")
                {
                    UserViewModel modelP = new UserViewModel();
                    BaseOutput per = srv.WS_GetPersonByUserId(baseInput, uid, true, out modelP.Person);

                    i = (long)modelP.Person.UserId;

                    modelUser.Person = new tblPerson();
                    modelUser.pType = type;
                }
                else if (type == "2")
                {
                    User userModel = new User();
                    BaseOutput personOut = srv.WS_GetForeign_OrganizationByUserId(baseInput, uid, true, out userModel.ForeignOrganisation);

                    i = (long)userModel.ForeignOrganisation.userId;

                    //modelUser.voen = userModel.ForeignOrganisation.voen;
                    modelUser.legalPersonName = userModel.ForeignOrganisation.name;
                    modelUser.pType = type;
                }

                if (!String.IsNullOrEmpty(type) && i == uid && uid != 0)
                {
                    orgRoles = "producerPerson";
                }
                else
                {
                    orgRoles = "sellerPerson";
                }

                return View(modelUser);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }

        public bool CheckExistence(UserViewModel form)
        {
            //baseInput = new BaseInput();
            //User modelUser = new User();

            //BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(baseInput, form.userName, out modelUser.FutureUser);

            //BaseOutput pereOut = srv.WS_GetEnumValueById(baseInput, long.Parse(perefix), true, out modelUser.EnumValue);
            //BaseOutput comOut = srv.WS_GetCommunications(baseInput, out modelUser.CommunicationInformationsArray);

            //string fullMobNumb = modelUser.EnumValue.name + mobile;

            ////BaseOutput checkEdxistenseOut = srv.WS_GetUsers(baseInput, out modelUser.UserArray);
            //List<tblCommunication> UserFromNumber = modelUser.CommunicationInformationsArray.Where(x => x.communication == fullMobNumb).ToList();

            ////BaseOutput personsOut = srv.WS_GetPersons(baseInput, out modelUser.PersonArray);
            ////List<tblPerson> PersonFromPin = form.fin == null ? new List<tblPerson>() : modelUser.PersonArray.Where(x => x.PinNumber == form.fin).ToList();

            ////BaseOutput orgOut = srv.WS_GetForeign_Organizations(baseInput, out modelUser.ForeignOrganisationArray);
            ////List<tblForeign_Organization> OrgFromVoen = form.voen == null ? new List<tblForeign_Organization>() : modelUser.ForeignOrganisationArray.Where(x => x.voen == form.voen).ToList();

            ////normal ucun olmalidir 
            ////if (modelUser.FutureUser != null || UserFromEmail.Count != 0 || PersonFromPin.Count != 0 || OrgFromVoen.Count != 0)
            ////{
            ////    return false;
            ////}
            ////else
            ////{
            ////    return true;
            ////}

            //if (modelUser.FutureUser != null || UserFromNumber.Count != 0)
            //{
            //    return false;
            //}
            //else
            //{
            return true;
            //}

        }

        [HttpPost]
        public ActionResult Index(UserViewModel mdl)
        {
            try
            {
                modelUser = new UserViewModel();
                baseInput = new BaseInput();

                BaseOutput mobileN = srv.WS_GetEnumCategorysByName(baseInput, "mobilePhonePrefix", out modelUser.EnumCategory);
                BaseOutput mobileİd = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                mdl.EnumValueMobilePhone = modelUser.EnumValueArray.ToList();

                BaseOutput educationN = srv.WS_GetEnumCategorysByName(baseInput, "Tehsil", out modelUser.EnumCategory);
                BaseOutput educationId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                mdl.EnumValueEducationList = modelUser.EnumValueArray.ToList();

                BaseOutput jobN = srv.WS_GetEnumCategorysByName(baseInput, "İş", out modelUser.EnumCategory);
                BaseOutput jobId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                mdl.EnumValueJobList = modelUser.EnumValueArray.ToList();
                mdl.warning = null;

                if (CheckExistence(mdl))
                {
                    if (mdl.fin != null)
                    {
                        BaseOutput finOut = srv.WS_GetPersonByPinNumber(baseInput, mdl.fin, out mdl.Person);
                        if (mdl.Person != null)
                        {
                            BaseOutput uidBase = srv.WS_GetUserById(baseInput, (long)mdl.Person.UserId, true, out modelUser.User);
                        }
                    }
                    else
                        if (mdl.voen != null)
                    {
                        BaseOutput voenOut = srv.WS_GetForeign_OrganizationByVoen(baseInput, mdl.voen, out mdl.ForeignOrganisation);
                        if (mdl.ForeignOrganisation != null)
                        {
                            BaseOutput uidBase = srv.WS_GetUserById(baseInput, (long)mdl.ForeignOrganisation.userId, true, out modelUser.User);
                        }
                    }
                    
                    if (modelUser.User != null)
                    {
                        BaseOutput gabui = srv.WS_GetAddressesByUserId(baseInput, modelUser.User.Id, true, out modelUser.AddressArray);
                        modelUser.Address = modelUser.AddressArray.ToList().FirstOrDefault();
                        BaseOutput gpbui = srv.WS_GetPersonByUserId(baseInput, modelUser.User.Id, true, out modelUser.Person);
                        if (!String.IsNullOrEmpty(mdl.voen))
                        {
                            BaseOutput fOut = srv.WS_GetForeign_OrganizationByVoen(baseInput, modelUser.voen, out modelUser.ForeignOrganisation);
                        }
                        //BaseOutput cominicationOut = srv.WS_GetCommunicationByPersonId(baseInput,(long)modelUser.Person.Id, true, out modelUser.Comminication);
                    }

                    if (modelUser.User == null)
                    {
                        modelUser.User = new tblUser();
                        modelUser.Address = new tblAddress();
                        modelUser.Person = new tblPerson();
                        if (!String.IsNullOrEmpty(mdl.voen))
                        {
                            modelUser.ForeignOrganisation = new tblForeign_Organization();
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(mdl.voen))
                        {
                            BaseOutput foreign = srv.WS_GetForeign_OrganizationByVoen(baseInput, modelUser.voen, out modelUser.ForeignOrganisation);
                            if (modelUser.ForeignOrganisation == null)
                            {
                                modelUser.ForeignOrganisation = new tblForeign_Organization();
                            }
                        }
                    }


                    modelUser.User.Username = mdl.userName;
                    modelUser.User.Email = mdl.eMail;
                    modelUser.User.Password = BCrypt.Net.BCrypt.HashPassword(mdl.passWord, 5); ;

                    string enumtype = "";
                    if (String.IsNullOrEmpty(mdl.voen))
                    {
                        enumtype = "fizikişexs";
                    }
                    else
                    {
                        enumtype = "legalPerson";
                    }

                    BaseOutput envalu = srv.WS_GetEnumValueByName(baseInput, enumtype, out modelUser.EnumValue); //TODO

                    modelUser.User.userType_eV_ID = modelUser.EnumValue.Id;
                    modelUser.User.userType_eV_IDSpecified = true;

                    if (modelUser.User.Id > 0)
                    {
                        BaseOutput apu = srv.WS_UpdateUser(baseInput, modelUser.User, out modelUser.User);
                    }
                    else
                    {
                        BaseOutput apu = srv.WS_AddUser(baseInput, modelUser.User, out modelUser.User);
                    }

                    modelUser.Address.adminUnit_Id = mdl.addressId;
                    modelUser.Address.adminUnit_IdSpecified = true;


                    BaseOutput galf = srv.WS_GetAdminUnitListForID(baseInput, mdl.addressId, true, out modelUser.PRMAdminUnitArray);


                    modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();
                    modelUser.Address.fullAddress = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Name));

                    modelUser.Address.addressDesc = mdl.descAddress;
                    modelUser.Address.user_Id = modelUser.User.Id;
                    modelUser.Address.user_IdSpecified = true;

                    if (modelUser.Address.Id > 0)
                    {
                        BaseOutput aa = srv.WS_UpdateAddress(baseInput, modelUser.Address);
                    }
                    else
                    {
                        BaseOutput aa = srv.WS_AddAddress(baseInput, modelUser.Address, out modelUser.Address);
                    }


                    modelUser.Person.Name = mdl.Name;
                    modelUser.Person.Surname = mdl.Surname;
                    modelUser.Person.FatherName = mdl.FatherName;

                    modelUser.Person.UserId = modelUser.User.Id;
                    modelUser.Person.UserIdSpecified = true;

                    if (String.IsNullOrEmpty(mdl.voen))
                    {
                        modelUser.Person.PinNumber = mdl.fin;
                        modelUser.Person.address_Id = modelUser.Address.Id;
                    }

                    modelUser.Person.address_IdSpecified = true;
                    modelUser.Person.gender = mdl.gender;
                    //modelUser.Person.birtday = (DateTime.Parse(mdl.birtday)).getInt64ShortDate();
                    modelUser.Person.birtday = (DateTime.Parse(mdl.birtday)).getInt64ShortDate();
                    modelUser.Person.birtdaySpecified = true;
                    modelUser.Person.profilePicture = mdl.createdUser;
                    if (mdl.education != null)
                    {
                        modelUser.Person.educationLevel_eV_Id = Int64.Parse(mdl.education);
                        modelUser.Person.educationLevel_eV_IdSpecified = true;
                    }

                    if (mdl.job != null)
                    {
                        modelUser.Person.job_eV_Id = Int64.Parse(mdl.job);
                        modelUser.Person.job_eV_IdSpecified = true;
                    }

                    if (modelUser.Person.Id > 0)
                    {
                        BaseOutput aper = srv.WS_UpdatePerson(baseInput, modelUser.Person, out modelUser.Person);
                    }
                    else
                    {
                        BaseOutput aper = srv.WS_AddPerson(baseInput, modelUser.Person, out modelUser.Person);

                        modelUser.UserRole = new tblUserRole();
                        if (orgRoles == "sellerPerson")
                        {
                            if (mdl.suplierType == 2)
                            {
                                orgRoles = "draftProducerPerson";
                            }
                            else
                            {
                                orgRoles = "sellerPerson";
                            }
                        }
                        BaseOutput role = srv.WS_GetRoleByName(baseInput, orgRoles, out modelUser.Role);

                        modelUser.UserRole.RoleId = modelUser.Role.Id;
                        modelUser.UserRole.RoleIdSpecified = true;
                        modelUser.UserRole.Status = 1;
                        modelUser.UserRole.StatusSpecified = true;
                        modelUser.UserRole.UserId = modelUser.User.Id;
                        modelUser.UserRole.UserIdSpecified = true;

                        BaseOutput addUserRole = srv.WS_AddUserRole(baseInput, modelUser.UserRole, out modelUser.UserRole);
                    }
                    BaseOutput mobOut;
                    modelUser.EnumValue = new tblEnumValue();
                    modelUser.Comminication = new tblCommunication();
                    mobOut = srv.WS_GetEnumValueById(baseInput, (long)(mdl.mPerefix), true, out modelUser.EnumValue);
                    modelUser.Comminication.communication = modelUser.EnumValue.name + mdl.mNumber;
                    modelUser.Comminication.description = mdl.mNumber;
                    modelUser.Comminication.priorty = 1;
                    modelUser.Comminication.Status = 1;
                    modelUser.Comminication.PersonId = modelUser.Person.Id;
                    modelUser.Comminication.PersonIdSpecified = true;
                    mobOut = srv.WS_GetEnumValueByName(baseInput, "mobilePhone", out modelUser.EnumValue);
                    modelUser.Comminication.comType = (Int32)modelUser.EnumValue.Id;
                    modelUser.Comminication.comTypeSpecified = true;

                    BaseOutput addMobileNumber = srv.WS_AddCommunication(baseInput, modelUser.Comminication, out modelUser.Comminication);
                    //BaseOutput addComOut = 

                    long adrrId = modelUser.Address.Id;


                    if (!String.IsNullOrEmpty(mdl.voen))
                    {
                        modelUser.ForeignOrganisation.name = mdl.legalPersonName;
                        modelUser.ForeignOrganisation.voen = mdl.voen;
                        modelUser.ForeignOrganisation.userId = modelUser.User.Id;
                        modelUser.ForeignOrganisation.userIdSpecified = true;
                        modelUser.ForeignOrganisation.address_Id = adrrId;
                        modelUser.ForeignOrganisation.address_IdSpecified = true;

                        if (mdl.ForeignOrganisation != null)
                        {
                            BaseOutput fO = srv.WS_UpdateForeign_Organization(baseInput, mdl.ForeignOrganisation, out modelUser.ForeignOrganisation);
                        }
                        else
                        {
                            BaseOutput fO = srv.WS_AddForeign_Organization(baseInput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);
                        }
                    }

                    MailMessage msg = new MailMessage();

                    msg.To.Add(mdl.eMail);
                    msg.Subject = "Qeydiyyat";

                    msg.Body = "<p>Hörmətli " + mdl.Name + " "+ mdl.Surname +"</p>" +
                        "<p>Təqdim etdiyiniz məlumatlara əsasən, “Satınalan təşkilatların ərzaq məhsullarına tələbatı” portalında (tedaruk.az) qeydiyyatdan keçdiniz.</p>" +
                        "<p>İstifadəçi adınız: " + mdl.userName + "</p>" +
                        "<p>Şifrəniz :  " + mdl.passWord + "</p>";

                    msg.IsBodyHtml = true;

                    

                    if (Mail.SendMail(msg))
                    {
                        TempData["Message"] = "Email göndərildi.";
                    }
                    else
                    {
                        TempData["Message"] = "Email göndərilmədi.";
                    }

                    TempData["personSignUp"] = "success";

                    bool verify = BCrypt.Net.BCrypt.Verify(mdl.passWord, modelUser.User.Password);
                    if (verify)
                    {
                        long uId = 0;
                        uId = modelUser.User.Id;
                        //LoginController lg = new LoginController();
                        //return lg.CreateTicket(1, "OfferProduction", modelUser.User, null);
                        //return RedirectToAction("CreateTicket", "Login",new { ticketNum = 1, route = "OfferProduction",  User = "", returnUrl = "", uId = uId });
                        //return RedirectToAction("CreateTicket", "Login", new { ticketNum = 1, route = "autoLogin", User = "", returnUrl = "", uId = uId });
                        //return CreateTicket(1, "AsanXidmetSpecial", modelUser.User, returnUrl);
                        //return RedirectToAction("Index", "OfferProduction");
                        Session["pass"] = mdl.passWord;
                        Session["uid"] = modelUser.User.Id;
                    }
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    //return RedirectToAction("Index", "Login");
                    BaseOutput gaalf = srv.WS_GetAdminUnitListForID(baseInput, modelUser.addressId, true, out mdl.PRMAdminUnitArray);
                    //modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();
                    //mdl.Address.fullAddress = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Name));

                    mdl.warning = "İstifadəçi artıq mövcuddur !";
                    return View("Index", mdl);
                }
            }
            catch (Exception ex)
            {
                if (modelUser.User != null) { 
                    BaseOutput delUserOut = srv.WS_DeleteUser(baseInput, modelUser.User);
                }

                if (modelUser.Address != null) { 
                BaseOutput delAdrrOut = srv.WS_DeleteAddress(baseInput, modelUser.Address);
                }

                if (modelUser.Person != null)
                {
                    BaseOutput delPerOut = srv.WS_DeletePerson(baseInput, modelUser.Person);
                }

                if (modelUser.UserRole != null)
                {
                    BaseOutput delRolOut = srv.WS_DeleteUserRole(baseInput, modelUser.UserRole);
                }

                if (modelUser.Comminication != null)
                {
                    BaseOutput delCommOut = srv.WS_DeleteCommunication(baseInput, modelUser.Comminication);
                }

                if (modelUser.ForeignOrganisation != null)
                {
                    BaseOutput delForOut = srv.WS_DeleteForeign_Organization(baseInput, modelUser.ForeignOrganisation);
                }
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public tblPerson GetPhysicalPerson(string fin, string type)
        {
            //sVoen = "";
            sType = "";

            modelUser = new UserViewModel();
            baseInput = new BaseInput();
            try
            {

                SingleServiceControl srvcontrol = new SingleServiceControl();
                getPersonalInfoByPinNewResponseResponse iamasPerson = null;
                tblPerson person = null;
                tblForeign_Organization foreignOrg = new tblForeign_Organization();
                long auid = 0;

                int control = 0;
                if (type == "1")
                {
                    control = srvcontrol.getPersonInfoByPin(fin, out person, out iamasPerson);
                }
                else
                  if (type == "2")
                {
                    BaseOutput foreign = srv.WS_GetForeign_OrganizationByVoen(baseInput, fin, out foreignOrg);
                    //sVoen = fin;
                    if (foreignOrg != null)
                    {
                        BaseOutput personOut = srv.WS_GetPersonByUserId(baseInput, Int64.Parse(foreignOrg.userId.ToString()), true, out person);
                        modelUser.legalPersonName = foreignOrg.name;
                    }
                    else
                    {
                        taxesService = taxessrv.getOrganisationInfobyVoen(fin);
                    }
                    //control = srvcontrol.getPersonInfoByPin(fin, out person, out iamasPerson);
                }


                modelUser.Person = new tblPerson();
                if (person != null)
                {
                    modelUser.Person.Name = person.Name;
                    modelUser.Person.Surname = person.Surname;
                    modelUser.Person.FatherName = person.FatherName;
                    modelUser.createdUser = person.profilePicture;
                    modelUser.Person.gender = person.gender;
                    if (person.birtday != null)
                    {
                        modelUser.Person.birtday = person.birtday;
                    }
                    modelUser.Person.UserId = person.UserId;

                    //sUid = (long)person.UserId;

                    if (person.profilePicture != null)
                    {
                        modelUser.profilePicture = Convert.ToBase64String(StringExtension.StringToByteArray(person.profilePicture));
                    }
                    modelUser.createdUser = person.profilePicture;

                    BaseOutput gabui = srv.WS_GetAddressesByUserId(baseInput, (long)person.UserId, true, out modelUser.AddressArray);

                    modelUser.Address = modelUser.AddressArray.ToList().FirstOrDefault();
                    auid = (long)modelUser.Address.adminUnit_Id;
                    addressDesc = modelUser.Address.addressDesc;

                    //orgRoles = "producerPerson";
                }
                else if (iamasPerson != null)
                {
                    if (iamasPerson.pin != null)
                    {
                        if (iamasPerson.Adress != null)
                        {
                            if (iamasPerson.Adress.cityId != null)
                            {
                                if (iamasPerson.Adress.cityId != "0")
                                {
                                    BaseOutput gaufci = srv.WS_GetPRM_AdminUnitByIamasId(baseInput, Int64.Parse(iamasPerson.Adress.cityId), true, true, true, out modelUser.PRMAdminUnit);
                                }
                            }

                            if (iamasPerson.Adress.districtId != null)
                            {
                                if (iamasPerson.Adress.districtId != "0")
                                {
                                    BaseOutput gaufci = srv.WS_GetPRM_AdminUnitByIamasId(baseInput, Int64.Parse(iamasPerson.Adress.districtId), true, false, true, out modelUser.PRMAdminUnit);
                                }
                            }

                            auid = modelUser.PRMAdminUnit.Id;


                            addressDesc = "";

                            if (!String.IsNullOrEmpty(iamasPerson.Adress.place))
                                addressDesc += iamasPerson.Adress.place;

                            if (!String.IsNullOrEmpty(iamasPerson.Adress.street))
                                addressDesc += ", " + iamasPerson.Adress.street;

                            if (!String.IsNullOrEmpty(iamasPerson.Adress.building))
                                addressDesc += ", " + iamasPerson.Adress.building;

                            if (!String.IsNullOrEmpty(iamasPerson.Adress.block))
                                addressDesc += ", " + iamasPerson.Adress.block;

                            if (!String.IsNullOrEmpty(iamasPerson.Adress.apartment))
                                addressDesc += ", " + iamasPerson.Adress.apartment;

                        }
                        //addressDesc = iamasPerson.Adress.place + ", " + iamasPerson.Adress.street + ", " + iamasPerson.Adress.apartment + ", " + iamasPerson.Adress.block + ", " + iamasPerson.Adress.building;

                        modelUser.Person.Name = iamasPerson.Name;
                        modelUser.Person.Surname = iamasPerson.Surname;
                        if (iamasPerson.Patronymic != null)
                        {
                            string[] pa = iamasPerson.Patronymic.Split(' ').ToArray();
                            modelUser.Person.FatherName = pa[0];
                        }
                        modelUser.Person.gender = iamasPerson.gender;
                        
                        modelUser.Person.birtday = (long)(DateTime.Parse(iamasPerson.birthDate)).getInt64ShortDate();
                        

                        if (iamasPerson.photo != null)
                        {
                            modelUser.createdUser = string.Join(",", iamasPerson.photo);
                            modelUser.profilePicture = Convert.ToBase64String(iamasPerson.photo);
                        }

                        if (iamasPerson.BirthPlace.country != "0")
                        {
                            fullAddressId += "1";
                        }

                        if (iamasPerson.Adress.cityId != "0")
                        {
                            BaseOutput gaufci = srv.WS_GetPRM_AdminUnitByIamasId(baseInput, Int64.Parse(iamasPerson.Adress.cityId), true, true, true, out modelUser.PRMAdminUnit);
                            fullAddressId += "," + modelUser.PRMAdminUnit.Id;
                        }

                        if (iamasPerson.Adress.districtId != "0")
                        {
                            BaseOutput gaufci = srv.WS_GetPRM_AdminUnitByIamasId(baseInput, Int64.Parse(iamasPerson.Adress.districtId), true, false, true, out modelUser.PRMAdminUnit);
                            fullAddressId += "," + modelUser.PRMAdminUnit.Id;
                        }

                    }
                    //orgRoles = "sellerPerson";
                }
                else if (taxesService != null)
                {
                    modelUser.Person.Name = taxesService.Name;
                    modelUser.Person.Surname = taxesService.Surname;
                    string[] pa = taxesService.MidleName.Split(' ').ToArray();
                    modelUser.Person.FatherName = pa[0];
                    modelUser.legalPersonName = taxesService.FullName;
                }

                 BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, auid, true, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();

                fullAddressId = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Id));

                BaseOutput adminUnit = srv.WS_GetAdminUnitsByParentId(baseInput, 0, true, out modelUser.PRMAdminUnitArray);


                BaseOutput mobileN = srv.WS_GetEnumCategorysByName(baseInput, "mobilePhonePrefix", out modelUser.EnumCategory);
                BaseOutput mobileİd = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueMobilePhone = modelUser.EnumValueArray.ToList();

                BaseOutput educationN = srv.WS_GetEnumCategorysByName(baseInput, "Tehsil", out modelUser.EnumCategory);
                BaseOutput educationId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueEducationList = modelUser.EnumValueArray.ToList();

                BaseOutput jobN = srv.WS_GetEnumCategorysByName(baseInput, "İş", out modelUser.EnumCategory);
                BaseOutput jobId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueJobList = modelUser.EnumValueArray.ToList();

                return modelUser.Person;
            }
            catch (Exception ex)
            {
                return modelUser.Person;
            }
        }

        public JsonResult FillRelations(string pId)
        {
            try
            {
                UserViewModel model = new UserViewModel();
                BaseOutput adminUnit = srv.WS_GetAdminUnitsByParentId(baseInput, int.Parse(pId), true, out model.PRMAdminUnitArray);
                List<regionClass> list = new List<regionClass>();
                model.PRMAdminUnitList = model.PRMAdminUnitArray.OrderBy(x => x.Name).ToList();
                foreach (var item in model.PRMAdminUnitList)
                {
                    list.Add(new regionClass { id = item.Id.ToString(), name = item.Name.ToString(), parentId = item.ParentID.ToString() });
                }
                return Json(new { data = list });
            }
            catch (Exception ex) { return null; }
        }

        public JsonResult Check(string pId, string type)
        {
            try
            {
                baseInput = new BaseInput();
                modelUser = new UserViewModel();

                modelUser.Person = new tblPerson();
                modelUser.Address = new tblAddress();

                modelUser.Person = GetPhysicalPerson(pId, type);
                Uri str = new Uri(Request.UrlReferrer.AbsoluteUri);
                string strr = HttpUtility.ParseQueryString(str.Query).Get("uid");
                if (modelUser.Person.UserId == null && strr == null)
                {
                    //modelUser.birtday = (modelUser.Person.birtday).toShortDate().ToString();
                    if (modelUser.Person != null)
                    {
                        if (modelUser.Person.birtday !=null)
                        {
                            modelUser.birtday = String.Format("{0:d.M.yyyy}", (modelUser.Person.birtday).toShortDate());
                        }
                        else
                        {
                            modelUser.birtday = String.Format("{0:d.M.yyyy}", DateTime.Today);
                        }
                        modelUser.FullAddress = fullAddressId;
                        modelUser.descAddress = addressDesc;
                    }
                    return Json(new { data = modelUser });
                }
                else
                {
                    
                    if (strr != null)
                    {
                        if (modelUser.Person.UserId != long.Parse(strr))
                        {
                            return null;
                        }
                    }
                    if (modelUser.Person.UserId > 0)
                    {
                        //modelUser.birtday = (modelUser.Person.birtday).toShortDate().ToString();
                        if (modelUser.Person != null)
                        {
                            if (modelUser.Person.birtday != null)
                            {
                                modelUser.birtday = String.Format("{0:d.M.yyyy}", (modelUser.Person.birtday).toShortDate());
                            }
                            else
                            {
                                modelUser.birtday = String.Format("{0:d.M.yyyy}", DateTime.Today);
                            }
                            modelUser.FullAddress = fullAddressId;
                            modelUser.descAddress = addressDesc;
                        }
                        return Json(new { data = modelUser });
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex) { return null; }
        }

        public DateTime FromSecondToDate(long seconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(seconds);
        }

        public long ConvertStringYearMonthDayFormatToTimestamp(string form)
        {
            Regex regex = new Regex(@"\.");
            string[] dates = regex.Split(form);
            int year = Convert.ToInt32(dates[2]);
            int month = Convert.ToInt32(dates[1]);
            int day = Convert.ToInt32(dates[0]);
            DateTime dTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long birthday = (long)(dTime - sTime).TotalSeconds;

            return birthday;
        }

        public JsonResult CheckForExistence(string userName, string perefix, string mobile)
        {

            baseInput = new BaseInput();
            User modelUser = new User();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(baseInput, userName, out modelUser.FutureUser);

            BaseOutput pereOut = srv.WS_GetEnumValueById(baseInput, long.Parse(perefix), true, out modelUser.EnumValue);
            BaseOutput comOut = srv.WS_GetCommunications(baseInput, out modelUser.CommunicationInformationsArray);

            string fullMobNumb = modelUser.EnumValue.name + mobile;

            //BaseOutput checkEdxistenseOut = srv.WS_GetUsers(baseInput, out modelUser.UserArray);
            List<tblCommunication> UserFromNumber = modelUser.CommunicationInformationsArray.Where(x => x.communication == fullMobNumb).ToList();

            //BaseOutput personsOut = srv.WS_GetPersons(baseInput, out modelUser.PersonArray);
            //List<tblPerson> PersonFromPin = form.fin == null ? new List<tblPerson>() : modelUser.PersonArray.Where(x => x.PinNumber == form.fin).ToList();

            //BaseOutput orgOut = srv.WS_GetForeign_Organizations(baseInput, out modelUser.ForeignOrganisationArray);
            //List<tblForeign_Organization> OrgFromVoen = form.voen == null ? new List<tblForeign_Organization>() : modelUser.ForeignOrganisationArray.Where(x => x.voen == form.voen).ToList();

            //normal ucun olmalidir 
            //if (modelUser.FutureUser != null || UserFromEmail.Count != 0 || PersonFromPin.Count != 0 || OrgFromVoen.Count != 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            if (modelUser.FutureUser != null || UserFromNumber.Count != 0)
            {
                return Json(new { err = "İstifadəçi adı və ya nömrə artıq qeydiyyatdan keçib!", suc = "" });
            }
            else
            {
                return Json(new { suc = "İstifadəçi uğurla qeydiyyatdan keçdi", err = "" });
            }

        }
    }

    public class regionClass
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string name { get; set; }
    }
}
