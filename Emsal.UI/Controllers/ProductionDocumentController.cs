using Emsal.UI.Models;
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
using Emsal.UI.Infrastructure;

namespace Emsal.UI.Controllers
{
    [EmsalAuthorization(AuthorizedAction = ActionName.Ordinary)]
    public class ProductionDocumentController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        Emsal.WebInt.IAMAS.Service1 iamasSrv = Emsal.WebInt.EmsalService.iamasService;
        

        private ProductionDocumentViewModel modelProductionDocument;

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
            modelProductionDocument = new ProductionDocumentViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelProductionDocument.User);
            baseInput.userName = modelProductionDocument.User.Username;


            BaseOutput gpd = srv.WS_GetProductionDocumentById(baseInput,Id, true, out modelProductionDocument.ProductionDocument);

            string fileName = modelProductionDocument.ProductionDocument.documentName;
            string targetPath = modelProductionDocument.tempFileDirectory;

            string sourceFile = System.IO.Path.Combine(modelProductionDocument.ProductionDocument.documentUrl, fileName);
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

            modelProductionDocument.FCType = registryKey.GetValue("Content Type") as string;




            System.IO.File.Copy(sourceFile, destFile, true);

            return View(modelProductionDocument);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


    }
}
