using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.KTNSpecial)]

    public class KTNSpecialSummaryController : Controller
    {
        //
        // GET: /ASCSpecialSummary/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;
        Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;

        private BaseInput binput;
        SpecialSummaryViewModel modelUser;

        public ActionResult Index(long? UserId)
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.User);
            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();
            return View("Index", modelUser);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToRoute("SpecialLogin");
        }

        public string CheckPassword(
        int? userId,
        string password,
        bool IdSpecified = true)
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();

            modelUser.User = new tblUser();

            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }
            modelUser.User.Id = (long)userId;

            modelUser.User.IdSpecified = true;
            modelUser.User.StatusSpecified = true;


            BaseOutput userGet = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
            if (password != null)
            {
                bool verify = BCrypt.Net.BCrypt.Verify(password, modelUser.User.Password);

                if (verify)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            else
            {
                return "false";
            }
        }

        public ActionResult ChangePassword(
    string userName,
    string name,
    string surname,
    int? userId,
    string email,
    string password,
    int? userType = null,
    bool IdSpecified = true
     )
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();



            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }

            try
            {
                BaseOutput userOutput = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
                modelUser.User.Username = modelUser.User.Username;
                modelUser.User.Id = modelUser.User.Id;
                modelUser.User.Email = modelUser.User.Email;
                modelUser.User.Password = BCrypt.Net.BCrypt.HashPassword(password, 5);
                modelUser.User.IdSpecified = true;
                BaseOutput pout = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }

        }
        public ActionResult ReceivedMessages(int? page, long? userId)
        {
            binput = new BaseInput();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            modelUser = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.LoggedInUser);

            BaseOutput bout = srv.WS_GetComMessagesyByToUserId(binput, (long)userId, true, out modelUser.ComMessageArray);


            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();
            modelUser.PagingReceivedMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            modelUser.UserList = new List<tblUser>();

            foreach (var message in modelUser.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.fromUserID, true, out modelUser.User);
                modelUser.UserList.Add(modelUser.User);
            }
            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)userId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();


            BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.UserRoleArray);
            BaseOutput LoggedInnUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
            return View(modelUser);
        }

        public ActionResult SentMessages(int? page, long? userId)
        {
            binput = new BaseInput();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            modelUser = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.LoggedInUser);

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserId(binput, (long)userId, true, out modelUser.ComMessageArray);

            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();
            modelUser.PagingSentMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            modelUser.UserList = new List<tblUser>();

            foreach (var message in modelUser.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.toUserID, true, out modelUser.User);
                modelUser.UserList.Add(modelUser.User);
            }
            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)userId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();

            BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.UserRoleArray);
            BaseOutput LoggedInnUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

            return View(modelUser);
        }

    }
}
