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

    public class KTNController : Controller
    {
        //
        // GET: /KTN/

        BaseInput binput;
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        Organisation modelOrganisation;
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
            BaseOutput KTNsOut = srv.WS_GetPRM_KTNBranches(binput, out modelOrganisation.KTNBranchArray);
            modelOrganisation.KTNBranchList = modelOrganisation.KTNBranchArray.ToList();

            modelOrganisation.PagingKTN = modelOrganisation.KTNBranchList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndex", modelOrganisation)
                : View(modelOrganisation);
        }

        public ActionResult AddKTN(long? UserId)
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
        public ActionResult AddKTN(Organisation form)
        {

            binput = new BaseInput();

            if (CheckExistence(form))
            {
                modelOrganisation = new Organisation();
                modelOrganisation.KTNBranch = new tblPRM_KTNBranch();
                //modelOrganisation.FutureAddress = new tblAddress();
                modelOrganisation.ktn = new List<string>();
                foreach (var item in form.ktnar)
                {
                    modelOrganisation.ktn.Add(item);
                }
                //modelOrganisation.FutureAddress.adminUnit_Id = form.adId.LastOrDefault();
                //modelOrganisation.FutureAddress.adminUnit_IdSpecified = true;
                //modelOrganisation.FutureAddress.fullAddress = form.FullAddress;

                //BaseOutput addressOut = srv.WS_AddAddress(binput, modelOrganisation.FutureAddress, out modelOrganisation.FutureAddress);

                modelOrganisation.KTNBranch.Name = form.Name;
                srv.WS_AddPRM_KTNBranch(binput, modelOrganisation.KTNBranch, out modelOrganisation.KTNBranch);

                //add branch responsibilities
                foreach (var item in modelOrganisation.ktn)
                {
                    modelOrganisation.branchResponsibility = new tblBranchResponsibility();

                    BaseOutput AscEnumOut = srv.WS_GetEnumValueByName(binput, "KTN", out modelOrganisation.EnumValue);
                    modelOrganisation.branchResponsibility.branchType_eVId = modelOrganisation.EnumValue.Id;
                    modelOrganisation.branchResponsibility.branchType_eVIdSpecified = true;
                    modelOrganisation.branchResponsibility.adminUnitId = Convert.ToInt64(item);
                    modelOrganisation.branchResponsibility.adminUnitIdSpecified = true;
                    modelOrganisation.branchResponsibility.branchId = modelOrganisation.KTNBranch.Id;
                    modelOrganisation.branchResponsibility.branchIdSpecified = true;

                    BaseOutput addResp = srv.WS_AddBranchResponsibility(binput, modelOrganisation.branchResponsibility);
                }

                TempData["KTNAddSuccess"] = "info";

                return RedirectToAction("Index", "KTN");
            }
            else
            {
                TempData["KTNAddError"] = "Bu adda KTN sistemdə mövcuddur.";
                return RedirectToAction("AddKTN");
            }

        }

        public ActionResult DeleteKTN(long Id)
        {
            binput = new BaseInput();
            modelOrganisation = new Organisation();

            BaseOutput KTNout = srv.WS_GetPRM_KTNBranchById(binput, Id, true, out modelOrganisation.KTNBranch);

            BaseOutput deleteKTN = srv.WS_DeletePRM_KTNBranch(binput, modelOrganisation.KTNBranch);

            //delete branch responsibilities
            BaseOutput respOut = srv.WS_GetBranchResponsibilities(binput, out modelOrganisation.branchRespArray);
            modelOrganisation.branchRespArray = modelOrganisation.branchRespArray.Where(x => x.branchId == modelOrganisation.KTNBranch.Id && x.branchType_eVId == 52).ToArray();

            foreach (var item in modelOrganisation.branchRespArray)
            {
                BaseOutput deleteResp = srv.WS_DeleteBranchResponsibility(binput, item);
            }

            //delete ktn individuals
            BaseOutput ktnenumOUt = srv.WS_GetEnumValueByName(binput, "KTN", out modelOrganisation.EnumValue);
            BaseOutput KTnsersOut = srv.WS_GetUsersByUserType(binput, modelOrganisation.EnumValue.Id, true, out modelOrganisation.UserArray);
            modelOrganisation.UserArray = modelOrganisation.UserArray.Where(x => x.KTN_ID == modelOrganisation.KTNBranch.Id).ToArray();
            foreach (var ktnUser in modelOrganisation.UserArray)
            {
                ktnUser.KTN_ID = 0;

                BaseOutput updateKTN = srv.WS_UpdateUser(binput, ktnUser, out modelOrganisation.User);
            }

            return RedirectToAction("Index", "KTN");

        }

        public ActionResult EditKTN(long Id, long? UserId)
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

            BaseOutput KTNout = srv.WS_GetPRM_KTNBranchById(binput, Id, true, out modelOrganisation.KTNBranch);

            modelOrganisation.Name = modelOrganisation.KTNBranch.Name;
            modelOrganisation.branchResponsibility = new tblBranchResponsibility();

            BaseOutput branchessOUt = srv.WS_GetBranchResponsibilities(binput, out modelOrganisation.branchRespArray);
            modelOrganisation.branchRespArray = modelOrganisation.branchRespArray.Where(x => x.branchId == modelOrganisation.KTNBranch.Id & x.branchType_eVId == 52).ToArray();

            modelOrganisation.branchesIdArr = new long[modelOrganisation.branchRespArray.Length];

            for (int i = 0; i < modelOrganisation.branchRespArray.Length; i++)
            {
                modelOrganisation.branchesIdArr[i] = modelOrganisation.branchRespArray[i].adminUnitId;
            }


            BaseOutput adminUnitOut = srv.WS_GetAdminUnitsByParentId(binput, 1, true, out modelOrganisation.PRMAdminUnitArray);

            return View(modelOrganisation);
        }

        [HttpPost]
        public ActionResult EditKTN(long Id, Organisation form)
        {
            Session["arrONum"] = null;

            binput = new BaseInput();
            modelOrganisation = new Organisation();
            modelOrganisation.KTNBranch = new tblPRM_KTNBranch();
            modelOrganisation.FutureAddress = new tblAddress();

            BaseOutput KTNout = srv.WS_GetPRM_KTNBranchById(binput, Id, true, out modelOrganisation.KTNBranch);

            modelOrganisation.KTNBranch.Name = form.Name;

            BaseOutput updateKTN = srv.WS_UpdatePRM_KTNBranch(binput, modelOrganisation.KTNBranch);


            //update branch responsibilities

            BaseOutput branchessOUt = srv.WS_GetBranchResponsibilities(binput, out modelOrganisation.branchRespArray);
            modelOrganisation.branchRespArray = modelOrganisation.branchRespArray.Where(x => x.branchId == modelOrganisation.KTNBranch.Id & x.branchType_eVId == 52).ToArray();

            //delete deselected branches
            foreach (var item in modelOrganisation.branchRespArray)
            {
                int a = Array.IndexOf(form.ktnar, item.adminUnitId.ToString());
                if (a < 0)
                {
                    BaseOutput deleteResponsibilty = srv.WS_DeleteBranchResponsibility(binput, item);
                }
            }

            //add new selected branches
            foreach (var item in form.ktnar)
            {
                bool isselected = true;

                foreach (var itemm in modelOrganisation.branchRespArray)
                {
                    if (itemm.adminUnitId == Convert.ToInt64(item))
                    {
                        isselected = false;
                    }

                }

                if (isselected)
                {
                    modelOrganisation.branchResponsibility = new tblBranchResponsibility();
                    BaseOutput AscEnumOut = srv.WS_GetEnumValueByName(binput, "KTN", out modelOrganisation.EnumValue);
                    modelOrganisation.branchResponsibility.branchType_eVId = modelOrganisation.EnumValue.Id;
                    modelOrganisation.branchResponsibility.branchType_eVIdSpecified = true;
                    modelOrganisation.branchResponsibility.adminUnitId = Convert.ToInt64(item);
                    modelOrganisation.branchResponsibility.adminUnitIdSpecified = true;
                    modelOrganisation.branchResponsibility.branchId = modelOrganisation.KTNBranch.Id;
                    modelOrganisation.branchResponsibility.branchIdSpecified = true;

                    BaseOutput addResp = srv.WS_AddBranchResponsibility(binput, modelOrganisation.branchResponsibility);
                }
            }
            return RedirectToAction("Index", "KTN");
        }

        public bool CheckExistence(Organisation form)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput checkExistenseOut = srv.WS_GetPRM_KTNBranchByName(binput, form.Name, out modelUser.KTNBranch);

            if (modelUser.KTNBranch != null)
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
