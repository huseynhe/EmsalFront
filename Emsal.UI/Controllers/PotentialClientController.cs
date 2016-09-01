using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{

     //[EmsalAuthorization(AuthorizedAction = ActionName.potentialProduction)]
    //[EmsalAuthorization(AuthorizedAction = ActionName.Ordinary)]
    public class PotentialClientController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private PotentialClientViewModel modelPotentialProduction;

        public ActionResult Index()
        {
            Session["arrPCNum"] = null;
            Session["arrNum"] = null;
            Session["arrNumFU"] = null;

            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();


            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;


            if (Session["documentGrupId"] == null)
            {
                Guid dg = Guid.NewGuid();
                Session["documentGrupId"] = dg;
                this.Session.Timeout = 20;
            }

            Session["arrPCNum"] = null;
            Session["arrNum"] = null;
            Session["arrNumFU"] = null;

            return View(modelPotentialProduction);
        }
        
        [HttpPost]
        public ActionResult Index(PotentialClientViewModel model, FormCollection collection)
        {    
            baseInput = new BaseInput();        

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out model.User);
            baseInput.userName = model.User.Username;

            modelPotentialProduction = new PotentialClientViewModel();
            modelPotentialProduction.PotentialProduction = new tblPotential_Production();


            //if (Session["SelectedProduct"] == null)
            //{
            //Guid sp = Guid.NewGuid();
            //Session["SelectedProduct"] = sp;
            //}
            //var dd = Session["SelectedProduct"];
            //Session.Contents.Remove("SelectedProduct");




            string fh = "";
            if (model.VOEN != "" && model.VOEN!=null)
            {
                fh = "legalPerson";
                BaseOutput gfo = srv.WS_GetForeign_OrganizationByVoen(baseInput, model.VOEN, out modelPotentialProduction.ForeignOrganization);
            }
            else if(model.FIN != "" && model.FIN!=null)
            {
                fh = "fizikişexs";
                BaseOutput gp = srv.WS_GetPersonByPinNumber(baseInput, model.FIN, out modelPotentialProduction.Person);
            }



            if(modelPotentialProduction.Person!=null)
            {
                BaseOutput gubf = srv.WS_GetUserById(baseInput, (long)modelPotentialProduction.Person.UserId, true, out modelPotentialProduction.User);
            }
            else if (modelPotentialProduction.ForeignOrganization != null)
            {
                BaseOutput gubv = srv.WS_GetUserById(baseInput, (long)modelPotentialProduction.ForeignOrganization.userId, true, out modelPotentialProduction.User);
            }
            else
            {
            modelPotentialProduction.User = new tblUser();
            }



            if (modelPotentialProduction.Person == null && modelPotentialProduction.ForeignOrganization == null)
            {
                BaseOutput envalu = srv.WS_GetEnumValueByName(baseInput, fh, out modelPotentialProduction.EnumValue);

                modelPotentialProduction.User.userType_eV_ID = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.User.userType_eV_IDSpecified = true;

                BaseOutput apu = srv.WS_AddUser(baseInput, modelPotentialProduction.User, out modelPotentialProduction.User);


                modelPotentialProduction.tblUserRole = new tblUserRole();
                modelPotentialProduction.tblUserRole.UserId = modelPotentialProduction.User.Id;
                modelPotentialProduction.tblUserRole.UserIdSpecified = true;
                modelPotentialProduction.tblUserRole.RoleId = 15;
                modelPotentialProduction.tblUserRole.RoleIdSpecified = true;

                BaseOutput aur = srv.WS_AddUserRole(baseInput, modelPotentialProduction.tblUserRole, out modelPotentialProduction.tblUserRole);

                modelPotentialProduction.Address = new tblAddress();
                modelPotentialProduction.Address.adminUnit_Id = model.addressIdFU;
                modelPotentialProduction.Address.adminUnit_IdSpecified = true;
                modelPotentialProduction.Address.addressDesc = model.descAddress;
                modelPotentialProduction.Address.user_Id = modelPotentialProduction.User.Id;
                modelPotentialProduction.Address.user_IdSpecified = true;

                BaseOutput galf = srv.WS_GetAdminUnitListForID(baseInput, model.addressIdFU, true, out modelPotentialProduction.PRMAdminUnitArray);
                modelPotentialProduction.PRMAdminUnitList = modelPotentialProduction.PRMAdminUnitArray.ToList();
                modelPotentialProduction.Address.fullAddress = string.Join(",", modelPotentialProduction.PRMAdminUnitList.Select(x => x.Name));

                BaseOutput aa = srv.WS_AddAddress(baseInput, modelPotentialProduction.Address, out modelPotentialProduction.Address);


                modelPotentialProduction.Person = new tblPerson();

                modelPotentialProduction.Person.UserId = modelPotentialProduction.User.Id;
                modelPotentialProduction.Person.UserIdSpecified = true;
                modelPotentialProduction.Person.address_Id = modelPotentialProduction.Address.Id;
                modelPotentialProduction.Person.address_IdSpecified = true;

                if (fh == "legalPerson")
                {
                    modelPotentialProduction.Person.Name = model.legalPName;
                    modelPotentialProduction.Person.Surname = model.legalPSurname;
                    modelPotentialProduction.Person.FatherName = model.legalPFathername;
                }
                else if (fh == "fizikişexs")
                {
                    modelPotentialProduction.Person.PinNumber = model.FIN;
                    modelPotentialProduction.Person.Name = model.name;
                    modelPotentialProduction.Person.Surname = model.surname;
                    modelPotentialProduction.Person.FatherName = model.fathername;
                }

                BaseOutput aper = srv.WS_AddPerson(baseInput, modelPotentialProduction.Person, out modelPotentialProduction.Person);


                if (fh == "legalPerson")
                {
                    modelPotentialProduction.ForeignOrganization = new tblForeign_Organization();
                    modelPotentialProduction.ForeignOrganization.voen = model.VOEN;
                    modelPotentialProduction.ForeignOrganization.name = model.legalLame;
                    modelPotentialProduction.ForeignOrganization.userId = modelPotentialProduction.User.Id;
                    modelPotentialProduction.ForeignOrganization.userIdSpecified = true;
                    modelPotentialProduction.ForeignOrganization.manager_Id = modelPotentialProduction.Person.Id;
                    modelPotentialProduction.ForeignOrganization.manager_IdSpecified = true;

                    BaseOutput afo = srv.WS_AddForeign_Organization(baseInput, modelPotentialProduction.ForeignOrganization, out modelPotentialProduction.ForeignOrganization);
                }
            }


            Guid grupId = Guid.NewGuid();
                modelPotentialProduction.PotentialProduction.grup_Id = grupId.ToString();

                modelPotentialProduction.PotentialProduction.user_Id = modelPotentialProduction.User.Id;
                modelPotentialProduction.PotentialProduction.user_IdSpecified = true;

                modelPotentialProduction.PotentialProduction.description = model.description;
                modelPotentialProduction.PotentialProduction.product_Id = model.productId;
                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelPotentialProduction.ProductCatalogArray);
                modelPotentialProduction.ProductCatalogList = modelPotentialProduction.ProductCatalogArray.ToList();

                modelPotentialProduction.PotentialProduction.fullProductId = string.Join(",", modelPotentialProduction.ProductCatalogList.Select(x=>x.Id));

                modelPotentialProduction.PotentialProduction.product_IdSpecified = true;

                //modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
                modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size);
                modelPotentialProduction.PotentialProduction.quantitySpecified = true;

                modelPotentialProduction.PotentialProduction.isSelected = true;
                modelPotentialProduction.PotentialProduction.isSelectedSpecified = true;

                modelPotentialProduction.PotentialProduction.Status = 1;
                modelPotentialProduction.PotentialProduction.StatusSpecified = true;

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelPotentialProduction.EnumValue);
                modelPotentialProduction.PotentialProduction.state_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput envalydn = srv.WS_GetEnumValueByName(baseInput, "new", out modelPotentialProduction.EnumValue);
                modelPotentialProduction.PotentialProduction.monitoring_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.PotentialProduction.monitoring_eV_IdSpecified = true;

                DateTime startDate = (DateTime)model.startDate;
                modelPotentialProduction.PotentialProduction.startDate = startDate.Ticks;
                modelPotentialProduction.PotentialProduction.startDateSpecified = true;

                DateTime endDate = (DateTime)model.endDate;
                modelPotentialProduction.PotentialProduction.endDate = endDate.Ticks;
                modelPotentialProduction.PotentialProduction.endDateSpecified = true;

                modelPotentialProduction.ProductAddress = new tblProductAddress();

                modelPotentialProduction.ProductAddress.adminUnit_Id = model.addressId;
                //modelPotentialProduction.ProductAddress.adminUnit_Id = model.adId.LastOrDefault();
                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelPotentialProduction.PRMAdminUnitArray);
                modelPotentialProduction.PRMAdminUnitList = modelPotentialProduction.PRMAdminUnitArray.ToList();
                modelPotentialProduction.ProductAddress.fullAddressId = string.Join(",", modelPotentialProduction.PRMAdminUnitList.Select(x=>x.Id));
                modelPotentialProduction.ProductAddress.fullAddress = string.Join(",", modelPotentialProduction.PRMAdminUnitList.Select(x => x.Name));

                modelPotentialProduction.ProductAddress.adminUnit_IdSpecified = true;
                modelPotentialProduction.ProductAddress.addressDesc = model.descAddress;

                BaseOutput apa = srv.WS_AddProductAddress(baseInput, modelPotentialProduction.ProductAddress, out  modelPotentialProduction.ProductAddress);

                modelPotentialProduction.PotentialProduction.productAddress_Id = modelPotentialProduction.ProductAddress.Id;
                modelPotentialProduction.PotentialProduction.productAddress_IdSpecified = true;


                BaseOutput app = srv.WS_AddPotential_Production(baseInput, modelPotentialProduction.PotentialProduction, out  modelPotentialProduction.PotentialProduction);

                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialProduction.EnumValue);

            if (model.enumCat != null)
            {
                for (int ecv = 0; ecv < model.enumCat.Length; ecv++)
                {
                    modelPotentialProduction.ProductionControl = new tblProductionControl();

                    modelPotentialProduction.ProductionControl.Potential_Production_Id = modelPotentialProduction.PotentialProduction.Id;
                    modelPotentialProduction.ProductionControl.Potential_Production_IdSpecified = true;

                    modelPotentialProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                    modelPotentialProduction.ProductionControl.EnumCategoryIdSpecified = true;

                    modelPotentialProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                    modelPotentialProduction.ProductionControl.EnumValueIdSpecified = true;

                    modelPotentialProduction.ProductionControl.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                    modelPotentialProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                    BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelPotentialProduction.ProductionControl, out modelPotentialProduction.ProductionControl);
                }
            }

            Session["documentGrupId"] = null;
            TempData["Success"] = modelPotentialProduction.messageSuccess;

            return RedirectToAction("Index", "PotentialClient");
        }


        public JsonResult GetPhysicalPerson(string fin)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            modelPotentialProduction.PotentialProduction = new tblPotential_Production();            

                //BaseOutput gfo = srv.WS_GetForeign_OrganizationByVoen(baseInput, voen, out modelPotentialProduction.ForeignOrganization);

                BaseOutput gp = srv.WS_GetPersonByPinNumber(baseInput, fin, out modelPotentialProduction.Person);

                modelPotentialProduction.Personr = new tblPerson();
                if (modelPotentialProduction.Person != null)
                {
                    modelPotentialProduction.Personr.Name = modelPotentialProduction.Person.Name;
                    modelPotentialProduction.Personr.Surname = modelPotentialProduction.Person.Surname;
                    modelPotentialProduction.Personr.FatherName = modelPotentialProduction.Person.FatherName;
                }

                return Json(modelPotentialProduction.Personr, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetLegalPerson(string voen)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;


            modelPotentialProduction.PotentialProduction = new tblPotential_Production();

            BaseOutput gfo = srv.WS_GetForeign_OrganizationByVoen(baseInput, voen, out modelPotentialProduction.ForeignOrganization);

            if (modelPotentialProduction.ForeignOrganization != null)
            {
                //BaseOutput gubv = srv.WS_GetUserById(baseInput, (long)modelPotentialProduction.ForeignOrganization.userId, true, out modelPotentialProduction.User);

                BaseOutput gp = srv.WS_GetPersonByUserId(baseInput, (long)modelPotentialProduction.ForeignOrganization.userId, true, out modelPotentialProduction.Person);
            }
            
            
            modelPotentialProduction.Personr = new tblPerson();
            if (modelPotentialProduction.Person != null)
            {
                modelPotentialProduction.Personr.Name = modelPotentialProduction.Person.Name;
                modelPotentialProduction.Personr.Surname = modelPotentialProduction.Person.Surname;
                modelPotentialProduction.Personr.FatherName = modelPotentialProduction.Person.FatherName;

                modelPotentialProduction.Personr.PinNumber = modelPotentialProduction.ForeignOrganization.name;
            }

            return Json(modelPotentialProduction.Personr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(long id)
        {
            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            modelPotentialProduction.PotentialProduction = new tblPotential_Production();
           
            BaseOutput gpp = srv.WS_GetPotential_ProductionById(baseInput, id, true, out modelPotentialProduction.PotentialProduction);

            if (modelPotentialProduction.PotentialProduction == null)
            {
                return RedirectToAction("Index", "PotentialClient");
            }

            //if (modelPotentialProduction.PotentialProduction.grup_Id != null)
            //{
            //    return RedirectToAction("Index", "PotentialClient");
            //}

            modelPotentialProduction.Id = modelPotentialProduction.PotentialProduction.Id;
            modelPotentialProduction.productId = (long)modelPotentialProduction.PotentialProduction.product_Id;
            modelPotentialProduction.description = modelPotentialProduction.PotentialProduction.description;
            modelPotentialProduction.productAddressId = (long)modelPotentialProduction.PotentialProduction.productAddress_Id;
            modelPotentialProduction.size = (modelPotentialProduction.PotentialProduction.quantity.ToString()).Replace(',', '.');

            DateTime startDate = new DateTime((long)modelPotentialProduction.PotentialProduction.startDate);
            modelPotentialProduction.startDate = startDate;
            DateTime endDate = new DateTime((long)modelPotentialProduction.PotentialProduction.endDate);
            modelPotentialProduction.endDate = endDate;


            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "shippingSchedule", out modelPotentialProduction.EnumCategory);
            if (modelPotentialProduction.EnumCategory == null)
                modelPotentialProduction.EnumCategory = new tblEnumCategory();

            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelPotentialProduction.EnumCategory.Id, true, out modelPotentialProduction.EnumValueArray);
            modelPotentialProduction.EnumValueShippingScheduleList = modelPotentialProduction.EnumValueArray.ToList();



            BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelPotentialProduction.EnumCategory);
            if (modelPotentialProduction.EnumCategory == null)
                modelPotentialProduction.EnumCategory = new tblEnumCategory();

            BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelPotentialProduction.EnumCategory.Id, true, out modelPotentialProduction.EnumValueArray);
            modelPotentialProduction.EnumValueMonthList = modelPotentialProduction.EnumValueArray.ToList();


            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialProduction.EnumValue);


            if (Session["documentGrupId"] == null)
            {
                Guid dg = Guid.NewGuid();
                Session["documentGrupId"] = dg;
                this.Session.Timeout = 20;
            }
            Session["arrPCNum"] = null;
            Session["arrNum"] = null;
            Session["arrNumFU"] = null;

            return View(modelPotentialProduction);
        }

        [HttpPost]
        public ActionResult Edit(PotentialClientViewModel model, FormCollection collection)
        {
            baseInput = new BaseInput();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out model.User);
            baseInput.userName = model.User.Username;


            modelPotentialProduction = new PotentialClientViewModel();


            BaseOutput gpp = srv.WS_GetPotential_ProductionById(baseInput, model.Id, true, out modelPotentialProduction.PotentialProduction);

            modelPotentialProduction.PotentialProduction.product_Id = model.productId;
                modelPotentialProduction.PotentialProduction.description = model.description;

                //modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
                modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size);
                modelPotentialProduction.PotentialProduction.quantitySpecified = true;

            DateTime startDate = (DateTime)model.startDate;
            modelPotentialProduction.PotentialProduction.startDate = startDate.Ticks;
            modelPotentialProduction.PotentialProduction.startDateSpecified = true;

            DateTime endDate = (DateTime)model.endDate;
            modelPotentialProduction.PotentialProduction.endDate = endDate.Ticks;
            modelPotentialProduction.PotentialProduction.endDateSpecified = true;


            BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelPotentialProduction.ProductCatalogArray);
            modelPotentialProduction.ProductCatalogList = modelPotentialProduction.ProductCatalogArray.ToList();

            modelPotentialProduction.PotentialProduction.fullProductId = string.Join(",", modelPotentialProduction.ProductCatalogList.Select(x => x.Id));

            BaseOutput app = srv.WS_UpdatePotential_Production(baseInput, modelPotentialProduction.PotentialProduction, out modelPotentialProduction.PotentialProduction);


            BaseOutput gpabi = srv.WS_GetProductAddressById(baseInput, (long)modelPotentialProduction.PotentialProduction.productAddress_Id, true, out modelPotentialProduction.ProductAddress);

            modelPotentialProduction.ProductAddress.adminUnit_Id = model.addressId;
            BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelPotentialProduction.PRMAdminUnitArray);
            modelPotentialProduction.PRMAdminUnitList = modelPotentialProduction.PRMAdminUnitArray.ToList();

            modelPotentialProduction.ProductAddress.fullAddressId = string.Join(",", modelPotentialProduction.PRMAdminUnitList.Select(x => x.Id));
            modelPotentialProduction.ProductAddress.fullAddress = string.Join(",", modelPotentialProduction.PRMAdminUnitList.Select(x=>x.Name));
            modelPotentialProduction.ProductAddress.adminUnit_IdSpecified = true;
            modelPotentialProduction.ProductAddress.addressDesc = model.descAddress;

            BaseOutput apa = srv.WS_UpdateProductAddress(baseInput, modelPotentialProduction.ProductAddress, out modelPotentialProduction.ProductAddress);


            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialProduction.EnumValue);


            if (model.enumCat != null)
            {
                for (long ecv = 0; ecv < model.enumCat.Length; ecv++)
                {
                    if (model.pcId != null)
                    {
                        BaseOutput gpcbi = srv.WS_GetProductionControlById(baseInput, model.pcId[ecv], true, out modelPotentialProduction.ProductionControl);

                        modelPotentialProduction.ProductionControl.Potential_Production_Id = modelPotentialProduction.PotentialProduction.Id;
                        modelPotentialProduction.ProductionControl.Potential_Production_IdSpecified = true;

                        modelPotentialProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                        modelPotentialProduction.ProductionControl.EnumCategoryIdSpecified = true;

                        modelPotentialProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                        modelPotentialProduction.ProductionControl.EnumValueIdSpecified = true;

                        modelPotentialProduction.ProductionControl.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                        modelPotentialProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                        BaseOutput ap = srv.WS_UpdateProductionControl(baseInput, modelPotentialProduction.ProductionControl, out modelPotentialProduction.ProductionControl);
                    }
                    else
                    {
                        BaseOutput pcb = srv.WS_GetProductionControls(baseInput, out modelPotentialProduction.ProductionControlArray);
                        modelPotentialProduction.ProductionControlList = modelPotentialProduction.ProductionControlArray.Where(x => x.Potential_Production_Id == modelPotentialProduction.PotentialProduction.Id).ToList();

                        if (ecv == 0)
                        {
                            foreach (tblProductionControl itm in modelPotentialProduction.ProductionControlList)
                            {
                                BaseOutput dcb = srv.WS_DeleteProductionControl(baseInput, itm);
                            }
                        }


                        modelPotentialProduction.ProductionControl = new tblProductionControl();

                        modelPotentialProduction.ProductionControl.Potential_Production_Id = modelPotentialProduction.PotentialProduction.Id;
                        modelPotentialProduction.ProductionControl.Potential_Production_IdSpecified = true;

                        modelPotentialProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                        modelPotentialProduction.ProductionControl.EnumCategoryIdSpecified = true;

                        modelPotentialProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                        modelPotentialProduction.ProductionControl.EnumValueIdSpecified = true;

                        modelPotentialProduction.ProductionControl.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                        modelPotentialProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                        BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelPotentialProduction.ProductionControl, out modelPotentialProduction.ProductionControl);
                    }

                }
            }

            Session["documentGrupId"] = null;
            TempData["Success"] = modelPotentialProduction.messageSuccess;

            return RedirectToAction("Index", "PotentialClient");
        }

        public ActionResult ProductCatalog(long pId = 0, long ppId=0)
        {
            baseInput = new BaseInput();

            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            //BaseOutput bouput = srv.WS_GetProductCatalogs(baseInput, out modelPotentialProduction.ProductCatalogArray);
            //modelPotentialProduction.ProductCatalogList = modelPotentialProduction.ProductCatalogArray.Where(x => x.ProductCatalogParentID == pId).ToList();

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, (int)pId, true, out modelPotentialProduction.ProductCatalogArray);
            modelPotentialProduction.ProductCatalogList = modelPotentialProduction.ProductCatalogArray.ToList();

            modelPotentialProduction.ProductCatalogListPC = new List<tblProductCatalog>();
            foreach (tblProductCatalog itm in modelPotentialProduction.ProductCatalogList)
            {
                BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelPotentialProduction.ProductCatalogArrayPC);
                if (modelPotentialProduction.ProductCatalogArrayPC.ToList().Count == 0)
                {
                    if (itm.canBeOrder == 1)
                        modelPotentialProduction.ProductCatalogListPC.Add(itm);
                }
                else
                {
                    modelPotentialProduction.ProductCatalogListPC.Add(itm);
                }
            }
            modelPotentialProduction.ProductCatalogList = null;
            modelPotentialProduction.ProductCatalogList = modelPotentialProduction.ProductCatalogListPC;



            BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelPotentialProduction.EnumCategoryArray);
            modelPotentialProduction.EnumCategoryList = modelPotentialProduction.EnumCategoryArray.ToList();

            BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelPotentialProduction.EnumValueArray);
            modelPotentialProduction.EnumValueList = modelPotentialProduction.EnumValueArray.ToList();

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelPotentialProduction.EnumCategory);

            BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelPotentialProduction.ProductCatalogControlArray);

            modelPotentialProduction.ProductCatalogControlList = modelPotentialProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelPotentialProduction.EnumCategory.Id).ToList();

            if (Session["arrPCNum"] == null)
            {
                Session["arrPCNum"] = modelPotentialProduction.arrPNum;
            }
            else
            {
                modelPotentialProduction.arrPNum = (long)Session["arrPCNum"] + 1;
                Session["arrPCNum"] = modelPotentialProduction.arrPNum;
            }


            //if (modelPotentialProduction.ProductCatalogList.Count() == 0 && ppId>0)
                if (ppId > 0)
                {
                BaseOutput gpp = srv.WS_GetPotential_ProductionById(baseInput, ppId, true, out modelPotentialProduction.PotentialProduction);

                modelPotentialProduction.Id = modelPotentialProduction.PotentialProduction.Id;
                modelPotentialProduction.productId = (long)modelPotentialProduction.PotentialProduction.product_Id;
                modelPotentialProduction.description = modelPotentialProduction.PotentialProduction.description;
                modelPotentialProduction.size = (modelPotentialProduction.PotentialProduction.quantity.ToString()).Replace(',', '.');
                modelPotentialProduction.price = (modelPotentialProduction.PotentialProduction.unit_price.ToString()).Replace(',', '.');
                modelPotentialProduction.PotentialProduction.fullProductId = "0,"+modelPotentialProduction.PotentialProduction.fullProductId;
                modelPotentialProduction.productIds = modelPotentialProduction.PotentialProduction.fullProductId.Split(',').Select(long.Parse).ToArray();

                modelPotentialProduction.ProductCatalogListFEA = new IList<tblProductCatalog>[modelPotentialProduction.productIds.Count()];
                int s = 0;
                foreach (long itm in modelPotentialProduction.productIds)
                {
                BaseOutput gpc = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm ,true, out modelPotentialProduction.ProductCatalogArray);

                    modelPotentialProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                    foreach (tblProductCatalog itmpc in modelPotentialProduction.ProductCatalogArray.ToList())
                    {
                        BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itmpc.Id, true, out modelPotentialProduction.ProductCatalogArrayPC);
                        if (modelPotentialProduction.ProductCatalogArrayPC.ToList().Count == 0)
                        {
                            if (itmpc.canBeOrder == 1)
                                modelPotentialProduction.ProductCatalogListPC.Add(itmpc);
                        }
                        else
                        {
                            modelPotentialProduction.ProductCatalogListPC.Add(itmpc);
                        }
                    }
                    modelPotentialProduction.ProductCatalogListFEA[s] = modelPotentialProduction.ProductCatalogListPC;

                    //modelPotentialProduction.ProductCatalogListFEA[s] = modelPotentialProduction.ProductCatalogArray.ToList();
                    s = s + 1;
                }

                BaseOutput pcln = srv.WS_GetProductCatalogControlsByProductID(baseInput, modelPotentialProduction.productIds.LastOrDefault(), true, out modelPotentialProduction.ProductCatalogControlArray);

                modelPotentialProduction.ProductCatalogControlList = modelPotentialProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelPotentialProduction.EnumCategory.Id).ToList();

                BaseOutput pcb = srv.WS_GetProductionControls(baseInput, out modelPotentialProduction.ProductionControlArray);
                modelPotentialProduction.ProductionControlList = modelPotentialProduction.ProductionControlArray.Where(x=>x.Potential_Production_Id== pId).ToList();

                modelPotentialProduction.productionControlEVIds = new long[modelPotentialProduction.ProductionControlList.Count()];
                for (int t=0;t< modelPotentialProduction.ProductionControlList.Count();t++)
                {
                    modelPotentialProduction.productionControlEVIds[t] = (long)modelPotentialProduction.ProductionControlList[t].EnumValueId;
                }
                    }
            
            return View(modelPotentialProduction);
        }

        public ActionResult AdminUnit(long pId = 0, long productAddressId = 0,string status=null)
        {
            baseInput = new BaseInput();
            
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(baseInput, out modelPotentialProduction.PRMAdminUnitArray);
            modelPotentialProduction.PRMAdminUnitList = modelPotentialProduction.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput,pId, true, out modelPotentialProduction.PRMAdminUnitArray);
            //modelPotentialProduction.PRMAdminUnitList = modelPotentialProduction.PRMAdminUnitArray.ToList();


            modelPotentialProduction.productAddressIds = null;
            if (productAddressId>0)
            {
                BaseOutput gpa = srv.WS_GetProductAddressById(baseInput, productAddressId, true, out modelPotentialProduction.ProductAddress);
                if(modelPotentialProduction.ProductAddress.fullAddressId!="")
                {

                    //modelPotentialProduction.productAddressIds = modelPotentialProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();
                //    modelPotentialProduction.PRMAdminUnitList = modelPotentialProduction.PRMAdminUnitArray.ToList();





                    modelPotentialProduction.ProductAddress.fullAddressId = "0," + modelPotentialProduction.ProductAddress.fullAddressId;
                    modelPotentialProduction.productAddressIds = modelPotentialProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();

                    modelPotentialProduction.PRMAdminUnitArrayFA = new IList<tblPRM_AdminUnit>[modelPotentialProduction.productAddressIds.Count()];
                    int s = 0;
                    foreach (long itm in modelPotentialProduction.productAddressIds)
                    {
                        BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(baseInput, (int)itm, true, out modelPotentialProduction.PRMAdminUnitArray);
                        modelPotentialProduction.PRMAdminUnitArrayFA[s] = modelPotentialProduction.PRMAdminUnitArray.ToList();
                        s = s + 1;
                    }

                    if (modelPotentialProduction.PRMAdminUnitArrayFA[s - 1].Count() > 0)
                    {
                        modelPotentialProduction.ProductAddress.fullAddressId = modelPotentialProduction.ProductAddress.fullAddressId + ",0";
                        modelPotentialProduction.productAddressIds = modelPotentialProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();
                    }
                }
            }

            
            if (status != null)
            {
                if (Session["arrNumFU"] == null)
                {
                    Session["arrNumFU"] = modelPotentialProduction.arrNumFU;
                }
                else
                {
                    modelPotentialProduction.arrNumFU = (long)Session["arrNumFU"] + 1;
                    Session["arrNumFU"] = modelPotentialProduction.arrNumFU;
                }

                return View("AdminUnitFU", modelPotentialProduction);
            }
            else
            {

                if (Session["arrNum"] == null)
                {
                    Session["arrNum"] = modelPotentialProduction.arrNum;
                }
                else
                {
                    modelPotentialProduction.arrNum = (long)Session["arrNum"] + 1;
                    Session["arrNum"] = modelPotentialProduction.arrNum;
                }

                return View("AdminUnit", modelPotentialProduction);
            } 
                        
        }
        
        public ActionResult SelectedProducts()
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelPotentialProduction.EnumCategory);

            BaseOutput gpd = srv.WS_GetPotensialProductionDetailistForCreateUser(baseInput, modelPotentialProduction.User.Username, out modelPotentialProduction.ProductionDetailArray);

            //BaseOutput gpd = srv.WS_GetPotensialProductionDetailistForUser(baseInput, (long)userId, true, out modelPotentialProduction.ProductionDetailArray);

            modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailArray.Where(x=>x.enumCategoryId== modelPotentialProduction.EnumCategory.Id).ToList();

            modelPotentialProduction.userId = (long)userId;

                return View(modelPotentialProduction);

        }
        
        public ActionResult ChooseFileTemplate(long pId)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelPotentialProduction.EnumCategory);

            if (modelPotentialProduction.EnumCategory == null)
                modelPotentialProduction.EnumCategory = new tblEnumCategory();

            BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelPotentialProduction.ProductCatalogControlArray);

            modelPotentialProduction.ProductCatalogControlDocumentTypeList = modelPotentialProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId == modelPotentialProduction.EnumCategory.Id).ToList();


            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelPotentialProduction.EnumCategory.Id, true, out modelPotentialProduction.EnumValueArray);
            modelPotentialProduction.EnumValueDocumentTypeList = modelPotentialProduction.EnumValueArray.ToList();


            string grup_Id = Session["documentGrupId"].ToString(); ;
            bool flag = true;
            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialProduction.EnumValue);

            BaseOutput tfs = srv.WS_GetDocumentSizebyGroupId(grup_Id, modelPotentialProduction.EnumValue.Id, true, out modelPotentialProduction.totalSize, out flag);

            return View(modelPotentialProduction);
        }
        
        [HttpPost]
        public void File(IList<HttpPostedFileBase> file, long documentType)
        {
            if (file != null)
            {
                string documentGrupId = Session["documentGrupId"].ToString();

                baseInput = new BaseInput();
                modelPotentialProduction = new PotentialClientViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
                baseInput.userName = modelPotentialProduction.User.Username;

                String sDate = DateTime.Now.ToString();
                DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                String dy = datevalue.Day.ToString();
                String mn = datevalue.Month.ToString();
                String yy = datevalue.Year.ToString();

                string path = modelPotentialProduction.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                foreach (var attachfile in file)
                {

                    //var dec=(attachfile.ContentLength / 1024);
                    //var decd = (decimal)731 / 1024;
                    //var fl = Math.Round(dec,2);

                    if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelPotentialProduction.fileSize && modelPotentialProduction.fileTypes.Contains(attachfile.ContentType))
                    {
                        var fileName = Path.GetFileName(attachfile.FileName);
                        var ofileName = fileName;

                        string ext = string.Empty;
                        int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
                        if (fileExtPos >= 0)
                            ext = fileName.Substring(fileExtPos, fileName.Length - fileExtPos);

                        var newFileName = Guid.NewGuid();
                        fileName = newFileName.ToString() + ext;

                        attachfile.SaveAs(Path.Combine(path, fileName));

                        modelPotentialProduction.ProductionDocument = new tblProduction_Document();
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialProduction.EnumValue);

                        modelPotentialProduction.ProductionDocument.grup_Id = documentGrupId;
                        modelPotentialProduction.ProductionDocument.documentUrl = path;
                        modelPotentialProduction.ProductionDocument.documentName = fileName;
                        modelPotentialProduction.ProductionDocument.documentRealName = ofileName;

                        modelPotentialProduction.ProductionDocument.document_type_ev_Id = documentType.ToString();
                        modelPotentialProduction.ProductionDocument.documentSize = attachfile.ContentLength;
                        modelPotentialProduction.ProductionDocument.documentSizeSpecified = true;

                        modelPotentialProduction.ProductionDocument.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                        modelPotentialProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                        BaseOutput apd = srv.WS_AddProductionDocument(baseInput, modelPotentialProduction.ProductionDocument, out modelPotentialProduction.ProductionDocument);
                    }
                }

            }
        }

        public ActionResult SelectedDocuments(long PId=0)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            string grup_Id = Session["documentGrupId"].ToString();

            BaseOutput gpbu = srv.WS_GetProductionDocuments(baseInput, out modelPotentialProduction.ProductionDocumentArray);

            if(PId>0)
            {
                modelPotentialProduction.ProductionDocumentList = modelPotentialProduction.ProductionDocumentArray.Where(x => x.Potential_Production_Id == PId || x.grup_Id == grup_Id).ToList();
            }
            else
            {
            modelPotentialProduction.ProductionDocumentList = modelPotentialProduction.ProductionDocumentArray.Where(x => x.grup_Id == grup_Id).ToList();
            }

            return View(modelPotentialProduction);
        }

        public void DeleteSelectedDocument(long id)
        {     
            baseInput = new BaseInput();  
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput, id, true, out modelPotentialProduction.ProductionDocument);

            BaseOutput dpd = srv.WS_DeleteProductionDocument(baseInput, modelPotentialProduction.ProductionDocument);
        }

        public void DeleteSelectedPotentialProduct(long id)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialClientViewModel();

            long? userId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelPotentialProduction.User);
            baseInput.userName = modelPotentialProduction.User.Username;

            BaseOutput gpd = srv.WS_GetPotential_ProductionById(baseInput, id, true, out modelPotentialProduction.PotentialProduction);

            BaseOutput dpd = srv.WS_DeletePotential_Production(baseInput, modelPotentialProduction.PotentialProduction);
        }
    }
}
