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
using Emsal.Utility.CustomObjects;
using System.Net.Mail;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.PotentialClientState)]

    public class PotentialClientStateController : Controller
    {
        private BaseInput baseInput;

        private static string snameSurnameFathername;
        private static string spin;
        private static string sfullAddress;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
       // Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private PotentialClientStateViewModel modelPotentialClientState;


        public ActionResult Index(int? page, string nameSurnameFathername = null, string pin = null, string fullAddress = null)
        {
            try
            {

                if (nameSurnameFathername != null)
                    nameSurnameFathername = StripTag.strSqlBlocker(nameSurnameFathername.ToLower());
                if (pin != null)
                    pin = StripTag.strSqlBlocker(pin.ToLower());
                if (fullAddress != null)
                    fullAddress = StripTag.strSqlBlocker(fullAddress.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (nameSurnameFathername == null && pin == null && fullAddress == null)
                {
                    snameSurnameFathername = null;
                    spin = null;
                    sfullAddress = null;
                }

                if (nameSurnameFathername != null)
                    snameSurnameFathername = nameSurnameFathername;
                if (pin != null)
                    spin = pin;
                if (fullAddress != null)
                    sfullAddress = fullAddress;

                baseInput = new BaseInput();
                modelPotentialClientState = new PotentialClientStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientState.User);
                baseInput.userName = modelPotentialClientState.User.Username;

                BaseOutput gpp = srv.WS_GetPersonalinformationByRoleId(baseInput, 24, true, modelPotentialClientState.User.Id, true, out modelPotentialClientState.UserInfoArray);
                                
                if (modelPotentialClientState.UserInfoArray != null)
                {
                    modelPotentialClientState.UserInfoList = modelPotentialClientState.UserInfoArray.ToList();
                }
                else
                {
                    modelPotentialClientState.UserInfoList = new List<UserInfo>();
                }

                if (snameSurnameFathername != null)
                {
                    modelPotentialClientState.UserInfoList = modelPotentialClientState.UserInfoList.Where(x => x.name.ToLower().Contains(snameSurnameFathername) || x.surname.ToLower().Contains(snameSurnameFathername) || x.fatherName.ToLower().Contains(snameSurnameFathername) || x.OrganisationName.ToLower().Contains(snameSurnameFathername)).ToList();
                }

                if (spin != null)
                {
                    modelPotentialClientState.UserInfoList = modelPotentialClientState.UserInfoList.Where(x =>x.pinNumber.ToLower().Contains(spin)).ToList();
                }

                if (sfullAddress != null)
                {
                    modelPotentialClientState.UserInfoList = modelPotentialClientState.UserInfoList.Where(x =>x.fullAddress.ToLower().Contains(sfullAddress) || x.personAdressDesc.ToLower().Contains(sfullAddress)).ToList();
                }
                modelPotentialClientState.itemCount = modelPotentialClientState.UserInfoList.Count();
                modelPotentialClientState.PagingUserInfo = modelPotentialClientState.UserInfoList.ToPagedList(pageNumber, pageSize);

                modelPotentialClientState.isMain = 0;

                modelPotentialClientState.nameSurnameFathername = snameSurnameFathername;
                modelPotentialClientState.pin = spin;
                modelPotentialClientState.fullAddress = sfullAddress;

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelPotentialClientState)
                   : View(modelPotentialClientState);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        [HttpPost]
        public ActionResult Approv(int[] ids)
        {
            try
            {

                baseInput = new BaseInput();
                modelPotentialClientState = new PotentialClientStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientState.User);
                baseInput.userName = modelPotentialClientState.User.Username;

                modelPotentialClientState.PotentialProduction = new tblPotential_Production();

                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput,ids[i], true, out modelPotentialClientState.UserRoleArray);
                        modelPotentialClientState.UserRole = modelPotentialClientState.UserRoleArray.FirstOrDefault();

                        if(modelPotentialClientState.UserRole.RoleId==24)
                        {
                            modelPotentialClientState.UserRole.RoleId = 15;
                        }

                        BaseOutput updateUserRole = srv.WS_UpdateUserRole(baseInput, modelPotentialClientState.UserRole, out modelPotentialClientState.UserRole);

                       
                        try
                        {
                            string sn = "";
                            BaseOutput muser = srv.WS_GetUserById(baseInput, ids[i], true, out modelPotentialClientState.User);
                            BaseOutput person = srv.WS_GetPersonByUserId(baseInput, ids[i], true, out modelPotentialClientState.Person);

                            if (modelPotentialClientState.Person!=null)
                            {
                                sn = modelPotentialClientState.Person.Surname + " " + modelPotentialClientState.Person.Name;
                            }

                            MailMessage msg = new MailMessage();

                            msg.To.Add(modelPotentialClientState.User.Email);
                            msg.Subject = "Potensial istehsalçının təsdiqi";

                            msg.Body = "<b>Hörmətli "+ sn + ", </b><br/><br/> Siz Kənd Təsərrüfatı Nazirliyi tərəfindən <b>potensial istehsalçı</b> kimi təsdiq edildiniz.<br/><br/>Azərbaycan Respublikasının Kənd Təsərrüfatı Nazirliyi";

                            msg.IsBodyHtml = true;

                            Mail.SendMail(msg);
                        }
                        catch { }


                        //BaseOutput gop = srv.WS_GetOffer_ProductionsByUserID(baseInput, ids[i], true, out modelPotentialClientState.Offer_ProductionArray);

                        //if (modelPotentialClientState.Offer_ProductionArray != null)
                        //{
                        //    modelPotentialClientState.Offer_ProductionList = modelPotentialClientState.Offer_ProductionArray.ToList();

                        //    foreach (var item in modelPotentialClientState.Offer_ProductionList)
                        //    {
                        //        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Yayinda", out modelPotentialClientState.EnumValue);
                        //        modelPotentialClientState.Offer_Production.state_eV_Id = modelPotentialClientState.EnumValue.Id;
                        //        modelPotentialClientState.Offer_Production.state_eV_IdSpecified = true;

                        //        BaseOutput uop = srv.WS_UpdateOffer_Production(baseInput, modelPotentialClientState.Offer_Production, out modelPotentialClientState.Offer_Production);
                        //    }
                        //}
                    }
                }

                return RedirectToAction("Index", "PotentialClientState");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult Edit(int id)
        {
            try
            {

                baseInput = new BaseInput();
                modelPotentialClientState = new PotentialClientStateViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelPotentialClientState.User);
                baseInput.userName = modelPotentialClientState.User.Username;
                
                BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput, id, true, out modelPotentialClientState.UserRoleArray);
                modelPotentialClientState.UserRole = modelPotentialClientState.UserRoleArray.FirstOrDefault();

                if (modelPotentialClientState.UserRole.RoleId == 24)
                {
                    modelPotentialClientState.UserRole.RoleId = 11;
                }

                BaseOutput updateUserRole = srv.WS_UpdateUserRole(baseInput, modelPotentialClientState.UserRole, out modelPotentialClientState.UserRole);


                try
                {
                    string sn = "";
                    BaseOutput muser = srv.WS_GetUserById(baseInput, id, true, out modelPotentialClientState.User);
                    BaseOutput person = srv.WS_GetPersonByUserId(baseInput, id, true, out modelPotentialClientState.Person);

                    if (modelPotentialClientState.Person != null)
                    {
                        sn = modelPotentialClientState.Person.Surname + " " + modelPotentialClientState.Person.Name;
                    }

                    MailMessage msg = new MailMessage();

                    msg.To.Add(modelPotentialClientState.User.Email);
                    msg.Subject = "İdxalçının təsdiqi";

                    msg.Body = "<b>Hörmətli " + sn + " </b><br/><br/> Siz <b>tedaruk.az</b> portalından <b>potensial istehsalçı</b> kimi qeydiyyatdan keçmişdiniz. Kənd Təsərrüfatı Nazirliyi Sizin müraciəti araşdırdıqdan sonra məlum oldu ki, Siz <b>potensial istehsalçı</b> deyilsiniz. Ona görə də, Kənd Təsərrüfatı Nazirliyi Sizi <b>idxalçı</b> kimi təsdiq etdi.<br/><br/>Azərbaycan Respublikasının Kənd Təsərrüfatı Nazirliyi";

                    msg.IsBodyHtml = true;

                    Mail.SendMail(msg);
                }
                catch { }


                return RedirectToAction("Index", "PotentialClientState");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(PotentialClientStateViewModel model, FormCollection collection)
        {
            try
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

                BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput, model.userId, true, out modelPotentialClientState.UserRoleArray);
                modelPotentialClientState.UserRole = modelPotentialClientState.UserRoleArray.FirstOrDefault();

                if (modelPotentialClientState.UserRole.RoleId == 24)
                {
                    modelPotentialClientState.UserRole.RoleId = 11;
                }

                BaseOutput updateUserRole = srv.WS_UpdateUserRole(baseInput, modelPotentialClientState.UserRole, out modelPotentialClientState.UserRole);

                model.ComMessage = new tblComMessage();
                model.ComMessage.message = model.message;
                model.ComMessage.fromUserID = (long)UserId;
                model.ComMessage.fromUserIDSpecified = true;
                model.ComMessage.toUserID = model.userId;
                model.ComMessage.toUserIDSpecified = true;
                model.ComMessage.Production_Id = model.PotentialProduction.Id;
                model.ComMessage.Production_IdSpecified = true;
                BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out model.EnumValue);
                model.ComMessage.Production_type_eV_Id = model.EnumValue.Id;
                model.ComMessage.Production_type_eV_IdSpecified = true;

                BaseOutput acm = srv.WS_AddComMessage(baseInput, model.ComMessage, out model.ComMessage);


                return RedirectToAction("Index", "PotentialClientState");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
