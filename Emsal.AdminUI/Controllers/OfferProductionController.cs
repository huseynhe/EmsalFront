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

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.ProductionDetailArray);

                modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferProduction.EnumCategory.Id && x.person != null).ToList();

                if (sproductName != null)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (suserInfo != null)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo)).ToList();
                }

                modelOfferProduction.Paging = modelOfferProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                    modelOfferProduction.isMain = 0;
                else
                    modelOfferProduction.isMain = 1;


                modelOfferProduction.statusEV = sstatusEV;
                modelOfferProduction.productName = sproductName;
                modelOfferProduction.userInfo = suserInfo;
                //return View(modelDemandProduction);

                return Request.IsAjaxRequest()
                   ? (ActionResult)PartialView("PartialIndex", modelOfferProduction)
                   : View(modelOfferProduction);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Indexwd(int? page, string statusEV = null, string productName = null, string fullAddress = null, string userInfo = null, bool excell = false)
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

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                if (productName == null && fullAddress == null && userInfo == null)
                {
                    sproductName = null;
                    sfullAddress = null;
                    suserInfo = null;
                }

                if (productName != null)
                    sproductName = productName;
                if (fullAddress != null)
                    sfullAddress = fullAddress;
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

                BaseOutput gpp = srv.WS_GetOfferProductionDetailistForEValueId(baseInput, modelOfferProduction.EnumValue.Id, true, out modelOfferProduction.ProductionDetailArray);

                modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailArray.Where(x => x.enumCategoryId == modelOfferProduction.EnumCategory.Id && x.person != null).ToList();

                if (sproductName != null)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.productName.ToLower().Contains(sproductName) || x.productParentName.ToLower().Contains(sproductName)).ToList();
                }

                if (sfullAddress != null)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.fullAddress.ToLower().Contains(fullAddress)).ToList();
                }

                if (suserInfo != null)
                {
                    modelOfferProduction.ProductionDetailList = modelOfferProduction.ProductionDetailList.Where(x => x.person.Name.ToLower().Contains(suserInfo) || x.person.Surname.ToLower().Contains(suserInfo) || x.person.FatherName.ToLower().Contains(suserInfo) || x.person.gender.ToLower().Contains(suserInfo) || x.personAdress.ToLower().Contains(suserInfo) || x.personAdressDesc.ToLower().Contains(suserInfo)).ToList();
                }

                modelOfferProduction.Paging = modelOfferProduction.ProductionDetailList.ToPagedList(pageNumber, pageSize);

                modelOfferProduction.allPagePrice = modelOfferProduction.ProductionDetailList.Sum(x => x.unitPrice);
                modelOfferProduction.currentPagePrice = modelOfferProduction.Paging.Sum(x => x.unitPrice);

                if (sstatusEV == "Yayinda" || sstatusEV == "yayinda")
                    modelOfferProduction.isMain = 0;
                else
                    modelOfferProduction.isMain = 1;

                modelOfferProduction.statusEV = sstatusEV;
                modelOfferProduction.productName = sproductName;
                modelOfferProduction.fullAddress = sfullAddress;
                modelOfferProduction.userInfo = suserInfo;
                //return View(modelDemandProduction);


                if (excell == true)
                {
                    DataTable products = new System.Data.DataTable("offer");

                    products.Columns.Add("Məhsulun adı", typeof(string));
                    products.Columns.Add("Dövrülük", typeof(string));
                    products.Columns.Add("Miqdarı (vahidi)", typeof(string));
                    products.Columns.Add("Qiyməti (AZN-lə)", typeof(string));
                    products.Columns.Add("Təklifin ünvanı - Mənşəyi", typeof(string));
                    products.Columns.Add("Təklif verənin soyadı, adı, atasının adı, ünvanı", typeof(string));

                    modelOfferProduction.OfferProduxtionExcellList = new List<OfferProduxtionExcell>();

                    foreach (var item in modelOfferProduction.ProductionDetailList)
                    {
                        modelOfferProduction.OfferProduxtionExcell = new OfferProduxtionExcell();

                        modelOfferProduction.OfferProduxtionExcell.productName = item.productName + " " + (item.productParentName);
                        if (item.productionCalendarList.FirstOrDefault().TypeDescription != null)
                        {
                            modelOfferProduction.OfferProduxtionExcell.typeDescription = item.productionCalendarList.FirstOrDefault().TypeDescription;
                        }

                        modelOfferProduction.OfferProduxtionExcell.quantity = item.quantity.ToString() + " " + item.enumValueName;

                        modelOfferProduction.OfferProduxtionExcell.unitPrice = item.unitPrice.ToString();
                        modelOfferProduction.OfferProduxtionExcell.fullAddress = item.fullAddress;
                        modelOfferProduction.OfferProduxtionExcell.personNameAddress = item.person.Name + " " + item.person.Surname + " " + item.person.FatherName + " " + (item.person.gender) + " " + item.personAdress + " " + (item.personAdressDesc);


                        products.Rows.Add(modelOfferProduction.OfferProduxtionExcell.productName, modelOfferProduction.OfferProduxtionExcell.typeDescription, modelOfferProduction.OfferProduxtionExcell.quantity, modelOfferProduction.OfferProduxtionExcell.unitPrice, modelOfferProduction.OfferProduxtionExcell.fullAddress, modelOfferProduction.OfferProduxtionExcell.personNameAddress);

                    }


                    products.Rows.Add("Bütün qiymət", "", "", modelOfferProduction.allPagePrice);

                    var grid = new GridView();
                    grid.DataSource = products;
                    grid.DataBind();

                    string filename = Guid.NewGuid() + ".xls";

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
                    Response.ContentType = "application/ms-excel";

                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();


                    //string filename = Guid.NewGuid() + ".xls";
                    //StringWriter tw = new StringWriter();
                    //HtmlTextWriter hw = new HtmlTextWriter(tw);

                    //DataGrid dgGrid = new DataGrid();
                    //dgGrid.DataSource = modelOfferProduction.OfferProduxtionExcellList;
                    //dgGrid.DataBind();
                    //dgGrid.RenderControl(hw);

                    //Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
                    //Response.ContentType = "application/vnd.ms-excel";
                    //Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
                    //Response.Write(tw.ToString());
                    //Response.End();
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

    }
}
