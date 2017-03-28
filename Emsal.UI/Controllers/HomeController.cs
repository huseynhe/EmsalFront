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
using reCaptcha;
using System.Configuration;

namespace Emsal.UI.Controllers
{

    public class HomeController : Controller
    {
        private BaseInput baseInput;

        private static int saddressId;
        private static long srId;
        private static string ssort;
        private static string sname;
        private static string ssurname;
        private static string saddress;
        private static string sproducts;
        private static long sproductId;
        private static string sform;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ProductCatalogViewModel modelProductCatalog;
        private ContactViewModel modelContact;


        public ActionResult Index(int pId = 0)
        {
            try
            {
                string hostAddress = Request.UserHostAddress;
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

                baseInput = new BaseInput();

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

                BaseOutput bouput = srv.WS_GetProductCatalogsByParentId(baseInput, pId, true, out modelProductCatalog.ProductCatalogArray);
                if (modelProductCatalog.ProductCatalogArray != null)
                {
                    modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.ToList();
                }
                else
                {
                    modelProductCatalog.ProductCatalogList = new List<tblProductCatalog>();
                }

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

        public ActionResult AdminUnit(int pId = 0, long rId = 0)
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
                    if (pId > 0)
                    {
                        modelProductCatalog.arrNum = (long)Session["arrNum"] + 1;
                        Session["arrNum"] = modelProductCatalog.arrNum;
                    }
                }

                modelProductCatalog.rIdau = rId;
                modelProductCatalog.pIdau = pId;

                if (modelProductCatalog.PRMAdminUnitList.Count() == 0)
                {
                    return new EmptyResult();
                    //BaseOutput gaud = srv.WS_GetPRM_AdminUnitById(baseInput, pId, true, out modelProductCatalog.PRMAdminUnit);
                }
                else
                {
                    modelProductCatalog.PRMAdminUnitList = modelProductCatalog.PRMAdminUnitList.OrderBy(x => x.Name).ToList();
                    return View(modelProductCatalog);
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult SearchAnnouncement(int? page, string value)
        {
            try
            {

                baseInput = new BaseInput();
                int pageSize = 50;
                int pageNumber = (page ?? 1);

                modelProductCatalog = new ProductCatalogViewModel();

                value = StripTag.strSqlBlocker(value.ToLower());


                if (string.IsNullOrWhiteSpace(value))
                {
                    modelProductCatalog.AnnouncementDetailList = null;
                }
                else
                {
                    modelProductCatalog.OfferProductionDetailSearch = new OfferProductionDetailSearch();
                    modelProductCatalog.OfferProductionDetailSearch.page = pageNumber;
                    modelProductCatalog.OfferProductionDetailSearch.pageSize = pageSize;
                    modelProductCatalog.OfferProductionDetailSearch.productName = value;

                    BaseOutput gap = srv.WS_GetAnnouncementDetails_Search(baseInput, modelProductCatalog.OfferProductionDetailSearch, out modelProductCatalog.AnnouncementDetailArray);

                    BaseOutput gapc = srv.WS_GetAnnouncementDetails_Search_OPC(baseInput, modelProductCatalog.OfferProductionDetailSearch, out modelProductCatalog.itemCount, out modelProductCatalog.itemCountB);

                }

                if(modelProductCatalog.AnnouncementDetailArray!=null)
                {
                    modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.ToList();
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

        public ActionResult Announcement(int? page, int productId = -1, string form = null)
        {
            try
            {
                baseInput = new BaseInput();
                string partial = "";

                if (productId == -1)
                {
                    sform = form;
                }

                if (productId == -1)
                {
                    productId = 0;
                }

                int pageSize = 36;
                int pageNumber = (page ?? 1);

                modelProductCatalog = new ProductCatalogViewModel();
                modelProductCatalog.noPaged = 0;

                if (productId > 0)
                {
                    modelProductCatalog.noPaged = 1;
                    BaseOutput gadp = srv.WS_GetAnnouncementDetailsByProductId_OP(baseInput, productId, true,  pageNumber, true, pageSize,  true, out modelProductCatalog.AnnouncementDetailArray);


                    BaseOutput gadpc = srv.WS_GetAnnouncementDetailsByProductId_OPC(baseInput, productId, true, out modelProductCatalog.itemCount, out modelProductCatalog.itemCountB);
                }
                else
                {
                    BaseOutput gadp = srv.WS_GetAnnouncementDetails_OP(baseInput, pageNumber, true, pageSize, true, out modelProductCatalog.AnnouncementDetailArray);

                    BaseOutput gadpc = srv.WS_GetAnnouncementDetails_OPC(baseInput, out modelProductCatalog.itemCount, out modelProductCatalog.itemCountB);
                }

                if (modelProductCatalog.AnnouncementDetailArray == null)
                {
                    modelProductCatalog.AnnouncementDetailList = new List<AnnouncementDetail>();
                }
                else
                {
                    modelProductCatalog.AnnouncementDetailList = modelProductCatalog.AnnouncementDetailArray.ToList();
                }


                long[] aic = new long[modelProductCatalog.itemCount];

                modelProductCatalog.PagingT = aic.ToPagedList(pageNumber, pageSize);

                if (sform == null)
                {
                    partial = "PartialAnnouncementG";
                }
                //else if (sform == "g")
                //{
                //    partial = "PartialAnnouncementG";
                //}
                else if (sform == "gl")
                {
                    partial = "PartialAnnouncementGL";
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                modelProductCatalog.form = form;
                modelProductCatalog.partial = partial;

                return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView(partial, modelProductCatalog)
                    : View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult UserInfo(int? page, int addressId = 0, long rId = 0, string sort = null, string name = null, string address = null, long productId = -1)
        {
            try
            {
                if (sort != null)
                    sort = StripTag.strSqlBlocker(sort.ToLower());
                if (name != null)
                    name = StripTag.strSqlBlocker(name.ToLower());
                if (address != null)
                    address = StripTag.strSqlBlocker(address.ToLower());

                if (addressId == 0 && rId == 0 && sort == null && name == null && address == null && productId == -1)
                {
                    saddressId = 0;
                    srId = 15;
                    ssort = null;
                    sname = null;
                    saddress = null;
                    sproductId = 0;
                }

                if (addressId == -1)
                    saddressId = 0;

                if (addressId > 0)
                    saddressId = addressId;

                if (rId > 0)
                    srId = rId;
                if (sort != null)
                    ssort = sort;
                if (name != null)
                    sname = name;
                if (address != null)
                    saddress = address;
                if (productId >= 0)
                    sproductId = productId;


                baseInput = new BaseInput();

                int pageSize = 30;
                int pageNumber = (page ?? 1);

                modelProductCatalog = new ProductCatalogViewModel();

                modelProductCatalog.PotensialUserForAdminUnitIdList = new PotensialUserForAdminUnitIdList();
                modelProductCatalog.PotensialUserForAdminUnitIdList.page = pageNumber;
                modelProductCatalog.PotensialUserForAdminUnitIdList.pageSize = pageSize;

                modelProductCatalog.PotensialUserForAdminUnitIdList.roleID = srId;
                modelProductCatalog.PotensialUserForAdminUnitIdList.name = sname;
                modelProductCatalog.PotensialUserForAdminUnitIdList.adminUnitID = saddressId;
                modelProductCatalog.PotensialUserForAdminUnitIdList.address = saddress;
                modelProductCatalog.PotensialUserForAdminUnitIdList.productID = sproductId;


                BaseOutput ui = srv.WS_GetPotensialUserList_OP(baseInput, modelProductCatalog.PotensialUserForAdminUnitIdList, out modelProductCatalog.UserInfoArray);


                if (modelProductCatalog.UserInfoArray != null)
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoArray.ToList();
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
                    pitem.parentAdminUnitName = string.Join(", ", pitem.productCatalogDetailList.Select(x => x.productName + " (" + x.productCatalog.ProductName + ")"));

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
                else if (ssort == "address asc")
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderBy(x => x.adminUnitName).ToList();
                }
                else if (ssort == "address desc")
                {
                    modelProductCatalog.UserInfoList = modelProductCatalog.UserInfoList.OrderByDescending(x => x.adminUnitName).ToList();
                }

                modelProductCatalog.addressId = saddressId;
                modelProductCatalog.rId = srId;
                modelProductCatalog.name = sname;
                modelProductCatalog.address = saddress;
                modelProductCatalog.productId = sproductId;
                modelProductCatalog.sort = ssort;

                BaseOutput gdpc = srv.WS_GetPotensialUserList_OPC(baseInput, modelProductCatalog.PotensialUserForAdminUnitIdList, out modelProductCatalog.itemCount, out modelProductCatalog.itemCountB);

                long[] aic = new long[modelProductCatalog.itemCount];

                modelProductCatalog.PagingT = aic.ToPagedList(pageNumber, pageSize);

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

                //if (pId == 0)
                //{
                //    return View("Main", modelProductCatalog);
                //}
                //else
                //{
                modelProductCatalog.pId = pId;
                return View(modelProductCatalog);
                //}

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

                //ViewBag.Recaptcha = ReCaptcha.GetHtml(ConfigurationManager.AppSettings["ReCaptcha:SiteKey"]);
                //ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];

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
                if (ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCaptcha:SecretKey"]))
                {
                    MailMessage msg = new MailMessage();

                    msg.To.Add("mail@tedaruk.gov.az");
                    msg.Subject = "Müraciət göndər";

                    msg.Body = "<p>Ad, soyad, ata adı: " + model.nameSurnameFathername + "</p>" +
                        "<p>E-poçt: " + model.mail + "</p>" +
                        "<p>Telefon: " + model.phone + "</p>" +
                        "<p>Müraciətin məzmunu:  " + model.appealBody + "</p>";

                    msg.IsBodyHtml = true;

                    Mail.SendMail(msg);
                    TempData["Message"] = "Müraciətiniz göndərildi.";

                    return RedirectToAction("Contact", "Home");
                }
                else
                {
                    ViewBag.RecaptchaLastErrors = ReCaptcha.GetLastErrors(this.HttpContext);

                    ViewBag.publicKey = ConfigurationManager.AppSettings["ReCaptcha:SiteKey"];
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = "Müraciətiniz göndərilmədi. Zəhmət olmasa məlumatın düzgünlüyünü yoxlayın.";
                return View(model);
                //return RedirectToAction("Contact", "Home", model);
                //return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
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


        public ActionResult ProductCatalogForSale(string actionName)
        {
            try
            {

                baseInput = new BaseInput();
                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelProductCatalog.ProductCatalogDetailArray);

                if (modelProductCatalog.ProductCatalogDetailArray == null)
                {
                    modelProductCatalog.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelProductCatalog.ProductCatalogDetailList = modelProductCatalog.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelProductCatalog.actionName = actionName;
                return View(modelProductCatalog);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductCatalogForSaleOffer(string actionName)
        {
            try
            {

                baseInput = new BaseInput();
                modelProductCatalog = new ProductCatalogViewModel();

                BaseOutput bouput = srv.WS_GetProductCatalogsOffer(baseInput, out modelProductCatalog.ProductCatalogDetailArray);

                if (modelProductCatalog.ProductCatalogDetailArray == null)
                {
                    modelProductCatalog.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelProductCatalog.ProductCatalogDetailList = modelProductCatalog.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
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
