using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using Emsal.AdminUI.Infrastructure;
using Emsal.Utility.CustomObjects;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class OfferProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string sfullAddress;
        private static string suserInfo;
        private static string sstatusEV;
        private static long saddressId;
        private static string sstartDate;
        private static string sendDate;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private OfferProductionViewModel modelOfferProduction;

        public ActionResult Index(int? page, string statusEV = null, string productName = null, string userInfo = null)
        {
            try
            {

                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && userInfo == null)
                {
                    sproductName = null;
                    suserInfo = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (userInfo != null)
                    suserInfo = userInfo;
                if (statusEV != null)
                    sstatusEV = statusEV;

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
                baseInput.userName = modelOfferProduction.Admin.Username;


                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstatusEV, out modelOfferProduction.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId_OP(baseInput, modelOfferProduction.EnumValue.Id, true, pageNumber, true, pageSize, true, out modelOfferProduction.ProductionDetailArray);

                if (modelOfferProduction.ProductionDetailArray == null)
                {
                    modelOfferProduction.ProductionDetailList = new List<ProductionDetail>();
                }
                else
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailArray.ToList();
                }

                //if (sproductName != null)
                //{
                //    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                //}

                //if (suserInfo != null)
                //{
                //    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                //}

                //modelOfferProduction.Paging = modelOfferProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                BaseOutput gppc = srv.WS_GetOfferProductionDetailistForEValueId_OPC(baseInput, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.itemCount, out modelOfferProduction.itemCountB);

                long[] aic = new long[modelOfferProduction.itemCount];

                modelOfferProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);


                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                    modelOfferProduction.isMain = 0;
                else
                    modelOfferProduction.isMain = 1;


                modelOfferProduction.statusEV = sstatusEV;
                modelOfferProduction.productName = sproductName;
                modelOfferProduction.userInfo = suserInfo;

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelOfferProduction)
                   : View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Indexwd(int? page, long addressId = 0, bool excell = false, string startDate = null, string endDate = null)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (addressId == 0 && startDate == null && endDate == null)
                {
                    addressId = 0;
                    sstartDate = null;
                    sendDate = null;
                }

                if (addressId > 0)
                    saddressId = addressId;
                
                if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    sstartDate = null;
                    sendDate = null;
                }
                else
                {
                    sstartDate = startDate;
                    sendDate = endDate;
                    //sstartDate = (Convert.ToDateTime(startDate)).getInt64ShortDate();
                    //sendDate = (Convert.ToDateTime(endDate)).getInt64ShortDate();
                }


                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
                baseInput.userName = modelOfferProduction.Admin.Username;


                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelOfferProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferProduction.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId_OP(baseInput, modelOfferProduction.EnumValue.Id, true, pageNumber, true, pageSize, true, out modelOfferProduction.ProductionDetailArray);

                if (modelOfferProduction.ProductionDetailArray == null)
                {
                    modelOfferProduction.ProductionDetailList = new List<ProductionDetail>();
                }
                else
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailArray.OrderBy(x=>x.fullAddress).ToList();
                }


                //if (sfullAddress != null)
                //{
                //    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.fullAddress.ToLower().Contains(fullAddress)).ToList();
                //}

                //if (suserInfo != null)
                //{
                //    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo) || x.person.gender.ToLower().Contains(suserInfo) || x.personAdress.ToLower().Contains(suserInfo) || x.personAdressDesc.ToLower().Contains(suserInfo)).ToList();
                //}

                //modelOfferProduction.Paging = modelOfferProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                BaseOutput gppc = srv.WS_GetOfferProductionDetailistForEValueId_OPC(baseInput, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.itemCount, out modelOfferProduction.itemCountB);

                long[] aic = new long[modelOfferProduction.itemCount];

                modelOfferProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelOfferProduction.allPagePrice = modelOfferProduction.ProductionDetailList.Sum(x => x.unitPrice);
                modelOfferProduction.currentPagePrice = modelOfferProduction.ProductionDetailList.Sum(x => x.unitPrice);

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                    modelOfferProduction.isMain = 0;
                else
                    modelOfferProduction.isMain = 1;
                
                modelOfferProduction.addressId = saddressId;
                modelOfferProduction.startDate = sstartDate;
                modelOfferProduction.endDate = sendDate;
                //return View(modelDemandProduction);

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Təklif");
                        sheet.Name = "Təklif";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Təklif olunan məhsullar";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 5].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı";
                        sheet.Cells[2, col++].Value = "Vahidi";
                        sheet.Cells[2, col++].Value = "Qiyməti (AZN-lə)";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 15;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 15;
                        sheet.Column(4).Width = 15;
                        sheet.Column(5).Width = 15;

                        int rowIndex = 3;
                        var ri = 1;
                       string pname = "";
                       string opname = "";
                       string m = "";
                       string om = "";
                        foreach (var item in modelOfferProduction.ProductionDetailList)
                        {
                            var col2 = 1;
                            pname = item.person.Name + " " + item.person.Surname + " " + item.person.FatherName;
                            m = item.fullAddress;

                            if (m != om)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;
                                sheet.Cells[rowIndex, 1].Value =  m;

                                sheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 20;
                                rowIndex++;
                            }

                            if (pname != opname)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;
                                sheet.Cells[rowIndex, 1].Value = " S.A.A: " + pname + "\n Qeydiyyat ünvanı: " + item.personAdress + " " + (item.personAdressDesc) + "\n Bank rekvizitləri: ";

                                sheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 5].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 50;
                                sheet.Row(rowIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                rowIndex++;
                            }


                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " (" + item.productParentName+")";
                            sheet.Cells[rowIndex, col2++].Value = item.quantity.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.enumValueName;
                            sheet.Cells[rowIndex, col2++].Value = item.unitPrice.ToString();


                            //sheet.Cells[rowIndex, 1, rowIndex, col2 - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            //sheet.Cells[rowIndex, 1, rowIndex, col2 - 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                            var col3 = 1;
                            rowIndex++;
                            sheet.Cells[rowIndex, col3++].Value = "dövrülük - gün, ay, il";
                            sheet.Cells[rowIndex, col3++].Value = "miqdar";
                            sheet.Cells[rowIndex, col3++].Value = "miqdar (cəmi)";
                            sheet.Cells[rowIndex, col3++].Value = "qiymət";
                            sheet.Cells[rowIndex, col3++].Value = "qiymət (cəmi)";

                            sheet.Row(rowIndex).Style.Font.Bold = true;
                            sheet.Row(rowIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            decimal tquantity;
                            decimal tprice;
                            string day = "-";
                            foreach (var item2 in item.productionCalendarList)
                            {
                                if (item2.day != 0)
                                {
                                    day = item2.day.ToString();
                                }
                                tquantity = (item2.quantity * item2.transportation_eV_Id);
                                tprice = (item2.price * tquantity);
                                var col4 = 1;
                                rowIndex++;
                                sheet.Cells[rowIndex, col4++].Value = item2.TypeDescription + " - " + day + " " + item2.MonthDescription + " " + item2.year;
                                sheet.Cells[rowIndex, col4++].Value = item2.quantity;
                                sheet.Cells[rowIndex, col4++].Value = tquantity;
                                sheet.Cells[rowIndex, col4++].Value = item2.price;
                                sheet.Cells[rowIndex, col4++].Value = tprice;
                            }
                            rowIndex++;
                            ri++;

                            opname = pname;
                            om = m;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 5].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                        sheet.Cells[rowIndex + 1, 2].Value = "Toplam qiyməti: " + modelOfferProduction.allPagePrice + " azn";
                        sheet.Cells[rowIndex + 1, 2].Style.Font.Bold = true;
                        sheet.Cells[rowIndex + 1, 2].Style.WrapText = true;

                        string fileName = Guid.NewGuid() + ".xls";

                        Response.ClearContent();
                        Response.BinaryWrite(excelPackage.GetAsByteArray());
                        Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                        Response.AppendCookie(new HttpCookie("fileDownloadToken", "1111"));
                        Response.ContentType = "application/excel";
                        Response.Flush();
                        Response.End();
                    }
                }

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndexwd", modelOfferProduction)
                   : View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public DataTable CustomTable(DataTable excelTable)
        {
            var table = new DataTable();
            var col0 = table.Columns.Add("Name", typeof(String));
            var col1 = table.Columns.Add("Age", typeof(int));
            var col2 = table.Columns.Add("Arabic Name", typeof(String));
            var col3 = table.Columns.Add("Category", typeof(String));
            var ok = false;
            for (var index = 0; index < excelTable.Rows.Count; index++)
            {
                DataRow excelRow = excelTable.Rows[index];
                if (ok)
                {
                    DataRow row = table.NewRow();
                    row[col0] = String.Format("{0}", excelRow["Name"]);
                    row[col1] = Convert.ToInt32(excelRow["Age"]);
                    row[col2] = String.Format("{0}", excelRow["Arabic Name"]);
                    row[col3] = String.Format("{0}", excelRow["Category"]);
                    table.Rows.Add(row);
                }
                else
                {
                    ok = (excelRow[0].ToString() == "Name");
                }
            }
            return table;
        }

        [HttpPost]
        public ActionResult Approv(int[] ids)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
                baseInput.userName = modelOfferProduction.Admin.Username;

                //Array arrid = ids.Split(',');
                //long id = 0;
                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        modelOfferProduction.OfferProduction = new tblOffer_Production();

                        BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, ids[i], true, out modelOfferProduction.OfferProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelOfferProduction.EnumValueST);

                        modelOfferProduction.OfferProduction.state_eV_Id = modelOfferProduction.EnumValueST.Id;
                        modelOfferProduction.OfferProduction.state_eV_IdSpecified = true;

                        modelOfferProduction.OfferProduction.isNew = 1;
                        modelOfferProduction.OfferProduction.isNewSpecified = true;

                        BaseOutput ecout = srv.WS_UpdateOffer_Production(baseInput, modelOfferProduction.OfferProduction, out modelOfferProduction.OfferProduction);

                        modelOfferProduction.ComMessage = new tblComMessage();
                        modelOfferProduction.ComMessage.message = "Təsdiqləndi";
                        modelOfferProduction.ComMessage.fromUserID = (long)UserId;
                        modelOfferProduction.ComMessage.fromUserIDSpecified = true;
                        modelOfferProduction.ComMessage.toUserID = modelOfferProduction.OfferProduction.user_Id;
                        modelOfferProduction.ComMessage.toUserIDSpecified = true;
                        modelOfferProduction.ComMessage.Production_Id = modelOfferProduction.OfferProduction.Id;
                        modelOfferProduction.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out modelOfferProduction.EnumValue);
                        modelOfferProduction.ComMessage.Production_type_eV_Id = modelOfferProduction.EnumValue.Id;
                        modelOfferProduction.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelOfferProduction.ComMessage, out modelOfferProduction.ComMessage);
                    }
                }

                return RedirectToAction("Index", "OfferProduction", new { statusEV = modelOfferProduction.EnumValueST.name });

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
                modelOfferProduction = new OfferProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
                baseInput.userName = modelOfferProduction.Admin.Username;

                BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, id, true, out modelOfferProduction.OfferProduction);

                modelOfferProduction.Id = modelOfferProduction.OfferProduction.Id;

                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(OfferProductionViewModel model, FormCollection collection)
        {
            try
            {

                baseInput = new BaseInput();

                model.ConfirmationMessage = new tblConfirmationMessage();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out model.Admin);
                baseInput.userName = model.Admin.Username;

                model.ConfirmationMessage.Message = model.message;

                BaseOutput pout = srv.WS_SendConfirmationMessageNew(baseInput, model.ConfirmationMessage, out model.ConfirmationMessage);


                model.OfferProduction = new tblOffer_Production();

                BaseOutput bouput = srv.WS_GetOffer_ProductionById(baseInput, model.Id, true, out model.OfferProduction);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reject", out model.EnumValueST);

                model.OfferProduction.state_eV_Id = model.EnumValueST.Id;
                model.OfferProduction.state_eV_IdSpecified = true;

                model.OfferProduction.isNew = 1;
                model.OfferProduction.isNewSpecified = true;

                BaseOutput ecout = srv.WS_UpdateOffer_Production(baseInput, model.OfferProduction, out model.OfferProduction);

                model.ComMessage = new tblComMessage();
                model.ComMessage.message = model.message;
                model.ComMessage.fromUserID = (long)UserId;
                model.ComMessage.fromUserIDSpecified = true;
                model.ComMessage.toUserID = model.OfferProduction.user_Id;
                model.ComMessage.toUserIDSpecified = true;
                model.ComMessage.Production_Id = model.OfferProduction.Id;
                model.ComMessage.Production_IdSpecified = true;
                BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "offer", out model.EnumValue);
                model.ComMessage.Production_type_eV_Id = model.EnumValue.Id;
                model.ComMessage.Production_type_eV_IdSpecified = true;

                BaseOutput acm = srv.WS_AddComMessage(baseInput, model.ComMessage, out model.ComMessage);

                return RedirectToAction("Index", "OfferProduction", new { statusEV = model.EnumValueST.name });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult ProductCatalogForSale(string actionName)
        {
            try
            {
                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
                baseInput.userName = modelOfferProduction.Admin.Username;

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelOfferProduction.ProductCatalogDetailArray);

                if (modelOfferProduction.ProductCatalogDetailArray == null)
                {
                    modelOfferProduction.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelOfferProduction.ProductCatalogDetailList = modelOfferProduction.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }
                modelOfferProduction.actionName = actionName;
                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult AdminUnit(string actionName)
        {
            try
            {
                baseInput = new BaseInput();
                modelOfferProduction = new OfferProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferProduction.Admin);
                baseInput.userName = modelOfferProduction.Admin.Username;


                BaseOutput bouput = srv.WS_GetAdminUnitsByParentId(baseInput,1,true, out modelOfferProduction.PRMAdminUnitArray);

                if (modelOfferProduction.PRMAdminUnitArray == null)
                {
                    modelOfferProduction.PRMAdminUnitList = new List<tblPRM_AdminUnit>();
                }
                else
                {
                    modelOfferProduction.PRMAdminUnitList = modelOfferProduction.PRMAdminUnitArray.ToList();
                }
                modelOfferProduction.actionName = actionName;
                return View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
