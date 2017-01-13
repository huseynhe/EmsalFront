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
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class PotentialProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string sstatusEV;
        private static long sproductId;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private PotentialProductionViewModel modelPotentialProduction;

        public ActionResult Index(int? page, string statusEV = null, long productId = -1, string userInfo = null)
        {
            try
            {

                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productId == -1 && userInfo == null)
                {
                    sproductId = 0;
                    suserInfo = null;
                }

                if (productId >= 0)
                    sproductId = productId;
                if (userInfo != null)
                    suserInfo = userInfo;
                if (statusEV != null)
                    sstatusEV = statusEV;

                baseInput = new BaseInput();
                modelPotentialProduction = new PotentialProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialProduction.Admin);
                baseInput.userName = modelPotentialProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelPotentialProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstatusEV, out modelPotentialProduction.EnumValue);

                modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch = new GetDemandProductionDetailistForEValueIdSearch();

                modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch.state_eV_Id = modelPotentialProduction.EnumValue.Id;
                modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch.page = pageNumber;
                modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch.pageSize = pageSize;
                modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch.prodcutID = sproductId;
                modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch.Name = suserInfo;

                BaseOutput gpp = srv.WS_GetPotensialProductionDetailistForEValueId_OP(baseInput, modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch, out modelPotentialProduction.ProductionDetailArray);

                if (modelPotentialProduction.ProductionDetailArray == null)
                {
                    modelPotentialProduction.ProductionDetailList = new List<ProductionDetail>();
                }
                else
                {
                    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailArray.ToList();
                }

                //modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelPotentialProduction.EnumCategory.Id && x.person != null).ToList();

                //if (sproductName != null)
                //{
                //    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                //}

                //if (suserInfo != null)
                //{
                //    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                //}

                //modelPotentialProduction.PagingDetail = modelPotentialProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);


                BaseOutput gppc = srv.WS_GetPotensialProductionDetailistForEValueId_OPC(baseInput, modelPotentialProduction.GetDemandProductionDetailistForEValueIdSearch, out modelPotentialProduction.itemCount, out modelPotentialProduction.itemCountB);

                long[] aic = new long[modelPotentialProduction.itemCount];

                modelPotentialProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                    modelPotentialProduction.isMain = 0;
                else
                    modelPotentialProduction.isMain = 1;


                modelPotentialProduction.statusEV = sstatusEV;
                modelPotentialProduction.productName = sproductName;
                modelPotentialProduction.userInfo = suserInfo;

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelPotentialProduction)
                   : View(modelPotentialProduction);

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
                modelPotentialProduction = new PotentialProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialProduction.Admin);
                baseInput.userName = modelPotentialProduction.Admin.Username;

                modelPotentialProduction.PotentialProduction = new tblPotential_Production();

                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, ids[i], true, out modelPotentialProduction.PotentialProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelPotentialProduction.EnumValueST);

                        modelPotentialProduction.PotentialProduction.state_eV_Id = modelPotentialProduction.EnumValueST.Id;
                        modelPotentialProduction.PotentialProduction.state_eV_IdSpecified = true;

                        BaseOutput ecout = srv.WS_UpdatePotential_Production(baseInput, modelPotentialProduction.PotentialProduction, out modelPotentialProduction.PotentialProduction);

                        modelPotentialProduction.ComMessage = new tblComMessage();
                        modelPotentialProduction.ComMessage.message = "Təsdiqləndi";
                        modelPotentialProduction.ComMessage.fromUserID = (long)UserId;
                        modelPotentialProduction.ComMessage.fromUserIDSpecified = true;
                        modelPotentialProduction.ComMessage.toUserID = modelPotentialProduction.PotentialProduction.user_Id;
                        modelPotentialProduction.ComMessage.toUserIDSpecified = true;
                        modelPotentialProduction.ComMessage.Production_Id = modelPotentialProduction.PotentialProduction.Id;
                        modelPotentialProduction.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialProduction.EnumValue);
                        modelPotentialProduction.ComMessage.Production_type_eV_Id = modelPotentialProduction.EnumValue.Id;
                        modelPotentialProduction.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelPotentialProduction.ComMessage, out modelPotentialProduction.ComMessage);
                    }
                }

                return RedirectToAction("Index", "PotentialProduction", new { statusEV = modelPotentialProduction.EnumValueST.name });

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
                modelPotentialProduction = new PotentialProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialProduction.Admin);
                baseInput.userName = modelPotentialProduction.Admin.Username;


                BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, id, true, out modelPotentialProduction.PotentialProduction);
                modelPotentialProduction.Id = modelPotentialProduction.PotentialProduction.Id;

                return View(modelPotentialProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(PotentialProductionViewModel model, FormCollection collection)
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

                return RedirectToAction("Index", "PotentialProduction", new { statusEV = model.EnumValueST.name });

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
                modelPotentialProduction = new PotentialProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialProduction.Admin);
                baseInput.userName = modelPotentialProduction.Admin.Username;

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelPotentialProduction.ProductCatalogDetailArray);

                if (modelPotentialProduction.ProductCatalogDetailArray == null)
                {
                    modelPotentialProduction.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelPotentialProduction.ProductCatalogDetailList = modelPotentialProduction.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelPotentialProduction.actionName = actionName;
                return View(modelPotentialProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
    }
}
