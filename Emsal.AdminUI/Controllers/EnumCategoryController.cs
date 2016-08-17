using Emsal.AdminUI.Models;
using Emsal.WebInt.EmsalSrv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.Security;
using Emsal.AdminUI.Infrastructure;

namespace Emsal.AdminUI.Controllers
{
    //[EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]

    public class EnumCategoryController : Controller
    {
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private EnumCategoryViewModel modelEnumCategory;

        private BaseInput baseInput;     
        public ActionResult Index(int? page)
        {
            baseInput = new BaseInput();

            int pageSize =20;
            int pageNumber = (page ?? 1);
            
            modelEnumCategory = new EnumCategoryViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumCategory.Admin);
            baseInput.userName = modelEnumCategory.Admin.Username;


            BaseOutput bouput = srv.WS_GetEnumCategorys(baseInput, out modelEnumCategory.EnumCategoryArray);

            modelEnumCategory.EnumCategoryList = modelEnumCategory.EnumCategoryArray.ToList();

            modelEnumCategory.Paging = modelEnumCategory.EnumCategoryList.ToPagedList(pageNumber, pageSize);

            return Request.IsAjaxRequest()
                ? (ActionResult)PartialView("PartialIndex", modelEnumCategory)
                : View(modelEnumCategory);

            //return View(modelEnumCategory);
        }


        public ActionResult Create()
        {
            //???????????????????
            baseInput = new BaseInput();

            modelEnumCategory = new EnumCategoryViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumCategory.Admin);
            baseInput.userName = modelEnumCategory.Admin.Username;

            return View(modelEnumCategory);
        }

        [HttpPost]
        public ActionResult Create(EnumCategoryViewModel model, FormCollection collection)
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


            modelEnumCategory = new EnumCategoryViewModel();
            modelEnumCategory.EnumCategory = new tblEnumCategory();

            if (model.isProductDescibe == true)
            {
                modelEnumCategory.EnumCategory.isProductDescibe = 1;
                modelEnumCategory.EnumCategory.isProductDescibeSpecified = true;
            }

            modelEnumCategory.EnumCategory.name = model.name;
            modelEnumCategory.EnumCategory.description = model.description;
            modelEnumCategory.EnumCategory.Status = 1;
            modelEnumCategory.EnumCategory.LastUpdatedStatus = 1;

            BaseOutput pout = srv.WS_AddEnumCategory(baseInput, modelEnumCategory.EnumCategory, out modelEnumCategory.EnumCategory);

            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            baseInput = new BaseInput();
            modelEnumCategory = new EnumCategoryViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumCategory.Admin);
            baseInput.userName = modelEnumCategory.Admin.Username;


            modelEnumCategory.EnumCategory = new tblEnumCategory();

            BaseOutput gec = srv.WS_GetEnumCategoryById(baseInput, id, true, out modelEnumCategory.EnumCategory);
            BaseOutput dec = srv.WS_DeleteEnumCategory(baseInput, modelEnumCategory.EnumCategory);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            baseInput = new BaseInput();

            modelEnumCategory = new EnumCategoryViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumCategory.Admin);
            baseInput.userName = modelEnumCategory.Admin.Username;


            modelEnumCategory.EnumCategory = new tblEnumCategory();

            BaseOutput bouput = srv.WS_GetEnumCategoryById(baseInput, id, true, out modelEnumCategory.EnumCategory);

            modelEnumCategory.Id= modelEnumCategory.EnumCategory.Id;
            modelEnumCategory.name = modelEnumCategory.EnumCategory.name;
            modelEnumCategory.description = modelEnumCategory.EnumCategory.description;
            if(modelEnumCategory.EnumCategory.isProductDescibe==1)
            modelEnumCategory.isProductDescibe = true;
            else
                modelEnumCategory.isProductDescibe = false;

            return View(modelEnumCategory);
        }

        [HttpPost]
        public ActionResult Edit(EnumCategoryViewModel model, FormCollection collection)
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


            modelEnumCategory = new EnumCategoryViewModel();


            modelEnumCategory.EnumCategory = new tblEnumCategory();

            BaseOutput bouput = srv.WS_GetEnumCategoryById(baseInput, model.Id, true, out modelEnumCategory.EnumCategory);
            modelEnumCategory.EnumCategory.name = model.name;
            modelEnumCategory.EnumCategory.description = model.description;
            if (model.isProductDescibe == true)
            {
                modelEnumCategory.EnumCategory.isProductDescibe = 1;
                modelEnumCategory.EnumCategory.isProductDescibeSpecified=true;                
            }
            else
            {
                modelEnumCategory.EnumCategory.isProductDescibe = null;
                modelEnumCategory.EnumCategory.isProductDescibeSpecified = true;
            }

            BaseOutput ecout = srv.WS_UpdateEnumCategory(baseInput, modelEnumCategory.EnumCategory, out modelEnumCategory.EnumCategory);

            return RedirectToAction("Index");
        }


    }
}
