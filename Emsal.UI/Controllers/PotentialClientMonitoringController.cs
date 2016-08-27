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

namespace Emsal.UI.Controllers
{
        [EmsalAuthorization(AuthorizedAction = ActionName.PotentialClientMonitoring)]
    public class PotentialClientMonitoringController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string suserInfo;
        private static string smonitoringStatusEV;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private PotentialClientMonitoringViewModel modelPotentialClientMonitoring;

        public ActionResult Index(int? page, string monitoringStatusEV = null, string productName = null, string userInfo = null)
        {
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
            modelPotentialClientMonitoring = new PotentialClientMonitoringViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientMonitoring.User);
            baseInput.userName = modelPotentialClientMonitoring.User.Username;

            BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelPotentialClientMonitoring.EnumCategory);

            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, smonitoringStatusEV, out modelPotentialClientMonitoring.EnumValue);

            BaseOutput gpp = srv.WS_GetPotensialProductionDetailistForMonitoringEVId(baseInput, (long)UserId, true, modelPotentialClientMonitoring.EnumValue.Id, true, out modelPotentialClientMonitoring.ProductionDetailArray);

            modelPotentialClientMonitoring.ProductionDetailList = modelPotentialClientMonitoring.ProductionDetailArray.Where(x => x.enumCategoryId == modelPotentialClientMonitoring.EnumCategory.Id).ToList();

            if (sproductName != null)
            {
                modelPotentialClientMonitoring.ProductionDetailList = modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.productName.Contains(sproductName)).ToList();
            }

            if (suserInfo != null)
            {
                if (modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.Name.Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelPotentialClientMonitoring.ProductionDetailList = modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.Name.Contains(suserInfo)).ToList();
                }

                if (modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.Surname.Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelPotentialClientMonitoring.ProductionDetailList = modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.Surname.Contains(suserInfo)).ToList();
                }

                if (modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.FatherName.Contains(suserInfo)).ToList().Count() > 0)
                {
                    modelPotentialClientMonitoring.ProductionDetailList = modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.FatherName.Contains(suserInfo)).ToList();
                }
                else
                {
                    modelPotentialClientMonitoring.ProductionDetailList = modelPotentialClientMonitoring.ProductionDetailList.Where(x => x.person.Name.Contains(suserInfo)).ToList();
                }
            }

            modelPotentialClientMonitoring.PagingDetail = modelPotentialClientMonitoring.ProductionDetailList.ToPagedList(pageNumber, pageSize);

            if (smonitoringStatusEV == "new")
                modelPotentialClientMonitoring.isMain = 0;
            else
                modelPotentialClientMonitoring.isMain = 1;


            modelPotentialClientMonitoring.monitoringStatusEV = smonitoringStatusEV;
            modelPotentialClientMonitoring.productName = sproductName;
            modelPotentialClientMonitoring.userInfo = suserInfo;

            return Request.IsAjaxRequest()
               ? (ActionResult)PartialView("PartialIndex", modelPotentialClientMonitoring)
               : View(modelPotentialClientMonitoring);

        }


        public ActionResult Approv(int[] ids)
        {
            baseInput = new BaseInput();
            modelPotentialClientMonitoring = new PotentialClientMonitoringViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientMonitoring.User);
            baseInput.userName = modelPotentialClientMonitoring.User.Username;

            modelPotentialClientMonitoring.PotentialProduction = new tblPotential_Production();

            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, ids[i], true, out modelPotentialClientMonitoring.PotentialProduction);

                    BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelPotentialClientMonitoring.EnumValueST);

                    modelPotentialClientMonitoring.PotentialProduction.state_eV_Id = modelPotentialClientMonitoring.EnumValueST.Id;
                    modelPotentialClientMonitoring.PotentialProduction.state_eV_IdSpecified = true;

                    BaseOutput ecout = srv.WS_UpdatePotential_Production(baseInput, modelPotentialClientMonitoring.PotentialProduction, out modelPotentialClientMonitoring.PotentialProduction);

                    modelPotentialClientMonitoring.ComMessage = new tblComMessage();
                    modelPotentialClientMonitoring.ComMessage.message = "Təsdiqləndi";
                    modelPotentialClientMonitoring.ComMessage.fromUserID = (long)UserId;
                    modelPotentialClientMonitoring.ComMessage.fromUserIDSpecified = true;
                    modelPotentialClientMonitoring.ComMessage.toUserID = modelPotentialClientMonitoring.PotentialProduction.user_Id;
                    modelPotentialClientMonitoring.ComMessage.toUserIDSpecified = true;
                    modelPotentialClientMonitoring.ComMessage.Production_Id = modelPotentialClientMonitoring.PotentialProduction.Id;
                    modelPotentialClientMonitoring.ComMessage.Production_IdSpecified = true;
                    BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelPotentialClientMonitoring.EnumValue);
                    modelPotentialClientMonitoring.ComMessage.Production_type_eV_Id = modelPotentialClientMonitoring.EnumValue.Id;
                    modelPotentialClientMonitoring.ComMessage.Production_type_eV_IdSpecified = true;

                    BaseOutput acm = srv.WS_AddComMessage(baseInput, modelPotentialClientMonitoring.ComMessage, out modelPotentialClientMonitoring.ComMessage);
                }
            }

            return RedirectToAction("Index", "PotentialClientMonitoring", new { monitoringStatusEV = modelPotentialClientMonitoring.EnumValueST.name });
        }


        public ActionResult Edit(int id)
        {
            baseInput = new BaseInput();
            modelPotentialClientMonitoring = new PotentialClientMonitoringViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientMonitoring.User);
            baseInput.userName = modelPotentialClientMonitoring.User.Username;


            BaseOutput bouput = srv.WS_GetPotential_ProductionById(baseInput, id, true, out modelPotentialClientMonitoring.PotentialProduction);
            modelPotentialClientMonitoring.Id = modelPotentialClientMonitoring.PotentialProduction.Id;

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "State", out modelPotentialClientMonitoring.EnumCategory);

            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelPotentialClientMonitoring.EnumCategory.Id, true, out modelPotentialClientMonitoring.EnumValueArray);
            modelPotentialClientMonitoring.EnumValueList = modelPotentialClientMonitoring.EnumValueArray.ToList();

            return View(modelPotentialClientMonitoring);
        }

        [HttpPost]
        public ActionResult Edit(PotentialClientMonitoringViewModel model, FormCollection collection)
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

            //BaseOutput envalyd = srv.WS_GetEnumValueById(baseInput, model.monitoringStatusEVId, true, out model.EnumValueST);
            BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reedited", out model.EnumValueST);

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


            return RedirectToAction("Index", "PotentialClientMonitoring", new { monitoringStatusEV = model.EnumValueST.name });
        }
    }
}
