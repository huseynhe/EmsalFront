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
    public class OfferProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string sstatusEV;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private OfferProductionViewModel modelOfferProduction;

        public ActionResult Index(int? page, string statusEV = null, string productName = null, string userInfo = null)
        {
            if (statusEV != null)
                statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
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
            if (statusEV != null)
                sstatusEV = statusEV;

            baseInput = new BaseInput();
            modelOfferProduction = new OfferProductionViewModel();


            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
            baseInput.userName = modelOfferProduction.Admin.Username;


            BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferProduction.EnumCategory);

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstatusEV, out modelOfferProduction.EnumValue);

            BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.ProductionDetailArray);

            modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferProduction.EnumCategory.Id).ToList();

            if (sproductName != null)
            {
                modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.productName.ToLowerInvariant().Contains(sproductName)).ToList();
            }

            if (suserInfo != null)
            {
                if (modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                }

                if (modelOfferProduction.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Surname.ToLowerInvariant().Contains(suserInfo)).ToList();
                }

                if (modelOfferProduction.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.FatherName.ToLowerInvariant().Contains(suserInfo)).ToList();
                }
                else
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLowerInvariant().Contains(suserInfo)).ToList();
                }
            }


            modelOfferProduction.Paging = modelOfferProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

            if (sstatusEV == "Yayinda")
                modelOfferProduction.isMain = 0;
            else
                modelOfferProduction.isMain = 1;


            modelOfferProduction.statusEV = sstatusEV;
            modelOfferProduction.productName = sproductName;
            modelOfferProduction.userInfo = suserInfo;
            //return View(modelDemandProduction);

            return Request.IsAjaxRequest()
               ? (ActionResult)PartialView("PartialIndex", modelOfferProduction)
               : View(modelOfferProduction);
        }


        [HttpPost]
        public ActionResult Approv(int[] ids)
        {
            baseInput = new BaseInput();
            modelOfferProduction = new OfferProductionViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
            baseInput.userName = modelOfferProduction.Admin.Username;

            //Array arrid = ids.Split(',');
            //long id = 0;
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {                    
                    modelOfferProduction.OfferProduction = new tblOffer_Production();

                    BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, ids[i], true, out modelOfferProduction.OfferProduction);

                    BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferProduction.EnumValueST);

                    modelOfferProduction.OfferProduction.state_eV_Id = modelOfferProduction.EnumValueST.Id;
                    modelOfferProduction.OfferProduction.state_eV_IdSpecified = true;

                    BaseOutput ecout = srv.WS_UpdateOffer_Production(baseInput, modelOfferProduction.OfferProduction, out modelOfferProduction.OfferProduction);

                    modelOfferProduction.ComMessage = new tblComMessage();
                    modelOfferProduction.ComMessage.message = "Təsdiqləndi";
                    modelOfferProduction.ComMessage.fromUserID = (long)UserId;
                    modelOfferProduction.ComMessage.fromUserIDSpecified = true;
                    modelOfferProduction.ComMessage.toUserID = modelOfferProduction.OfferProduction.user_Id;
                    modelOfferProduction.ComMessage.toUserIDSpecified = true;
                    modelOfferProduction.ComMessage.Production_Id = modelOfferProduction.OfferProduction.Id;
                    modelOfferProduction.ComMessage.Production_IdSpecified = true;
                    BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);
                    modelOfferProduction.ComMessage.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                    modelOfferProduction.ComMessage.Production_type_eV_IdSpecified = true;

                    BaseOutput acm = srv.WS_AddComMessage(baseInput, modelOfferProduction.ComMessage, out modelOfferProduction.ComMessage);
                }
            }

            return RedirectToAction("Index", "OfferProduction", new { statusEV = modelOfferProduction.EnumValueST.name });
        }

        public ActionResult Edit(int id)
        {
            baseInput = new BaseInput();
            modelOfferProduction = new OfferProductionViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
            baseInput.userName = modelOfferProduction.Admin.Username;

            BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferProduction.OfferProduction);

            modelOfferProduction.Id = modelOfferProduction.OfferProduction.Id;

            return View(modelOfferProduction);
        }

        [HttpPost]
        public ActionResult Edit(OfferProductionViewModel model, FormCollection collection)
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
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out model.Admin);
            baseInput.userName = model.Admin.Username;

            model.ConfirmationMessage.Message = model.message;

            BaseOutput pout = srv.WS_SendConfirmationMessage(baseInput, model.ConfirmationMessage);


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

            return RedirectToAction("Index", "OfferProduction", new { statusEV = model.EnumValueST.name });
        }

    }
}
