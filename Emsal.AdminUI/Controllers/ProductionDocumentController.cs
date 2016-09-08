using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Emsal.Utility.CustomObjects;
using Microsoft.Win32;
using System.Web.Security;
using Emsal.AdminUI.Infrastructure;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class ProductionDocumentController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ProductionDocumentViewModel modelproductionDocument;

        public ActionResult Index()
        {
            try { 

            return View();

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult GetFile(long Id)
        {
            try { 

            baseInput = new BaseInput();
            modelproductionDocument = new ProductionDocumentViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelproductionDocument.Admin);
            baseInput.userName = modelproductionDocument.Admin.Username;


            BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput,Id, true, out modelproductionDocument.ProductionDocument);

            string fileName = modelproductionDocument.ProductionDocument.documentName;
            string targetPath = modelproductionDocument.tempFileDirectory;

            string sourceFile = System.IO.Path.Combine(modelproductionDocument.ProductionDocument.documentUrl, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            //var extension = Path.Get(fileName);


                var extension = Path.GetExtension(fileName);

                if (String.IsNullOrWhiteSpace(extension))
                {
                    return null;
                }

                var registryKey = Registry.ClassesRoot.OpenSubKey(extension);

                if (registryKey == null)
                {
                    return null;
                }

            modelproductionDocument.FCType = registryKey.GetValue("Content Type") as string;




            System.IO.File.Copy(sourceFile, destFile, true);

            return View(modelproductionDocument);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


    }
}
