using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        //Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;
        private BaseInput binput;
        UserViewModel modelUser;
        //burda ticket yaradırıq
        public ActionResult CreateTicket(int ticketNum, string route, tblUser User, string returnUrl, long uId=0)
        {
            transactionID = Emsal.Utility.UtilityObjects.IOUtil.GetFunctionRequestID();
            //burda userData yerine ne yazırsan yaz elebele user.ID.Tosring qoymuşam

            if(uId>0)
            {
                binput = new BaseInput();
                modelUser = new UserViewModel();
                BaseOutput uidBase = srv.WS_GetUserById(binput, (long)uId, true, out modelUser.User);
                User = modelUser.User;
            }

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
            //return RedirectToAction("Index", route);
            //}

        }

        public ActionResult Index(string uid = null, string type = null)
        {
                      binput = new BaseInput();

                      AuthLogin authLogin = new AuthLogin();

                authLogin.uid = uid;
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


        public ActionResult Redirect()
        {
            binput = new BaseInput();

            modelUser = new UserViewModel();

            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(binput, userId, true, out modelUser.User);

            if(modelUser.User==null)
            {
                return RedirectToAction("Index", "Login");
            }

            binput.userName = modelUser.User.Username;



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
                                if (item.RoleId == asanRoleID)
                                {
                                    ifAsan = true;
                                }
                                else if (item.RoleId == ascRoleID)
                                {
                                    ifASC = true;
                                }
                                else if (item.RoleId == ktnRoleID)
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
                                return CreateTicket(1, "AsanXidmetSpecial", modelUser.User, "");
                            }
                            else
                            {
                                if (modelUser.EnumValue.name == "fizikişexs")
                                {
                                    return CreateTicket(1, "Special", modelUser.User, "");
                                }
                                else if (ifASC)
                                {
                                    return CreateTicket(1, "ASCSpecial", modelUser.User, "");
                                }
                                else if (ifKTN)
                                {
                                    return CreateTicket(1, "KTNSpecial", modelUser.User, "");
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
                                        return CreateTicket(1, "GovernmentOrganisationSpecial", modelUser.User, "");
                                    }
                                    else
                                    {
                                        return CreateTicket(1, "Special", modelUser.User, "");
                                    }

                                }
                                else
                                {
                                return RedirectToAction("Index", "Home");
                            }
                            }
        }
        public ActionResult ForgetPassword()
        {
            User modelUser = new User();
            BaseInput binput = new BaseInput();
            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelUser.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelUser.EnumCategory.Id, true, out modelUser.EnumValueArray);
            modelUser.MobilePhonePrefixList = modelUser.EnumValueArray.ToList();

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult ForgetPassword(User form)
        {
            User modelUser = new User();

            BaseInput binput = new BaseInput();

            //Get the user

            BaseOutput userOUt = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);

            if (modelUser.FutureUser != null)
            {
                SendUserPassword(modelUser.FutureUser.Username, modelUser.FutureUser.Password, form.Email);
                TempData["sendUserInfo"]= "info";
            }

            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(string username)
        {
            if (TempData["oneTime"] != null)
            {
               
                User modelUser = new User();
                modelUser.UserName = username;
                return View(modelUser);
            }
            else
            {
                return HttpNotFound();

            }

        }

        [HttpPost]
        public ActionResult ResetPassword(User form)
        {
            User modelUser = new User();
            BaseInput binput = new BaseInput();

            BaseOutput userOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelUser.FutureUser);
            modelUser.FutureUser.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);

            BaseOutput updateUser = srv.WS_UpdateUser(binput, modelUser.FutureUser, out modelUser.FutureUser);

            TempData.Clear();
            TempData["passwordChanged"] = "info";
            return Redirect("/Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            TempData.Clear();
            Session["musername"] = null;
            return RedirectToAction("Index");
        }

        public void SendUserPassword(string userName, string password, string email)
        {
            if (CheckForInternetConnection())
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("ferid.heziyev@gmail.com", "tedaruk.az");
                if (String.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains(".com"))
                {
                    email = "ferid.heziyev@gmail.com";
                }
                msg.To.Add(email);
                string fromPassword = "e1701895";
                msg.Subject = "Giriş Məlumatlarınız";

                msg.Body = "<p>İstifadəçi adınız:" + userName + "</p>" +
                           "<p>Şifrənizi aşağıdakı linkdən dəyişdirin</p>" +
                           "<p>http://localhost:56557/Login/ResetPassword/?username=" + userName + "</p>";

                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
                smtp.Timeout = 20000;

                TempData["onetime"] = "info";

                smtp.Send(msg);
            }

        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
