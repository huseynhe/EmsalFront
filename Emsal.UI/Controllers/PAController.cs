using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using Emsal.UI.Infrastructure;
using Emsal.Utility.CustomObjects;
using System.IO;
using WordDoc.Models;
using Microsoft.Win32;
using WebBarCodec.Core;
using System.Net.Mail;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.OfferMonitoring)]

    public class PAController : Controller
    {
        private BaseInput baseInput;
        
        private static long sproductId;
        private static long suserType;
        private static string sfinVoen;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private OfferMonitoringViewModel modelOfferMonitoring;


        public ActionResult TotalDemandOffersGroup(int? page, long productId = -1, long userType = -1, string finVoen = null)
        {
            try
            {
                int pageSize = 36;
                int pageNumber = (page ?? 1);

                if (productId == -1 && userType == -1 && finVoen == null)
                {
                    sproductId = 0;
                    suserType = 0;
                    sfinVoen = null;
                }

                if (productId >= 0)
                    sproductId = productId;
                if (userType >= 0)
                    suserType = userType;
                if (finVoen != null)
                    sfinVoen = finVoen;

                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;


                modelOfferMonitoring.DemandOfferProductsSearch = new DemandOfferProductsSearch();

                modelOfferMonitoring.DemandOfferProductsSearch.productId = sproductId;
                modelOfferMonitoring.DemandOfferProductsSearch.productIdSpecified = true;
                modelOfferMonitoring.DemandOfferProductsSearch.roleID = suserType;
                modelOfferMonitoring.DemandOfferProductsSearch.pinNumber = sfinVoen;
                modelOfferMonitoring.DemandOfferProductsSearch.voen = sfinVoen;

                BaseOutput gpp = srv.WS_GetTotalDemandOffers(baseInput, pageNumber, true, pageSize, true, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.DemanProductionGroupArray);


                if (modelOfferMonitoring.DemanProductionGroupArray == null)
                {
                    modelOfferMonitoring.DemanProductionGroupList = new List<DemanProductionGroup>();
                }
                else
                {
                    modelOfferMonitoring.DemanProductionGroupList = modelOfferMonitoring.DemanProductionGroupArray.OrderBy(x => x.productParentName).ToList();
                }

                BaseOutput gdpc = srv.WS_GetTotalDemandOffers_OPC(baseInput, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.itemCount, out modelOfferMonitoring.itemCountB);

                long[] aic = new long[modelOfferMonitoring.itemCount];

                modelOfferMonitoring.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelOfferMonitoring.productId = sproductId;
                modelOfferMonitoring.userType = suserType;
                modelOfferMonitoring.finVoen = sfinVoen;


                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialTotalDemandOffersGroup", modelOfferMonitoring)
                   : View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult TotalDemandOffersGroupDetail(int? page, long productId = -1)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productId == -1)
                {
                    sproductId = 0;
                }

                if (productId >= 0)
                    sproductId = productId;

                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                modelOfferMonitoring.DemandOfferProductsSearch = new DemandOfferProductsSearch();

                modelOfferMonitoring.DemandOfferProductsSearch.productId = sproductId;
                modelOfferMonitoring.DemandOfferProductsSearch.productIdSpecified = true;

                BaseOutput gpp = srv.WS_GetTotalDemandOffers(baseInput, pageNumber, true, pageSize, true, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.DemanProductionGroupArray);


                if (modelOfferMonitoring.DemanProductionGroupArray == null)
                {
                    modelOfferMonitoring.DemanProductionGroup = new DemanProductionGroup();
                }
                else
                {
                    modelOfferMonitoring.DemanProductionGroup = modelOfferMonitoring.DemanProductionGroupArray.FirstOrDefault();
                }
                modelOfferMonitoring.itemCount = modelOfferMonitoring.DemanProductionGroup.offerProductsList.Count();


                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialTotalDemandOffersGroupDetail", modelOfferMonitoring)
                   : View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult ProductCatalogForSale(string actionName)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelOfferMonitoring.ProductCatalogDetailArray);

                if (modelOfferMonitoring.ProductCatalogDetailArray == null)
                {
                    modelOfferMonitoring.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelOfferMonitoring.ProductCatalogDetailList = modelOfferMonitoring.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelOfferMonitoring.actionName = actionName;
                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
