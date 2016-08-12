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

namespace Emsal.AdminUI.Controllers
{
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class PotentialProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string sstatusEV;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private PotentialProductionViewModel modelPotentialProduction;

        public ActionResult Index(int? page, string statusEV = null, string productName = null, string userInfo = null)
        {
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

            BaseOutput gpp = srv.WS_GetPotensialProductionDetailistForEValueId(baseInput, modelPotentialProduction.EnumValue.Id,true, out modelPotentialProduction.ProductionDetailArray);

            modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelPotentialProduction.EnumCategory.Id).ToList();

            if (sproductName != null)
            {
                modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.productName.Contains(sproductName)).ToList();
            }

            if (suserInfo != null)
            {
                if (modelPotentialProduction.ProductionDetailList.Where(x => x.person.Name.Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.person.Name.Contains(suserInfo)).ToList();
                }

                if (modelPotentialProduction.ProductionDetailList.Where(x => x.person.Surname.Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.person.Surname.Contains(suserInfo)).ToList();
                }

                if (modelPotentialProduction.ProductionDetailList.Where(x => x.person.FatherName.Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.person.FatherName.Contains(suserInfo)).ToList();
                }
                else
                {
                    modelPotentialProduction.ProductionDetailList = modelPotentialProduction.ProductionDetailList.Where(x => x.person.Name.Contains(suserInfo)).ToList();
                }
            }

            modelPotentialProduction.PagingDetail = modelPotentialProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

            if (sstatusEV == "Yayinda")
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

         [HttpPost]
        public ActionResult Approv(int[] ids)
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
                    modelPotentialProduction.ComMessage.message="Təsdiqləndi";
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


         public ActionResult Edit(int id)
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

         [HttpPost]
         public ActionResult Edit(PotentialProductionViewModel model, FormCollection collection)
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
    }
}
