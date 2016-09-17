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
    [EmsalAuthorization(AuthorizedAction = ActionName.PotentialClientState)]

    public class PotentialClientStateController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string sstateStatusEV;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private PotentialClientStateViewModel modelPotentialClientState;


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
                modelPotentialClientState = new PotentialClientStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientState.User);
                baseInput.userName = modelPotentialClientState.User.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelPotentialClientState.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstateStatusEV, out modelPotentialClientState.EnumValue);

                BaseOutput gpp = srv.WS_GetPotensialProductionDetailistForStateEVId(baseInput, (long)UserId, true, modelPotentialClientState.EnumValue.Id, true, out modelPotentialClientState.ProductionDetailArray);

                modelPotentialClientState.ProductionDetailList = modelPotentialClientState.ProductionDetailArray.Where(x => x.enumCategoryId == modelPotentialClientState.EnumCategory.Id).ToList();

                if (sproductName != null)
                {
                    modelPotentialClientState.ProductionDetailList = modelPotentialClientState.ProductionDetailList.Where(x => x.productName.ToLowerInvariant().Contains(sproductName)).ToList();
                }

                if (suserInfo != null)
                {
                    if (modelPotentialClientState.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelPotentialClientState.ProductionDetailList = modelPotentialClientState.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }

                    if (modelPotentialClientState.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelPotentialClientState.ProductionDetailList = modelPotentialClientState.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }

                    if (modelPotentialClientState.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelPotentialClientState.ProductionDetailList = modelPotentialClientState.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }
                    else
                    {
                        modelPotentialClientState.ProductionDetailList = modelPotentialClientState.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }
                }

                modelPotentialClientState.PagingDetail = modelPotentialClientState.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                if (sstateStatusEV == "Tesdiqlenen" || sstateStatusEV == "tesdiqlenen")
                    modelPotentialClientState.isMain = 0;
                else
                    modelPotentialClientState.isMain = 1;


                modelPotentialClientState.stateStatusEV = sstateStatusEV;
                modelPotentialClientState.productName = sproductName;
                modelPotentialClientState.userInfo = suserInfo;

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelPotentialClientState)
                   : View(modelPotentialClientState);

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
                modelPotentialClientState = new PotentialClientStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientState.User);
                baseInput.userName = modelPotentialClientState.User.Username;

                modelPotentialClientState.PotentialProduction = new tblPotential_Production();

                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, ids[i], true, out modelPotentialClientState.PotentialProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelPotentialClientState.EnumValueST);

                        modelPotentialClientState.PotentialProduction.state_eV_Id = modelPotentialClientState.EnumValueST.Id;
                        modelPotentialClientState.PotentialProduction.state_eV_IdSpecified = true;

                        BaseOutput ecout = srv.WS_UpdatePotential_Production(baseInput, modelPotentialClientState.PotentialProduction, out modelPotentialClientState.PotentialProduction);

                        modelPotentialClientState.ComMessage = new tblComMessage();
                        modelPotentialClientState.ComMessage.message = "Təsdiqləndi";
                        modelPotentialClientState.ComMessage.fromUserID = (long)UserId;
                        modelPotentialClientState.ComMessage.fromUserIDSpecified = true;
                        modelPotentialClientState.ComMessage.toUserID = modelPotentialClientState.PotentialProduction.user_Id;
                        modelPotentialClientState.ComMessage.toUserIDSpecified = true;
                        modelPotentialClientState.ComMessage.Production_Id = modelPotentialClientState.PotentialProduction.Id;
                        modelPotentialClientState.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialClientState.EnumValue);
                        modelPotentialClientState.ComMessage.Production_type_eV_Id = modelPotentialClientState.EnumValue.Id;
                        modelPotentialClientState.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelPotentialClientState.ComMessage, out modelPotentialClientState.ComMessage);
                    }
                }

                return RedirectToAction("Index", "PotentialClientState", new { stateStatusEV = modelPotentialClientState.EnumValueST.name });

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
                modelPotentialClientState = new PotentialClientStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientState.User);
                baseInput.userName = modelPotentialClientState.User.Username;


                BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, id, true, out modelPotentialClientState.PotentialProduction);
                modelPotentialClientState.Id = modelPotentialClientState.PotentialProduction.Id;

                return View(modelPotentialClientState);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(PotentialClientStateViewModel model, FormCollection collection)
        {
            try
            {

                baseInput = new BaseInput();

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

                model.PotentialProduction = new tblPotential_Production();

                BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, model.Id, true, out model.PotentialProduction);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reject", out model.EnumValueST);

                model.PotentialProduction.state_eV_Id = model.EnumValueST.Id;
                model.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput ecout = srv.WS_UpdatePotential_Production(baseInput, model.PotentialProduction, out model.PotentialProduction);

                model.ComMessage = new tblComMessage();
                model.ComMessage.message = model.message;
                model.ComMessage.fromUserID = (long)UserId;
                model.ComMessage.fromUserIDSpecified = true;
                model.ComMessage.toUserID = model.PotentialProduction.user_Id;
                model.ComMessage.toUserIDSpecified = true;
                model.ComMessage.Production_Id = model.PotentialProduction.Id;
                model.ComMessage.Production_IdSpecified = true;
                BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out model.EnumValue);
                model.ComMessage.Production_type_eV_Id = model.EnumValue.Id;
                model.ComMessage.Production_type_eV_IdSpecified = true;

                BaseOutput acm = srv.WS_AddComMessage(baseInput, model.ComMessage, out model.ComMessage);


                return RedirectToAction("Index", "PotentialClientState", new { stateStatusEV = model.EnumValueST.name });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
