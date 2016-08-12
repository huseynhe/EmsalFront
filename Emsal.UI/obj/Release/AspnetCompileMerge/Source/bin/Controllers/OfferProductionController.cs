using Emsal.UI.Infrastructure;
using Emsal.UI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.offerProduction)]

    public class OfferProductionController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private OfferProductionViewModel modelOfferProduction;

        public ActionResult Index(long?userId)
        {
            //long unixDate = DateTime.Now.Ticks;
            //DateTime start = new DateTime(636012864000000000);
            //var rdd = start.Ticks;

            string userIpAddress = this.Request.ServerVariables["REMOTE_ADDR"];

            baseInput = new BaseInput();
            modelOfferProduction = new OfferProductionViewModel();

            //ferid
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            BaseOutput user = srv.WS_GetUserById(baseInput, (long)userId, true, out modelOfferProduction.User);
            BaseOutput person = srv.WS_GetPersonByUserId(baseInput, (long)userId, true, out modelOfferProduction.Person);
            //////


            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "shippingSchedule", out modelOfferProduction.EnumCategory);
            if (modelOfferProduction.EnumCategory == null)
                modelOfferProduction.EnumCategory = new tblEnumCategory();

            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
            modelOfferProduction.EnumValueShippingScheduleList = modelOfferProduction.EnumValueArray.ToList();


            BaseOutput ecat = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelOfferProduction.EnumCategory);
            if (modelOfferProduction.EnumCategory == null)
                modelOfferProduction.EnumCategory = new tblEnumCategory();

            BaseOutput eval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
            modelOfferProduction.EnumValueMonthList = modelOfferProduction.EnumValueArray.ToList();


            if (Session["documentGrupId"] == null)
            {
                Guid dg = Guid.NewGuid();
                Session["documentGrupId"] = dg;
                this.Session.Timeout = 20;
            }

            return View(modelOfferProduction);
        }

        //public ActionResult GeneratePDF()
        //{
        //    return new Rotativa.ActionAsPdf("SelectedProducts");
        //}

        public ActionResult ProductCatalog(int pId = 0)
        {
            baseInput = new BaseInput();

            modelOfferProduction = new OfferProductionViewModel();

            BaseOutput bouput = srv.WS_GetProductCatalogs(baseInput, out modelOfferProduction.ProductCatalogArray);
            modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.Where(x => x.ProductCatalogParentID == pId).ToList();


            BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelOfferProduction.EnumCategoryArray);
            modelOfferProduction.EnumCategoryList = modelOfferProduction.EnumCategoryArray.ToList();

            BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelOfferProduction.EnumValueArray);
            modelOfferProduction.EnumValueList = modelOfferProduction.EnumValueArray.ToList();

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelOfferProduction.EnumCategory);

            BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelOfferProduction.ProductCatalogControlArray);
            modelOfferProduction.ProductCatalogControlList = modelOfferProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId != modelOfferProduction.EnumCategory.Id).ToList();


            if (Session["arrOPNum"] == null)
            {
                Session["arrOPNum"] = modelOfferProduction.arrPNum;
            }
            else
            {
                modelOfferProduction.arrPNum = (int)Session["arrOPNum"] + 1;
                Session["arrOPNum"] = modelOfferProduction.arrPNum;
            }

            return View(modelOfferProduction);
        }

        public ActionResult AdminUnit(int pId = 0)
        {
            baseInput = new BaseInput();

            modelOfferProduction = new OfferProductionViewModel();

            BaseOutput bouput = srv.WS_GetPRM_AdminUnits(baseInput, out modelOfferProduction.PRMAdminUnitArray);
            modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.Where(x => x.ParentID == pId).ToList();

            //BaseOutput bouputs = srv.WS_GetAdminUnitsByParentId(baseInput, pId, true, out modelOfferProduction.PRMAdminUnitArray);
            //modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();


            if (Session["arrONum"] == null)
            {
                Session["arrONum"] = modelOfferProduction.arrNum;
            }
            else
            {
                modelOfferProduction.arrNum = (int)Session["arrONum"] + 1;
                Session["arrONum"] = modelOfferProduction.arrNum;
            }

            return View(modelOfferProduction);
        }

        [HttpPost]
        public ActionResult Index(OfferProductionViewModel model, FormCollection collection)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;

            modelOfferProduction = new OfferProductionViewModel();
            modelOfferProduction.OfferProduction = new tblOffer_Production();

            //if (Session["SelectedProduct"] == null)
            //{
            //Guid sp = Guid.NewGuid();
            //Session["SelectedProduct"] = sp;
            //}
            //var dd = Session["SelectedProduct"];
            //Session.Contents.Remove("SelectedProduct");

            //if (model.confirmList == true)
            //{
            //    Guid grupId = Guid.NewGuid();
            //    modelOfferProduction.OfferProduction.grup_Id = grupId.ToString();
            //    modelOfferProduction.OfferProduction.isSelected = true;
            //    modelOfferProduction.OfferProduction.user_Id = 1;
            //    modelOfferProduction.OfferProduction.user_IdSpecified = true;
            //    modelOfferProduction.OfferProduction.isSelected = true;
            //    modelOfferProduction.OfferProduction.isSelectedSpecified = true;

            //    //BaseOutput upp = srv.WS_UpdateOfffer_ProductionForUserID(modelOfferProduction.OfferProduction, out  modelOfferProduction.OfferProductionArray);

            //}
            //else
            //{
            Guid grupId = Guid.NewGuid();

            modelOfferProduction.OfferProduction.grup_Id = grupId.ToString();

                modelOfferProduction.OfferProduction.description = model.description;
                modelOfferProduction.OfferProduction.product_Id = model.productId;
                modelOfferProduction.OfferProduction.product_IdSpecified = true;

                BaseOutput gcl = srv.WS_GetProductCatalogListForID(baseInput, model.productId, true, out modelOfferProduction.ProductCatalogArray);
                modelOfferProduction.ProductCatalogList = modelOfferProduction.ProductCatalogArray.ToList();

                modelOfferProduction.OfferProduction.title = string.Join(",", modelOfferProduction.ProductCatalogList.Select(x => x.Id));


                modelOfferProduction.OfferProduction.unit_price = Convert.ToDecimal(model.price.Replace('.', ','));
                //modelOfferProduction.OfferProduction.unit_price = Convert.ToDecimal(model.price);
                modelOfferProduction.OfferProduction.unit_priceSpecified = true;

                modelOfferProduction.OfferProduction.quantity = Convert.ToDecimal(model.size.Replace('.', ','));
                //modelOfferProduction.OfferProduction.quantity = Convert.ToDecimal(model.size);
                modelOfferProduction.OfferProduction.quantitySpecified = true;

                modelOfferProduction.OfferProduction.isSelected = false;
                modelOfferProduction.OfferProduction.isSelectedSpecified = true;

                modelOfferProduction.OfferProduction.user_Id = userId;
                modelOfferProduction.OfferProduction.user_IdSpecified = true;

                modelOfferProduction.OfferProduction.Status = 1;
                modelOfferProduction.OfferProduction.StatusSpecified = true;

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Yayinda", out modelOfferProduction.EnumValue);
                modelOfferProduction.OfferProduction.state_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.OfferProduction.state_eV_IdSpecified = true;

                DateTime startDate = (DateTime)model.startDate;
                modelOfferProduction.OfferProduction.startDate = startDate.Ticks;
                modelOfferProduction.OfferProduction.startDateSpecified = true;

                DateTime endDate = (DateTime)model.endDate;
                modelOfferProduction.OfferProduction.endDate = endDate.Ticks;
                modelOfferProduction.OfferProduction.endDateSpecified = true;

                modelOfferProduction.ProductAddress = new tblProductAddress();

                modelOfferProduction.ProductAddress.adminUnit_Id = model.addressId;

                BaseOutput gal = srv.WS_GetAdminUnitListForID(baseInput, model.addressId, true, out modelOfferProduction.PRMAdminUnitArray);
                modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
                modelOfferProduction.ProductAddress.fullAddressId = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Id));
                modelOfferProduction.ProductAddress.fullAddress = string.Join(",", modelOfferProduction.PRMAdminUnitList.Select(x => x.Name));

                modelOfferProduction.ProductAddress.adminUnit_IdSpecified = true;
                modelOfferProduction.ProductAddress.addressDesc = model.descAddress;

                BaseOutput apa = srv.WS_AddProductAddress(baseInput, modelOfferProduction.ProductAddress, out  modelOfferProduction.ProductAddress);

                modelOfferProduction.OfferProduction.productAddress_Id = modelOfferProduction.ProductAddress.Id;
                modelOfferProduction.OfferProduction.productAddress_IdSpecified = true;


                BaseOutput app = srv.WS_AddOffer_Production(baseInput, modelOfferProduction.OfferProduction, out  modelOfferProduction.OfferProduction);

                BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);
                modelOfferProduction.ProductionCalendar = new tblProduction_Calendar();


                for (int ecv = 0; ecv < model.enumCat.Length; ecv++)
                {
                    modelOfferProduction.ProductionControl = new tblProductionControl();

                    modelOfferProduction.ProductionControl.Offer_Production_Id = modelOfferProduction.OfferProduction.Id;
                    modelOfferProduction.ProductionControl.Offer_Production_IdSpecified = true;

                    modelOfferProduction.ProductionControl.EnumCategoryId = model.enumCat[ecv];
                    modelOfferProduction.ProductionControl.EnumCategoryIdSpecified = true;

                    modelOfferProduction.ProductionControl.EnumValueId = model.enumVal[ecv];
                    modelOfferProduction.ProductionControl.EnumValueIdSpecified = true;

                    modelOfferProduction.ProductionControl.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                    modelOfferProduction.ProductionControl.Production_type_eV_IdSpecified = true;

                    BaseOutput ap = srv.WS_AddProductionControl(baseInput, modelOfferProduction.ProductionControl, out modelOfferProduction.ProductionControl);
                }


                modelOfferProduction.ProductionCalendar.Production_Id = modelOfferProduction.OfferProduction.Id;
                modelOfferProduction.ProductionCalendar.Production_IdSpecified = true;

                modelOfferProduction.ProductionCalendar.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.ProductionCalendar.Production_type_eV_IdSpecified = true;

                modelOfferProduction.ProductionCalendar.Months = model.checkedMonth;
                modelOfferProduction.ProductionCalendar.Transportation_eV_Id = model.shippingSchedule;
                modelOfferProduction.ProductionCalendar.Transportation_eV_IdSpecified = true;

                BaseOutput apc = srv.WS_AddProduction_Calendar(baseInput, modelOfferProduction.ProductionCalendar, out modelOfferProduction.ProductionCalendar);



                modelOfferProduction.ProductionDocument = new tblProduction_Document();
                modelOfferProduction.ProductionDocument.grup_Id = Session["documentGrupId"].ToString();
                modelOfferProduction.ProductionDocument.Offer_Production_Id = modelOfferProduction.OfferProduction.Id;
                modelOfferProduction.ProductionDocument.Offer_Production_IdSpecified = true;
                modelOfferProduction.ProductionDocument.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                modelOfferProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                BaseOutput updfg = srv.WS_UpdateProductionDocumentForGroupID(baseInput, modelOfferProduction.ProductionDocument, out modelOfferProduction.ProductionDocumentArray);

            //}

            Session["documentGrupId"] = null;
            TempData["Success"] = modelOfferProduction.messageSuccess;

            return RedirectToAction("Index", "OfferProduction");
        }


        public ActionResult ChooseFileTemplate(int pId)
        {
            baseInput = new BaseInput();
            modelOfferProduction = new OfferProductionViewModel();

            BaseOutput enumcat = srv.WS_GetEnumCategorysByName(baseInput, "documentTypes", out modelOfferProduction.EnumCategory);

            if (modelOfferProduction.EnumCategory == null)
                modelOfferProduction.EnumCategory = new tblEnumCategory();

            BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput, pId, true, out modelOfferProduction.ProductCatalogControlArray);

            modelOfferProduction.ProductCatalogControlDocumentTypeList = modelOfferProduction.ProductCatalogControlArray.Where(x => x.Status == 1).Where(x => x.EnumCategoryId == modelOfferProduction.EnumCategory.Id).ToList();


            BaseOutput enumval = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferProduction.EnumCategory.Id, true, out modelOfferProduction.EnumValueArray);
            modelOfferProduction.EnumValueDocumentTypeList = modelOfferProduction.EnumValueArray.ToList();


            string grup_Id = Session["documentGrupId"].ToString(); ;
            bool flag = true;
            BaseOutput enval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

            BaseOutput tfs = srv.WS_GetDocumentSizebyGroupId(grup_Id, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.totalSize, out flag);

            return View(modelOfferProduction);
        }


        [HttpPost]
        public void File(IList<HttpPostedFileBase> file, int documentType)
        {
            if (file != null)
            {
                string documentGrupId = Session["documentGrupId"].ToString();

                long userId = 0;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        userId = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                baseInput = new BaseInput();
                baseInput.userID = userId;

                modelOfferProduction = new OfferProductionViewModel();

                String sDate = DateTime.Now.ToString();
                DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                String dy = datevalue.Day.ToString();
                String mn = datevalue.Month.ToString();
                String yy = datevalue.Year.ToString();

                string path = modelOfferProduction.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                foreach (var attachfile in file)
                {
                    //var dec=(attachfile.ContentLength / 1024);
                    //var decd = (decimal)731 / 1024;
                    //var fl = Math.Round(dec,2);

                    if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelOfferProduction.fileSize && modelOfferProduction.fileTypes.Contains(attachfile.ContentType))
                    {
                        var fileName = Path.GetFileName(attachfile.FileName);
                        var ofileName = fileName;

                        string ext = string.Empty;
                        int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
                        if (fileExtPos >= 0)
                            ext = fileName.Substring(fileExtPos, fileName.Length - fileExtPos);

                        var newFileName = Guid.NewGuid();
                        fileName = newFileName.ToString() + ext;

                        attachfile.SaveAs(Path.Combine(path, fileName));


                        modelOfferProduction.ProductionDocument = new tblProduction_Document();
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);

                        modelOfferProduction.ProductionDocument.grup_Id = documentGrupId;
                        modelOfferProduction.ProductionDocument.documentUrl = path;
                        modelOfferProduction.ProductionDocument.documentName = fileName;
                        modelOfferProduction.ProductionDocument.documentRealName = ofileName;

                        modelOfferProduction.ProductionDocument.document_type_ev_Id = documentType.ToString();
                        modelOfferProduction.ProductionDocument.documentSize = attachfile.ContentLength;
                        modelOfferProduction.ProductionDocument.documentSizeSpecified = true;

                        modelOfferProduction.ProductionDocument.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.ProductionDocument.Production_type_eV_IdSpecified = true;

                        BaseOutput apd = srv.WS_AddProductionDocument(baseInput, modelOfferProduction.ProductionDocument, out modelOfferProduction.ProductionDocument);
                    }
                }

            }
        }

        public ActionResult SelectedDocuments()
        {
            baseInput = new BaseInput();
            modelOfferProduction = new OfferProductionViewModel();
            string grup_Id = Session["documentGrupId"].ToString();

            BaseOutput gpbu = srv.WS_GetProductionDocuments(baseInput, out modelOfferProduction.ProductionDocumentArray);

            modelOfferProduction.ProductionDocumentList = modelOfferProduction.ProductionDocumentArray.Where(x => x.grup_Id == grup_Id).ToList();

            return View(modelOfferProduction);
        }

        public void DeleteSelectedDocument(long id)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;
            modelOfferProduction = new OfferProductionViewModel();

            BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput, id, true, out modelOfferProduction.ProductionDocument);

            BaseOutput dpd = srv.WS_DeleteProductionDocument(baseInput, modelOfferProduction.ProductionDocument);
        }

        public void DeleteSelectedOfferProduct(long id)
        {
            long userId = 0;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    userId = Int32.Parse(identity.Ticket.UserData);
                }
            }

            baseInput = new BaseInput();
            baseInput.userID = userId;
            modelOfferProduction = new OfferProductionViewModel();

            BaseOutput gpd = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferProduction.OfferProduction);

            BaseOutput dpd = srv.WS_DeleteOffer_Production(baseInput, modelOfferProduction.OfferProduction);
        }
    }
}
