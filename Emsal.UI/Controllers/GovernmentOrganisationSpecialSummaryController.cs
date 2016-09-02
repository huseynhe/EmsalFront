using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Emsal.Utility.CustomObjects;
using PagedList;
using System.Text.RegularExpressions;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.governmentOrganisation)]

    public class GovernmentOrganisationSpecialSummaryController : Controller
    {
        //
        // GET: /GovernmentOrganisation/
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private BaseInput binput;
        UserViewModel modelUser;
        SpecialSummaryViewModel modelSpecial;

        public ActionResult Index(int? page, long? UserId)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();

            //get the informations of logged in user
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;
            //////////////////////////////////////////////////////////////////


            //get the confirmed demand productions of the logged in user
            modelSpecial.DemandProduction.user_Id = UserId;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demand = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);

            modelSpecial.DemandProductionList = modelSpecial.DemandProductionArray.ToList();
            //////////////////////////////////////////////////////////////////////////////////////


            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();

            modelSpecial.OrgDemandList = new List<GovernmentOrganisationDemand>();

            foreach (var item in modelSpecial.DemandProductionList)
            {
                modelSpecial.OrgDemand = new GovernmentOrganisationDemand();

                BaseOutput product = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                //get the quanity of the demanded product
                modelSpecial.OrgDemand.ProductQuantity = (double)item.quantity;

                //get the id of the demanded product
                modelSpecial.OrgDemand.ProductId = item.Id;

                //get the name of the demanded product
                modelSpecial.OrgDemand.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the total price of the demanded product
                modelSpecial.OrgDemand.ProductTotalPrice = (item.quantity == null || item.unit_price == null) ? 0 : (double)item.quantity * (double)item.unit_price;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.OrgDemand.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                //get the parent name of the product
                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                modelSpecial.OrgDemand.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the demanded product's quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByDemandProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.OrgDemand.QuantityType = modelSpecial.EnumValue.name;
                    }
                }

                //get the shipment period
                BaseOutput enval = srv.WS_GetEnumValueByName(binput, "Demand", out modelSpecial.EnumValue);
                long envalId = modelSpecial.EnumValue.Id;

                BaseOutput calendar = srv.WS_GetProductionCalendarByProductionId(binput, (long)item.Id, true, envalId, true, out modelSpecial.ProductionCalendar);
                string[] months = modelSpecial.ProductionCalendar.Months.Split(',');
                List<string> monthString = new List<string>();
                foreach (var month in months)
                {
                    BaseOutput monthEnum = srv.WS_GetEnumValueById(binput, Convert.ToInt32(month), true, out modelSpecial.EnumValue);
                    monthString.Add(modelSpecial.EnumValue.name);
                }

                modelSpecial.OrgDemand.ShipmentPeriod = string.Join(",", monthString);
                ////////////////////////////////////////////////////////////////////////////////////////////

                //get the shipment place
                BaseOutput address = srv.WS_GetProductAddressById(binput, (long)item.address_Id, true, out modelSpecial.ProductAddress);
                modelSpecial.OrgDemand.ShipmentPlace = modelSpecial.ProductAddress.fullAddress;
                ////////////////////////////////////////////////////////////////////////////////////////////

                modelSpecial.OrgDemandList.Add(modelSpecial.OrgDemand);
            }
            modelSpecial.PagingConfirmedDemand = modelSpecial.OrgDemandList.ToPagedList(pageNumber, pageSize);

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();
            return View(modelSpecial);
        }

        public ActionResult OnAirDemands(int?page,long? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            int pageNumber = (page ?? 1);
            int pageSize = 2;
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();

            //get the informations of logged in user

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;

            ////////////////////////////////////////////////////////////////////////////////////


            //get on air demands

            modelSpecial.DemandProduction.user_Id = UserID;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demand = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);

            modelSpecial.DemandProductionList = modelSpecial.DemandProductionArray.ToList();
            
            //////////////////////////////////////////////////////////////////////////////////////



            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
            modelSpecial.OrgDemandList = new List<GovernmentOrganisationDemand>();

            foreach (var item in modelSpecial.DemandProductionList)
            {
                modelSpecial.OrgDemand = new GovernmentOrganisationDemand();

                BaseOutput product = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                //get the quanity of the demanded product
                modelSpecial.OrgDemand.ProductQuantity = (double)item.quantity;

                //get the id of the demanded product
                modelSpecial.OrgDemand.ProductId = item.Id;

                //get the name of the demanded product
                modelSpecial.OrgDemand.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the total price of the demanded product
                modelSpecial.OrgDemand.ProductTotalPrice = (item.quantity == null || item.unit_price == null) ? 0 : (double)item.quantity * (double)item.unit_price;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.OrgDemand.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                //get the parent name of the product
                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                modelSpecial.OrgDemand.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the demanded product's quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByDemandProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.OrgDemand.QuantityType = modelSpecial.EnumValue.name;
                    }
                }

                //get the shipment period
                BaseOutput envall = srv.WS_GetEnumValueByName(binput, "Demand", out modelSpecial.EnumValue);
                long envalIdl = modelSpecial.EnumValue.Id;

                BaseOutput calendar = srv.WS_GetProductionCalendarByProductionId(binput, (long)item.Id, true, envalIdl, true, out modelSpecial.ProductionCalendar);
                string[] months = modelSpecial.ProductionCalendar.Months.Split(',');
                List<string> monthString = new List<string>();
                foreach (var month in months)
                {
                    BaseOutput monthEnum = srv.WS_GetEnumValueById(binput, Convert.ToInt32(month), true, out modelSpecial.EnumValue);
                    monthString.Add(modelSpecial.EnumValue.name);
                }

                modelSpecial.OrgDemand.ShipmentPeriod = string.Join(",", monthString);
                ////////////////////////////////////////////////////////////////////////////////////////////

                //get the shipment place
                BaseOutput address = srv.WS_GetProductAddressById(binput, (long)item.address_Id, true, out modelSpecial.ProductAddress);
                modelSpecial.OrgDemand.ShipmentPlace = modelSpecial.ProductAddress.fullAddress;
                ////////////////////////////////////////////////////////////////////////////////////////////

                modelSpecial.OrgDemandList.Add(modelSpecial.OrgDemand);
            }

            modelSpecial.PagingDemand = modelSpecial.OrgDemandList.ToPagedList(pageNumber, pageSize);

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();

            return View(modelSpecial);
        }

        public ActionResult ExpiredDemands(int?page,long?UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            int pageNumber = (page ?? 1);
            int pageSize = 2;
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();

            //get the information of the logged in user

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;

            ////////////////////////////////////////////////////////////////////////////////////////

            //get expired demands
            modelSpecial.DemandProduction.user_Id = UserID;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demand = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);
            modelSpecial.DemandProductionList = modelSpecial.DemandProductionArray.ToList();

            ////////////////////////////////////////////////////////////////

            modelSpecial.OrgDemandList = new List<GovernmentOrganisationDemand>();

            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
            foreach (var item in modelSpecial.DemandProductionList)
            {
                modelSpecial.OrgDemand = new GovernmentOrganisationDemand();

                BaseOutput product = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                //get the quanity of the demanded product
                modelSpecial.OrgDemand.ProductQuantity = (double)item.quantity;

                //get the id of the demanded product
                modelSpecial.OrgDemand.ProductId = item.Id;

                //get the name of the demanded product
                modelSpecial.OrgDemand.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the total price of the demanded product
                modelSpecial.OrgDemand.ProductTotalPrice = (item.quantity == null || item.unit_price == null) ? 0 : (double)item.quantity * (double)item.unit_price;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.OrgDemand.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                //get the parent name of the product
                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                modelSpecial.OrgDemand.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the demanded product's quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByDemandProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.OrgDemand.QuantityType = modelSpecial.EnumValue.name;
                    }
                }

                //get the shipment period
                BaseOutput envall = srv.WS_GetEnumValueByName(binput, "Demand", out modelSpecial.EnumValue);
                long envalIdl = modelSpecial.EnumValue.Id;

                BaseOutput calendar = srv.WS_GetProductionCalendarByProductionId(binput, (long)item.Id, true, envalIdl, true, out modelSpecial.ProductionCalendar);
                string[] months = modelSpecial.ProductionCalendar == null ? new string[0] : modelSpecial.ProductionCalendar.Months.Split(',');
                List<string> monthString = new List<string>();
                foreach (var month in months)
                {
                    BaseOutput monthEnum = srv.WS_GetEnumValueById(binput, Convert.ToInt32(month), true, out modelSpecial.EnumValue);
                    monthString.Add(modelSpecial.EnumValue.name);
                }

                modelSpecial.OrgDemand.ShipmentPeriod = string.Join(",", monthString);
                ////////////////////////////////////////////////////////////////////////////////////////////

                //get the shipment place
                BaseOutput address = srv.WS_GetProductAddressById(binput, (long)item.address_Id, true, out modelSpecial.ProductAddress);
                modelSpecial.OrgDemand.ShipmentPlace = modelSpecial.ProductAddress.fullAddress;
                ////////////////////////////////////////////////////////////////////////////////////////////

                modelSpecial.OrgDemandList.Add(modelSpecial.OrgDemand);
            }

            modelSpecial.PagingOffAirDemand = modelSpecial.OrgDemandList.ToPagedList(pageNumber, pageSize);


            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();
            return View(modelSpecial);

        }

        public ActionResult RejectedDemands(int?page,long? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "reject", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();


            //get the informations of the logged in user

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;

            //////////////////////////////////////////////////////////////////////////


            //get the rejected demands

            modelSpecial.DemandProduction.user_Id = UserID;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demand = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);
            modelSpecial.DemandProductionList = modelSpecial.DemandProductionArray.ToList();

            ////////////////////////////////////////////////////////////////////////////////////////////////

            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
            modelSpecial.OrgDemandList = new List<GovernmentOrganisationDemand>();

            foreach (var item in modelSpecial.DemandProductionList)
            {
                modelSpecial.OrgDemand = new GovernmentOrganisationDemand();

                BaseOutput product = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                //get the quanity of the demanded product
                modelSpecial.OrgDemand.ProductQuantity = (double)item.quantity;

                //get the id of the demanded product
                modelSpecial.OrgDemand.ProductId = item.Id;

                //get the name of the demanded product
                modelSpecial.OrgDemand.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the total price of the demanded product
                modelSpecial.OrgDemand.ProductTotalPrice = (item.quantity == null || item.unit_price == null) ? 0 : (double)item.quantity * (double)item.unit_price;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.OrgDemand.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                //get the parent name of the product
                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                modelSpecial.OrgDemand.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the demanded product's quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByDemandProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.OrgDemand.QuantityType = modelSpecial.EnumValue.name;
                    }
                }

                //get the shipment period
                BaseOutput enval = srv.WS_GetEnumValueByName(binput, "Demand", out modelSpecial.EnumValue);
                long envalId = modelSpecial.EnumValue.Id;

                BaseOutput calendar = srv.WS_GetProductionCalendarByProductionId(binput, (long)item.Id, true, envalId, true, out modelSpecial.ProductionCalendar);
                string[] months = modelSpecial.ProductionCalendar.Months.Split(',');
                List<string> monthString = new List<string>();
                foreach (var month in months)
                {
                    BaseOutput monthEnum = srv.WS_GetEnumValueById(binput, Convert.ToInt32(month), true, out modelSpecial.EnumValue);
                    monthString.Add(modelSpecial.EnumValue.name);
                }

                modelSpecial.OrgDemand.ShipmentPeriod = string.Join(",", monthString);
                ////////////////////////////////////////////////////////////////////////////////////////////

                //get the shipment place
                BaseOutput address = srv.WS_GetProductAddressById(binput, (long)item.address_Id, true, out modelSpecial.ProductAddress);
                modelSpecial.OrgDemand.ShipmentPlace = modelSpecial.ProductAddress.fullAddress;
                ////////////////////////////////////////////////////////////////////////////////////////////

                modelSpecial.OrgDemandList.Add(modelSpecial.OrgDemand);
            }

            modelSpecial.PagingRejectedDemand = modelSpecial.OrgDemandList.ToPagedList(pageNumber, pageSize);

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();
            return View(modelSpecial);
        }
        public ActionResult ReceivedMessages(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput bout = srv.WS_GetComMessagesyByToUserId(binput, userId, true, out userModel.ComMessageArray);
            

            userModel.ComMessageList = userModel.ComMessageArray.ToList();
            foreach (var item in userModel.ComMessageList)
            {
                DateTime a= item.createdDate.toLongDate();
            }

            userModel.UserList = new List<tblUser>();

            foreach (var message in userModel.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.fromUserID, true, out userModel.User);
                userModel.UserList.Add(userModel.User);
            }

            return View(userModel);
        }

        public ActionResult SentMessages(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserId(binput, userId, true, out userModel.ComMessageArray);

            userModel.ComMessageList = userModel.ComMessageArray.ToList();

            userModel.UserList = new List<tblUser>();

            foreach (var message in userModel.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.toUserID, true, out userModel.User);
                userModel.UserList.Add(userModel.User);
            }

            return View(userModel);
        }

        public ActionResult ReceivedMessagesSortedForDateAsc(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput bout = srv.WS_GetComMessagesyByToUserIdSortedForDateAsc(binput, userId, true, out userModel.ComMessageArray);


            userModel.ComMessageList = userModel.ComMessageArray.ToList();

            userModel.UserList = new List<tblUser>();

            foreach (var message in userModel.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.fromUserID, true, out userModel.User);
                userModel.UserList.Add(userModel.User);
            }

            return View(userModel);
        }

        public ActionResult ReceivedMessagesSortedForDateDes(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput bout = srv.WS_GetComMessagesyByToUserIdSortedForDateDes(binput, userId, true, out userModel.ComMessageArray);


            userModel.ComMessageList = userModel.ComMessageArray.ToList();

            userModel.UserList = new List<tblUser>();

            foreach (var message in userModel.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.fromUserID, true, out userModel.User);
                userModel.UserList.Add(userModel.User);
            }

            return View(userModel);
        }

        public ActionResult SentMessagesSortedForDateAsc(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserIdSortedForDateAsc(binput, userId, true, out userModel.ComMessageArray);

            userModel.ComMessageList = userModel.ComMessageArray.ToList();

            userModel.UserList = new List<tblUser>();

            foreach (var message in userModel.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.toUserID, true, out userModel.User);
                userModel.UserList.Add(userModel.User);
            }

            return View(userModel);
        }

        public ActionResult SentMessagesSortedForDateDes(int userId)
        {
            binput = new BaseInput();

            UserViewModel userModel = new UserViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput bout = srv.WS_GetComMessagesyByFromUserIdSortedForDateDes(binput, userId, true, out userModel.ComMessageArray);

            userModel.ComMessageList = userModel.ComMessageArray.ToList();

            userModel.UserList = new List<tblUser>();

            foreach (var message in userModel.ComMessageList)
            {
                srv.WS_GetUserById(binput, (long)message.toUserID, true, out userModel.User);
                userModel.UserList.Add(userModel.User);
            }

            return View(userModel);
        }

        public ActionResult DeleteComMessage(int Id)
        {
            UserViewModel UserModel = new UserViewModel();


            binput = new BaseInput();

            srv.WS_GetComMessageById(binput, Id, true, out UserModel.ComMessage);

            srv.WS_DeleteComMessage(binput, UserModel.ComMessage);

            return RedirectToAction("Index");
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

            modelUser = new UserViewModel();

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

            modelUser = new UserViewModel();



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

       

        public ActionResult ReEditedOrders(long? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "reedited", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }

            modelSpecial.DemandProduction.user_Id = UserID;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demand = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);

            modelSpecial.DemandProductionList = modelSpecial.DemandProductionArray.ToList();

            BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelSpecial.EnumCategory);

            BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);

            modelSpecial.EnumValueList = modelSpecial.EnumValueArray.ToList();

            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
            foreach (var item in modelSpecial.DemandProductionList)
            {
                BaseOutput product = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);
                modelSpecial.ProductCatalogList.Add(modelSpecial.ProductCatalog);
                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                modelSpecial.ProductCatalogList.Add(modelSpecial.ProductCatalog);
            }
            BaseOutput enval = srv.WS_GetEnumValueByName(binput, "Demand", out modelSpecial.EnumValue);
            long envalId = modelSpecial.EnumValue.Id;
            modelSpecial.modelMonthsList = new List<MonthsModel>();
            foreach (var production in modelSpecial.DemandProductionList)
            {
                BaseOutput calendar = srv.WS_GetProductionCalendarByProductionId(binput, (long)production.Id, true, envalId, true, out modelSpecial.ProductionCalendar);
                string[] months = modelSpecial.ProductionCalendar.Months.Split(',');


                foreach (var month in months)
                {
                    modelSpecial.modelMonths = new MonthsModel();

                    BaseOutput monthEnum = srv.WS_GetEnumValueById(binput, Convert.ToInt32(month), true, out modelSpecial.EnumValue);
                    modelSpecial.modelMonths.monthName = modelSpecial.EnumValue.name;
                    modelSpecial.modelMonths.productionId = (int)production.Id;
                    modelSpecial.modelMonthsList.Add(modelSpecial.modelMonths);
                }
            }
            modelSpecial.ProductAddressList = new List<tblProductAddress>();
            foreach (var prod in modelSpecial.DemandProductionList)
            {
                BaseOutput address = srv.WS_GetProductAddressById(binput, (long)prod.address_Id, true, out modelSpecial.ProductAddress);
                modelSpecial.ProductAddressList.Add(modelSpecial.ProductAddress);
            }
            foreach (var item in modelSpecial.DemandProductionList)
            {
                BaseOutput productControlOut = srv.WS_GetProductionControlsByDemandProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        modelSpecial.ProductionControlList.Add(itemm);
                    }
                }
            }
            return View(modelSpecial);
        }

        public ActionResult DemandProductInfo(int? Id, long? UserId)
        {
            binput = new BaseInput();
            DemandProductionViewModel modelProduction = new DemandProductionViewModel();

            BaseOutput offerOut = srv.WS_GetDemand_ProductionById(binput, (long)Id, true, out modelProduction.DemandProduction);
            BaseOutput prodOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.DemandProduction.product_Id, true, out modelProduction.ProductCatalog);


            modelProduction.ProductName = modelProduction.ProductCatalog.ProductName;

            //get product parent
            BaseOutput parentOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.ProductCatalog.ProductCatalogParentID, true, out modelProduction.ProductCatalog);
            modelProduction.ProductParentName = modelProduction.ProductCatalog.ProductName;


            modelProduction.Quantity = modelProduction.DemandProduction.quantity.ToString();

            //get quantity type from production control
            BaseOutput controlOut = srv.WS_GetProductionControlsByDemandProductionId(binput, modelProduction.DemandProduction.Id, true, out modelProduction.ProductionControlArray);

            modelProduction.QuantityType = modelProduction.ProductionControlArray.FirstOrDefault() == null ? 0 : (long)modelProduction.ProductionControlArray.FirstOrDefault().EnumValueId;
            BaseOutput quantTypeOut = srv.WS_GetEnumValueById(binput, modelProduction.QuantityType, true, out modelProduction.EnumValue);
            modelProduction.QuantityTypeStr = modelProduction.EnumValue == null ? "" : modelProduction.EnumValue.name;
            modelProduction.startDate = new DateTime((long)modelProduction.DemandProduction.startDate);
            modelProduction.endDate = new DateTime((long)modelProduction.DemandProduction.endDate);

            DateTime end = (DateTime)modelProduction.endDate;
            modelProduction.EndDateStr = end.ToString("d");

            DateTime start = (DateTime)modelProduction.startDate;
            modelProduction.StartDateStr = start.ToString("d");


            //get full address
            BaseOutput prodAddrOut = srv.WS_GetProductAddressById(binput, (long)modelProduction.DemandProduction.address_Id, true, out modelProduction.ProductAddress);
            modelProduction.Address = modelProduction.ProductAddress.fullAddress;
            ///////


            //user info

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(binput, (long)UserId, true, out modelProduction.User);
            BaseOutput person = srv.WS_GetPersonByUserId(binput, (long)UserId, true, out modelProduction.Person);


            return View(modelProduction);
        }
    
        public ActionResult EditRejectedOrders(long? UserId, int? Id)
        {
            Session["arrONum"] = null;

            binput = new BaseInput();
            DemandProductionViewModel modelDemandProduct = new DemandProductionViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(binput, (long)UserId, true, out modelDemandProduct.User);
            BaseOutput person = srv.WS_GetPersonByUserId(binput, (long)UserId, true, out modelDemandProduct.Person);

            //get enum categories with name olcu vahidi
            BaseOutput enumCatOut = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelDemandProduct.EnumCategorySS);
            //get enum categories with the selected enum value id
            BaseOutput enumValOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelDemandProduct.EnumCategorySS.Id, true, out modelDemandProduct.EnumValueArray);
            modelDemandProduct.EnumValueList = modelDemandProduct.EnumValueArray.ToList();

            BaseOutput productOut = srv.WS_GetDemand_ProductionById(binput, (long)Id, true, out modelDemandProduct.DemandProduction);
            //modelDemandProduct.Id = (long)Id;
            //var productArr = modelDemandProduct.DemandProduction.fullProductId;
            //var productMain = productArr.Split(',');
            BaseOutput prOut = srv.WS_GetProductCatalogsById(binput, (int)modelDemandProduct.DemandProduction.product_Id, true, out modelDemandProduct.ProductCatalog);

            ////get types of the product ex -> çekilmiş et, hise verilmiş et exc.
            //BaseOutput typeOut = srv.WS_GetProductCatalogsByParentId(binput, (int)modelDemandProduct.ProductCatalog.Id, true, out modelDemandProduct.ProductCatalogArray);

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(binput, "shippingSchedule", out modelDemandProduct.EnumCategory);
            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelDemandProduct.EnumCategory.Id, true, out modelDemandProduct.EnumValueArray);
            modelDemandProduct.EnumValueShippingScheduleList = modelDemandProduct.EnumValueArray.ToList();

            BaseOutput enumcate = srv.WS_GetEnumCategorysByName(binput, "month", out modelDemandProduct.EnumCategory);
            BaseOutput enumvalue = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelDemandProduct.EnumCategory.Id, true, out modelDemandProduct.EnumValueArray);

            modelDemandProduct.EnumValueMonthList = modelDemandProduct.EnumValueArray.ToList();
            modelDemandProduct.Quantity = modelDemandProduct.DemandProduction.quantity.ToString().Replace(',', '.'); ;


            return View(modelDemandProduct);
        }

        [HttpPost]
        public ActionResult EditRejectedOrders(DemandProductionViewModel form, long? Id)
        {
            DemandProductionViewModel modelDemand = new DemandProductionViewModel();
            binput = new BaseInput();

            BaseOutput demandOut = srv.WS_GetDemand_ProductionById(binput, (long)Id, true, out modelDemand.DemandProduction);

            DateTime startDate = (DateTime)form.startDate;
            DateTime endDate = (DateTime)form.endDate;


            modelDemand.DemandProduction.startDate = startDate.Ticks;
            modelDemand.DemandProduction.startDateSpecified = true;

            modelDemand.DemandProduction.endDate = endDate.Ticks;
            modelDemand.DemandProduction.endDateSpecified = true;


            ////get product type from the db given product type name

            //modelDemand.DemandProduction.product_Id = form.ProductType;
            //modelDemand.DemandProduction.product_IdSpecified = true;
            /////////////


            //update full product id
            var arr = modelDemand.DemandProduction.fullProductId;
            var b = arr.Split(',');
            b[b.Length - 1] = modelDemand.DemandProduction.product_Id.ToString();

            var c = string.Join(",", b);


            modelDemand.DemandProduction.fullProductId = c;


            modelDemand.DemandProduction.quantity = (decimal)float.Parse(form.Quantity.Replace('.', ',')); 
            modelDemand.DemandProduction.quantitySpecified = true;
            

            modelDemand.DemandProduction.title = form.Title;


            

            //edit state
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelDemand.EnumValue);
            modelDemand.DemandProduction.state_eV_Id = modelDemand.EnumValue.Id;
            modelDemand.DemandProduction.state_eV_IdSpecified = true;
            //update product potential product

            BaseOutput potUpdate = srv.WS_UpdateDemand_Production(binput, modelDemand.DemandProduction, out modelDemand.DemandProduction);


            //update production calendar
            BaseOutput productionTypeOut = srv.WS_GetEnumValueByName(binput, "Demand", out modelDemand.EnumValue);
            BaseOutput calendarOut = srv.WS_GetProductionCalendarByProductionId(binput, modelDemand.DemandProduction.Id, true, modelDemand.EnumValue.Id, true, out modelDemand.ProductionCalendar);

            modelDemand.ProductionCalendar.Months = form.checkedMonth;
            modelDemand.ProductionCalendar.Transportation_eV_Id = form.shippingSchedule;
            modelDemand.ProductionCalendar.Transportation_eV_IdSpecified = true;

            BaseOutput updateCalendar = srv.WS_UpdateProductionCalendar(binput, modelDemand.ProductionCalendar, out modelDemand.ProductionCalendar);
            return RedirectToAction("Index");
        }

        public ActionResult NotSentYetOrders(long?UserID)
        {
            SpecialSummaryViewModel modelSpecial = new SpecialSummaryViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;
            return View(modelSpecial);
        }


        public JsonResult GetOrganisations(long? orgId, long?UserId)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);

            BaseOutput ForeignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserId, true, out modelSpecial.ParentOrganisation);
            long organisationId = orgId == null ? (long)modelSpecial.Organisation.Id : (long)orgId;

            BaseOutput organisationByParentId = srv.WS_GetForeign_OrganisationsByParentId(binput, (long)organisationId, true, out modelSpecial.OrganisationArray);
            return Json(modelSpecial.OrganisationArray, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Organisations(long? UserId, long? orgId)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;

            BaseOutput ForeignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserId, true, out modelSpecial.ParentOrganisation);

            long organisationId = orgId == null ? (long)modelSpecial.ParentOrganisation.Id : (long)orgId;


            BaseOutput organisationByParentId = srv.WS_GetForeign_OrganisationsByParentId(binput, (long)organisationId, true, out modelSpecial.OrganisationArray);


          
            return View(modelSpecial);
        }
        public ActionResult AddChildOrganisation(long?UserId, long? parentId)
        {
            binput = new BaseInput();

            Session["arrONum"] = null;
            modelSpecial = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.User);
            modelSpecial.ParentOrganisationId = (long)parentId;


            BaseOutput edcationCatOut = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelSpecial.EnumCategory);
            BaseOutput eductationOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.EducationList = modelSpecial.EnumValueArray.ToList();

            BaseOutput jobCatOut = srv.WS_GetEnumCategorysByName(binput, "İş-Təşkilat", out modelSpecial.EnumCategory);
            BaseOutput jobbOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.JobList = modelSpecial.EnumValueArray.ToList();


            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


            BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();




            //get user roles to use 
            BaseOutput userRoleOut = srv.WS_GetUserRolesByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.UserRolesArray);

            bool addOrg = false;



            foreach (var role in modelSpecial.UserRolesArray)
            {
                if (role.RoleId == 21)
                {
                    addOrg = true;
                }
            }

            if (!addOrg)
            {
                TempData["denied"] = "denied";
                return RedirectToAction("Organisations");
            }

            return View("AddChildOrganisation", modelSpecial);
        }

        [HttpPost]
        public ActionResult AddChildOrganisation(long? UserId,SpecialSummaryViewModel form)
        {
            if (CheckExistence(form))
            {
                binput = new BaseInput();
                modelSpecial = new SpecialSummaryViewModel();

                modelSpecial.User = new tblUser();
                modelSpecial.UserRole = new tblUserRole();
                modelSpecial.Address = new tblAddress();
                modelSpecial.Organisation = new tblForeign_Organization();

                modelSpecial.User.Username = form.UserName;
                modelSpecial.User.Email = form.Email;
                modelSpecial.User.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);
                modelSpecial.User.Status = 1;

                BaseOutput personEnumOut = srv.WS_GetEnumValueByName(binput, "legalPerson", out modelSpecial.EnumValue);
                modelSpecial.User.userType_eV_ID = modelSpecial.EnumValue.Id;

                modelSpecial.User.userType_eV_IDSpecified = true;
                modelSpecial.User.StatusSpecified = true;


                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);

                binput.userName = modelSpecial.LoggedInUser.Username;
                BaseOutput userOut = srv.WS_AddUser(binput, modelSpecial.User, out modelSpecial.User);


                //give roles to user
                BaseOutput usertypeProducerOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelSpecial.Role);

                modelSpecial.UserRole.RoleId = modelSpecial.Role.Id;
                modelSpecial.UserRole.RoleIdSpecified = true;
                modelSpecial.UserRole.UserId = modelSpecial.User.Id;
                modelSpecial.UserRole.UserIdSpecified = true;
                modelSpecial.UserRole.Status = 1;
                modelSpecial.UserRole.StatusSpecified = true;
                BaseOutput addUserRole = srv.WS_AddUserRole(binput, modelSpecial.UserRole, out modelSpecial.UserRole);


                //add address informations
                modelSpecial.Address.fullAddress = form.FullAddress;
                modelSpecial.Address.Status = 1;
                modelSpecial.Address.StatusSpecified = true;
                modelSpecial.Address.user_Id = modelSpecial.User.Id;
                modelSpecial.Address.user_IdSpecified = true;
                modelSpecial.Address.user_type_eV_IdSpecified = true;
                modelSpecial.Address.adminUnit_Id = form.adId.LastOrDefault();

                modelSpecial.Address.adminUnit_IdSpecified = true;
                BaseOutput address = srv.WS_AddAddress(binput, modelSpecial.Address, out modelSpecial.Address);


                //add manager

                modelSpecial.Manager = new tblPerson();
                modelSpecial.Manager.Name = form.ManagerName;

                modelSpecial.Manager.PinNumber = form.Pin;
                modelSpecial.Manager.FatherName = form.FatherName;
                modelSpecial.Manager.Surname = form.Surname;
                modelSpecial.Manager.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
                modelSpecial.Manager.birtdaySpecified = true;
                modelSpecial.Manager.gender = form.Gender;

                BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelSpecial.EnumValue);
                modelSpecial.Manager.educationLevel_eV_Id = modelSpecial.EnumValue == null ? 0 : modelSpecial.EnumValue.Id;
                modelSpecial.Manager.educationLevel_eV_IdSpecified = true;

                BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelSpecial.EnumValue);
                modelSpecial.Manager.job_eV_Id = modelSpecial.EnumValue == null ? 0 : modelSpecial.EnumValue.Id;
                modelSpecial.Manager.job_eV_IdSpecified = true;

                modelSpecial.Manager.Status = 1;
                modelSpecial.Manager.StatusSpecified = true;


                BaseOutput managerOut = srv.WS_AddPerson(binput, modelSpecial.Manager, out modelSpecial.Manager);

                //add manager communication informations
                if (form.ManagerEmail != null)
                {
                    modelSpecial.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "email", out modelSpecial.EnumValue);
                    modelSpecial.ComunicationInformations.comType = (int)modelSpecial.EnumValue.Id;
                    modelSpecial.ComunicationInformations.comTypeSpecified = true;
                    modelSpecial.ComunicationInformations.communication = form.ManagerEmail;
                    modelSpecial.ComunicationInformations.description = form.ManagerEmail;
                    modelSpecial.ComunicationInformations.PersonId = modelSpecial.Manager.Id;
                    modelSpecial.ComunicationInformations.PersonIdSpecified = true;
                    modelSpecial.ComunicationInformations.priorty = 1;
                    modelSpecial.ComunicationInformations.priortySpecified = true;

                    BaseOutput comunicationOUtt = srv.WS_AddCommunication(binput, modelSpecial.ComunicationInformations, out modelSpecial.ComunicationInformations);
                }


                if (form.ManagerMobilePhone != null)
                {
                    modelSpecial.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelSpecial.EnumValue);
                    modelSpecial.ComunicationInformations.comType = (int)modelSpecial.EnumValue.Id;
                    modelSpecial.ComunicationInformations.comTypeSpecified = true;
                    modelSpecial.ComunicationInformations.communication = form.mobilePhonePrefix + form.ManagerMobilePhone;
                    modelSpecial.ComunicationInformations.description = form.ManagerMobilePhone;
                    modelSpecial.ComunicationInformations.PersonId = modelSpecial.Manager.Id;
                    modelSpecial.ComunicationInformations.PersonIdSpecified = true;
                    modelSpecial.ComunicationInformations.priorty = 2;
                    modelSpecial.ComunicationInformations.priortySpecified = true;
                    BaseOutput comunicationOUt = srv.WS_AddCommunication(binput, modelSpecial.ComunicationInformations, out modelSpecial.ComunicationInformations);

                }

                if (form.ManagerWorkPhone != null)
                {
                    modelSpecial.ComunicationInformations = new tblCommunication();
                    BaseOutput emailOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelSpecial.EnumValue);
                    modelSpecial.ComunicationInformations.comType = (int)modelSpecial.EnumValue.Id;
                    modelSpecial.ComunicationInformations.comTypeSpecified = true;
                    modelSpecial.ComunicationInformations.communication = form.WorkPhonePrefix + form.ManagerWorkPhone;
                    modelSpecial.ComunicationInformations.description = form.ManagerWorkPhone;
                    modelSpecial.ComunicationInformations.PersonId = modelSpecial.Manager.Id;
                    modelSpecial.ComunicationInformations.PersonIdSpecified = true;
                    modelSpecial.ComunicationInformations.priorty = 1;
                    modelSpecial.ComunicationInformations.priortySpecified = true;
                    BaseOutput comunicationnOUtt = srv.WS_AddCommunication(binput, modelSpecial.ComunicationInformations, out modelSpecial.ComunicationInformations);

                }
                //add foreign organisation
                modelSpecial.Organisation.name = form.Name;
                modelSpecial.Organisation.Status = 1;

                modelSpecial.Organisation.address_Id = modelSpecial.Address.Id;
                modelSpecial.Organisation.address_IdSpecified = true;

                modelSpecial.Organisation.userId = modelSpecial.User.Id;
                modelSpecial.Organisation.userIdSpecified = true;

                modelSpecial.Organisation.voen = form.Voen;
                modelSpecial.Organisation.manager_Id = modelSpecial.Manager.Id;

                modelSpecial.Organisation.manager_IdSpecified = true;

                modelSpecial.Organisation.parent_Id = form.ParentOrganisationId;
                modelSpecial.Organisation.parent_IdSpecified = true;

                BaseOutput foreignOrganisationOut = srv.WS_AddForeign_Organization(binput, modelSpecial.Organisation, out modelSpecial.Organisation);


                return RedirectToAction("Organisations");
            }
            else
            {
                TempData["ChildOrganisationExists"] = "Bu adda istifadəçi sistemdə mövcuddur.";
                return RedirectToAction("AddChildOrganisation", new { parentId = form.ParentOrganisationId });
            }
           
        }


        public ActionResult DeleteChildOrganisation(long?UserId, long?Id, long? parentId)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            BaseOutput UserOut = srv.WS_GetUserById(binput, (long)Id, true, out modelSpecial.User);
            BaseOutput foreignOrganisationOut = srv.WS_GetForeign_OrganizationByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.Organisation);
            BaseOutput addressesOut = srv.WS_GetAddressesByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.AddressArray);
            BaseOutput managerOut = srv.WS_GetPersonById(binput, (long)modelSpecial.Organisation.manager_Id, true, out modelSpecial.Manager);

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);

            //check authorization

            //get user roles of logged in user to use 
            BaseOutput userRoleOut = srv.WS_GetUserRolesByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.UserRolesArray);

            bool deleteOrg = false;

            foreach (var role in modelSpecial.UserRolesArray)
            {
                if (role.RoleId == 23)
                {
                    deleteOrg = true;
                }
            }

            if (!deleteOrg)
            {
                TempData["denied"] = "denied";
                return RedirectToAction("Organisations");
            }

            binput.userName = modelSpecial.LoggedInUser.Username;

            modelSpecial.User.updatedUser = binput.userName;

            BaseOutput deleteChildOrganisationUser = srv.WS_DeleteUser(binput, modelSpecial.User);
            BaseOutput deleteChildOrganisation = srv.WS_DeleteForeign_Organization(binput, modelSpecial.Organisation);

            //delete all the addreses of the child organisation
            foreach (var item in modelSpecial.AddressArray)
            {
                srv.WS_DeleteAddress(binput, item);
            }

            //delete all the roles of the child organisation
            BaseOutput userRolesOut = srv.WS_GetUserRolesByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.UserRolesArray);

            foreach (var item in modelSpecial.UserRolesArray)
            {
                BaseOutput deletedRole = srv.WS_DeleteUserRole(binput, item);
            }

            //delete manager of the child organisation
            BaseOutput deleteManager = srv.WS_DeletePerson(binput, modelSpecial.Manager);




            return RedirectToAction("Organisations", new { parentId = parentId});
        }

        public ActionResult EditChildOrganisation(long? UserId, long? Id)
        {
            binput = new BaseInput();

            Session["arrONum"] = null;
            modelSpecial = new SpecialSummaryViewModel();

            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.User);

            modelSpecial.OrganisationId = (long)Id; 
            //get the orgaisation  that is going to be updated
            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, (long)Id, true, out modelSpecial.Organisation);
            modelSpecial.Voen = modelSpecial.Organisation.voen;
            modelSpecial.Name = modelSpecial.Organisation.name;

            //get the user registered to the organisation
            BaseOutput orgOut = srv.WS_GetUserById(binput, (long)modelSpecial.Organisation.userId, true, out modelSpecial.User);
            modelSpecial.UserName = modelSpecial.User.Username;
            //modelSpecial.Password = modelSpecial.User.Password;
            modelSpecial.Email = modelSpecial.User.Email;

            //get the address of the organisation
            //this is biased. we do not know if an organisation can have more than one address.***********************
            BaseOutput addressOut = srv.WS_GetAddressById(binput, (long)modelSpecial.Organisation.address_Id, true, out modelSpecial.Address);
            modelSpecial.AdminUnitId = (long)modelSpecial.Address.adminUnit_Id;

            modelSpecial.FullAddress = modelSpecial.Address.fullAddress;
            //*********************************************************


            //get the address hierarchy this is also biased *************************
            //BaseOutput adminUnitOut = srv.WS_GetPRM_AdminUnitById(binput, (long)modelSpecial.AddressArray.FirstOrDefault().adminUnit_Id, true, out modelSpecial.PRMAdminUnit);
            //BaseOutput addressesOut = srv.WS_GETPRM_AdminUnitsByChildId(binput, modelSpecial.PRMAdminUnit, out modelSpecial.PRMAdminUnitArray);

            BaseOutput edcationCatOut = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelSpecial.EnumCategory);
            BaseOutput eductationOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.EducationList = modelSpecial.EnumValueArray.ToList();


            BaseOutput jobCatOut = srv.WS_GetEnumCategorysByName(binput, "İş-Təşkilat", out modelSpecial.EnumCategory);
            BaseOutput jobbOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.JobList = modelSpecial.EnumValueArray.ToList();

            //get manager infos
            BaseOutput ManagerOut = srv.WS_GetPersonById(binput, (long)modelSpecial.Organisation.manager_Id, true, out modelSpecial.Manager);
            modelSpecial.ManagerName = modelSpecial.Manager.Name;
            modelSpecial.Surname = modelSpecial.Manager.Surname;
            modelSpecial.FatherName = modelSpecial.Manager.FatherName;
            modelSpecial.Pin = modelSpecial.Manager.PinNumber;

            //get communication informations of the manager
            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);
            modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Manager.Id).ToList();


            //get emailtype enum value
            BaseOutput emailEnumOut = srv.WS_GetEnumValueByName(binput, "email", out modelSpecial.EnumValue);
            modelSpecial.ManagerEmail = modelSpecial.CommunicationInformationsList.Where(x => x.comType == modelSpecial.EnumValue.Id).ToList().Count == 0 ? null : modelSpecial.CommunicationInformationsList.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;


            //get MobilePhone type enum value
            BaseOutput mobilePhoneOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelSpecial.EnumValue);
            string a = modelSpecial.CommunicationInformationsList.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;
            modelSpecial.mobilePhonePrefix = a.Remove(a.Length - 7);
            modelSpecial.ManagerMobilePhone = a.Substring(modelSpecial.mobilePhonePrefix.Length, 7);

            //get WorkPhone type enum value
            BaseOutput workPhoneOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelSpecial.EnumValue);
            string b = modelSpecial.CommunicationInformationsList.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;
            modelSpecial.WorkPhonePrefix = b.Remove(b.Length -7);
            modelSpecial.ManagerWorkPhone = b.Substring(modelSpecial.WorkPhonePrefix.Length, 7);


            //get the gender
            modelSpecial.Gender = modelSpecial.Manager.gender;
            modelSpecial.ManagerEducation = (long)modelSpecial.Manager.educationLevel_eV_Id;
            modelSpecial.ManagerJob = (long)modelSpecial.Manager.job_eV_Id;

            //birthday not finished
            //modelSpecial.Birthday = modelSpecial.Manager.birtday;

            BaseOutput adminnOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.User);


            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


            BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

            //get user roles to use 
            BaseOutput userRoleOut = srv.WS_GetUserRolesByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.UserRolesArray);

            bool editOrg = false;

            foreach (var role in modelSpecial.UserRolesArray)
            {
                if (role.RoleId == 22)
                {
                    editOrg = true;
                }
            }

            if (!editOrg)
            {
                TempData["denied"] = "denied";
                return RedirectToAction("Organisations");
            }

            return View("EditChildOrganisation", modelSpecial);
        }

        [HttpPost]
        public ActionResult EditChildOrganisation(long?UserId,SpecialSummaryViewModel form, long OrganisationId)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.User = new tblUser();
            modelSpecial.Address = new tblAddress();
            modelSpecial.Organisation = new tblForeign_Organization();

            //update user

            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, OrganisationId, true, out modelSpecial.Organisation);
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)modelSpecial.Organisation.userId, true, out modelSpecial.User);

            modelSpecial.User.Username = form.UserName;
            modelSpecial.User.Email = form.Email;
            modelSpecial.User.Password = BCrypt.Net.BCrypt.HashPassword(form.Password, 5);


            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput adminOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);

            binput.userName = modelSpecial.LoggedInUser.Username;
            BaseOutput userUpdate = srv.WS_UpdateUser(binput, modelSpecial.User, out modelSpecial.User);

            //update address
            BaseOutput addressOUT = srv.WS_GetAddressById(binput, (long)modelSpecial.Organisation.address_Id, true, out modelSpecial.Address);
            modelSpecial.Address.fullAddress = form.FullAddress;
            modelSpecial.Address.adminUnit_Id = form.adId.LastOrDefault();
            BaseOutput address = srv.WS_UpdateAddress(binput, modelSpecial.Address);


            //update manager
            BaseOutput managerOut = srv.WS_GetPersonById(binput, (long)modelSpecial.Organisation.manager_Id, true, out modelSpecial.Manager);

            modelSpecial.Manager.Name = form.ManagerName;
            modelSpecial.Manager.PinNumber = form.Pin;
            modelSpecial.Manager.FatherName = form.FatherName;
            modelSpecial.Manager.Surname = form.Surname;
            modelSpecial.Manager.birtday = ConvertStringYearMonthDayFormatToTimestamp(form);
            modelSpecial.Manager.gender = form.Gender;

            BaseOutput educationEnum = srv.WS_GetEnumValueByName(binput, form.Education, out modelSpecial.EnumValue);
            modelSpecial.Manager.educationLevel_eV_Id = modelSpecial.EnumValue == null ? 0 : modelSpecial.EnumValue.Id;

            BaseOutput jobEnum = srv.WS_GetEnumValueByName(binput, form.Job, out modelSpecial.EnumValue);
            modelSpecial.Manager.job_eV_Id = modelSpecial.EnumValue == null ? 0 : modelSpecial.EnumValue.Id;


            BaseOutput updatemanager = srv.WS_UpdatePerson(binput, modelSpecial.Manager, out modelSpecial.Manager);

            //update manager communication informations

            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);
            modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Manager.Id).ToList();

            foreach (var item in modelSpecial.CommunicationInformationsList)
            {
                BaseOutput emailEnumOut = srv.WS_GetEnumValueByName(binput, "email", out modelSpecial.EnumValue);
                if(item.comType == modelSpecial.EnumValue.Id)
                {
                    item.communication = form.ManagerEmail;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelSpecial.ComunicationInformations);
                }

                BaseOutput mobilePhoneOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelSpecial.EnumValue);
                if (item.comType == modelSpecial.EnumValue.Id)
                {
                    item.communication = form.mobilePhonePrefix + form.ManagerMobilePhone;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelSpecial.ComunicationInformations);
                }
                BaseOutput workPhoneOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelSpecial.EnumValue);
                if (item.comType == modelSpecial.EnumValue.Id)
                {
                    item.communication = form.WorkPhonePrefix + form.ManagerWorkPhone;
                    BaseOutput updateemail = srv.WS_UpdateCommunication(binput, item, out modelSpecial.ComunicationInformations);
                }
            }
            //update address
            BaseOutput addressOut = srv.WS_GetAddressById(binput, (long)modelSpecial.Organisation.address_Id, true, out modelSpecial.Address);
            modelSpecial.Address.adminUnit_Id = form.adId.LastOrDefault();


            //update foreign organisation
            modelSpecial.Organisation.name = form.Name;
            modelSpecial.Organisation.voen = form.Voen;

            BaseOutput updateOrganisation = srv.WS_UpdateForeign_Organization(binput, modelSpecial.Organisation, out modelSpecial.Organisation);


            return RedirectToAction("Organisations","GovernmentOrganisationSpecialSummary");
        }


        public ActionResult OrganisationInfo(long?UserId, long?Id)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            //get the organisation
            BaseOutput organisationOut = srv.WS_GetForeign_OrganizationById(binput, (long)Id, true, out modelSpecial.Organisation);
            modelSpecial.Name = modelSpecial.Organisation.name;

            //get the manager
            BaseOutput managerOut = srv.WS_GetPersonById(binput, (long)modelSpecial.Organisation.manager_Id, true, out modelSpecial.Manager);
            modelSpecial.ManagerName = modelSpecial.Manager.Name;
            modelSpecial.Surname = modelSpecial.Manager.Surname;
            //get manager infos
            BaseOutput managerInfosOut = srv.WS_GetCommunicationByPersonId(binput, modelSpecial.Manager.Id, true, out modelSpecial.CommunicationInformationsArray);

            BaseOutput emailEnumOut = srv.WS_GetEnumValueByName(binput, "email", out modelSpecial.EnumValue);
            modelSpecial.ManagerEmail = modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;

            BaseOutput homePhoneOut = srv.WS_GetEnumValueByName(binput, "homePhone", out modelSpecial.EnumValue);
            modelSpecial.ManagerHomePhone = modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;

            BaseOutput mobilePhoneOut = srv.WS_GetEnumValueByName(binput, "mobilePhone", out modelSpecial.EnumValue);
            modelSpecial.ManagerMobilePhone = modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;

            BaseOutput workPhoneOut = srv.WS_GetEnumValueByName(binput, "workPhone", out modelSpecial.EnumValue);
            modelSpecial.ManagerWorkPhone = modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).ToList().Count == 0 ? "Sistemdə Yoxdur" : modelSpecial.CommunicationInformationsArray.Where(x => x.comType == modelSpecial.EnumValue.Id).FirstOrDefault().communication;


            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput userOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);
            modelSpecial.NameSurname = modelSpecial.LoggedInUser.Username;

            return View(modelSpecial);
        }

        public long ConvertStringYearMonthDayFormatToTimestamp(SpecialSummaryViewModel form)
        {
            string[] dates = Regex.Split(form.Birthday, @"\.");
            int year = dates.Length == 0 ? 0 : Convert.ToInt32(dates[2]);
            int month = Convert.ToInt32(dates[1]);
            int day = Convert.ToInt32(dates[0]);
            DateTime dTime = new DateTime(year, month, day);
            DateTime sTime = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            long birthday = (long)(dTime - sTime).TotalSeconds;

            return birthday;
        }


        public bool CheckExistence(SpecialSummaryViewModel form)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            BaseOutput checkExistenseOut = srv.WS_GetUserByUserName(binput, form.UserName, out modelSpecial.User);

            tblUser UserFromUsername = modelSpecial.User;


            BaseOutput checkEdxistenseOut = srv.WS_GetUsers(binput, out modelSpecial.UserArray);
            List<tblUser> UserFromEmail = modelSpecial.UserArray.Where(x => x.Email == form.Email).ToList();

            BaseOutput orgOut = srv.WS_GetForeign_Organizations(binput, out modelSpecial.OrganisationArray);
            List<tblForeign_Organization> OrgFromVoen = form.Voen == null ? new List<tblForeign_Organization>() : modelSpecial.OrganisationArray.Where(x => x.voen == form.Voen).ToList();


            if (modelSpecial.User != null || UserFromEmail.Count != 0 || OrgFromVoen.Count != 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public void UpdateOnAirDemands(int?UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();

            //get the informations of logged in user

            binput = new BaseInput();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }

            ////////////////////////////////////////////////////////////////


            //update on air demands
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();

            modelSpecial.DemandProduction.user_Id = UserID;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demand = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);


            foreach (var item in modelSpecial.DemandProductionArray)
            {
                if (item.endDate < DateTime.Now.Ticks)
                {
                    PutToExpiredDemands((int)item.Id);
                }
            }
            ///////////////////////////////////////////////////////////////////////

            //update confirmed demands

            BaseOutput enumdVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);
            modelSpecial.DemandProduction = new tblDemand_Production();

            modelSpecial.DemandProduction.user_Id = UserID;
            modelSpecial.DemandProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.DemandProduction.user_IdSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;

            BaseOutput demandd = srv.WS_GetDemand_ProductionsByStateAndUserID(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProductionArray);

            foreach (var item in modelSpecial.DemandProductionArray)
            {
                if (item.endDate < DateTime.Now.Ticks)
                {
                    PutToExpiredDemands((int)item.Id);
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////
        }

        public void PutToExpiredDemands(int demandId)
        {
            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.EnumValue = new tblEnumValue();

            binput = new BaseInput();
            //get demand production u want to put to off air demand
            BaseOutput Production = srv.WS_GetDemand_ProductionById(binput, demandId, true, out modelSpecial.DemandProduction);
            ///////////////////////////////

            //Delete demand production u want to put to off air offer, from demand production table


            //BaseOutput delet = srv.WS_DeleteDemand_Production(binput, modelSpecial.DemandProduction);

            ////////////////////////////////////////////////////

            //Add it to the off air demands table

            //get the enum Id of yayinda
            modelSpecial.EnumValue = new tblEnumValue();
            BaseOutput eval = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelSpecial.EnumValue);
            long enumYayindaDeyil = modelSpecial.EnumValue.Id;
            ///////////////////////////

            modelSpecial.DemandProduction.state_eV_Id = enumYayindaDeyil;
            modelSpecial.DemandProduction.Status = 1;

            modelSpecial.DemandProduction.StatusSpecified = true;
            modelSpecial.DemandProduction.state_eV_IdSpecified = true;
            BaseOutput addProduct = srv.WS_UpdateDemand_Production(binput, modelSpecial.DemandProduction, out modelSpecial.DemandProduction);

            ////////////////////////////////

        }

    }
}
