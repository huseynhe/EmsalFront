using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emsal.Utility.CustomObjects;
using System.Web.Security;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using WebBarCodec.Core;

namespace Emsal.UI.Controllers
{

    public class OfferHomeController : Controller
    {
        private BaseInput baseInput;

        private static int saddressId;
        private static string ssort;
        private static string sname;
        private static string ssurname;
        private static string sproductName;
        private static string sfv;
        private static long sproductId;


        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        //Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private ProductCatalogViewModel modelProductCatalog;
        private ContactViewModel modelContact;


        public ActionResult Index(int pId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelProductCatalog = new ProductCatalogViewModel();

                //BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, pId, true, out modelProductCatalog.ProductCatalogArray);
                //modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.ToList();

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult OfferProduction(int? page, int productId = -1, string fv = null)
        {
            try
            {
                if (fv != null)
                    fv = StripTag.strSqlBlocker(fv.ToLower());

                if (fv == "")
                    fv = null;

                if (fv == null && productId == -1)
                {
                    sfv = null;
                    sproductId = 0;
                }

                if (fv != null)
                    sfv = fv;
                if (productId >= 0)
                    sproductId = productId;

                baseInput = new BaseInput();

                int pageSize = 50;
                int pageNumber = (page ?? 1);

                modelProductCatalog = new ProductCatalogViewModel();
                modelProductCatalog.noPaged = 0;

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

                modelProductCatalog.OfferProductionDetailSearch = new OfferProductionDetailSearch();

                modelProductCatalog.OfferProductionDetailSearch.state_eV_Id = modelProductCatalog.EnumValue.Id;
                modelProductCatalog.OfferProductionDetailSearch.page = pageNumber;
                modelProductCatalog.OfferProductionDetailSearch.pageSize = pageSize;
                modelProductCatalog.OfferProductionDetailSearch.productID = sproductId;
                modelProductCatalog.OfferProductionDetailSearch.pinNumber = sfv;
                modelProductCatalog.OfferProductionDetailSearch.voen = sfv;


                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId_OP1(baseInput, modelProductCatalog.OfferProductionDetailSearch, out modelProductCatalog.GetOfferProductionDetailistForEValueIdArray);

                if (modelProductCatalog.GetOfferProductionDetailistForEValueIdArray != null)
                {
                    if (sproductId > 0 || sfv != null)
                    {
                        modelProductCatalog.noPaged = 1;
                    }

                    modelProductCatalog.GetOfferProductionDetailistForEValueIdList = modelProductCatalog.GetOfferProductionDetailistForEValueIdArray.ToList();
                }
                else
                {
                    modelProductCatalog.GetOfferProductionDetailistForEValueIdList = new List<GetOfferProductionDetailistForEValueId>();
                }

                BaseOutput gppc = srv.WS_GetOfferProductionDetailistForEValueId_OPC1(baseInput, modelProductCatalog.OfferProductionDetailSearch, out modelProductCatalog.itemCount, out modelProductCatalog.itemCountB);

                long[] aic = new long[modelProductCatalog.itemCount];

                modelProductCatalog.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelProductCatalog.fv = sfv;
                modelProductCatalog.productId = sproductId;

                return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("PartialOfferProduction", modelProductCatalog)
                    : View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductCatalog(int pId = 0)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, pId, true, out modelProductCatalog.ProductCatalogArray);
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.OrderBy(x => x.ProductName).ToList();

            modelProductCatalog.ProductionDetailList = new List<ProductionDetail>();


            modelProductCatalog.ProductCatalogListPC = new List<tblProductCatalog>();

            long itemCount = 0;
            bool itemCountB = true;
            foreach (tblProductCatalog itm in modelProductCatalog.ProductCatalogList)
            {
                BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelProductCatalog.ProductCatalogArrayPC);

                BaseOutput gpp = srv.WS_GetOffer_ProductionByProductIdandStateEVId(baseInput, (long)itm.Id, true, (long)modelProductCatalog.EnumValue.Id, true, out itemCount, out itemCountB);


                itm.ProductDescription = itemCount.ToString();

                if (modelProductCatalog.ProductCatalogArrayPC.ToList().Count == 0)
                {
                    if (itm.canBeOrder == 1 && Int32.Parse(itm.ProductDescription) > 0)
                    {
                        modelProductCatalog.ProductCatalogListPC.Add(itm);
                    }
                }
                else
                {
                    modelProductCatalog.ProductCatalogListPC.Add(itm);
                }
            }
            modelProductCatalog.ProductCatalogList = null;
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogListPC;



            //if (pId == 0)
            //{
            //    return View("PCDetail", modelProductCatalog);
            //}
            //else
            //{
            modelProductCatalog.pId = pId;
            return View(modelProductCatalog);
            //}
        }

        public ActionResult OfferProductionDetail(long id)
        {
            try
            {

                baseInput = new BaseInput();

                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

                BaseOutput gopbid = srv.WS_GetOfferProductionDetailById(baseInput, id, true, out modelProductCatalog.ProductionDetail);

                return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Signin()
        {
            try
            {

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult Signout()
        {
            try
            {

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult PotentialUser()
        {
            try
            {

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Region()
        {
            try
            {

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult District()
        {
            try
            {

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Pages()
        {
            try
            {

                return View();

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
                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput gpca = srv.WS_GetProductionCalendarOfferId(baseInput, dId, true, out modelProductCatalog.LProductionCalendarDetailArray);

                modelProductCatalog.LProductionCalendarDetailList = new List<ProductionCalendarDetail>();

                if (modelProductCatalog.LProductionCalendarDetailArray != null)
                {
                    modelProductCatalog.LProductionCalendarDetailList = modelProductCatalog.LProductionCalendarDetailArray.ToList();
                }

                return View(modelProductCatalog);

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
                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput bouput = srv.WS_GetOrderProducts(baseInput, out modelProductCatalog.OfferProductsArray);

                if (modelProductCatalog.OfferProductsArray == null)
                {
                    modelProductCatalog.OfferProductsList = new List<OfferProducts>();
                }
                else
                {
                    modelProductCatalog.OfferProductsList = modelProductCatalog.OfferProductsArray.ToList();
                }

                modelProductCatalog.actionName = actionName;
                return View(modelProductCatalog);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
