using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emsal.AdminUI.Infrastructure
{
    public class EmsalAdminAuthentication : AuthorizeAttribute
    {
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        UserViewModel modelUser = new UserViewModel();
        BaseInput binput = new BaseInput();

        public string AuthorizedAction { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            List<string> AccessLevels = new List<string>();
            binput = new BaseInput();

            BaseOutput partOut = srv.WS_GetAuthenticatedPartByName(binput, this.AuthorizedAction, out modelUser.AuthenticatedPart);
            //srv.WS_createDb();
            BaseOutput privilegedRolesOut = srv.WS_GetPrivilegedRolesByAuthenticatedPartId(binput, modelUser.AuthenticatedPart.ID, true, out modelUser.PrivilegedRolesArray);

            foreach (var item in modelUser.PrivilegedRolesArray)
            {
                BaseOutput roleOut = srv.WS_GetRoleById(binput, item.RoleID, true, out modelUser.Role);

                AccessLevels.Add(modelUser.Role.Name);
            }


            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, Convert.ToInt64(httpContext.User.Identity.Name), true, out modelUser.UserRoleArray);

            string privilegeLevels = string.Join("", GetUserRights(modelUser.UserRoleArray)); // Call another method to get rights of the user from DB

            bool passCard = false;

            foreach (var item in AccessLevels)
            {
                if (privilegeLevels.Contains(item))
                {
                    passCard = true;
                }
            }

            if (passCard)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private List<string> GetUserRights(tblUserRole[] UserRoleArray)
        {
            List<string> a = new List<string>();

            foreach (var item in UserRoleArray)
            {
                srv.WS_GetRoleById(binput, item.RoleId, true, out modelUser.Role);

                a.Add(modelUser.Role.Name);
            }

            return a;
        }
    }
}