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

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.OfferMonitoring)]

    public class OfferMonitoringController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string smonitoringStatusEV;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
       // Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private OfferMonitoringViewModel modelOfferMonitoring;

        public ActionResult Index(int? page, string monitoringStatusEV = null, string productName = null, string userInfo = null)
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

                modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferMonitoring.EnumCategory.Id).ToList();

                if (sproductName != null)
                {
                    modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.productName.ToLowerInvariant().Contains(sproductName)).ToList();
                }

                if (suserInfo != null)
                {
                    if (modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }

                    if (modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }

                    if (modelOfferMonitoring.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }
                    else
                    {
                        modelOfferMonitoring.ProductionDetailList = modelOfferMonitoring.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }
                }


                modelOfferMonitoring.Paging = modelOfferMonitoring.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                if (smonitoringStatusEV == "new")
                    modelOfferMonitoring.isMain = 0;
                else
                    modelOfferMonitoring.isMain = 1;


                modelOfferMonitoring.monitoringStatusEV = smonitoringStatusEV;
                modelOfferMonitoring.productName = sproductName;
                modelOfferMonitoring.userInfo = suserInfo;
                //return View(modelDemandProduction);

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelOfferMonitoring)
                   : View(modelOfferMonitoring);


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

                BaseOutput pout = srv.WS_SendConfirmationMessage(baseInput, model.ConfirmationMessage);


                model.OfferProduction = new tblOffer_Production();

                BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, model.Id, true, out model.OfferProduction);

                //BaseOutput envalyd = srv.WS_GetEnumValueById(baseInput, model.monitoringStatusEVId,true, out model.EnumValueST);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reedited", out model.EnumValueST);


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

                return RedirectToAction("Index", "OfferMonitoring", new { monitoringStatusEV = model.EnumValueST.name });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
