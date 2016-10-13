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
using Emsal.Utility.UtilityObjects;
using Emsal.WebInt.IAMAS;
using System.Web.Hosting;

namespace Emsal.UI.Controllers
{

    public class HomeController : Controller
    {
        private BaseInput baseInput;

        private static int saddressId;
        private static string ssort;
        private static string sname;
        private static string ssurname;
        private static string saddress;
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
       // Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private ProductCatalogViewModel modelProductCatalog;
        private ContactViewModel modelContact;
        

        public ActionResult Index(int pId = 0)
        {
            try
            {
                string hostAddress = Request.UserHostAddress;     
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

                //srv.WS_createDb();

                //SingleServiceControl srvcontrol = new SingleServiceControl();
                //tblPerson person ;
                //getPersonalInfoByPinNewResponseResponse iamasPerson;

                //int control = srvcontrol.getPersonInfoByPin("", out person, out iamasPerson);

                baseInput = new BaseInput();

                baseInput.TransactionId = Int64.Parse(IOUtil.GetFunctionRequestID());
                baseInput.ChannelId = ChannelEnum.Emsal.ToString();

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
                    return View("Main", modelProductCatalog);
                }
                else
                {
                    return View(modelProductCatalog);
                }

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult AdminUnit(int pId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelProductCatalog = new ProductCatalogViewModel();


                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, pId, true, out modelProductCatalog.PRMAdminUnitArray);
                modelProductCatalog.PRMAdminUnitList = modelProductCatalog.PRMAdminUnitArray.ToList();
                modelProductCatalog.fullAddress = string.Join(",", modelProductCatalog.PRMAdminUnitList.Select(x => x.Name));


                BaseOutput bouput = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelProductCatalog.PRMAdminUnitArray);
                modelProductCatalog.PRMAdminUnitList = modelProductCatalog.PRMAdminUnitArray.ToList();


