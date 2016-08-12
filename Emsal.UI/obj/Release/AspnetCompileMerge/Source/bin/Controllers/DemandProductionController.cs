using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.demandProduction)]

    public class DemandProductionController : Controller
    {
        private BaseInput baseInput;     

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private DemandProductionViewModel modelDemandProduction;

        public ActionResult Index(long?userId)
        {

            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelDemandProduction = new DemandProductionViewModel();


            //ferid
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
            BaseOutput person = srv.WS_GetPersonByUserId(baseInput, (long)userId, true, out modelDemandProduction.Person);
            //////


            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "shippingSchedule", out modelDemandProduction.EnumCategory);
            if (modelDemandProduction.EnumCategory == null)
                modelDemandProduction.EnumCategory = new tblEnumCategory();

            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategory.Id, true, out modelDemandProduction.EnumValueArray);
                modelDemandProduction.EnumValueShippingScheduleList = modelDemandProduction.EnumValueArray.ToList();


            BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelDemandProduction.EnumCategory);
            if (modelDemandProduction.EnumCategory == null)
                modelDemandProduction.EnumCategory = new tblEnumCategory();

                BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategory.Id, true, out modelDemandProduction.EnumValueArray);
                modelDemandProduction.EnumValueMonthList = modelDemandProduction.EnumValueArray.ToList();


            if (Session["documentGrupId"] == null)
            {
            Guid dg = Guid.NewGuid();
            Session["documentGrupId"] = dg;
            this.Session.Timeout = 20;
            }

            return View(modelDemandProduction);
        }

        //public ActionResult GeneratePDF()
        //{
        //    return new Rotativa.ActionAsPdf("SelectedProducts");
        //}

        public ActionResult ProductCatalog(long pId = 0, long ppId = 0)
        {
            baseInput = new BaseInput();

            modelDemandProduction = new DemandProductionViewModel();

            BaseOutput bouput = srv.WS_GetProductCatalogs(baseInput, out modelDemandProduction.ProductCatalogArray);
            modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.Where(x => x.ProductCatalogParentID == pId).ToList();

            BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelDemandProduction.EnumCategoryArray);
        modelDemandProduction.EnumCategoryList = modelDemandProduction.EnumCategoryArray.ToList();

        BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelDemandProduction.EnumValueArray);
        modelDemandProduction.EnumValueList = modelDemandProduction.EnumValueArray.ToList();

        BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelDemandProduction.EnumCategory);

        BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelDemandProduction.ProductCatalogControlArray);

        modelDemandProduction.ProductCatalogControlList = modelDemandProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategory.Id).ToList();

        if (Session["arrDPNum"] == null)
        {
                Session["arrDPNum"] = modelDemandProduction.arrPNum;
        }
        else
            {
                modelDemandProduction.arrPNum = (int)Session["arrDPNum"] + 1;
                Session["arrDPNum"] = modelDemandProduction.arrPNum;
        }


            if (modelDemandProduction.ProductCatalogList.Count() == 0 && ppId > 0)
            {
                BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, ppId, true, out modelDemandProduction.DemandProduction);

                modelDemandProduction.Id = modelDemandProduction.DemandProduction.Id;
                modelDemandProduction.productId = (int)modelDemandProduction.DemandProduction.product_Id;
                modelDemandProduction.description = modelDemandProduction.DemandProduction.description;
                modelDemandProduction.size = (modelDemandProduction.DemandProduction.quantity.ToString()).Replace(',', '.');
                modelDemandProduction.price = (modelDemandProduction.DemandProduction.unit_price.ToString()).Replace(',', '.');
                modelDemandProduction.DemandProduction.fullProductId = "0," + modelDemandProduction.DemandProduction.fullProductId;
                modelDemandProduction.productIds = modelDemandProduction.DemandProduction.fullProductId.Split(',').Select(long.Parse).ToArray();

                modelDemandProduction.ProductCatalogListFEA = new IList<tblProductCatalog>[modelDemandProduction.productIds.Count()];
                int s = 0;
                foreach (long itm in modelDemandProduction.productIds)
                {
                    BaseOutput gpc = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm, true, out modelDemandProduction.ProductCatalogArray);
                    modelDemandProduction.ProductCatalogListFEA[s] = modelDemandProduction.ProductCatalogArray.ToList();
                    s = s + 1;
                }

                BaseOutput pcln = srv.WS_GetProductCatalogControlsByProductID(baseInput, modelDemandProduction.productIds.LastOrDefault(), true, out modelDemandProduction.ProductCatalogControlArray);

                modelDemandProduction.ProductCatalogControlList = modelDemandProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategory.Id).ToList();

                BaseOutput pcb = srv.WS_GetProductionControls(baseInput, out modelDemandProduction.ProductionControlArray);
                modelDemandProduction.ProductionControlList = modelDemandProduction.ProductionControlArray.Where(x => x.Demand_Production_Id == pId).ToList();

                modelDemandProduction.productionControlEVIds = new long[modelDemandProduction.ProductionControlList.Count()];
                for (int t = 0; t < modelDemandProduction.ProductionControlList.Count(); t++)
                {
                    modelDemandProduction.productionControlEVIds[t] = (long)modelDemandProduction.ProductionControlList[t].EnumValueId;
                }
            }



            return View(modelDemandProduction);
        }

        public ActionResult AdminUnit(long pId = 0, long productAddressId = 0)
        {
            baseInput = new BaseInput();

            modelDemandProduction = new DemandProductionViewModel();

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(baseInput, out modelDemandProduction.PRMAdminUnitArray);
            modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelDemandProduction.PRMAdminUnitArray);
            //modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();

            modelDemandProduction.productAddressIds = null;
            if (productAddressId > 0)
            {
                BaseOutput gpa = srv.WS_GetProductAddressById(baseInput, productAddressId, true, out modelDemandProduction.ProductAddress);
                if (modelDemandProduction.ProductAddress.fullAddressId != "")
                {

                    //modelDemandProduction.productAddressIds = modelDemandProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();
                    //    modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();





                    modelDemandProduction.ProductAddress.fullAddressId = "0," + modelDemandProduction.ProductAddress.fullAddressId;
                    modelDemandProduction.productAddressIds = modelDemandProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();

                    modelDemandProduction.PRMAdminUnitArrayFA = new IList<tblPRM_AdminUnit>[modelDemandProduction.productAddressIds.Count()];
                    int s = 0;
                    foreach (long itm in modelDemandProduction.productAddressIds)
                    {
                        BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(baseInput, (int)itm, true, out modelDemandProduction.PRMAdminUnitArray);
                        modelDemandProduction.PRMAdminUnitArrayFA[s] = modelDemandProduction.PRMAdminUnitArray.ToList();
                        s = s + 1;
                    }
                }
            }


            if (Session["arrDNum"] == null)
            {
                Session["arrDNum"] = modelDemandProduction.arrNum;
            }
            else
            {
                modelDemandProduction.arrNum = (int)Session["arrDNum"] + 1;
                Session["arrDNum"] = modelDemandProduction.arrNum;
            }

            return View(modelDemandProduction);
        }

        [HttpPost]
        public ActionResult Index(DemandProductionViewModel model, FormCollection collection, IList<HttpPostedFileBase> attachfiles)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;

            modelDemandProduction = new DemandProductionViewModel();
            modelDemandProduction.DemandProduction = new tblDemand_Production();

            //if (Session["SelectedProduct"] == null)
            //{
            //Guid sp = Guid.NewGuid();
            //Session["SelectedProduct"] = sp;
            //}
            //var dd = Session["SelectedProduct"];
            //Session.Contents.Remove("SelectedProduct");

            if(model.confirmList==true)
            {
                Guid grupId = Guid.NewGuid();
                modelDemandProduction.DemandProduction.grup_Id = grupId.ToString();
                modelDemandProduction.DemandProduction.isSelected = true;
                modelDemandProduction.DemandProduction.user_Id = userId;
                modelDemandProduction.DemandProduction.user_IdSpecified = true;
                modelDemandProduction.DemandProduction.isSelected = true;
                modelDemandProduction.DemandProduction.isSelectedSpecified = true;

                BaseOutput upp = srv.WS_UpdateDemand_ProductionForUserID(baseInput, modelDemandProduction.DemandProduction, out  modelDemandProduction.DemandProductionArray);


                if (attachfiles != null)
                {
                    baseInput = new BaseInput();

                    modelDemandProduction = new DemandProductionViewModel();

                    String sDate = DateTime.Now.ToString();
                    DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                    String dy = datevalue.Day.ToString();
                    String mn = datevalue.Month.ToString();
                    String yy = datevalue.Year.ToString();

                    string path = modelDemandProduction.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }


                    foreach (var attachfile in attachfiles)
                    {

                        if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelDemandProduction.fileSize && modelDemandProduction.fileTypes.Contains(attachfile.ContentType))
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


                            modelDemandProduction.ProductionDocument = new tblProduction_Document();
                            BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                            modelDemandProduction.ProductionDocument.grup_Id = grupId.ToString();
                            modelDemandProduction.ProductionDocument.documentUrl = path;
                            modelDemandProduction.ProductionDocument.documentName = fileName;
                            modelDemandProduction.ProductionDocument.documentRealName = ofileName;

                            //modelDemandProduction.ProductionDocument.document_type_ev_Id = documentType.ToString();
                            modelDemandProduction.ProductionDocument.documentSize = attachfile.ContentLength;
                            modelDemandProduction.ProductionDocument.documentSizeSpecified = true;

                            modelDemandProduction.ProductionDocument.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                            modelDemandProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                            BaseOutput apd = srv.WS_AddProductionDocument(baseInput, modelDemandProduction.ProductionDocument, out modelDemandProduction.ProductionDocument);
                        }
                    }

                }



            }else
            {

            modelDemandProduction.DemandProduction.description = model.description;
            modelDemandProduction.DemandProduction.product_Id = model.productId;

                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelDemandProduction.ProductCatalogArray);
                modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.ToList();

                modelDemandProduction.DemandProduction.fullProductId = string.Join(",", modelDemandProduction.ProductCatalogList.Select(x => x.Id));

                modelDemandProduction.DemandProduction.product_IdSpecified = true;

            modelDemandProduction.DemandProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
            //modelDemandProduction.DemandProduction.unit_price = Convert.ToDecimal(model.price);
            modelDemandProduction.DemandProduction.unit_priceSpecified = true;

            modelDemandProduction.DemandProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
            //modelDemandProduction.DemandProduction.quantity = Convert.ToDecimal(model.size);
            modelDemandProduction.DemandProduction.quantitySpecified = true;
            
            modelDemandProduction.DemandProduction.isSelected = true;
            modelDemandProduction.DemandProduction.isSelectedSpecified= true;

            modelDemandProduction.DemandProduction.user_Id = userId;
            modelDemandProduction.DemandProduction.user_IdSpecified = true;

            modelDemandProduction.DemandProduction.Status = 1;
            modelDemandProduction.DemandProduction.StatusSpecified = true;

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Yayinda", out modelDemandProduction.EnumValue);
            modelDemandProduction.DemandProduction.state_eV_Id = modelDemandProduction.EnumValue.Id;
            modelDemandProduction.DemandProduction.state_eV_IdSpecified = true;

            DateTime startDate = (DateTime)model.startDate;
            modelDemandProduction.DemandProduction.startDate = startDate.Ticks;
            modelDemandProduction.DemandProduction.startDateSpecified = true;

            DateTime endDate = (DateTime)model.endDate;
            modelDemandProduction.DemandProduction.endDate = endDate.Ticks;
            modelDemandProduction.DemandProduction.endDateSpecified = true;

            modelDemandProduction.ProductAddress = new tblProductAddress();

            modelDemandProduction.ProductAddress.adminUnit_Id = model.addressId;
                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelDemandProduction.PRMAdminUnitArray);
                modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();
                modelDemandProduction.ProductAddress.fullAddressId = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Id));
                modelDemandProduction.ProductAddress.fullAddress = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Name));
                modelDemandProduction.ProductAddress.adminUnit_IdSpecified = true;
            modelDemandProduction.ProductAddress.addressDesc = model.descAddress;

            BaseOutput apa = srv.WS_AddProductAddress(baseInput, modelDemandProduction.ProductAddress, out  modelDemandProduction.ProductAddress);

            modelDemandProduction.DemandProduction.address_Id = modelDemandProduction.ProductAddress.Id;
            modelDemandProduction.DemandProduction.address_IdSpecified = true;


            BaseOutput app = srv.WS_AddDemand_Production(baseInput, modelDemandProduction.DemandProduction, out  modelDemandProduction.DemandProduction);

            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);
            modelDemandProduction.ProductionCalendar = new tblProduction_Calendar();


            for(int ecv=0;ecv<model.enumCat.Length;ecv++)
            {
                modelDemandProduction.ProductionControl = new tblProductionControl();

                modelDemandProduction.ProductionControl.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
                modelDemandProduction.ProductionControl.Demand_Production_IdSpecified = true;

                modelDemandProduction.ProductionControl.EnumCategoryId=model.enumCat[ecv];
                modelDemandProduction.ProductionControl.EnumCategoryIdSpecified = true;

                modelDemandProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                modelDemandProduction.ProductionControl.EnumValueIdSpecified = true;

                modelDemandProduction.ProductionControl.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                modelDemandProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelDemandProduction.ProductionControl, out modelDemandProduction.ProductionControl);
            }


            modelDemandProduction.ProductionCalendar.Production_Id = modelDemandProduction.DemandProduction.Id;
            modelDemandProduction.ProductionCalendar.Production_IdSpecified = true;

            modelDemandProduction.ProductionCalendar.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
            modelDemandProduction.ProductionCalendar.Production_type_eV_IdSpecified = true;

            modelDemandProduction.ProductionCalendar.Months = model.checkedMonth;
            modelDemandProduction.ProductionCalendar.Transportation_eV_Id = model.shippingSchedule;
            modelDemandProduction.ProductionCalendar.Transportation_eV_IdSpecified = true;

            BaseOutput apc = srv.WS_AddProduction_Calendar(baseInput, modelDemandProduction.ProductionCalendar, out modelDemandProduction.ProductionCalendar);



            modelDemandProduction.ProductionDocument = new tblProduction_Document();
            modelDemandProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
            modelDemandProduction.ProductionDocument.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
            modelDemandProduction.ProductionDocument.Demand_Production_IdSpecified = true;
            modelDemandProduction.ProductionDocument.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
            modelDemandProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

            BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelDemandProduction.ProductionDocument, out modelDemandProduction.ProductionDocumentArray);

            }

            Session["documentGrupId"] = null;
            TempData["Success"] = modelDemandProduction.messageSuccess;

            return RedirectToAction("Index", "DemandProduction");
        }



        public ActionResult Edit(long id, long? userId)
        {
            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelDemandProduction = new DemandProductionViewModel();
            modelDemandProduction.DemandProduction = new tblDemand_Production();


            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
            BaseOutput person = srv.WS_GetPersonByUserId(baseInput, (long)userId, true, out modelDemandProduction.Person);


            BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);

            if (modelDemandProduction.DemandProduction == null)
            {
                return RedirectToAction("Index", "DemandProduction");
            }

            if (modelDemandProduction.DemandProduction.grup_Id != null)
            {
                return RedirectToAction("Index", "DemandProduction");
            }

            modelDemandProduction.Id = modelDemandProduction.DemandProduction.Id;
            modelDemandProduction.productId = (int)modelDemandProduction.DemandProduction.product_Id;
            modelDemandProduction.description = modelDemandProduction.DemandProduction.description;
            modelDemandProduction.productAddressId = (long)modelDemandProduction.DemandProduction.address_Id;
            modelDemandProduction.size = (modelDemandProduction.DemandProduction.quantity.ToString()).Replace(',', '.');
            modelDemandProduction.price = (modelDemandProduction.DemandProduction.unit_price.ToString()).Replace(',', '.');
            DateTime startDate = new DateTime((long)modelDemandProduction.DemandProduction.startDate);
            modelDemandProduction.startDate = startDate;
            DateTime endDate = new DateTime((long)modelDemandProduction.DemandProduction.endDate);
            modelDemandProduction.endDate = endDate;

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "shippingSchedule", out modelDemandProduction.EnumCategory);
            if (modelDemandProduction.EnumCategory == null)
                modelDemandProduction.EnumCategory = new tblEnumCategory();

            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategory.Id, true, out modelDemandProduction.EnumValueArray);
            modelDemandProduction.EnumValueShippingScheduleList = modelDemandProduction.EnumValueArray.ToList();



            BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelDemandProduction.EnumCategory);
            if (modelDemandProduction.EnumCategory == null)
                modelDemandProduction.EnumCategory = new tblEnumCategory();

            BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategory.Id, true, out modelDemandProduction.EnumValueArray);
            modelDemandProduction.EnumValueMonthList = modelDemandProduction.EnumValueArray.ToList();


            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "Demand", out modelDemandProduction.EnumValue);
            //BaseOutput sml = srv.WS_GetProductionCalenadarProductionId(baseInput, modelDemandProduction.DemandProduction.Id, true, out modelDemandProduction.ProductionCalendarArray);
            BaseOutput sml = srv.WS_GetProductionCalendarByProductionId(baseInput, modelDemandProduction.DemandProduction.Id, true,modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.ProductionCalendar);

            //modelDemandProduction.ProductionCalendar = modelDemandProduction.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == modelDemandProduction.EnumValue.Id).FirstOrDefault() ;

            string[] months = modelDemandProduction.ProductionCalendar.Months.Split(',');
            modelDemandProduction.selectedMonth = months;

            modelDemandProduction.shippingSchedule = (long)modelDemandProduction.ProductionCalendar.Transportation_eV_Id;
            modelDemandProduction.productionCalendarId = modelDemandProduction.ProductionCalendar.Id;

            if (Session["documentGrupId"] == null)
            {
                Guid dg = Guid.NewGuid();
                Session["documentGrupId"] = dg;
                this.Session.Timeout = 20;
            }
            Session["arrDPNum"] = null;
            Session["arrDNum"] = null;

            return View(modelDemandProduction);
        }

        [HttpPost]
        public ActionResult Edit(DemandProductionViewModel model, FormCollection collection)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;

            modelDemandProduction = new DemandProductionViewModel();

            BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, model.Id, true, out modelDemandProduction.DemandProduction);

            modelDemandProduction.DemandProduction.description = model.description;
            modelDemandProduction.DemandProduction.product_Id = model.productId;


            modelDemandProduction.DemandProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
            modelDemandProduction.DemandProduction.unit_priceSpecified = true;

            modelDemandProduction.DemandProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
            modelDemandProduction.DemandProduction.quantitySpecified = true;

            DateTime startDate = (DateTime)model.startDate;
            modelDemandProduction.DemandProduction.startDate = startDate.Ticks;
            modelDemandProduction.DemandProduction.startDateSpecified = true;

            DateTime endDate = (DateTime)model.endDate;
            modelDemandProduction.DemandProduction.endDate = endDate.Ticks;
            modelDemandProduction.DemandProduction.endDateSpecified = true;

            BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelDemandProduction.ProductCatalogArray);
            modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.ToList();

            modelDemandProduction.DemandProduction.fullProductId = string.Join(",", modelDemandProduction.ProductCatalogList.Select(x => x.Id));

            BaseOutput app = srv.WS_UpdateDemand_Production(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProduction);


            BaseOutput gpabi = srv.WS_GetProductAddressById(baseInput, (long)modelDemandProduction.DemandProduction.address_Id, true, out modelDemandProduction.ProductAddress);

            modelDemandProduction.ProductAddress.adminUnit_Id = model.addressId;
            BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelDemandProduction.PRMAdminUnitArray);
            modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();

            modelDemandProduction.ProductAddress.fullAddressId = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Id));
            modelDemandProduction.ProductAddress.fullAddress = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Name));
            modelDemandProduction.ProductAddress.adminUnit_IdSpecified = true;
            modelDemandProduction.ProductAddress.addressDesc = model.descAddress;

            BaseOutput apa = srv.WS_UpdateProductAddress(baseInput, modelDemandProduction.ProductAddress, out modelDemandProduction.ProductAddress);


            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "Demand", out modelDemandProduction.EnumValue);


            for (long ecv = 0; ecv < model.enumCat.Length; ecv++)
            {
                if (model.pcId != null)
                {
                    BaseOutput gpcbi = srv.WS_GetProductionControlById(baseInput, model.pcId[ecv], true, out modelDemandProduction.ProductionControl);

                    modelDemandProduction.ProductionControl.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
                    modelDemandProduction.ProductionControl.Demand_Production_IdSpecified = true;

                    modelDemandProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                    modelDemandProduction.ProductionControl.EnumCategoryIdSpecified = true;

                    modelDemandProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                    modelDemandProduction.ProductionControl.EnumValueIdSpecified = true;

                    modelDemandProduction.ProductionControl.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                    modelDemandProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                    BaseOutput ap = srv.WS_UpdateProductionControl(baseInput, modelDemandProduction.ProductionControl, out modelDemandProduction.ProductionControl);
                }
                else
                {
                    BaseOutput pcb = srv.WS_GetProductionControls(baseInput, out modelDemandProduction.ProductionControlArray);
                    modelDemandProduction.ProductionControlList = modelDemandProduction.ProductionControlArray.Where(x => x.Demand_Production_Id == modelDemandProduction.DemandProduction.Id).ToList();
                    if (ecv == 0)
                    {
                        foreach (tblProductionControl itm in modelDemandProduction.ProductionControlList)
                        {
                            BaseOutput dcb = srv.WS_DeleteProductionControl(baseInput, itm);
                        }
                    }


                    modelDemandProduction.ProductionControl = new tblProductionControl();

                    modelDemandProduction.ProductionControl.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
                    modelDemandProduction.ProductionControl.Demand_Production_IdSpecified = true;

                    modelDemandProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                    modelDemandProduction.ProductionControl.EnumCategoryIdSpecified = true;

                    modelDemandProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                    modelDemandProduction.ProductionControl.EnumValueIdSpecified = true;

                    modelDemandProduction.ProductionControl.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                    modelDemandProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                    BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelDemandProduction.ProductionControl, out modelDemandProduction.ProductionControl);
                }

            }


            BaseOutput pc = srv.WS_GetProduction_CalendarById(baseInput, model.productionCalendarId, true, out modelDemandProduction.ProductionCalendar);

            modelDemandProduction.ProductionCalendar.Production_Id = modelDemandProduction.DemandProduction.Id;
            modelDemandProduction.ProductionCalendar.Production_IdSpecified = true;

            modelDemandProduction.ProductionCalendar.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
            modelDemandProduction.ProductionCalendar.Production_type_eV_IdSpecified = true;

            modelDemandProduction.ProductionCalendar.Months = model.checkedMonth;
            modelDemandProduction.ProductionCalendar.Transportation_eV_Id = model.shippingSchedule;
            modelDemandProduction.ProductionCalendar.Transportation_eV_IdSpecified = true;

            BaseOutput apc = srv.WS_UpdateProductionCalendar(baseInput, modelDemandProduction.ProductionCalendar, out modelDemandProduction.ProductionCalendar);



            modelDemandProduction.ProductionDocument = new tblProduction_Document();
            modelDemandProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
            modelDemandProduction.ProductionDocument.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
            modelDemandProduction.ProductionDocument.Demand_Production_IdSpecified = true;
            modelDemandProduction.ProductionDocument.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
            modelDemandProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

            BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelDemandProduction.ProductionDocument, out modelDemandProduction.ProductionDocumentArray);

            Session["documentGrupId"] = null;
            TempData["Success"] = modelDemandProduction.messageSuccess;

            return RedirectToAction("Edit", "DemandProduction", new { id = model.Id });
        }


        public ActionResult SelectedProducts(bool pdf=false)
        {
            baseInput = new BaseInput();
            modelDemandProduction = new DemandProductionViewModel();

            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }


            BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

            BaseOutput gpd = srv.WS_GetDemandProductionDetailistForUser(baseInput, userId, true, out modelDemandProduction.ProductionDetailArray);
            modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelDemandProduction.EnumCategory.Id).ToList();

            modelDemandProduction.isPDF = pdf;
            modelDemandProduction.userId = userId;

            var gd = Guid.NewGuid();

            if (modelDemandProduction.isPDF == true)
            {
                return new Rotativa.PartialViewAsPdf(modelDemandProduction) { FileName = gd + ".pdf" };
            }
            else
            {
                return View(modelDemandProduction);
            }
        }


        public ActionResult ChooseFileTemplate(int pId)
        {
            baseInput = new BaseInput();
            modelDemandProduction = new DemandProductionViewModel();

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelDemandProduction.EnumCategory);

            if (modelDemandProduction.EnumCategory == null)
                modelDemandProduction.EnumCategory = new tblEnumCategory() ;
            
            BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelDemandProduction.ProductCatalogControlArray);

            modelDemandProduction.ProductCatalogControlDocumentTypeList = modelDemandProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId == modelDemandProduction.EnumCategory.Id).ToList();


                BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategory.Id, true, out modelDemandProduction.EnumValueArray);
                modelDemandProduction.EnumValueDocumentTypeList = modelDemandProduction.EnumValueArray.ToList();


                string grup_Id = Session["documentGrupId"].ToString(); ;
                bool flag = true;
                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                BaseOutput tfs = srv.WS_GetDocumentSizebyGroupId(grup_Id, modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.totalSize, out flag);

            return View(modelDemandProduction);
        }


        [HttpPost]
        public void File(IList<HttpPostedFileBase> file, int documentType)
        {
            if (file != null)
            {
                string documentGrupId = Session["documentGrupId"].ToString();

                long userId = 0;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                baseInput = new BaseInput();
                baseInput.userID = userId;

                modelDemandProduction = new DemandProductionViewModel();

                String sDate = DateTime.Now.ToString();
                DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                String dy = datevalue.Day.ToString();
                String mn = datevalue.Month.ToString();
                String yy = datevalue.Year.ToString();

                string path = modelDemandProduction.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                foreach (var attachfile in file)
                {
                    //var dec=(attachfile.ContentLength / 1024);
                    //var decd = (decimal)731 / 1024;
                    //var fl = Math.Round(dec,2);

                    if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelDemandProduction.fileSize && modelDemandProduction.fileTypes.Contains(attachfile.ContentType))
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


                        modelDemandProduction.ProductionDocument = new tblProduction_Document();
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                        modelDemandProduction.ProductionDocument.grup_Id = documentGrupId;
                        modelDemandProduction.ProductionDocument.documentUrl = path;
                        modelDemandProduction.ProductionDocument.documentName = fileName;
                        modelDemandProduction.ProductionDocument.documentRealName= ofileName;

                        modelDemandProduction.ProductionDocument.document_type_ev_Id = documentType.ToString();
                        modelDemandProduction.ProductionDocument.documentSize = attachfile.ContentLength;
                        modelDemandProduction.ProductionDocument.documentSizeSpecified = true;

                        modelDemandProduction.ProductionDocument.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                        modelDemandProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                        BaseOutput apd = srv.WS_AddProductionDocument(baseInput, modelDemandProduction.ProductionDocument, out modelDemandProduction.ProductionDocument);
                    }
                }

            }
        }

        public ActionResult SelectedDocuments(long PId = 0)
        {
            baseInput = new BaseInput();
            modelDemandProduction = new DemandProductionViewModel();
            string grup_Id=Session["documentGrupId"].ToString();

            BaseOutput gpbu = srv.WS_GetProductionDocuments(baseInput, out modelDemandProduction.ProductionDocumentArray);         


            if (PId > 0)
            {
                modelDemandProduction.ProductionDocumentList = modelDemandProduction.ProductionDocumentArray.Where(x => x.Potential_Production_Id == PId || x.grup_Id == grup_Id).ToList();
            }
            else
            {
                modelDemandProduction.ProductionDocumentList = modelDemandProduction.ProductionDocumentArray.Where(x => x.grup_Id == grup_Id).ToList();
            }

            return View(modelDemandProduction);
        }

        public void DeleteSelectedDocument(long id)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;
            modelDemandProduction = new DemandProductionViewModel();

            BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput, id, true, out modelDemandProduction.ProductionDocument);

            BaseOutput dpd = srv.WS_DeleteProductionDocument(baseInput, modelDemandProduction.ProductionDocument);
        }

        public void DeleteSelectedDemandProduct(long id)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;
            modelDemandProduction = new DemandProductionViewModel();

            BaseOutput gpd = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);

            BaseOutput dpd = srv.WS_DeleteDemand_Production(baseInput, modelDemandProduction.DemandProduction);
        }
    }
}
