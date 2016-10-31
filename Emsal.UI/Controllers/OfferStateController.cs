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
    [EmsalAuthorization(AuthorizedAction = ActionName.OfferState)]

    public class OfferStateController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string sstateStatusEV;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
       // Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private OfferStateViewModel modelOfferState;


        public ActionResult Index(int? page, string stateStatusEV = null, string productName = null, string userInfo = null)
        {
            try
            {

                if (stateStatusEV != null)
                    stateStatusEV = StripTag.strSqlBlocker(stateStatusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());


                int pageSize = 20;
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
                if (stateStatusEV != null)
                    sstateStatusEV = stateStatusEV;

                baseInput = new BaseInput();
                modelOfferState = new OfferStateViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferState.User);
                baseInput.userName = modelOfferState.User.Username;


                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferState.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstateStatusEV, out modelOfferState.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForStateEVId(baseInput, (long)UserId, true, modelOfferState.EnumValue.Id, true, out modelOfferState.ProductionDetailArray);

                if (modelOfferState.ProductionDetailArray != null)
                {
                    modelOfferState.ProductionDetailList = modelOfferState.ProductionDetailArray.Where(x=>x.person!=null).ToList();
                }
                else
                {
                    modelOfferState.ProductionDetailList = new List<ProductionDetail>();
                }

                if (sproductName != null)
                {
                    modelOfferState.ProductionDetailList = modelOfferState.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (suserInfo != null)
                {
                    modelOfferState.ProductionDetailList = modelOfferState.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                }

                modelOfferState.itemCount = modelOfferState.ProductionDetailList.Count();
                modelOfferState.Paging = modelOfferState.ProductionDetailList.ToPagedList(pageNumber, pageSize);


                if (sstateStatusEV == "Yayinda" || sstateStatusEV == "yayinda" || sstateStatusEV=="new")
                    modelOfferState.isMain = 0;
                else
                    modelOfferState.isMain = 1;


                modelOfferState.stateStatusEV = sstateStatusEV;
                modelOfferState.productName = sproductName;
                modelOfferState.userInfo = suserInfo;
                //return View(modelDemandProduction);

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelOfferState)
                   : View(modelOfferState);

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
                modelOfferState = new OfferStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferState.User);
                baseInput.userName = modelOfferState.User.Username;

                //Array arrid = ids.Split(',');
                //long id = 0;
                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        modelOfferState.OfferProduction = new tblOffer_Production();

                        BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, ids[i], true, out modelOfferState.OfferProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferState.EnumValueST);

                        modelOfferState.OfferProduction.state_eV_Id = modelOfferState.EnumValueST.Id;
                        modelOfferState.OfferProduction.state_eV_IdSpecified = true;

                        BaseOutput ecout = srv.WS_UpdateOffer_Production(baseInput, modelOfferState.OfferProduction, out modelOfferState.OfferProduction);

                        modelOfferState.ComMessage = new tblComMessage();
                        modelOfferState.ComMessage.message = "Təsdiqləndi";
                        modelOfferState.ComMessage.fromUserID = (long)UserId;
                        modelOfferState.ComMessage.fromUserIDSpecified = true;
                        modelOfferState.ComMessage.toUserID = modelOfferState.OfferProduction.user_Id;
                        modelOfferState.ComMessage.toUserIDSpecified = true;
                        modelOfferState.ComMessage.Production_Id = modelOfferState.OfferProduction.Id;
                        modelOfferState.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferState.EnumValue);
                        modelOfferState.ComMessage.Production_type_eV_Id = modelOfferState.EnumValue.Id;
                        modelOfferState.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelOfferState.ComMessage, out modelOfferState.ComMessage);
                    }
                }

                return RedirectToAction("Index", "OfferState", new { stateStatusEV = modelOfferState.EnumValueST.name });

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
                modelOfferState = new OfferStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferState.User);
                baseInput.userName = modelOfferState.User.Username;

                BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferState.OfferProduction);

                modelOfferState.Id = modelOfferState.OfferProduction.Id;

                return View(modelOfferState);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(OfferStateViewModel model, FormCollection collection)
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

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reject", out model.EnumValueST);

                model.OfferProduction.state_eV_Id = model.EnumValueST.Id;
                model.OfferProduction.state_eV_IdSpecified = true;

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

                return RedirectToAction("Index", "OfferState", new { stateStatusEV = model.EnumValueST.name });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
