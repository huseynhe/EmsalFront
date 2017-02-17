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
using System.IO;
using WordDoc.Models;
using Microsoft.Win32;
using WebBarCodec.Core;
using System.Net.Mail;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.paAdmin)]

    public class PAController : Controller
    {
        private BaseInput baseInput;

        private static long sproductId;
        private static long suserTypeId;
        private static long smonthEVId;
        private static string sfin;
        private static string svoen;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private OfferMonitoringViewModel modelOfferMonitoring;


        private static long[] sproductIdArray = {
            334, //1-ci növ buğda unundan bişirilmiş ağ çörək
            335, //2-ci növ buğda unundan bişirilmiş ağ çörək
            343, //1-ci növ buğda unu
            344, //2-ci növ buğda unu
            346, //Doğranmış zülalsız buğda çörəyi
            347, //Çovdar çörəyi
            363 //Toyuq əti
        };

        public ActionResult TotalDemandOffersGroup(int? page, long productId = -1, long userTypeId = -1, long monthEVId = -1, string fin = null, string voen = null, bool excell = false)
        {
            try
            {
                int pageSize = 36;
                int pageNumber = (page ?? 1);

                if (fin != null)
                    fin = StripTag.strSqlBlocker(fin.ToLower());
                if (voen != null)
                    voen = StripTag.strSqlBlocker(voen.ToLower());

                if (productId == -1 && userTypeId == -1 && fin == null && voen == null)
                {
                    sproductId = 0;
                    suserTypeId = 0;
                    smonthEVId = 0;
                    sfin = null;
                    svoen = null;
                }

                if (productId >= 0)
                    sproductId = productId;
                if (userTypeId >= 0)
                    suserTypeId = userTypeId;
                if (monthEVId >= 0)
                    smonthEVId = monthEVId;
                if (fin != null)
                    sfin = fin;
                if (voen != null)
                    svoen = voen;

                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                if (excell == true)
                {
                    pageNumber = 1;
                    pageSize = 1000000;
                }

                modelOfferMonitoring.DemandOfferProductsSearch = new DemandOfferProductsSearch();

                modelOfferMonitoring.DemandOfferProductsSearch.page = pageNumber;
                modelOfferMonitoring.DemandOfferProductsSearch.page_size = pageSize;
                modelOfferMonitoring.DemandOfferProductsSearch.productId = sproductId;
                modelOfferMonitoring.DemandOfferProductsSearch.monthID = smonthEVId;
                modelOfferMonitoring.DemandOfferProductsSearch.roleID = suserTypeId;
                modelOfferMonitoring.DemandOfferProductsSearch.pinNumber = sfin;
                modelOfferMonitoring.DemandOfferProductsSearch.voen = svoen;

                if (excell == false)
                {
                    BaseOutput gpp = srv.WS_GetTotalDemandOffersPA(baseInput, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.DemanProductionGroupArray);
                }
                else if (excell == true)
                {
                    BaseOutput gpp = srv.WS_GetTotalDemandOffers(baseInput, pageNumber, true, pageSize, true, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.DemanProductionGroupArray);
                }

                if (modelOfferMonitoring.DemanProductionGroupArray == null)
                {
                    modelOfferMonitoring.DemanProductionGroupList = new List<DemanProductionGroup>();
                }
                else
                {
                    modelOfferMonitoring.DemanProductionGroupList = modelOfferMonitoring.DemanProductionGroupArray.OrderBy(x => x.productParentName).ThenBy(x => x.productName).ToList();
                }

                BaseOutput gdpc = srv.WS_GetTotalDemandOffers_OPC(baseInput, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.itemCount, out modelOfferMonitoring.itemCountB);

                long[] aic = new long[modelOfferMonitoring.itemCount];

                modelOfferMonitoring.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelOfferMonitoring.productId = sproductId;
                modelOfferMonitoring.userTypeId = suserTypeId;
                modelOfferMonitoring.monthEVId = smonthEVId;
                modelOfferMonitoring.fin = sfin;
                modelOfferMonitoring.voen = svoen;


                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Təklif");
                        sheet.Name = "Təklif";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Məhsul üzrə tələb və təkliflər";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 21].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının növü";
                        sheet.Cells[2, col++].Value = "Tələbatın həcmi (miqdar)";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Cəmi (AZN)";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının adı";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının VÖEN-i";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçı ilə alqı-satqı müqaviləsinin tarixi və nömrəsi";
                        sheet.Cells[2, col++].Value = "Malın təklif edilən qiyməti (manatla)";
                        sheet.Cells[2, col++].Value = "Müqavilə üzrə malın qiyməti";
                        sheet.Cells[2, col++].Value = "Təchizat qiyməti (manatla)";
                        sheet.Cells[2, col++].Value = "Ticarət əlavəsi (manatla)";
                        sheet.Cells[2, col++].Value = "Ticarət əlavəsi (faizlə)";
                        sheet.Cells[2, col++].Value = "Təklifin həcmi (miqdarı)";
                        sheet.Cells[2, col++].Value = "Müqavilə üzrə malın həcmi (miqdarı)";
                        sheet.Cells[2, col++].Value = "Təklifin ümumi dəyəri (manatla)";
                        sheet.Cells[2, col++].Value = "Müqavilənin ümumi dəyəri (manatla)";
                        sheet.Cells[2, col++].Value = "Tədarükçünün növü (istehsalçı satıcı və ya idxalçı)";
                        sheet.Cells[2, col++].Value = "Tədarükçünün hansı növ vergi ödəyicisi olması (ƏDV, sadələşdirilmiş K/T məhsulu istehsalçısı)";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[2, 1, 2, 21].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        sheet.Cells[2, 1, 2, 21].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 7;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 30;
                        sheet.Column(4).Width = 20;
                        sheet.Column(5).Width = 20;
                        sheet.Column(6).Width = 20;
                        sheet.Column(7).Width = 20;
                        sheet.Column(8).Width = 50;
                        sheet.Column(9).Width = 20;
                        sheet.Column(10).Width = 20;
                        sheet.Column(11).Width = 20;
                        sheet.Column(12).Width = 20;
                        sheet.Column(13).Width = 20;
                        sheet.Column(14).Width = 20;
                        sheet.Column(15).Width = 20;
                        sheet.Column(16).Width = 20;
                        sheet.Column(17).Width = 20;
                        sheet.Column(18).Width = 20;
                        sheet.Column(19).Width = 20;
                        sheet.Column(20).Width = 20;
                        sheet.Column(21).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;
                        string orgName = "";
                        string contractNumber = "";
                        string faiz = "";
                        string manat = "";

                        foreach (var item in modelOfferMonitoring.DemanProductionGroupList)
                        {
                            var col2 = 0;
                            if (item.offerProductsList.Count() == 0)
                            {
                                col2 = 1;
                                sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                                sheet.Cells[rowIndex, col2++].Value = item.productParentName; ;
                                sheet.Cells[rowIndex, col2++].Value = item.productName;
                                sheet.Cells[rowIndex, col2++].Value = @Custom.ConverPriceDelZero((decimal)item.totalQuantity);

                                sheet.Cells[rowIndex, col2++].Value = item.enumValueName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalQuantityPrice);

                                sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                rowIndex++;
                                ri++;
                            }
                            foreach (var itemo in item.offerProductsList)
                            {
                                col2 = 1;
                                contractNumber = "";
                                orgName = "";
                                manat = "";
                                faiz = "";
                                modelOfferMonitoring.ContractDetailTemp = new tblContractDetailTemp();

                                if (itemo.contractTempList.Count() > 0)
                                {
                                    modelOfferMonitoring.ContractDetailTemp = itemo.contractTempList.FirstOrDefault();

                                    manat = (modelOfferMonitoring.ContractDetailTemp.product_asc_price - modelOfferMonitoring.ContractDetailTemp.product_unit_price).ToString();

                                    faiz = (((modelOfferMonitoring.ContractDetailTemp.product_asc_price - modelOfferMonitoring.ContractDetailTemp.product_unit_price) * 100) / modelOfferMonitoring.ContractDetailTemp.product_unit_price).ToString();
                                }

                                sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                                sheet.Cells[rowIndex, col2++].Value = item.productParentName; ;
                                sheet.Cells[rowIndex, col2++].Value = item.productName;
                                sheet.Cells[rowIndex, col2++].Value = @Custom.ConverPriceDelZero((decimal)item.totalQuantity);

                                sheet.Cells[rowIndex, col2++].Value = item.enumValueName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.unitPrice);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)item.totalQuantityPrice);

                                if (!string.IsNullOrEmpty(itemo.organizationName.Trim()))
                                {
                                    orgName = "\n" + itemo.organizationName;
                                }

                                sheet.Cells[rowIndex, col2++].Value = itemo.personName + " " + itemo.surname + " " + itemo.fatherName + orgName + "\n" + string.Join(", ", itemo.comList.Select(x => x.communication).LastOrDefault());

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

                                if (!string.IsNullOrEmpty(modelOfferMonitoring.ContractDetailTemp.ContractNumber_))
                                {
                                    contractNumber = DatetimeExtension.toShortDate(modelOfferMonitoring.ContractDetailTemp.ContractDate).ToShortDateString() + "\n" + modelOfferMonitoring.ContractDetailTemp.ContractNumber_;
                                }

                                sheet.Cells[rowIndex, col2++].Value = contractNumber;

                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)(itemo.unit_price));

                                if (modelOfferMonitoring.ContractDetailTemp.product_unit_price != null)
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_unit_price);
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_asc_price);
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                if (!string.IsNullOrEmpty(manat))
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero(decimal.Parse(manat));
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero(decimal.Parse(faiz));
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.quantity);

                                if (modelOfferMonitoring.ContractDetailTemp.product_total_count != null)
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_total_count);
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.totalQuantityPrice);

                                if (modelOfferMonitoring.ContractDetailTemp.product_total_price != null)
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_total_price);
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                sheet.Cells[rowIndex, col2++].Value = itemo.roledesc;
                                sheet.Cells[rowIndex, col2++].Value = itemo.taxesType;

                                sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                if (sproductIdArray.Contains(itemo.productId))
                                {
                                    sheet.Row(rowIndex).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    sheet.Row(rowIndex).Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                                }

                                rowIndex++;
                                ri++;
                            }
                        }

                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                        sheet.Cells[rowIndex + 1, 2, rowIndex + 1, 4].Merge = true;

                        sheet.Cells[rowIndex + 1, 2].Value = "Qeyd: ƏDV şamil edilməyən məhsullar rəngli xanalarda göstərilib";
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
                   ? (ActionResult)PartialView("PartialTotalDemandOffersGroup", modelOfferMonitoring)
                   : View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult TotalDemandOffersGroupDetail(int? page, bool excell = false, long productId = 0, long userTypeId = -1, long monthEVId = -1, string fin = null, string voen = null)
        {
            try
            {
                int pageSize = 20;
                int pageNumber = (page ?? 1);

                fin = StripTag.strSqlBlocker(fin.ToLower());
                voen = StripTag.strSqlBlocker(voen.ToLower());

                if (userTypeId == -1 && monthEVId == -1 && fin == null && voen == null)
                {
                    suserTypeId = 0;
                    smonthEVId = 0;
                    sfin = null;
                    svoen = null;
                }

                if (productId > 0)
                    sproductId = productId;
                if (userTypeId >= 0)
                    suserTypeId = userTypeId;
                if (monthEVId >= 0)
                    smonthEVId = monthEVId;
                if (fin != null)
                    sfin = fin;
                if (voen != null)
                    svoen = voen;

                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                modelOfferMonitoring.DemandOfferProductsSearch = new DemandOfferProductsSearch();

                if (excell == true)
                {
                    pageNumber = 1;
                    pageSize = 1000000;
                }

                modelOfferMonitoring.DemandOfferProductsSearch.page = 1;
                modelOfferMonitoring.DemandOfferProductsSearch.page_size = 2;
                modelOfferMonitoring.DemandOfferProductsSearch.productId = sproductId;
                modelOfferMonitoring.DemandOfferProductsSearch.monthID = smonthEVId;

                BaseOutput gpp = srv.WS_GetTotalDemandOffersPA(baseInput, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.DemanProductionGroupArray);

                if (modelOfferMonitoring.DemanProductionGroupArray == null)
                {
                    modelOfferMonitoring.DemanProductionGroup = new DemanProductionGroup();
                }
                else
                {
                    modelOfferMonitoring.DemanProductionGroup = modelOfferMonitoring.DemanProductionGroupArray.FirstOrDefault();
                }


                modelOfferMonitoring.DemandOfferProductsSearch.page = pageNumber;
                modelOfferMonitoring.DemandOfferProductsSearch.page_size = pageSize;

                modelOfferMonitoring.DemandOfferProductsSearch.roleID = suserTypeId;
                modelOfferMonitoring.DemandOfferProductsSearch.pinNumber = sfin;
                modelOfferMonitoring.DemandOfferProductsSearch.voen = svoen;

                BaseOutput dtop = srv.WS_GetTotalOffersbyProductID(baseInput, sproductId, true, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.DemanOfferProductionArray);


                if (modelOfferMonitoring.DemanOfferProductionArray == null)
                {
                    modelOfferMonitoring.DemanOfferProductionList = new List<DemanOfferProduction>();
                }
                else
                {
                    modelOfferMonitoring.DemanOfferProductionList = modelOfferMonitoring.DemanOfferProductionArray.OrderBy(x => x.unit_price).ToList().ToList();
                }


                BaseOutput dtopc = srv.WS_GetTotalOffersbyProductID_OPC(baseInput, sproductId, true, modelOfferMonitoring.DemandOfferProductsSearch, out modelOfferMonitoring.itemCount, out modelOfferMonitoring.itemCountB);

                long[] aic = new long[modelOfferMonitoring.itemCount];

                modelOfferMonitoring.PagingT = aic.ToPagedList(pageNumber, pageSize);

                modelOfferMonitoring.productId = sproductId;
                modelOfferMonitoring.userTypeId = suserTypeId;
                modelOfferMonitoring.monthEVId = smonthEVId;
                modelOfferMonitoring.fin = sfin;
                modelOfferMonitoring.voen = svoen;

                modelOfferMonitoring.productIdArray = sproductIdArray;


                if (excell == true)
                {
                    using (var excelPackage = new ExcelPackage())
                    {
                        excelPackage.Workbook.Properties.Author = "tedaruk";
                        excelPackage.Workbook.Properties.Title = "tedaruk.az";
                        var sheet = excelPackage.Workbook.Worksheets.Add("Təklif");
                        sheet.Name = "Təklif";

                        var col = 1;
                        sheet.Cells[1, col++].Value = "Məhsul üzrə tələb və təkliflər";
                        sheet.Row(1).Height = 50;
                        sheet.Row(1).Style.Font.Size = 14;
                        sheet.Row(1).Style.Font.Bold = true;
                        sheet.Row(1).Style.WrapText = true;
                        sheet.Cells[1, 1, 1, 21].Merge = true;
                        sheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        col = 1;
                        sheet.Cells[2, col++].Value = "S/N";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının adı";
                        sheet.Cells[2, col++].Value = "Ərzaq məhsullarının növü";
                        sheet.Cells[2, col++].Value = "Tələbatın həcmi (miqdar)";
                        sheet.Cells[2, col++].Value = "Ölçü vahidi";
                        sheet.Cells[2, col++].Value = "Qiymət (AZN)";
                        sheet.Cells[2, col++].Value = "Cəmi (AZN)";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının adı";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçının VÖEN-i";
                        sheet.Cells[2, col++].Value = "Satıcı və ya istehsalçı ilə alqı-satqı müqaviləsinin tarixi və nömrəsi";
                        sheet.Cells[2, col++].Value = "Malın təklif edilən qiyməti (manatla)";
                        sheet.Cells[2, col++].Value = "Müqavilə üzrə malın qiyməti";
                        sheet.Cells[2, col++].Value = "Təchizat qiyməti (manatla)";
                        sheet.Cells[2, col++].Value = "Ticarət əlavəsi (manatla)";
                        sheet.Cells[2, col++].Value = "Ticarət əlavəsi (faizlə)";
                        sheet.Cells[2, col++].Value = "Təklifin həcmi (miqdarı)";
                        sheet.Cells[2, col++].Value = "Müqavilə üzrə malın həcmi (miqdarı)";
                        sheet.Cells[2, col++].Value = "Təklifin ümumi dəyəri (manatla)";
                        sheet.Cells[2, col++].Value = "Müqavilənin ümumi dəyəri (manatla)";
                        sheet.Cells[2, col++].Value = "Tədarükçünün növü (istehsalçı satıcı və ya idxalçı)";
                        sheet.Cells[2, col++].Value = "Tədarükçünün hansı növ vergi ödəyicisi olması (ƏDV, sadələşdirilmiş K/T məhsulu istehsalçısı)";

                        sheet.Row(2).Style.Font.Bold = true;
                        sheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[2, 1, 2, 21].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        sheet.Cells[2, 1, 2, 21].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                        sheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Column(1).Width = 7;
                        sheet.Column(2).Width = 30;
                        sheet.Column(3).Width = 30;
                        sheet.Column(4).Width = 20;
                        sheet.Column(5).Width = 20;
                        sheet.Column(6).Width = 20;
                        sheet.Column(7).Width = 20;
                        sheet.Column(8).Width = 50;
                        sheet.Column(9).Width = 20;
                        sheet.Column(10).Width = 20;
                        sheet.Column(11).Width = 20;
                        sheet.Column(12).Width = 20;
                        sheet.Column(13).Width = 20;
                        sheet.Column(14).Width = 20;
                        sheet.Column(15).Width = 20;
                        sheet.Column(16).Width = 20;
                        sheet.Column(17).Width = 20;
                        sheet.Column(18).Width = 20;
                        sheet.Column(19).Width = 20;
                        sheet.Column(20).Width = 20;
                        sheet.Column(21).Width = 20;

                        int rowIndex = 3;
                        var ri = 1;
                        string orgName = "";
                        string contractNumber = "";
                        string faiz = "";
                        string manat = "";

                        var col2 = 0;

                        if (modelOfferMonitoring.DemanOfferProductionList.Count() == 0)
                        {
                            col2 = 1;

                            sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                            sheet.Cells[rowIndex, col2++].Value = modelOfferMonitoring.DemanProductionGroup.productParentName;
                            sheet.Cells[rowIndex, col2++].Value = modelOfferMonitoring.DemanProductionGroup.productName;
                            sheet.Cells[rowIndex, col2++].Value = @Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.DemanProductionGroup.totalQuantity);

                            sheet.Cells[rowIndex, col2++].Value = modelOfferMonitoring.DemanProductionGroup.enumValueName;
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.DemanProductionGroup.unitPrice);
                            sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.DemanProductionGroup.totalQuantityPrice);

                            sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            rowIndex++;
                            ri++;
                        }
                        else
                        {
                            foreach (var itemo in modelOfferMonitoring.DemanOfferProductionList)
                            {
                                col2 = 1;
                                contractNumber = "";
                                orgName = "";
                                manat = "";
                                faiz = "";
                                modelOfferMonitoring.ContractDetailTemp = new tblContractDetailTemp();

                                if (itemo.contractTempList.Count() > 0)
                                {
                                    modelOfferMonitoring.ContractDetailTemp = itemo.contractTempList.FirstOrDefault();

                                    manat = (modelOfferMonitoring.ContractDetailTemp.product_asc_price - modelOfferMonitoring.ContractDetailTemp.product_unit_price).ToString();

                                    faiz = (((modelOfferMonitoring.ContractDetailTemp.product_asc_price - modelOfferMonitoring.ContractDetailTemp.product_unit_price) * 100) / modelOfferMonitoring.ContractDetailTemp.product_unit_price).ToString();
                                }


                                sheet.Cells[rowIndex, col2++].Value = ri.ToString();
                                sheet.Cells[rowIndex, col2++].Value = modelOfferMonitoring.DemanProductionGroup.productParentName;
                                sheet.Cells[rowIndex, col2++].Value = modelOfferMonitoring.DemanProductionGroup.productName;
                                sheet.Cells[rowIndex, col2++].Value = @Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.DemanProductionGroup.totalQuantity);

                                sheet.Cells[rowIndex, col2++].Value = modelOfferMonitoring.DemanProductionGroup.enumValueName;
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.DemanProductionGroup.unitPrice);
                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.DemanProductionGroup.totalQuantityPrice);

                                if (!string.IsNullOrEmpty(itemo.organizationName.Trim()))
                                {
                                    orgName = "\n" + itemo.organizationName;
                                }

                                sheet.Cells[rowIndex, col2++].Value = itemo.personName + " " + itemo.surname + " " + itemo.fatherName + orgName + "\n" + string.Join(", ", itemo.comList.Select(x => x.communication).LastOrDefault());

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
                                if (!string.IsNullOrEmpty(modelOfferMonitoring.ContractDetailTemp.ContractNumber_))
                                {
                                    contractNumber = DatetimeExtension.toShortDate(modelOfferMonitoring.ContractDetailTemp.ContractDate).ToShortDateString() + "\n" + modelOfferMonitoring.ContractDetailTemp.ContractNumber_;
                                }

                                sheet.Cells[rowIndex, col2++].Value = contractNumber;

                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)(itemo.unit_price));

                                if (modelOfferMonitoring.ContractDetailTemp.product_unit_price != null)
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_unit_price);
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_asc_price);
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                if (!string.IsNullOrEmpty(manat))
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero(decimal.Parse(manat));
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero(decimal.Parse(faiz));
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.quantity);

                                if (modelOfferMonitoring.ContractDetailTemp.product_total_count != null)
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_total_count);
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)itemo.totalQuantityPrice);

                                if (modelOfferMonitoring.ContractDetailTemp.product_total_price != null)
                                {
                                    sheet.Cells[rowIndex, col2++].Value = Custom.ConverPriceDelZero((decimal)modelOfferMonitoring.ContractDetailTemp.product_total_price);
                                }
                                else
                                {
                                    sheet.Cells[rowIndex, col2++].Value = "";
                                }

                                sheet.Cells[rowIndex, col2++].Value = itemo.roledesc;
                                sheet.Cells[rowIndex, col2++].Value = itemo.taxesType;

                                sheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                //sheet.Row(rowIndex).Style.Numberformat.Format = "0:#,##0.000000";

                                rowIndex++;
                                ri++;
                            }
                        }
                        //var col21 = 1;

                        //string s = "158 490 005.88";
                        //int i = 1000;
                        //sheet.Cells[rowIndex, col21++].Value = "";
                        //sheet.Cells[rowIndex, col21++].Value = s; //Will not be formated
                        //sheet.Cells[rowIndex, col21++].Value = i; //Will be formated

                        //NumberFormatInfo nfi = new CultureInfo("az-Latn-AZ").NumberFormat;

                        //nfi.NumberGroupSeparator = " ";
                        //sheet.Cells[rowIndex, col21++].Value = double.Parse("23 232 323.54", nfi);

                        //sheet.Cells[rowIndex, col21++].Value = decimal.Parse("5 454 500.0", new NumberFormatInfo() { NumberGroupSeparator = " " });

                        //sheet.Cells[rowIndex, col21++].Value = 54554500;
                        //sheet.Cells[rowIndex, col21++].Value = 54554500.8500500;

                        //sheet.Row(rowIndex).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        //sheet.Row(rowIndex).Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        //sheet.Cells[rowIndex, col2++].Style.NumberFormat.Format = "#,##0";
                        //sheet.Row(3).Style.Numberformat.Format = "#,##0.#########";

                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.WrapText = true;
                        sheet.Cells[1, 1, rowIndex - 1, 21].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                   ? (ActionResult)PartialView("PartialTotalDemandOffersGroupDetail", modelOfferMonitoring)
                   : View(modelOfferMonitoring);
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
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                BaseOutput bouput = srv.GetProductCatalogsWithParent(baseInput, out modelOfferMonitoring.ProductCatalogDetailArray);

                if (modelOfferMonitoring.ProductCatalogDetailArray == null)
                {
                    modelOfferMonitoring.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelOfferMonitoring.ProductCatalogDetailList = modelOfferMonitoring.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelOfferMonitoring.actionName = actionName;
                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProductCatalogForSaleDemand(string actionName)
        {
            try
            {

                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                BaseOutput bouput = srv.WS_GetProductCatalogsDemand(baseInput, out modelOfferMonitoring.ProductCatalogDetailArray);

                if (modelOfferMonitoring.ProductCatalogDetailArray == null)
                {
                    modelOfferMonitoring.ProductCatalogDetailList = new List<ProductCatalogDetail>();
                }
                else
                {
                    modelOfferMonitoring.ProductCatalogDetailList = modelOfferMonitoring.ProductCatalogDetailArray.Where(x => x.productCatalog.canBeOrder == 1).ToList();
                }

                modelOfferMonitoring.actionName = actionName;
                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Month(string actionName, long id = 0)
        {
            try
            {
                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;


                BaseOutput ecy = srv.WS_GetEnumCategorysByName(baseInput, "month", out modelOfferMonitoring.EnumCategoryMonth);

                BaseOutput evyf = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelOfferMonitoring.EnumCategoryMonth.Id, true, out modelOfferMonitoring.EnumValueMonthArray);
                modelOfferMonitoring.EnumValueMonthList = modelOfferMonitoring.EnumValueMonthArray.ToList();

                modelOfferMonitoring.actionName = actionName;
                modelOfferMonitoring.monthEVId = id;
                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult UserType(string actionName, long id = 0)
        {
            try
            {
                baseInput = new BaseInput();
                modelOfferMonitoring = new OfferMonitoringViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelOfferMonitoring.User);
                baseInput.userName = modelOfferMonitoring.User.Username;

                BaseOutput bouput = srv.WS_GetRoles1(baseInput, out modelOfferMonitoring.RoleArray);

                if (modelOfferMonitoring.RoleArray == null)
                {
                    modelOfferMonitoring.RoleList = new List<tblRole>();
                }
                else
                {
                    modelOfferMonitoring.RoleList = modelOfferMonitoring.RoleArray.ToList();
                }

                modelOfferMonitoring.actionName = actionName;
                modelOfferMonitoring.userTypeId = id;
                return View(modelOfferMonitoring);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
