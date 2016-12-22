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
using Emsal.Utility.CustomObjects;

namespace Emsal.AdminUI.Controllers
{
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class EnumValueController : Controller
    {
        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private EnumValueViewModel modelEnumValue;

        private BaseInput baseInput;
        private static string sname;

        public ActionResult IndexBy(int enumCategoryId)
        {
            try { 

            baseInput = new BaseInput();
            
            modelEnumValue = new EnumValueViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumValue.Admin);
            baseInput.userName = modelEnumValue.Admin.Username;

            BaseOutput bouput = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, enumCategoryId, true,  out modelEnumValue.EnumValueArray);

            modelEnumValue.EnumValueList = modelEnumValue.EnumValueArray.ToList();

            return View(modelEnumValue);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }
   
        public ActionResult Index(int? page, string name = null)
        {
            try { 

            baseInput = new BaseInput();

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            
            modelEnumValue = new EnumValueViewModel();

                if (name != null)
                    name = StripTag.strSqlBlocker(name.ToLower());

                if (name == null)
                {
                    sname = null;
                }

                if (name != null)
                    sname = name;


                long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumValue.Admin);
            baseInput.userName = modelEnumValue.Admin.Username;

            BaseOutput bouput = srv.WS_GetEnumValues(baseInput, out modelEnumValue.EnumValueArray);

            modelEnumValue.EnumValueList = modelEnumValue.EnumValueArray.ToList();

                if (sname != null)
                {
                    modelEnumValue.EnumValueList = modelEnumValue.EnumValueList.Where(x=>x.name.ToLower().Contains(sname)).ToList();
                }

                modelEnumValue.Paging = modelEnumValue.EnumValueList.ToPagedList(pageNumber, pageSize);

                modelEnumValue.name = sname;

                return Request.IsAjaxRequest()
         ? (ActionResult)PartialView("PartialIndex", modelEnumValue)
         : View(modelEnumValue);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Create(int enumCategoryId)
        {
            try { 

            baseInput = new BaseInput();
            modelEnumValue = new EnumValueViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumValue.Admin);
            baseInput.userName = modelEnumValue.Admin.Username;

            modelEnumValue.enumCategoryId = enumCategoryId;

            modelEnumValue.EnumCategory=new tblEnumCategory();
             BaseOutput bouput = srv.WS_GetEnumCategoryById(baseInput, enumCategoryId, true, out modelEnumValue.EnumCategory);

            return View(modelEnumValue);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        [HttpPost]
        public ActionResult Create(EnumValueViewModel model, FormCollection collection)
        {
            try { 

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

            modelEnumValue = new EnumValueViewModel();

            modelEnumValue.EnumValue = new tblEnumValue();

            modelEnumValue.EnumValue.enumCategory_enumCategoryId = model.enumCategoryId;
            modelEnumValue.EnumValue.enumCategory_enumCategoryIdSpecified = true;

            modelEnumValue.EnumValue.name = model.name;
            modelEnumValue.EnumValue.description = model.description;
            //modelEnumValue.EnumValue.Status = 1;
            //modelEnumValue.EnumValue.LastUpdatedStatus = 1;
            //modelEnumValue.EnumValue.createdDate= DateTime.Now.Ticks;
            //modelEnumValue.EnumValue.updatedDate= DateTime.Now.Ticks;


            BaseOutput pout = srv.WS_AddEnumValue(baseInput, modelEnumValue.EnumValue, out modelEnumValue.EnumValue);

            return RedirectToAction("Create", "EnumValue", new { enumCategoryId = model.enumCategoryId });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult Delete(int id)
        {
            try { 

            baseInput = new BaseInput();
            modelEnumValue = new EnumValueViewModel();


            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumValue.Admin);
            baseInput.userName = modelEnumValue.Admin.Username;

            modelEnumValue.EnumValue=new tblEnumValue();

             BaseOutput gev = srv.WS_GetEnumValueById(baseInput, id, true, out modelEnumValue.EnumValue);
             BaseOutput dpc = srv.WS_DeleteEnumValue(baseInput, modelEnumValue.EnumValue);

            return RedirectToAction("Create", "EnumValue", new { enumCategoryId = modelEnumValue.EnumValue.enumCategory_enumCategoryId });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        public ActionResult Edit(int id)
        {
            try { 

            baseInput = new BaseInput();

            modelEnumValue = new EnumValueViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelEnumValue.Admin);
            baseInput.userName = modelEnumValue.Admin.Username;

            modelEnumValue.EnumValue = new tblEnumValue();

            BaseOutput bouput = srv.WS_GetEnumValueById(baseInput, id, true, out modelEnumValue.EnumValue);

            modelEnumValue.Id= modelEnumValue.EnumValue.Id;
            modelEnumValue.name = modelEnumValue.EnumValue.name;
            modelEnumValue.description = modelEnumValue.EnumValue.description;

            return View(modelEnumValue);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

        [HttpPost]
        public ActionResult Edit(EnumValueViewModel model, FormCollection collection)
        {
            try { 

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

            modelEnumValue = new EnumValueViewModel();

            modelEnumValue.EnumValue = new tblEnumValue();

            BaseOutput bouput = srv.WS_GetEnumValueById(baseInput, model.Id, true, out modelEnumValue.EnumValue);
            modelEnumValue.EnumValue.name = model.name;
            modelEnumValue.EnumValue.description = model.description;

            BaseOutput ecout = srv.WS_UpdateEnumValue(baseInput, modelEnumValue.EnumValue, out modelEnumValue.EnumValue);

            //return RedirectToAction("Index");
            return RedirectToAction("Create", "EnumValue", new { enumCategoryId = modelEnumValue.EnumValue.enumCategory_enumCategoryId });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


    }
}
