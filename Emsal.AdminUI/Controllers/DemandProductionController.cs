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

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class DemandProductionController : Controller
    {
        private BaseInput baseInput;

        private static string sproductName;
        private static string sfullAddress;
        private static string suserInfo;
        private static string sadminUnit;
        private static string sstatusEV;


        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private DemandProductionViewModel modelDemandProduction;

        public ActionResult Index(int? page, string statusEV = null, string productName = null, string userInfo = null, string adminUnit = null)
        {
            try
            {

                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());
                if (adminUnit != null)
                    adminUnit = StripTag.strSqlBlocker(adminUnit.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && userInfo == null && adminUnit == null)
                {
                    sproductName = null;
                    suserInfo = null;
                    sadminUnit = null;
                }

                if (productName != null)
                    sproductName = productName;
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

                BaseOutput gpp = srv.WS_GetDemandProductionDetailistForEValueId(baseInput, modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.ProductionDetailArray);

                modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelDemandProduction.EnumCategory.Id && x.foreignOrganization != null).ToList();

                if (sproductName != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (sadminUnit != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.foreignOrganization.name.ToLower().Contains(sadminUnit)).ToList();
                }

                if (suserInfo != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                }


                modelDemandProduction.Paging = modelDemandProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

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

        public ActionResult Indexwd(int? page, string statusEV = null, string productName = null, string fullAddress = null, string userInfo = null, string adminUnit = null, bool excell = false)
        {
            try
            {

                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (fullAddress != null)
                    fullAddress = StripTag.strSqlBlocker(fullAddress.ToLower());
                if (userInfo != null)
                    userInfo = StripTag.strSqlBlocker(userInfo.ToLower());
                if (adminUnit != null)
                    adminUnit = StripTag.strSqlBlocker(adminUnit.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && fullAddress == null && userInfo == null && adminUnit == null)
                {
                    sproductName = null;
                    sfullAddress = null;
                    suserInfo = null;
                    sadminUnit = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (fullAddress != null)
                    sfullAddress = fullAddress;
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

                BaseOutput gpp = srv.WS_GetDemandProductionDetailistForEValueId(baseInput, modelDemandProduction.EnumValue.Id, true, out modelDemandProduction.ProductionDetailArray);

                modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelDemandProduction.EnumCategory.Id && x.foreignOrganization != null).ToList();

                if (sproductName != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (sfullAddress != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.fullAddress.ToLower().Contains(sfullAddress) || x.addressDesc.ToLower().Contains(sfullAddress)).ToList();
                }
                if (sadminUnit != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.foreignOrganization.name.ToLower().Contains(sadminUnit)).ToList();
                }

                if (suserInfo != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                }


                modelDemandProduction.Paging = modelDemandProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                foreach (var item in modelDemandProduction.Paging)
                {
                    modelDemandProduction.currentPagePrice = modelDemandProduction.currentPagePrice + (item.quantity * item.productUnitPrice);
                }

                foreach (var item in modelDemandProduction.ProductionDetailList)
                {
                    modelDemandProduction.allPagePrice = modelDemandProduction.allPagePrice + (item.quantity * item.productUnitPrice);
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
                modelDemandProduction.userInfo = suserInfo;
                modelDemandProduction.adminUnit = sadminUnit;
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
                        sheet.Cells[1, col++].Value = "Tələb olunan məhsullar";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 7].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı (vahidi)";
                        sheet.Cells[2, col++].Value = "Qiyməti (vahidin)";
                        sheet.Cells[2, col++].Value = "Dövrülük";
                        sheet.Cells[2, col++].Value = "Təşkilat";
                        sheet.Cells[2, col++].Value = "Çatdırılma ünvanı";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 9;
                        sheet.Column(4).Width = 15;
                        sheet.Column(5).Width = 9;
                        sheet.Column(6).Width = 30;
                        sheet.Column(7).Width = 30;

                        int rowIndex = 3;
                        var ri = 1;
                        foreach (var item in modelDemandProduction.ProductionDetailList)
                        {
                            var col2 = 1;
                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " " + (item.productParentName);
                            sheet.Cells[rowIndex, col2++].Value = item.quantity.ToString() + " " + item.enumValueName;
                            sheet.Cells[rowIndex, col2++].Value = item.productUnitPrice.ToString();
                            if (item.productionCalendarList.FirstOrDefault().TypeDescription != null)
                            {
                                sheet.Cells[rowIndex, col2++].Value = item.productionCalendarList.FirstOrDefault().TypeDescription;
                            }
                            if (item.foreignOrganization != null)
                            {
                                sheet.Cells[rowIndex, col2++].Value = item.foreignOrganization.name;
                            }

                            sheet.Cells[rowIndex, col2++].Value = item.fullAddress + " " + (item.addressDesc);

                            sheet.Cells[rowIndex, 1, rowIndex, col2 - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            sheet.Cells[rowIndex, 1, rowIndex, col2 - 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                            var col3 = 1;
                            rowIndex++;
                            sheet.Cells[rowIndex, col3++].Value = "tipi";
                            sheet.Cells[rowIndex, col3++].Value = "il";
                            sheet.Cells[rowIndex, col3++].Value = "rüb";
                            sheet.Cells[rowIndex, col3++].Value = "gün, ay";
                            sheet.Cells[rowIndex, col3++].Value = "saat";
                            sheet.Cells[rowIndex, col3++].Value = "miqdar";
                            sheet.Cells[rowIndex, col3++].Value = "miqdar (cəmi)";

                            sheet.Row(rowIndex).Style.Font.Bold = true;
                            sheet.Row(rowIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            decimal tquantity;
                            string day = "-";
                            foreach (var item2 in item.productionCalendarList)
                            {
                                if (item2.day!=0)
                                {
                                    day = item2.day.ToString();
                                }
                                tquantity = (item2.quantity * item2.transportation_eV_Id);
                                var col4 = 1;
                                rowIndex++;
                                sheet.Cells[rowIndex, col4++].Value = item2.TypeDescription;
                                sheet.Cells[rowIndex, col4++].Value = item2.year;
                                sheet.Cells[rowIndex, col4++].Value = item2.partOfyear;
                                sheet.Cells[rowIndex, col4++].Value = day + " " + item2.MonthDescription;
                                sheet.Cells[rowIndex, col4++].Value = item2.oclock + ":00";
                                sheet.Cells[rowIndex, col4++].Value = item2.quantity;
                                sheet.Cells[rowIndex, col4++].Value = tquantity;
                            }
                            rowIndex++;
                            ri++;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 7].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 7].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 7].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 7].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


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

        public ActionResult DemandProductDetailInfoForAccounting(int? page, string statusEV = null, string productName = null, string adminUnit = null, string fullAddress = null, bool excell = false)
        {
            try
            {
                if (statusEV != null)
                    statusEV = StripTag.strSqlBlocker(statusEV.ToLower());
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());
                if (adminUnit != null)
                    adminUnit = StripTag.strSqlBlocker(adminUnit.ToLower());
                if (fullAddress != null)
                    fullAddress = StripTag.strSqlBlocker(fullAddress.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && adminUnit == null && fullAddress == null)
                {
                    sproductName = null;
                    sadminUnit = null;
                    sfullAddress = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (adminUnit != null)
                    sadminUnit = adminUnit;
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

                DateTime date = DateTime.Now;
                long year = date.Year;
                long rub = Int32.Parse(String.Format("{0}", (date.Month + 2) / 3));

                BaseOutput gpp = srv.WS_GetDemandProductDetailInfoForAccounting(baseInput, modelDemandProduction.EnumValue.Id, true, year, true, rub, true, out modelDemandProduction.ProductionDetailArray);

                modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailArray.ToList();

                if (sproductName != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (sadminUnit != null)
                {
                    modelDemandProduction.ProductionDetailList = modelDemandProduction.ProductionDetailList.Where(x => x.name.ToLower().Contains(sadminUnit)).ToList();
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
                modelDemandProduction.adminUnit = sadminUnit;
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
                        sheet.Cells[1, 1, 1, 8].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Təşkilatın adı";
                        sheet.Cells[2, col++].Value = "Məhsulun adı";
                        sheet.Cells[2, col++].Value = "Miqdarı (vahidi)";
                        sheet.Cells[2, col++].Value = "Qiymət (vahidin)";
                        sheet.Cells[2, col++].Value = "Toplam qiymət";
                        sheet.Cells[2, col++].Value = "Dövrülük";
                        sheet.Cells[2, col++].Value = "Çatdırılma ünvanı";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 6;
                        sheet.Column(2).Width = 20;
                        sheet.Column(2).Width = 20;
                        sheet.Column(3).Width = 10;
                        sheet.Column(4).Width = 10;
                        sheet.Column(5).Width = 10;
                        sheet.Column(6).Width = 10;
                        sheet.Column(7).Width = 10;
                        sheet.Column(8).Width = 25;

                        int rowIndex = 3;
                        var ri = 1;
                        foreach (var item in modelDemandProduction.ProductionDetailList)
                        {
                            var col2 = 1;
                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.name;
                            sheet.Cells[rowIndex, col2++].Value = item.productName + " " + (item.productParentName);
                            sheet.Cells[rowIndex, col2++].Value = item.quantity.ToString() + " " + item.kategoryName;
                            sheet.Cells[rowIndex, col2++].Value = item.unitPrice.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.totalPrice.ToString();
                            if (item.productionCalendarList.FirstOrDefault().TypeDescription != null)
                            {
                                sheet.Cells[rowIndex, col2++].Value = item.productionCalendarList.FirstOrDefault().TypeDescription;
                            }

                            sheet.Cells[rowIndex, col2++].Value = item.fullAddress + " " + (item.addressDesc);

                            rowIndex++;
                            ri++;
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 8].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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

        public ActionResult DemandOfferProductionTotal(int? page, string productName = null, bool excell = false)
        {
            try
            {
                if (productName != null)
                    productName = StripTag.strSqlBlocker(productName.ToLower());

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null)
                {
                    sproductName = null;
                }

                if (productName != null)
                    sproductName = productName;

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

                BaseOutput gpp = srv.WS_GetDemandOfferProductionTotal(baseInput, out modelDemandProduction.DemandOfferDetailArray);

                modelDemandProduction.DemandOfferDetailList = modelDemandProduction.DemandOfferDetailArray.ToList();

                modelDemandProduction.DemandProductionViewModelList = new List<DemandProductionViewModel>();
                long oproductID = 0;
                foreach (var item in modelDemandProduction.DemandOfferDetailList)
                {
                    if (oproductID != item.productID)
                    {
                        modelDemandProduction.DemandProductionVModel = new DemandProductionViewModel();
                        modelDemandProduction.DemandProductionVModel.productName = item.productName + " (" + item.productParentName + ")";
                    }

                    if(item.type=="Demand")
                    {
                        modelDemandProduction.DemandProductionVModel.totalDemand = item.offerDemand;
                    }
                    else if (item.type == "Offer")
                    {
                        modelDemandProduction.DemandProductionVModel.totalOffer= item.offerDemand;
                    }

                    modelDemandProduction.DemandProductionVModel.quantityType = item.kategoryName;

                    if (oproductID!= item.productID)
                    {
                        modelDemandProduction.DemandProductionViewModelList.Add(modelDemandProduction.DemandProductionVModel);
                    }
                    else
                    {
                        //modelDemandProduction.DemandProductionViewModelList.Update(modelDemandProduction.DemandProductionVModel);
                    }

                    oproductID = item.productID;
                }

                if (sproductName != null)
                {
                    modelDemandProduction.DemandProductionViewModelList = modelDemandProduction.DemandProductionViewModelList.Where(x => x.productName.ToLower().Contains(sproductName)).ToList();
                }

                modelDemandProduction.DemandOfferPaging = modelDemandProduction.DemandProductionViewModelList.ToPagedList(pageNumber, pageSize);

                modelDemandProduction.productName = sproductName;

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
                        foreach (var item in modelDemandProduction.DemandProductionViewModelList)
                        {
                            var col2 = 1;
                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = item.productName ;
                            sheet.Cells[rowIndex, col2++].Value = item.quantityType;
                            sheet.Cells[rowIndex, col2++].Value = item.totalDemand;
                            sheet.Cells[rowIndex, col2++].Value = item.totalOffer;
                            sheet.Cells[rowIndex, col2++].Value = (item.totalDemand - item.totalOffer).ToString();

                            rowIndex++;
                            ri++;
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
    }
}
