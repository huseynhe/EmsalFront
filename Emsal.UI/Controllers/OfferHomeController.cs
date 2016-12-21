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


        public ActionResult OfferProduction(int? page, int productId = 0, string productName = null, string fv = null)
        {
            try
            {
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (fv != null)
                    fv = StripTag.strSqlBlocker(fv.ToLower());

                if (fv == "")
                    fv = null;

                if (productName == null && fv == null && productId==0)
                {
                    sproductName = null;
                    sfv = null;
                    sproductId = 0;
                }

                if (productName != null)
                    sproductName = productName;
                if (fv != null)
                    sfv = fv;
                if (productId >0)
                    sproductId = productId;

                baseInput = new BaseInput();

                int pageSize = 50;
                int pageNumber = (page ?? 1);

                modelProductCatalog = new ProductCatalogViewModel();
                modelProductCatalog.noPaged = 0;

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, (long)modelProductCatalog.EnumValue.Id, true, out modelProductCatalog.ProductionDetailArray);

                if (modelProductCatalog.ProductionDetailArray != null)
                {
                    if (sproductId > 0)
                    {
                        modelProductCatalog.noPaged = 1;

                        modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailArray.Where(x => x.productId == sproductId).ToList();
                    }
                    else
                    {
                        modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailArray.ToList();
                    }

                    if (sproductName != null)
                    {
                        modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                    }

                    if (sfv != null)
                    {
                        modelProductCatalog.ProductionDetailListFV = modelProductCatalog.ProductionDetailList.ToList();

                        modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailListFV.Where(x => x.voen.ToLower() == sfv).ToList();

                        if(modelProductCatalog.ProductionDetailList == null || modelProductCatalog.ProductionDetailList.Count == 0)
                        {
                            modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailListFV.Where(x=>x.person!=null).Where(x=>x.person.PinNumber!=null).ToList();
                            
                            modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailList.Where(x => x.person.PinNumber.ToLower() == sfv).ToList();
                        }                        
                    }

                }
                else
                {
                    modelProductCatalog.ProductionDetailList = new List<ProductionDetail>();
                }

                modelProductCatalog.itemCount = modelProductCatalog.ProductionDetailList.Count();
                modelProductCatalog.PagingProduction = modelProductCatalog.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                modelProductCatalog.pName = sproductName;
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

    }
}
