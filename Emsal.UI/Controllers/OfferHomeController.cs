﻿using Emsal.UI.Models;
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

namespace Emsal.UI.Controllers
{

    public class OfferHomeController : Controller
    {
         private BaseInput baseInput;

        private static int saddressId;
        private static string ssort;
        private static string sname;
        private static string ssurname;
        private static string sadminUnitName;


        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ProductCatalogViewModel modelProductCatalog;
        private ContactViewModel modelContact;


        public ActionResult Index(int pId = 0)
        {
            //srv.WS_createDb();

            //baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, pId, true, out modelProductCatalog.ProductCatalogArray);
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.ToList();

            if (pId == 0)
            {
                return View("PCDetail", modelProductCatalog);
            }
            else
            {
                return View(modelProductCatalog);
            }
        } 
  
        public ActionResult OfferProduction(int? page, int productId=0)
        {
            baseInput = new BaseInput();

            int pageSize = 12;
            int pageNumber = (page ?? 1);

            modelProductCatalog = new ProductCatalogViewModel();
            modelProductCatalog.noPaged = 0;

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

            BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, (long)modelProductCatalog.EnumValue.Id, true, out modelProductCatalog.ProductionDetailArray);

            if (productId > 0)
            {
                modelProductCatalog.noPaged = 1;

                modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailArray.Where(x => x.productId == productId).ToList();
            }
            else
            {
                modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailArray.ToList();
            }

            modelProductCatalog.PagingProduction = modelProductCatalog.ProductionDetailList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialOfferProduction", modelProductCatalog)
                : View(modelProductCatalog);

            //return View(modelProductCatalog);
        }

        public ActionResult ProductCatalog(int pId = 0)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput,pId,true, out modelProductCatalog.ProductCatalogArray);
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.OrderBy(x => x.ProductName).ToList();

            modelProductCatalog.ProductCatalogListPC = new List<tblProductCatalog>();
            foreach (tblProductCatalog itm in modelProductCatalog.ProductCatalogList)
            {
                BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelProductCatalog.ProductCatalogArrayPC);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, (long)modelProductCatalog.EnumValue.Id, true, out modelProductCatalog.ProductionDetailArray);
                modelProductCatalog.ProductionDetailList = modelProductCatalog.ProductionDetailArray.Where(x => x.productId == itm.Id).ToList();
                itm.ProductDescription = modelProductCatalog.ProductionDetailList.Count().ToString();

                if (modelProductCatalog.ProductCatalogArrayPC.ToList().Count == 0)
                {
                    if (itm.canBeOrder == 1)
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

            if (pId==0)
            {
                return View("PCDetail", modelProductCatalog);
            }
            else
            {
            return View(modelProductCatalog);
            }
        }
       
        public ActionResult OfferProductionDetail(int id)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelProductCatalog.EnumValue);

            BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, (long)modelProductCatalog.EnumValue.Id, true, out modelProductCatalog.ProductionDetailArray);

            modelProductCatalog.ProductionDetail = modelProductCatalog.ProductionDetailArray.Where(x => x.productionID == id).FirstOrDefault();

            return View(modelProductCatalog);
        }

        public ActionResult Signin()
        {
            return View();
        }
        public ActionResult Signout()
        {
            return View();
        }

        public ActionResult PotentialUser()
        {
            return View();
        }

        public ActionResult Region()
        {
            return View();
        }

        public ActionResult District()
        {
            return View();
        }

        public ActionResult Pages()
        {
            return View();
        }
    }
}
