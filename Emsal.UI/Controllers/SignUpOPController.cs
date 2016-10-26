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

namespace Emsal.UI.Controllers
{
    public class SignUpOPController : Controller
    {
        //
        // GET: /SignUpOP/

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private static string fullAddressId = "";
        private static string addressDesc = "";
        private static string fin = "";
        private static string sVoen = "";
        private static string orgRoles = "";
        private static long sUid = 0;
        private static string sType = null;
        private BaseInput baseInput;
        private UserViewModel modelUser;
        tblPRM_AdminUnit tblAdminUnit;

        

        public ActionResult Index(long uid = 0, string type = null)
        {
            try
            {
                UserViewModel model = new UserViewModel();

                baseInput = new BaseInput();
                modelUser = new UserViewModel();

                long userId = 0;
                sUid = uid;
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

                    modelUser.Person =new tblPerson();
                    //modelUser.fin = modelP.Person.PinNumber;
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
            baseInput = new BaseInput();
            User modelUser = new User();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(baseInput, form.userName, out modelUser.FutureUser);

            BaseOutput checkEdxistenseOut = srv.WS_GetUsers(baseInput, out modelUser.UserArray);
            List<tblUser> UserFromEmail = modelUser.UserArray.Where(x => x.Email == form.eMail).ToList();

            BaseOutput personsOut = srv.WS_GetPersons(baseInput, out modelUser.PersonArray);
            List<tblPerson> PersonFromPin = form.fin == null ? new List<tblPerson>() : modelUser.PersonArray.Where(x => x.PinNumber == form.fin).ToList();

            BaseOutput orgOut = srv.WS_GetForeign_Organizations(baseInput, out modelUser.ForeignOrganisationArray);
            List<tblForeign_Organization> OrgFromVoen = form.voen == null ? new List<tblForeign_Organization>() : modelUser.ForeignOrganisationArray.Where(x => x.voen == form.voen).ToList();

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
                    BaseOutput uidBase = srv.WS_GetUserById(baseInput, sUid, true, out modelUser.User);
                    if (modelUser.User != null)
                    {
                        BaseOutput gabui = srv.WS_GetAddressesByUserId(baseInput, modelUser.User.Id, true, out modelUser.AddressArray);
                        modelUser.Address = modelUser.AddressArray.ToList().FirstOrDefault();
                        BaseOutput gpbui = srv.WS_GetPersonByUserId(baseInput, modelUser.User.Id, true, out modelUser.Person);
                        //BaseOutput cominicationOut = srv.WS_GetCommunicationByPersonId(baseInput,(long)modelUser.Person.Id, true, out modelUser.Comminication);
                    }

                    if (modelUser.User == null)
                    {
                        modelUser.User = new tblUser();
                        modelUser.Address = new tblAddress();
                        modelUser.Person = new tblPerson();
                        if (!String.IsNullOrEmpty(sVoen))
                        {
                            modelUser.ForeignOrganisation = new tblForeign_Organization();
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(sVoen))
                        {
                            BaseOutput foreign = srv.WS_GetForeign_OrganizationByVoen(baseInput, sVoen, out modelUser.ForeignOrganisation);
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
                    if (String.IsNullOrEmpty(sVoen))
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

                    if (String.IsNullOrEmpty(sVoen)) {
                        modelUser.Person.PinNumber = mdl.fin;
                        modelUser.Person.address_Id = modelUser.Address.Id;
                    }

                    modelUser.Person.address_IdSpecified = true;
                    modelUser.Person.gender = mdl.gender;
                    //modelUser.Person.birtday = (DateTime.Parse(mdl.birtday)).getInt64ShortDate();
                    modelUser.Person.birtday = ConvertStringYearMonthDayFormatToTimestamp(mdl.birtday);
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


                    if (modelUser.ForeignOrganisation!=null)
                    {
                        modelUser.ForeignOrganisation.name = mdl.legalPersonName;
                        modelUser.ForeignOrganisation.voen = mdl.voen;
                        modelUser.ForeignOrganisation.userId = modelUser.User.Id;
                        modelUser.ForeignOrganisation.userIdSpecified = true;
                        modelUser.ForeignOrganisation.address_Id = adrrId;
                        modelUser.ForeignOrganisation.address_IdSpecified = true;

                        if (modelUser.ForeignOrganisation.Id > 0)
                        {
                            BaseOutput fO = srv.WS_UpdateForeign_Organization(baseInput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);
                        }
                        else
                        {
                            BaseOutput fO = srv.WS_AddForeign_Organization(baseInput, modelUser.ForeignOrganisation, out modelUser.ForeignOrganisation);
                        }
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
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public tblPerson GetPhysicalPerson(string fin, string type)
        {
            sVoen = "";
            sType = "";
            
            modelUser = new UserViewModel();
            baseInput = new BaseInput();
            try
            {
                                            
                SingleServiceControl srvcontrol = new SingleServiceControl();
                getPersonalInfoByPinNewResponseResponse iamasPerson = null;
                tblPerson person = null;
                tblForeign_Organization foreignOrg;
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
                    sVoen = fin;
                    if (foreignOrg != null) { 
                        BaseOutput personOut = srv.WS_GetPersonByUserId(baseInput, Int64.Parse(foreignOrg.userId.ToString()), true, out person);
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
                        modelUser.Person.birtday = (DateTime.Parse(FromSecondToDate((long)person.birtday).ToString())).getInt64ShortDate(); 
                        modelUser.Person.UserId = person.UserId;

                        sUid =(long) person.UserId;
                        
                        modelUser.profilePicture = Convert.ToBase64String(StringExtension.StringToByteArray(person.profilePicture));
                        modelUser.createdUser = person.profilePicture;

                        BaseOutput gabui = srv.WS_GetAddressesByUserId(baseInput, (long)person.UserId, true, out modelUser.AddressArray);

                        modelUser.Address = modelUser.AddressArray.ToList().FirstOrDefault();
                        auid = (long)modelUser.Address.adminUnit_Id;
                        addressDesc = modelUser.Address.addressDesc;

                        //orgRoles = "producerPerson";
                }
                    else if (iamasPerson != null)
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

                        //addressDesc = iamasPerson.Adress.place + ", " + iamasPerson.Adress.street + ", " + iamasPerson.Adress.apartment + ", " + iamasPerson.Adress.block + ", " + iamasPerson.Adress.building;

                        modelUser.Person.Name = iamasPerson.Name;
                        modelUser.Person.Surname = iamasPerson.Surname;
                        string[] pa = iamasPerson.Patronymic.Split(' ').ToArray();
                        modelUser.Person.FatherName = pa[0];
                        modelUser.Person.gender = iamasPerson.gender;
                        modelUser.Person.birtday = (DateTime.Parse(iamasPerson.birthDate)).getInt64ShortDate();
                        modelUser.createdUser = string.Join(",", iamasPerson.photo);
                        modelUser.profilePicture = Convert.ToBase64String(iamasPerson.photo);

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


                        //orgRoles = "sellerPerson";
                }
                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, auid, true, out modelUser.PRMAdminUnitArray);
                modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.ToList();

                fullAddressId = string.Join(",", modelUser.PRMAdminUnitList.Select(x => x.Id));

                BaseOutput adminUnit = srv.WS_GetAdminUnitsByParentId(baseInput, 0, true, out modelUser.PRMAdminUnitArray);


                BaseOutput mobileN = srv.WS_GetEnumCategorysByName(baseInput, "mobilePhonePrefix", out modelUser.EnumCategory);
                BaseOutput mobileİd = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
                modelUser.EnumValueMobilePhone = modelUser.EnumValueArray.ToList();

                BaseOutput educationN = srv.WS_GetEnumCategorysByName(baseInput, "Tehsil", out modelUser.EnumCategory);
                BaseOutput educationId = srv.WS_GetEnumValuesByEnumCategoryId(baseInput,modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
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
            try { 
            UserViewModel model = new UserViewModel();
            BaseOutput adminUnit = srv.WS_GetAdminUnitsByParentId(baseInput, int.Parse(pId), true, out model.PRMAdminUnitArray);
            List<regionClass> list = new List<regionClass>();
            foreach (var item in model.PRMAdminUnitArray)
            {
                list.Add(new regionClass { id = item.Id.ToString(), name = item.Name.ToString(), parentId = item.ParentID.ToString() });
            }
            return  Json(new {data = list});
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
                if (sUid == 0)
                {
                    //modelUser.birtday = (modelUser.Person.birtday).toShortDate().ToString();
                    if (modelUser.Person != null)
                    {
                        modelUser.birtday = String.Format("{0:d.M.yyyy}", (modelUser.Person.birtday).toShortDate());
                        modelUser.FullAddress = fullAddressId;
                        modelUser.descAddress = addressDesc;
                    }
                    else
                    {
                        modelUser.birtday = String.Format("{0:d.M.yyyy}", DateTime.Today);
                    }

                        return Json(new { data = modelUser });
                }
                else
                {
                    if (modelUser.Person.UserId == sUid)
                    {
                        //modelUser.birtday = (modelUser.Person.birtday).toShortDate().ToString();
                        if (modelUser.Person != null)
                        {
                            modelUser.birtday = String.Format("{0:d.M.yyyy}", (modelUser.Person.birtday).toShortDate());
                            modelUser.FullAddress = fullAddressId;
                            modelUser.descAddress = addressDesc;
                        }
                        else
                        {
                            modelUser.birtday = String.Format("{0:d.M.yyyy}", DateTime.Today);
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

        public JsonResult CheckForExistence(string userName, string email ) {

            baseInput = new BaseInput();
            User modelUser = new User();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(baseInput, userName, out modelUser.FutureUser);

            BaseOutput checkEdxistenseOut = srv.WS_GetUsers(baseInput, out modelUser.UserArray);
            List<tblUser> UserFromEmail = modelUser.UserArray.Where(x => x.Email == email).ToList();

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

            if (modelUser.FutureUser != null || UserFromEmail.Count != 0)
            {
                return Json(new { err = "İstifadəçi artıq qeydiyyatdan keçib!", suc = "" });
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
