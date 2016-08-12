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
    [EmsalAuthorization(AuthorizedAction = ActionName.potentialProduction)]

    public class ProfileController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private PotentialProductionViewModel modelPotentialProduction;

        public ActionResult Index()
        {
            Session["arrPNum"] = null;
            Session["arrNum"] = null;

            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialProductionViewModel();

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


            if (Session["documentGrupId"] == null)
            {
                Guid dg = Guid.NewGuid();
                Session["documentGrupId"] = dg;
                this.Session.Timeout = 20;
            }

            Session["arrPNum"] = null;
            Session["arrNum"] = null;

            return View(modelPotentialProduction);
        }

        [HttpPost]
        public ActionResult Index(PotentialProductionViewModel model, FormCollection collection)
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

            modelPotentialProduction = new PotentialProductionViewModel();
            modelPotentialProduction.PotentialProduction = new tblPotential_Production(); 

            //if (Session["SelectedProduct"] == null)
            //{
            //Guid sp = Guid.NewGuid();
            //Session["SelectedProduct"] = sp;
            //}
            //var dd = Session["SelectedProduct"];
            //Session.Contents.Remove("SelectedProduct");

            if (model.confirmList == true)
            {
                Guid grupId = Guid.NewGuid();
                modelPotentialProduction.PotentialProduction.grup_Id = grupId.ToString();
                modelPotentialProduction.PotentialProduction.user_Id = userId;
                modelPotentialProduction.PotentialProduction.user_IdSpecified = true;
                modelPotentialProduction.PotentialProduction.isSelected = true;
                modelPotentialProduction.PotentialProduction.isSelectedSpecified = true;
                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelPotentialProduction.EnumValue);
                modelPotentialProduction.PotentialProduction.state_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput upp = srv.WS_UpdatePotential_ProductionForUserID(modelPotentialProduction.PotentialProduction, out  modelPotentialProduction.PotentialProductionArray);

            }
            else
            {

                modelPotentialProduction.PotentialProduction.description = model.description;
                modelPotentialProduction.PotentialProduction.product_Id = model.productId;
                //modelPotentialProduction.PotentialProduction.product_Id = model.prodId.LastOrDefault();
                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelPotentialProduction.ProductCatalogArray);
                modelPotentialProduction.ProductCatalogList = modelPotentialProduction.ProductCatalogArray.ToList();

                modelPotentialProduction.PotentialProduction.fullProductId = string.Join(",", modelPotentialProduction.ProductCatalogList.Select(x=>x.Id));

                modelPotentialProduction.PotentialProduction.product_IdSpecified = true;

                modelPotentialProduction.PotentialProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
                //modelPotentialProduction.PotentialProduction.unit_price = Convert.ToDecimal(model.price);
                modelPotentialProduction.PotentialProduction.unit_priceSpecified = true;

                modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
                //modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size);
                modelPotentialProduction.PotentialProduction.quantitySpecified = true;

                modelPotentialProduction.PotentialProduction.isSelected = true;
                modelPotentialProduction.PotentialProduction.isSelectedSpecified = true;

                modelPotentialProduction.PotentialProduction.user_Id = userId;
                modelPotentialProduction.PotentialProduction.user_IdSpecified = true;

                modelPotentialProduction.PotentialProduction.Status = 1;
                modelPotentialProduction.PotentialProduction.StatusSpecified = true;

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "new", out modelPotentialProduction.EnumValue);
                modelPotentialProduction.PotentialProduction.state_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.PotentialProduction.state_eV_IdSpecified = true;

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
                modelPotentialProduction.ProductionCalendar = new tblProduction_Calendar();

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


                modelPotentialProduction.ProductionCalendar.Production_Id = modelPotentialProduction.PotentialProduction.Id;
                modelPotentialProduction.ProductionCalendar.Production_IdSpecified = true;

                modelPotentialProduction.ProductionCalendar.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.ProductionCalendar.Production_type_eV_IdSpecified = true;

                modelPotentialProduction.ProductionCalendar.Months = model.checkedMonth;
                modelPotentialProduction.ProductionCalendar.Transportation_eV_Id = model.shippingSchedule;
                modelPotentialProduction.ProductionCalendar.Transportation_eV_IdSpecified = true;

                BaseOutput apc = srv.WS_AddProduction_Calendar(baseInput, modelPotentialProduction.ProductionCalendar, out modelPotentialProduction.ProductionCalendar);



                modelPotentialProduction.ProductionDocument = new tblProduction_Document();
                modelPotentialProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
                modelPotentialProduction.ProductionDocument.Potential_Production_Id = modelPotentialProduction.PotentialProduction.Id;
                modelPotentialProduction.ProductionDocument.Potential_Production_IdSpecified = true;
                modelPotentialProduction.ProductionDocument.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelPotentialProduction.ProductionDocument, out modelPotentialProduction.ProductionDocumentArray);

            }

            Session["documentGrupId"] = null;
            TempData["Success"] = modelPotentialProduction.messageSuccess;

            return RedirectToAction("Index", "Profile");
        }


        public ActionResult Edit(long id)
        {
            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialProductionViewModel();

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
                return RedirectToAction("Index", "Profile");
            }

            if (modelPotentialProduction.PotentialProduction.grup_Id != null)
            {
                return RedirectToAction("Index", "Profile");
            }

            modelPotentialProduction.Id = modelPotentialProduction.PotentialProduction.Id;
            modelPotentialProduction.productId = (long)modelPotentialProduction.PotentialProduction.product_Id;
            modelPotentialProduction.description = modelPotentialProduction.PotentialProduction.description;
            modelPotentialProduction.productAddressId = (long)modelPotentialProduction.PotentialProduction.productAddress_Id;
            modelPotentialProduction.size = (modelPotentialProduction.PotentialProduction.quantity.ToString()).Replace(',', '.');
            modelPotentialProduction.price = (modelPotentialProduction.PotentialProduction.unit_price.ToString()).Replace(',', '.');
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
            //BaseOutput sml = srv.WS_GetProductionCalenadarProductionId(baseInput, modelPotentialProduction.PotentialProduction.Id, true, out modelPotentialProduction.ProductionCalendarArray);
            BaseOutput sml = srv.WS_GetProductionCalendarByProductionId(baseInput, modelPotentialProduction.PotentialProduction.Id, true,modelPotentialProduction.EnumValue.Id, true, out modelPotentialProduction.ProductionCalendar);

            //modelPotentialProduction.ProductionCalendar = modelPotentialProduction.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == modelPotentialProduction.EnumValue.Id).FirstOrDefault() ;

            string[] months = modelPotentialProduction.ProductionCalendar.Months.Split(',');
            modelPotentialProduction.selectedMonth = months;

            modelPotentialProduction.shippingSchedule = (long)modelPotentialProduction.ProductionCalendar.Transportation_eV_Id;
            modelPotentialProduction.productionCalendarId = modelPotentialProduction.ProductionCalendar.Id;

            if (Session["documentGrupId"] == null)
            {
                Guid dg = Guid.NewGuid();
                Session["documentGrupId"] = dg;
                this.Session.Timeout = 20;
            }
            Session["arrPNum"] = null;
            Session["arrNum"] = null;

            return View(modelPotentialProduction);
        }

        [HttpPost]
        public ActionResult Edit(PotentialProductionViewModel model, FormCollection collection)
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

            modelPotentialProduction = new PotentialProductionViewModel();

            BaseOutput gpp = srv.WS_GetPotential_ProductionById(baseInput, model.Id, true, out modelPotentialProduction.PotentialProduction);

            modelPotentialProduction.PotentialProduction.product_Id = model.productId;
                modelPotentialProduction.PotentialProduction.description = model.description;

                modelPotentialProduction.PotentialProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
                modelPotentialProduction.PotentialProduction.unit_priceSpecified = true;

                modelPotentialProduction.PotentialProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
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


            BaseOutput pc = srv.WS_GetProduction_CalendarById(baseInput, model.productionCalendarId, true, out modelPotentialProduction.ProductionCalendar);

            modelPotentialProduction.ProductionCalendar.Production_Id = modelPotentialProduction.PotentialProduction.Id;
            modelPotentialProduction.ProductionCalendar.Production_IdSpecified = true;

            modelPotentialProduction.ProductionCalendar.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
            modelPotentialProduction.ProductionCalendar.Production_type_eV_IdSpecified = true;

            modelPotentialProduction.ProductionCalendar.Months = model.checkedMonth;
            modelPotentialProduction.ProductionCalendar.Transportation_eV_Id = model.shippingSchedule;
            modelPotentialProduction.ProductionCalendar.Transportation_eV_IdSpecified = true;

            BaseOutput apc = srv.WS_UpdateProductionCalendar(baseInput, modelPotentialProduction.ProductionCalendar, out modelPotentialProduction.ProductionCalendar);



            modelPotentialProduction.ProductionDocument = new tblProduction_Document();
            modelPotentialProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
            modelPotentialProduction.ProductionDocument.Potential_Production_Id = modelPotentialProduction.PotentialProduction.Id;
            modelPotentialProduction.ProductionDocument.Potential_Production_IdSpecified = true;
            modelPotentialProduction.ProductionDocument.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
            modelPotentialProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

            BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelPotentialProduction.ProductionDocument, out modelPotentialProduction.ProductionDocumentArray);

            Session["documentGrupId"] = null;
            TempData["Success"] = modelPotentialProduction.messageSuccess;

            return RedirectToAction("Index", "Profile");
        }

        public ActionResult ProductCatalog(long pId = 0, long ppId=0)
        {
            baseInput = new BaseInput();

            modelPotentialProduction = new PotentialProductionViewModel();

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

            if (Session["arrPNum"] == null)
            {
                Session["arrPNum"] = modelPotentialProduction.arrPNum;
            }
            else
            {
                modelPotentialProduction.arrPNum = (long)Session["arrPNum"] + 1;
                Session["arrPNum"] = modelPotentialProduction.arrPNum;
            }


            if (modelPotentialProduction.ProductCatalogList.Count() == 0 && ppId>0)
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

        public ActionResult AdminUnit(long pId = 0, long productAddressId = 0)
        {
            baseInput = new BaseInput();            
            modelPotentialProduction = new PotentialProductionViewModel();

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
                }
            }

            if (Session["arrNum"] == null)
            {
                Session["arrNum"] = modelPotentialProduction.arrNum;
            }
            else
            {
                modelPotentialProduction.arrNum = (long)Session["arrNum"] + 1;
                Session["arrNum"] = modelPotentialProduction.arrNum;
            }

            return View(modelPotentialProduction);
        }

        public ActionResult SelectedProducts(bool pdf = false)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialProductionViewModel();

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

            BaseOutput gpd = srv.WS_GetPotensialProductionDetailistForUser(baseInput, (long)userId, true, out modelPotentialProduction.ProductionDetailArray);
            modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailArray.Where(x=>x.enumCategoryId== modelPotentialProduction.EnumCategory.Id).ToList();

            modelPotentialProduction.isPDF = pdf;
            modelPotentialProduction.userId = (long)userId;

            var gd = Guid.NewGuid();

            if (modelPotentialProduction.isPDF == true)
            {
                return new Rotativa.PartialViewAsPdf(modelPotentialProduction) { FileName = gd + ".pdf" };
            }
            else
            {
                return View(modelPotentialProduction);
            }
        }

        public ActionResult ChooseFileTemplate(long pId)
        {
            baseInput = new BaseInput();
            modelPotentialProduction = new PotentialProductionViewModel();

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
                modelPotentialProduction = new PotentialProductionViewModel();

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
            modelPotentialProduction = new PotentialProductionViewModel();

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
            modelPotentialProduction = new PotentialProductionViewModel();

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
            modelPotentialProduction = new PotentialProductionViewModel();

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
