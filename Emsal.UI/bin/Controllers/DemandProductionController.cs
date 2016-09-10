using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static string fullAddressId = "";
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private DemandProductionViewModel modelDemandProduction;

        public ActionResult Index()
        {
            try
            {

                Session["arrDNum"] = null;
                string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;


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

                modelDemandProduction.fullAddressId = fullAddressId;
                if (Session["documentGrupId"] == null)
                {
                    Guid dg = Guid.NewGuid();
                    Session["documentGrupId"] = dg;
                    this.Session.Timeout = 20;
                }

                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductCatalog(long pId = 0, long ppId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, (int)pId, true, out modelDemandProduction.ProductCatalogArray);
                modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.ToList();

                modelDemandProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                foreach (tblProductCatalog itm in modelDemandProduction.ProductCatalogList)
                {
                    BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelDemandProduction.ProductCatalogArrayPC);
                    if (modelDemandProduction.ProductCatalogArrayPC.ToList().Count == 0)
                    {
                        if (itm.canBeOrder == 1)
                            modelDemandProduction.ProductCatalogListPC.Add(itm);
                    }
                    else
                    {
                        modelDemandProduction.ProductCatalogListPC.Add(itm);
                    }
                }
                modelDemandProduction.ProductCatalogList = null;
                modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogListPC;



                BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelDemandProduction.EnumCategoryArray);
                modelDemandProduction.EnumCategoryList = modelDemandProduction.EnumCategoryArray.ToList();

                BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelDemandProduction.EnumValueArray);
                modelDemandProduction.EnumValueList = modelDemandProduction.EnumValueArray.ToList();

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelDemandProduction.EnumCategory);
                BaseOutput enumcatm = srv.WS_GetEnumCategorysByName(baseInput, "Markalar", out modelDemandProduction.EnumCategoryM);

                BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelDemandProduction.ProductCatalogControlArray);

                modelDemandProduction.ProductCatalogControlList = modelDemandProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategory.Id).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategoryM.Id).ToList();

                if (Session["arrDPNum"] == null)
                {
                    Session["arrDPNum"] = modelDemandProduction.arrPNum;
                }
                else
                {
                    modelDemandProduction.arrPNum = (int)Session["arrDPNum"] + 1;
                    Session["arrDPNum"] = modelDemandProduction.arrPNum;
                }


                //if (modelDemandProduction.ProductCatalogList.Count() == 0 && ppId > 0)
                if (ppId > 0)
                {
                    BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, ppId, true, out modelDemandProduction.DemandProduction);

                    modelDemandProduction.Id = modelDemandProduction.DemandProduction.Id;
                    modelDemandProduction.productId = (int)modelDemandProduction.DemandProduction.product_Id;
                    modelDemandProduction.description = modelDemandProduction.DemandProduction.description;
                    //modelDemandProduction.size = (modelDemandProduction.DemandProduction.quantity.ToString()).Replace(',', '.');
                    //modelDemandProduction.price = (modelDemandProduction.DemandProduction.unit_price.ToString()).Replace(',', '.');
                    modelDemandProduction.DemandProduction.fullProductId = "0," + modelDemandProduction.DemandProduction.fullProductId;
                    modelDemandProduction.productIds = modelDemandProduction.DemandProduction.fullProductId.Split(',').Select(long.Parse).ToArray();

                    modelDemandProduction.ProductCatalogListFEA = new IList<tblProductCatalog>[modelDemandProduction.productIds.Count()];
                    int s = 0;
                    foreach (long itm in modelDemandProduction.productIds)
                    {
                        BaseOutput gpc = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm, true, out modelDemandProduction.ProductCatalogArray);

                        modelDemandProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                        foreach (tblProductCatalog itmpc in modelDemandProduction.ProductCatalogArray.ToList())
                        {
                            BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itmpc.Id, true, out modelDemandProduction.ProductCatalogArrayPC);
                            if (modelDemandProduction.ProductCatalogArrayPC.ToList().Count == 0)
                            {
                                if (itmpc.canBeOrder == 1)
                                    modelDemandProduction.ProductCatalogListPC.Add(itmpc);
                            }
                            else
                            {
                                modelDemandProduction.ProductCatalogListPC.Add(itmpc);
                            }
                        }
                        modelDemandProduction.ProductCatalogListFEA[s] = modelDemandProduction.ProductCatalogListPC;

                        //modelDemandProduction.ProductCatalogListFEA[s] = modelDemandProduction.ProductCatalogArray.ToList();
                        s = s + 1;
                    }

                    BaseOutput pcln = srv.WS_GetProductCatalogControlsByProductID(baseInput, modelDemandProduction.productIds.LastOrDefault(), true, out modelDemandProduction.ProductCatalogControlArray);

                    modelDemandProduction.ProductCatalogControlList = modelDemandProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategory.Id).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategoryM.Id).ToList();

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
                modelDemandProduction = new DemandProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, (int)pId, true, out modelDemandProduction.ProductCatalogArray);
                modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.ToList();

                modelDemandProduction.ProductCatalogListPC = new List<tblProductCatalog>();
                foreach (tblProductCatalog itm in modelDemandProduction.ProductCatalogList)
                {
                    BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelDemandProduction.ProductCatalogArrayPC);
                    if (modelDemandProduction.ProductCatalogArrayPC.ToList().Count == 0)
                    {
                        if (itm.canBeOrder == 1)
                            modelDemandProduction.ProductCatalogListPC.Add(itm);
                    }
                    else
                    {
                        modelDemandProduction.ProductCatalogListPC.Add(itm);
                    }
                }
                modelDemandProduction.ProductCatalogList = null;
                modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogListPC;

                BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelDemandProduction.EnumCategoryArray);
                modelDemandProduction.EnumCategoryList = modelDemandProduction.EnumCategoryArray.ToList();

                BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelDemandProduction.EnumValueArray);
                modelDemandProduction.EnumValueList = modelDemandProduction.EnumValueArray.ToList();

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelDemandProduction.EnumCategory);

                BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelDemandProduction.ProductCatalogControlArray);
                modelDemandProduction.ProductCatalogControlList = modelDemandProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelDemandProduction.EnumCategory.Id).ToList();

                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Getmonth(string startDateYear, string startDateMonth, string endDateYear, string endDateMonth, long id = 0)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();


                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelDemandProduction.EnumCategory);
                if (modelDemandProduction.EnumCategory == null)
                    modelDemandProduction.EnumCategory = new tblEnumCategory();

                BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategory.Id, true, out modelDemandProduction.EnumValueArray);
                modelDemandProduction.EnumValueMonthList = modelDemandProduction.EnumValueArray.ToList();

                modelDemandProduction.EnumValueMonthListFV = new List<tblEnumValue>();


                if (startDateYear == endDateYear)
                {
                    modelDemandProduction.EnumValueMonthList = modelDemandProduction.EnumValueMonthList.Where(x => x.Id >= long.Parse(startDateMonth)).ToList();

                    modelDemandProduction.EnumValueMonthList = modelDemandProduction.EnumValueMonthList.Where(x => x.Id <= long.Parse(endDateMonth)).ToList();

                    foreach (var item in modelDemandProduction.EnumValueMonthList)
                    {
                        item.description = item.description + " (" + endDateYear + ")";
                        item.Status = long.Parse(GetQuarter(DateTime.ParseExact(item.name, "MMMM", CultureInfo.InvariantCulture).Month).ToString() + endDateYear);
                        modelDemandProduction.EnumValueMonthListFV.Add(item);
                    }
                }
                else
                {
                    //if ((Int32.Parse(endDateYear) - Int32.Parse(startDateYear)) == 1)
                    //{
                    modelDemandProduction.EnumValueMonthListFVE = modelDemandProduction.EnumValueMonthList.Where(x => x.Id >= long.Parse(startDateMonth)).ToList();

                    foreach (var item in modelDemandProduction.EnumValueMonthListFVE)
                    {
                        tblEnumValue envb = new tblEnumValue();

                        envb.Id = item.Id;
                        envb.name = item.name;
                        envb.description = item.description + " (" + startDateYear + ")";
                        envb.Status = long.Parse(GetQuarter(DateTime.ParseExact(item.name, "MMMM", CultureInfo.InvariantCulture).Month).ToString() + startDateYear);
                        modelDemandProduction.EnumValueMonthListFV.Add(envb);
                    }

                    modelDemandProduction.EnumValueMonthListFVE = modelDemandProduction.EnumValueMonthList.Where(x => x.Id <= long.Parse(endDateMonth)).ToList();

                    foreach (var item in modelDemandProduction.EnumValueMonthListFVE)
                    {
                        item.description = item.description + " (" + endDateYear + ")";
                        item.Status = long.Parse(GetQuarter(DateTime.ParseExact(item.name, "MMMM", CultureInfo.InvariantCulture).Month).ToString() + endDateYear);
                        modelDemandProduction.EnumValueMonthListFV.Add(item);
                    }

                    //}
                }

                modelDemandProduction.EnumValueMonthList = modelDemandProduction.EnumValueMonthListFV;

                //BaseOutput gal = srv.WS_GetProductionCalendarDemandID(baseInput, id, true, out modelDemandProduction.LProductionCalendarArray);
                //modelDemandProduction.LProductionCalendarList = modelDemandProduction.LProductionCalendarArray.ToList();

                return View(modelDemandProduction);

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

        public ActionResult AdminUnit(long pId = 0, long productAddressId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;


                BaseOutput bouput = srv.WS_GetPRM_AdminUnits(baseInput, out modelDemandProduction.PRMAdminUnitArray);
                modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

                //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelDemandProduction.PRMAdminUnitArray);
                //modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();

                modelDemandProduction.productAddressIds = null;
                if (productAddressId > 0)
                {
                    BaseOutput gpa = srv.WS_GetProductAddressById(baseInput, productAddressId, true, out modelDemandProduction.ProductAddress);

                    BaseOutput gfol = srv.WS_GetForeign_OrganizationsListForID(baseInput, (long)modelDemandProduction.ProductAddress.forgId, true, out modelDemandProduction.ForeignOrganizationArray);

                    modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.ToList();

                    if (modelDemandProduction.ProductAddress.fullAddressId != "")
                    {
                        modelDemandProduction.ProductAddress.fullAddressId = "0," + string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.Id));
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

                if (fullAddressId != "")
                {
                    fullAddressId = "0," + fullAddressId;
                    modelDemandProduction.productAddressIds = fullAddressId.Split(',').Select(long.Parse).ToArray();

                    modelDemandProduction.PRMAdminUnitArrayFA = new IList<tblPRM_AdminUnit>[modelDemandProduction.productAddressIds.Count()];
                    int s = 0;
                    foreach (long itm in modelDemandProduction.productAddressIds)
                    {
                        BaseOutput gpc = srv.WS_GetAdminUnitsByParentId(baseInput, (int)itm, true, out modelDemandProduction.PRMAdminUnitArray);
                        modelDemandProduction.PRMAdminUnitArrayFA[s] = modelDemandProduction.PRMAdminUnitArray.ToList();
                        s = s + 1;
                    }
                }

                fullAddressId = "";


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
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ForeignOrganization(long pId = 0, long productAddressId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                //BaseOutput folbidp = srv.WS_GetForeign_OrganisationsByParentId(baseInput, pId, true, out modelDemandProduction.ForeignOrganizationArray);

                BaseOutput folbid = srv.WS_GetForeignOrganizationListByUserId(baseInput, (long)userId, true, out modelDemandProduction.ForeignOrganizationArray);

                if (modelDemandProduction.ForeignOrganizationArray == null)
                {
                    modelDemandProduction.ForeignOrganizationList = new List<tblForeign_Organization>();
                    modelDemandProduction.ForeignOrganizationArray = modelDemandProduction.ForeignOrganizationList.ToArray();
                }

                modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.Where(x => x.parent_Id == pId).ToList();



                modelDemandProduction.productAddressIds = null;
                if (productAddressId > 0)
                {
                    BaseOutput gpa = srv.WS_GetProductAddressById(baseInput, productAddressId, true, out modelDemandProduction.ProductAddress);

                    BaseOutput gfol = srv.WS_GetForeign_OrganizationsListForID(baseInput, (long)modelDemandProduction.ProductAddress.forgId, true, out modelDemandProduction.ForeignOrganizationArray);

                    modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.ToList();

                    if (modelDemandProduction.ProductAddress.fullAddressId != "")
                    {
                        modelDemandProduction.ProductAddress.fullAddressId = "0," + string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.Id));
                        modelDemandProduction.productAddressIds = modelDemandProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();

                        modelDemandProduction.ForeignOrganizationArrayFA = new IList<tblForeign_Organization>[modelDemandProduction.productAddressIds.Count()];
                        int s = 0;
                        foreach (long itm in modelDemandProduction.productAddressIds)
                        {
                            //BaseOutput gfolbid = srv.WS_GetForeign_OrganisationsByParentId(baseInput, (int)itm, true, out modelDemandProduction.ForeignOrganizationArray);
                            BaseOutput gfolbid = srv.WS_GetForeignOrganizationListByUserId(baseInput, (long)userId, true, out modelDemandProduction.ForeignOrganizationArray);

                            modelDemandProduction.ForeignOrganizationArray = modelDemandProduction.ForeignOrganizationArray.Where(x => x.parent_Id == itm).ToArray();

                            modelDemandProduction.ForeignOrganizationArrayFA[s] = modelDemandProduction.ForeignOrganizationArray.ToList();
                            s = s + 1;
                        }

                        if (modelDemandProduction.ForeignOrganizationArrayFA[s - 1].Count() > 0)
                        {
                            modelDemandProduction.ProductAddress.fullAddressId = modelDemandProduction.ProductAddress.fullAddressId + ",0";
                            modelDemandProduction.productAddressIds = modelDemandProduction.ProductAddress.fullAddressId.Split(',').Select(long.Parse).ToArray();
                        }
                    }
                }

                if (fullAddressId != "" && pId == 0 && productAddressId == 0)
                {
                    //fullAddressId = "0," + fullAddressId;
                    modelDemandProduction.productAddressIds = ("0," + fullAddressId).Split(',').Select(long.Parse).ToArray();

                    modelDemandProduction.ForeignOrganizationArrayFA = new IList<tblForeign_Organization>[modelDemandProduction.productAddressIds.Count()];
                    int s = 0;
                    foreach (long itm in modelDemandProduction.productAddressIds)
                    {
                        BaseOutput gfolbid = srv.WS_GetForeign_OrganisationsByParentId(baseInput, (int)itm, true, out modelDemandProduction.ForeignOrganizationArray);

                        modelDemandProduction.ForeignOrganizationArrayFA[s] = modelDemandProduction.ForeignOrganizationArray.ToList();
                        s = s + 1;
                    }

                    if (modelDemandProduction.ForeignOrganizationArrayFA[s - 1].Count() > 0)
                    {
                        //fullAddressId = fullAddressId + ",0";
                        modelDemandProduction.productAddressIds = ("0," + fullAddressId + ",0").Split(',').Select(long.Parse).ToArray();
                    }

                    modelDemandProduction.ProductAddress = new tblProductAddress();
                }

                //fullAddressId = "";


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
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Index(DemandProductionViewModel model, FormCollection collection, IList<HttpPostedFileBase> attachfiles)
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

                modelDemandProduction = new DemandProductionViewModel();
                modelDemandProduction.DemandProduction = new tblDemand_Production();


                BaseOutput gfolid = srv.WS_GetForeign_OrganizationsListForID(baseInput, model.addressId, true, out modelDemandProduction.ForeignOrganizationArray);

                modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.ToList();
                fullAddressId = string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.Id));


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
                    modelDemandProduction.DemandProduction.grup_Id = grupId.ToString();
                    modelDemandProduction.DemandProduction.user_Id = userId;
                    modelDemandProduction.DemandProduction.user_IdSpecified = true;
                    modelDemandProduction.DemandProduction.isSelected = true;
                    modelDemandProduction.DemandProduction.isSelectedSpecified = true;

                    BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Yayinda", out modelDemandProduction.EnumValue);
                    modelDemandProduction.DemandProduction.state_eV_Id = modelDemandProduction.EnumValue.Id;
                    modelDemandProduction.DemandProduction.state_eV_IdSpecified = true;

                    BaseOutput upp = srv.WS_UpdateDemand_ProductionForUserID(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProductionArray);


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



                }
                else
                {

                    modelDemandProduction.DemandProduction.description = model.description;
                    modelDemandProduction.DemandProduction.product_Id = model.productId;

                    BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelDemandProduction.ProductCatalogArray);
                    modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.ToList();

                    modelDemandProduction.DemandProduction.fullProductId = string.Join(",", modelDemandProduction.ProductCatalogList.Select(x => x.Id));

                    modelDemandProduction.DemandProduction.product_IdSpecified = true;

                    //modelDemandProduction.DemandProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
                    ////modelDemandProduction.DemandProduction.unit_price = Convert.ToDecimal(model.price);
                    //modelDemandProduction.DemandProduction.unit_priceSpecified = true;

                    //    decimal qt = 0;
                    //    for (int i = 0; i < model.size.Length; i++)
                    //    {
                    //        qt = qt + (Convert.ToDecimal(model.size[i].Replace('.', ',')) * model.howMany[i]);
                    //    }

                    //    modelDemandProduction.DemandProduction.quantity = qt;
                    ////modelDemandProduction.DemandProduction.quantity = Convert.ToDecimal(model.size);
                    //modelDemandProduction.DemandProduction.quantitySpecified = true;

                    modelDemandProduction.DemandProduction.isSelected = true;
                    modelDemandProduction.DemandProduction.isSelectedSpecified = true;

                    modelDemandProduction.DemandProduction.user_Id = userId;
                    modelDemandProduction.DemandProduction.user_IdSpecified = true;

                    modelDemandProduction.DemandProduction.Status = 1;
                    modelDemandProduction.DemandProduction.StatusSpecified = true;

                    BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "new", out modelDemandProduction.EnumValue);
                    modelDemandProduction.DemandProduction.state_eV_Id = modelDemandProduction.EnumValue.Id;
                    modelDemandProduction.DemandProduction.state_eV_IdSpecified = true;

                    BaseOutput evbid = srv.WS_GetEnumValueById(baseInput, long.Parse(model.startDateMonth), true, out modelDemandProduction.EnumValue);
                    int sm = DateTime.ParseExact(modelDemandProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                    model.startDate = DateTime.Parse(model.startDateYear + "-" + sm.ToString() + "-01");
                    DateTime startDate = (DateTime)model.startDate;
                    modelDemandProduction.DemandProduction.startDate = startDate.Ticks;
                    modelDemandProduction.DemandProduction.startDateSpecified = true;

                    BaseOutput evbide = srv.WS_GetEnumValueById(baseInput, long.Parse(model.endDateMonth), true, out modelDemandProduction.EnumValue);
                    int em = DateTime.ParseExact(modelDemandProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                    model.endDate = DateTime.Parse(model.endDateYear + "-" + em.ToString() + "-01");
                    DateTime endDate = (DateTime)model.endDate;
                    modelDemandProduction.DemandProduction.endDate = endDate.Ticks;
                    modelDemandProduction.DemandProduction.endDateSpecified = true;

                    modelDemandProduction.ProductAddress = new tblProductAddress();

                    BaseOutput gfobid = srv.WS_GetForeign_OrganizationById(baseInput, model.addressId, true, out modelDemandProduction.ForeignOrganization);
                    BaseOutput gaid = srv.WS_GetAddressById(baseInput, (long)modelDemandProduction.ForeignOrganization.address_Id, true, out modelDemandProduction.UnitAddress);

                    modelDemandProduction.ProductAddress.adminUnit_Id = modelDemandProduction.UnitAddress.adminUnit_Id;
                    modelDemandProduction.ProductAddress.adminUnit_IdSpecified = true;

                    BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, (long)modelDemandProduction.UnitAddress.adminUnit_Id, true, out modelDemandProduction.PRMAdminUnitArray);
                    modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();
                    modelDemandProduction.ProductAddress.fullAddressId = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Id));
                    modelDemandProduction.ProductAddress.fullAddress = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Name));
                    modelDemandProduction.ProductAddress.addressDesc = model.descAddress;


                    BaseOutput gfol = srv.WS_GetForeign_OrganizationsListForID(baseInput, model.addressId, true, out modelDemandProduction.ForeignOrganizationArray);
                    modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.ToList();

                    modelDemandProduction.ProductAddress.forgId = model.addressId;
                    modelDemandProduction.ProductAddress.forgIdSpecified = true;
                    //modelDemandProduction.ProductAddress.fullAddressId = string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.Id));
                    modelDemandProduction.ProductAddress.fullForeignOrganization = string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.name));

                    BaseOutput apa = srv.WS_AddProductAddress(baseInput, modelDemandProduction.ProductAddress, out modelDemandProduction.ProductAddress);

                    modelDemandProduction.DemandProduction.address_Id = modelDemandProduction.ProductAddress.Id;
                    modelDemandProduction.DemandProduction.address_IdSpecified = true;


                    BaseOutput app = srv.WS_AddDemand_Production(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProduction);


                    if (model.size != null)
                    {
                        for (int i = 0; i < model.size.Length; i++)
                        {
                            BaseOutput envalpc = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                            modelDemandProduction.LProductionCalendar = new tblProductionCalendar();

                            modelDemandProduction.LProductionCalendar.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                            modelDemandProduction.LProductionCalendar.Production_type_eV_IdSpecified = true;

                            modelDemandProduction.LProductionCalendar.Production_Id = modelDemandProduction.DemandProduction.Id;
                            modelDemandProduction.LProductionCalendar.Production_IdSpecified = true;

                            modelDemandProduction.LProductionCalendar.year = model.year[i];
                            modelDemandProduction.LProductionCalendar.yearSpecified = true;

                            if (model.day != null)
                            {
                                modelDemandProduction.LProductionCalendar.day = model.day[i];
                                modelDemandProduction.LProductionCalendar.daySpecified = true;
                            }

                            BaseOutput evbidq = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelDemandProduction.EnumValue);
                            modelDemandProduction.LProductionCalendar.partOfyear = GetQuarter(DateTime.ParseExact(modelDemandProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month);
                            modelDemandProduction.LProductionCalendar.partOfyearSpecified = true;

                            BaseOutput envalgmbid = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelDemandProduction.EnumValue);
                            modelDemandProduction.LProductionCalendar.months_eV_Id = modelDemandProduction.EnumValue.Id;
                            modelDemandProduction.LProductionCalendar.months_eV_IdSpecified = true;

                            modelDemandProduction.LProductionCalendar.oclock = model.hour[i];
                            modelDemandProduction.LProductionCalendar.oclockSpecified = true;

                            modelDemandProduction.LProductionCalendar.transportation_eV_Id = model.howMany[i];
                            modelDemandProduction.LProductionCalendar.transportation_eV_IdSpecified = true;

                            if (model.price != null)
                            {
                                modelDemandProduction.LProductionCalendar.price = Convert.ToDecimal(model.price[i].Replace('.', ','));
                                modelDemandProduction.LProductionCalendar.priceSpecified = true;
                            }

                            modelDemandProduction.LProductionCalendar.quantity = Convert.ToDecimal(model.size[i].Replace('.', ','));
                            modelDemandProduction.LProductionCalendar.quantitySpecified = true;

                            modelDemandProduction.LProductionCalendar.demand_Id = modelDemandProduction.DemandProduction.Id;
                            modelDemandProduction.LProductionCalendar.demand_IdSpecified = true;

                            modelDemandProduction.LProductionCalendar.type_eV_Id = model.shippingSchedule;
                            modelDemandProduction.LProductionCalendar.type_eV_IdSpecified = true;

                            BaseOutput gpcall = srv.WS_GetProductionCalendar(baseInput, out modelDemandProduction.LProductionCalendarArray);

                            modelDemandProduction.LProductionCalendarList = modelDemandProduction.LProductionCalendarArray.ToList();

                            modelDemandProduction.LProductionCalendarList = modelDemandProduction.LProductionCalendarList.Where(x => x.demand_Id == modelDemandProduction.LProductionCalendar.demand_Id).Where(x => x.Production_type_eV_Id == modelDemandProduction.LProductionCalendar.Production_type_eV_Id).Where(x => x.year == modelDemandProduction.LProductionCalendar.year).Where(x => x.months_eV_Id == modelDemandProduction.LProductionCalendar.months_eV_Id).Where(x => x.type_eV_Id == modelDemandProduction.LProductionCalendar.type_eV_Id).ToList();

                            if (modelDemandProduction.LProductionCalendarList.Count() == 0)
                            {
                                BaseOutput alpc = srv.WS_AddProductionCalendar(baseInput, modelDemandProduction.LProductionCalendar, out modelDemandProduction.LProductionCalendar);
                            }
                        }
                    }

                    UpdateDP(modelDemandProduction.DemandProduction.Id);

                    BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                    if (model.enumCat != null)
                    {
                        for (int ecv = 0; ecv < model.enumCat.Length; ecv++)
                        {
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

                    //modelDemandProduction.ProductionDocument = new tblProduction_Document();
                    //modelDemandProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
                    //modelDemandProduction.ProductionDocument.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
                    //modelDemandProduction.ProductionDocument.Demand_Production_IdSpecified = true;
                    //modelDemandProduction.ProductionDocument.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                    //modelDemandProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                    //BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelDemandProduction.ProductionDocument, out modelDemandProduction.ProductionDocumentArray);

                }

                Session["documentGrupId"] = null;
                TempData["Success"] = modelDemandProduction.messageSuccess;

                return RedirectToAction("Index", "DemandProduction");

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
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                modelDemandProduction.DemandProduction = new tblDemand_Production();

                BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);

                if (modelDemandProduction.DemandProduction == null)
                {
                    return RedirectToAction("Index", "DemandProduction");
                }

                //if (modelDemandProduction.DemandProduction.grup_Id != null)
                //{
                //    return RedirectToAction("Index", "DemandProduction");
                //}

                modelDemandProduction.Id = modelDemandProduction.DemandProduction.Id;
                modelDemandProduction.productId = (int)modelDemandProduction.DemandProduction.product_Id;
                modelDemandProduction.description = modelDemandProduction.DemandProduction.description;
                modelDemandProduction.productAddressId = (long)modelDemandProduction.DemandProduction.address_Id;
                //modelDemandProduction.size = (modelDemandProduction.DemandProduction.quantity.ToString()).Replace(',', '.');
                //modelDemandProduction.price = (modelDemandProduction.DemandProduction.unit_price.ToString()).Replace(',', '.');
                DateTime startDate = new DateTime((long)modelDemandProduction.DemandProduction.startDate);
                modelDemandProduction.startDateYear = startDate.Year;
                modelDemandProduction.startDateMonth = startDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("en"));
                DateTime endDate = new DateTime((long)modelDemandProduction.DemandProduction.endDate);
                modelDemandProduction.endDateYear = endDate.Year;
                modelDemandProduction.endDateMonth = endDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("en"));

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
                BaseOutput sml = srv.WS_GetProductionCalendarByProductionId(baseInput, modelDemandProduction.DemandProduction.Id, true, modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.ProductionCalendar);

                //modelDemandProduction.ProductionCalendar = modelDemandProduction.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == modelDemandProduction.EnumValue.Id).FirstOrDefault() ;

                //string[] months = modelDemandProduction.ProductionCalendar.Months.Split(',');
                //modelDemandProduction.selectedMonth = months;

                //modelDemandProduction.shippingSchedule = (long)modelDemandProduction.ProductionCalendar.Transportation_eV_Id;
                //modelDemandProduction.productionCalendarId = modelDemandProduction.ProductionCalendar.Id;

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
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(DemandProductionViewModel model, FormCollection collection)
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


                modelDemandProduction = new DemandProductionViewModel();

                BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, model.Id, true, out modelDemandProduction.DemandProduction);

                modelDemandProduction.DemandProduction.description = model.description;
                modelDemandProduction.DemandProduction.product_Id = model.productId;
                modelDemandProduction.DemandProduction.product_IdSpecified = true;


                //modelDemandProduction.DemandProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
                //modelDemandProduction.DemandProduction.unit_priceSpecified = true;

                //modelDemandProduction.DemandProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));


                BaseOutput evbid = srv.WS_GetEnumValueById(baseInput, long.Parse(model.startDateMonth), true, out modelDemandProduction.EnumValue);
                int sm = DateTime.ParseExact(modelDemandProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                model.startDate = DateTime.Parse(model.startDateYear + "-" + sm.ToString() + "-01");
                DateTime startDate = (DateTime)model.startDate;
                modelDemandProduction.DemandProduction.startDate = startDate.Ticks;
                modelDemandProduction.DemandProduction.startDateSpecified = true;

                BaseOutput evbide = srv.WS_GetEnumValueById(baseInput, long.Parse(model.endDateMonth), true, out modelDemandProduction.EnumValue);
                int em = DateTime.ParseExact(modelDemandProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month;
                model.endDate = DateTime.Parse(model.endDateYear + "-" + em.ToString() + "-01");
                DateTime endDate = (DateTime)model.endDate;
                modelDemandProduction.DemandProduction.endDate = endDate.Ticks;
                modelDemandProduction.DemandProduction.endDateSpecified = true;

                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelDemandProduction.ProductCatalogArray);
                modelDemandProduction.ProductCatalogList = modelDemandProduction.ProductCatalogArray.ToList();

                modelDemandProduction.DemandProduction.fullProductId = string.Join(",", modelDemandProduction.ProductCatalogList.Select(x => x.Id));

                BaseOutput app = srv.WS_UpdateDemand_Production(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProduction);

                BaseOutput gpabi = srv.WS_GetProductAddressById(baseInput, (long)modelDemandProduction.DemandProduction.address_Id, true, out modelDemandProduction.ProductAddress);


                BaseOutput gfobid = srv.WS_GetForeign_OrganizationById(baseInput, model.addressId, true, out modelDemandProduction.ForeignOrganization);
                BaseOutput gaid = srv.WS_GetAddressById(baseInput, (long)modelDemandProduction.ForeignOrganization.address_Id, true, out modelDemandProduction.UnitAddress);

                modelDemandProduction.ProductAddress.adminUnit_Id = modelDemandProduction.UnitAddress.adminUnit_Id;
                modelDemandProduction.ProductAddress.adminUnit_IdSpecified = true;

                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, (long)modelDemandProduction.UnitAddress.adminUnit_Id, true, out modelDemandProduction.PRMAdminUnitArray);
                modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();
                modelDemandProduction.ProductAddress.fullAddressId = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Id));
                modelDemandProduction.ProductAddress.fullAddress = string.Join(",", modelDemandProduction.PRMAdminUnitList.Select(x => x.Name));
                modelDemandProduction.ProductAddress.addressDesc = model.descAddress;

                BaseOutput gfol = srv.WS_GetForeign_OrganizationsListForID(baseInput, model.addressId, true, out modelDemandProduction.ForeignOrganizationArray);
                modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.ToList();

                modelDemandProduction.ProductAddress.forgId = model.addressId;
                modelDemandProduction.ProductAddress.forgIdSpecified = true;

                //modelDemandProduction.ProductAddress.fullAddressId = string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.Id));
                //modelDemandProduction.ProductAddress.fullAddress = string.Join(",", modelDemandProduction.ForeignOrganizationList.Select(x => x.name));

                BaseOutput apa = srv.WS_UpdateProductAddress(baseInput, modelDemandProduction.ProductAddress, out modelDemandProduction.ProductAddress);

                if (model.size != null)
                {
                    for (int i = 0; i < model.size.Length; i++)
                    {
                        BaseOutput gpca = srv.WS_GetProductionCalendar(baseInput, out modelDemandProduction.LProductionCalendarArray);

                        BaseOutput envalpc = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                        modelDemandProduction.LProductionCalendar = new tblProductionCalendar();

                        modelDemandProduction.LProductionCalendar.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                        modelDemandProduction.LProductionCalendar.Production_type_eV_IdSpecified = true;

                        modelDemandProduction.LProductionCalendar.Production_Id = modelDemandProduction.DemandProduction.Id;
                        modelDemandProduction.LProductionCalendar.Production_IdSpecified = true;

                        modelDemandProduction.LProductionCalendar.year = model.year[i];
                        modelDemandProduction.LProductionCalendar.yearSpecified = true;

                        if (model.day != null)
                        {
                            modelDemandProduction.LProductionCalendar.day = model.day[i];
                            modelDemandProduction.LProductionCalendar.daySpecified = true;
                        }

                        BaseOutput evbidq = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelDemandProduction.EnumValue);
                        modelDemandProduction.LProductionCalendar.partOfyear = GetQuarter(DateTime.ParseExact(modelDemandProduction.EnumValue.name, "MMMM", CultureInfo.InvariantCulture).Month);
                        modelDemandProduction.LProductionCalendar.partOfyearSpecified = true;

                        BaseOutput envalgmbid = srv.WS_GetEnumValueById(baseInput, (long)(model.month[i]), true, out modelDemandProduction.EnumValue);
                        modelDemandProduction.LProductionCalendar.months_eV_Id = modelDemandProduction.EnumValue.Id;
                        modelDemandProduction.LProductionCalendar.months_eV_IdSpecified = true;

                        modelDemandProduction.LProductionCalendar.oclock = model.hour[i];
                        modelDemandProduction.LProductionCalendar.oclockSpecified = true;

                        modelDemandProduction.LProductionCalendar.transportation_eV_Id = model.howMany[i];
                        modelDemandProduction.LProductionCalendar.transportation_eV_IdSpecified = true;

                        if (model.price != null)
                        {
                            modelDemandProduction.LProductionCalendar.price = Convert.ToDecimal(model.price[i].Replace('.', ','));
                            modelDemandProduction.LProductionCalendar.priceSpecified = true;
                        }

                        modelDemandProduction.LProductionCalendar.quantity = Convert.ToDecimal(model.size[i].Replace('.', ','));
                        modelDemandProduction.LProductionCalendar.quantitySpecified = true;

                        modelDemandProduction.LProductionCalendar.demand_Id = modelDemandProduction.DemandProduction.Id;
                        modelDemandProduction.LProductionCalendar.demand_IdSpecified = true;

                        modelDemandProduction.LProductionCalendar.type_eV_Id = model.shippingSchedule;
                        modelDemandProduction.LProductionCalendar.type_eV_IdSpecified = true;

                        BaseOutput gpcall = srv.WS_GetProductionCalendar(baseInput, out modelDemandProduction.LProductionCalendarArray);

                        modelDemandProduction.LProductionCalendarList = modelDemandProduction.LProductionCalendarArray.ToList();

                        modelDemandProduction.LProductionCalendarList = modelDemandProduction.LProductionCalendarList.Where(x => x.demand_Id == modelDemandProduction.LProductionCalendar.demand_Id).Where(x => x.Production_type_eV_Id == modelDemandProduction.LProductionCalendar.Production_type_eV_Id).Where(x => x.year == modelDemandProduction.LProductionCalendar.year).Where(x => x.months_eV_Id == modelDemandProduction.LProductionCalendar.months_eV_Id).Where(x => x.type_eV_Id == modelDemandProduction.LProductionCalendar.type_eV_Id).ToList();

                        if (modelDemandProduction.LProductionCalendarList.Count() == 0)
                        {
                            BaseOutput alpc = srv.WS_AddProductionCalendar(baseInput, modelDemandProduction.LProductionCalendar, out modelDemandProduction.LProductionCalendar);
                        }
                    }
                }

                UpdateDP(modelDemandProduction.DemandProduction.Id);

                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);

                if (model.enumCat != null)
                {
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
                }

                //modelDemandProduction.ProductionDocument = new tblProduction_Document();
                //modelDemandProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
                //modelDemandProduction.ProductionDocument.Demand_Production_Id = modelDemandProduction.DemandProduction.Id;
                //modelDemandProduction.ProductionDocument.Demand_Production_IdSpecified = true;
                //modelDemandProduction.ProductionDocument.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                //modelDemandProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                //BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelDemandProduction.ProductionDocument, out modelDemandProduction.ProductionDocumentArray);

                Session["documentGrupId"] = null;
                TempData["Success"] = modelDemandProduction.messageSuccess;

                return RedirectToAction("Index", "DemandProduction");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public void UpdateDP(long id)
        {
            try
            {

                baseInput = new BaseInput();

                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput gpp = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);


                decimal qt = 0;

                BaseOutput gpcfd = srv.WS_GetProductionCalendarDemandId(baseInput, modelDemandProduction.DemandProduction.Id, true, out modelDemandProduction.LProductionCalendarDetailArray);
                modelDemandProduction.LProductionCalendarDetailList = modelDemandProduction.LProductionCalendarDetailArray.ToList();

                foreach (var item in modelDemandProduction.LProductionCalendarDetailList)
                {
                    if (item.transportation_eV_Id == 0)
                    {
                        item.transportation_eV_Id = 1;
                    }
                    qt = qt + ((decimal)item.quantity * (long)item.transportation_eV_Id);
                }

                modelDemandProduction.DemandProduction.quantity = qt;
                modelDemandProduction.DemandProduction.quantitySpecified = true;

                BaseOutput udp = srv.WS_UpdateDemand_Production(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProduction);


            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public ActionResult ProductionCalendar(long dId, bool pdf = false)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput gpca = srv.WS_GetProductionCalendarDemandId(baseInput, dId, true, out modelDemandProduction.LProductionCalendarDetailArray);

                modelDemandProduction.LProductionCalendarDetailList = new List<ProductionCalendarDetail>();

                if (modelDemandProduction.LProductionCalendarDetailArray != null)
                {
                    modelDemandProduction.LProductionCalendarDetailList = modelDemandProduction.LProductionCalendarDetailArray.ToList();
                }

                modelDemandProduction.isPDF = pdf;

                return View(modelDemandProduction);

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
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput gpc = srv.WS_GetProductionCalendarById(baseInput, id, true, out modelDemandProduction.LProductionCalendar);
                BaseOutput dpc = srv.WS_DeleteProductionCalendar(baseInput, modelDemandProduction.LProductionCalendar);

                UpdateDP((long)modelDemandProduction.LProductionCalendar.demand_Id);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public ActionResult SelectedProducts(bool pdf = false, bool noButton = true)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput gpd = srv.WS_GetDemandProductionDetailistForUser(baseInput, (long)userId, true, out modelDemandProduction.ProductionDetailArray);

                if (modelDemandProduction.ProductionDetailArray != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelDemandProduction.EnumCategory.Id).ToList();
                }
                else
                {
                    modelDemandProduction.ProductionDetailList = new List<ProductionDetail>();
                }

                modelDemandProduction.isPDF = pdf;
                modelDemandProduction.userId = (long)userId;
                modelDemandProduction.noButton = noButton;

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
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ChooseFileTemplate(int pId)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelDemandProduction.EnumCategory);

                if (modelDemandProduction.EnumCategory == null)
                    modelDemandProduction.EnumCategory = new tblEnumCategory();

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
                    modelDemandProduction = new DemandProductionViewModel();

                    long? userId = null;
                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        FormsIdentity identity = (FormsIdentity)User.Identity;
                        if (identity.Ticket.UserData.Length > 0)
                        {
                            userId = Int32.Parse(identity.Ticket.UserData);
                        }
                    }
                    BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                    baseInput.userName = modelDemandProduction.User.Username;

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
                            modelDemandProduction.ProductionDocument.documentRealName = ofileName;

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
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public ActionResult SelectedDocuments(long PId = 0)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                string grup_Id = Session["documentGrupId"].ToString();

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
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public void DeleteSelectedDocument(long id)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput, id, true, out modelDemandProduction.ProductionDocument);

                BaseOutput dpd = srv.WS_DeleteProductionDocument(baseInput, modelDemandProduction.ProductionDocument);


            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public void DeleteSelectedDemandProduct(long id)
        {
            try
            {

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? userId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelDemandProduction.User);
                baseInput.userName = modelDemandProduction.User.Username;

                BaseOutput gpd = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);

                BaseOutput dpd = srv.WS_DeleteDemand_Production(baseInput, modelDemandProduction.DemandProduction);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }
    }
}
