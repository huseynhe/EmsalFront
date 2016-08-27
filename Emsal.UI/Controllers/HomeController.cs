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

namespace Emsal.UI.Controllers
{

    public class HomeController : Controller
    {
         private BaseInput baseInput;

        private static int saddressId;
        private static string ssort;
        private static string sname;
        private static string ssurname;
        private static string sproducts;

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
        private ContactViewModel modelContact;


        public ActionResult Index(int pId = 0)
        {
            //srv.WS_createDb();

            //baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            long userId = 0;

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelProductCatalog.User);

            if (modelProductCatalog.User != null)
            {
                Session["musername"] = modelProductCatalog.User.Username;
            }
            else
            {
                Session["musername"] = null;
            }
            //BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelProductCatalog.User);
            //BaseOutput person = srv.WS_GetPersonByUserId(baseInput, (long)userId, true, out modelProductCatalog.Person);


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
                return new EmptyResult();
                //BaseOutput gaud = srv.WS_GetPRM_AdminUnitById(baseInput, pId, true, out modelProductCatalog.PRMAdminUnit);
            }
            else
            {
                return View(modelProductCatalog);
            }
        }

        public ActionResult SearchAnnouncement(string value)
        {
            modelProductCatalog = new ProductCatalogViewModel();

            value =StripTag.strSqlBlocker(value.ToLower());

            if (value=="")
            {
                modelProductCatalog.AnnouncementDetailList = null;
            }
            else
            {
                BaseOutput gap = srv.WS_GetAnnouncementDetails(baseInput, out modelProductCatalog.AnnouncementDetailArray);

            modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.Where(x=>x.announcement.product_name.ToLowerInvariant().Contains(value)).ToList();
            }

            return View(modelProductCatalog);
        }

        public ActionResult LastAnnouncement()
        {
            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput gap = srv.WS_GetAnnouncementDetails(baseInput, out modelProductCatalog.AnnouncementDetailArray);
            modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.Take(5).ToList();

            return View(modelProductCatalog);
        }

        public ActionResult Announcement(int? page, int productId=0)
        {
            baseInput = new BaseInput();

            int pageSize = 12;
            int pageNumber = (page ?? 1);

            modelProductCatalog = new ProductCatalogViewModel();
            modelProductCatalog.noPaged = 0;

            if (productId > 0)
            {
                modelProductCatalog.noPaged = 1;
                BaseOutput gai = srv.WS_GetAnnouncementDetailsByProductId(baseInput, productId, true, out modelProductCatalog.AnnouncementDetailArray);
                modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.ToList();
            }
            else
            {
                BaseOutput gap = srv.WS_GetAnnouncementDetails(baseInput, out modelProductCatalog.AnnouncementDetailArray);
                modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.ToList();
            }

            modelProductCatalog.Paging = modelProductCatalog.AnnouncementDetailList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialAnnouncement", modelProductCatalog)
                : View(modelProductCatalog);

            //return View(modelProductCatalog);
        }

        public ActionResult UserInfo( int? page, int addressId=0, string sort=null, string name=null, string surname=null, string products = null)
        {
            if(sort!=null)
                sort = StripTag.strSqlBlocker(sort.ToLower());
            if (name != null)
                name = StripTag.strSqlBlocker(name.ToLower());
            if (surname != null)
                surname = StripTag.strSqlBlocker(surname.ToLower());
            if (products != null)
                products = StripTag.strSqlBlocker(products.ToLower());

            if (addressId==0 && sort==null && name==null && surname==null && products == null)
            {
                saddressId = 0;
                ssort = null;
                sname = null;
                ssurname = null;
                sproducts = null;
            }

            if (addressId > 0)
                saddressId = addressId;
            if (sort != null)
                ssort = sort;
            if (name != null)
                sname = name;
            if (surname != null)
                ssurname = surname;
            if (products != null)
                sproducts = products;


            baseInput = new BaseInput();

            int pageSize = 25;
            int pageNumber = (page ?? 1);

            modelProductCatalog = new ProductCatalogViewModel();

            if (saddressId > 0)
            {
                BaseOutput uia = srv.WS_GetPotensialUserForAdminUnitIdList(baseInput, saddressId, true, out modelProductCatalog.UserInfoArray);
            }
            else {
                BaseOutput ui = srv.WS_GetPotensialUserList(baseInput, out modelProductCatalog.UserInfoArray);
            }

            modelProductCatalog.UserInfoList= modelProductCatalog.UserInfoArray.ToList();


            if (ssort == "name asc")
            {
                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderBy(x => x.name).ToList();
            }
            else if (ssort == "name desc")
            {
                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderByDescending(x => x.name).ToList();
            }
            else if (ssort == "surname asc")
            {
                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderBy(x => x.surname).ToList();
            }
            else if (ssort == "surname desc")
            {
                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderByDescending(x => x.surname).ToList();
            }


            if (sname!=null)
            {
                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.name.ToLowerInvariant().Contains(sname)).ToList();
            }
            if (ssurname != null)
            {
                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.surname.ToLowerInvariant().Contains(ssurname)).ToList();
            }

            modelProductCatalog.addressId = saddressId;
            modelProductCatalog.sort = ssort;
            modelProductCatalog.name = sname;
            modelProductCatalog.surname = ssurname; 
            modelProductCatalog.products = sproducts;            

            modelProductCatalog.PagingUserInfo = modelProductCatalog.UserInfoList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialUserInfo", modelProductCatalog)
                : View(modelProductCatalog);
        }


        public ActionResult ProductCatalog(int pId = 0)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput,pId,true, out modelProductCatalog.ProductCatalogArray);
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.OrderBy(x=>x.ProductName).ToList();

            modelProductCatalog.ProductCatalogListPC = new List<tblProductCatalog>();
            foreach (tblProductCatalog itm in modelProductCatalog.ProductCatalogList)
            {
                BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelProductCatalog.ProductCatalogArrayPC);

                BaseOutput gai = srv.WS_GetAnnouncementDetailsByProductId(baseInput, itm.Id, true, out modelProductCatalog.AnnouncementDetailArray);
                modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.ToList();
                itm.ProductDescription = modelProductCatalog.AnnouncementDetailList.Count().ToString();

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
        public ActionResult Contact()
        {
            baseInput = new BaseInput();
            modelContact = new ContactViewModel();

            return View(modelContact);
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel model, FormCollection collection)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("ferid.heziyev@gmail.com", "emsal.az");

                msg.To.Add("qorxmazz@gmail.com");
                string fromPassword = "e1701895";
                msg.Subject = "Müraciət göndər";

                msg.Body = "<p>Ad, soyad, ata adı: " + model.nameSurnameFathername + "</p>" +
                    "<p>E-poçt: " + model.mail + "</p>" +
                    "<p>Telefon: " + model.phone + "</p>" +
                    "<p>Müraciətin məzmunu:  " + model.appealBody + "</p>";

                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
                smtp.Timeout = 20000;
                smtp.Send(msg);

                return RedirectToAction("Index", "Contact");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }
            
        }

        public ActionResult AnnouncementDetail(int id)
        {
            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            BaseOutput ga = srv.WS_GetAnnouncementDetailById(baseInput, id, true, out modelProductCatalog.AnnouncementDetail);

            modelProductCatalog.startDate = DatetimeExtension.toShortDate(modelProductCatalog.AnnouncementDetail.announcement.startDate);

            modelProductCatalog.endDate = DatetimeExtension.toShortDate(modelProductCatalog.AnnouncementDetail.announcement.endDate);

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
