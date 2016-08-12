using System;
using System.Linq;
using System.Web.Mvc;
using Emsal.WebInt.EmsalSrv;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using Emsal.UI.Models;
using System.Web.Security;
using System.Web;
using Emsal.UI.Infrastructure;
using System.Collections.Generic;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.specialSummary)]

    public class SpecialSummaryController : Controller
    {

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private BaseInput binput;
        UserViewModel modelUser;
        //
        // GET: /SpecialSummary/

        public ActionResult Index(long? UserId)
        {
            binput = new BaseInput();

            modelUser = new UserViewModel();
            modelUser.RoleTypes = new List<string>();
            //tehsilin enum categorisini getirib id-sini alırıq
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.User);
                BaseOutput person = srv.WS_GetPersonByUserId(binput, (long)UserId, true, out modelUser.Person);
                BaseOutput enumcatEd = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelUser.EnumCategory);
                if (modelUser.EnumCategory == null)
                {
                    modelUser.EnumCategory = new tblEnumCategory();
                }
                BaseOutput enumvalEd = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueEducationArray);

                //işin enumcategorisini getirib idsini alırıq
                BaseOutput enumcatJob = srv.WS_GetEnumCategorysByName(binput, "İş", out modelUser.EnumCategory);
                if (modelUser.EnumCategory == null)
                {
                    modelUser.EnumCategory = new tblEnumCategory();
                }
                BaseOutput enumvalJob = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueJobArray);


                modelUser.EnumValueEducationList = modelUser.EnumValueEducationArray.ToList();
                modelUser.EnumValueJobList = modelUser.EnumValueJobArray.ToList();

                BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, (long)UserId, true, out modelUser.UserRoleArray);

                BaseOutput rolesOut = srv.WS_GetRoleByName(binput, "producerPerson", out modelUser.Role);
                long clientPersonId = modelUser.Role.Id;

                BaseOutput rolesOutSeler = srv.WS_GetRoleByName(binput, "sellerPerson", out modelUser.Role);
                long sellerPersonId = modelUser.Role.Id;

                if (modelUser.UserRoleArray.Length > 1)
                {
                    foreach (var item in modelUser.UserRoleArray)
                    {
                        if (item.RoleId == clientPersonId)
                        {
                            modelUser.RoleTypes.Add("producerPerson");
                        }
                        else if(item.RoleId == sellerPersonId)
                        {
                            modelUser.RoleTypes.Add("sellerPerson");
                        }
                    }
                    string roles = string.Join("", modelUser.RoleTypes);
                    if(roles.Contains("producerPerson") && roles.Contains("sellerPerson"))
                    {
                        modelUser.RoleType = "clSelPerson";
                    }
                    else if (roles.Contains("producerPerson"))
                    {
                        modelUser.RoleType = "producerPerson";
                    }
                    else if (roles.Contains("sellerPerson"))
                    {
                        modelUser.RoleType = "sellerPerson";
                    }
                }
                else
                {
                    if(modelUser.UserRoleArray[0].RoleId == clientPersonId)
                    {
                        modelUser.RoleType = "producerPerson";
                    }
                    else
                    {
                        modelUser.RoleType = "sellerPerson";
                    }
                }
                
                return View(modelUser);
            }
            catch (Exception err)
            {
                return View(err.Message); 
            }
           
        }

        public JsonResult GetUserById(long Id)
        {
            binput = new BaseInput();

            UserViewModel modelUser = new UserViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    if(Id == 1)
                    {
                        Id = Int32.Parse(identity.Ticket.UserData);
                    }
                }
            }

            try
            {
                BaseOutput bout = srv.WS_GetUserById(binput, Id, true, out modelUser.User);
                BaseOutput personBout = srv.WS_GetPersonByUserId(binput, Id, true, out modelUser.Person);
                BaseOutput educOut = srv.WS_GetEnumValueById(binput, (long)modelUser.Person.educationLevel_eV_Id, true, out modelUser.EducationEnumValue);
                modelUser.EducationLevel = modelUser.EducationEnumValue.name;
                BaseOutput jobOut = srv.WS_GetEnumValueById(binput, (long)modelUser.Person.job_eV_Id, true, out modelUser.JobEnumValue);
                modelUser.Job = modelUser.JobEnumValue.name;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return Json(modelUser, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateUser(
           string userName,
           string gender = null,
           int? educationId = null,
           int? jobId = null,
           string job = null,
           int? userId = null,
           int? personId = null,
           string email = null,
           int? userType = null,
           bool IdSpecified = true
           )
        {
            binput = new BaseInput();

            modelUser = new UserViewModel();

            modelUser.Person = new tblPerson();
            modelUser.EnumCategory = new tblEnumCategory();
            modelUser.EducationEnumValue = new tblEnumValue();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            try
            {
                BaseOutput userOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
                BaseOutput personOutPut = srv.WS_GetPersonByUserId(binput, (long)userId, true, out modelUser.Person);
                personId = (int)modelUser.Person.Id;
                modelUser.User.Username = userName;
                modelUser.User.Id = (long)userId;
                modelUser.User.Status = 1;
                modelUser.User.Email = email;

                modelUser.User.IdSpecified = true;
                modelUser.User.StatusSpecified = true;

                if (personId != null)
                {
                    modelUser.Person.Status = 1;
                    modelUser.Person.gender = gender;
                    modelUser.Person.Id = (long)personId;
                    modelUser.Person.educationLevel_eV_Id = educationId;
                    modelUser.Person.job_eV_Id = jobId;

                    modelUser.Person.IdSpecified = true;
                    modelUser.Person.StatusSpecified = true;
                    modelUser.Person.birtdaySpecified = true;
                    modelUser.Person.educationLevel_eV_IdSpecified = true;
                    modelUser.Person.job_eV_IdSpecified = true;

                }

                BaseOutput pout = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);
                BaseOutput personOut = srv.WS_UpdatePerson(binput, modelUser.Person, out modelUser.Person);

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }
           
        }

        public string CheckPassword(
           int? userId,
           string password,
           bool IdSpecified = true)
        {
            binput = new BaseInput();

            modelUser = new UserViewModel();

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

        //to change the current password
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

            modelUser = new UserViewModel();

        

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


        public JsonResult GetOfferProductionsByUserId(int UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();

            try
            {
                BaseOutput offer = srv.WS_GetOffer_ProductionsByUserID(binput, UserID, true, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                return Json(modelOfferProducts.OfferProductionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
           
        }

        public JsonResult GetOffAirOfferProductionsByUserId(int userId)
        {
            binput = new BaseInput();
            ProductsViewModel modelOfferProducts = new ProductsViewModel();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelOfferProducts.EnumVal);

                modelOfferProducts.OfferProduction = new tblOffer_Production();

                modelOfferProducts.OfferProduction.user_Id = userId;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserID(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                return Json(modelOfferProducts.OfferProductionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
           
        }

        public JsonResult GetOnAirOfferProductionsByUserId(int userId)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();

                modelOfferProducts.OfferProduction.user_Id = userId;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                return Json(modelOfferProducts.OfferProductionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult OnAirOffers(int UserID)
        {
            var a = User.Identity.ToString();

            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if(itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    } 
                }

                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OnAirOffersSortedForDateAsc(int UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserIDSortedForDate(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OnAirOffersSortedForDateDes(int UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserIDSortedForDateDes(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OnAirOffersSortedForPriceAsc(int UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserIDSortedForPriceAsc(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OnAirOffersSortedForPriceDes(int? UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserIDSortedForPriceDes(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OffAirOffers(int? UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserID(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OffAirOffersSortedForDateAsc(int? UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserIDSortedForDate(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
            
        }

        public ActionResult OffAirOffersSortedForDateDes(int? UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserIDSortedForDateDes(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
        
        }

        public ActionResult OffAirOffersSortedForPriceAsc(int? UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserIDSortedForPriceAsc(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
       
        }

        public ActionResult OffAirOffersSortedForPriceDes(int? UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelOfferProducts.EnumVal);
                modelOfferProducts.OfferProduction = new tblOffer_Production();
                modelOfferProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelOfferProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelOfferProducts.OfferProduction.user_Id = UserID;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserIDSortedForPriceDes(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelOfferProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelOfferProducts.EnumCat.Id, true, out modelOfferProducts.EnumValueArray);

                modelOfferProducts.EnumValueList = modelOfferProducts.EnumValueArray.ToList();

                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelOfferProducts.ProductCatalog);
                    modelOfferProducts.ProductCatalogList.Add(modelOfferProducts.ProductCatalog);
                }
                foreach (var item in modelOfferProducts.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelOfferProducts.ProductionControlArray);
                    foreach (var itemm in modelOfferProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelOfferProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelOfferProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
         
        }

        public ActionResult ConfirmedPotential(int? UserID)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();
                modelPotentialProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelPotentialProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelPotentialProducts.PotentialProduction.user_Id = UserID;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserId(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProducts.EnumCat.Id, true, out modelPotentialProducts.EnumValueArray);

                modelPotentialProducts.EnumValueList = modelPotentialProducts.EnumValueArray.ToList();

                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelPotentialProducts.ProductCatalog);
                    modelPotentialProducts.ProductCatalogList.Add(modelPotentialProducts.ProductCatalog);
                }
                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelPotentialProducts.ProductionControlArray);
                    foreach (var itemm in modelPotentialProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelPotentialProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }

                return PartialView(modelPotentialProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
           
        }

        public ActionResult ConfirmedPotentialForPriceAsc(int? UserID)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();
                modelPotentialProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelPotentialProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelPotentialProducts.PotentialProduction.user_Id = UserID;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserIdForPriceAsc(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProducts.EnumCat.Id, true, out modelPotentialProducts.EnumValueArray);

                modelPotentialProducts.EnumValueList = modelPotentialProducts.EnumValueArray.ToList();

                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelPotentialProducts.ProductCatalog);
                    modelPotentialProducts.ProductCatalogList.Add(modelPotentialProducts.ProductCatalog);
                }
                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelPotentialProducts.ProductionControlArray);
                    foreach (var itemm in modelPotentialProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelPotentialProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelPotentialProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
        
        }

        public ActionResult ConfirmedPotentialForPriceDes(int? UserID)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();
                modelPotentialProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelPotentialProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelPotentialProducts.PotentialProduction.user_Id = UserID;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserIdForPriceDes(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProducts.EnumCat.Id, true, out modelPotentialProducts.EnumValueArray);

                modelPotentialProducts.EnumValueList = modelPotentialProducts.EnumValueArray.ToList();

                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelPotentialProducts.ProductCatalog);
                    modelPotentialProducts.ProductCatalogList.Add(modelPotentialProducts.ProductCatalog);
                }
                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelPotentialProducts.ProductionControlArray);
                    foreach (var itemm in modelPotentialProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelPotentialProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelPotentialProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
       
        }

        public ActionResult NotConfirmedPotential(int? UserID)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();
            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenmeyen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();
                modelPotentialProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelPotentialProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelPotentialProducts.PotentialProduction.user_Id = UserID;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserId(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProducts.EnumCat.Id, true, out modelPotentialProducts.EnumValueArray);

                modelPotentialProducts.EnumValueList = modelPotentialProducts.EnumValueArray.ToList();

                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelPotentialProducts.ProductCatalog);
                    modelPotentialProducts.ProductCatalogList.Add(modelPotentialProducts.ProductCatalog);
                }
                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelPotentialProducts.ProductionControlArray);
                    foreach (var itemm in modelPotentialProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelPotentialProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelPotentialProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
           
        }

        public ActionResult NotConfirmedPotentialForPriceAsc(int? UserID)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenmeyen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();
                modelPotentialProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelPotentialProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelPotentialProducts.PotentialProduction.user_Id = UserID;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserIdForPriceAsc(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProducts.EnumCat.Id, true, out modelPotentialProducts.EnumValueArray);

                modelPotentialProducts.EnumValueList = modelPotentialProducts.EnumValueArray.ToList();

                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelPotentialProducts.ProductCatalog);
                    modelPotentialProducts.ProductCatalogList.Add(modelPotentialProducts.ProductCatalog);
                }
                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelPotentialProducts.ProductionControlArray);
                    foreach (var itemm in modelPotentialProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelPotentialProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelPotentialProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }

        }

        public ActionResult NotConfirmedPotentialForPriceDes(int? UserID)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenmeyen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();
                modelPotentialProducts.ProductCatalogList = new List<tblProductCatalog>();
                modelPotentialProducts.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelPotentialProducts.PotentialProduction.user_Id = UserID;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserIdForPriceDes(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProducts.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProducts.EnumCat.Id, true, out modelPotentialProducts.EnumValueArray);

                modelPotentialProducts.EnumValueList = modelPotentialProducts.EnumValueArray.ToList();

                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelPotentialProducts.ProductCatalog);
                    modelPotentialProducts.ProductCatalogList.Add(modelPotentialProducts.ProductCatalog);
                }
                foreach (var item in modelPotentialProducts.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelPotentialProducts.ProductionControlArray);
                    foreach (var itemm in modelPotentialProducts.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelPotentialProducts.ProductionControlList.Add(itemm);
                        }
                    }
                }
                return PartialView(modelPotentialProducts);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
         
        }

        public JsonResult GetOnAirOfferProductionsSortedForPriceAscByUserId(int? userId)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);

                modelOfferProducts.OfferProduction = new tblOffer_Production();

                modelOfferProducts.OfferProduction.user_Id = userId;
                modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

                modelOfferProducts.OfferProduction.user_IdSpecified = true;
                modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

                //modelOfferProducts.OfferProductionList = modelOfferProducts.OfferProductionArray.ToList();

                return Json(modelOfferProducts.OfferProductionArray, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
        
        }

        public JsonResult GetPotentialProductionsByUserId(int userId)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();

            try
            {
                BaseOutput offer = srv.WS_GetPotential_ProductionsByUserId(binput, userId, true, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                return Json(modelPotentialProducts.PotentialProductionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
         
        }

        public JsonResult GetConfirmedPotential_ProductionsByStateAndUserId(int userId)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {

                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();

                modelPotentialProducts.PotentialProduction.user_Id = userId;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserId(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);
                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                return Json(modelPotentialProducts.PotentialProductionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult GetNotConfirmedPotential_ProductionsByStateAndUserId(int userId)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenmeyen", out modelPotentialProducts.EnumVal);
                modelPotentialProducts.PotentialProduction = new tblPotential_Production();

                modelPotentialProducts.PotentialProduction.user_Id = userId;
                modelPotentialProducts.PotentialProduction.state_eV_Id = modelPotentialProducts.EnumVal.Id;

                modelPotentialProducts.PotentialProduction.user_IdSpecified = true;
                modelPotentialProducts.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserId(binput, modelPotentialProducts.PotentialProduction, out modelPotentialProducts.PotentialProductionArray);

                modelPotentialProducts.PotentialProductionList = modelPotentialProducts.PotentialProductionArray.ToList();

                return Json(modelPotentialProducts.PotentialProductionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
        

        }

        public JsonResult GetProductionDocumentsByOfferProductionId(int prodId)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();

            modelOfferProducts.ProductionDocument = new tblProduction_Document();


            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "offer", out modelOfferProducts.EnumVal);

                modelOfferProducts.ProductionDocument.Offer_Production_Id = prodId;
                modelOfferProducts.ProductionDocument.Production_type_eV_Id = modelOfferProducts.EnumVal.Id;
                modelOfferProducts.ProductionDocument.Offer_Production_IdSpecified = true;
                modelOfferProducts.ProductionDocument.Production_type_eV_IdSpecified = true;


                BaseOutput offer = srv.WS_GetProductionDocumentsByOffer_Production_Id(binput, modelOfferProducts.ProductionDocument, out modelOfferProducts.ProductionDocumentArray);

                modelOfferProducts.ProductionDocumentList = modelOfferProducts.ProductionDocumentArray.ToList();

                return Json(modelOfferProducts.ProductionDocumentList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
         
        }

        public JsonResult GetProductionDocumentsByPotentialProductionId(int prodId)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();

            modelPotentialProducts.ProductionDocument = new tblProduction_Document();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Potential", out modelPotentialProducts.EnumVal);

                modelPotentialProducts.ProductionDocument.Potential_Production_Id = prodId;
                modelPotentialProducts.ProductionDocument.Production_type_eV_Id = modelPotentialProducts.EnumVal.Id;
                modelPotentialProducts.ProductionDocument.Potential_Production_IdSpecified = true;
                modelPotentialProducts.ProductionDocument.Production_type_eV_IdSpecified = true;


                BaseOutput offer = srv.WS_GetProductionDocumentsByPotential_Production_Id(binput, modelPotentialProducts.ProductionDocument, out modelPotentialProducts.ProductionDocumentArray);

                modelPotentialProducts.ProductionDocumentList = modelPotentialProducts.ProductionDocumentArray.ToList();

                return Json(modelPotentialProducts.ProductionDocument, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
        
        }

        public JsonResult GetSentMessagesByFromUserId(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            try
            {
                BaseOutput bout = srv.WS_GetComMessagesyByFromUserId(binput, userId, true, out userModel.ComMessageArray);

                userModel.ComMessageList = userModel.ComMessageArray.ToList();
                return Json(userModel.ComMessageList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
           

        }

        public JsonResult GetInboxMessagesByToUserId(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            try
            {
                BaseOutput bout = srv.WS_GetComMessagesyByToUserId(binput, userId, true, out userModel.ComMessageArray);

                userModel.ComMessageList = userModel.ComMessageArray.ToList();
                return Json(userModel.ComMessageList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                return Json(err.Message, JsonRequestBehavior.AllowGet);
            }
  

        }

        public ActionResult DeleteComMessage(int Id)
        {
            UserViewModel UserModel = new UserViewModel();


            binput = new BaseInput();
            try
            {
                BaseOutput bout = srv.WS_GetComMessageById(binput, Id, true, out UserModel.ComMessage);

                BaseOutput dout = srv.WS_DeleteComMessage(binput, UserModel.ComMessage);

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }
         
        }

        public ActionResult DeleteOfferProduction(int Id)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();

            modelOfferProducts.OfferProduction = new tblOffer_Production();

            try
            {
                binput = new BaseInput();

                BaseOutput Offer = srv.WS_GetOffer_ProductionById(binput, Id, true, out modelOfferProducts.OfferProduction);

                BaseOutput delet = srv.WS_DeleteOffer_Production(binput, modelOfferProducts.OfferProduction);

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }
           
        }

        public ActionResult DeletePotentialProduction(int Id)
        {
            ProductsViewModel modelPotentialProducts = new ProductsViewModel();

            modelPotentialProducts.PotentialProduction = new tblPotential_Production();
            try
            {
                binput = new BaseInput();
                BaseOutput Offer = srv.WS_GetPotential_ProductionById(binput, Id, true, out modelPotentialProducts.PotentialProduction);

                BaseOutput delet = srv.WS_DeletePotential_Production(binput, modelPotentialProducts.PotentialProduction);

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }
           
        }

        public ActionResult PutToOfferProduction(int Id)
        {

            ProductsViewModel modelProducts = new ProductsViewModel();

            modelProducts.PotentialProduction = new tblPotential_Production();
            modelProducts.OfferProduction = new tblOffer_Production();
            modelProducts.ProductionDocument = new tblProduction_Document();
            modelProducts.EnumVal = new tblEnumValue();

            try
            {
                binput = new BaseInput();
                //get potential production u want to put to offer
                BaseOutput Production = srv.WS_GetPotential_ProductionById(binput, Id, true, out modelProducts.PotentialProduction);
                ///////////////////////////////

                //Delete potential production u want to put to offer, from potential production table



                BaseOutput delet = srv.WS_DeletePotential_Production(binput, modelProducts.PotentialProduction);

                ////////////////////////////////////////////////////

                //Add it to the offers table

                //get the enum Id of yayinda
                modelProducts.EnumVal = new tblEnumValue();
                BaseOutput eval = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelProducts.EnumVal);
                long enumYayinda = modelProducts.EnumVal.Id;
                ///////////////////////////

                modelProducts.OfferProduction.title = modelProducts.PotentialProduction.title;
                modelProducts.OfferProduction.description = modelProducts.PotentialProduction.description;
                modelProducts.OfferProduction.grup_Id = modelProducts.PotentialProduction.grup_Id;
                modelProducts.OfferProduction.quantity = modelProducts.PotentialProduction.quantity;
                modelProducts.OfferProduction.state_eV_Id = enumYayinda;
                modelProducts.OfferProduction.Status = 1;
                modelProducts.OfferProduction.total_price = modelProducts.PotentialProduction.total_price;
                modelProducts.OfferProduction.unit_price = modelProducts.PotentialProduction.unit_price;
                modelProducts.OfferProduction.updatedUser = modelProducts.PotentialProduction.updatedUser;
                modelProducts.OfferProduction.user_Id = modelProducts.PotentialProduction.user_Id;
                modelProducts.OfferProduction.createdUser = modelProducts.PotentialProduction.createdUser;
                modelProducts.OfferProduction.startDate = modelProducts.PotentialProduction.startDate;
                modelProducts.OfferProduction.endDate = modelProducts.PotentialProduction.endDate;
                modelProducts.OfferProduction.product_Id = modelProducts.PotentialProduction.product_Id;
                modelProducts.OfferProduction.productAddress_Id = modelProducts.PotentialProduction.productAddress_Id;

                modelProducts.OfferProduction.StatusSpecified = true;
                modelProducts.OfferProduction.state_eV_IdSpecified = true;
                modelProducts.OfferProduction.user_IdSpecified = true;
                modelProducts.OfferProduction.unit_priceSpecified = true;
                modelProducts.OfferProduction.total_priceSpecified = true;
                modelProducts.OfferProduction.endDateSpecified = true;
                modelProducts.OfferProduction.quantitySpecified = true;
                modelProducts.OfferProduction.product_IdSpecified = true;
                modelProducts.OfferProduction.productAddress_IdSpecified = true;
                modelProducts.OfferProduction.startDateSpecified = true;
                BaseOutput addProduct = srv.WS_AddOffer_Production(binput, modelProducts.OfferProduction, out modelProducts.OfferProduction);

                ////////////////////////////////


                //Get the production documents wich have production id of the selected potential production item,

                //get the enum Id of tesdiqlenen
                modelProducts.EnumVal = new tblEnumValue();
                BaseOutput eval2 = srv.WS_GetEnumValueByName(binput, "Potential", out modelProducts.EnumVal);
                long prodTypeeVId = modelProducts.EnumVal.Id;
                ///////////////////////////

                //get enumId of offer
                modelProducts.EnumVal = new tblEnumValue();
                BaseOutput eval3 = srv.WS_GetEnumValueByName(binput, "Offer", out modelProducts.EnumVal);
                long offerTypeeVId = modelProducts.EnumVal.Id;
                ///

                modelProducts.ProductionDocument.Potential_Production_Id = Id;
                modelProducts.ProductionDocument.Production_type_eV_Id = prodTypeeVId;

                modelProducts.ProductionDocument.Production_type_eV_IdSpecified = true;
                modelProducts.ProductionDocument.Potential_Production_IdSpecified = true;

                BaseOutput prodDoc = srv.WS_GetProductionDocumentsByPotential_Production_Id(binput, modelProducts.ProductionDocument, out modelProducts.ProductionDocumentArray);

                modelProducts.ProductionDocumentList = modelProducts.ProductionDocumentArray.ToList();

                ////////////////////////////



                //update production documents
                foreach (var item in modelProducts.ProductionDocumentArray)
                {
                    binput = new BaseInput();
                    modelProducts.ProductionDocument = item;
                    modelProducts.ProductionDocument.Offer_Production_Id = modelProducts.OfferProduction.Id;
                    modelProducts.ProductionDocument.Offer_Production_IdSpecified = true;
                    modelProducts.ProductionDocument.Production_type_eV_Id = offerTypeeVId;

                    srv.WS_UpdateProductionDocument(binput, item, out modelProducts.ProductionDocument);
                }

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }
       
        }

        public ActionResult PotensialClientInsert()
        {
            return View();
        }

        public ActionResult PhoneRegister1()
        {
            return View();
        }

        public ActionResult PhoneRegister2()
        {
            return View();
        }

        public ActionResult PhoneRegisterLast()
        {
            return View();
        }
 
        //send message demo
        public bool SendConfirmationMessage(string phoneNumEntered)
        {
            BaseInput baseinput = new BaseInput();
            tblConfirmationMessage ConfirmationMessage = new tblConfirmationMessage();
            Regex checkPhone = new Regex(@"^[\d*]{7}$");
            Match match = checkPhone.Match(phoneNumEntered);
            if (match.Success)
            {
                Random rnd = new Random();
                int reqem = rnd.Next(100000, 1000000);
                ConfirmationMessage.Message = reqem.ToString();
              
                BaseOutput pout = srv.WS_SendConfirmationMessage(baseinput, ConfirmationMessage);
                SendMail(ConfirmationMessage.Message);
                return true;
            }
            else
            {
                return false;
            }

        }
        private void SendMail(string kod = null)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("ferid.heziyev@gmail.com", "emsal.az");
            msg.To.Add("qala2009@gmail.com");
            string fromPassword = "e1701895";
            msg.Subject = "Üzvlüyü tesdiqle";
            if(kod == null)
            {
                msg.Body = "<a href = 'http://localhost:56557/SpecialSummary/Index/'>Hesabınızı təsdiqləyin</a>";
            }
            else
            {
                msg.Body = "<p>Doğrulama kodunuz:" + kod + "</p>";
            }
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
            smtp.Timeout = 20000;
            smtp.Send(msg);
        }
        public bool CheckConfirmationMessage(string message)
        {
            tblConfirmationMessage[] confirmationMessages;
            BaseInput baseinput = new BaseInput();
            BaseOutput bouput = srv.WS_GetConfirmationMessages(baseinput,out confirmationMessages);

            bool stance = false;

            foreach (tblConfirmationMessage item in confirmationMessages)
            {
                if (message == item.Message)
                {
                    SendMail();
                    stance = true;
                }
            }

            return stance;
        }

        public JsonResult GetEnumValueByName(string name)
        {
            ProductsViewModel modelEnums = new ProductsViewModel();

            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, name, out modelEnums.EnumVal);

            return Json(modelEnums.EnumVal, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoleByName(string name)
        {
            UserViewModel modelRoles = new UserViewModel();

            BaseOutput roleVal = srv.WS_GetRoleByName(binput, name, out modelRoles.Role);

            return Json(modelRoles.Role, JsonRequestBehavior.AllowGet);
        }
        public void UpdateEmail(string email, int? userId)
        {
            binput = new BaseInput();

            modelUser = new UserViewModel();

            modelUser.User = new tblUser();
            modelUser.Person = new tblPerson();
            modelUser.EnumCategory = new tblEnumCategory();
            modelUser.EducationEnumValue = new tblEnumValue();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            try
            {
                BaseOutput userOutput = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

                modelUser.User.Username = modelUser.User.Username;
                modelUser.User.Id = (long)userId;
                modelUser.User.Status = 1;
                modelUser.User.Email = email;

                modelUser.User.IdSpecified = true;
                modelUser.User.StatusSpecified = true;

                BaseOutput pout = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
         
        }


        public void PutToOffAirProduction(int offerId)
        {
            ProductsViewModel modelProducts = new ProductsViewModel();

            modelProducts.ProductionDocument = new tblProduction_Document();
            modelProducts.EnumVal = new tblEnumValue();

            binput = new BaseInput();
            //get offer production u want to put to off air offer
            BaseOutput Production = srv.WS_GetOffer_ProductionById(binput, offerId, true, out modelProducts.OfferProduction);
            ///////////////////////////////

            //Delete offer production u want to put to off air offer, from offer production table


            BaseOutput delet = srv.WS_DeleteOffer_Production(binput, modelProducts.OfferProduction);

            ////////////////////////////////////////////////////

            //Add it to the off air offers table

            //get the enum Id of yayinda
            modelProducts.EnumVal = new tblEnumValue();
            BaseOutput eval = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelProducts.EnumVal);
            long enumYayindaDeyil = modelProducts.EnumVal.Id;
            ///////////////////////////

            modelProducts.OfferProduction.state_eV_Id = enumYayindaDeyil;
            modelProducts.OfferProduction.Status = 1;

            modelProducts.OfferProduction.StatusSpecified = true;
            modelProducts.OfferProduction.state_eV_IdSpecified = true;
            BaseOutput addProduct = srv.WS_AddOffer_Production(binput, modelProducts.OfferProduction, out modelProducts.OfferProduction);

            ////////////////////////////////


        }

        public void UpdateOnAirOffers(int UserID)
        {
            ProductsViewModel modelOfferProducts = new ProductsViewModel();

            binput = new BaseInput();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelOfferProducts.EnumVal);
            modelOfferProducts.OfferProduction = new tblOffer_Production();

            modelOfferProducts.OfferProduction.user_Id = UserID;
            modelOfferProducts.OfferProduction.state_eV_Id = modelOfferProducts.EnumVal.Id;

            modelOfferProducts.OfferProduction.user_IdSpecified = true;
            modelOfferProducts.OfferProduction.state_eV_IdSpecified = true;

            BaseOutput OnAirOffersOut = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelOfferProducts.OfferProduction, out modelOfferProducts.OfferProductionArray);

            foreach (var item in modelOfferProducts.OfferProductionArray)
            {
                if(item.endDate < DateTime.Now.Ticks)
                {
                    PutToOffAirProduction((int)item.Id);
                }
            }

        }
    }
}

