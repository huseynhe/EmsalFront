﻿using Emsal.UI.Infrastructure;
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
    [EmsalAuthorization(AuthorizedAction = ActionName.Ordinary)]

    public class OrdinaryController : Controller
    {
        //
        // GET: /Ordinary/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private BaseInput binput;
        SpecialSummaryViewModel modelUser;
        public ActionResult ReceivedMessages(int?page,long? userId, string type)
        {
            binput = new BaseInput();
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            if (TempData["gov"] != null)
            {
                type = "gov";
            }
            if (TempData["ktn"] != null)
            {
                type = "ktn";
            }
            if (TempData["asc"] != null)
            {
                type = "asc";
            }
            if (TempData["asan"] != null)
            {
                type = "asan";
            }
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

            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.Person);

            modelUser.NameSurname = modelUser.Person == null ? modelUser.LoggedInUser.Username : modelUser.Person.Name + ' ' + modelUser.Person.Surname;

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)userId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();



            BaseOutput bout = srv.WS_GetComMessagesyByToUserId(binput, (long)userId, true, out modelUser.ComMessageArray);


            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();
            modelUser.PagingReceivedMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            modelUser.UserList = new List<tblUser>();

            foreach (var message in modelUser.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.fromUserID, true, out modelUser.User);
                modelUser.UserList.Add(modelUser.User);
            }
          

            BaseOutput LoggedInnUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

            BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.UserRoleArray);
            if (type == "gov")
            {
                TempData["gov"] = "gov";
                return View("ReceivedMessagesGov", modelUser);
            }
            if(type == "ktn")
            {
                TempData["ktn"] = "ktn";
                return View("ReceivedMessagesKTN", modelUser);
            }
            if (type == "asc")
            {
                TempData["asc"] = "acs";
                return View("ReceivedMessagesASC", modelUser);
            }
            if (type == "asan")
            {
                TempData["asan"] = "asan";
                return View("ReceivedMessagesAsan", modelUser);
            }
            return View(modelUser);
        }

        public ActionResult SentMessages(int?page,long? userId, string type)
        {
            binput = new BaseInput();
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            modelUser = new SpecialSummaryViewModel();
            if (TempData["gov"] != null)
            {
                type = "gov";
            }
            if (TempData["ktn"] != null)
            {
                type = "ktn";
            }
            if (TempData["asc"] != null)
            {
                type = "asc";
            }
            if (TempData["asan"] != null)
            {
                type = "asan";
            }
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.LoggedInUser);
            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.Person);

            modelUser.NameSurname = modelUser.Person == null ? modelUser.LoggedInUser.Username : modelUser.Person.Name + ' ' + modelUser.Person.Surname;

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)userId, true, out modelUser.NotReadComMessageArray);
            modelUser.ComMessageList = modelUser.NotReadComMessageArray == null ? null : modelUser.NotReadComMessageArray.ToList();
            modelUser.MessageCount = modelUser.ComMessageList == null ? 0 : modelUser.ComMessageList.Count();

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserId(binput, (long)userId, true, out modelUser.ComMessageArray);

            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();
            modelUser.PagingSentMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            modelUser.UserList = new List<tblUser>();
         
            foreach (var message in modelUser.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.toUserID, true, out modelUser.User);
                modelUser.UserList.Add(modelUser.User);
            }
          

            BaseOutput LoggedInnUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

            BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.LoggedInUser.Id, true, out modelUser.UserRoleArray);
            if (type == "gov")
            {
                TempData["gov"] = "gov";

                return View("SentMessagesGov", modelUser);
            }
            if (type == "ktn")
            {
                TempData["ktn"] = "ktn";
                return View("SentMessagesKTN", modelUser);
            }
            if (type == "asc")
            {
                TempData["asc"] = "asc";
                return View("SentMessagesASC", modelUser);
            }
            if (type == "asan")
            {
                TempData["asan"] = "asan";
                return View("SentMessagesAsan", modelUser);
            }
            return View(modelUser);
        }

        public ActionResult PrivateConversation(long?userId, long?otherUser, int? page)
        {
            binput = new BaseInput();
            int pageSize = 3;
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
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserIDToUserId(binput, (long)otherUser, true, (long)userId, true, out modelUser.ComMessageArray);
            modelUser.ComMessageList = modelUser.ComMessageArray.ToList();

            BaseOutput boutt = srv.WS_GetComMessagesyByFromUserIDToUserId(binput, (long)userId, true, (long)otherUser, true, out modelUser.ComMessageArray);
            modelUser.ComMessageList = modelUser.ComMessageList.Count == 0 ? new List<tblComMessage>() : modelUser.ComMessageList;
            foreach (var item in modelUser.ComMessageArray)
            {
                modelUser.ComMessageList.Add(item);
            }

            modelUser.PagingPrivateMessages = modelUser.ComMessageList.ToPagedList(pageNumber, pageSize);

            BaseOutput otherUserr = srv.WS_GetUserById(binput, (long)otherUser, true, out modelUser.CommunicatedUser);
            return View(modelUser);
        }
        public ActionResult DeleteComMessage(long? userId,int Id, string type)
        {
            SpecialSummaryViewModel UserModel = new SpecialSummaryViewModel();

            binput = new BaseInput();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)userId, true, out UserModel.LoggedInUser);
            srv.WS_GetComMessageById(binput, Id, true, out UserModel.ComMessage);

            srv.WS_DeleteComMessage(binput, UserModel.ComMessage);

            //send to the page where the delete button clicked
            return SendToDestination((long)userId, type, UserModel.ComMessage, null);
            
        }

        public ActionResult AnswerMessage(long?Id, long?userId, string type, long?comunicatedUserId)
        {
            binput = new BaseInput();
            modelUser = new SpecialSummaryViewModel();
            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }
            BaseOutput userGet = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

            if (Id != null)
            {
                BaseOutput mesOut = srv.WS_GetComMessageById(binput, (long)Id, true, out modelUser.ComMessage);
                modelUser.ComMessageId = modelUser.ComMessage.Id;
            }
            //get the communicated user    
            BaseOutput comunicatedOut = srv.WS_GetUserById(binput, (long)comunicatedUserId, true, out modelUser.CommunicatedUser);
            modelUser.CommunicatedUserId = modelUser.CommunicatedUser == null ? 0 : modelUser.CommunicatedUser.Id;

            modelUser.Type = type;

            return View(modelUser);
        }

        [HttpPost]
        public ActionResult AnswerMessage(long?UserId, SpecialSummaryViewModel form)
        {
            binput = new BaseInput();
            modelUser = new SpecialSummaryViewModel();


            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                UserId = Int32.Parse(identity.Ticket.UserData);
            }
            BaseOutput userGet = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.User);
            //to whom we will send
            long toUser;
            //who we are
            long fromUser = modelUser.User.Id;
            if (form.ComMessageId != 0)
            {
                BaseOutput messageOut = srv.WS_GetComMessageById(binput, form.ComMessageId, true, out modelUser.ComMessage);
               
                if (modelUser.User.Id == (long)modelUser.ComMessage.fromUserID)
                {
                    toUser = (long)modelUser.ComMessage.toUserID;
                }
                else
                {
                    toUser = (long)modelUser.ComMessage.fromUserID;
                }

                string createdUser = modelUser.ComMessage.createdUser;

                modelUser.ComMessage = new tblComMessage();
                modelUser.ComMessage.message = form.Message;
                modelUser.ComMessage.toUserID = toUser;
                modelUser.ComMessage.toUserIDSpecified = true;
                modelUser.ComMessage.fromUserID = fromUser;
                modelUser.ComMessage.fromUserIDSpecified = true;
                srv.WS_AddComMessage(binput, modelUser.ComMessage, out modelUser.ComMessage);

            }

            if (form.CommunicatedUserId != 0)
            {
                toUser = form.CommunicatedUserId;

                modelUser.ComMessage = new tblComMessage();
                modelUser.ComMessage.message = form.Message;
                modelUser.ComMessage.toUserID = toUser;
                modelUser.ComMessage.toUserIDSpecified = true;
                modelUser.ComMessage.fromUserID = fromUser;
                modelUser.ComMessage.fromUserIDSpecified = true;
                srv.WS_AddComMessage(binput, modelUser.ComMessage, out modelUser.ComMessage);
            }

            //get the fizikisexs enumvalue
            BaseOutput userrOut = srv.WS_GetUserById(binput, (long)modelUser.ComMessage.fromUserID, true, out modelUser.User);

            return SendToDestination((long)UserId, form.Type, modelUser.ComMessage, null);


        }

        public ActionResult AdminUnit(int pId = 0)
        {
            binput = new BaseInput();

            UserViewModel modelUser = new UserViewModel();

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(binput, out modelUser.PRMAdminUnitArray);
            modelUser.PRMAdminUnitList = modelUser.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelOfferProduction.PRMAdminUnitArray);
            //modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
            if (Session["arrONum"] == null)
            {
                Session["arrONum"] = modelUser.arrNum;
            }
            else
            {
                modelUser.arrNum = (int)Session["arrONum"] + 1;
                Session["arrONum"] = modelUser.arrNum;
            }

            return View(modelUser);
        }

        public void UpdateEmail(string email, int? userId)
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();

            modelUser.User = new tblUser();
            modelUser.Person = new tblPerson();
          
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            try
            {
                BaseOutput userOutput = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);

                modelUser.User.Email = email;
                BaseOutput pout = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

        }

        public string UpdateUser(
          string userName,
          string gender = null,
          int? educationId = null,
          int? jobId = null,
          string job = null,
          int? userId = null,
          int? personId = null,
          string email = null,
          int? userType = null,
          bool IdSpecified = true
          )
        {
            binput = new BaseInput();

            modelUser = new SpecialSummaryViewModel();

            modelUser.Person = new tblPerson();
            modelUser.EnumCategory = new tblEnumCategory();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            try
            {
                BaseOutput userOut = srv.WS_GetUserById(binput, (long)userId, true, out modelUser.User);
                BaseOutput personOutPut = srv.WS_GetPersonByUserId(binput, (long)userId, true, out modelUser.Person);
                personId = (int)modelUser.Person.Id;
                modelUser.User.Username = userName;
                modelUser.User.Id = (long)userId;
                modelUser.User.Status = 1;
                modelUser.User.Email = email;

                modelUser.User.IdSpecified = true;
                modelUser.User.StatusSpecified = true;

                if (personId != null)
                {
                    modelUser.Person.Status = 1;
                    modelUser.Person.gender = gender;
                    modelUser.Person.Id = (long)personId;
                    modelUser.Person.educationLevel_eV_Id = educationId;
                    modelUser.Person.job_eV_Id = jobId;

                    modelUser.Person.IdSpecified = true;
                    modelUser.Person.StatusSpecified = true;
                    modelUser.Person.birtdaySpecified = true;
                    modelUser.Person.educationLevel_eV_IdSpecified = true;
                    modelUser.Person.job_eV_IdSpecified = true;

                }

                BaseOutput pout = srv.WS_UpdateUser(binput, modelUser.User, out modelUser.User);
                BaseOutput personOut = srv.WS_UpdatePerson(binput, modelUser.Person, out modelUser.Person);
               
                //to redirect to governmentOrganisation or special summary controller
                BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRoleArray);
                BaseOutput roleOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelUser.Role);
                foreach (var item in modelUser.UserRoleArray)
                {
                    if (item.RoleId == modelUser.Role.Id)
                    {
                        return ActionName.governmentOrganisation;
                    }
                }

                return ActionName.specialSummary;

            }
            catch (Exception err)
            {
                return "error";
            }

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


        public string ChangePassword(
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

                //to redirect to governmentOrganisation or special summary controller
                BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelUser.User.Id, true, out modelUser.UserRoleArray);
                foreach (var item in modelUser.UserRoleArray)
                {
                    if(item.RoleId == 12)
                    {
                        return ActionName.governmentOrganisation;
                    }
                }
                return ActionName.specialSummary;
            }
            catch (Exception err)
            {
                return "hello";
            }

        }

        //where to send after action completed
        public ActionResult SendToDestination(long userId, string type, tblComMessage mes, long?toUserId)
        {
            if (type == "ktn")
            {
                if (mes.toUserID == userId)
                {
                    return RedirectToAction("ReceivedMessages", "KTNSpecialSummary");
                }
                return RedirectToAction("SentMessages", "KTNSpecialSummary");
            }
            if (type == "asc")
            {
                if (mes.toUserID == userId)
                {
                    return RedirectToAction("ReceivedMessages", "ASCSpecialSummary");
                }
                return RedirectToAction("SentMessages", "ASCSpecialSummary");
            }
            if (type == "asan")
            {
                if (mes.toUserID == userId)
                {
                    return RedirectToAction("ReceivedMessages", "AsanXidmetSpecialSummary");
                }
                return RedirectToAction("SentMessages", "AsanXidmetSpecialSummary");
            }
            if (mes.toUserID == userId|| toUserId == userId)
            {
                return RedirectToAction("ReceivedMessages");
            }
            return RedirectToAction("SentMessages");
        }


        public ActionResult MessageDetail(long?UserId,long?Id,string type)
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
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelUser.User);
            BaseOutput messageOut = srv.WS_GetComMessageById(binput, (long)Id, true, out modelUser.ComMessage);
            if(modelUser.ComMessage.toUserID == UserId)
            {
                BaseOutput fromUserOut = srv.WS_GetUserById(binput, (long)modelUser.ComMessage.fromUserID, true, out modelUser.CommunicatedUser);
            }
            else
            {
                BaseOutput toUserOut = srv.WS_GetUserById(binput, (long)modelUser.ComMessage.toUserID, true, out modelUser.CommunicatedUser);
            }

            modelUser.ComMessage.isRead = 1;
            modelUser.ComMessage.isReadSpecified = true;

            BaseOutput updateComMessage = srv.WS_UpdateComMessage(binput, modelUser.ComMessage, out modelUser.ComMessage);
            return View(modelUser);
        }


        //public void ChangeMessageIsReadStatus(long?messageId)
        //{
        //    binput = new BaseInput();
        //    modelUser = new SpecialSummaryViewModel();
        //    BaseOutput messageOut = srv.WS_GetComMessageById(binput, (long)messageId, true, out modelUser.ComMessage);

        //    modelUser.ComMessage.isRead = 1;
        //    modelUser.ComMessage.isReadSpecified = true;

        //    BaseOutput updateComMessage = srv.WS_UpdateComMessage(binput, modelUser.ComMessage, out modelUser.ComMessage);

        //}

    }
}