                if (Session["arrNum"] == null)
                {
                    Session["arrNum"] = modelProductCatalog.arrNum;
                }
                else
                {
                    modelProductCatalog.arrNum = (long)Session["arrNum"] + 1;
                    Session["arrNum"] = modelProductCatalog.arrNum;
                }

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
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult SearchAnnouncement(string value)
        {
            try
            {

                modelProductCatalog = new ProductCatalogViewModel();

                value = StripTag.strSqlBlocker(value.ToLower());

                if (value == "")
                {
                    modelProductCatalog.AnnouncementDetailList = null;
                }
                else
                {
                    BaseOutput gap = srv.WS_GetAnnouncementDetails(baseInput, out modelProductCatalog.AnnouncementDetailArray);

                    modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.Where(x => x.announcementDetail.announcement.product_name.ToLower().Contains(value) || x.parentName.ToLower().Contains(value)).ToList();
                }

                if (modelProductCatalog.AnnouncementDetailList != null)
                {
                    modelProductCatalog.itemCount = modelProductCatalog.AnnouncementDetailList.Count();
                }

                return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult LastAnnouncement()
        {
            try
            {
                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput gap = srv.WS_GetAnnouncementDetails(baseInput, out modelProductCatalog.AnnouncementDetailArray);
                modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.Take(5).ToList();

                return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Announcement(int? page, int productId = 0)
        {
            try
            {
                baseInput = new BaseInput();

                int pageSize = 24;
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

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult UserInfo(int? page, int addressId = 0, string sort = null, string name = null, string surname = null, string address = null, string products = null)
        {
            try
            {
                if (sort != null)
                    sort = StripTag.strSqlBlocker(sort.ToLower());
                if (name != null)
                    name = StripTag.strSqlBlocker(name.ToLower());
                if (surname != null)
                    surname = StripTag.strSqlBlocker(surname.ToLower());
                if (address != null)
                    address = StripTag.strSqlBlocker(address.ToLower());
                if (products != null)
                    products = StripTag.strSqlBlocker(products.ToLower());

                if (addressId == 0 && sort == null && name == null && surname == null && address == null && products == null)
                {
                    saddressId = 0;
                    ssort = null;
                    sname = null;
                    ssurname = null;
                    saddress = null;
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
                if (address != null)
                    saddress = address;
                if (products != null)
                    sproducts = products;


                baseInput = new BaseInput();

                int pageSize = 30;
                int pageNumber = (page ?? 1);

                modelProductCatalog = new ProductCatalogViewModel();

                if (saddressId > 0)
                {
                    BaseOutput uia = srv.WS_GetPotensialUserForAdminUnitIdList(baseInput, saddressId, true, out modelProductCatalog.UserInfoArray);
                }
                else
                {
                    BaseOutput ui = srv.WS_GetPotensialUserList(baseInput, out modelProductCatalog.UserInfoArray);
                }

                if (modelProductCatalog.UserInfoArray != null)
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoArray.ToList();
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.pinNumber != "" || x.voen != "").ToList();
                }
                else
                {
                    modelProductCatalog.UserInfoList = new List<UserInfo>();
                }

                modelProductCatalog.UserInfoListP = new List<UserInfo>();
                foreach (var pitem in modelProductCatalog.UserInfoList)
                {
                    var sd = pitem.fullAddress.Split(',').ToArray();
                    if (sd.Length > 1)
                    {
                        pitem.adminUnitName = sd[1];
                    }
                    else
                    {
                        pitem.adminUnitName = sd[0];
                    }

                    pitem.name = pitem.surname + " " + pitem.name;
                    pitem.parentAdminUnitName = string.Join(", ", pitem.productCatalogDetailList.Select(x => x.productCatalog.ProductName + " (" + x.productName + ")"));

                    modelProductCatalog.UserInfoListP.Add(pitem);
                }

                modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoListP;


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
                else if (ssort == "address asc")
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderBy(x => x.adminUnitName).ToList();
                }
                else if (ssort == "address desc")
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderByDescending(x => x.adminUnitName).ToList();
                }


                if (sname != null)
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.name.ToLower().Contains(sname)).ToList();
                }
                if (ssurname != null)
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.surname.ToLower().Contains(ssurname)).ToList();
                }
                if (saddress != null)
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.adminUnitName.ToLower().Contains(saddress)).ToList();
                }
                if (sproducts != null)
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.Where(x => x.parentAdminUnitName.ToLower().Contains(sproducts)).ToList();
                }


                modelProductCatalog.addressId = saddressId;
                modelProductCatalog.sort = ssort;
                modelProductCatalog.name = sname;
                modelProductCatalog.surname = ssurname;
                modelProductCatalog.address = saddress;
                modelProductCatalog.products = sproducts;

                modelProductCatalog.itemCount = modelProductCatalog.UserInfoList.Count();

                modelProductCatalog.PagingUserInfo = modelProductCatalog.UserInfoList.ToPagedList(pageNumber, pageSize);

                return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("PartialUserInfo", modelProductCatalog)
                    : View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult ProductCatalog(int pId = 0)
        {
            try
            {

                baseInput = new BaseInput();

                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, pId, true, out modelProductCatalog.ProductCatalogArray);
                modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.OrderBy(x => x.ProductName).ToList();

                modelProductCatalog.ProductCatalogListPC = new List<tblProductCatalog>();
                foreach (tblProductCatalog itm in modelProductCatalog.ProductCatalogList)
                {
                    BaseOutput gpcbpid = srv.WS_GetProductCatalogsByParentId(baseInput, (int)itm.Id, true, out modelProductCatalog.ProductCatalogArrayPC);

                    BaseOutput gai = srv.WS_GetAnnouncementDetailsByProductId(baseInput, itm.Id, true, out modelProductCatalog.AnnouncementDetailArray);
                    modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.ToList();
                    itm.ProductDescription = modelProductCatalog.AnnouncementDetailList.Count().ToString();

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
                modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogListPC.OrderBy(x => x.ProductName).ToList();

                if (pId == 0)
                {
                    return View("Main", modelProductCatalog);
                }
                else
                {
                    return View(modelProductCatalog);
                }

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult Contact()
        {
            try
            {

                baseInput = new BaseInput();
                modelContact = new ContactViewModel();

                return View(modelContact);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel model, FormCollection collection)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("ferid.heziyev@gmail.com", "emsal.az");

                msg.To.Add("qorxmazz@gmail.com");
                //string fromPassword = "e1701895";
                msg.Subject = "Müraciət göndər";

                msg.Body = "<p>Ad, soyad, ata adı: " + model.nameSurnameFathername + "</p>" +
                    "<p>E-poçt: " + model.mail + "</p>" +
                    "<p>Telefon: " + model.phone + "</p>" +
                    "<p>Müraciətin məzmunu:  " + model.appealBody + "</p>";

                msg.IsBodyHtml = true;

                Mail.SendMail(msg);

                return RedirectToAction("Index", "Contact");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Services()
        {
            try
            {

                baseInput = new BaseInput();

                return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult AnnouncementDetail(int id)
        {
            try
            {
                baseInput = new BaseInput();

                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput ga = srv.WS_GetAnnouncementDetailById(baseInput, id, true, out modelProductCatalog.AnnouncementDetail);

                modelProductCatalog.startDate = DatetimeExtension.toShortDate(modelProductCatalog.AnnouncementDetail.announcement.startDate);

                modelProductCatalog.endDate = DatetimeExtension.toShortDate(modelProductCatalog.AnnouncementDetail.announcement.endDate);

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

                Session["arrNum"] = null;
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

        public ActionResult Style()
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
    }
}
