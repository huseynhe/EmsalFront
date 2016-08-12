using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Emsal.Utility.CustomObjects;
using System.Web.Security;
using Emsal.AdminUI.Infrastructure;

namespace Emsal.AdminUI.Controllers
{
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class AnnouncementController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private AnnouncementViewModel modelAnnouncement;

        public ActionResult Index()
        {
            baseInput = new BaseInput();
            modelAnnouncement = new AnnouncementViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelAnnouncement.Admin);
            baseInput.userName = modelAnnouncement.Admin.Username;


            modelAnnouncement.DemanProductionGroupList = new List<DemanProductionGroup>();

            modelAnnouncement.startDate = Convert.ToDateTime("01.01.2016");
            modelAnnouncement.endDate = DateTime.Now.Date;

            DateTime edDate = (DateTime)modelAnnouncement.endDate;

            long stYear = edDate.Year;
            long edRub = Int32.Parse(String.Format("{0}", (edDate.Month + 2) / 3));


            BaseOutput envalyd = srv.WS_GetDemanProductionGroupList(baseInput, ((DateTime)modelAnnouncement.startDate).getInt64ShortDate(), true, ((DateTime)modelAnnouncement.endDate).getInt64ShortDate(), true, stYear, true, edRub, true, out modelAnnouncement.DemanProductionGroupArray);
            modelAnnouncement.DemanProductionGroupList = modelAnnouncement.DemanProductionGroupArray.ToList();

            return View(modelAnnouncement);
        }

        [HttpPost]
        public ActionResult Index(AnnouncementViewModel model)
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

            model.AnnouncementList = new List<tblAnnouncement>();

            DateTime edDate = (DateTime)model.endDate;

            long stYear = edDate.Year;
            long edRub = Int32.Parse(String.Format("{0}", (edDate.Month + 2) / 3));


            BaseOutput envalyd = srv.WS_GetDemanProductionGroupList(baseInput, ((DateTime)model.startDate).getInt64ShortDate(), true, ((DateTime)model.endDate).getInt64ShortDate(), true, stYear,true, edRub,true, out model.DemanProductionGroupArray);
            model.DemanProductionGroupList = model.DemanProductionGroupArray.ToList();

            if (model.approv == 1)
            {
                model.DemandProduction = new tblDemand_Production();
                model.DemandProduction.startDate = ((DateTime)model.startDate).getInt64ShortDate();
                model.DemandProduction.startDateSpecified = true;
                model.DemandProduction.endDate = ((DateTime)model.endDate).getInt64ShortDate();
                model.DemandProduction.endDateSpecified = true;

                BaseOutput ap = srv.WS_UpdateDemand_ProductionForStartAndEndDate(baseInput, model.DemandProduction, out model.DemandProductionArray);
                var i=0;
                foreach (var item in model.DemanProductionGroupList)
                {
                    model.Announcement = new tblAnnouncement();
                    model.Announcement.quantity = item.totalQuantity;
                    model.Announcement.quantitySpecified = true;
                    model.Announcement.quantity_type_Name = item.enumValueName;
                    model.Announcement.unit_price = item.unitPrice;
                    model.Announcement.unit_priceSpecified = true;
                    model.Announcement.product_id = item.productId;
                    model.Announcement.product_idSpecified = true;
                    model.Announcement.product_name = item.productName;

                    model.Announcement.startDate = ((DateTime)model.arrayStartDate[i]).getInt64ShortDate();
                    model.Announcement.startDateSpecified = true;
                    model.Announcement.endDate = ((DateTime)model.arrayEndDate[i]).getInt64ShortDate();
                    model.Announcement.endDateSpecified = true;

                    BaseOutput aa = srv.WS_AddAnnouncement(baseInput, model.Announcement, out model.AnnouncementOUT);
                    i=i+1;
                }



                return RedirectToAction("Approv", "Announcement");

            }
            else
            {
                return View(model);
            }
        }


        public ActionResult Approv(int? page)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            baseInput = new BaseInput();
            modelAnnouncement = new AnnouncementViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelAnnouncement.Admin);
            baseInput.userName = modelAnnouncement.Admin.Username;


            BaseOutput gpp = srv.WS_GetAnnouncements(baseInput, out modelAnnouncement.AnnouncementArray);
            modelAnnouncement.AnnouncementList = modelAnnouncement.AnnouncementArray.ToList();

            modelAnnouncement.Paging = modelAnnouncement.AnnouncementArray.ToPagedList(pageNumber, pageSize);

            return View(modelAnnouncement);
        }


        public ActionResult Delete(int Id)
        {
            baseInput = new BaseInput();
            modelAnnouncement = new AnnouncementViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelAnnouncement.Admin);
            baseInput.userName = modelAnnouncement.Admin.Username;

            BaseOutput ga = srv.WS_GetAnnouncementById(baseInput,Id, true, out modelAnnouncement.Announcement);

            BaseOutput da = srv.WS_DeleteAnnouncement(baseInput, modelAnnouncement.Announcement);

            return RedirectToAction("Approv", "Announcement");
        }

    }
}
