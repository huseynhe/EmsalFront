using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using Emsal.AdminUI.Infrastructure;
using Emsal.Utility.CustomObjects;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class DemandProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string sadminUnit;
        private static string sstatusEV;


        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private DemandProductionViewModel modelDemandProduction;

        public ActionResult Index(int? page, string statusEV = null, string productName = null, string userInfo = null, string adminUnit = null)
        {
            try
            {

                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());
                if (adminUnit != null)
                    adminUnit = StripTag.strSqlBlocker(adminUnit.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && userInfo == null && adminUnit == null)
                {
                    sproductName = null;
                    suserInfo = null;
                    sadminUnit = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (userInfo != null)
                    suserInfo = userInfo;
                if (adminUnit != null)
                    sadminUnit = adminUnit;
                if (statusEV != null)
                    sstatusEV = statusEV;

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstatusEV, out modelDemandProduction.EnumValue);

                BaseOutput gpp = srv.WS_GetDemandProductionDetailistForEValueId(baseInput, modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.ProductionDetailArray);

                modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelDemandProduction.EnumCategory.Id).ToList();

                if (sproductName != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.productName.ToLowerInvariant().Contains(sproductName)).ToList();
                }

                if (sadminUnit != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.foreignOrganization.name.ToLowerInvariant().Contains(sadminUnit)).ToList();
                }

                if (suserInfo != null)
                {
                    if (modelDemandProduction.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }

                    if (modelDemandProduction.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }

                    if (modelDemandProduction.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                    {
                        modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }
                    else
                    {
                        modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                    }
                }


                modelDemandProduction.Paging = modelDemandProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                {
                    modelDemandProduction.isMain = 0;
                }
                else
                {
                    modelDemandProduction.isMain = 1;
                }


                modelDemandProduction.statusEV = sstatusEV;
                modelDemandProduction.productName = sproductName;
                modelDemandProduction.userInfo = suserInfo;
                modelDemandProduction.adminUnit = sadminUnit;
                //return View(modelDemandProduction);

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelDemandProduction)
                   : View(modelDemandProduction);

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
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        modelDemandProduction.DemandProduction = new tblDemand_Production();

                        BaseOutput bouput = srv.WS_GetDemand_ProductionById(baseInput, ids[i], true, out modelDemandProduction.DemandProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValueST);

                        modelDemandProduction.DemandProduction.state_eV_Id = modelDemandProduction.EnumValueST.Id;
                        modelDemandProduction.DemandProduction.state_eV_IdSpecified = true;

                        BaseOutput ecout = srv.WS_UpdateDemand_Production(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProduction);

                        modelDemandProduction.ComMessage = new tblComMessage();
                        modelDemandProduction.ComMessage.message = "Təsdiqləndi";
                        modelDemandProduction.ComMessage.fromUserID = (long)UserId;
                        modelDemandProduction.ComMessage.fromUserIDSpecified = true;
                        modelDemandProduction.ComMessage.toUserID = modelDemandProduction.DemandProduction.user_Id;
                        modelDemandProduction.ComMessage.toUserIDSpecified = true;
                        modelDemandProduction.ComMessage.Production_Id = modelDemandProduction.DemandProduction.Id;
                        modelDemandProduction.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);
                        modelDemandProduction.ComMessage.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                        modelDemandProduction.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelDemandProduction.ComMessage, out modelDemandProduction.ComMessage);
                    }
                }

                return RedirectToAction("Index", "DemandProduction", new { statusEV = modelDemandProduction.EnumValueST.name });

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
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                BaseOutput bouput = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);
                modelDemandProduction.Id = modelDemandProduction.DemandProduction.Id;

                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(DemandProductionViewModel model, FormCollection collection)
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
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out model.Admin);
                baseInput.userName = model.Admin.Username;

                model.ConfirmationMessage = new tblConfirmationMessage();
                model.ConfirmationMessage.Message = model.message;

                BaseOutput pout = srv.WS_SendConfirmationMessage(baseInput, model.ConfirmationMessage);


                model.DemandProduction = new tblDemand_Production();

                BaseOutput bouput = srv.WS_GetDemand_ProductionById(baseInput, model.Id, true, out model.DemandProduction);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reject", out model.EnumValueST);

                model.DemandProduction.state_eV_Id = model.EnumValueST.Id;
                model.DemandProduction.state_eV_IdSpecified = true;

                BaseOutput ecout = srv.WS_UpdateDemand_Production(baseInput, model.DemandProduction, out model.DemandProduction);

                model.ComMessage = new tblComMessage();
                model.ComMessage.message = model.message;
                model.ComMessage.fromUserID = (long)UserId;
                model.ComMessage.fromUserIDSpecified = true;
                model.ComMessage.toUserID = model.DemandProduction.user_Id;
                model.ComMessage.toUserIDSpecified = true;
                model.ComMessage.Production_Id = model.DemandProduction.Id;
                model.ComMessage.Production_IdSpecified = true;
                BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "demand", out model.EnumValue);
                model.ComMessage.Production_type_eV_Id = model.EnumValue.Id;
                model.ComMessage.Production_type_eV_IdSpecified = true;

                BaseOutput acm = srv.WS_AddComMessage(baseInput, model.ComMessage, out model.ComMessage);

                return RedirectToAction("Index", "DemandProduction", new { statusEV = model.EnumValueST.name });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
    }
}
