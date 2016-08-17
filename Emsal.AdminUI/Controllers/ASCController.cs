using Emsal.AdminUI.Infrastructure;
using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class ASCController : Controller
    {
        //
        // GET: /Asc/
        BaseInput binput;
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        Organisation modelOrganisation = new Organisation();
        public ActionResult Index(int? page, long? UserId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            modelOrganisation = new Organisation();
            //get roles by name gelecek bura
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelOrganisation.Admin);
            BaseOutput ascsOut = srv.WS_GetPRM_ASCBranches(binput, out modelOrganisation.ASCBranchArray);
            modelOrganisation.ASCBranchList = modelOrganisation.ASCBranchArray.ToList();

            modelOrganisation.PagingASC = modelOrganisation.ASCBranchList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndex", modelOrganisation)
                : View(modelOrganisation);
        }


        public ActionResult AddASC(long?UserId)
        {
            Session["arrONum"] = null;

            modelOrganisation = new Organisation();

            BaseOutput roles = srv.WS_GetRoles(binput, out modelOrganisation.UserRoleArray);

            modelOrganisation.UserRoleList = modelOrganisation.UserRoleArray.ToList();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelOrganisation.Admin);
            BaseOutput adminUnitOut = srv.WS_GetAdminUnitsByParentId(binput, 1, true, out modelOrganisation.PRMAdminUnitArray);

            return View(modelOrganisation);

        }

        [HttpPost]
        public ActionResult AddASC(Organisation form, long?UserId)
        {

            binput = new BaseInput();

            if (CheckExistence(form))
            {
                modelOrganisation = new Organisation();
                modelOrganisation.asc = new List<string>();

                modelOrganisation.ASCBranch = new tblPRM_ASCBranch();
                modelOrganisation.FutureAddress = new tblAddress();

                //modelOrganisation.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();
                //modelOrganisation.FutureAddress.adminUnit_IdSpecified = true;
                //modelOrganisation.FutureAddress.fullAddress = form.FullAddress;

                //BaseOutput addressOut = srv.WS_AddAddress(binput, modelOrganisation.FutureAddress, out modelOrganisation.FutureAddress);

                modelOrganisation.ASCBranch.Name = form.Name;
                //modelOrganisation.ASCBranch.address_Id = modelOrganisation.FutureAddress.Id;
                //modelOrganisation.ASCBranch.address_IdSpecified = true;
                srv.WS_AddPRM_ASCBranch(binput, modelOrganisation.ASCBranch, out modelOrganisation.ASCBranch);

                //add branch responsibilities

                foreach (var item in form.ascar)
                {
                    modelOrganisation.asc.Add(item);
                }

                foreach (var item in modelOrganisation.asc)
                {
                    modelOrganisation.branchResponsibility = new tblBranchResponsibility();
                    BaseOutput AscEnumOut = srv.WS_GetEnumValueByName(binput, "ASC", out modelOrganisation.EnumValue);
                    modelOrganisation.branchResponsibility.branchType_eVId = modelOrganisation.EnumValue.Id;
                    modelOrganisation.branchResponsibility.branchType_eVIdSpecified = true;
                    modelOrganisation.branchResponsibility.adminUnitId = Convert.ToInt64(item);
                    modelOrganisation.branchResponsibility.adminUnitIdSpecified = true;
                    modelOrganisation.branchResponsibility.branchId = modelOrganisation.ASCBranch.Id;
                    modelOrganisation.branchResponsibility.branchIdSpecified = true;

                    BaseOutput addResp = srv.WS_AddBranchResponsibility(binput, modelOrganisation.branchResponsibility);

                }


                TempData["ASCAddSuccess"] = "info";
                return RedirectToAction("Index", "ASC");
            }
            else
            {
                TempData["ASCAddError"] = "Bu adda ASC sistemdə mövcuddur.";
                return RedirectToAction("AddASC");
            }

        }

        public ActionResult DeleteASC(long Id)
        {
            binput = new BaseInput();
            modelOrganisation = new Organisation();

            BaseOutput ASCout = srv.WS_GetPRM_ASCBranchById(binput, Id, true, out modelOrganisation.ASCBranch);

            BaseOutput deleteAsc = srv.WS_DeletePRM_ASCBranch(binput, modelOrganisation.ASCBranch);

            return RedirectToAction("Index", "ASC");

        }

        public ActionResult EditASC(long Id, long?UserId)
        {
            modelOrganisation = new Organisation();

            BaseOutput roles = srv.WS_GetRoles(binput, out modelOrganisation.UserRoleArray);

            modelOrganisation.UserRoleList = modelOrganisation.UserRoleArray.ToList();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelOrganisation.Admin);

            BaseOutput ASCout = srv.WS_GetPRM_ASCBranchById(binput, Id, true, out modelOrganisation.ASCBranch);

            modelOrganisation.Name = modelOrganisation.ASCBranch.Name;
            return View(modelOrganisation);
        }

        [HttpPost]
        public ActionResult EditASC(long Id, Organisation form)
        {
            Session["arrONum"] = null;

            binput = new BaseInput();
            modelOrganisation = new Organisation();
            modelOrganisation.ASCBranch = new tblPRM_ASCBranch();
            modelOrganisation.FutureAddress = new tblAddress();

            BaseOutput ASCout = srv.WS_GetPRM_ASCBranchById(binput, Id, true, out modelOrganisation.ASCBranch);

            modelOrganisation.ASCBranch.Name = form.Name;

            BaseOutput updateAsc = srv.WS_UpdatePRM_ASCBranch(binput, modelOrganisation.ASCBranch);

            return RedirectToAction("Index", "ASC");
        }


        public bool CheckExistence(Organisation form)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput checkExistenseOut = srv.WS_GetPRM_ASCBranchByName(binput, form.Name, out modelUser.ASCBranch);

            if (modelUser.ASCBranch != null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }
}
