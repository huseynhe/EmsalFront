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

    public class OfferMonitoringController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string smonitoringStatusEV;
        private static string snameSurnameFathername;
        private static string spin;
        private static bool sisApprov;
        private static bool sisSeller;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        // Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private OfferMonitoringViewModel modelOfferMonitoring;

        public ActionResult Index(int? page, string monitoringStatusEV = null, string productName = null, string userInfo = null, bool pdf = false)
        {
            try
            {

                if (monitoringStatusEV != null)
                    monitoringStatusEV = StripTag.strSqlBlocker(monitoringStatusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                if (productName == null && userInfo == null)
                {
                    sproductName = null;
                    suserInfo = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (userInfo != null)
                    suserInfo = userInfo;
                if (monitoringStatusEV != null)
                    smonitoringStatusEV = monitoringStatusEV;

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


                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferMonitoring.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, smonitoringStatusEV, out modelOfferMonitoring.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForMonitoringEVId(baseInput, (long)UserId, true, modelOfferMonitoring.EnumValue.Id, true, out modelOfferMonitoring.ProductionDetailArray);

                modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferMonitoring.EnumCategory.Id && x.person != null).ToList();

                if (sproductName != null)
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (suserInfo != null)
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                }

                modelOfferMonitoring.isPDF = pdf;
                modelOfferMonitoring.itemCount = modelOfferMonitoring.ProductionDetailList.Count();

                if (modelOfferMonitoring.isPDF == true)
                {
                    modelOfferMonitoring.Paging = modelOfferMonitoring.ProductionDetailList.ToPagedList(1, 50000);
                }
                else
                {
                    modelOfferMonitoring.Paging = modelOfferMonitoring.ProductionDetailList.ToPagedList(pageNumber, pageSize);
                }

                if (smonitoringStatusEV == "new")
                    modelOfferMonitoring.isMain = 0;
                else
                    modelOfferMonitoring.isMain = 1;


                modelOfferMonitoring.monitoringStatusEV = smonitoringStatusEV;
                modelOfferMonitoring.productName = sproductName;
                modelOfferMonitoring.userInfo = suserInfo;
                //return View(modelDemandProduction);

                var gd = Guid.NewGuid();

                if (modelOfferMonitoring.isPDF == true)
                {
                    return new Rotativa.PartialViewAsPdf("PartialIndex", modelOfferMonitoring) { FileName = gd + ".pdf" };
                }
                else
                {
                    return Request.IsAjaxRequest()
                       ? (ActionResult)PartialView("PartialIndex", modelOfferMonitoring)
                       : View(modelOfferMonitoring);
                }
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        [HttpPost]
        public ActionResult Approv(int[] ids)
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

                //Array arrid = ids.Split(',');
                //long id = 0;
                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        modelOfferMonitoring.OfferProduction = new tblOffer_Production();

                        BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, ids[i], true, out modelOfferMonitoring.OfferProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferMonitoring.EnumValueST);

                        modelOfferMonitoring.OfferProduction.monitoring_eV_Id = modelOfferMonitoring.EnumValueST.Id;
                        modelOfferMonitoring.OfferProduction.monitoring_eV_IdSpecified = true;

                        BaseOutput ecout = srv.WS_UpdateOffer_Production(baseInput, modelOfferMonitoring.OfferProduction, out modelOfferMonitoring.OfferProduction);

                        modelOfferMonitoring.ComMessage = new tblComMessage();
                        modelOfferMonitoring.ComMessage.message = "Təsdiqləndi";
                        modelOfferMonitoring.ComMessage.fromUserID = (long)UserId;
                        modelOfferMonitoring.ComMessage.fromUserIDSpecified = true;
                        modelOfferMonitoring.ComMessage.toUserID = modelOfferMonitoring.OfferProduction.user_Id;
                        modelOfferMonitoring.ComMessage.toUserIDSpecified = true;
                        modelOfferMonitoring.ComMessage.Production_Id = modelOfferMonitoring.OfferProduction.Id;
                        modelOfferMonitoring.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferMonitoring.EnumValue);
                        modelOfferMonitoring.ComMessage.Production_type_eV_Id = modelOfferMonitoring.EnumValue.Id;
                        modelOfferMonitoring.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelOfferMonitoring.ComMessage, out modelOfferMonitoring.ComMessage);


                        try
                        {
                            string sn = "";
                            string pr = "";
                            BaseOutput muser = srv.WS_GetUserById(baseInput, (long)modelOfferMonitoring.OfferProduction.user_Id, true, out modelOfferMonitoring.User);

                            BaseOutput person = srv.WS_GetPersonByUserId(baseInput, modelOfferMonitoring.User.Id, true, out modelOfferMonitoring.Person);

                            BaseOutput product = srv.WS_GetProductCatalogsById(baseInput, (int)modelOfferMonitoring.OfferProduction.product_Id, true, out modelOfferMonitoring.ProductCatalog);

                            if (modelOfferMonitoring.ProductCatalog != null)
                            {
                                pr = modelOfferMonitoring.ProductCatalog.ProductName;
                            }

                            if (modelOfferMonitoring.Person != null)
                            {
                                sn = modelOfferMonitoring.Person.Surname + " " + modelOfferMonitoring.Person.Name;
                            }

                            MailMessage msg = new MailMessage();

                            msg.To.Add(modelOfferMonitoring.User.Email);
                            msg.Subject = "Təklifin təsdiqi";

                            msg.Body = "<b>Hörmətli " + sn + ", </b><br/><br/> Sizin <b>tedaruk.az</b> portalı vasitəsi ilə " + pr + " bağlı verdiyiniz təklif Ərzaq məhsullarının tədarükü və təchizatı Açıq Səhmdar Cəmiyyəti tərəfindən təsdiq etdilmişdir. Öz təklifinizi portalda təkliflər bölməsində yoxlaya bilərsiniz. <br/><br/>Azərbaycan Respublikasının Kənd Təsərrüfatı Nazirliyi";

                            msg.IsBodyHtml = true;

                            Mail.SendMail(msg);
                        }
                        catch { }
                    }
                }

                return RedirectToAction("Index", "OfferMonitoring", new { monitoringStatusEV = modelOfferMonitoring.EnumValueST.name });


            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Edit(int id)
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

                BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferMonitoring.OfferProduction);

                modelOfferMonitoring.Id = modelOfferMonitoring.OfferProduction.Id;

                BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "State", out modelOfferMonitoring.EnumCategory);

                BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferMonitoring.EnumCategory.Id, true, out modelOfferMonitoring.EnumValueArray);
                modelOfferMonitoring.EnumValueList = modelOfferMonitoring.EnumValueArray.ToList();

                return View(modelOfferMonitoring);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(OfferMonitoringViewModel model, FormCollection collection)
        {
            try
            {

                baseInput = new BaseInput();

                model.ConfirmationMessage = new tblConfirmationMessage();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out model.User);
                baseInput.userName = model.User.Username;

                model.ConfirmationMessage.Message = model.message;

                BaseOutput pout = srv.WS_SendConfirmationMessageNew(baseInput, model.ConfirmationMessage, out model.ConfirmationMessage);


                model.OfferProduction = new tblOffer_Production();

                BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, model.Id, true, out model.OfferProduction);

                BaseOutput envalyd = srv.WS_GetEnumValueById(baseInput, model.monitoringStatusEVId, true, out model.EnumValueST);

                //BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reedited", out model.EnumValueST);


                model.OfferProduction.monitoring_eV_Id = model.EnumValueST.Id;
                model.OfferProduction.monitoring_eV_IdSpecified = true;

                BaseOutput ecout = srv.WS_UpdateOffer_Production(baseInput, model.OfferProduction, out model.OfferProduction);

                model.ComMessage = new tblComMessage();
                model.ComMessage.message = model.message;
                model.ComMessage.fromUserID = (long)UserId;
                model.ComMessage.fromUserIDSpecified = true;
                model.ComMessage.toUserID = model.OfferProduction.user_Id;
                model.ComMessage.toUserIDSpecified = true;
                model.ComMessage.Production_Id = model.OfferProduction.Id;
                model.ComMessage.Production_IdSpecified = true;

                BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out model.EnumValue);
                model.ComMessage.Production_type_eV_Id = model.EnumValue.Id;
                model.ComMessage.Production_type_eV_IdSpecified = true;

                BaseOutput acm = srv.WS_AddComMessage(baseInput, model.ComMessage, out model.ComMessage);


                try
                {
                    modelOfferMonitoring = new OfferMonitoringViewModel();

                    string sn = "";
                    string pr = "";
                    BaseOutput muser = srv.WS_GetUserById(baseInput, (long)model.OfferProduction.user_Id, true, out modelOfferMonitoring.User);

                    BaseOutput person = srv.WS_GetPersonByUserId(baseInput, modelOfferMonitoring.User.Id, true, out modelOfferMonitoring.Person);

                    BaseOutput product = srv.WS_GetProductCatalogsById(baseInput, (int)model.OfferProduction.product_Id, true, out modelOfferMonitoring.ProductCatalog);

                    if (modelOfferMonitoring.ProductCatalog != null)
                    {
                        pr = modelOfferMonitoring.ProductCatalog.ProductName;
                    }

                    if (modelOfferMonitoring.Person != null)
                    {
                        sn = modelOfferMonitoring.Person.Surname + " " + modelOfferMonitoring.Person.Name;
                    }

                    MailMessage msg = new MailMessage();

                    msg.To.Add(modelOfferMonitoring.User.Email);

                    if (model.OfferProduction.monitoring_eV_Id == 41)
                    {
                        msg.Subject = "Təklifə imtina edilməsi";

                        msg.Body = "<b>Hörmətli " + sn + ", </b><br/><br/> Sizin <b>tedaruk.az</b> portalı vasitəsi ilə " + pr + " bağlı verdiyiniz təklif Ərzaq məhsullarının tədarükü və təchizatı Açıq Səhmdar Cəmiyyəti tərəfindən imtina edilmişdir. Həmin təklifə şəxsi səhifənizdə <b>Yararsız sayılan təkliflər</b> bölməsinə daxil olaraq görə bilərsiniz.<br/>İmtinanın səbəbi: " + model.ComMessage.message + " <br/><br/>Azərbaycan Respublikasının Kənd Təsərrüfatı Nazirliyi";
                    }
                    else if (model.OfferProduction.monitoring_eV_Id == 10117)
                    {
                        msg.Subject = "Təklifə düzəliş edilməsi";

                        msg.Body = "<b>Hörmətli " + sn + ", </b><br/><br/> Sizin <b>tedaruk.az</b> portalı vasitəsi ilə " + pr + " bağlı verdiyiniz təklif Ərzaq məhsullarının tədarükü və təchizatı Açıq Səhmdar Cəmiyyəti tərəfindən düzəliş üçün yenidən Sizə qaytarılmışdır. Həmin təklifə şəxsi səhifənizdə <b>Yararsız sayılan təkliflər</b> bölməsinə daxil olaraq düzəliş edə bilərsiniz.<br/>Düzəlişin səbəbi: " + model.ComMessage.message + " <br/><br/>Azərbaycan Respublikasının Kənd Təsərrüfatı Nazirliyi";
                    }
                    msg.IsBodyHtml = true;

                    Mail.SendMail(msg);
                }
                catch { }


                if (model.attachfiles != null)
                {
                    baseInput = new BaseInput();

                    modelOfferMonitoring = new OfferMonitoringViewModel();

                    String sDate = DateTime.Now.ToString();
                    DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                    String dy = datevalue.Day.ToString();
                    String mn = datevalue.Month.ToString();
                    String yy = datevalue.Year.ToString();

                    string path = modelOfferMonitoring.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (var attachfile in model.attachfiles)
                    {
                        if (attachfile == null)
                        {
                            return RedirectToAction("Index", "OfferMonitoring", new { monitoringStatusEV = model.EnumValueST.name });
                        }

                        string fre = FileExtension.GetMimeType(attachfile.InputStream, attachfile.FileName);

                        if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelOfferMonitoring.fileSize && modelOfferMonitoring.fileTypes.Contains(fre))
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

                            modelOfferMonitoring.ComMessageAttachment = new tblComMessageAttachment();

                            modelOfferMonitoring.ComMessageAttachment.CMessageId = model.ComMessage.Id;
                            modelOfferMonitoring.ComMessageAttachment.CMessageIdSpecified = true;

                            modelOfferMonitoring.ComMessageAttachment.UserID = model.User.Id;
                            modelOfferMonitoring.ComMessageAttachment.UserIDSpecified = true;

                            modelOfferMonitoring.ComMessageAttachment.documentUrl = path;
                            modelOfferMonitoring.ComMessageAttachment.documentName = fileName;
                            modelOfferMonitoring.ComMessageAttachment.documentRealName = ofileName;

                            modelOfferMonitoring.ComMessageAttachment.documentSize = attachfile.ContentLength;
                            modelOfferMonitoring.ComMessageAttachment.documentSizeSpecified = true;

                            BaseOutput apd = srv.WS_AddComMessageAttachment(baseInput, modelOfferMonitoring.ComMessageAttachment, out modelOfferMonitoring.ComMessageAttachment);
                        }
                    }
                }


                return RedirectToAction("Index", "OfferMonitoring", new { monitoringStatusEV = model.EnumValueST.name });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Contract(int? page, bool isApprov = false, bool isSeller = false, string nameSurnameFathername = null, string pin = null)
        {
            try
            {
                if (nameSurnameFathername != null)
                    nameSurnameFathername = StripTag.strSqlBlocker(nameSurnameFathername.ToLower());
                if (pin != null)
                    pin = StripTag.strSqlBlocker(pin.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (nameSurnameFathername == null && pin == null && isApprov == false && isSeller == false)
                {
                    snameSurnameFathername = null;
                    spin = null;
                    sisApprov = false;
                    sisSeller = false;
                }

                if (nameSurnameFathername != null)
                    snameSurnameFathername = nameSurnameFathername;
                if (pin != null)
                    spin = pin;
                if (isApprov == true || isSeller == true)
                {
                    sisApprov = isApprov;
                    sisSeller = isSeller;
                }


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

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferMonitoring.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferMonitoring.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForMonitoringEVId(baseInput, (long)UserId, true, modelOfferMonitoring.EnumValue.Id, true, out modelOfferMonitoring.ProductionDetailArray);

                modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferMonitoring.EnumCategory.Id && x.person != null).ToList();

                modelOfferMonitoring.PersonList = new List<tblPerson>();
                long opid = 0;
                modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.OrderBy(x => x.person.Id).ToList();

                if (sisApprov == false)
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.contractID == 0 || x.contractID == null).ToList();
                }
                else
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.contractID > 0).ToList();
                }

                if (sisSeller == true)
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.roleID == 11).ToList();
                }
                else
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.roleID != 11).ToList();
                }

                foreach (var item in modelOfferMonitoring.ProductionDetailList)
                {
                    if (opid != item.person.Id)
                    {
                        modelOfferMonitoring.Person = new tblPerson();

                        modelOfferMonitoring.Person.Id = item.person.Id;
                        modelOfferMonitoring.Person.Name = item.person.Name;
                        modelOfferMonitoring.Person.Surname = item.person.Surname;
                        modelOfferMonitoring.Person.FatherName = item.person.FatherName;
                        modelOfferMonitoring.Person.gender = item.person.gender;
                        modelOfferMonitoring.Person.PinNumber = item.person.PinNumber;
                        modelOfferMonitoring.Person.profilePicture = item.person.profilePicture;

                        modelOfferMonitoring.PersonList.Add(modelOfferMonitoring.Person);
                    }

                    opid = item.person.Id;
                }

                if (snameSurnameFathername != null)
                {
                    modelOfferMonitoring.PersonList = modelOfferMonitoring.PersonList.Where(x => x.Name.ToLower().Contains(snameSurnameFathername) || x.Surname.ToLower().Contains(snameSurnameFathername) || x.FatherName.ToLower().Contains(snameSurnameFathername)).ToList();
                }

                if (spin != null)
                {
                    modelOfferMonitoring.PersonList = modelOfferMonitoring.PersonList.Where(x => x.PinNumber.ToLower().Contains(spin)).ToList();
                }

                modelOfferMonitoring.itemCount = modelOfferMonitoring.PersonList.Count();

                modelOfferMonitoring.PagingPerson = modelOfferMonitoring.PersonList.ToPagedList(pageNumber, pageSize);

                modelOfferMonitoring.nameSurnameFathername = snameSurnameFathername;
                modelOfferMonitoring.pin = spin;
                modelOfferMonitoring.isApprov = sisApprov;
                modelOfferMonitoring.isSeller = sisSeller;

                return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialContract", modelOfferMonitoring)
                : View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [WordDocument]
        public ActionResult ContractForm(long pid, bool isContract)
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

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferMonitoring.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferMonitoring.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForMonitoringEVId(baseInput, (long)UserId, true, modelOfferMonitoring.EnumValue.Id, true, out modelOfferMonitoring.ProductionDetailArray);

                modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferMonitoring.EnumCategory.Id && x.person != null).ToList();

                modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Id == pid).ToList();

                BaseOutput gpbui = srv.WS_GetPersonByUserId(baseInput, modelOfferMonitoring.User.Id, true, out modelOfferMonitoring.Person);

                modelOfferMonitoring.icraci = modelOfferMonitoring.Person.Surname + " " + modelOfferMonitoring.Person.Name + " " + modelOfferMonitoring.Person.FatherName;

                //Guid barcode = Guid.NewGuid();
                //string bcode = barcode.ToString();
                //string barCode = BarCodeToHTML.get39(bcode, 2, 20);
                //ViewBag.htmlBarcode = barCode;

                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult FileTemplate(long pid)
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

                //BaseOutput gpbuid = srv.WS_GetPersonById(baseInput, pid, true, out modelOfferMonitoring.Person);

                BaseOutput gfobuid = srv.WS_GetContractBySupplierUserID(baseInput, pid, true, out modelOfferMonitoring.ContractArray);

                //BaseOutput gfobuid = srv.WS_GetContract(baseInput, out modelOfferMonitoring.ContractArray);

                if (modelOfferMonitoring.ContractArray != null)
                {
                    modelOfferMonitoring.ContractList = modelOfferMonitoring.ContractArray.ToList();
                }
                else
                {
                    modelOfferMonitoring.ContractList = new List<tblContract>();
                }

                foreach (var item in modelOfferMonitoring.ContractList)
                {
                    string fileName = item.documentName;
                    string targetPath = modelOfferMonitoring.tempFileDirectory;

                    string sourceFile = System.IO.Path.Combine(item.documentUrl, fileName);
                    string destFile = System.IO.Path.Combine(targetPath, fileName);

                    string[] files = Directory.GetFiles(targetPath);

                    foreach (string file in files)
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastAccessTime < DateTime.Now.AddHours(-1))
                            fi.Delete();
                    }

                    if (!System.IO.Directory.Exists(targetPath))
                    {
                        System.IO.Directory.CreateDirectory(targetPath);
                    }

                    var extension = Path.GetExtension(fileName);

                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return null;
                    }


                    //var registryKey = Registry.ClassesRoot.OpenSubKey(extension);

                    //if (registryKey == null)
                    //{
                    //    return null;
                    //}

                    //modelOfferMonitoring.FCType = registryKey.GetValue("Content Type") as string;

                    modelOfferMonitoring.FCType = FileExtension.GetMimeTypeSimple(extension);

                    System.IO.File.Copy(sourceFile, destFile, true);
                }

                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        [HttpPost]
        public void File(IList<HttpPostedFileBase> file, long pid)
        {
            try
            {
                if (file != null)
                {
                    baseInput = new BaseInput();
                    modelOfferMonitoring = new OfferMonitoringViewModel();

                    long? userId = null;
                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        FormsIdentity identity = (FormsIdentity)User.Identity;
                        if (identity.Ticket.UserData.Length > 0)
                        {
                            userId = Int32.Parse(identity.Ticket.UserData);
                        }
                    }
                    BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferMonitoring.User);
                    baseInput.userName = modelOfferMonitoring.User.Username;


                    String sDate = DateTime.Now.ToString();
                    DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                    String dy = datevalue.Day.ToString();
                    String mn = datevalue.Month.ToString();
                    String yy = datevalue.Year.ToString();

                    string path = modelOfferMonitoring.fileDirectoryContract + @"\" + yy + @"\" + mn + @"\" + dy;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferMonitoring.EnumValue);

                    BaseOutput gpbuid = srv.WS_GetPersonById(baseInput, pid, true, out modelOfferMonitoring.Person);

                    BaseOutput gfobuid = srv.WS_GetForeign_OrganizationByUserId(baseInput, (long)modelOfferMonitoring.Person.UserId, true, out modelOfferMonitoring.Foreign_Organization);

                    BaseOutput fop = srv.WS_GetOffer_ProductionsByUserID(baseInput, (long)modelOfferMonitoring.Person.UserId, true, out modelOfferMonitoring.OfferProductionArray);

                    if (modelOfferMonitoring.OfferProductionArray != null)
                    {
                        modelOfferMonitoring.OfferProductionList = modelOfferMonitoring.OfferProductionArray.ToList();
                    }
                    else
                    {
                        modelOfferMonitoring.OfferProductionList = new List<tblOffer_Production>();
                    }

                    modelOfferMonitoring.OfferProductionList = modelOfferMonitoring.OfferProductionList.Where(x => x.contractId == 0 || x.contractId == null).Where(x => x.monitoring_eV_Id == modelOfferMonitoring.EnumValue.Id).ToList();



                    foreach (var attachfile in file)
                    {
                        //var dec=(attachfile.ContentLength / 1024);
                        //var decd = (decimal)731 / 1024;
                        //var fl = Math.Round(dec,2);

                        string fre = FileExtension.GetMimeType(attachfile.InputStream, attachfile.FileName);

                        if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelOfferMonitoring.fileSize && modelOfferMonitoring.fileTypesPDF.Contains(fre))
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


                            modelOfferMonitoring.Contract = new tblContract();

                            modelOfferMonitoring.Contract.AgentUserID = modelOfferMonitoring.User.Id;
                            modelOfferMonitoring.Contract.AgentUserIDSpecified = true;

                            if (modelOfferMonitoring.Foreign_Organization != null)
                            {
                                modelOfferMonitoring.Contract.SupplierOrganisationID = modelOfferMonitoring.Foreign_Organization.Id;
                                modelOfferMonitoring.Contract.SupplierOrganisationIDSpecified = true;
                            }

                            modelOfferMonitoring.Contract.SupplierUserID = pid;
                            modelOfferMonitoring.Contract.SupplierUserIDSpecified = true;

                            modelOfferMonitoring.Contract.documentUrl = path;
                            modelOfferMonitoring.Contract.documentName = fileName;
                            modelOfferMonitoring.Contract.documentRealName = ofileName;
                            modelOfferMonitoring.Contract.documentSize = attachfile.ContentLength;
                            modelOfferMonitoring.Contract.documentSizeSpecified = true;

                            BaseOutput apd = srv.WS_AddContract(baseInput, modelOfferMonitoring.Contract, out modelOfferMonitoring.Contract);

                            foreach (var item in modelOfferMonitoring.OfferProductionList)
                            {
                                item.contractId = modelOfferMonitoring.Contract.Id;

                                BaseOutput upop = srv.WS_UpdateOffer_Production(baseInput, item, out modelOfferMonitoring.OfferProduction);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

        public ActionResult GetContractFile(string fname)
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

                modelOfferMonitoring.fname = fname;

                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public void DeleteContract(long id)
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


                BaseOutput gc = srv.WS_GetContractById(baseInput, id, true, out modelOfferMonitoring.Contract);

                BaseOutput dc = srv.WS_DeleteContract(baseInput, modelOfferMonitoring.Contract);



                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferMonitoring.EnumValue);

                BaseOutput gpbuid = srv.WS_GetPersonById(baseInput, (long)modelOfferMonitoring.Contract.SupplierUserID, true, out modelOfferMonitoring.Person);

                BaseOutput fop = srv.WS_GetOffer_ProductionsByUserID(baseInput, (long)modelOfferMonitoring.Person.UserId, true, out modelOfferMonitoring.OfferProductionArray);

                if (modelOfferMonitoring.OfferProductionArray != null)
                {
                    modelOfferMonitoring.OfferProductionList = modelOfferMonitoring.OfferProductionArray.ToList();
                }
                else
                {
                    modelOfferMonitoring.OfferProductionList = new List<tblOffer_Production>();
                }

                modelOfferMonitoring.OfferProductionList = modelOfferMonitoring.OfferProductionList.Where(x => x.contractId == id).Where(x => x.monitoring_eV_Id == modelOfferMonitoring.EnumValue.Id).ToList();

                foreach (var item in modelOfferMonitoring.OfferProductionList)
                {
                    item.contractId = 0;

                    BaseOutput upop = srv.WS_UpdateOffer_Production(baseInput, item, out modelOfferMonitoring.OfferProduction);
                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }

    }
}
