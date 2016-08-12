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
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class RoleManagementController : Controller
    {
        //
        // GET: /RoleManagement/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        private BaseInput binput;
        public ActionResult Index(int? page, long?UserId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            Organisation modelUser = new Organisation();
            //get roles by name gelecek bura

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            BaseOutput bouput = srv.WS_GetUsers(binput, out modelUser.UserArray);
            modelUser.UserList = modelUser.UserArray.Where(x=>x.Username!=null).ToList();

            modelUser.Paging = modelUser.UserList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndex", modelUser)
                : View(modelUser);
        }

        public ActionResult Pages(int? page, long? UserId)
        {
            binput = new BaseInput();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            Organisation modelUser = new Organisation();


            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            BaseOutput bouput = srv.WS_GetAuthenticatedParts(binput, out modelUser.AuthenticatedPartArray);

            modelUser.AuthenticatedPartList = modelUser.AuthenticatedPartArray.ToList();

            BaseOutput privilegeOut = srv.WS_GetPrivilegedRoles(binput, out modelUser.PrivilegedRolesArray);

            BaseOutput roleOut = srv.WS_GetRoles(binput, out modelUser.UserRoleArray);

            modelUser.PagingParts = modelUser.AuthenticatedPartList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndividuals", modelUser)
                : View(modelUser);
        }

        public ActionResult SelectedUserRoles(long Id, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            modelUser.UserRoles = new List<tblRole>();
            BaseOutput userOut = srv.WS_GetUserById(binput, Id, true, out modelUser.User);

            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, Id, true, out modelUser.UserRolesArray);
            foreach (var item in modelUser.UserRolesArray)
            {
                BaseOutput roleOut = srv.WS_GetRoleById(binput, item.RoleId, true, out modelUser.Role);
                modelUser.UserRoles.Add(modelUser.Role);
            }
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return View("UserRoles", modelUser);
        }

        public ActionResult Delete(long userId,long roleId, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            BaseOutput userRoleOut = srv.WS_GetUserRolesByUserId(binput, userId, true, out modelUser.UserRolesArray);

            modelUser.UserRole = modelUser.UserRolesArray.Where(x => x.RoleId == roleId).FirstOrDefault();

            BaseOutput testOut = srv.WS_GetRolesNotOwnedByUser(binput, 1, true, out modelUser.UserRoleArray);

            BaseOutput deleteUserRole = srv.WS_DeleteUserRole(binput, modelUser.UserRole);

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return RedirectToAction("SelectedUserRoles", new { Id = userId});
        }

        public ActionResult AddUserRole(long userId, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput userOut = srv.WS_GetUserById(binput, userId, true, out modelUser.User);
            BaseOutput testOut = srv.WS_GetRolesNotOwnedByUser(binput, userId, true, out modelUser.UserRoleArray);

            modelUser.User.Id = userId;

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return View(modelUser);
        }

        [HttpPost]
        public ActionResult AddUserRole(long userId, Organisation form, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            modelUser.UserRole = new tblUserRole();

            var aRole = form.UserRoleType.Split('-');
            BaseOutput roleOut = srv.WS_GetRoleByName(binput, aRole[0], out modelUser.Role);
            modelUser.UserRole.RoleId = modelUser.Role == null ? 0 : modelUser.Role.Id;
            modelUser.UserRole.UserId = userId;
            modelUser.UserRole.RoleIdSpecified = true;
            modelUser.UserRole.UserIdSpecified = true;
            BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelUser.UserRole, out modelUser.UserRole);

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return RedirectToAction("SelectedUserRoles", new { Id = userId });
        }

        public ActionResult SelectedPageRoles(long Id, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            modelUser.PageRoles = new List<tblRole>();
            BaseOutput pageOut = srv.WS_GetAuthenticatedPartById(binput, Id, true, out modelUser.AuthenticatedPart);

            BaseOutput pageRolesOut = srv.WS_GetPrivilegedRolesByAuthenticatedPartId(binput, modelUser.AuthenticatedPart.ID, true, out modelUser.PrivilegedRolesArray);
            foreach (var item in modelUser.PrivilegedRolesArray)
            {
                BaseOutput roleOut = srv.WS_GetRoleById(binput, item.RoleID, true, out modelUser.Role);
                modelUser.PageRoles.Add(modelUser.Role);
            }

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            return View("PageRoles", modelUser);
        }

        public ActionResult DeletePageRole(long pageId, long roleId, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            BaseOutput pageRoleOut = srv.WS_GetPrivilegedRolesByAuthenticatedPartId(binput, pageId, true, out modelUser.PrivilegedRolesArray);

            modelUser.PrivilegedRole = modelUser.PrivilegedRolesArray.Where(x => x.RoleID == roleId).FirstOrDefault();


            BaseOutput privilegedRoleOut = srv.WS_DeletePrivilegedRole(binput, modelUser.PrivilegedRole);

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            return RedirectToAction("SelectedPageRoles", new { Id = pageId });
        }

        public ActionResult AddRoleToPage(long pageId, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();

            BaseOutput pageOut = srv.WS_GetAuthenticatedPartById(binput, pageId, true, out modelUser.AuthenticatedPart);
            BaseOutput privilegedOut = srv.WS_GetRolesNotAllowedInPage(binput, pageId, true, out modelUser.UserRoleArray);

            modelUser.AuthenticatedPart.ID = pageId;

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);
            return View(modelUser);
        }


        [HttpPost]
        public ActionResult AddRoleToPage(long pageId, Organisation form, long?UserId)
        {
            binput = new BaseInput();
            Organisation modelUser = new Organisation();
            modelUser.PrivilegedRole = new tblPrivilegedRole();

            var pageRole = form.UserRoleType.Split('-');

            BaseOutput roleOut = srv.WS_GetRoleByName(binput, pageRole[0], out modelUser.Role);
            modelUser.PrivilegedRole.RoleID = modelUser.Role == null ? 0 : modelUser.Role.Id;
            modelUser.PrivilegedRole.AuthenticatedPartID = pageId;
            modelUser.PrivilegedRole.RoleIDSpecified = true;
            modelUser.PrivilegedRole.AuthenticatedPartIDSpecified = true;
            BaseOutput addPrivilegedRole = srv.WS_AddPrivilegedRole(binput, modelUser.PrivilegedRole, out modelUser.PrivilegedRole);

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.Admin);

            return RedirectToAction("SelectedPageRoles", new { Id = pageId });
        }
    }
}
