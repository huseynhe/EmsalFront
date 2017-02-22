using Emsal.AdminUI.Infrastructure;
using Emsal.AdminUI.Models;
using Emsal.Utility.CustomObjects;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Emsal.AdminUI.Controllers
{
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class HomeController : Controller
    {
        //
        // GET: /Home/

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ProductCatalogViewModel modelProductCatalog;
        private BaseInput baseInput;

        public ActionResult Index()
        {
            //srv.WS_createDb();

            try { 

            baseInput = new BaseInput();
                modelProductCatalog = new ProductCatalogViewModel();

                long? UserId=null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            BaseOutput userRole = srv.WS_GetUserRolesByUserId(baseInput, (long)UserId, true, out modelProductCatalog.userRole);

            BaseOutput bouput = srv.WS_GetProductCatalogs(baseInput, out modelProductCatalog.ProductCatalogArray);
            modelProductCatalog.ProductCatalogList = modelProductCatalog.ProductCatalogArray.ToList();

            return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult DeleteProdCat(int productCatalogId = 0)
        {
            try { 

            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            BaseOutput gpc = srv.WS_GetProductCatalogsById(baseInput, productCatalogId, true, out modelProductCatalog.ProductCatalog);

            BaseOutput dpc = srv.WS_DeleteProductCatalog(baseInput, modelProductCatalog.ProductCatalog);

            return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult ProdCatVal(int productCatalogId=0)
        {
            try { 

            baseInput = new BaseInput();

            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            BaseOutput gpd = srv.WS_GetProductDocumentsByProductCatalogId(baseInput, productCatalogId, true, out modelProductCatalog.ProductDocumentArray);
            modelProductCatalog.ProductDocument = modelProductCatalog.ProductDocumentArray.LastOrDefault();


            BaseOutput ec = srv.WS_GetEnumCategorysForProduct(baseInput, out modelProductCatalog.EnumCategoryArray);
            modelProductCatalog.EnumCategoryList = modelProductCatalog.EnumCategoryArray.ToList();

            BaseOutput ev = srv.WS_GetEnumValuesForProduct(baseInput, out modelProductCatalog.EnumValueArray);
            modelProductCatalog.EnumValueList = modelProductCatalog.EnumValueArray.ToList();


            BaseOutput pcl = srv.WS_GetProductCatalogControlsByProductID(baseInput,productCatalogId, true, out modelProductCatalog.ProductCatalogControlArray);
            modelProductCatalog.ProductCatalogControlList = modelProductCatalog.ProductCatalogControlArray.Where(x=>x.Status==1).ToList();

            return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public long AddProductCatalog(
            string productName,
            string productDescription,
            string productCode,
            int productCatalogParentID = 0,
            string enumCatVal=null,
            bool isEdit=false,
            int canBeOrder = 0
            )
        {
            try { 

            int pid=0;
            int p = 1;
            int enumCategory=0;
            int enumValue;
            baseInput = new BaseInput();
            Array ecv = enumCatVal.Split('*');

            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            modelProductCatalog.ProductCatalog = new tblProductCatalog();

           if (isEdit == true)
            {
                BaseOutput bouput  = srv.WS_GetProductCatalogsById(baseInput, productCatalogParentID, true, out modelProductCatalog.ProductCatalog);
            }

           if (isEdit == false)
           {
            modelProductCatalog.ProductCatalog.ProductCatalogParentID = (int)productCatalogParentID;
            modelProductCatalog.ProductCatalog.ProductCatalogParentIDSpecified = true;
           }

            modelProductCatalog.ProductCatalog.ProductName = productName;
            modelProductCatalog.ProductCatalog.ProductDescription = productDescription;
            modelProductCatalog.ProductCatalog.productCode = productCode;
            modelProductCatalog.ProductCatalog.canBeOrder = (int)canBeOrder;
            modelProductCatalog.ProductCatalog.canBeOrderSpecified = true;


            if (isEdit == false)
            {
                BaseOutput pout = srv.WS_AddProductCatalog(baseInput, modelProductCatalog.ProductCatalog, out modelProductCatalog.ProductCatalog);
            }
            else if (isEdit == true)
            {
                BaseOutput pout = srv.WS_UpdateProductCatalog(baseInput, modelProductCatalog.ProductCatalog, out modelProductCatalog.ProductCatalog);
            }


           BaseOutput dp = srv.WS_DeleteAllProductCatalogControlByProductID(baseInput, modelProductCatalog.ProductCatalog.Id, true);

            foreach (string item in ecv)
            {
               Array enumCatValArray = item.Split(',');

                foreach (string item2 in enumCatValArray)
                {
                    if (item2 != "")
                    {
                        if (p == 1)
                        {
                            enumCategory = Convert.ToInt32(item2);
                        }
                        else if (p > 1)
                        {

                            enumValue = Convert.ToInt32(item2);

                            modelProductCatalog.ProductCatalogControl = new tblProductCatalogControl();
                            modelProductCatalog.ProductCatalogControl.ProductId = modelProductCatalog.ProductCatalog.Id;
                            modelProductCatalog.ProductCatalogControl.ProductIdSpecified = true;
                            modelProductCatalog.ProductCatalogControl.EnumCategoryId = enumCategory;
                            modelProductCatalog.ProductCatalogControl.EnumCategoryIdSpecified = true;
                            modelProductCatalog.ProductCatalogControl.EnumValueId = enumValue;
                            modelProductCatalog.ProductCatalogControl.EnumValueIdSpecified = true;

                            BaseOutput pcc = srv.WS_AddProductCatalogControl(baseInput, modelProductCatalog.ProductCatalogControl, out modelProductCatalog.ProductCatalogControl);
                        }
                        p = p + 1;
                    }
                }
                p = 1;
            }

            return modelProductCatalog.ProductCatalog.Id;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        [HttpPost]
        public void File(IList<HttpPostedFileBase> file, long prodId)
        {
            try { 

            if (file != null)
            {
                baseInput = new BaseInput();

                modelProductCatalog = new ProductCatalogViewModel();

                long? UserId = null;
                if (User != null && User.Identity.IsAuthenticated)
                {
                    FormsIdentity identity = (FormsIdentity)User.Identity;
                    if (identity.Ticket.UserData.Length > 0)
                    {
                        UserId = Int32.Parse(identity.Ticket.UserData);
                    }
                }
                BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
                baseInput.userName = modelProductCatalog.Admin.Username;

                BaseOutput gpd = srv.WS_GetProductDocumentsByProductCatalogId(baseInput, prodId,true, out modelProductCatalog.ProductDocumentArrayFile);

                BaseOutput dpd = srv.WS_DeleteProductDocument(baseInput, modelProductCatalog.ProductDocumentArrayFile.LastOrDefault());

                BaseOutput envalyd = srv.WS_GetEnumValueByName(baseInput, "profilePicture", out modelProductCatalog.EnumValueFP);

                String sDate = DateTime.Now.ToString();
                DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

                String dy = datevalue.Day.ToString();
                String mn = datevalue.Month.ToString();
                String yy = datevalue.Year.ToString();

                string path = modelProductCatalog.fileDirectory;
                //string pathu = path.Replace("Emsal.AdminUI", "Emsal.UI");
                //string path = modelProductCatalog.fileDirectory + @"\" + yy + @"\" + mn + @"\" + dy;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }



                foreach (var attachfile in file)
                {

                        //var dec=(attachfile.ContentLength / 1024);
                        //var decd = (decimal)731 / 1024;
                        //var fl = Math.Round(dec,2);
                        string fre = FileExtension.GetMimeType(attachfile.InputStream, attachfile.FileName);

                        if (attachfile != null && attachfile.ContentLength > 0 && attachfile.ContentLength <= modelProductCatalog.fileSize && modelProductCatalog.fileTypes.Contains(fre))
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
                        //attachfile.SaveAs(Path.Combine(pathu, fileName));

                        modelProductCatalog.ProductDocument = new tblProduct_Document();
                        BaseOutput enumval = srv.WS_GetEnumValueByName(baseInput, "potential", out modelProductCatalog.EnumValue);

                        modelProductCatalog.ProductDocument.Product_catalog_Id = prodId;
                        modelProductCatalog.ProductDocument.Product_catalog_IdSpecified = true;
                      
                        modelProductCatalog.ProductDocument.documentUrl = modelProductCatalog.fileDirectorySV;
                        modelProductCatalog.ProductDocument.documentName = fileName;
                        modelProductCatalog.ProductDocument.documentRealName = ofileName;


                        modelProductCatalog.ProductDocument.documentSize = attachfile.ContentLength;
                        modelProductCatalog.ProductDocument.documentSizeSpecified = true;

                        modelProductCatalog.ProductDocument.document_type_ev_Id = modelProductCatalog.EnumValueFP.Id.ToString();

                       BaseOutput apd = srv.WS_AddProductDocument(baseInput, modelProductCatalog.ProductDocument, out modelProductCatalog.ProductDocument);
                    }
                }

                }

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message + ex.Source + ex.StackTrace;
            }
        }


        public ActionResult MainPage()
        {
            try { 

            return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult UserProfile()
        {
            try { 

            baseInput = new BaseInput();
            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult UserAppeal()
        {
            try { 

            baseInput = new BaseInput();
            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult UserAppealDetail()
        {
            try { 

            baseInput = new BaseInput();
            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult Login()
        {
            try { 

            return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult Report()
        {
            try { 

            baseInput = new BaseInput();
            modelProductCatalog = new ProductCatalogViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductCatalog.Admin);
            baseInput.userName = modelProductCatalog.Admin.Username;

            return View(modelProductCatalog);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Report2()
        {
            try { 

            return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult Report3()
        {
            try { 

            return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
        public ActionResult Report4()
        {
            try { 

            return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
    }
}
