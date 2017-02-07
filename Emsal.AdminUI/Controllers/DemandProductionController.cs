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
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace Emsal.AdminUI.Controllers
{
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class DemandProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string sfullAddress;
        private static string suserInfo;
        private static string sadminUnit;
        private static string sstatusEV;
        private static string sstartDate;
        private static string sendDate;
        private static long sproductId;
        private static string saddressIdString;
        private static long seconomicZoneId;
        private static long sorganisationId;
        private static long syear;
        private static long saddressId;
        private static long suserType;
        private static string sfinVoen;


        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private DemandProductionViewModel modelDemandProduction;

        public ActionResult Index(int? page, string statusEV = null, long productId = -1, string userInfo = null, string adminUnit = null)
        {
            try
            {

                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());
                if (adminUnit != null)
                    adminUnit = StripTag.strSqlBlocker(adminUnit.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productId == -1 && userInfo == null && adminUnit == null)
                {
                    sproductId = 0;
                    suserInfo = null;
                    sadminUnit = null;
                }

                if (productId >= 0)
                    sproductId = productId;
                if (userInfo != null)
                    suserInfo = userInfo;
                if (adminUnit != null)
                    sadminUnit = adminUnit;
                if (statusEV != null)
                    sstatusEV = statusEV;

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstatusEV, out modelDemandProduction.EnumValue);

                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch = new GetDemandProductionDetailistForEValueIdSearch();

                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.state_eV_Id = modelDemandProduction.EnumValue.Id;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.page = pageNumber;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.pageSize = pageSize;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.prodcutID = sproductId;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.Name = suserInfo;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.organizationName = sadminUnit;

                BaseOutput gpp = srv.WS_GetDemandProductionDetailistForEValueId_OP(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, out modelDemandProduction.ProductionDetailArray);

                if (modelDemandProduction.ProductionDetailArray == null)
                {
                    modelDemandProduction.ProductionDetailList = new List<ProductionDetail>();
                }
                else
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.ToList();

                    //modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelDemandProduction.EnumCategory.Id && x.foreignOrganization != null).ToList();
                }

                BaseOutput gdpc = srv.WS_GetDemandProductionDetailistForEValueId_OPC(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, out modelDemandProduction.itemCount, out modelDemandProduction.itemCountB);

                long[] aic = new long[modelDemandProduction.itemCount];

                modelDemandProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);


                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                {
                    modelDemandProduction.isMain = 0;
                }
                else
                {
                    modelDemandProduction.isMain = 1;
                }

                modelDemandProduction.statusEV = sstatusEV;
                modelDemandProduction.productId = sproductId;
                modelDemandProduction.userInfo = suserInfo;
                modelDemandProduction.adminUnit = sadminUnit;
                //return View(modelDemandProduction);

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelDemandProduction)
                   : View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Indexwd(int? page, bool excell = false, string startDate = null, string endDate = null)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

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
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValue);

                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch = new GetDemandProductionDetailistForEValueIdSearch();

                if (excell == true)
                {
                    pageNumber = 1;
                    pageSize = 10000;
                }

                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.state_eV_Id = modelDemandProduction.EnumValue.Id;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.page = pageNumber;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.pageSize = pageSize;

                //if (excell == false)
                //{
                BaseOutput gpp = srv.WS_GetDemandProductionDetailistForEValueId_OP(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, out modelDemandProduction.ProductionDetailArray);
                //}
                //else
                //{
                //    BaseOutput gpp = srv.WS_GetDemandProductionDetailistForEValueId_OP(baseInput, modelDemandProduction.EnumValue.Id, true, 1, true, 1000, true, out modelDemandProduction.ProductionDetailArray);
                //}

                if (modelDemandProduction.ProductionDetailArray == null)
                {
                    modelDemandProduction.ProductionDetailList = new List<ProductionDetail>();
                }
                else
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.OrderBy(x => x.foreignOrganization.name).ToList();
                }

                //if (sfullAddress != null)
                //{
                //    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.fullAddress.ToLower().Contains(sfullAddress) || x.addressDesc.ToLower().Contains(sfullAddress)).ToList();
                //}
                //if (sadminUnit != null)
                //{
                //    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.foreignOrganization.name.ToLower().Contains(sadminUnit)).ToList();
                //}

                //if (suserInfo != null)
                //{
                //    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                //}


                //modelDemandProduction.Paging = modelDemandProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                BaseOutput gdpc = srv.WS_GetDemandProductionDetailistForEValueId_OPC(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, out modelDemandProduction.itemCount, out modelDemandProduction.itemCountB);

                long[] aic = new long[modelDemandProduction.itemCount];

                modelDemandProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);


                //foreach (var item in modelDemandProduction.ProductionDetailList)
                //{
                //    modelDemandProduction.currentPagePrice = modelDemandProduction.currentPagePrice + (item.quantity * item.productUnitPrice);
                //}

                //foreach (var item in modelDemandProduction.ProductionDetailList)
                //{
                //    modelDemandProduction.allPagePrice = modelDemandProduction.allPagePrice + (item.quantity * item.productUnitPrice);
                //}

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                {
                    modelDemandProduction.isMain = 0;
                }
                else
                {
                    modelDemandProduction.isMain = 1;
                }


                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;
                //return View(modelDemandProduction);

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb");
                        sheet.Name = "Tələb";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Satınalan təşkilatlar üzrə tələbat cədvəli (ətraflı)";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 4].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı (vahidi)";
                        sheet.Cells[2, col++].Value = "Qiyməti (vahidin)";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 10;
                        sheet.Column(2).Width = 35;
                        sheet.Column(3).Width = 15;
                        sheet.Column(4).Width = 15;

                        int rowIndex = 3;
                        var ri = 1;
                        string auname = "";
                        string oauname = "";

                        foreach (var item in modelDemandProduction.ProductionDetailList)
                        {
                            var col2 = 1;
                            if (item.foreignOrganization != null)
                            {
                                auname = item.foreignOrganization.name;
                            }

                            if (auname != oauname)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 4].Merge = true;
                                sheet.Cells[rowIndex, 1].Value = auname + "\n" + item.fullAddress + " (" + item.addressDesc + ")";

                                sheet.Cells[rowIndex, 1, rowIndex, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 4].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 50;
                                rowIndex++;
                            }

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " (" + item.productParentName + ")";
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.quantity) + " " + item.enumValueName;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.productUnitPrice);

                            sheet.Cells[rowIndex, 1, rowIndex, col2 - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            sheet.Cells[rowIndex, 1, rowIndex, col2 - 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                            var col3 = 1;
                            rowIndex++;
                            sheet.Cells[rowIndex, col3++].Value = "dövrülük";
                            sheet.Cells[rowIndex, col3++].Value = "gün, ay, il";
                            sheet.Cells[rowIndex, col3++].Value = "miqdar";
                            sheet.Cells[rowIndex, col3++].Value = "miqdar (cəmi)";

                            sheet.Row(rowIndex).Style.Font.Bold = true;
                            sheet.Row(rowIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            decimal tquantity;
                            string day = "-";
                            foreach (var item2 in item.productionCalendarList)
                            {
                                if (item2.day != 0)
                                {
                                    day = item2.day.ToString();
                                }
                                tquantity = (item2.quantity * item2.transportation_eV_Id);
                                var col4 = 1;
                                rowIndex++;
                                sheet.Cells[rowIndex, col4++].Value = item2.TypeDescription;
                                sheet.Cells[rowIndex, col4++].Value = day + " " + item2.MonthDescription + " " + item2.year;
                                sheet.Cells[rowIndex, col4++].Value = Custom.ConverPriceDelZero((decimal)item2.quantity);
                                sheet.Cells[rowIndex, col4++].Value = Custom.ConverPriceDelZero((decimal)tquantity);
                            }
                            rowIndex++;
                            ri++;

                            oauname = auname;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                        sheet.Cells[rowIndex + 1, 2].Value = "Toplam qiyməti: " + modelDemandProduction.allPagePrice + " azn";
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
                   ? (ActionResult)PartialView("PartialIndexwd", modelDemandProduction)
                   : View(modelDemandProduction);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult DemandByForganistion(int? page, bool excell = false, string startDate = null, string endDate = null, long economicZoneId = -1, string addressId = null, long organisationId = -1, long productId = -1, long year = -1)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (economicZoneId == -1 && addressId == null && organisationId == -1 && productId == -1 && year == -1 && startDate == null && endDate == null)
                {
                    seconomicZoneId = 0;
                    saddressIdString = null;
                    sorganisationId = 0;
                    sproductId = 0;
                    syear = 0;
                    sstartDate = null;
                    sendDate = null;
                }

                if (economicZoneId >= 0)
                {
                    seconomicZoneId = economicZoneId;
                    saddressId = 0;
                }
                if (addressId != null)
                {
                    saddressIdString = addressId;
                    sorganisationId = 0;
                }
                if (organisationId >= 0)
                    sorganisationId = organisationId;
                if (productId >= 0)
                    sproductId = productId;
                if (year >= 0)
                    syear = year;

                if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    sstartDate = null;
                    sendDate = null;
                }
                else
                {
                    sstartDate = startDate;
                    sendDate = endDate;
                }

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValue);

                if (excell == true)
                {
                    pageNumber = 1;
                    pageSize = 1000000;
                }

                if (!string.IsNullOrEmpty(saddressIdString))
                {
                    modelDemandProduction.addressIdList = saddressIdString.Split(',').Select(long.Parse).ToArray();
                }


                modelDemandProduction.DemandForegnOrganization = new DemandForegnOrganization();

                modelDemandProduction.DemandForegnOrganization.page = pageNumber;
                modelDemandProduction.DemandForegnOrganization.page_size = pageSize;
                modelDemandProduction.DemandForegnOrganization.regionId = seconomicZoneId;
                modelDemandProduction.DemandForegnOrganization.addressID = saddressId;
                modelDemandProduction.DemandForegnOrganization.organizationID = sorganisationId;
                modelDemandProduction.DemandForegnOrganization.productID = sproductId;
                modelDemandProduction.DemandForegnOrganization.year = syear;

                BaseOutput gpp = srv.WS_GetDemandByForganistion_OP(baseInput, modelDemandProduction.DemandForegnOrganization, out modelDemandProduction.OrganizationDetailArray);


                if (modelDemandProduction.OrganizationDetailArray == null)
                {
                    modelDemandProduction.OrganizationDetailList = new List<OrganizationDetail>();
                }
                else
                {
                    modelDemandProduction.OrganizationDetailList = modelDemandProduction.OrganizationDetailArray.ToList();
                }

                BaseOutput gdpc = srv.WS_GetDemandByForganistion_OPC(baseInput, modelDemandProduction.DemandForegnOrganization, out modelDemandProduction.itemCount, out modelDemandProduction.itemCountB);

                long[] aic = new long[modelDemandProduction.itemCount];

                modelDemandProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.economicZoneId = seconomicZoneId;
                modelDemandProduction.addressIdString = saddressIdString;
                modelDemandProduction.organisationId = sorganisationId;
                modelDemandProduction.productId = sproductId;
                modelDemandProduction.year = syear;
                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb");
                        sheet.Name = "Tələb";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Proqnoz";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 10].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Bölgə";
                        sheet.Cells[2, col++].Value = "Şəhər";
                        sheet.Cells[2, col++].Value = "Vöen";
                        sheet.Cells[2, col++].Value = "Müəssisə";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı, növü və spesifikasiyasi";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Miqdar";
                        sheet.Cells[2, col++].Value = "Vahidin qiyməti, AZN";
                        sheet.Cells[2, col++].Value = "Ümumi dəyəri, AZN";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 7;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 20;
                        sheet.Column(4).Width = 30;
                        sheet.Column(5).Width = 30;
                        sheet.Column(6).Width = 25;
                        sheet.Column(7).Width = 10;
                        sheet.Column(8).Width = 20;
                        sheet.Column(9).Width = 20;
                        sheet.Column(10).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;
                        string auname = "";
                        decimal tquantity = 0;

                        foreach (var item in modelDemandProduction.OrganizationDetailList)
                        {
                            var col2 = 1;
                            tquantity = item.quantity * item.unit_price;

                            modelDemandProduction.auArrName = item.fullAddress.Split(',').ToArray();

                            if (modelDemandProduction.auArrName.Count() > 1)
                            {
                                auname = modelDemandProduction.auArrName[1];
                            }
                            else if (modelDemandProduction.auArrName.Count() == 1)
                            {
                                auname = modelDemandProduction.auArrName[0];
                            }
                            else
                            {
                                auname = "";
                            }


                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.regionName;
                            sheet.Cells[rowIndex, col2++].Value = item.adminName;

                            sheet.Cells[rowIndex, col2++].IsRichText = true;
                            col2--;
                            ExcelRichTextCollection rtfCollection = sheet.Cells[rowIndex, col2++].RichText;
                            ExcelRichText ert = rtfCollection.Add(item.managerSurname + " " + item.managerName + " " + item.fatherName + "\n");
                            ert.Bold = false;

                            if (!string.IsNullOrEmpty(item.pinNumber.Trim()))
                            {
                                ert = rtfCollection.Add("FİN: ");
                                ert.Bold = true;
                                ert = rtfCollection.Add(item.pinNumber);
                                ert.Bold = false;
                            }

                            if (!string.IsNullOrEmpty(item.voen.Trim()))
                            {
                                if (!string.IsNullOrEmpty(item.pinNumber.Trim()))
                                {
                                    ert = rtfCollection.Add("\n");
                                    ert.Bold = false;
                                }

                                ert = rtfCollection.Add("VÖEN: ");
                                ert.Bold = true;
                                ert = rtfCollection.Add(item.voen);
                                ert.Bold = false;
                            }


                            sheet.Cells[rowIndex, col2++].Value = item.organizationName;
                            sheet.Cells[rowIndex, col2++].Value = item.prodcutName + " (" + item.parentProductName + ")";
                            sheet.Cells[rowIndex, col2++].Value = item.unitOfMeasurement;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.quantity);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unit_price);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)tquantity);

                            //sheet.Row(rowIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            rowIndex++;
                            ri++;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 10].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialDemandByForganistion", modelDemandProduction)
                   : View(modelDemandProduction);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult TotalDemandOffers(int? page, bool excell = false, long productId = -1, long userType = -1, string finVoen = null)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productId == -1 && userType == -1 && finVoen == null)
                {
                    sproductId = 0;
                    suserType = 0;
                    sfinVoen = null;
                }

                if (productId >= 0)
                    sproductId = productId;
                if (userType >= 0)
                    suserType = userType;
                if (finVoen != null)
                    sfinVoen = finVoen;

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValue);

                if (excell == true)
                {
                    pageNumber = 1;
                    pageSize = 1000000;
                }

                modelDemandProduction.DemandOfferProductsSearch = new DemandOfferProductsSearch();

                modelDemandProduction.DemandOfferProductsSearch.productId = sproductId;
                modelDemandProduction.DemandOfferProductsSearch.roleID = suserType;
                modelDemandProduction.DemandOfferProductsSearch.pinNumber = sfinVoen;
                modelDemandProduction.DemandOfferProductsSearch.voen = sfinVoen;

                BaseOutput gpp = srv.WS_GetTotalDemandOffers(baseInput, pageNumber, true, pageSize, true, modelDemandProduction.DemandOfferProductsSearch, out modelDemandProduction.DemanProductionGroupArray);


                if (modelDemandProduction.DemanProductionGroupArray == null)
                {
                    modelDemandProduction.DemanProductionGroupList = new List<DemanProductionGroup>();
                }
                else
                {
                    modelDemandProduction.DemanProductionGroupList = modelDemandProduction.DemanProductionGroupArray.OrderBy(x => x.productParentName).ToList();
                }

                BaseOutput gdpc = srv.WS_GetTotalDemandOffers_OPC(baseInput, modelDemandProduction.DemandOfferProductsSearch, out modelDemandProduction.itemCount, out modelDemandProduction.itemCountB);

                long[] aic = new long[modelDemandProduction.itemCount];

                modelDemandProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.productId = sproductId;
                modelDemandProduction.userType = suserType;
                modelDemandProduction.finVoen = sfinVoen;

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb");
                        sheet.Name = "Tələb";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Məhsul üzrə tələb və təkliflər";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 15].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı və növü";
                        sheet.Cells[2, col++].Value = "Tələbatın həcmi (miqdar)";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Cəm (AZN-lə)";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının adı";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının VÖEN-i";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçı ilə alqı-satqı müqaviləsinin tarixi və nömrəsi";
                        sheet.Cells[2, col++].Value = "Müqavilə üzrə malın qiyməti";
                        sheet.Cells[2, col++].Value = "Müqavilə üzrə malın həcmi (miqdarı)";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının nümayəndəsinin adı və əlaqə vasitəsi";
                        sheet.Cells[2, col++].Value = "Tədarükçünün növü (istehsalçı, satıcı və ya idxalçı)";
                        sheet.Cells[2, col++].Value = "Tədarükçünün hansı növ vergi ödəyicisi olması (ƏDV, sadələşdirilmiş K/T məhsulu istehsalçısı )";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 7;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 20;
                        sheet.Column(4).Width = 20;
                        sheet.Column(5).Width = 20;
                        sheet.Column(6).Width = 20;
                        sheet.Column(7).Width = 40;
                        sheet.Column(8).Width = 20;
                        sheet.Column(9).Width = 20;
                        sheet.Column(10).Width = 20;
                        sheet.Column(11).Width = 20;
                        sheet.Column(12).Width = 20;
                        sheet.Column(13).Width = 20;
                        sheet.Column(14).Width = 20;
                        sheet.Column(15).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;
                        string orgName = "";
                        string ppname = "";
                        string oppname = "";

                        foreach (var item in modelDemandProduction.DemanProductionGroupList)
                        {
                            foreach (var itemo in item.offerProductsList)
                            {
                                ppname = item.productParentName;
                            var col2 = 1;
                                orgName = "";

                                if (ppname != oppname)
                                {
                                    sheet.Cells[rowIndex, 1, rowIndex, 4].Merge = true;

                                    sheet.Cells[rowIndex, 1].Value = ppname;

                                    sheet.Cells[rowIndex, 1, rowIndex, 15].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    sheet.Cells[rowIndex, 1, rowIndex, 15].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                    sheet.Row(rowIndex).Height = 20;
                                    rowIndex++;
                                }

                                sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                                sheet.Cells[rowIndex, col2++].Value = item.productName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalQuantity);

                                //NumberFormatInfo nfi = new CultureInfo("az-Latn-AZ").NumberFormat;

                                //nfi.NumberGroupSeparator = " ";
                                //sheet.Cells[rowIndex, col2++].Value = double.Parse("23 232 323.54", nfi);


                                //sheet.Cells[rowIndex, col2++].Value = decimal.Parse("5 454 500.85", new NumberFormatInfo() { NumberGroupSeparator = " " });

                                //sheet.Cells[rowIndex, col2++].Value = "5 454 500.85";                               


                                sheet.Cells[rowIndex, col2++].Value = item.enumValueName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalQuantityPrice);

                                if (!string.IsNullOrEmpty(itemo.voen.Trim()))
                                {
                                    orgName = "\n" + itemo.organizationName;
                                }

                                sheet.Cells[rowIndex, col2++].Value = itemo.personName + " " + itemo.surname + " " + itemo.fatherName+ orgName;


                                sheet.Cells[rowIndex, col2++].IsRichText = true;
                                col2--;
                                ExcelRichTextCollection rtfCollection = sheet.Cells[rowIndex, col2++].RichText;
                                ExcelRichText ert;

                                if (!string.IsNullOrEmpty(itemo.pinNumber.Trim()))
                                {
                                    ert = rtfCollection.Add("FİN: ");
                                    ert.Bold = true;
                                    ert = rtfCollection.Add(itemo.pinNumber);
                                    ert.Bold = false;
                                }

                                if (!string.IsNullOrEmpty(itemo.voen.Trim()))
                                {
                                    if (!string.IsNullOrEmpty(itemo.pinNumber.Trim()))
                                    {
                                        ert = rtfCollection.Add("\n");
                                        ert.Bold = false;
                                    }

                                    ert = rtfCollection.Add("VÖEN: ");
                                    ert.Bold = true;
                                    ert = rtfCollection.Add(itemo.voen);
                                    ert.Bold = false;
                                }

                                sheet.Cells[rowIndex, col2++].Value = "";
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.totalQuantityPrice);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.quantity);
                                sheet.Cells[rowIndex, col2++].Value = itemo.enumValueName;

                                sheet.Cells[rowIndex, col2++].Value = string.Join(", ", itemo.comList.Select(x => x.communication).LastOrDefault());
                                sheet.Cells[rowIndex, col2++].Value = itemo.roledesc;

                                sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                
                                //sheet.Row(rowIndex).Style.Numberformat.Format = "0:#,##0.000000";

                                rowIndex++;
                            ri++;

                            oppname = ppname;
                            }
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 15].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 15].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 15].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 15].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialTotalDemandOffers", modelDemandProduction)
                   : View(modelDemandProduction);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult TotalDemandOffersn(int? page, bool excell = false, long productId = -1, long userType = -1, string finVoen = null)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productId == -1 && userType == -1 && finVoen == null)
                {
                    sproductId = 0;
                    suserType = 0;
                    sfinVoen = null;
                }

                if (productId >= 0)
                    sproductId = productId;
                if (userType >= 0)
                    suserType = userType;
                if (finVoen != null)
                    sfinVoen = finVoen;

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }

                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValue);

                if (excell == true)
                {
                    pageNumber = 1;
                    pageSize = 1000000;
                }

                modelDemandProduction.DemandOfferProductsSearch = new DemandOfferProductsSearch();

                modelDemandProduction.DemandOfferProductsSearch.productId = sproductId;
                modelDemandProduction.DemandOfferProductsSearch.roleID = suserType;
                modelDemandProduction.DemandOfferProductsSearch.pinNumber = sfinVoen;
                modelDemandProduction.DemandOfferProductsSearch.voen = sfinVoen;

                BaseOutput gpp = srv.WS_GetTotalDemandOffers(baseInput, pageNumber, true, pageSize, true, modelDemandProduction.DemandOfferProductsSearch, out modelDemandProduction.DemanProductionGroupArray);


                if (modelDemandProduction.DemanProductionGroupArray == null)
                {
                    modelDemandProduction.DemanProductionGroupList = new List<DemanProductionGroup>();
                }
                else
                {
                    modelDemandProduction.DemanProductionGroupList = modelDemandProduction.DemanProductionGroupArray.OrderBy(x => x.productParentName).ToList();
                }

                BaseOutput gdpc = srv.WS_GetTotalDemandOffers_OPC(baseInput, modelDemandProduction.DemandOfferProductsSearch, out modelDemandProduction.itemCount, out modelDemandProduction.itemCountB);

                long[] aic = new long[modelDemandProduction.itemCount];

                modelDemandProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.productId = sproductId;
                modelDemandProduction.userType = suserType;
                modelDemandProduction.finVoen = sfinVoen;

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb");
                        sheet.Name = "Tələb";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Məhsul üzrə tələb və təkliflər";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 16].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı və növü";
                        sheet.Cells[2, col++].Value = "Tələbatın həcmi (miqdar)";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Cəmi (Manatla)";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının adı";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının VÖEN-i";
                        sheet.Cells[2, col++].Value = "Malın təklif edilən qiyməti";
                        sheet.Cells[2, col++].Value = "Malın həcmi (miqdarı)";
                        sheet.Cells[2, col++].Value = "Ümumi dəyər (manatla)";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının nümayəndəsinin adı";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının nümayəndəsinin əlaqə vasitəsi";
                        sheet.Cells[2, col++].Value = "Tədarükçünün növü (istehsalçı, satıcı və ya idxalçı)";
                        sheet.Cells[2, col++].Value = "Tədarükçünün hansı növ vergi ödəyicisi olması (ƏDV, sadələşdirilmiş K/T məhsulu istehsalçısı )";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[2, 1, 2, 16].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        sheet.Cells[2, 1, 2, 16].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 7;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 20;
                        sheet.Column(4).Width = 20;
                        sheet.Column(5).Width = 20;
                        sheet.Column(6).Width = 20;
                        sheet.Column(7).Width = 20;
                        sheet.Column(8).Width = 30;
                        sheet.Column(9).Width = 20;
                        sheet.Column(10).Width = 20;
                        sheet.Column(11).Width = 20;
                        sheet.Column(12).Width = 20;
                        sheet.Column(13).Width = 30;
                        sheet.Column(14).Width = 20;
                        sheet.Column(15).Width = 20;
                        sheet.Column(16).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;

                        foreach (var item in modelDemandProduction.DemanProductionGroupList)
                        {
                            foreach (var itemo in item.offerProductsList)
                            {
                                var col2 = 1;

                                sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                                sheet.Cells[rowIndex, col2++].Value = item.productParentName;
                                sheet.Cells[rowIndex, col2++].Value = item.productName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalQuantity);

                                //NumberFormatInfo nfi = new CultureInfo("az-Latn-AZ").NumberFormat;

                                //nfi.NumberGroupSeparator = " ";
                                //sheet.Cells[rowIndex, col2++].Value = double.Parse("23 232 323.54", nfi);


                                //sheet.Cells[rowIndex, col2++].Value = decimal.Parse("5 454 500.85", new NumberFormatInfo() { NumberGroupSeparator = " " });

                                //sheet.Cells[rowIndex, col2++].Value = "5 454 500.85";                               


                                sheet.Cells[rowIndex, col2++].Value = item.enumValueName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalQuantityPrice);

                                if (!string.IsNullOrEmpty(itemo.voen.Trim()))
                                {
                                    sheet.Cells[rowIndex, col2++].Value = itemo.organizationName;
                                }
                                else
                                {
                                sheet.Cells[rowIndex, col2++].Value = itemo.personName + " " + itemo.surname + " " + itemo.fatherName;
                                }


                                sheet.Cells[rowIndex, col2++].IsRichText = true;
                                col2--;
                                ExcelRichTextCollection rtfCollection = sheet.Cells[rowIndex, col2++].RichText;
                                ExcelRichText ert;

                                if (!string.IsNullOrEmpty(itemo.pinNumber.Trim()))
                                {
                                    ert = rtfCollection.Add("FİN: ");
                                    ert.Bold = true;
                                    ert = rtfCollection.Add(itemo.pinNumber);
                                    ert.Bold = false;
                                }

                                if (!string.IsNullOrEmpty(itemo.voen.Trim()))
                                {
                                    if (!string.IsNullOrEmpty(itemo.pinNumber.Trim()))
                                    {
                                        ert = rtfCollection.Add("\n");
                                        ert.Bold = false;
                                    }

                                    ert = rtfCollection.Add("VÖEN: ");
                                    ert.Bold = true;
                                    ert = rtfCollection.Add(itemo.voen);
                                    ert.Bold = false;
                                }

                                sheet.Cells[rowIndex, col2++].Value = "";
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.quantity);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.totalQuantityPrice);

                                sheet.Cells[rowIndex, col2++].Value = itemo.personName + " " + itemo.surname + " " + itemo.fatherName;

                                sheet.Cells[rowIndex, col2++].Value = string.Join(", ", itemo.comList.Select(x => x.communication).LastOrDefault());
                                sheet.Cells[rowIndex, col2++].Value = itemo.roledesc;

                                sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                //sheet.Row(rowIndex).Style.Numberformat.Format = "0:#,##0.000000";

                                sheet.Cells[rowIndex, 8, rowIndex, 16].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 8, rowIndex, 16].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                                rowIndex++;
                                ri++;
                            }
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 16].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialTotalDemandOffersn", modelDemandProduction)
                   : View(modelDemandProduction);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DemandProductDetailInfoForAccounting(int? page, long productId = -1, bool excell = false, string startDate = null, string endDate = null)
        {
            try
            {
                if (startDate != null)
                    startDate = StripTag.strSqlBlocker(startDate.ToLower());
                if (endDate != null)
                    endDate = StripTag.strSqlBlocker(endDate.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);


                if (productId == -1 && startDate == null && endDate == null)
                {
                    sproductId = 0;
                    sstartDate = null;
                    sendDate = null;
                }


                if (productId >= 0)
                    sproductId = productId;

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
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                if (sstartDate != null && sendDate != null)
                {

                }

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "tesdiqlenen", out modelDemandProduction.EnumValue);

                DateTime date = DateTime.Now;
                long year = date.Year;
                long rub = Int32.Parse(String.Format("{0}", (date.Month + 2) / 3));

                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch = new GetDemandProductionDetailistForEValueIdSearch();

                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.state_eV_Id = modelDemandProduction.EnumValue.Id;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.page = pageNumber;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.pageSize = pageSize;
                modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch.prodcutID = sproductId;

                BaseOutput gpp = srv.WS_GetDemandProductDetailInfoForAccounting_OP(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, year, true, rub, true, out modelDemandProduction.ProductionDetailArray);

                if (modelDemandProduction.ProductionDetailArray == null)
                {
                    modelDemandProduction.ProductionDetailList = new List<ProductionDetail>();
                }
                else
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.OrderBy(x => x.name).ToList();
                }


                BaseOutput gdpc = srv.WS_GetDemandProductDetailInfoForAccounting_OPC(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, year, true, rub, true, out modelDemandProduction.itemCount, out modelDemandProduction.itemCountB);

                long[] aic = new long[modelDemandProduction.itemCount];

                modelDemandProduction.PagingT = aic.ToPagedList(pageNumber, pageSize);


                BaseOutput gdp = srv.WS_GetDemandProductDetailInfoForAccounting_OPP(baseInput, modelDemandProduction.GetDemandProductionDetailistForEValueIdSearch, year, true, rub, true, out modelDemandProduction.tPrice, out modelDemandProduction.tPriceSp);


                foreach (var item in modelDemandProduction.ProductionDetailList)
                {
                    modelDemandProduction.currentPagePrice = modelDemandProduction.currentPagePrice + (item.quantity * item.unitPrice);
                }


                modelDemandProduction.productId = sproductId;
                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;


                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb");
                        sheet.Name = "Tələb";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Satınalan təşkilatlar üzrə tələbat cədvəli (ümumi)";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 6].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı (vahidi)";
                        sheet.Cells[2, col++].Value = "Qiymət (vahidin)";
                        sheet.Cells[2, col++].Value = "Toplam qiymət";
                        sheet.Cells[2, col++].Value = "Dövrülük";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 15;
                        sheet.Column(4).Width = 10;
                        sheet.Column(5).Width = 15;
                        sheet.Column(6).Width = 10;

                        int rowIndex = 3;
                        var ri = 1;
                        string otname = "";
                        foreach (var item in modelDemandProduction.ProductionDetailList)
                        {
                            var col2 = 1;

                            if (otname != item.name)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;
                                sheet.Cells[rowIndex, 1].Value = item.name + "\n (" + item.fullAddress + " (" + item.addressDesc + "))";

                                sheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 50;
                                rowIndex++;
                            }

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " (" + item.productParentName + ")";
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.quantity) + " " + item.kategoryName;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalPrice);
                            if (item.productionCalendarList.FirstOrDefault().TypeDescription != null)
                            {
                                sheet.Cells[rowIndex, col2++].Value = item.productionCalendarList.FirstOrDefault().TypeDescription;
                            }

                            otname = item.name;
                            rowIndex++;
                            ri++;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        sheet.Cells[rowIndex + 1, 2].Value = "Toplam qiyməti: " + modelDemandProduction.allPagePrice + " azn";
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
                   ? (ActionResult)PartialView("PartialDemandProductDetailInfoForAccounting", modelDemandProduction)
                   : View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DemandProductsForAccounting(int? page, string statusEV = null, string productName = null, string fullAddress = null, bool excell = false)
        {
            try
            {
                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (fullAddress != null)
                    fullAddress = StripTag.strSqlBlocker(fullAddress.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && fullAddress == null)
                {
                    sproductName = null;
                    sfullAddress = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (fullAddress != null)
                    sfullAddress = fullAddress;
                if (statusEV != null)
                    sstatusEV = statusEV;

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, sstatusEV, out modelDemandProduction.EnumValue);

                BaseOutput gpp = srv.WS_GetDemandProductsForAccounting(baseInput, modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.ProductionDetailArray);

                if (modelDemandProduction.ProductionDetailArray != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.ToList();
                }
                else
                {
                    modelDemandProduction.ProductionDetailList = new List<ProductionDetail>();
                }

                if (sproductName != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (sfullAddress != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.fullAddress.ToLower().Contains(sfullAddress) || x.addressDesc.ToLower().Contains(sfullAddress)).ToList();
                }

                modelDemandProduction.Paging = modelDemandProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                foreach (var item in modelDemandProduction.Paging)
                {
                    modelDemandProduction.currentPagePrice = modelDemandProduction.currentPagePrice + (item.quantity * item.unitPrice);
                }

                foreach (var item in modelDemandProduction.ProductionDetailList)
                {
                    modelDemandProduction.allPagePrice = modelDemandProduction.allPagePrice + (item.quantity * item.unitPrice);
                }

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                {
                    modelDemandProduction.isMain = 0;
                }
                else
                {
                    modelDemandProduction.isMain = 1;
                }


                modelDemandProduction.statusEV = sstatusEV;
                modelDemandProduction.productName = sproductName;
                modelDemandProduction.fullAddress = sfullAddress;



                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb");
                        sheet.Name = "Tələb";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Tələb olunan məhsullar";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 6].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı (vahidi)";
                        sheet.Cells[2, col++].Value = "Qiymət (vahidin)";
                        sheet.Cells[2, col++].Value = "Toplam qiymət";
                        sheet.Cells[2, col++].Value = "Çatdırılma ünvanı";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 20;
                        sheet.Column(3).Width = 10;
                        sheet.Column(4).Width = 10;
                        sheet.Column(5).Width = 10;
                        sheet.Column(6).Width = 25;

                        int rowIndex = 3;
                        var ri = 1;
                        foreach (var item in modelDemandProduction.ProductionDetailList)
                        {
                            var col2 = 1;
                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " " + (item.productParentName);
                            sheet.Cells[rowIndex, col2++].Value = item.quantity.ToString() + " " + item.kategoryName;
                            sheet.Cells[rowIndex, col2++].Value = item.unitPrice.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.totalPrice.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.fullAddress + " " + (item.addressDesc);

                            rowIndex++;
                            ri++;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        sheet.Cells[rowIndex + 1, 2].Value = "Toplam qiyməti: " + modelDemandProduction.allPagePrice + " azn";
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
                   ? (ActionResult)PartialView("PartialDemandProductsForAccounting", modelDemandProduction)
                   : View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DemandOfferProductionTotal(int? page, long addressId = 0, string startDate = null, string endDate = null, bool excell = false)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (startDate != null)
                    startDate = StripTag.strSqlBlocker(startDate.ToLower());
                if (endDate != null)
                    endDate = StripTag.strSqlBlocker(endDate.ToLower());

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();



                if (addressId == 0 && startDate == null && endDate == null)
                {
                    saddressId = 0;
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


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput gpp = srv.WS_GetDemandOfferProductionTotal(baseInput, saddressId, true, out modelDemandProduction.DemandOfferDetailArray);

                if (modelDemandProduction.DemandOfferDetailArray == null)
                {
                    modelDemandProduction.DemandOfferDetailList = new List<DemandOfferDetail>();
                }
                else
                {
                    modelDemandProduction.DemandOfferDetailList = modelDemandProduction.DemandOfferDetailArray.ToList();
                }

                modelDemandProduction.DemandProductionViewModelList = new List<DemandProductionViewModel>();
                long oproductID = 0;
                foreach (var item in modelDemandProduction.DemandOfferDetailList)
                {
                    if (oproductID != item.productID)
                    {
                        modelDemandProduction.DemandProductionVModel = new DemandProductionViewModel();
                        modelDemandProduction.DemandProductionVModel.productName = item.productName;
                        modelDemandProduction.DemandProductionVModel.parentProductName = item.productParentName;
                    }

                    if (item.type == "Demand")
                    {
                        modelDemandProduction.DemandProductionVModel.totalDemand = item.offerDemand;
                    }
                    else if (item.type == "Offer")
                    {
                        modelDemandProduction.DemandProductionVModel.totalOffer = item.offerDemand;
                    }

                    modelDemandProduction.DemandProductionVModel.quantityType = item.kategoryName;

                    if (oproductID != item.productID)
                    {
                        modelDemandProduction.DemandProductionViewModelList.Add(modelDemandProduction.DemandProductionVModel);
                    }
                    else
                    {
                        //modelDemandProduction.DemandProductionViewModelList.Update(modelDemandProduction.DemandProductionVModel);
                    }

                    oproductID = item.productID;
                }


                modelDemandProduction.DemandProductionViewModelList = modelDemandProduction.DemandProductionViewModelList.OrderBy(x => x.parentProductName).ToList();

                modelDemandProduction.itemCount = modelDemandProduction.DemandProductionViewModelList.Count();

                modelDemandProduction.DemandOfferPaging = modelDemandProduction.DemandProductionViewModelList.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.addressId = addressId;
                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb-Təklif");
                        sheet.Name = "Tələb-Təklif";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Ərzaq məhsullarına olan tələbatın və təklifin müqayisəsi";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 6].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı və növü";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Cəmi tələbat";
                        sheet.Cells[2, col++].Value = "Cəmi təklif";
                        sheet.Cells[2, col++].Value = "Tələb və təklifin fərqi";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 10;
                        sheet.Column(4).Width = 10;
                        sheet.Column(5).Width = 10;
                        sheet.Column(6).Width = 10;

                        int rowIndex = 3;
                        var ri = 1;
                        string ppname = "";
                        string oppname = "";
                        decimal dquantity = 0;
                        string dsquantity = "";

                        foreach (var item in modelDemandProduction.DemandProductionViewModelList)
                        {
                            var col2 = 1;
                            ppname = item.parentProductName;
                            dquantity = Custom.ConverPriceDelZero((decimal)(item.totalOffer - item.totalDemand));
                            if (dquantity > 0)
                            {
                                dsquantity = "+" + Custom.ConverPriceDelZero((decimal)(dquantity));
                            }
                            else
                            {
                                dsquantity = Custom.ConverPriceDelZero((decimal)(dquantity)).ToString() ;
                            }

                            if (ppname != oppname)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 2].Merge = true;

                                sheet.Cells[rowIndex, 1].Value = ppname;

                                sheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 20;
                                rowIndex++;
                            }

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName;
                            sheet.Cells[rowIndex, col2++].Value = item.quantityType;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalDemand);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalOffer);
                            sheet.Cells[rowIndex, col2++].Value = dsquantity;

                            rowIndex++;
                            ri++;

                            oppname = ppname;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialDemandOfferProductionTotal", modelDemandProduction)
                   : View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DemandOfferProductionTotaln(int? page, long addressId = 0, string startDate = null, string endDate = null, bool excell = false)
        {
            try
            {

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (startDate != null)
                    startDate = StripTag.strSqlBlocker(startDate.ToLower());
                if (endDate != null)
                    endDate = StripTag.strSqlBlocker(endDate.ToLower());


                if (addressId == 0 && startDate == null && endDate == null)
                {
                    saddressId = 0;
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
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput gpp = srv.WS_GetDemandOfferProductionTotal(baseInput, saddressId, true, out modelDemandProduction.DemandOfferDetailArray);

                if (modelDemandProduction.DemandOfferDetailArray == null)
                {
                    modelDemandProduction.DemandOfferDetailList = new List<DemandOfferDetail>();
                }
                else
                {
                    modelDemandProduction.DemandOfferDetailList = modelDemandProduction.DemandOfferDetailArray.OrderBy(x => x.productParentName).ToList();
                }

                modelDemandProduction.DemandProductionViewModelList = new List<DemandProductionViewModel>();
                long oproductID = 0;
                foreach (var item in modelDemandProduction.DemandOfferDetailList)
                {
                    if (oproductID != item.productID)
                    {
                        modelDemandProduction.DemandProductionVModel = new DemandProductionViewModel();
                        modelDemandProduction.DemandProductionVModel.productName = item.productName;
                        modelDemandProduction.DemandProductionVModel.productParentName = item.productParentName;
                    }

                    if (item.type == "Demand")
                    {
                        modelDemandProduction.DemandProductionVModel.totalDemand = item.offerDemand;
                        modelDemandProduction.DemandProductionVModel.unitPrice = item.unitPrice;
                        modelDemandProduction.DemandProductionVModel.totalPrice = item.offerDemand * item.unitPrice;
                    }
                    else if (item.type == "Offer")
                    {
                        modelDemandProduction.DemandProductionVModel.totalOffer = item.offerDemand;
                    }

                    modelDemandProduction.DemandProductionVModel.quantityType = item.kategoryName;

                    if (oproductID != item.productID)
                    {
                        modelDemandProduction.DemandProductionViewModelList.Add(modelDemandProduction.DemandProductionVModel);
                    }
                    else
                    {
                        //modelDemandProduction.DemandProductionViewModelList.Update(modelDemandProduction.DemandProductionVModel);
                    }

                    oproductID = item.productID;
                }


                modelDemandProduction.itemCount = modelDemandProduction.DemandProductionViewModelList.Count();
                modelDemandProduction.DemandOfferPaging = modelDemandProduction.DemandProductionViewModelList.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.addressId = addressId;
                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb-Təklif");
                        sheet.Name = "Tələb-Təklif";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Ərzaq məhsullarının illik tələbatı və təklifi üzrə ümumi cədvəl";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 9].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı və növü";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Cəmi tələbat";
                        sheet.Cells[2, col++].Value = "Qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Cəmi qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Cəmi təklif";
                        sheet.Cells[2, col++].Value = "Tələb və təklifin fərqi";
                        sheet.Cells[2, col++].Value = "Qeyd";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 10;
                        sheet.Column(4).Width = 10;
                        sheet.Column(5).Width = 10;
                        sheet.Column(6).Width = 10;
                        sheet.Column(7).Width = 10;
                        sheet.Column(8).Width = 10;
                        sheet.Column(9).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;
                        string oppName = "";

                        foreach (var item in modelDemandProduction.DemandProductionViewModelList)
                        {
                            var col2 = 1;
                            if (oppName != item.productParentName)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 2].Merge = true;
                                sheet.Cells[rowIndex, 1].Value = item.productParentName;

                                sheet.Cells[rowIndex, 1, rowIndex, 9].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 9].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 20;

                                rowIndex++;
                            }

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName;
                            sheet.Cells[rowIndex, col2++].Value = item.quantityType;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalDemand);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalPrice);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalOffer);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)(item.totalOffer - item.totalDemand));
                            sheet.Cells[rowIndex, col2++].Value = "";

                            rowIndex++;
                            ri++;
                            oppName = item.productParentName;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 9].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 9].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 9].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 9].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialDemandOfferProductionTotaln", modelDemandProduction)
                   : View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DemandProductionAmountOfEachProduct(int? page, string productName = null, bool excell = false, string startDate = null, string endDate = null)
        {
            try
            {
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (startDate != null)
                    startDate = StripTag.strSqlBlocker(startDate.ToLower());
                if (endDate != null)
                    endDate = StripTag.strSqlBlocker(endDate.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && startDate == null && endDate == null)
                {
                    sproductName = null;
                    sstartDate = null;
                    sendDate = null;
                }

                if (productName != null)
                    sproductName = productName;

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    sstartDate = startDate;
                    sendDate = endDate;

                    //sstartDate = (Convert.ToDateTime(startDate)).getInt64ShortDate();
                    //sendDate = (Convert.ToDateTime(endDate)).getInt64ShortDate();
                }

                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                if (sstartDate != null && sendDate != null)
                {

                }

                BaseOutput gpp = srv.WS_GetDemandProductionAmountOfEachProduct(baseInput, out modelDemandProduction.DemandOfferDetailArray);

                if (modelDemandProduction.DemandOfferDetailArray == null)
                {
                    modelDemandProduction.DemandOfferDetailList = new List<DemandOfferDetail>();
                }
                else
                {
                    modelDemandProduction.DemandOfferDetailList = modelDemandProduction.DemandOfferDetailArray.Where(x => x.enumKategoryID == modelDemandProduction.EnumCategory.Id).OrderBy(x => x.productParentName).ToList();
                }

                if (sproductName != null)
                {
                    modelDemandProduction.DemandOfferDetailList = modelDemandProduction.DemandOfferDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                modelDemandProduction.DemandOfferDetailPaging = modelDemandProduction.DemandOfferDetailList.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.itemCount = modelDemandProduction.DemandOfferDetailList.Count();
                modelDemandProduction.productName = sproductName;
                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;

                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Tələb-Təklif");
                        sheet.Name = "Tələb-Təklif";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Ərzaq məhsullarının illik tələbat və qiymətlər üzrə məlumat";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 6].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı və növü";
                        sheet.Cells[2, col++].Value = "Tələbatın həcmi (miqdar)";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Qeyd";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 10;
                        sheet.Column(4).Width = 10;
                        sheet.Column(5).Width = 10;
                        sheet.Column(6).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;
                        string oppName = "";

                        foreach (var item in modelDemandProduction.DemandOfferDetailList)
                        {
                            var col2 = 1;
                            if (oppName != item.productParentName)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 2].Merge = true;
                                sheet.Cells[rowIndex, 1].Value = item.productParentName;

                                sheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 6].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                rowIndex++;
                            }

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.quantity);
                            sheet.Cells[rowIndex, col2++].Value = item.kategoryName;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                            sheet.Cells[rowIndex, col2++].Value = "";

                            rowIndex++;
                            ri++;
                            oppName = item.productParentName;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialDemandProductionAmountOfEachProduct", modelDemandProduction)
                   : View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DemandOfferGroupRegion(int? page, long addressId = -1, long productId = -1, bool excell = false, string startDate = null, string endDate = null, long userType = -1)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (addressId == -1 && userType == -1 && productId == -1 && startDate == null && endDate == null)
                {
                    saddressId = 0;
                    sproductId = 0;
                    sstartDate = null;
                    sendDate = null;
                    suserType = 0;
                }

                if (addressId >= 0)
                    saddressId = addressId;
                if (productId >= 0)
                    sproductId = productId;
                if (userType >= 0)
                    suserType = userType;

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
                modelDemandProduction = new DemandProductionViewModel();


                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                BaseOutput enumcatid = srv.WS_GetEnumCategorysByName(baseInput, "olcuVahidi", out modelDemandProduction.EnumCategory);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValue);

                BaseOutput gpp = srv.WS_GetOfferGroupedProductionDetailistForAccountingByRoleId(baseInput, suserType, true, out modelDemandProduction.OfferProductionDetailArray);

                if (modelDemandProduction.OfferProductionDetailArray == null)
                {
                    modelDemandProduction.OfferProductionDetailList = new List<OfferProductionDetail>();
                }
                else
                {
                    modelDemandProduction.OfferProductionDetailList = modelDemandProduction.OfferProductionDetailArray.OrderBy(x => x.adminName).ToList();
                }

                if (sproductId > 0)
                {
                    modelDemandProduction.OfferProductionDetailList = modelDemandProduction.OfferProductionDetailList.Where(x => x.productID == sproductId).ToList();
                }

                if (saddressId > 0)
                {
                    modelDemandProduction.OfferProductionDetailList = modelDemandProduction.OfferProductionDetailList.Where(x => x.adminID == saddressId).ToList();
                }

                modelDemandProduction.itemCount = modelDemandProduction.OfferProductionDetailList.Count();
                modelDemandProduction.DemandOfferGroupRegionPaging = modelDemandProduction.OfferProductionDetailList.ToList().ToPagedList(pageNumber, pageSize);

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                    modelDemandProduction.isMain = 0;
                else
                    modelDemandProduction.isMain = 1;

                modelDemandProduction.addressId = saddressId;
                modelDemandProduction.productId = sproductId;
                modelDemandProduction.startDate = sstartDate;
                modelDemandProduction.endDate = sendDate;

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
                        sheet.Cells[1, col++].Value = "Tələb və təklifin müqayisəsi (İstehsalçılara görə)";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 4].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 8;
                        sheet.Column(2).Width = 40;
                        sheet.Column(3).Width = 15;
                        sheet.Column(4).Width = 15;

                        int rowIndex = 3;
                        var ri = 1;
                        string m = "";
                        string om = "";
                        foreach (var item in modelDemandProduction.OfferProductionDetailList)
                        {
                            var col2 = 1;

                            m = item.adminName;

                            if (m != om)
                            {
                                sheet.Cells[rowIndex, 1, rowIndex, 4].Merge = true;
                                sheet.Cells[rowIndex, 1].Value = m;

                                sheet.Cells[rowIndex, 1, rowIndex, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                sheet.Cells[rowIndex, 1, rowIndex, 4].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                                sheet.Row(rowIndex).Height = 20;
                                sheet.Row(rowIndex).Style.Font.Bold = true;
                                rowIndex++;
                            }

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " (" + item.productParentName + ")";
                            sheet.Cells[rowIndex, col2++].Value = item.totalQuantity.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.quantityType;

                            rowIndex++;
                            ri++;

                            om = m;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialDemandOfferGroupRegion", modelDemandProduction)
                   : View(modelDemandProduction);

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
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                if (ids != null)
                {
                    for (int i = 0; i < ids.Length; i++)
                    {
                        modelDemandProduction.DemandProduction = new tblDemand_Production();

                        BaseOutput bouput = srv.WS_GetDemand_ProductionById(baseInput, ids[i], true, out modelDemandProduction.DemandProduction);

                        BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "Tesdiqlenen", out modelDemandProduction.EnumValueST);

                        modelDemandProduction.DemandProduction.state_eV_Id = modelDemandProduction.EnumValueST.Id;
                        modelDemandProduction.DemandProduction.state_eV_IdSpecified = true;

                        modelDemandProduction.DemandProduction.isNew = 1;
                        modelDemandProduction.DemandProduction.isNewSpecified = true;

                        BaseOutput ecout = srv.WS_UpdateDemand_Production(baseInput, modelDemandProduction.DemandProduction, out modelDemandProduction.DemandProduction);

                        modelDemandProduction.ComMessage = new tblComMessage();
                        modelDemandProduction.ComMessage.message = "Təsdiqləndi";
                        modelDemandProduction.ComMessage.fromUserID = (long)UserId;
                        modelDemandProduction.ComMessage.fromUserIDSpecified = true;
                        modelDemandProduction.ComMessage.toUserID = modelDemandProduction.DemandProduction.user_Id;
                        modelDemandProduction.ComMessage.toUserIDSpecified = true;
                        modelDemandProduction.ComMessage.Production_Id = modelDemandProduction.DemandProduction.Id;
                        modelDemandProduction.ComMessage.Production_IdSpecified = true;
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "demand", out modelDemandProduction.EnumValue);
                        modelDemandProduction.ComMessage.Production_type_eV_Id = modelDemandProduction.EnumValue.Id;
                        modelDemandProduction.ComMessage.Production_type_eV_IdSpecified = true;

                        BaseOutput acm = srv.WS_AddComMessage(baseInput, modelDemandProduction.ComMessage, out modelDemandProduction.ComMessage);
                    }
                }

                return RedirectToAction("Index", "DemandProduction", new { statusEV = modelDemandProduction.EnumValueST.name });

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
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                BaseOutput bouput = srv.WS_GetDemand_ProductionById(baseInput, id, true, out modelDemandProduction.DemandProduction);
                modelDemandProduction.Id = modelDemandProduction.DemandProduction.Id;

                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(DemandProductionViewModel model, FormCollection collection)
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
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out model.Admin);
                baseInput.userName = model.Admin.Username;

                model.ConfirmationMessage = new tblConfirmationMessage();
                model.ConfirmationMessage.Message = model.message;

                BaseOutput pout = srv.WS_SendConfirmationMessageNew(baseInput, model.ConfirmationMessage, out model.ConfirmationMessage);


                model.DemandProduction = new tblDemand_Production();

                BaseOutput bouput = srv.WS_GetDemand_ProductionById(baseInput, model.Id, true, out model.DemandProduction);

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "reject", out model.EnumValueST);

                model.DemandProduction.state_eV_Id = model.EnumValueST.Id;
                model.DemandProduction.state_eV_IdSpecified = true;

                model.DemandProduction.isNew = 1;
                model.DemandProduction.isNewSpecified = true;

                BaseOutput ecout = srv.WS_UpdateDemand_Production(baseInput, model.DemandProduction, out model.DemandProduction);

                model.ComMessage = new tblComMessage();
                model.ComMessage.message = model.message;
                model.ComMessage.fromUserID = (long)UserId;
                model.ComMessage.fromUserIDSpecified = true;
                model.ComMessage.toUserID = model.DemandProduction.user_Id;
                model.ComMessage.toUserIDSpecified = true;
                model.ComMessage.Production_Id = model.DemandProduction.Id;
                model.ComMessage.Production_IdSpecified = true;
                BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "demand", out model.EnumValue);
                model.ComMessage.Production_type_eV_Id = model.EnumValue.Id;
                model.ComMessage.Production_type_eV_IdSpecified = true;

                BaseOutput acm = srv.WS_AddComMessage(baseInput, model.ComMessage, out model.ComMessage);

                return RedirectToAction("Index", "DemandProduction", new { statusEV = model.EnumValueST.name });

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
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelDemandProduction.ProductCatalogDetailArray);

                if (modelDemandProduction.ProductCatalogDetailArray == null)
                {
                    modelDemandProduction.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelDemandProduction.ProductCatalogDetailList = modelDemandProduction.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelDemandProduction.actionName = actionName;
                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult AdminUnit(string actionName, long economicZoneId = 0)
        {
            try
            {
                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                if (economicZoneId == 0)
                {
                    BaseOutput bouput = srv.WS_GetAdminUnitsByParentId(baseInput, 1, true, out modelDemandProduction.PRMAdminUnitArray);
                }
                else
                {
                    BaseOutput bouput = srv.WS_GetPRM_AdminUnitRegionByAddressId(baseInput, (int)economicZoneId, true, out modelDemandProduction.PRMAdminUnitArray);
                }


                if (modelDemandProduction.PRMAdminUnitArray == null)
                {
                    modelDemandProduction.PRMAdminUnitList = new List<tblPRM_AdminUnit>();
                }
                else
                {
                    modelDemandProduction.PRMAdminUnitList = modelDemandProduction.PRMAdminUnitArray.ToList();
                }
                modelDemandProduction.actionName = actionName;
                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult EconomicZone(string actionName)
        {
            try
            {
                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;
                
                BaseOutput bouput = srv.WS_GetPRM_AdminUnitRegionList(baseInput, out modelDemandProduction.AdminUnitRegionArray);

                if (modelDemandProduction.AdminUnitRegionArray == null)
                {
                    modelDemandProduction.AdminUnitRegionList = new List<AdminUnitRegion>();
                }
                else
                {
                    modelDemandProduction.AdminUnitRegionList = modelDemandProduction.AdminUnitRegionArray.ToList();
                }
                modelDemandProduction.actionName = actionName;
                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult Organisation(string actionName, string adminUnitId = null)
        {
            try
            {
                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;


                if (!string.IsNullOrEmpty(saddressIdString))
                {
                    modelDemandProduction.addressIdList = adminUnitId.Split(',').Select(long.Parse).ToArray();
                }


                BaseOutput bouput = srv.WS_GetGovermentOrganisatinByAdminID(baseInput, 1, true, out modelDemandProduction.ForeignOrganizationArray);


                if (modelDemandProduction.ForeignOrganizationArray == null)
                {
                    modelDemandProduction.ForeignOrganizationList = new List<ForeignOrganization>();
                }
                else
                {
                    modelDemandProduction.ForeignOrganizationList = modelDemandProduction.ForeignOrganizationArray.ToList();
                }
                modelDemandProduction.actionName = actionName;
                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult Year(string actionName)
        {
            try
            {
                baseInput = new BaseInput();
                modelDemandProduction = new DemandProductionViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelDemandProduction.Admin);
                baseInput.userName = modelDemandProduction.Admin.Username;



                BaseOutput ecy = srv.WS_GetEnumCategorysByName(baseInput, "year", out modelDemandProduction.EnumCategoryYear);

                BaseOutput evyf = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelDemandProduction.EnumCategoryYear.Id, true, out modelDemandProduction.EnumValueArrayYear);
                modelDemandProduction.EnumValueListYear = modelDemandProduction.EnumValueArrayYear.ToList();


                modelDemandProduction.actionName = actionName;

                return View(modelDemandProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
