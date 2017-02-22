using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emsal.AdminUI.Models;
using System.Web.Security;
using Emsal.AdminUI.Infrastructure;

namespace Emsal.AdminUI.Controllers
{

    public class AdminLoginController : Controller
    {
        //
        // GET: /Login/
        public static string transactionID;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private BaseInput binput;
        UserViewModel modelUser;
        //burda ticket yaradırıq
        public ActionResult CreateTicket(int ticketNum, string route, tblUser User, string returnUrl)
        {
            transactionID = Emsal.Utility.UtilityObjects.IOUtil.GetFunctionRequestID();
            //burda userData yerine ne yazırsan yaz elebele user.ID.Tosring qoymuşam
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(ticketNum,
                                                                             User.Id.ToString(),
                                                                             DateTime.Now,
                                                                             DateTime.Now.AddMinutes(30),
                                                                             true,
                                                                             User.Id.ToString(), //bura lazım olan datanı yaz bilersen meselen transaction id
                                                                             FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                    encTicket);
            Response.Cookies.Add(cookie);

            Session["rolId"] = "";
            string str = null;
            BaseOutput rolOut = srv.WS_GetUserRolesByUserId(binput, (long)User.Id, true, out modelUser.UserRoleArray);

            foreach (var item in modelUser.UserRoleArray)
            {
                str = str + "/" + item.RoleId + "/";
            }
            Session["rolId"] = str;

            return RedirectToRoute(route);

        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(AuthLogin form, string returnUrl)
        {
            binput = new BaseInput();

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            else
            {

                modelUser = new UserViewModel();
                //get the user from service by the username entered
                BaseOutput userOut = srv.WS_GetUserByUserName(binput, form.Username, out modelUser.User);
                //if the username entered matches a user enter
                if (modelUser.User != null)
                {
                    if (modelUser.User.Password != null)
                    {
                        bool verify = BCrypt.Net.BCrypt.Verify(form.Password, modelUser.User.Password);
                        if (verify)
                        {
                            BaseOutput userTypeOut = srv.WS_GetEnumValueById(binput, (long)modelUser.User.userType_eV_ID, true, out modelUser.EnumValue);

                            bool ifAdmin = false;
                            BaseOutput authUserRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRoleArray);
                            BaseOutput roleOut = srv.WS_GetRoleByName(binput, "admin", out modelUser.Role);
                            foreach (var item in modelUser.UserRoleArray)
                            {
                                if (item.RoleId == modelUser.Role.Id)
                                {
                                    ifAdmin = true;
                                }
                            }
                            if (ifAdmin)
                            {
                                return CreateTicket(1, "Admin", modelUser.User, returnUrl);
                            }
                           
                            else
                            {
                                return View(form);
                            }
                           
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Şifrə düzgün yazılmayıb");
                            return View(form);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Şifrə mövcud deyil");
                        return View(form);
                    }


                }
                else
                {
                    ModelState.AddModelError("Username", "İstifadəçi adı düzgün yazılmayıb");
                    return View(form);
                }
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }


    }

}
