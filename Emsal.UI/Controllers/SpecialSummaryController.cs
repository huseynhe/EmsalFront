using System;
using System.Linq;
using System.Web.Mvc;
using Emsal.WebInt.EmsalSrv;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using Emsal.UI.Models;
using System.Web.Security;
using System.Web;
using Emsal.UI.Infrastructure;
using System.Collections.Generic;
using PagedList;
using Emsal.Utility.CustomObjects;
using WordDoc.Models;

namespace Emsal.UI.Controllers
{
    //[EmsalAuthorization(AuthorizedAction = ActionName.specialSummary)]

    public class SpecialSummaryController : Controller
    {

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private BaseInput binput;
        SpecialSummaryViewModel modelSpecial;
        UserViewModel modelUser;
        //
        // GET: /SpecialSummary/

        public ActionResult Index(int? page, long? UserId)
        {
            binput = new BaseInput();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();



            //get the informations of logged in user
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            List<products> productsList = new List<products>();
            productsList = getCountsOffProducts((long)UserId);

            modelSpecial.countNewAcceptedOffers = productsList.Where(p => p.type == 1).Count();
            modelSpecial.countNewOffAirOffers = productsList.Where(p => p.type == 3).Count();
            modelSpecial.countNewOnAirOffers = productsList.Where(p => p.type == 2).Count();
            modelSpecial.countNewRejectedOffers = productsList.Where(p => p.type == 4).Count();

            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.LoggedInUser);

            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.Person);
            BaseOutput orgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserId, true, out modelSpecial.ForeignOrganisation);

            if (modelSpecial.Person.educationLevel_eV_Id != null)
            {
                BaseOutput eduOut = srv.WS_GetEnumValueById(binput, (long)modelSpecial.Person.educationLevel_eV_Id, true, out modelSpecial.EnumValue);
                modelSpecial.Education = modelSpecial.EnumValue.description;
            }

            if (modelSpecial.Person.job_eV_Id != null)
            {
                BaseOutput jobOut = srv.WS_GetEnumValueById(binput, (long)modelSpecial.Person.job_eV_Id, true, out modelSpecial.EnumValue);
                modelSpecial.Job = modelSpecial.EnumValue.description;
            }

            modelSpecial.NameSurname = modelSpecial.Person == null ? modelSpecial.ForeignOrganisation.name : modelSpecial.Person.Name + ' ' + modelSpecial.Person.Surname;
            if(modelSpecial.ForeignOrganisation==null)
            {
                modelSpecial.ForeignOrganisation = new tblForeign_Organization();
            }
            //get communications
            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);

            if (modelSpecial.LoggedInUser.userType_eV_ID == 26)
            {
                modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Person.Id).ToList();
            }

            if (modelSpecial.LoggedInUser.userType_eV_ID == 50)
            {
                modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.ForeignOrganisation.manager_Id).ToList();
            }

            if (modelSpecial.CommunicationInformationsList != null)
            {
                foreach (var item in modelSpecial.CommunicationInformationsList)
                {
                    //if (modelSpecial.LoggedInUserInfos == null)
                       // break;
                    if (item.comType == 10120)
                    {
                        modelSpecial.MobilePhone = item.communication;
                    }
                    if (item.comType == 10122)
                    {
                        modelSpecial.WorkPhone = item.communication;
                    }

                }
            }



            ///toget confirmed on air offers
            //firts get the type tesdiwlenen (approved offers) from table
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);
            modelSpecial.OfferProduction = new tblOffer_Production();

            modelSpecial.OfferProduction.user_Id = UserId;
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.OfferProduction.user_IdSpecified = true;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;

            BaseOutput enumValr = srv.WS_GetEnumValueByName(binput, "reedited", out modelSpecial.EnumValue);

            modelSpecial.OfferProduction.monitoring_eV_Id = modelSpecial.EnumValue.Id;
            modelSpecial.OfferProduction.monitoring_eV_IdSpecified = true;

            BaseOutput offer = srv.WS_GetOffer_ProductionsByUserID(binput, (long)UserId, true, out modelSpecial.OfferProductionArray);
            modelSpecial.OfferProductionList = modelSpecial.OfferProductionArray.Where(p => (p.state_eV_Id == 2 && p.monitoring_eV_Id == 2) || (p.state_eV_Id == 2 && p.monitoring_eV_Id == 10118)).ToList();

            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();
            modelSpecial.ProductDocumentList = new List<tblProduct_Document>();
            modelSpecial.SpOfferList = new List<SpecialSummaryPotentialAndOffer>();
            foreach (var item in modelSpecial.OfferProductionList)
            {
                modelSpecial.SpOffer = new SpecialSummaryPotentialAndOffer();
                BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                //get the id of the offered product
                modelSpecial.SpOffer.ProductId = item.Id;

                //get the name of the offered product
                modelSpecial.SpOffer.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the endddate of the offered product
                modelSpecial.SpOffer.ProductEndDate = (long)item.endDate;

                //get the quantity of the offered product
                modelSpecial.SpOffer.ProductQuantity = item.quantity != null ? (long)item.quantity : 0;


                //get the total price of the offered product
                modelSpecial.SpOffer.ProductTotalPrice = item.total_price != null ? (double)item.total_price : 0;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.SpOffer.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);

                //get the parent name of the product
                modelSpecial.SpOffer.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the offered product quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.SpOffer.QuantityType = modelSpecial.EnumValue.name;
                    }
                }

                //get the informations in the product calendar
                modelSpecial.SpOffer.DemandCalendarList = new List<DemandCalendar>();

                BaseOutput prodcuctCalendarOut = srv.WS_GetProductionCalendarProductionId2(binput, item.Id, true, out modelSpecial.ProductionCalendarArray);
                modelSpecial.ProductionCalendarArray = modelSpecial.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == 3).ToArray();
                foreach (var itemm in modelSpecial.ProductionCalendarArray)
                {
                    modelSpecial.SpOffer.DemandCalendar = new DemandCalendar();

                    BaseOutput monthOut = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                    modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.name;
                    modelSpecial.SpOffer.DemandCalendar.day = itemm.day != null ? itemm.day.ToString() : null;
                    modelSpecial.SpOffer.DemandCalendar.ocklock = itemm.oclock.ToString();

                    BaseOutput shipmenttypeOUt = srv.WS_GetEnumValueById(binput, (long)itemm.type_eV_Id, true, out modelSpecial.EnumValue);
                    modelSpecial.SpOffer.DemandCalendar.shipmetType = modelSpecial.EnumValue.name;

                    BaseOutput monthOUt = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                    modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.description;
                    modelSpecial.SpOffer.DemandCalendar.quantity = itemm.quantity != null ? itemm.quantity.ToString() : null;
                    modelSpecial.SpOffer.DemandCalendar.price = itemm.price.ToString();


                    modelSpecial.SpOffer.DemandCalendarList.Add(modelSpecial.SpOffer.DemandCalendar);
                }

                modelSpecial.SpOfferList.Add(modelSpecial.SpOffer);

                tblOffer_Production mtbl = new tblOffer_Production();
                if (item.isNew == 1)
                {
                    item.isNew = 0;
                    BaseOutput up = srv.WS_UpdateOffer_Production(binput, item, out mtbl);
                }
            }
            modelSpecial.PagingConfirmedOffer = modelSpecial.SpOfferList.ToPagedList(pageNumber, pageSize);

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserId, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();

            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


            BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

            

            BaseOutput contOut = srv.WS_GetContractBySupplierUserID(binput, modelSpecial.Person.Id, true, out modelSpecial.ContractArray);

            //modelSpecial.Contract = new tblContract();
            //BaseOutput contOut = srv.WS_GetContractBySupplierUserID()

            if (UserId > 0)
            {
                BaseOutput userRolId = srv.WS_GetUserRolesByUserId(binput, (long)UserId, true, out modelSpecial.UserRoleArray);

                modelSpecial.UserRole = modelSpecial.UserRoleArray.ToList().Where(x => (x.RoleId == 11 || x.RoleId == 24)).FirstOrDefault();

                if (modelSpecial.UserRole != null)
                    modelSpecial.roleStatus = 1;
            }

            modelSpecial.Birthday = String.Format("{0:d.M.yyyy}", (modelSpecial.Person.birtday).toShortDate());

            return Request.IsAjaxRequest()
            ? (ActionResult)PartialView("PartialIndex", modelSpecial)
            : View(modelSpecial);
        }

        public ActionResult OnAirOffers(int? page, int? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            
            modelSpecial.OfferProduction = new tblOffer_Production();
            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
            modelSpecial.ProductionControlList = new List<tblProductionControl>();
            modelSpecial.ProductDocumentList = new List<tblProduct_Document>();

            //get the logged in user  informations
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }


            List<products> productsList = new List<products>();
            productsList = getCountsOffProducts((long)UserID);

            modelSpecial.countNewAcceptedOffers = productsList.Where(p => p.type == 1).Count();
            modelSpecial.countNewOffAirOffers = productsList.Where(p => p.type == 3).Count();
            modelSpecial.countNewOnAirOffers = productsList.Where(p => p.type == 2).Count();
            modelSpecial.countNewRejectedOffers = productsList.Where(p => p.type == 4).Count();

            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.Person);
            BaseOutput orgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserID, true, out modelSpecial.ForeignOrganisation);

            modelSpecial.NameSurname = modelSpecial.Person == null ? modelSpecial.ForeignOrganisation.name : modelSpecial.Person.Name + ' ' + modelSpecial.Person.Surname;
            ///////////////////////////////////////////


            //get the on air offer productions
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelSpecial.EnumValue);
            modelSpecial.OfferProduction.user_Id = UserID;
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;


            modelSpecial.OfferProduction.user_IdSpecified = true;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;

            BaseOutput enumValr = srv.WS_GetEnumValueByName(binput, "reedited", out modelSpecial.EnumValue);

            modelSpecial.OfferProduction.monitoring_eV_Id = modelSpecial.EnumValue.Id;
            modelSpecial.OfferProduction.monitoring_eV_IdSpecified = true;

            BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelSpecial.OfferProduction, out modelSpecial.OfferProductionArray);

            modelSpecial.OfferProductionList = modelSpecial.OfferProductionArray.ToList();
            ///////////////////////////////////////////////

            modelSpecial.SpOfferList = new List<SpecialSummaryPotentialAndOffer>();

            foreach (var item in modelSpecial.OfferProductionList)
            {
                modelSpecial.SpOffer = new SpecialSummaryPotentialAndOffer();
                BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                //get the id of the offered product
                modelSpecial.SpOffer.ProductId = item.Id;

                //get the name of the offered product
                modelSpecial.SpOffer.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the endddate of the offered product
                modelSpecial.SpOffer.ProductEndDate = (long)item.endDate;

                //get the quantity of the offered product
                modelSpecial.SpOffer.ProductQuantity = item.quantity != null ? (long)item.quantity : 0;

                //get the total price of the offered product
                modelSpecial.SpOffer.ProductTotalPrice = item.total_price != null ? (double)item.total_price : 0;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.SpOffer.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);

                //get the parent name of the product
                modelSpecial.SpOffer.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the offered product quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.SpOffer.QuantityType = modelSpecial.EnumValue.name;
                    }
                }


                //get the informations in the product calendar
                modelSpecial.SpOffer.DemandCalendarList = new List<DemandCalendar>();

                BaseOutput prodcuctCalendarOut = srv.WS_GetProductionCalendarProductionId2(binput, item.Id, true, out modelSpecial.ProductionCalendarArray);
                modelSpecial.ProductionCalendarArray = modelSpecial.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == 3).ToArray();
                foreach (var itemm in modelSpecial.ProductionCalendarArray)
                {
                    modelSpecial.SpOffer.DemandCalendar = new DemandCalendar();

                    BaseOutput monthOut = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                    modelSpecial.SpOffer.DemandCalendar.year = itemm.year.ToString();
                    modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.name;
                    modelSpecial.SpOffer.DemandCalendar.day = itemm.day != null ? itemm.day.ToString() : null;
                    modelSpecial.SpOffer.DemandCalendar.ocklock = itemm.oclock.ToString();

                    BaseOutput shipmenttypeOUt = srv.WS_GetEnumValueById(binput, (long)itemm.type_eV_Id, true, out modelSpecial.EnumValue);
                    modelSpecial.SpOffer.DemandCalendar.shipmetType = modelSpecial.EnumValue.name;

                    BaseOutput monthOUt = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                    modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.description;
                    modelSpecial.SpOffer.DemandCalendar.quantity = itemm.quantity != null ? itemm.quantity.ToString() : null;
                    modelSpecial.SpOffer.DemandCalendar.price = itemm.price.ToString();


                    modelSpecial.SpOffer.DemandCalendarList.Add(modelSpecial.SpOffer.DemandCalendar);
                }



                modelSpecial.SpOfferList.Add(modelSpecial.SpOffer);

                tblOffer_Production mtbl = new tblOffer_Production();
                if (item.isNew == 1)
                {
                    item.isNew = 0;
                    BaseOutput up = srv.WS_UpdateOffer_Production(binput, item, out mtbl);
                }
            }

            modelSpecial.PagingOffer = modelSpecial.SpOfferList.ToPagedList(pageNumber, pageSize);


            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserID, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();


            //get communications
            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);

            if (modelSpecial.LoggedInUser.userType_eV_ID == 26)
            {
                modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Person.Id).ToList();
            }

            if (modelSpecial.LoggedInUser.userType_eV_ID == 50)
            {
                modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.ForeignOrganisation.manager_Id).ToList();
            }

            foreach (var item in modelSpecial.CommunicationInformationsList)
            {
                if (item.comType == 10120)
                {
                    modelSpecial.MobilePhone = item.description;
                }
                if (item.comType == 10122)
                {
                    modelSpecial.WorkPhone = item.description;
                }

            }

            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


            BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

            if (UserID > 0)
            {
                BaseOutput userRolId = srv.WS_GetUserRolesByUserId(binput, (long)UserID, true, out modelSpecial.UserRoleArray);

                modelSpecial.UserRole = modelSpecial.UserRoleArray.ToList().Where(x => (x.RoleId == 11 || x.RoleId == 24)).FirstOrDefault();

                if (modelSpecial.UserRole != null)
                    modelSpecial.roleStatus = 1;
            }


            return PartialView(modelSpecial);

        }

        public ActionResult RejectedOffers(int? page, int? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            try
            {
                modelSpecial.OfferProduction = new tblOffer_Production();
                modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
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


                List<products> productsList = new List<products>();
                productsList = getCountsOffProducts((long)UserID);

                modelSpecial.countNewAcceptedOffers = productsList.Where(p => p.type == 1).Count();
                modelSpecial.countNewOffAirOffers = productsList.Where(p => p.type == 3).Count();
                modelSpecial.countNewOnAirOffers = productsList.Where(p => p.type == 2).Count();
                modelSpecial.countNewRejectedOffers = productsList.Where(p => p.type == 4).Count();

                BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
                BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.Person);
                BaseOutput orgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserID, true, out modelSpecial.ForeignOrganisation);

                modelSpecial.NameSurname = modelSpecial.Person == null ? modelSpecial.ForeignOrganisation.name : modelSpecial.Person.Name + ' ' + modelSpecial.Person.Surname;
                ///////////////////////////////

                //get the rejected offer productions

                //BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "reject", out modelSpecial.EnumValue);

                //modelSpecial.OfferProduction.user_Id = UserID;
                //modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;

                //modelSpecial.OfferProduction.user_IdSpecified = true;
                //modelSpecial.OfferProduction.state_eV_IdSpecified = true;

                //BaseOutput enumValr = srv.WS_GetEnumValueByName(binput, "reedited", out modelSpecial.EnumValue);
                //modelSpecial.OfferProduction.monitoring_eV_Id = modelSpecial.EnumValue.Id;
                //modelSpecial.OfferProduction.monitoring_eV_IdSpecified = true;

                //modelSpecial.OfferProduction.Status =2;
                //modelSpecial.OfferProduction.StatusSpecified = true;

                BaseOutput offer = srv.WS_GetOffer_ProductionsByUserID(binput, (long)UserID, true ,out modelSpecial.OfferProductionArray);

                //////////////////////////////////////////////////


                modelSpecial.OfferProductionList = modelSpecial.OfferProductionArray.Where(p => ((p.state_eV_Id == 41 && p.monitoring_eV_Id == 10118) ||( p.state_eV_Id == 2 && p.monitoring_eV_Id == 41) || (p.state_eV_Id == 2 && p.monitoring_eV_Id ==10117)) && p.state_eV_Id != 1) .ToList();
                modelSpecial.SpOfferList = new List<SpecialSummaryPotentialAndOffer>();

                foreach (var item in modelSpecial.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                    modelSpecial.SpOffer = new SpecialSummaryPotentialAndOffer();
                    //get the id of the offered product
                    modelSpecial.SpOffer.ProductId = item.Id;

                    //get the name of the offered product
                    modelSpecial.SpOffer.ProductName = modelSpecial.ProductCatalog.ProductName;

                    //get the endddate of the offered product
                    modelSpecial.SpOffer.ProductEndDate = (long)item.endDate;

                    //get the quantity of the offered product
                    modelSpecial.SpOffer.ProductQuantity = item.quantity != null ? (long)item.quantity : 0;

                    //get the total price of the offered product
                    modelSpecial.SpOffer.ProductTotalPrice = item.total_price != null ? (double)item.total_price : 0;

                    BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                    //get the profile picture of the offered product
                    modelSpecial.SpOffer.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                    BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);

                    //get the parent name of the product
                    modelSpecial.SpOffer.ParentName = modelSpecial.ProductCatalog.ProductName;

                    //get the offered product quantity unit
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                    foreach (var itemm in modelSpecial.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                            modelSpecial.SpOffer.QuantityType = modelSpecial.EnumValue.name;
                        }
                    }

                    //get the informations in the product calendar
                    modelSpecial.SpOffer.DemandCalendarList = new List<DemandCalendar>();

                    BaseOutput prodcuctCalendarOut = srv.WS_GetProductionCalendarProductionId2(binput, item.Id, true, out modelSpecial.ProductionCalendarArray);
                    modelSpecial.ProductionCalendarArray = modelSpecial.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == 3).ToArray();

                    foreach (var itemm in modelSpecial.ProductionCalendarArray)
                    {
                        modelSpecial.SpOffer.DemandCalendar = new DemandCalendar();

                        BaseOutput monthOut = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                        modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.name;
                        modelSpecial.SpOffer.DemandCalendar.year = itemm.year.ToString();
                        modelSpecial.SpOffer.DemandCalendar.day = itemm.day != null ? itemm.day.ToString() : null;
                        modelSpecial.SpOffer.DemandCalendar.ocklock = itemm.oclock.ToString();

                        BaseOutput shipmenttypeOUt = srv.WS_GetEnumValueById(binput, (long)itemm.type_eV_Id, true, out modelSpecial.EnumValue);
                        modelSpecial.SpOffer.DemandCalendar.shipmetType = modelSpecial.EnumValue.name;

                        BaseOutput monthOUt = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                        modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.description;
                        modelSpecial.SpOffer.DemandCalendar.quantity = itemm.quantity != null ? itemm.quantity.ToString() : null;
                        modelSpecial.SpOffer.DemandCalendar.price = itemm.price.ToString();


                        modelSpecial.SpOffer.DemandCalendarList.Add(modelSpecial.SpOffer.DemandCalendar);
                    }

                    modelSpecial.SpOfferList.Add(modelSpecial.SpOffer);

                    tblOffer_Production mtbl = new tblOffer_Production();
                    if (item.isNew == 1)
                    {
                        item.isNew = 0;
                        BaseOutput up = srv.WS_UpdateOffer_Production(binput, item, out mtbl);
                    }
                }

                modelSpecial.PagingRejectedOffer = modelSpecial.SpOfferList.ToPagedList(pageNumber, pageSize);


                //get the inbox messages
                BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.ComMessageArray);
                modelSpecial.ComMessageList = modelSpecial.ComMessageArray == null ? null : modelSpecial.ComMessageArray.ToList();
                modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();


                //get communications
                BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);

                if (modelSpecial.LoggedInUser.userType_eV_ID == 26)
                {
                    modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Person.Id).ToList();
                }

                if (modelSpecial.LoggedInUser.userType_eV_ID == 50)
                {
                    modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.ForeignOrganisation.manager_Id).ToList();
                }

                foreach (var item in modelSpecial.CommunicationInformationsList)
                {
                    if (item.comType == 10120)
                    {
                        modelSpecial.MobilePhone = item.description;
                    }
                    if (item.comType == 10122)
                    {
                        modelSpecial.WorkPhone = item.description;
                    }

                }

                BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
                BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


                BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
                BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

                if (UserID > 0)
                {
                    BaseOutput userRolId = srv.WS_GetUserRolesByUserId(binput, (long)UserID, true, out modelSpecial.UserRoleArray);

                    modelSpecial.UserRole = modelSpecial.UserRoleArray.ToList().Where(x => (x.RoleId == 11 || x.RoleId == 24)).FirstOrDefault();

                    if (modelSpecial.UserRole != null)
                        modelSpecial.roleStatus = 1;
                }

                return PartialView(modelSpecial);
            }
            catch (Exception err)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult OffAirOffers(int? page, int? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            try
            {
                modelSpecial.OfferProduction = new tblOffer_Production();
                modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
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

                List<products> productsList = new List<products>();
                productsList = getCountsOffProducts((long)UserID);

                modelSpecial.countNewAcceptedOffers = productsList.Where(p => p.type == 1).Count();
                modelSpecial.countNewOffAirOffers = productsList.Where(p => p.type == 3).Count();
                modelSpecial.countNewOnAirOffers = productsList.Where(p => p.type == 2).Count();
                modelSpecial.countNewRejectedOffers = productsList.Where(p => p.type == 4).Count();

                BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
                BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.Person);
                BaseOutput orgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserID, true, out modelSpecial.ForeignOrganisation);

                modelSpecial.NameSurname = modelSpecial.Person == null ? modelSpecial.ForeignOrganisation.name : modelSpecial.Person.Name + ' ' + modelSpecial.Person.Surname;
                ////////////////////////////////

                //get the offair offer productions
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelSpecial.EnumValue);

                modelSpecial.OfferProduction.user_Id = UserID;
                modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;

                modelSpecial.OfferProduction.user_IdSpecified = true;
                modelSpecial.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput enumValr = srv.WS_GetEnumValueByName(binput, "reedited", out modelSpecial.EnumValue);

                modelSpecial.OfferProduction.monitoring_eV_Id = modelSpecial.EnumValue.Id;
                modelSpecial.OfferProduction.monitoring_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOffAirOffer_ProductionsByUserID(binput, modelSpecial.OfferProduction, out modelSpecial.OfferProductionArray);
                modelSpecial.OfferProductionList = modelSpecial.OfferProductionArray.ToList();

                ////////////////////////

                modelSpecial.SpOfferList = new List<SpecialSummaryPotentialAndOffer>();

                foreach (var item in modelSpecial.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                    modelSpecial.SpOffer = new SpecialSummaryPotentialAndOffer();
                    //get the id of the offered product
                    modelSpecial.SpOffer.ProductId = item.Id;

                    //get the name of the offered product
                    modelSpecial.SpOffer.ProductName = modelSpecial.ProductCatalog.ProductName;

                    //get the endddate of the offered product
                    modelSpecial.SpOffer.ProductEndDate = (long)item.endDate;

                    //get the quantity of the offered product
                    modelSpecial.SpOffer.ProductQuantity = item.quantity != null ? (long)item.quantity : 0;

                    //get the total price of the offered product
                    modelSpecial.SpOffer.ProductTotalPrice = item.total_price != null ? (double)item.total_price : 0;

                    BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                    //get the profile picture of the offered product
                    modelSpecial.SpOffer.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                    BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);

                    //get the parent name of the product
                    modelSpecial.SpOffer.ParentName = modelSpecial.ProductCatalog.ProductName;

                    //get the offered product quantity unit
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                    foreach (var itemm in modelSpecial.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                            modelSpecial.SpOffer.QuantityType = modelSpecial.EnumValue.name;
                        }
                    }


                    //get the informations in the product calendar
                    modelSpecial.SpOffer.DemandCalendarList = new List<DemandCalendar>();

                    BaseOutput prodcuctCalendarOut = srv.WS_GetProductionCalendarProductionId2(binput, item.Id, true, out modelSpecial.ProductionCalendarArray);
                    modelSpecial.ProductionCalendarArray = modelSpecial.ProductionCalendarArray.Where(x => x.Production_type_eV_Id == 3).ToArray();

                    foreach (var itemm in modelSpecial.ProductionCalendarArray)
                    {
                        modelSpecial.SpOffer.DemandCalendar = new DemandCalendar();

                        BaseOutput monthOut = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                        modelSpecial.SpOffer.DemandCalendar.year = itemm.year.ToString();
                        modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.name;
                        modelSpecial.SpOffer.DemandCalendar.day = itemm.day != null ? itemm.day.ToString() : null;
                        modelSpecial.SpOffer.DemandCalendar.ocklock = itemm.oclock.ToString();

                        BaseOutput shipmenttypeOUt = srv.WS_GetEnumValueById(binput, (long)itemm.type_eV_Id, true, out modelSpecial.EnumValue);
                        modelSpecial.SpOffer.DemandCalendar.shipmetType = modelSpecial.EnumValue.name;

                        BaseOutput monthOUt = srv.WS_GetEnumValueById(binput, (long)itemm.months_eV_Id, true, out modelSpecial.EnumValue);

                        modelSpecial.SpOffer.DemandCalendar.month = modelSpecial.EnumValue.description;
                        modelSpecial.SpOffer.DemandCalendar.quantity = itemm.quantity != null ? itemm.quantity.ToString() : null;

                        modelSpecial.SpOffer.DemandCalendar.price = itemm.price.ToString();
                        modelSpecial.SpOffer.DemandCalendarList.Add(modelSpecial.SpOffer.DemandCalendar);
                    }

                    modelSpecial.SpOfferList.Add(modelSpecial.SpOffer);

                    SpecialSummaryViewModel sp = new SpecialSummaryViewModel();
                    sp.OfferProduction = new tblOffer_Production();
                    if (item.isNew == 1)
                    {
                        item.isNew = 0;
                        BaseOutput up = srv.WS_UpdateOffer_Production(binput, item,out modelSpecial.OfferProduction);
                    }
                }

                modelSpecial.PagingOffAirOffer = modelSpecial.SpOfferList.ToPagedList(pageNumber, pageSize);

                //get the inbox messages
                BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserID, true, out modelSpecial.NotReadComMessageArray);
                modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
                modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();

                //get communications
                BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);

                if (modelSpecial.LoggedInUser.userType_eV_ID == 26)
                {
                    modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Person.Id).ToList();
                }

                if (modelSpecial.LoggedInUser.userType_eV_ID == 50)
                {
                    modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.ForeignOrganisation.manager_Id).ToList();
                }

                foreach (var item in modelSpecial.CommunicationInformationsList)
                {
                    if (item.comType == 10120)
                    {
                        modelSpecial.MobilePhone = item.description;
                    }
                    if (item.comType == 10122)
                    {
                        modelSpecial.WorkPhone = item.description;
                    }

                }

                BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
                BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


                BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
                BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

                if (UserID > 0)
                {
                    BaseOutput userRolId = srv.WS_GetUserRolesByUserId(binput, (long)UserID, true, out modelSpecial.UserRoleArray);

                    modelSpecial.UserRole = modelSpecial.UserRoleArray.ToList().Where(x => (x.RoleId == 11 || x.RoleId == 24)).FirstOrDefault();

                    if (modelSpecial.UserRole != null)
                        modelSpecial.roleStatus = 1;
                }

                return PartialView(modelSpecial);
            }
            catch (Exception err)
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult ConfirmedPotential(int? page, int? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);
            modelSpecial.PotentialProduction = new tblPotential_Production();
            modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
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
            BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
            BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.Person);
            BaseOutput orgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserID, true, out modelSpecial.ForeignOrganisation);

            modelSpecial.NameSurname = modelSpecial.Person == null ? modelSpecial.ForeignOrganisation.name : modelSpecial.Person.Name + ' ' + modelSpecial.Person.Surname;
            ////////////////////////////////////

            //get the confirmed potential productions of the logged in user                
            modelSpecial.PotentialProduction.user_Id = UserID;
            modelSpecial.PotentialProduction.state_eV_Id = modelSpecial.EnumValue.Id;

            modelSpecial.PotentialProduction.user_IdSpecified = true;
            modelSpecial.PotentialProduction.state_eV_IdSpecified = true;

            BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserId(binput, modelSpecial.PotentialProduction, out modelSpecial.PotentialProductionArray);

            modelSpecial.PotentialProductionList = modelSpecial.PotentialProductionArray.ToList();
            ///////////////////////////////////////////////////////////////////////////////////////////////

            modelSpecial.SpOfferList = new List<SpecialSummaryPotentialAndOffer>();
            foreach (var item in modelSpecial.PotentialProductionList)
            {
                BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);

                modelSpecial.SpOffer = new SpecialSummaryPotentialAndOffer();
                //get the id of the offered product
                modelSpecial.SpOffer.ProductId = item.Id;

                //get the name of the offered product
                modelSpecial.SpOffer.ProductName = modelSpecial.ProductCatalog.ProductName;

                //get the endddate of the offered product
                modelSpecial.SpOffer.ProductEndDate = item.endDate == null ? 0 : (long)item.endDate;

                //get the quantity of the offered product
                modelSpecial.SpOffer.ProductQuantity = item.quantity == null ? 0 : (long)item.quantity;

                //get the total price of the offered product
                modelSpecial.SpOffer.ProductTotalPrice = (item.unit_price == null || item.quantity == null) ? 0 : (double)item.unit_price * (double)item.quantity;

                BaseOutput producttionDocumentsOut = srv.WS_GetProductDocumentsByProductCatalogId(binput, modelSpecial.ProductCatalog.Id, true, out modelSpecial.ProductDocumentArray);

                //get the profile picture of the offered product
                modelSpecial.SpOffer.ProductProfilePicture = modelSpecial.ProductDocumentArray.Length == 0 ? null : modelSpecial.ProductDocumentArray.LastOrDefault().documentUrl + modelSpecial.ProductDocumentArray.LastOrDefault().documentName;

                BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);

                //get the parent name of the product
                modelSpecial.SpOffer.ParentName = modelSpecial.ProductCatalog.ProductName;

                //get the offered product quantity unit
                BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                foreach (var itemm in modelSpecial.ProductionControlArray)
                {
                    if (itemm.EnumCategoryId == 5)
                    {
                        BaseOutput quantityOut = srv.WS_GetEnumValueById(binput, (long)itemm.EnumValueId, true, out modelSpecial.EnumValue);
                        modelSpecial.SpOffer.QuantityType = modelSpecial.EnumValue.name;
                    }
                }

                modelSpecial.SpOfferList.Add(modelSpecial.SpOffer);
            }

            modelSpecial.PagingConfirmedPotential = modelSpecial.SpOfferList.ToPagedList(pageNumber, pageSize);

            //get the inbox messages
            BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserID, true, out modelSpecial.NotReadComMessageArray);
            modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
            modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();

            //get communications
            BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);

            if (modelSpecial.LoggedInUser.userType_eV_ID == 26)
            {
                modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Person.Id).ToList();
            }

            if (modelSpecial.LoggedInUser.userType_eV_ID == 50)
            {
                modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.ForeignOrganisation.manager_Id).ToList();
            }

            foreach (var item in modelSpecial.CommunicationInformationsList)
            {
                if (item.comType == 10120)
                {
                    modelSpecial.LoggedInUserInfos.MobilePhone = item.description;
                }
                if (item.comType == 10122)
                {
                    modelSpecial.LoggedInUserInfos.WorkPhone = item.description;
                }

            }

            BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


            BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
            BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

            if (UserID > 0)
            {
                BaseOutput userRolId = srv.WS_GetUserRolesByUserId(binput, (long)UserID, true, out modelSpecial.UserRoleArray);

                modelSpecial.UserRole = modelSpecial.UserRoleArray.ToList().Where(x => (x.RoleId == 11 || x.RoleId == 24)).FirstOrDefault();

                if (modelSpecial.UserRole != null)
                    modelSpecial.roleStatus = 15;
            }

            return PartialView(modelSpecial);


        }

        public ActionResult NotConfirmedPotential(int? UserID)
        {
            modelSpecial = new SpecialSummaryViewModel();
            binput = new BaseInput();
            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenmeyen", out modelSpecial.EnumValue);
                modelSpecial.PotentialProduction = new tblPotential_Production();
                modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
                modelSpecial.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput LoggedInUserOut = srv.WS_GetUserById(binput, (long)UserID, true, out modelSpecial.LoggedInUser);
                BaseOutput personOut = srv.WS_GetPersonByUserId(binput, modelSpecial.LoggedInUser.Id, true, out modelSpecial.Person);
                BaseOutput orgOUt = srv.WS_GetForeign_OrganizationByUserId(binput, (long)UserID, true, out modelSpecial.ForeignOrganisation);

                modelSpecial.NameSurname = modelSpecial.Person == null ? modelSpecial.ForeignOrganisation.name : modelSpecial.Person.Name + ' ' + modelSpecial.Person.Surname;

                modelSpecial.PotentialProduction.user_Id = UserID;
                modelSpecial.PotentialProduction.state_eV_Id = modelSpecial.EnumValue.Id;

                modelSpecial.PotentialProduction.user_IdSpecified = true;
                modelSpecial.PotentialProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetConfirmedPotential_ProductionsByStateAndUserId(binput, modelSpecial.PotentialProduction, out modelSpecial.PotentialProductionArray);

                modelSpecial.PotentialProductionList = modelSpecial.PotentialProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelSpecial.EnumCategory);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);

                modelSpecial.EnumValueList = modelSpecial.EnumValueArray.ToList();

                foreach (var item in modelSpecial.PotentialProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);
                    modelSpecial.ProductCatalogList.Add(modelSpecial.ProductCatalog);

                    BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                    modelSpecial.ProductCatalogList.Add(modelSpecial.ProductCatalog);
                }
                foreach (var item in modelSpecial.PotentialProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                    foreach (var itemm in modelSpecial.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelSpecial.ProductionControlList.Add(itemm);
                        }
                    }
                }
                //get the quantity type
                BaseOutput catout = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelSpecial.EnumCategory);
                BaseOutput enumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.EnumValueList = modelSpecial.EnumValueArray.ToList();

                //get the inbox messages
                BaseOutput mesOut = srv.WS_GetNotReadComMessagesByToUserId(binput, (long)UserID, true, out modelSpecial.NotReadComMessageArray);
                modelSpecial.ComMessageList = modelSpecial.NotReadComMessageArray == null ? null : modelSpecial.NotReadComMessageArray.ToList();
                modelSpecial.MessageCount = modelSpecial.ComMessageList == null ? 0 : modelSpecial.ComMessageList.Count();


                //get communications
                BaseOutput communicationsOut = srv.WS_GetCommunications(binput, out modelSpecial.CommunicationInformationsArray);

                if (modelSpecial.LoggedInUser.userType_eV_ID == 26)
                {
                    modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.Person.Id).ToList();
                }

                if (modelSpecial.LoggedInUser.userType_eV_ID == 50)
                {
                    modelSpecial.CommunicationInformationsList = modelSpecial.CommunicationInformationsArray.Where(x => x.PersonId == modelSpecial.ForeignOrganisation.manager_Id).ToList();
                }

                foreach (var item in modelSpecial.CommunicationInformationsList)
                {
                    if (item.comType == 10120)
                    {
                        modelSpecial.LoggedInUserInfos.MobilePhone = item.description;
                    }
                    if (item.comType == 10122)
                    {
                        modelSpecial.LoggedInUserInfos.WorkPhone = item.description;
                    }

                }

                BaseOutput gecbn = srv.WS_GetEnumCategorysByName(binput, "mobilePhonePrefix", out modelSpecial.EnumCategory);
                BaseOutput gevbci = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.MobilePhonePrefixList = modelSpecial.EnumValueArray.ToList();


                BaseOutput workPhoneCat = srv.WS_GetEnumCategorysByName(binput, "workPhonePrefix", out modelSpecial.EnumCategory);
                BaseOutput workPhoneEnumsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
                modelSpecial.WorkPhonePrefixList = modelSpecial.EnumValueArray.ToList();

                return PartialView(modelSpecial);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }

        }

        public ActionResult DeleteOfferProduction(int Id)
        {
            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.OfferProduction = new tblOffer_Production();
            modelSpecial.LoggedInUserInfos = new LoggedInUserInfos();

            try
            {
                binput = new BaseInput();

                BaseOutput Offer = srv.WS_GetOffer_ProductionById(binput, Id, true, out modelSpecial.OfferProduction);

                BaseOutput delet = srv.WS_DeleteOffer_Production(binput, modelSpecial.OfferProduction);


                //redirect to the page of the deleted offers type -- ex. redirect to on air offers if state_evıd of item is yayinda
                long onair, offair, rejected, confirmed;
                BaseOutput onairout = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelSpecial.EnumValue);
                onair = modelSpecial.EnumValue.Id;
                BaseOutput offairOut = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelSpecial.EnumValue);
                offair = modelSpecial.EnumValue.Id;
                BaseOutput rejectedOut = srv.WS_GetEnumValueByName(binput, "reject", out modelSpecial.EnumValue);
                rejected = modelSpecial.EnumValue.Id;
                BaseOutput confirmedOut = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);
                confirmed = modelSpecial.EnumValue.Id;
                if (modelSpecial.OfferProduction.state_eV_Id == onair)
                {
                    return RedirectToAction("OnAirOffers");
                }
                else if (modelSpecial.OfferProduction.state_eV_Id == offair)
                {
                    return RedirectToAction("OffAirOffers");
                }
                else if (modelSpecial.OfferProduction.state_eV_Id == rejected)
                {
                    return RedirectToAction("RejectedOffers");
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception err)
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult DeletePotentialProduction(int Id)
        {
            ProductsViewModel modelSpecial = new ProductsViewModel();

            modelSpecial.PotentialProduction = new tblPotential_Production();

            try
            {
                binput = new BaseInput();
                BaseOutput Offer = srv.WS_GetPotential_ProductionById(binput, Id, true, out modelSpecial.PotentialProduction);

                BaseOutput delet = srv.WS_DeletePotential_Production(binput, modelSpecial.PotentialProduction);

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return View(err.Message);
            }

        }

        public ActionResult PutToOfferProduction(int Id)
        {

            ProductsViewModel modelProducts = new ProductsViewModel();

            modelProducts.PotentialProduction = new tblPotential_Production();
            modelProducts.OfferProduction = new tblOffer_Production();
            modelProducts.ProductionDocument = new tblProduction_Document();
            modelProducts.EnumVal = new tblEnumValue();

            try
            {
                binput = new BaseInput();
                //get potential production u want to put to offer
                BaseOutput Production = srv.WS_GetPotential_ProductionById(binput, Id, true, out modelProducts.PotentialProduction);
                ///////////////////////////////

                //Delete potential production u want to put to offer, from potential production table



                BaseOutput delet = srv.WS_DeletePotential_Production(binput, modelProducts.PotentialProduction);

                ////////////////////////////////////////////////////

                //Add it to the offers table

                //get the enum Id of yayinda
                modelProducts.EnumVal = new tblEnumValue();
                BaseOutput eval = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelProducts.EnumVal);
                long enumYayinda = modelProducts.EnumVal.Id;
                ///////////////////////////

                modelProducts.OfferProduction.title = modelProducts.PotentialProduction.title;
                modelProducts.OfferProduction.description = modelProducts.PotentialProduction.description;
                modelProducts.OfferProduction.grup_Id = modelProducts.PotentialProduction.grup_Id;
                modelProducts.OfferProduction.quantity = modelProducts.PotentialProduction.quantity;
                modelProducts.OfferProduction.state_eV_Id = enumYayinda;
                modelProducts.OfferProduction.Status = 1;
                modelProducts.OfferProduction.total_price = modelProducts.PotentialProduction.total_price;
                modelProducts.OfferProduction.unit_price = modelProducts.PotentialProduction.unit_price;
                modelProducts.OfferProduction.updatedUser = modelProducts.PotentialProduction.updatedUser;
                modelProducts.OfferProduction.user_Id = modelProducts.PotentialProduction.user_Id;
                modelProducts.OfferProduction.createdUser = modelProducts.PotentialProduction.createdUser;
                modelProducts.OfferProduction.startDate = modelProducts.PotentialProduction.startDate;
                modelProducts.OfferProduction.endDate = modelProducts.PotentialProduction.endDate;
                modelProducts.OfferProduction.product_Id = modelProducts.PotentialProduction.product_Id;
                modelProducts.OfferProduction.productAddress_Id = modelProducts.PotentialProduction.productAddress_Id;
                modelProducts.OfferProduction.quantity_type_eV_Id = modelProducts.PotentialProduction.quantity_type_eV_Id;
                modelProducts.OfferProduction.monitoring_eV_Id = modelProducts.PotentialProduction.monitoring_eV_Id;
                modelProducts.OfferProduction.monitoring_eV_IdSpecified = true;

                modelProducts.OfferProduction.StatusSpecified = true;
                modelProducts.OfferProduction.state_eV_IdSpecified = true;
                modelProducts.OfferProduction.user_IdSpecified = true;
                modelProducts.OfferProduction.unit_priceSpecified = true;
                modelProducts.OfferProduction.total_priceSpecified = true;
                modelProducts.OfferProduction.endDateSpecified = true;
                modelProducts.OfferProduction.quantitySpecified = true;
                modelProducts.OfferProduction.product_IdSpecified = true;
                modelProducts.OfferProduction.productAddress_IdSpecified = true;
                modelProducts.OfferProduction.startDateSpecified = true;
                modelProducts.OfferProduction.quantity_type_eV_IdSpecified = true;
                BaseOutput addProduct = srv.WS_AddOffer_Production(binput, modelProducts.OfferProduction, out modelProducts.OfferProduction);

                ////////////////////////////////


                ////get production controls

                BaseOutput controlsOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, modelProducts.PotentialProduction.Id, true, out modelProducts.ProductionControlArray);
                BaseOutput typeOut = srv.WS_GetEnumValueByName(binput, "Offer", out modelProducts.EnumVal);
                foreach (var item in modelProducts.ProductionControlArray)
                {
                    modelProducts.ProductionControl = new tblProductionControl();
                    modelProducts.ProductionControl = item;
                    modelProducts.ProductionControl.Offer_Production_Id = item.Potential_Production_Id;
                    modelProducts.ProductionControl.Potential_Production_Id = -1;
                    modelProducts.ProductionControl.Production_type_eV_Id = modelProducts.EnumVal.Id;
                    modelProducts.ProductionControl.Offer_Production_IdSpecified = true;
                    modelProducts.ProductionControl.EnumValueIdSpecified = true;
                    srv.WS_AddProductionControl(binput, modelProducts.ProductionControl, out modelProducts.ProductionControl);
                }







                //Get the production documents wich have production id of the selected potential production item,

                //get the enum Id of tesdiqlenen
                modelProducts.EnumVal = new tblEnumValue();
                BaseOutput eval2 = srv.WS_GetEnumValueByName(binput, "Potential", out modelProducts.EnumVal);
                long prodTypeeVId = modelProducts.EnumVal.Id;
                ///////////////////////////

                //get enumId of offer
                modelProducts.EnumVal = new tblEnumValue();
                BaseOutput eval3 = srv.WS_GetEnumValueByName(binput, "Offer", out modelProducts.EnumVal);
                long offerTypeeVId = modelProducts.EnumVal.Id;
                ///

                modelProducts.ProductionDocument.Potential_Production_Id = Id;
                modelProducts.ProductionDocument.Production_type_eV_Id = prodTypeeVId;

                modelProducts.ProductionDocument.Production_type_eV_IdSpecified = true;
                modelProducts.ProductionDocument.Potential_Production_IdSpecified = true;

                BaseOutput prodDoc = srv.WS_GetProductionDocumentsByPotential_Production_Id(binput, modelProducts.ProductionDocument, out modelProducts.ProductionDocumentArray);

                modelProducts.ProductionDocumentList = modelProducts.ProductionDocumentArray.ToList();

                ////////////////////////////



                //update production documents
                foreach (var item in modelProducts.ProductionDocumentArray)
                {
                    binput = new BaseInput();
                    modelProducts.ProductionDocument = item;
                    modelProducts.ProductionDocument.Offer_Production_Id = modelProducts.OfferProduction.Id;
                    modelProducts.ProductionDocument.Offer_Production_IdSpecified = true;
                    modelProducts.ProductionDocument.Production_type_eV_Id = offerTypeeVId;

                    srv.WS_UpdateProductionDocument(binput, item, out modelProducts.ProductionDocument);
                }

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                return RedirectToAction("Index");
            }

        }

        public ActionResult PotensialClientInsert()
        {
            return View();
        }

        public ActionResult PhoneRegister1()
        {
            return View();
        }

        public ActionResult PhoneRegister2()
        {
            return View();
        }

        public ActionResult PhoneRegisterLast()
        {
            return View();
        }

        //send message demo
        public bool SendConfirmationMessage(string phoneNumEntered)
        {
            BaseInput baseinput = new BaseInput();
            tblConfirmationMessage ConfirmationMessage = new tblConfirmationMessage();
            Regex checkPhone = new Regex(@"^[\d*]{7}$");
            Match match = checkPhone.Match(phoneNumEntered);
            if (match.Success)
            {
                Random rnd = new Random();
                int reqem = rnd.Next(100000, 1000000);
                ConfirmationMessage.Message = reqem.ToString();

                BaseOutput pout = srv.WS_SendConfirmationMessageNew(baseinput, ConfirmationMessage, out ConfirmationMessage);
                SendMail(ConfirmationMessage.Message);
                return true;
            }
            else
            {
                return false;
            }

        }
        private void SendMail(string kod = null)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("ferid.heziyev@gmail.com", "tedaruk.az");
            msg.To.Add("qala2009@gmail.com");
            string fromPassword = "e1701895";
            msg.Subject = "Üzvlüyü tesdiqle";
            if (kod == null)
            {
                msg.Body = "<a href = 'http://localhost:56557/SpecialSummary/Index/'>Hesabınızı təsdiqləyin</a>";
            }
            else
            {
                msg.Body = "<p>Doğrulama kodunuz:" + kod + "</p>";
            }
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(msg.From.Address, fromPassword);
            smtp.Timeout = 20000;
            smtp.Send(msg);
        }
        public bool CheckConfirmationMessage(string message)
        {
            tblConfirmationMessage[] confirmationMessages;
            BaseInput baseinput = new BaseInput();
            BaseOutput bouput = srv.WS_GetConfirmationMessages(baseinput, out confirmationMessages);

            bool stance = false;

            foreach (tblConfirmationMessage item in confirmationMessages)
            {
                if (message == item.Message)
                {
                    SendMail();
                    stance = true;
                }
            }

            return stance;
        }

        public ActionResult PutToOffAirProduction(int offerId)
        {
            ProductsViewModel modelProducts = new ProductsViewModel();

            modelProducts.ProductionDocument = new tblProduction_Document();
            modelProducts.EnumVal = new tblEnumValue();

            binput = new BaseInput();
            //get offer production u want to put to off air offer
            BaseOutput Production = srv.WS_GetOffer_ProductionById(binput, offerId, true, out modelProducts.OfferProduction);
            ///////////////////////////////

            //Delete offer production u want to put to off air offer, from offer production table


            //BaseOutput delet = srv.WS_DeleteOffer_Production(binput, modelProducts.OfferProduction);

            ////////////////////////////////////////////////////

            //Add it to the off air offers table

            //get the enum Id of yayinda
            modelProducts.EnumVal = new tblEnumValue();
            BaseOutput eval = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelProducts.EnumVal);
            long enumYayindaDeyil = modelProducts.EnumVal.Id;
            ///////////////////////////

            modelProducts.OfferProduction.state_eV_Id = enumYayindaDeyil;
            modelProducts.OfferProduction.Status = 1;

            modelProducts.OfferProduction.StatusSpecified = true;
            modelProducts.OfferProduction.state_eV_IdSpecified = true;
            BaseOutput addProduct = srv.WS_UpdateOffer_Production(binput, modelProducts.OfferProduction, out modelProducts.OfferProduction);

            ////////////////////////////////

            return RedirectToAction("Index");
        }

        public void UpdateOnAirOffers(int UserID)
        {
            ProductsViewModel modelSpecial = new ProductsViewModel();

            binput = new BaseInput();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserID = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelSpecial.EnumVal);
            modelSpecial.OfferProduction = new tblOffer_Production();

            modelSpecial.OfferProduction.user_Id = UserID;
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumVal.Id;

            modelSpecial.OfferProduction.user_IdSpecified = true;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;

            BaseOutput OnAirOffersOut = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelSpecial.OfferProduction, out modelSpecial.OfferProductionArray);

            foreach (var item in modelSpecial.OfferProductionArray)
            {
                if (item.endDate < DateTime.Now.Ticks)
                {
                    PutToOffAirProduction((int)item.Id);
                }
            }

            BaseOutput enumdVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumVal);
            modelSpecial.OfferProduction = new tblOffer_Production();

            modelSpecial.OfferProduction.user_Id = UserID;
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumVal.Id;

            modelSpecial.OfferProduction.user_IdSpecified = true;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;

            BaseOutput OnAirOffersdOut = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelSpecial.OfferProduction, out modelSpecial.OfferProductionArray);
            foreach (var item in modelSpecial.OfferProductionArray)
            {
                if (item.endDate < DateTime.Now.Ticks)
                {
                    PutToOffAirProduction((int)item.Id);
                }
            }

        }

        public ActionResult EditToOffer(long? UserId, int? Id)
        {
            Session["arrONum"] = null;

            binput = new BaseInput();
            PotentialProductionViewModel modelPotentialProduct = new PotentialProductionViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(binput, (long)UserId, true, out modelPotentialProduct.User);
            BaseOutput person = srv.WS_GetPersonByUserId(binput, (long)UserId, true, out modelPotentialProduct.Person);

            //get enum categories with name olcu vahidi
            BaseOutput enumCatOut = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelPotentialProduct.EnumCategorySS);
            //get enum categories with the selected enum value id
            BaseOutput enumValOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelPotentialProduct.EnumCategorySS.Id, true, out modelPotentialProduct.EnumValueArray);


            BaseOutput productOut = srv.WS_GetPotential_ProductionById(binput, (long)Id, true, out modelPotentialProduct.PotentialProduction);
            modelPotentialProduct.Id = (long)Id;
            //var productArr = modelPotentialProduct.PotentialProduction.fullProductId;
            //var productMain = productArr.Split(',');
            BaseOutput prOut = srv.WS_GetProductCatalogsById(binput, (int)modelPotentialProduct.PotentialProduction.product_Id, true, out modelPotentialProduct.ProductCatalog);

            ////get types of the product ex -> çekilmiş et, hise verilmiş et exc.
            //BaseOutput typeOut = srv.WS_GetProductCatalogsByParentId(binput, (int)modelPotentialProduct.ProductCatalog.Id, true, out modelPotentialProduct.ProductCatalogArray);

            //duz
            modelPotentialProduct.Quantity = modelPotentialProduct.PotentialProduction.quantity == null ? null : modelPotentialProduct.PotentialProduction.quantity.ToString().Replace(',', '.');
            modelPotentialProduct.UnitPrice = modelPotentialProduct.PotentialProduction.unit_price == null ? null : modelPotentialProduct.PotentialProduction.unit_price.ToString().Replace(',', '.'); ;
            //
            return View(modelPotentialProduct);
        }


        [HttpPost]
        public ActionResult EditToOffer(PotentialProductionViewModel form, long? Id)
        {
            PotentialProductionViewModel modelPotential = new PotentialProductionViewModel();
            binput = new BaseInput();

            BaseOutput potentialOut = srv.WS_GetPotential_ProductionById(binput, (long)Id, true, out modelPotential.PotentialProduction);

            DateTime startDate = (DateTime)form.startDate;
            DateTime endDate = (DateTime)form.endDate;


            modelPotential.PotentialProduction.startDate = startDate.Ticks;
            modelPotential.PotentialProduction.startDateSpecified = true;

            modelPotential.PotentialProduction.endDate = endDate.Ticks;
            modelPotential.PotentialProduction.endDateSpecified = true;


            ////get product type from the db given product type name

            //modelPotential.PotentialProduction.product_Id = form.ProductType;
            //modelPotential.PotentialProduction.product_IdSpecified = true;
            /////////////


            //update full product id
            var arr = modelPotential.PotentialProduction.fullProductId;
            var b = arr.Split(',');
            b[b.Length - 1] = modelPotential.PotentialProduction.product_Id.ToString();

            var c = string.Join(",", b);


            modelPotential.PotentialProduction.fullProductId = c;
            try
            {
                //duz
                modelPotential.PotentialProduction.quantity = (decimal)float.Parse(form.Quantity.Replace('.', ','));
                modelPotential.PotentialProduction.quantitySpecified = true;
                //

                //modelPotential.PotentialProduction.quantity_type_eV_Id = form.QuantityType;
                //modelPotential.PotentialProduction.quantity_type_eV_IdSpecified = true;

                modelPotential.PotentialProduction.total_price = modelPotential.PotentialProduction.quantity * modelPotential.PotentialProduction.unit_price;
                modelPotential.PotentialProduction.total_priceSpecified = true;

                //duz
                modelPotential.PotentialProduction.unit_price = (decimal)float.Parse(form.UnitPrice.Replace('.', ',')); ;
                modelPotential.PotentialProduction.unit_priceSpecified = true;
                //
            }
            catch (Exception)
            {


            }



            modelPotential.PotentialProduction.title = form.Title;


            //update product potential product

            BaseOutput potUpdate = srv.WS_UpdatePotential_Production(binput, modelPotential.PotentialProduction, out modelPotential.PotentialProduction);
            return RedirectToAction("PutToOfferProduction", new { Id = (int)Id });
        }

        public ActionResult OfferProductInfo(int? Id, long? UserId)
        {
            binput = new BaseInput();
            OfferProductionViewModel modelProduction = new OfferProductionViewModel();

            BaseOutput offerOut = srv.WS_GetOffer_ProductionById(binput, (long)Id, true, out modelProduction.OfferProduction);
            BaseOutput prodOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.OfferProduction.product_Id, true, out modelProduction.ProductCatalog);


            modelProduction.ProductName = modelProduction.ProductCatalog.ProductName;

            //get product parent
            BaseOutput parentOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.ProductCatalog.ProductCatalogParentID, true, out modelProduction.ProductCatalog);
            //duz
            modelProduction.ProductParentName = modelProduction.ProductCatalog.ProductName;


            modelProduction.Quantity = modelProduction.OfferProduction.quantity.ToString();
            //

            //get the quantity type

            BaseOutput productControllOut = srv.WS_GetProductionControlsByOfferProductionId(binput, modelProduction.OfferProduction.Id, true, out modelProduction.ProductionControlArray);


            modelProduction.QuantityType = modelProduction.ProductionControlArray.Length == 0 ? 0 : (long)modelProduction.ProductionControlArray.FirstOrDefault().EnumValueId;
            BaseOutput quantTypeOut = srv.WS_GetEnumValueById(binput, modelProduction.QuantityType, true, out modelProduction.EnumValue);
            modelProduction.QuantityTypeStr = modelProduction.EnumValue == null ? "" : modelProduction.EnumValue.name;
            modelProduction.startDate = new DateTime((long)modelProduction.OfferProduction.startDate);
            modelProduction.endDate = new DateTime((long)modelProduction.OfferProduction.endDate);

            //duz
            DateTime end = (DateTime)modelProduction.endDate;
            modelProduction.EndDateStr = end.ToString("d");

            DateTime start = (DateTime)modelProduction.startDate;
            modelProduction.StartDateStr = start.ToString("d");
            //


            //get full address
            BaseOutput prodAddrOut = srv.WS_GetProductAddressById(binput, (long)modelProduction.OfferProduction.productAddress_Id, true, out modelProduction.ProductAddress);
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

        public ActionResult PotentialProductInfo(int? Id, long? UserId)
        {
            binput = new BaseInput();
            OfferProductionViewModel modelProduction = new OfferProductionViewModel();

            BaseOutput offerOut = srv.WS_GetPotential_ProductionById(binput, (long)Id, true, out modelProduction.PotentialProduction);
            BaseOutput prodOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.PotentialProduction.product_Id, true, out modelProduction.ProductCatalog);


            modelProduction.ProductName = modelProduction.ProductCatalog.ProductName;

            //get product parent
            BaseOutput parentOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.ProductCatalog.ProductCatalogParentID, true, out modelProduction.ProductCatalog);

            modelProduction.ProductParentName = modelProduction.ProductCatalog.ProductName;

            //duz
            modelProduction.Quantity = modelProduction.PotentialProduction.quantity.ToString();
            //

            //get the quantity type
            BaseOutput productControllOut = srv.WS_GetProductionControlsByPotentialProductionId(binput, modelProduction.PotentialProduction.Id, true, out modelProduction.ProductionControlArray);
            modelProduction.QuantityType = modelProduction.ProductionControlArray.Length == 0 ? 0 : (long)modelProduction.ProductionControlArray.FirstOrDefault().EnumValueId;


            BaseOutput quantTypeOut = srv.WS_GetEnumValueById(binput, modelProduction.QuantityType, true, out modelProduction.EnumValue);
            modelProduction.QuantityTypeStr = modelProduction.EnumValue == null ? "" : modelProduction.EnumValue.name;
            modelProduction.startDate = new DateTime((long)modelProduction.PotentialProduction.startDate);
            modelProduction.endDate = new DateTime((long)modelProduction.PotentialProduction.endDate);

            //duz
            DateTime end = (DateTime)modelProduction.endDate;
            modelProduction.EndDateStr = end.ToString("d");

            DateTime start = (DateTime)modelProduction.startDate;
            modelProduction.StartDateStr = start.ToString("d");
            //

            //get full address
            BaseOutput prodAddrOut = srv.WS_GetProductAddressById(binput, (long)modelProduction.PotentialProduction.productAddress_Id, true, out modelProduction.ProductAddress);
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

        public ActionResult EditRejectedOffers(int? Id, long? UserId)
        {
            binput = new BaseInput();
            OfferProductionViewModel modelProduction = new OfferProductionViewModel();

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


            Session["arrONum"] = null;


            BaseOutput productOut = srv.WS_GetOffer_ProductionById(binput, (long)Id, true, out modelProduction.OfferProduction);
            //get products catalog

            BaseOutput productCatOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.OfferProduction.product_Id, true, out modelProduction.ProductCatalog);


            ////get types of the product ex -> çekilmiş et, hise verilmiş et exc.
            //BaseOutput typeOut = srv.WS_GetProductCatalogsByParentId(binput, (int)modelProduction.ProductCatalog.ProductCatalogParentID, true, out modelProduction.ProductCatalogArray);

            //duz
            modelProduction.UnitPrice = modelProduction.OfferProduction.unit_price.ToString().Replace(',', '.');
            //
            //BaseOutput productParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelProduction.ProductCatalog.ProductCatalogParentID, true, out modelProduction.ProductCatalog);
            //get the ölçü vahidi
            //duz
            modelProduction.Quantity = modelProduction.OfferProduction.quantity.ToString().Replace(',', '.');
            //
            BaseOutput measureOut = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelProduction.EnumCategory);
            BaseOutput enumValueOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelProduction.EnumCategory.Id, true, out modelProduction.EnumValueArray);

            return View(modelProduction);
        }

        [HttpPost]
        public ActionResult EditRejectedOffers(OfferProductionViewModel form, long? Id)
        {
            OfferProductionViewModel modeloffer = new OfferProductionViewModel();
            binput = new BaseInput();

            BaseOutput offerOut = srv.WS_GetOffer_ProductionById(binput, (long)Id, true, out modeloffer.OfferProduction);

            DateTime startDate = (DateTime)form.startDate;
            DateTime endDate = (DateTime)form.endDate;


            modeloffer.OfferProduction.startDate = startDate.Ticks;
            modeloffer.OfferProduction.startDateSpecified = true;

            modeloffer.OfferProduction.endDate = endDate.Ticks;
            modeloffer.OfferProduction.endDateSpecified = true;


            //get product type from the db given product type name

            //modeloffer.OfferProduction.product_Id = form.ProductType;
            //modeloffer.OfferProduction.product_IdSpecified = true;
            ///////////
            try
            {
                //duz
                modeloffer.OfferProduction.unit_price = (decimal)float.Parse(form.UnitPrice.Replace('.', ','));
                modeloffer.OfferProduction.unit_priceSpecified = true;

                modeloffer.OfferProduction.quantity = (decimal)float.Parse(form.Quantity.Replace('.', ','));
                modeloffer.OfferProduction.quantitySpecified = true;
                //
            }
            catch (Exception ex)
            {


            }



            modeloffer.OfferProduction.total_price = modeloffer.OfferProduction.quantity * modeloffer.OfferProduction.unit_price;
            modeloffer.OfferProduction.total_priceSpecified = true;

            //modeloffer.OfferProduction.title = form.Title;

            //update status
            BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modeloffer.EnumValue);
            modeloffer.OfferProduction.state_eV_Id = modeloffer.EnumValue.Id;
            modeloffer.OfferProduction.state_eV_IdSpecified = true;

            //update product potential product

            BaseOutput potUpdate = srv.WS_UpdateOffer_Production(binput, modeloffer.OfferProduction, out modeloffer.OfferProduction);





            return RedirectToAction("OnAirOffers");
        }


        public ActionResult ReEditedOffers(long? UserID)
        {
            var a = User.Identity.ToString();

            ProductsViewModel modelSpecial = new ProductsViewModel();
            binput = new BaseInput();

            try
            {
                BaseOutput enumVal = srv.WS_GetEnumValueByName(binput, "reedited", out modelSpecial.EnumVal);
                modelSpecial.OfferProduction = new tblOffer_Production();
                modelSpecial.ProductCatalogList = new List<tblProductCatalog>();
                modelSpecial.ProductionControlList = new List<tblProductionControl>();

                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserID = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                modelSpecial.OfferProduction.user_Id = UserID;
                modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumVal.Id;

                modelSpecial.OfferProduction.user_IdSpecified = true;
                modelSpecial.OfferProduction.state_eV_IdSpecified = true;

                BaseOutput offer = srv.WS_GetOnAirOffer_ProductionsByUserID(binput, modelSpecial.OfferProduction, out modelSpecial.OfferProductionArray);

                modelSpecial.OfferProductionList = modelSpecial.OfferProductionArray.ToList();

                BaseOutput enumOlcu = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelSpecial.EnumCat);

                BaseOutput enumValList = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCat.Id, true, out modelSpecial.EnumValueArray);

                modelSpecial.EnumValueList = modelSpecial.EnumValueArray.ToList();

                foreach (var item in modelSpecial.OfferProductionList)
                {
                    BaseOutput productCatalogOut = srv.WS_GetProductCatalogsById(binput, (int)item.product_Id, true, out modelSpecial.ProductCatalog);
                    modelSpecial.ProductCatalogList.Add(modelSpecial.ProductCatalog);
                    BaseOutput productCatalogParentOut = srv.WS_GetProductCatalogsById(binput, (int)modelSpecial.ProductCatalog.ProductCatalogParentID, true, out modelSpecial.ProductCatalog);
                    modelSpecial.ProductCatalogList.Add(modelSpecial.ProductCatalog);
                }

                foreach (var item in modelSpecial.OfferProductionList)
                {
                    BaseOutput productControlOut = srv.WS_GetProductionControlsByOfferProductionId(binput, item.Id, true, out modelSpecial.ProductionControlArray);
                    foreach (var itemm in modelSpecial.ProductionControlArray)
                    {
                        if (itemm.EnumCategoryId == 5)
                        {
                            modelSpecial.ProductionControlList.Add(itemm);
                        }
                    }
                }

                return PartialView(modelSpecial);
            }
            catch (Exception err)
            {
                return PartialView(err.Message);
            }
        }

        public JsonResult GetEducation()
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            BaseOutput categoryOut = srv.WS_GetEnumCategorysByName(binput, "Tehsil", out modelSpecial.EnumCategory);
            BaseOutput educationsOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.EnumValueList = modelSpecial.EnumValueArray.ToList();

            return Json(modelSpecial.EnumValueList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJob()
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();

            BaseOutput categoryOut = srv.WS_GetEnumCategorysByName(binput, "İş", out modelSpecial.EnumCategory);
            BaseOutput jobssOut = srv.WS_GetEnumValuesByEnumCategoryId(binput, modelSpecial.EnumCategory.Id, true, out modelSpecial.EnumValueArray);
            modelSpecial.EnumValueList = modelSpecial.EnumValueArray.ToList();

            return Json(modelSpecial.EnumValueList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserById(long Id)
        {
            binput = new BaseInput();

            UserViewModel modelUser = new UserViewModel();
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    if (Id == 1)
                    {
                        Id = Int32.Parse(identity.Ticket.UserData);
                    }
                }
            }

            try
            {
                BaseOutput bout = srv.WS_GetUserById(binput, Id, true, out modelUser.User);
                BaseOutput personBout = srv.WS_GetPersonByUserId(binput, Id, true, out modelUser.Person);
                BaseOutput foreOut = srv.WS_GetForeign_OrganizationByUserId(binput, Id, true, out modelUser.ForeignOrganisation);
                modelUser.NameSurname = modelUser.Person == null ? modelUser.ForeignOrganisation.name : modelUser.Person.Name + ' ' + modelUser.Person.Surname;

                if (modelUser.Person.educationLevel_eV_Id == null)
                    modelUser.Person.educationLevel_eV_Id = 0;

                BaseOutput educOut = srv.WS_GetEnumValueById(binput, (long)modelUser.Person.educationLevel_eV_Id, true, out modelUser.EducationEnumValue);

                if (modelUser.EducationEnumValue == null)
                    modelUser.EducationEnumValue = new tblEnumValue();

                modelUser.EducationLevel = modelUser.EducationEnumValue.name;


                if (modelUser.Person.job_eV_Id == null)
                    modelUser.Person.job_eV_Id = 0;
                BaseOutput jobOut = srv.WS_GetEnumValueById(binput, (long)modelUser.Person.job_eV_Id, true, out modelUser.JobEnumValue);
                if (modelUser.JobEnumValue == null)
                    modelUser.JobEnumValue = new tblEnumValue();
                modelUser.job = modelUser.JobEnumValue.name;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            if (modelUser.Person != null)
            {
                if (modelUser.Person.profilePicture == null)
                    modelUser.Person.profilePicture = "1";
                modelUser.Person.profilePicture = Convert.ToBase64String(StringExtension.StringToByteArray(modelUser.Person.profilePicture));
            }
            return Json(modelUser, JsonRequestBehavior.AllowGet);
        }

        public List<products> getCountsOffProducts(long userId)
        {
            binput = new BaseInput();
            modelSpecial = new SpecialSummaryViewModel();
            SpecialSummaryViewModel model = new SpecialSummaryViewModel();
            modelSpecial.OfferProduction = new tblOffer_Production();
            BaseOutput enumVal = null;
            BaseOutput offerOutCount = null;
            List<products> newOffers = new List<products>();
            //type Tesdiqlenen = 1; Yayinda = 2; YayindaDeyil = 3; reject = 4;

            modelSpecial.OfferProduction.user_Id = userId;
            modelSpecial.OfferProduction.user_IdSpecified = true;
            //begin

            enumVal = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;
            offerOutCount = srv.WS_GetOnAirOfferCount_ProductionsByUserId(binput, modelSpecial.OfferProduction, out model.OfferProductionArray);
            if (model.OfferProductionArray != null)
            {
                model.OfferProductionList = model.OfferProductionArray.ToList();

                foreach (var item in model.OfferProductionList)
                {
                    newOffers.Add(new products { id = item.Id, type = 1 });
                }
            }

            //end 
            //begin

            enumVal = srv.WS_GetEnumValueByName(binput, "Yayinda", out modelSpecial.EnumValue);
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;
            offerOutCount = srv.WS_GetOnAirOfferCount_ProductionsByUserId(binput, modelSpecial.OfferProduction, out model.OfferProductionArray);

            if (model.OfferProductionArray != null)
            {
                model.OfferProductionList = model.OfferProductionArray.ToList();

                foreach (var item in model.OfferProductionList)
                {
                    newOffers.Add(new products { id = item.Id, type = 2 });
                }
            }

            //end
            //begin

            enumVal = srv.WS_GetEnumValueByName(binput, "YayindaDeyil", out modelSpecial.EnumValue);
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;
            offerOutCount = srv.WS_GetOnAirOfferCount_ProductionsByUserId(binput, modelSpecial.OfferProduction, out model.OfferProductionArray);

            if (model.OfferProductionArray != null)
            {
                model.OfferProductionList = model.OfferProductionArray.ToList();

                foreach (var item in model.OfferProductionList)
                {
                    newOffers.Add(new products { id = item.Id, type = 3 });
                }
            }

            //end
            //begin

            enumVal = srv.WS_GetEnumValueByName(binput, "reject", out modelSpecial.EnumValue);
            modelSpecial.OfferProduction.state_eV_Id = modelSpecial.EnumValue.Id;
            modelSpecial.OfferProduction.state_eV_IdSpecified = true;
            offerOutCount = srv.WS_GetOnAirOfferCount_ProductionsByUserId(binput, modelSpecial.OfferProduction, out model.OfferProductionArray);

            if (model.OfferProductionArray != null)
            {
                model.OfferProductionList = model.OfferProductionArray.ToList();

                foreach (var item in model.OfferProductionList)
                {
                    newOffers.Add(new products { id = item.Id, type = 4 });
                }
            }

            //end
            return newOffers; 
        }

        [WordDocument]
        public ActionResult ContractForm(long pid, bool isContract)
        {
            try
            {
                modelSpecial = new SpecialSummaryViewModel();
                binput = new BaseInput();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.User);
                binput.userName = modelSpecial.User.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(binput, "olcuVahidi", out modelSpecial.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(binput, "Tesdiqlenen", out modelSpecial.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForMonitoringEVId(binput, (long)UserId, true, modelSpecial.EnumValue.Id, true, out modelSpecial.ProductionDetailArray);

                modelSpecial.ProductionDetailList = modelSpecial.ProductionDetailArray.Where(x => x.enumCategoryId == modelSpecial.EnumCategory.Id && x.person != null).ToList();

                modelSpecial.ProductionDetailList = modelSpecial.ProductionDetailList.Where(x => x.person.Id == pid).ToList();

                BaseOutput gpbui = srv.WS_GetPersonByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.Person);

                modelSpecial.icraci = modelSpecial.Person.Surname + " " + modelSpecial.Person.Name + " " + modelSpecial.Person.FatherName;

                return View(modelSpecial);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult GetContractFile(string fname)
        {
            try
            {
                modelSpecial = new SpecialSummaryViewModel();
                binput = new BaseInput();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(binput, (long)UserId, true, out modelSpecial.User);
                binput.userName = modelSpecial.User.Username;

                modelSpecial.fname = fname;

                return View(modelSpecial);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public void UpdateEmail(string email, int? userId)
        {
            binput = new BaseInput();

            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.User = new tblUser();
            modelSpecial.Person = new tblPerson();

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
                BaseOutput userOutput = srv.WS_GetUserById(binput, (long)userId, true, out modelSpecial.User);

                modelSpecial.User.Email = email;
                BaseOutput pout = srv.WS_UpdateUser(binput, modelSpecial.User, out modelSpecial.User);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

        }

        public string UpdateUser(
          //string userName,
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

            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.Person = new tblPerson();
            modelSpecial.EnumCategory = new tblEnumCategory();

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
                BaseOutput userOut = srv.WS_GetUserById(binput, (long)userId, true, out modelSpecial.User);
                BaseOutput personOutPut = srv.WS_GetPersonByUserId(binput, (long)userId, true, out modelSpecial.Person);
                personId = (int)modelSpecial.Person.Id;
                //modelSpecial.User.Username = userName;
                modelSpecial.User.Id = (long)userId;
                modelSpecial.User.Status = 1;
                //modelSpecial.User.Email = email;

                modelSpecial.User.IdSpecified = true;
                modelSpecial.User.StatusSpecified = true;

                if (personId != null)
                {
                    modelSpecial.Person.Status = 1;
                    modelSpecial.Person.gender = gender;
                    modelSpecial.Person.Id = (long)personId;
                    modelSpecial.Person.educationLevel_eV_Id = educationId;
                    modelSpecial.Person.job_eV_Id = jobId;

                    modelSpecial.Person.IdSpecified = true;
                    modelSpecial.Person.StatusSpecified = true;
                    modelSpecial.Person.birtdaySpecified = true;
                    modelSpecial.Person.educationLevel_eV_IdSpecified = true;
                    modelSpecial.Person.job_eV_IdSpecified = true;

                }

                BaseOutput pout = srv.WS_UpdateUser(binput, modelSpecial.User, out modelSpecial.User);
                BaseOutput personOut = srv.WS_UpdatePerson(binput, modelSpecial.Person, out modelSpecial.Person);

                //to redirect to governmentOrganisation or special summary controller
                BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.UserRoleArray);
                BaseOutput roleOut = srv.WS_GetRoleByName(binput, "governmentOrganisation", out modelSpecial.Role);
                foreach (var item in modelSpecial.UserRoleArray)
                {
                    if (item.RoleId == modelSpecial.Role.Id)
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

            modelSpecial = new SpecialSummaryViewModel();

            modelSpecial.User = new tblUser();

            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }
            modelSpecial.User.Id = (long)userId;

            modelSpecial.User.IdSpecified = true;
            modelSpecial.User.StatusSpecified = true;


            BaseOutput userGet = srv.WS_GetUserById(binput, (long)userId, true, out modelSpecial.User);
            if (password != null)
            {
                bool verify = BCrypt.Net.BCrypt.Verify(password, modelSpecial.User.Password);

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

            modelSpecial = new SpecialSummaryViewModel();



            FormsIdentity identity = (FormsIdentity)User.Identity;
            if (identity.Ticket.UserData.Length > 0)
            {
                userId = Int32.Parse(identity.Ticket.UserData);
            }

            try
            {
                BaseOutput userOutput = srv.WS_GetUserById(binput, (long)userId, true, out modelSpecial.User);
                modelSpecial.User.Username = modelSpecial.User.Username;
                modelSpecial.User.Id = modelSpecial.User.Id;
                modelSpecial.User.Email = modelSpecial.User.Email;
                modelSpecial.User.Password = BCrypt.Net.BCrypt.HashPassword(password, 5);
                modelSpecial.User.IdSpecified = true;
                BaseOutput pout = srv.WS_UpdateUser(binput, modelSpecial.User, out modelSpecial.User);

                //to redirect to governmentOrganisation or special summary controller
                BaseOutput userRole = srv.WS_GetUserRolesByUserId(binput, modelSpecial.User.Id, true, out modelSpecial.UserRoleArray);
                foreach (var item in modelSpecial.UserRoleArray)
                {
                    if (item.RoleId == 12)
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


    }

    public class products
    {
        public long id { get; set; }
        public int type { get; set; }
    }
}

