using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emsal.UI.Controllers
{
    public class HomeController : Controller
    {
         private BaseInput baseInput;       

        //Bir defe calisdir Db yaratsin sonra comment et 
        //srv.WS_createDb();

        //tblProductCatalog pCatalog = new tblProductCatalog();

        //pCatalog = new tblProductCatalog
        //{
        //    ProductName = "Mehsul",
        //    ProductDescription = "Mehsul Catalogu",
        //    ProductCatalogParentID = 0,
        //    canBeOrder = 0,

        //};
        //BaseOutput pout = srv.WS_AddProductCatalog(pCatalog);

        //tblProductCatalog[] pCatalogArray;

        //BaseOutput bouput = srv.WS_GetProductCatalogs(out pCatalogArray);
        //return View(pCatalogArray.ToList());


        //
        // GET: /Home/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ProductCatalogViewModel modelProductCatalog;


        public ActionResult Index()
        {
            //srv.WS_createDb();

            baseInput = new BaseInput();
            modelProductCatalog = new ProductCatalogViewModel();

            return View(modelProductCatalog);
        }

        public ActionResult AdminUnit(int pId = 0)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();


            BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, pId, true, out modelProductCatalog.PRMAdminUnitArray);
            modelProductCatalog.PRMAdminUnitList = modelProductCatalog.PRMAdminUnitArray.ToList();
            modelProductCatalog.fullAddress = string.Join(",", modelProductCatalog.PRMAdminUnitList.Select(x => x.Name));


            BaseOutput bouput = srv.WS_GetAdminUnitsByParentId(baseInput,pId,true, out modelProductCatalog.PRMAdminUnitArray);
            modelProductCatalog.PRMAdminUnitList = modelProductCatalog.PRMAdminUnitArray.ToList();


            if (modelProductCatalog.PRMAdminUnitList.Count() == 0)
            {
                BaseOutput gaud = srv.WS_GetPRM_AdminUnitById(baseInput, pId, true, out modelProductCatalog.PRMAdminUnit);
            }


            return View(modelProductCatalog);

        }
        public ActionResult Announcement(int? page, int productId=0)
        {
            baseInput = new BaseInput();

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            modelProductCatalog = new ProductCatalogViewModel();
            modelProductCatalog.noPaged = 0;

            BaseOutput gap = srv.WS_GetAnnouncements(baseInput, out modelProductCatalog.AnnouncementArray);
            modelProductCatalog.AnnouncementList = modelProductCatalog.AnnouncementArray.ToList();

            if (productId > 0)
            {
                modelProductCatalog.noPaged = 1;
                BaseOutput gai = srv.WS_GetAnnouncementsByProductId(baseInput, productId, true, out modelProductCatalog.AnnouncementArray);
                modelProductCatalog.AnnouncementList = modelProductCatalog.AnnouncementArray.ToList();
            }

            modelProductCatalog.Paging = modelProductCatalog.AnnouncementList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialAnnouncement", modelProductCatalog)
                : View(modelProductCatalog);

            //return View(modelProductCatalog);
        }

        public ActionResult UserInfo(int? page)
        {
            baseInput = new BaseInput();

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput ui = srv.WS_GetPotensialUserList(baseInput, out modelProductCatalog.UserInfoArray);
            modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoArray.ToList();

            modelProductCatalog.PagingUserInfo = modelProductCatalog.UserInfoList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialUserInfo", modelProductCatalog)
                : View(modelProductCatalog);

            //return View(modelProductCatalog);
        }

        public ActionResult UserInfoBy(int addressId, int? page)
        {
            baseInput = new BaseInput();

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput ui = srv.WS_GetPotensialUserForAdminUnitIdList(baseInput, addressId, true, out modelProductCatalog.UserInfoArray);
            modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoArray.ToList();

            modelProductCatalog.PagingUserInfo = modelProductCatalog.UserInfoList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialUserInfo", modelProductCatalog)
                : View(modelProductCatalog);

            //return View(modelProductCatalog);
        }



        public ActionResult ProductCatalog(int pId = 0)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput,pId,true, out modelProductCatalog.ProductCatalogArray);
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.ToList();

            if (pId==0)
            {
                return View("PCDetail", modelProductCatalog);
            }
            else
            {
            return View(modelProductCatalog);
            }
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult AnnouncementDetail(int id)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput ga = srv.WS_GetAnnouncementById(baseInput, id, true, out modelProductCatalog.Announcement);

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
    }
}
