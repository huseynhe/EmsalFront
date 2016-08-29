using Emsal.AdminUI.Infrastructure;
using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class ReportController : Controller
    {
        //
        // GET: /Home/

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ReportViewModel modelReport;
        private BaseInput baseInput;

        public ActionResult Report()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }

        public ActionResult DemandOffer(int auid = 1)
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            BaseOutput dor = srv.WS_GetDemandOfferDetailID(baseInput, auid, true, out modelReport.DemandOfferDetailArray);

            if (modelReport.DemandOfferDetailArray != null)
            {
                modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailArray.ToList();
            }
            else
            {
                modelReport.DemandOfferDetailList = new List<DemandOfferDetail>();
            }
            

            int i = 0;
            string oldPr = "";
            var cnt = modelReport.DemandOfferDetailList.GroupBy(x => x.productName).Count();

            modelReport.demands = new long[cnt];
            modelReport.offers = new long[cnt];
            modelReport.products = new string[cnt];

            foreach (var item in modelReport.DemandOfferDetailList)
            {
                if (oldPr != item.productName)
                {
                    modelReport.products[i] = item.productName+item.productParentName;
                    var ss = modelReport.DemandOfferDetailList.Where(x => x.productName == item.productName).ToList();

                    foreach (var itm in ss)
                    {
                        if (itm.productType == "Demand")
                        {
                            modelReport.demands[i] = itm.count;
                        }

                        if (itm.productType == "Offer")
                        {
                            modelReport.offers[i] = itm.count;
                        }
                    }
                    i = i + 1;
                }
                oldPr = item.productName;
            }

            {
                modelReport.strDemand = String.Join(",", modelReport.demands);
                modelReport.strOffer = String.Join(",", modelReport.offers);
                modelReport.strProducts = String.Join(",", modelReport.products);
            }

            return View(modelReport);
        }

        public ActionResult AdminUnit(int pId = 0)
        {
            baseInput = new BaseInput();

            modelReport = new ReportViewModel();


            BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, pId, true, out modelReport.PRMAdminUnitArray);
            modelReport.PRMAdminUnitList = modelReport.PRMAdminUnitArray.ToList();
            modelReport.fullAddress = string.Join(",", modelReport.PRMAdminUnitList.Select(x => x.Name));


            BaseOutput bouput = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelReport.PRMAdminUnitArray);
            modelReport.PRMAdminUnitList = modelReport.PRMAdminUnitArray.ToList();


            if (modelReport.PRMAdminUnitList.Count() == 0)
            {
                return new EmptyResult();
            }
            else
            {
                return View(modelReport);
            }
        }

        public ActionResult ForeignOrganization(int pId = 0)
        {
            baseInput = new BaseInput();

            modelReport = new ReportViewModel();


            BaseOutput gfolid = srv.WS_GetForeign_OrganizationsListForID(baseInput, pId, true, out modelReport.ForeignOrganizationArray);
            modelReport.ForeignOrganizationList = modelReport.ForeignOrganizationArray.ToList();
            modelReport.fullFO = string.Join(",", modelReport.ForeignOrganizationList.Select(x => x.name));


            BaseOutput bouput = srv.WS_GetForeign_OrganisationsByParentId(baseInput, pId, true, out modelReport.ForeignOrganizationArray);
            modelReport.ForeignOrganizationList = modelReport.ForeignOrganizationArray.ToList();


            if (modelReport.ForeignOrganizationList.Count() == 0)
            {
                return new EmptyResult();
            }
            else
            {
                return View(modelReport);
            }
        }

        public ActionResult DemandOfferProduct(int prodId = 0)
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            BaseOutput dor = srv.WS_GetDemandOfferDetailByProductID(baseInput, out modelReport.DemandOfferDetailArray);
            modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailArray.ToList();

            if(prodId>0)
            {
                modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailList.Where(x => x.productID == prodId).ToList();
            }

            int i = 0;
            string oldPr = "";
            var cnt = modelReport.DemandOfferDetailList.GroupBy(x => x.adminUnittName).Count();

            modelReport.demands = new long[cnt];
            modelReport.offers = new long[cnt];
            modelReport.products = new string[cnt];

            foreach (var item in modelReport.DemandOfferDetailList)
            {
                if (oldPr != item.adminUnittName)
                {
                    modelReport.products[i] = item.adminUnittName;
                    var doda = modelReport.DemandOfferDetailList.Where(x => x.adminUnittName == item.adminUnittName).ToList();
                    //int dodg = doda.GroupBy(x => x.productType).ToList().Count();

                    //for (int p= 0; p < dodg;p++)
                    //{
                    long prodCnt = 0;

                    var demandList = doda.Where(x => x.productType == "Demand").ToList();

                    foreach(var itm in demandList)
                    {
                        prodCnt = prodCnt + itm.count;
                    }

                    modelReport.demands[i] = prodCnt;

                    var offerList = doda.Where(x => x.productType == "Offer").ToList();
                    prodCnt = 0;
                    foreach (var itm in offerList)
                    {
                        prodCnt = prodCnt + itm.count;
                    }
                    
                            modelReport.offers[i] = prodCnt;
                    //}
                    i = i + 1;
                }
                oldPr = item.adminUnittName;
            }

            {
                modelReport.strDemand = String.Join(",", modelReport.demands);
                modelReport.strOffer = String.Join(",", modelReport.offers);
                modelReport.strProducts = String.Join(",", modelReport.products);
            }

            return View(modelReport);
        }

        public ActionResult DemandOfferProductM()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }

        public ActionResult ProductCatalog(int pId = 0)
        {
            baseInput = new BaseInput();

            modelReport = new ReportViewModel();

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, pId, true, out modelReport.ProductCatalogArray);
            modelReport.ProductCatalogList= modelReport.ProductCatalogArray.ToList();


            if (modelReport.ProductCatalogList.Count() == 0)
            {
                return new EmptyResult();
            }
            else
            {
                return View(modelReport);
            }
        }



        public ActionResult PotentialClient (int prodId = 0)
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            BaseOutput dor = srv.WS_GetPotentialClientCount(baseInput, out modelReport.PotentialClientDetailArray);
            modelReport.PotentialClientDetailList = modelReport.PotentialClientDetailArray.ToList();

            if (prodId > 0)
            {
                modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailList.Where(x => x.productID == prodId).ToList();
            }

            int i = 0;
            string oldPr = "";
            var cnt = modelReport.PotentialClientDetailList.GroupBy(x => x.adminUnitName).Count();

            modelReport.demands = new long[cnt];
            modelReport.offers = new long[cnt];
            modelReport.products = new string[cnt];

            foreach (var item in modelReport.PotentialClientDetailList)
            {
                if (oldPr != item.adminUnitName)
                {
                    modelReport.products[i] = item.adminUnitName;
                    var doda = modelReport.PotentialClientDetailList.Where(x => x.adminUnitName == item.adminUnitName).ToList();
                    //int dodg = doda.GroupBy(x => x.productType).ToList().Count();

                    //for (int p= 0; p < dodg;p++)
                    //{
                    long prodCnt = 0;

                    var demandList = doda.Where(x => x.fromOrganisation == "KTN").ToList();

                    foreach (var itm in demandList)
                    {
                        prodCnt = prodCnt + itm.count;
                    }

                    modelReport.demands[i] = prodCnt;

                    var offerList = doda.Where(x => x.fromOrganisation == "Asan").ToList();
                    prodCnt = 0;
                    foreach (var itm in offerList)
                    {
                        prodCnt = prodCnt + itm.count;
                    }

                    modelReport.offers[i] = prodCnt;
                    //}
                    i = i + 1;
                }
                oldPr = item.adminUnitName;
            }

            {
                modelReport.strDemand = String.Join(",", modelReport.demands);
                modelReport.strOffer = String.Join(",", modelReport.offers);
                modelReport.strProducts = String.Join(",", modelReport.products);
            }

            return View(modelReport);
        }

        public ActionResult PotentialClientM()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }


        public ActionResult DemandOfferDonut()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }

        public ActionResult DemandOfferDonutM(int auid = 1)
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            BaseOutput dor = srv.WS_GetOfferDetailByAmdminID(baseInput, auid, true, out modelReport.DemandOfferDetailArray);

            if(modelReport.DemandOfferDetailArray!=null)
            {               
                modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailArray.ToList();
            }
            else
            {
                modelReport.DemandOfferDetailList = new List<DemandOfferDetail>();
            }

            modelReport.ReportDonutList = new  List<ReportDonut>();

           foreach(var item in modelReport.DemandOfferDetailList)
            {
                modelReport.ReportDonut = new ReportDonut();
                modelReport.ReportDonut.label = item.productName+item.productParentName;
                modelReport.ReportDonut.value = item.count;

                modelReport.ReportDonutList.Add(modelReport.ReportDonut);
            }

            modelReport.strOffer = JsonConvert.SerializeObject(modelReport.ReportDonutList).ToString() ;

            return View(modelReport);
        }


        public JsonResult OfferDonut(int auid = 1)
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            BaseOutput dor = srv.WS_GetOfferDetailByAmdminID(baseInput, auid, true, out modelReport.DemandOfferDetailArray);
            if (modelReport.DemandOfferDetailArray != null)
            {
                modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailArray.ToList();
            }
            else
            {
                modelReport.DemandOfferDetailList=new List<DemandOfferDetail>();
            }
            

            modelReport.ReportDonutList = new List<ReportDonut>();

            foreach (var item in modelReport.DemandOfferDetailList)
            {
                modelReport.ReportDonut = new ReportDonut();
                modelReport.ReportDonut.label = item.productName+item.productParentName;
                modelReport.ReportDonut.value = item.count;

                modelReport.ReportDonutList.Add(modelReport.ReportDonut);
            }

            if(modelReport.DemandOfferDetailList.Count() == 0)
            {
                modelReport.ReportDonut = new ReportDonut();
                modelReport.ReportDonut.label = "";
                modelReport.ReportDonut.value = 0;

                modelReport.ReportDonutList.Add(modelReport.ReportDonut);
            }
            return Json(modelReport.ReportDonutList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemandDonut(int auid = 1)
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            BaseOutput dor = srv.WS_GetDemanDetailByAdminID(baseInput, auid, true, out modelReport.DemandOfferDetailArray);
            modelReport.DemandOfferDetailList = modelReport.DemandOfferDetailArray.ToList();

            modelReport.ReportDonutList = new List<ReportDonut>();

            foreach (var item in modelReport.DemandOfferDetailList)
            {
                modelReport.ReportDonut = new ReportDonut();
                modelReport.ReportDonut.label = item.productName+item.productParentName;
                modelReport.ReportDonut.value = item.count;

                modelReport.ReportDonutList.Add(modelReport.ReportDonut);
            }

            if (modelReport.DemandOfferDetailList.Count()==0)
            {
                modelReport.ReportDonut = new ReportDonut();
                modelReport.ReportDonut.label = "";
                modelReport.ReportDonut.value = 0;

                modelReport.ReportDonutList.Add(modelReport.ReportDonut);
            }

            return Json(modelReport.ReportDonutList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Report2()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }

        public ActionResult Report3()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }
        public ActionResult Report4()
        {
            baseInput = new BaseInput();
            modelReport = new ReportViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelReport.Admin);
            baseInput.userName = modelReport.Admin.Username;


            return View(modelReport);
        }
    }
}
