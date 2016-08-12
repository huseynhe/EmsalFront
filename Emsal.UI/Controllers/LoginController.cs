using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    public class LoginController : Controller
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
            //if (route == "Admin")
            //{
            //    return Redirect("http://localhost:57242/");
            //}
            //else
            //{
                return RedirectToRoute(route);
            //}
           
        }

        public ActionResult Index(string finvoen = null, string type = null)
        {
                      binput = new BaseInput();

                      AuthLogin authLogin = new AuthLogin();

                authLogin.finvoen = finvoen;
                authLogin.type = type;

                return View(authLogin);
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
                            bool ifASC = false; 
                            //bool ifAdmin = false;
                            bool ifAsan = false;
                            bool ifKTN = false;

                            BaseOutput roleOut = srv.WS_GetRoleByName(binput, "admin", out modelUser.Role);
                            long adminRoleID = modelUser.Role.Id;

                            BaseOutput asanRoleOut = srv.WS_GetRoleByName(binput, "asan", out modelUser.Role);
                            long asanRoleID = modelUser.Role.Id;


                            BaseOutput ascRoleOut = srv.WS_GetRoleByName(binput, "ascUser", out modelUser.Role);
                            long ascRoleID = modelUser.Role.Id;

                            BaseOutput ktnRoleOut = srv.WS_GetRoleByName(binput, "ktnUser", out modelUser.Role);
                            long ktnRoleID = modelUser.Role.Id;

                            BaseOutput authUserRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRoleArray);

                            foreach (var item in modelUser.UserRoleArray)
                            {
                                //if (item.RoleId == adminRoleID)
                                //{
                                //    ifAdmin = true;
                                //}
                                if(item.RoleId == asanRoleID)
                                {
                                    ifAsan = true;
                                }
                                else if(item.RoleId == ascRoleID)
                                {
                                    ifASC = true;
                                }
                                else if(item.RoleId == ktnRoleID)
                                {
                                    ifKTN = true;
                                }
                            }
                            //if (ifAdmin)
                            //{
                            //    return CreateTicket(1, "Admin", modelUser.User, returnUrl);
                            //}
                            if (ifAsan)
                            {
                                return CreateTicket(1, "AsanXidmetSpecial", modelUser.User, returnUrl);
                            }
                            else
                            {
                                if (modelUser.EnumValue.name == "fizikişexs")
                                {
                                    return CreateTicket(1, "Special", modelUser.User, returnUrl);
                                }
                                else if (ifASC)
                                {
                                    return CreateTicket(1, "ASCSpecial", modelUser.User, returnUrl);
                                }
                                else if (ifKTN)
                                {
                                    return CreateTicket(1, "KTNSpecial", modelUser.User, returnUrl);
                                }
                                else if (modelUser.EnumValue.name == "legalPerson")
                                {
                                    BaseOutput govRoleOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);
                                    BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRoleArray);

                                    bool auth = false;

                                    foreach (var item in modelUser.UserRoleArray)
                                    {
                                        if (item.RoleId == modelUser.Role.Id)
                                        {
                                            auth = true;
                                        }
                                    }

                                    if (auth)
                                    {
                                        return CreateTicket(1, "GovernmentOrganisationSpecial", modelUser.User, returnUrl);
                                    }
                                    else
                                    {
                                        return CreateTicket(1, "Special", modelUser.User, returnUrl);
                                    }

                                }
                                else
                                {
                                    return View(form);
                                }
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
            TempData.Clear();
            return RedirectToAction("Index");
        }
    }
}
