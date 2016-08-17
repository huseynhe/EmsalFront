using Emsal.AdminUI.Infrastructure;
using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class AdminUnitController : Controller
    {
        //
        // GET: /AdminUnit/
        BaseInput baseInput;
        AdminUnitViewModel modelAdminUnit;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        public ActionResult Index()
        {
            //srv.WS_createDb();

            baseInput = new BaseInput();
            modelAdminUnit = new AdminUnitViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelAdminUnit.Admin);
            baseInput.userName = modelAdminUnit.Admin.Username;

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(baseInput, out modelAdminUnit.AdminUnitArray);
            modelAdminUnit.AdminUnitList = modelAdminUnit.AdminUnitArray.ToList();

            return View(modelAdminUnit);
        }


        public ActionResult AddAdminUnit(
         string adminUnitName,
         string adminUnitDescription,
         int adminUnitParentID = 0,
         int enumValueID = 0,
         int status = 1
         )
        {
            baseInput = new BaseInput();

            modelAdminUnit = new AdminUnitViewModel();
            modelAdminUnit.AdminUnit = new tblPRM_AdminUnit();

            modelAdminUnit.AdminUnit.Name = adminUnitName;
            modelAdminUnit.AdminUnit.Description = adminUnitDescription;
            modelAdminUnit.AdminUnit.ParentIDSpecified = true;
            modelAdminUnit.AdminUnit.ParentID = (int)adminUnitParentID;
            modelAdminUnit.AdminUnit.Status = status;
            modelAdminUnit.AdminUnit.StatusSpecified = true;
            BaseOutput pout = srv.WS_AddPRM_AdminUnit(baseInput, modelAdminUnit.AdminUnit);

            return RedirectToAction("Index");
        }


        //public JsonResult GetAdminUnitsByParentId(int parentId)
        //{
        //    baseInput = new BaseInput();

        //    modelAdminUnit = new AdminUnitViewModel();
        //    BaseOutput bouput = srv.WS_GetAdminUnitsByParentId(baseInput, parentId, true, out modelAdminUnit.AdminUnitArray);

        //    return Json(modelAdminUnit.AdminUnitArray, JsonRequestBehavior.AllowGet);

        //}

        public ActionResult UpdateAdminUnit(
           Int64 Id,
           string Name,
           string Description,
           Int64  ParentID = 0,
           int EnumValueId = 0,
           int status = 1,
           bool IdSpecified = true
           )
        {
            baseInput = new BaseInput();

            modelAdminUnit = new AdminUnitViewModel();

            modelAdminUnit.AdminUnit = new tblPRM_AdminUnit();

            modelAdminUnit.AdminUnit.Id = Id;

            modelAdminUnit.AdminUnit.Name = Name;
            modelAdminUnit.AdminUnit.Description = Description;
            modelAdminUnit.AdminUnit.ParentIDSpecified = true;
            modelAdminUnit.AdminUnit.IdSpecified = true;
            modelAdminUnit.AdminUnit.EnumValueIDSpecified = true;
            modelAdminUnit.AdminUnit.ParentID = ParentID;
            modelAdminUnit.AdminUnit.EnumValueID = EnumValueId;
            modelAdminUnit.AdminUnit.Status = status;
            modelAdminUnit.AdminUnit.StatusSpecified = true;
            BaseOutput pout = srv.WS_UpdatePRM_AdminUnit(baseInput, modelAdminUnit.AdminUnit);


            return RedirectToAction("Index", "AdminUnit");
        }



    }
}
