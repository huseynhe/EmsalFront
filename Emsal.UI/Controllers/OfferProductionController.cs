using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    //[EmsalAuthorization(AuthorizedAction = ActionName.offerProduction)]
    [EmsalAuthorization(AuthorizedAction = ActionName.offerProduction)]
    public class OfferProductionController : Controller
    {

        private static string fullAddressId = "";
        private BaseInput baseInput;
        private static IList<long> soldPID;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
       // Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private OfferProductionViewModel modelOfferProduction;

        public ActionResult Index(long ppId = 0)
        {
            try
            {
                //TempData["Success"] = null;
                //long unixDate = DateTime.Now.Ticks;
                //DateTime start = new DateTime(636012864000000000);
                //var rdd = start.Ticks;

                Session["arrONum"] = null;
                string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                if (ppId > 0)
                {
                    modelOfferProduction.ppId = ppId;
                }
                else
                {
                    BaseOutput userRolId = srv.WS_GetUserRolesByUserId(baseInput, (long)userId, true, out modelOfferProduction.UserRoleArray);

                    modelOfferProduction.UserRole = modelOfferProduction.UserRoleArray.ToList().Where(x => x.RoleId == 15).FirstOrDefault();

                    if (modelOfferProduction.UserRole != null)
                        return RedirectToAction("Redirect", "Home");
                }

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "shippingSchedule", out modelOfferProduction.EnumCategory);
                if (modelOfferProduction.EnumCategory == null)
                    modelOfferProduction.EnumCategory = new tblEnumCategory();

                BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueShippingScheduleList = modelOfferProduction.EnumValueArray.ToList();


                BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelOfferProduction.EnumCategory);
                if (modelOfferProduction.EnumCategory == null)
                    modelOfferProduction.EnumCategory = new tblEnumCategory();

                BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueArray.ToList();


                BaseOutput gop = srv.WS_GetOffer_ProductionsByUserID(baseInput, (long)userId, true, out modelOfferProduction.OfferProductionArray);
                soldPID = new List<long>();
                if (modelOfferProduction.OfferProductionArray != null)
                {
                    foreach (long item in modelOfferProduction.OfferProductionArray.Select(x => x.product_Id).ToList())
                    {
                        soldPID.Add(item);
                    }
                }

                modelOfferProduction.oldPID = soldPID;

                modelOfferProduction.fullAddressId = fullAddressId;

                if (Session["documentGrupId"] == null)
                {
                    Guid dg = Guid.NewGuid();
                    Session["documentGrupId"] = dg;
                    this.Session.Timeout = 20;
                }

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductOrigin(long selectedPOriginId = 0)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput gpo = srv.WS_GetAdminUnitsByParentId(baseInput, 0, true, out modelOfferProduction.PRMAdminUnitArray);
                modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.OrderBy(x=>x.Name).ToList();

                if (selectedPOriginId > 0)
                {
                    modelOfferProduction.selectedPOriginId = selectedPOriginId;
                }


                BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput, modelOfferProduction.User.Id, true, out modelOfferProduction.UserRoleArray);
                modelOfferProduction.UserRole = modelOfferProduction.UserRoleArray.FirstOrDefault();

                if (modelOfferProduction.UserRole.RoleId == 15 || modelOfferProduction.UserRole.RoleId == 24)
                {
                    modelOfferProduction.originStatus = 1;
                }

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductCatalogForSale()
        {
            try
            {
                baseInput = new BaseInput();

                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelOfferProduction.ProductCatalogDetailArray);

                if (modelOfferProduction.ProductCatalogDetailArray == null)
                {
                    modelOfferProduction.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelOfferProduction.ProductCatalogDetailList = modelOfferProduction.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelOfferProduction.oldPID = new List<long>();
                modelOfferProduction.oldPID = soldPID;

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public string ProductCatalogForSaleAS(long prId)
        {
            try
            {
                baseInput = new BaseInput();

                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;                       

                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, prId, true, out modelOfferProduction.ProductCatalogArray);
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                return string.Join(",", modelOfferProduction.ProductCatalogList.Select(x => x.Id)); 

            }
            catch (Exception ex)
            {
                return ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public string AnnouncementBy(long prId)
        {
            try
            {
                baseInput = new BaseInput();

                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput ga = srv.WS_GetAnnouncementsByProductId(baseInput, prId, true, out modelOfferProduction.AnnouncementArray);

                modelOfferProduction.AnnouncementList = modelOfferProduction.AnnouncementArray.ToList();

                string up = null;
                if (modelOfferProduction.AnnouncementList.Count > 0)
                {
                    up = modelOfferProduction.AnnouncementList.FirstOrDefault().unit_price.ToString()+" ("+ modelOfferProduction.AnnouncementList.FirstOrDefault().quantity_type_Name+ " / man)";
                }

                return up;
            }
            catch (Exception ex)
            {
                return ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public ActionResult ProductCatalog(int pId = 0, long ppId = 0, long opId = 0, string fpid = "")
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                //BaseOutput bouput = srv.WS_GetProductCatalogs(baseInput, out modelOfferProduction.ProductCatalogArray);
                //modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.Where(x => x.ProductCatalogParentID == pId).ToList();

                BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, (int)pId, true, out modelOfferProduction.ProductCatalogArray);
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                modelOfferProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                foreach (tblProductCatalog itm in modelOfferProduction.ProductCatalogList)
                {
                    BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelOfferProduction.ProductCatalogArrayPC);
                    if (modelOfferProduction.ProductCatalogArrayPC.ToList().Count == 0)
                    {
                        if (itm.canBeOrder == 1)
                            modelOfferProduction.ProductCatalogListPC.Add(itm);
                    }
                    else
                    {
                        modelOfferProduction.ProductCatalogListPC.Add(itm);
                    }
                }
                modelOfferProduction.ProductCatalogList = null;
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogListPC;


                BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelOfferProduction.EnumCategoryArray);
                modelOfferProduction.EnumCategoryList = modelOfferProduction.EnumCategoryArray.ToList();

                BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueList = modelOfferProduction.EnumValueArray.ToList();

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelOfferProduction.EnumCategory);

                BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelOfferProduction.ProductCatalogControlArray);
                modelOfferProduction.ProductCatalogControlList = modelOfferProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelOfferProduction.EnumCategory.Id).ToList();


                if (Session["arrOPNum"] == null)
                {
                    Session["arrOPNum"] = modelOfferProduction.arrPNum;
                }
                else
                {
                    modelOfferProduction.arrPNum = (int)Session["arrOPNum"] + 1;
                    Session["arrOPNum"] = modelOfferProduction.arrPNum;
                }


                if (ppId > 0 || opId>0 || fpid!="")
                {
                    if (fpid == "")
                    {
                        if (ppId > 0)
                        {
                            BaseOutput gpp = srv.WS_GetPotential_ProductionById(baseInput, ppId, true, out modelOfferProduction.PotentialProduction);

                            modelOfferProduction.Id = pId;
                            modelOfferProduction.productId = (int)modelOfferProduction.PotentialProduction.product_Id;
                            modelOfferProduction.description = modelOfferProduction.PotentialProduction.description;
                            modelOfferProduction.psize = (modelOfferProduction.PotentialProduction.quantity.ToString()).Replace(',', '.');
                            modelOfferProduction.pprice = (modelOfferProduction.PotentialProduction.unit_price.ToString()).Replace(',', '.');
                            modelOfferProduction.PotentialProduction.fullProductId = "0," + modelOfferProduction.PotentialProduction.fullProductId;
                            modelOfferProduction.productIds = modelOfferProduction.PotentialProduction.fullProductId.Split(',').Select(long.Parse).ToArray();

                        }

                        if (opId > 0)
                        {
                            modelOfferProduction.opId = opId;
                            BaseOutput gop = srv.WS_GetOffer_ProductionById(baseInput, opId, true, out modelOfferProduction.OfferProduction);

                            BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, (long)modelOfferProduction.OfferProduction.product_Id, true, out modelOfferProduction.ProductCatalogArray);
                            modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                            modelOfferProduction.Id = modelOfferProduction.OfferProduction.Id;
                            modelOfferProduction.productId = (int)modelOfferProduction.OfferProduction.product_Id;
                            modelOfferProduction.description = modelOfferProduction.OfferProduction.description;
                            modelOfferProduction.productIds = ("0," + string.Join(",", modelOfferProduction.ProductCatalogList.Select(x => x.Id))).Split(',').Select(long.Parse).ToArray();
                        }
                    }
                    else
                    {
                        modelOfferProduction.OfferProduction = new tblOffer_Production();
                        modelOfferProduction.productId = Int32.Parse(ppId.ToString());
                        modelOfferProduction.description = "";
                        modelOfferProduction.fpid = fpid;


                        modelOfferProduction.productIds = ("0," + fpid).Split(',').Select(long.Parse).ToArray();
                    }

                    modelOfferProduction.ProductCatalogListFEA = new IList<tblProductCatalog>[modelOfferProduction.productIds.Count()];
                    int s = 0;
                    foreach (long itm in modelOfferProduction.productIds)
                    {
                        BaseOutput gpc = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm, true, out modelOfferProduction.ProductCatalogArray);

                        modelOfferProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                        foreach (tblProductCatalog itmpc in modelOfferProduction.ProductCatalogArray.ToList())
                        {
                            BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itmpc.Id, true, out modelOfferProduction.ProductCatalogArrayPC);
                            if (modelOfferProduction.ProductCatalogArrayPC.ToList().Count == 0)
                            {
                                if (itmpc.canBeOrder == 1)
                                    modelOfferProduction.ProductCatalogListPC.Add(itmpc);
                            }
                            else
                            {
                                modelOfferProduction.ProductCatalogListPC.Add(itmpc);
                            }
                        }
                        modelOfferProduction.ProductCatalogListFEA[s] = modelOfferProduction.ProductCatalogListPC;


                        s = s + 1;
                    }

                    if (fpid == "")
                    {
                        BaseOutput pcln = srv.WS_GetProductCatalogControlsByProductID(baseInput, modelOfferProduction.productIds.LastOrDefault(), true, out modelOfferProduction.ProductCatalogControlArray);

                        modelOfferProduction.ProductCatalogControlList = modelOfferProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelOfferProduction.EnumCategory.Id).ToList();

                        
                        if (opId > 0)
                        {
                            BaseOutput pcb = srv.WS_GetProductionControlsByOfferProductionId(baseInput, opId,true, out modelOfferProduction.ProductionControlArray);

                            modelOfferProduction.ProductionControlList = modelOfferProduction.ProductionControlArray.ToList();
                            //modelOfferProduction.ProductionControlList = modelOfferProduction.ProductionControlArray.Where(x => x.Offer_Production_Id == opId).ToList();
                        }
                        else if (ppId > 0)
                        {
                            BaseOutput ppcb = srv.WS_GetProductionControlsByPotentialProductionId(baseInput, ppId,true, out modelOfferProduction.ProductionControlArray);

                            modelOfferProduction.ProductionControlList = modelOfferProduction.ProductionControlArray.ToList();

                            //modelOfferProduction.ProductionControlList = modelOfferProduction.ProductionControlArray.Where(x => x.Potential_Production_Id == ppId).ToList();
                        }


                        modelOfferProduction.productionControlEVIds = new long[modelOfferProduction.ProductionControlList.Count()];
                        for (int t = 0; t < modelOfferProduction.ProductionControlList.Count(); t++)
                        {
                            modelOfferProduction.productionControlEVIds[t] = (long)modelOfferProduction.ProductionControlList[t].EnumValueId;
                        }
                    }
                }

                modelOfferProduction.oldPID = new List<long>();
                modelOfferProduction.oldPID = soldPID;

                modelOfferProduction.Id = pId;
                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Unitofmeasurement(int pId = 0)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                //BaseOutput bouput = srv.WS_GetProductCatalogs(baseInput, out modelOfferProduction.ProductCatalogArray);
                //modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.Where(x => x.ProductCatalogParentID == pId).ToList();

                BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, (int)pId, true, out modelOfferProduction.ProductCatalogArray);
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                modelOfferProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                foreach (tblProductCatalog itm in modelOfferProduction.ProductCatalogList)
                {
                    BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelOfferProduction.ProductCatalogArrayPC);
                    if (modelOfferProduction.ProductCatalogArrayPC.ToList().Count == 0)
                    {
                        if (itm.canBeOrder == 1)
                            modelOfferProduction.ProductCatalogListPC.Add(itm);
                    }
                    else
                    {
                        modelOfferProduction.ProductCatalogListPC.Add(itm);
                    }
                }
                modelOfferProduction.ProductCatalogList = null;
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogListPC;

                BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelOfferProduction.EnumCategoryArray);
                modelOfferProduction.EnumCategoryList = modelOfferProduction.EnumCategoryArray.ToList();

                BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueList = modelOfferProduction.EnumValueArray.ToList();

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelOfferProduction.EnumCategory);

                BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelOfferProduction.ProductCatalogControlArray);
                modelOfferProduction.ProductCatalogControlList = modelOfferProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelOfferProduction.EnumCategory.Id).ToList();

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Getmonth(string startDateYear, string startDateMonth, string endDateYear, string endDateMonth)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelOfferProduction.EnumCategory);
                if (modelOfferProduction.EnumCategory == null)
                    modelOfferProduction.EnumCategory = new tblEnumCategory();

                BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueArray.ToList();

                modelOfferProduction.EnumValueMonthListFV = new List<tblEnumValue>();

                if (startDateYear == endDateYear)
                {
                    modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueMonthList.Where(x => x.Id >= long.Parse(startDateMonth)).ToList();

                    modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueMonthList.Where(x => x.Id <= long.Parse(endDateMonth)).ToList();

                    foreach (var item in modelOfferProduction.EnumValueMonthList)
                    {
                        item.description = item.description + " (" + endDateYear + ")";
                        item.Status = long.Parse(GetQuarter(DateTime.ParseExact(item.name, "MMMM", CultureInfo.InvariantCulture).Month).ToString() + endDateYear);
                        modelOfferProduction.EnumValueMonthListFV.Add(item);
                    }
                }
                else
                {
                    //if ((Int32.Parse(endDateYear) - Int32.Parse(startDateYear)) == 1)
                    //{
                    modelOfferProduction.EnumValueMonthListFVE = modelOfferProduction.EnumValueMonthList.Where(x => x.Id >= long.Parse(startDateMonth)).ToList();

                    foreach (var item in modelOfferProduction.EnumValueMonthListFVE)
                    {
                        tblEnumValue envb = new tblEnumValue();

                        envb.Id = item.Id;
                        envb.name = item.name;
                        envb.description = item.description + " (" + startDateYear + ")";
                        envb.Status = long.Parse(GetQuarter(DateTime.ParseExact(item.name, "MMMM", CultureInfo.InvariantCulture).Month).ToString() + startDateYear);
                        modelOfferProduction.EnumValueMonthListFV.Add(envb);
                    }

                    modelOfferProduction.EnumValueMonthListFVE = modelOfferProduction.EnumValueMonthList.Where(x => x.Id <= long.Parse(endDateMonth)).ToList();

                    foreach (var item in modelOfferProduction.EnumValueMonthListFVE)
                    {
                        item.description = item.description + " (" + endDateYear + ")";
                        item.Status = long.Parse(GetQuarter(DateTime.ParseExact(item.name, "MMMM", CultureInfo.InvariantCulture).Month).ToString() + endDateYear);
                        modelOfferProduction.EnumValueMonthListFV.Add(item);
                    }

                    //}
                }

                modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueMonthListFV;

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult AdminUnit(int pId = 0, long ppId = 0, long productAddressId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelOfferProduction = new OfferProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput bouput = srv.WS_GetPRM_AdminUnits(baseInput, out modelOfferProduction.PRMAdminUnitArray);
                modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

                //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelOfferProduction.PRMAdminUnitArray);
                //modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();


                modelOfferProduction.productAddressIds = null;
                if (productAddressId > 0)
                {
                    BaseOutput gpa = srv.WS_GetProductAddressById(baseInput, productAddressId, true, out modelOfferProduction.ProductAddress);
                    if (modelOfferProduction.ProductAddress.fullAddressId != "")
                    {

                        modelOfferProduction.ProductAddress.fullAddressId = "0," + modelOfferProduction.ProductAddress.fullAddressId;
                        modelOfferProduction.productAddressIds = modelOfferProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();

                        modelOfferProduction.PRMAdminUnitArrayFA = new IList<tblPRM_AdminUnit>[modelOfferProduction.productAddressIds.Count()];
                        int s = 0;
                        foreach (long itm in modelOfferProduction.productAddressIds)
                        {
                            BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(baseInput, (int)itm, true, out modelOfferProduction.PRMAdminUnitArray);
                            modelOfferProduction.PRMAdminUnitArrayFA[s] = modelOfferProduction.PRMAdminUnitArray.ToList();
                            s = s + 1;
                        }

                    if (modelOfferProduction.PRMAdminUnitArrayFA[s - 1].Count() > 0)
                    {
                        modelOfferProduction.productAddressIds = (modelOfferProduction.ProductAddress.fullAddressId + ",0").Split(',').Select(long.Parse).ToArray();
                    }

                    //modelOfferProduction.ProductAddress = new tblProductAddress();
                    }
                }


                if (ppId > 0)
                {
                    BaseOutput gppbid = srv.WS_GetPotential_ProductionById(baseInput, ppId, true, out modelOfferProduction.PotentialProduction);

                    BaseOutput gpa = srv.WS_GetProductAddressById(baseInput, (long)modelOfferProduction.PotentialProduction.productAddress_Id, true, out modelOfferProduction.ProductAddress);

                    if (modelOfferProduction.ProductAddress.fullAddressId != "")
                    {
                        modelOfferProduction.ProductAddress.fullAddressId = "0," + modelOfferProduction.ProductAddress.fullAddressId;
                        modelOfferProduction.productAddressIds = modelOfferProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();

                        modelOfferProduction.PRMAdminUnitArrayFA = new IList<tblPRM_AdminUnit>[modelOfferProduction.productAddressIds.Count()];
                        int s = 0;
                        foreach (long itm in modelOfferProduction.productAddressIds)
                        {
                            BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(baseInput, (int)itm, true, out modelOfferProduction.PRMAdminUnitArray);
                            modelOfferProduction.PRMAdminUnitArrayFA[s] = modelOfferProduction.PRMAdminUnitArray.ToList();
                            s = s + 1;
                        }
                        if (modelOfferProduction.PRMAdminUnitArrayFA[s - 1].Count() > 0)
                        {
                            modelOfferProduction.ProductAddress.fullAddressId = modelOfferProduction.ProductAddress.fullAddressId + ",0";
                            modelOfferProduction.productAddressIds = modelOfferProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();
                        }
                    }
                }

                //fullAddressId = "";


                BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput, modelOfferProduction.User.Id, true, out modelOfferProduction.UserRoleArray);
                modelOfferProduction.UserRole = modelOfferProduction.UserRoleArray.FirstOrDefault();

                if (modelOfferProduction.UserRole.RoleId == 15 || modelOfferProduction.UserRole.RoleId == 24)
                {
                    modelOfferProduction.isPotential = true;
                }

                if (Session["arrONum"] == null)
                {
                    Session["arrONum"] = modelOfferProduction.arrNum;
                }
                else
                {
                    modelOfferProduction.arrNum = (int)Session["arrONum"] + 1;
                    Session["arrONum"] = modelOfferProduction.arrNum;
                }

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Index(OfferProductionViewModel model, FormCollection collection)
        {
            try
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

                modelOfferProduction = new OfferProductionViewModel();
                modelOfferProduction.OfferProduction = new tblOffer_Production();


                //if (Session["SelectedProduct"] == null)
                //{
                //Guid sp = Guid.NewGuid();
                //Session["SelectedProduct"] = sp;
                //}
                //var dd = Session["SelectedProduct"];
                //Session.Contents.Remove("SelectedProduct");

                //if (model.confirmList == true)
                //{
                //    Guid grupId = Guid.NewGuid();
                //    modelOfferProduction.OfferProduction.grup_Id = grupId.ToString();
                //    modelOfferProduction.OfferProduction.isSelected = true;
                //    modelOfferProduction.OfferProduction.user_Id = 1;
                //    modelOfferProduction.OfferProduction.user_IdSpecified = true;
                //    modelOfferProduction.OfferProduction.isSelected = true;
                //    modelOfferProduction.OfferProduction.isSelectedSpecified = true;

                //    //BaseOutput upp = srv.WS_UpdateOfffer_ProductionForUserID(modelOfferProduction.OfferProduction, out  modelOfferProduction.OfferProductionArray);

                //}
                //else
                //{

                BaseOutput galf = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelOfferProduction.PRMAdminUnitArray);
                modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
                fullAddressId = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Id));


                Guid grupId = Guid.NewGuid();

                modelOfferProduction.OfferProduction.grup_Id = grupId.ToString();

                modelOfferProduction.OfferProduction.description = model.description;
                modelOfferProduction.OfferProduction.product_Id = model.productId;
                modelOfferProduction.OfferProduction.product_IdSpecified = true;

                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelOfferProduction.ProductCatalogArray);
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                modelOfferProduction.OfferProduction.title = string.Join(",", modelOfferProduction.ProductCatalogList.Select(x => x.Id));

                modelOfferProduction.OfferProduction.isSelected = false;
                modelOfferProduction.OfferProduction.isSelectedSpecified = true;

                modelOfferProduction.OfferProduction.user_Id = userId;
                modelOfferProduction.OfferProduction.user_IdSpecified = true;

                modelOfferProduction.OfferProduction.Status = 1;
                modelOfferProduction.OfferProduction.StatusSpecified = true;

                string status = "";
                BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput, model.User.Id, true, out model.UserRoleArray);
                model.UserRole = model.UserRoleArray.FirstOrDefault();

                if (model.ppId > 0 || model.UserRole.RoleId==11)
                {
                    status = "Yayinda";
                }
                else
                {
                    status = "Yayinda";
                    //status = "new";
                }

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, status, out modelOfferProduction.EnumValue);
                modelOfferProduction.OfferProduction.state_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.OfferProduction.state_eV_IdSpecified = true;


                BaseOutput envalydn = srv.WS_GetEnumValueByName(baseInput, "new", out modelOfferProduction.EnumValue);
                modelOfferProduction.OfferProduction.monitoring_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.OfferProduction.monitoring_eV_IdSpecified = true;

                BaseOutput evbid = srv.WS_GetEnumValueById(baseInput, long.Parse(model.startDateMonth), true, out modelOfferProduction.EnumValue);
                int sm = DateTime.ParseExact(modelOfferProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                model.startDate = DateTime.Parse(model.startDateYear + "-" + sm.ToString() + "-01");
                DateTime startDate = (DateTime)model.startDate;
                modelOfferProduction.OfferProduction.startDate = startDate.Ticks;
                modelOfferProduction.OfferProduction.startDateSpecified = true;

                BaseOutput evbide = srv.WS_GetEnumValueById(baseInput, long.Parse(model.endDateMonth), true, out modelOfferProduction.EnumValue);
                int em = DateTime.ParseExact(modelOfferProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                model.endDate = DateTime.Parse(model.endDateYear + "-" + em.ToString() + "-01");
                DateTime endDate = (DateTime)model.endDate;
                modelOfferProduction.OfferProduction.endDate = endDate.Ticks;
                modelOfferProduction.OfferProduction.endDateSpecified = true;

                modelOfferProduction.ProductAddress = new tblProductAddress();

                modelOfferProduction.ProductAddress.adminUnit_Id = model.addressId;

                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelOfferProduction.PRMAdminUnitArray);
                modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
                modelOfferProduction.ProductAddress.fullAddressId = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Id));
                modelOfferProduction.ProductAddress.fullAddress = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Name));

                modelOfferProduction.ProductAddress.adminUnit_IdSpecified = true;
                modelOfferProduction.ProductAddress.addressDesc = model.descAddress;

                BaseOutput apa = srv.WS_AddProductAddress(baseInput, modelOfferProduction.ProductAddress, out modelOfferProduction.ProductAddress);

                modelOfferProduction.OfferProduction.productAddress_Id = modelOfferProduction.ProductAddress.Id;
                modelOfferProduction.OfferProduction.productAddress_IdSpecified = true;

                modelOfferProduction.OfferProduction.potentialProduct_Id = model.ppId;
                modelOfferProduction.OfferProduction.potentialProduct_IdSpecified = true;

                modelOfferProduction.OfferProduction.productOrigin = model.productOriginId;
                modelOfferProduction.OfferProduction.productOriginSpecified = true;

                modelOfferProduction.OfferProduction.isNew = 1;
                modelOfferProduction.OfferProduction.isNewSpecified = true;

                BaseOutput app = srv.WS_AddOffer_Production(baseInput, modelOfferProduction.OfferProduction, out modelOfferProduction.OfferProduction);

                if (model.price != null)
                {
                    for (int i = 0; i < model.price.Length; i++)
                    {
                        BaseOutput envalpc = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                        modelOfferProduction.LProductionCalendar = new tblProductionCalendar();

                        modelOfferProduction.LProductionCalendar.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.LProductionCalendar.Production_type_eV_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.Production_Id = modelOfferProduction.OfferProduction.Id;
                        modelOfferProduction.LProductionCalendar.Production_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.year = model.year[i];
                        modelOfferProduction.LProductionCalendar.yearSpecified = true;

                        if (model.day != null)
                        {
                            modelOfferProduction.LProductionCalendar.day = model.day[i];
                            modelOfferProduction.LProductionCalendar.daySpecified = true;
                        }

                        BaseOutput evbidq = srv.WS_GetEnumValueById(baseInput, (long)model.month[i], true, out modelOfferProduction.EnumValue);
                        modelOfferProduction.LProductionCalendar.partOfyear = GetQuarter(DateTime.ParseExact(modelOfferProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month);
                        modelOfferProduction.LProductionCalendar.partOfyearSpecified = true;

                        BaseOutput envalgmbid = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelOfferProduction.EnumValue);
                        modelOfferProduction.LProductionCalendar.months_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.LProductionCalendar.months_eV_IdSpecified = true;

                        //modelOfferProduction.LProductionCalendar.oclock = model.hour[i];
                        //modelOfferProduction.LProductionCalendar.oclockSpecified = true;

                        modelOfferProduction.LProductionCalendar.transportation_eV_Id = model.howMany[i];
                        modelOfferProduction.LProductionCalendar.transportation_eV_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.quantity = Convert.ToDecimal(model.size[i].Replace('.', ','));
                        modelOfferProduction.LProductionCalendar.quantitySpecified = true;

                        modelOfferProduction.LProductionCalendar.price = Convert.ToDecimal(model.price[i].Replace('.', ','));
                        modelOfferProduction.LProductionCalendar.priceSpecified = true;

                        modelOfferProduction.LProductionCalendar.offer_Id = modelOfferProduction.OfferProduction.Id;
                        modelOfferProduction.LProductionCalendar.offer_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.type_eV_Id = model.shippingSchedule;
                        modelOfferProduction.LProductionCalendar.type_eV_IdSpecified = true;

                        BaseOutput gpcbi = srv.WS_GetProductionCalendarByInstance(baseInput, modelOfferProduction.LProductionCalendar, out modelOfferProduction.LNProductionCalendar);

                        //BaseOutput gpcall = srv.WS_GetProductionCalendar(baseInput, out modelOfferProduction.LProductionCalendarArray);

                        //modelOfferProduction.LProductionCalendarList = modelOfferProduction.LProductionCalendarArray.ToList();

                        //modelOfferProduction.LProductionCalendarList = modelOfferProduction.LProductionCalendarList.Where(x => x.offer_Id == modelOfferProduction.LProductionCalendar.offer_Id).Where(x => x.Production_type_eV_Id == modelOfferProduction.LProductionCalendar.Production_type_eV_Id).Where(x => x.year == modelOfferProduction.LProductionCalendar.year).Where(x => x.months_eV_Id == modelOfferProduction.LProductionCalendar.months_eV_Id).Where(x => x.type_eV_Id == modelOfferProduction.LProductionCalendar.type_eV_Id).ToList();

                        if (modelOfferProduction.LNProductionCalendar == null)
                        {
                            BaseOutput alpc = srv.WS_AddProductionCalendar(baseInput, modelOfferProduction.LProductionCalendar, out modelOfferProduction.LProductionCalendar);
                        }
                    }
                }

                UpdateDP(modelOfferProduction.OfferProduction.Id);


                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                if (model.enumCat != null)
                {
                    for (int ecv = 0; ecv < model.enumCat.Length; ecv++)
                    {
                        modelOfferProduction.ProductionControl = new tblProductionControl();

                        modelOfferProduction.ProductionControl.Offer_Production_Id = modelOfferProduction.OfferProduction.Id;
                        modelOfferProduction.ProductionControl.Offer_Production_IdSpecified = true;

                        modelOfferProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                        modelOfferProduction.ProductionControl.EnumCategoryIdSpecified = true;

                        modelOfferProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                        modelOfferProduction.ProductionControl.EnumValueIdSpecified = true;

                        modelOfferProduction.ProductionControl.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                        BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelOfferProduction.ProductionControl, out modelOfferProduction.ProductionControl);
                    }
                }

                modelOfferProduction.ProductionDocument = new tblProduction_Document();
                modelOfferProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
                modelOfferProduction.ProductionDocument.Offer_Production_Id = modelOfferProduction.OfferProduction.Id;
                modelOfferProduction.ProductionDocument.Offer_Production_IdSpecified = true;
                modelOfferProduction.ProductionDocument.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelOfferProduction.ProductionDocument, out modelOfferProduction.ProductionDocumentArray);

                //}

                Session["documentGrupId"] = null;


                BaseOutput person = srv.WS_GetPersonByUserId(baseInput, model.User.Id, true, out model.Person);
               
                string namesname = "";

                if (model.Person != null)
                {
                    namesname = model.Person.Name + " " + model.Person.Surname;
                }else
                {
                    namesname = model.User.Username;
                }

                string message = "<b>Hörmətli "+ namesname+ ",</b> <br/><br/> Sizin ərzaq məhsulu təklifiniz  portalda qeydiyyata alınmışdır.<br/> Potensial istehsalçı kimi təqdim etmək istədiyiniz ərzaq məhsulu barədə yerləşdirdiyiniz təkliflər haqqında məlumat 48 saat ərzində təsdiqlənərək portalın “Potensial istehsalçılar və satıcılar” və “Təkliflər” bölmələrində əks olunacaq və eyni zamanda  elektron poçt ünvanınıza bu barədə məlumat göndəriləcəkdir.<br/> <br/> İstehsalçıların və digər satıcıların (idxalçı) fəaliyyətinin və təqdim edilən təkliflərin monitorinqi, habelə həmin ərzaq məhsullarının satın alınması prosedurları Azərbaycan Respublikası Prezidentinin 2016-cı il 11 aprel tarixli 859 nömrəli Fərmanı ilə təsdiq edilmiş “Dövlət müəssisə və təşkilatları (idarələri) tərəfindən ərzaq məhsullarının mərkəzləşdirilmiş qaydada dövlət büdcəsinin vəsaitləri hesabına satın alınması Qaydası”na uğun olaraq həyata keçirləcəkdir.<br/><br/>Azərbaycan Respublikasının Kənd Təsərrüfatı Nazirliyi";

                try
                {
                    MailMessage msg = new MailMessage();

                    msg.To.Add(model.User.Email);
                    msg.Subject = "tedatuk.az";
                    msg.Body = message;
                    msg.IsBodyHtml = true;

                    Mail.SendMail(msg);
                }
                catch { }


                TempData["Success"] = message;

                if (model.ppId > 0)
                {
                    BaseOutput Production = srv.WS_GetPotential_ProductionById(baseInput, model.ppId, true, out modelOfferProduction.PotentialProduction);
                    BaseOutput delet = srv.WS_DeletePotential_Production(baseInput, modelOfferProduction.PotentialProduction);

                    return RedirectToAction("Redirect", "Login");
                }
                else
                {
                    return RedirectToAction("Index", "OfferProduction");
                }

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Edit(long id)
        {
            try
            {

                string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                modelOfferProduction.OfferProduction = new tblOffer_Production();

                BaseOutput gpp = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferProduction.OfferProduction);

                if (modelOfferProduction.OfferProduction == null)
                {
                    return RedirectToAction("Index", "OfferProduction");
                }

                //if (modelOfferProduction.OfferProduction.grup_Id != null)
                //{
                //    return RedirectToAction("Index", "OfferProduction");
                //}

                modelOfferProduction.Id = modelOfferProduction.OfferProduction.Id;
                modelOfferProduction.productId = (int)modelOfferProduction.OfferProduction.product_Id;
                modelOfferProduction.description = modelOfferProduction.OfferProduction.description;
                modelOfferProduction.productAddressId = (long)modelOfferProduction.OfferProduction.productAddress_Id;

                DateTime startDate = new DateTime((long)modelOfferProduction.OfferProduction.startDate);
                modelOfferProduction.startDateYear = startDate.Year;
                modelOfferProduction.startDateMonth = startDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("en"));
                DateTime endDate = new DateTime((long)modelOfferProduction.OfferProduction.endDate);
                modelOfferProduction.endDateYear = endDate.Year;
                modelOfferProduction.endDateMonth = endDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("en"));

                modelOfferProduction.productOriginId = (long)modelOfferProduction.OfferProduction.productOrigin;


                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "shippingSchedule", out modelOfferProduction.EnumCategory);
                if (modelOfferProduction.EnumCategory == null)
                    modelOfferProduction.EnumCategory = new tblEnumCategory();

                BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueShippingScheduleList = modelOfferProduction.EnumValueArray.ToList();



                BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelOfferProduction.EnumCategory);
                if (modelOfferProduction.EnumCategory == null)
                    modelOfferProduction.EnumCategory = new tblEnumCategory();

                BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueArray.ToList();


                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);
                BaseOutput sml = srv.WS_GetProductionCalendarByProductionId(baseInput, modelOfferProduction.OfferProduction.Id, true, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.ProductionCalendar);
                if (Session["documentGrupId"] == null)
                {
                    Guid dg = Guid.NewGuid();
                    Session["documentGrupId"] = dg;
                    this.Session.Timeout = 20;
                }
                Session["arrDPNum"] = null;
                Session["arrDNum"] = null;

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(OfferProductionViewModel model, FormCollection collection)
        {
            try
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


                modelOfferProduction = new OfferProductionViewModel();

                BaseOutput gpp = srv.WS_GetOffer_ProductionById(baseInput, model.Id, true, out modelOfferProduction.OfferProduction);

                modelOfferProduction.OfferProduction.description = model.description;
                modelOfferProduction.OfferProduction.product_Id = model.productId;
                modelOfferProduction.OfferProduction.product_IdSpecified = true;

                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, (long)modelOfferProduction.OfferProduction.product_Id, true, out modelOfferProduction.ProductCatalogArray);
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                modelOfferProduction.OfferProduction.title = string.Join(",", modelOfferProduction.ProductCatalogList.Select(x => x.Id));


                BaseOutput evbid = srv.WS_GetEnumValueById(baseInput, long.Parse(model.startDateMonth), true, out modelOfferProduction.EnumValue);
                int sm = DateTime.ParseExact(modelOfferProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                model.startDate = DateTime.Parse(model.startDateYear + "-" + sm.ToString() + "-01");
                DateTime startDate = (DateTime)model.startDate;
                modelOfferProduction.OfferProduction.startDate = startDate.Ticks;
                modelOfferProduction.OfferProduction.startDateSpecified = true;

                BaseOutput evbide = srv.WS_GetEnumValueById(baseInput, long.Parse(model.endDateMonth), true, out modelOfferProduction.EnumValue);
                int em = DateTime.ParseExact(modelOfferProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                model.endDate = DateTime.Parse(model.endDateYear + "-" + em.ToString() + "-01");
                DateTime endDate = (DateTime)model.endDate;
                modelOfferProduction.OfferProduction.endDate = endDate.Ticks;
                modelOfferProduction.OfferProduction.endDateSpecified = true;

                BaseOutput envalyo = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferProduction.EnumValueo);

                if(modelOfferProduction.EnumValueo.Id!=modelOfferProduction.OfferProduction.state_eV_Id)
                {
                    BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Yayinda", out modelOfferProduction.EnumValue);
                    modelOfferProduction.OfferProduction.state_eV_Id = modelOfferProduction.EnumValue.Id;
                    modelOfferProduction.OfferProduction.state_eV_IdSpecified = true;
                }    

                BaseOutput envalydn = srv.WS_GetEnumValueByName(baseInput, "new", out modelOfferProduction.EnumValue);
                modelOfferProduction.OfferProduction.monitoring_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.OfferProduction.monitoring_eV_IdSpecified = true;

                //BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelOfferProduction.ProductCatalogArray);
                //modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                //modelOfferProduction.OfferProduction.fullProductId = string.Join(",", modelOfferProduction.ProductCatalogList.Select(x => x.Id));

                modelOfferProduction.OfferProduction.productOrigin = model.productOriginId;
                modelOfferProduction.OfferProduction.productOriginSpecified = true;

                modelOfferProduction.OfferProduction.isNew = 1;
                modelOfferProduction.OfferProduction.isNewSpecified = true;

                BaseOutput app = srv.WS_UpdateOffer_Production(baseInput, modelOfferProduction.OfferProduction, out modelOfferProduction.OfferProduction);

                BaseOutput gpabi = srv.WS_GetProductAddressById(baseInput, (long)modelOfferProduction.OfferProduction.productAddress_Id, true, out modelOfferProduction.ProductAddress);


                modelOfferProduction.ProductAddress.adminUnit_Id = model.addressId;
                modelOfferProduction.ProductAddress.adminUnit_IdSpecified = true;

                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelOfferProduction.PRMAdminUnitArray);
                modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
                modelOfferProduction.ProductAddress.fullAddressId = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Id));
                modelOfferProduction.ProductAddress.fullAddress = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Name));

                modelOfferProduction.ProductAddress.adminUnit_IdSpecified = true;
                modelOfferProduction.ProductAddress.addressDesc = model.descAddress;

                BaseOutput apa = srv.WS_UpdateProductAddress(baseInput, modelOfferProduction.ProductAddress, out modelOfferProduction.ProductAddress);

                if (model.size != null)
                {
                    for (int i = 0; i < model.size.Length; i++)
                    {
                        //BaseOutput gpca = srv.WS_GetProductionCalendar(baseInput, out modelOfferProduction.LProductionCalendarArray);

                        BaseOutput envalpc = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                        modelOfferProduction.LProductionCalendar = new tblProductionCalendar();

                        modelOfferProduction.LProductionCalendar.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.LProductionCalendar.Production_type_eV_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.Production_Id = modelOfferProduction.OfferProduction.Id;
                        modelOfferProduction.LProductionCalendar.Production_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.year = model.year[i];
                        modelOfferProduction.LProductionCalendar.yearSpecified = true;

                        if (model.day != null)
                        {
                            modelOfferProduction.LProductionCalendar.day = model.day[i];
                            modelOfferProduction.LProductionCalendar.daySpecified = true;
                        }

                        BaseOutput evbidq = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelOfferProduction.EnumValue);
                        modelOfferProduction.LProductionCalendar.partOfyear = GetQuarter(DateTime.ParseExact(modelOfferProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month);
                        modelOfferProduction.LProductionCalendar.partOfyearSpecified = true;

                        BaseOutput envalgmbid = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelOfferProduction.EnumValue);
                        modelOfferProduction.LProductionCalendar.months_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.LProductionCalendar.months_eV_IdSpecified = true;

                        //modelOfferProduction.LProductionCalendar.oclock = model.hour[i];
                        //modelOfferProduction.LProductionCalendar.oclockSpecified = true;

                        modelOfferProduction.LProductionCalendar.transportation_eV_Id = model.howMany[i];
                        modelOfferProduction.LProductionCalendar.transportation_eV_IdSpecified = true;

                        if (model.price != null)
                        {
                            modelOfferProduction.LProductionCalendar.price = Convert.ToDecimal(model.price[i].Replace('.', ','));
                            modelOfferProduction.LProductionCalendar.priceSpecified = true;
                        }

                        modelOfferProduction.LProductionCalendar.quantity = Convert.ToDecimal(model.size[i].Replace('.', ','));
                        modelOfferProduction.LProductionCalendar.quantitySpecified = true;

                        modelOfferProduction.LProductionCalendar.offer_Id = modelOfferProduction.OfferProduction.Id;
                        modelOfferProduction.LProductionCalendar.offer_IdSpecified = true;

                        modelOfferProduction.LProductionCalendar.type_eV_Id = model.shippingSchedule;
                        modelOfferProduction.LProductionCalendar.type_eV_IdSpecified = true;

                        BaseOutput gpcbi = srv.WS_GetProductionCalendarByInstance(baseInput, modelOfferProduction.LProductionCalendar, out modelOfferProduction.LNProductionCalendar);

                        //BaseOutput gpcall = srv.WS_GetProductionCalendar(baseInput, out modelOfferProduction.LProductionCalendarArray);

                        //modelOfferProduction.LProductionCalendarList = modelOfferProduction.LProductionCalendarArray.ToList();

                        //modelOfferProduction.LProductionCalendarList = modelOfferProduction.LProductionCalendarList.Where(x => x.offer_Id == modelOfferProduction.LProductionCalendar.offer_Id).Where(x => x.Production_type_eV_Id == modelOfferProduction.LProductionCalendar.Production_type_eV_Id).Where(x => x.year == modelOfferProduction.LProductionCalendar.year).Where(x => x.months_eV_Id == modelOfferProduction.LProductionCalendar.months_eV_Id).Where(x => x.type_eV_Id == modelOfferProduction.LProductionCalendar.type_eV_Id).ToList();

                        if (modelOfferProduction.LNProductionCalendar == null)
                        {
                            BaseOutput alpc = srv.WS_AddProductionCalendar(baseInput, modelOfferProduction.LProductionCalendar, out modelOfferProduction.LProductionCalendar);
                        }
                    }
                }

                UpdateDP(modelOfferProduction.OfferProduction.Id);

                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                if (model.enumCat != null)
                {
                    for (long ecv = 0; ecv < model.enumCat.Length; ecv++)
                    {
                        if (model.pcId != null)
                        {
                            BaseOutput gpcbi = srv.WS_GetProductionControlById(baseInput, model.pcId[ecv], true, out modelOfferProduction.ProductionControl);

                            modelOfferProduction.ProductionControl.Offer_Production_Id = modelOfferProduction.OfferProduction.Id;
                            modelOfferProduction.ProductionControl.Offer_Production_IdSpecified = true;

                            modelOfferProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                            modelOfferProduction.ProductionControl.EnumCategoryIdSpecified = true;

                            modelOfferProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                            modelOfferProduction.ProductionControl.EnumValueIdSpecified = true;

                            modelOfferProduction.ProductionControl.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                            modelOfferProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                            BaseOutput ap = srv.WS_UpdateProductionControl(baseInput, modelOfferProduction.ProductionControl, out modelOfferProduction.ProductionControl);
                        }
                        else
                        {
                            BaseOutput pcb = srv.WS_GetProductionControls(baseInput, out modelOfferProduction.ProductionControlArray);
                            modelOfferProduction.ProductionControlList = modelOfferProduction.ProductionControlArray.Where(x => x.Offer_Production_Id == modelOfferProduction.OfferProduction.Id).ToList();
                            if (ecv == 0)
                            {
                                foreach (tblProductionControl itm in modelOfferProduction.ProductionControlList)
                                {
                                    BaseOutput dcb = srv.WS_DeleteProductionControl(baseInput, itm);
                                }
                            }

                            modelOfferProduction.ProductionControl = new tblProductionControl();

                            modelOfferProduction.ProductionControl.Offer_Production_Id = modelOfferProduction.OfferProduction.Id;
                            modelOfferProduction.ProductionControl.Offer_Production_IdSpecified = true;

                            modelOfferProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                            modelOfferProduction.ProductionControl.EnumCategoryIdSpecified = true;

                            modelOfferProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                            modelOfferProduction.ProductionControl.EnumValueIdSpecified = true;

                            modelOfferProduction.ProductionControl.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                            modelOfferProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                            BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelOfferProduction.ProductionControl, out modelOfferProduction.ProductionControl);
                        }

                    }
                }

                Session["documentGrupId"] = null;
                TempData["Success"] = modelOfferProduction.messageSuccess;

                return RedirectToAction("Index", "OfferProduction");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public static int GetQuarter(int month)
        {
            try
            {

                if (month >= 4 && month <= 6)
                    return 2;
                else if (month >= 7 && month <= 9)
                    return 3;
                else if (month >= 10 && month <= 12)
                    return 4;
                else
                    return 1;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public ActionResult ChooseFileTemplate(int pId)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelOfferProduction.EnumCategory);

                if (modelOfferProduction.EnumCategory == null)
                    modelOfferProduction.EnumCategory = new tblEnumCategory();

                BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelOfferProduction.ProductCatalogControlArray);

                modelOfferProduction.ProductCatalogControlDocumentTypeList = modelOfferProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId == modelOfferProduction.EnumCategory.Id).ToList();


                BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
                modelOfferProduction.EnumValueDocumentTypeList = modelOfferProduction.EnumValueArray.ToList();


                string grup_Id = Session["documentGrupId"].ToString(); ;
                bool flag = true;
                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                BaseOutput tfs = srv.WS_GetDocumentSizebyGroupId(grup_Id, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.totalSize, out flag);

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public void File(IList<HttpPostedFileBase> file, int documentType)
        {
            try
            {

                if (file != null)
                {
                    string documentGrupId = Session["documentGrupId"].ToString();

                    baseInput = new BaseInput();
                    modelOfferProduction = new OfferProductionViewModel();

                    long? userId = null;
                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        FormsIdentity identity = (FormsIdentity)User.Identity;
                        if (identity.Ticket.UserData.Length > 0)
                        {
                            userId = Int32.Parse(identity.Ticket.UserData);
                        }
                    }
                    BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                    baseInput.userName = modelOfferProduction.User.Username;


                    String sDate = DateTime.Now.ToString();
                    DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                    String dy = datevalue.Day.ToString();
                    String mn = datevalue.Month.ToString();
                    String yy = datevalue.Year.ToString();

                    string path = modelOfferProduction.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }



                    foreach (var attachfile in file)
                    {
                        //var dec=(attachfile.ContentLength / 1024);
                        //var decd = (decimal)731 / 1024;
                        //var fl = Math.Round(dec,2);

                        string fre = FileExtension.GetMimeType(attachfile.InputStream, attachfile.FileName);

                        if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelOfferProduction.fileSize && modelOfferProduction.fileTypes.Contains(fre))
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


                            modelOfferProduction.ProductionDocument = new tblProduction_Document();
                            BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                            modelOfferProduction.ProductionDocument.grup_Id = documentGrupId;
                            modelOfferProduction.ProductionDocument.documentUrl = path;
                            modelOfferProduction.ProductionDocument.documentName = fileName;
                            modelOfferProduction.ProductionDocument.documentRealName = ofileName;

                            modelOfferProduction.ProductionDocument.document_type_ev_Id = documentType.ToString();
                            modelOfferProduction.ProductionDocument.documentSize = attachfile.ContentLength;
                            modelOfferProduction.ProductionDocument.documentSizeSpecified = true;

                            modelOfferProduction.ProductionDocument.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                            modelOfferProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                            BaseOutput apd = srv.WS_AddProductionDocument(baseInput, modelOfferProduction.ProductionDocument, out modelOfferProduction.ProductionDocument);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public ActionResult SelectedDocuments()
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                string grup_Id = Session["documentGrupId"].ToString();

                BaseOutput gpbu = srv.WS_GetProductionDocuments(baseInput, out modelOfferProduction.ProductionDocumentArray);

                modelOfferProduction.ProductionDocumentList = modelOfferProduction.ProductionDocumentArray.Where(x => x.grup_Id == grup_Id).ToList();

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductionCalendar(long dId)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput gpca = srv.WS_GetProductionCalendarOfferId(baseInput, dId, true, out modelOfferProduction.LProductionCalendarDetailArray);

                modelOfferProduction.LProductionCalendarDetailList = new List<ProductionCalendarDetail>();

                if (modelOfferProduction.LProductionCalendarDetailArray != null)
                {
                    modelOfferProduction.LProductionCalendarDetailList = modelOfferProduction.LProductionCalendarDetailArray.ToList();
                }

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public void DeleteProductionCalendar(long id)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput gpc = srv.WS_GetProductionCalendarById(baseInput, id, true, out modelOfferProduction.LProductionCalendar);
                BaseOutput dpc = srv.WS_DeleteProductionCalendar(baseInput, modelOfferProduction.LProductionCalendar);

                UpdateDP((long)modelOfferProduction.LProductionCalendar.offer_Id);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public void UpdateDP(long id)
        {
            try
            {

                baseInput = new BaseInput();

                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput gpp = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferProduction.OfferProduction);


                decimal tp = 0;
                decimal qt = 0;

                BaseOutput gpcfd = srv.WS_GetProductionCalendarOfferId(baseInput, modelOfferProduction.OfferProduction.Id, true, out modelOfferProduction.LProductionCalendarDetailArray);

                modelOfferProduction.LProductionCalendarDetailList = modelOfferProduction.LProductionCalendarDetailArray.ToList();

                foreach (var item in modelOfferProduction.LProductionCalendarDetailList)
                {
                    if (item.transportation_eV_Id == 0)
                    {
                        item.transportation_eV_Id = 1;
                    }

                    qt = qt + ((decimal)item.quantity * (long)item.transportation_eV_Id);
                    tp = tp + ((decimal)item.price * (long)item.transportation_eV_Id * item.quantity);
                }

                modelOfferProduction.OfferProduction.total_price = tp;
                modelOfferProduction.OfferProduction.total_priceSpecified = true;

                modelOfferProduction.OfferProduction.unit_price = tp;
                modelOfferProduction.OfferProduction.unit_priceSpecified = true;

                modelOfferProduction.OfferProduction.quantity = qt;
                modelOfferProduction.OfferProduction.quantitySpecified = true;

                modelOfferProduction.OfferProduction.isNew = 1;
                modelOfferProduction.OfferProduction.isNewSpecified = true;

                BaseOutput appfc = srv.WS_UpdateOffer_Production(baseInput, modelOfferProduction.OfferProduction, out modelOfferProduction.OfferProduction);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public void DeleteSelectedDocument(long id)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;


                BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput, id, true, out modelOfferProduction.ProductionDocument);

                BaseOutput dpd = srv.WS_DeleteProductionDocument(baseInput, modelOfferProduction.ProductionDocument);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public void DeleteSelectedOfferProduct(long id)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
                baseInput.userName = modelOfferProduction.User.Username;

                BaseOutput gpd = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferProduction.OfferProduction);

                BaseOutput dpd = srv.WS_DeleteOffer_Production(baseInput, modelOfferProduction.OfferProduction);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }
    }
}
