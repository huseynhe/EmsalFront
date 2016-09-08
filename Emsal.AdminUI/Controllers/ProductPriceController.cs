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
    [EmsalAdminAuthentication(AuthorizedAction = ActionName.admin)]
    public class ProductPriceController : Controller
    {
        private BaseInput baseInput;

        Emsal.WebInt.EmsalSrv.EmsalService srv = Emsal.WebInt.EmsalService.emsalService;

        private ProductPriceViewModel modelproductPrice;

        public ActionResult Index()
        {
            try { 

            baseInput = new BaseInput();
            modelproductPrice = new ProductPriceViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelproductPrice.Admin);
            baseInput.userName = modelproductPrice.Admin.Username;

            modelproductPrice.ProductPriceDetailList = new List<ProductPriceDetail>();
            modelproductPrice.ProductPriceDetailListPaging = modelproductPrice.ProductPriceDetailList.ToPagedList(1, 1);

            BaseOutput ecy = srv.WS_GetEnumCategorysByName(baseInput, "year", out modelproductPrice.EnumCategoryYear);
            BaseOutput ecr = srv.WS_GetEnumCategorysByName(baseInput, "rub", out modelproductPrice.EnumCategoryRub);

            BaseOutput evy = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelproductPrice.EnumCategoryYear.Id, true, out modelproductPrice.EnumValueArrayYear);
            modelproductPrice.EnumValueListYear = modelproductPrice.EnumValueArrayYear.ToList();

            BaseOutput evr = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelproductPrice.EnumCategoryRub.Id, true, out modelproductPrice.EnumValueArrayRub);
            modelproductPrice.EnumValueListRub = modelproductPrice.EnumValueArrayRub.ToList();

            return View(modelproductPrice);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        [HttpPost]
        public ActionResult Index(ProductPriceViewModel model)
        {
            try { 

            baseInput = new BaseInput();
            int pageSize = 100;
            int pageNumber = 1;

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

            //model.ProductCatalogList = new List<tblProductCatalog>();

            BaseOutput ecy = srv.WS_GetEnumValueById(baseInput, model.year, true, out model.EnumValueYear);
            BaseOutput ecr = srv.WS_GetEnumValueById(baseInput, model.rub, true, out model.EnumValueRub);

            BaseOutput envalyd = srv.WS_GetProductPriceListNotPrice(baseInput, Convert.ToInt64(model.EnumValueYear.name), true, Convert.ToInt64(model.EnumValueRub.name), true, out model.ProductPriceDetailArray);
            //model.ProductPriceDetailList = model.ProductPriceDetailArray.ToList();
            model.ProductPriceDetailListPaging = model.ProductPriceDetailArray.ToList().ToPagedList(pageNumber, pageSize);



            if (model.approv == 1)
            {

                for (int i = 0; i < model.prodId.Count(); i++)
                {
                    model.ProductPrice = new tblProductPrice();

                    model.ProductPrice.productId = model.prodId[i];
                    model.ProductPrice.productIdSpecified = true;
                    model.ProductPrice.unit_price = Convert.ToDecimal(model.price[i]);
                    //model.ProductPrice.unit_price = Convert.ToDecimal(model.price[i].Replace('.', ','));
                    model.ProductPrice.unit_priceSpecified = true;
                    model.ProductPrice.year = Convert.ToInt64(model.EnumValueYear.name);
                    model.ProductPrice.yearSpecified = true;
                    model.ProductPrice.partOfYear = Convert.ToInt64(model.EnumValueRub.name);
                    model.ProductPrice.partOfYearSpecified = true;

                    BaseOutput app = srv.WS_AddProductPrice(baseInput, model.ProductPrice, out model.ProductPriceOUT);
                }

                return RedirectToAction("Approv", "ProductPrice", new { year = model.year, rub = model.rub });

            }
            else
            {
                return View(model);
                }

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }


        public ActionResult Approv(int year = 0, int rub = 0, int page = 1)
        {
            try { 

            //Bura tamamlanmadi
            int pageSize = 25;
            int pageNumber = page;

            baseInput = new BaseInput();
            modelproductPrice = new ProductPriceViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelproductPrice.Admin);
            baseInput.userName = modelproductPrice.Admin.Username;


            modelproductPrice.NPCount = 0;

            if (year > 0 && rub > 0)
            {

                BaseOutput evy = srv.WS_GetEnumValueById(baseInput, year, true, out modelproductPrice.EnumValueYear);
                BaseOutput evr = srv.WS_GetEnumValueById(baseInput, rub, true, out modelproductPrice.EnumValueRub);

                BaseOutput gpp = srv.WS_GetProductPriceList(baseInput, Convert.ToInt64(modelproductPrice.EnumValueYear.name), true, Convert.ToInt64(modelproductPrice.EnumValueRub.name), true, out modelproductPrice.ProductPriceDetailArray);

                modelproductPrice.ProductPriceDetailList = modelproductPrice.ProductPriceDetailArray.ToList();

                modelproductPrice.Paging = modelproductPrice.ProductPriceDetailArray.ToPagedList(pageNumber, pageSize);

                modelproductPrice.year = modelproductPrice.EnumValueYear.Id;
                modelproductPrice.rub = modelproductPrice.EnumValueRub.Id;


                BaseOutput envalyd = srv.WS_GetProductPriceListNotPrice(baseInput, Convert.ToInt64(modelproductPrice.EnumValueYear.name), true, Convert.ToInt64(modelproductPrice.EnumValueRub.name), true, out modelproductPrice.ProductPriceDetailArrayNP);
                modelproductPrice.NPCount = modelproductPrice.ProductPriceDetailArrayNP.ToList().Count();
            }
            else
            {
                modelproductPrice.ProductPriceDetailList = new List<ProductPriceDetail>();
            }
            BaseOutput ecy = srv.WS_GetEnumCategorysByName(baseInput, "year", out modelproductPrice.EnumCategoryYear);
            BaseOutput ecr = srv.WS_GetEnumCategorysByName(baseInput, "rub", out modelproductPrice.EnumCategoryRub);

            BaseOutput evyf = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelproductPrice.EnumCategoryYear.Id, true, out modelproductPrice.EnumValueArrayYear);
            modelproductPrice.EnumValueListYear = modelproductPrice.EnumValueArrayYear.ToList();

            BaseOutput evrf = srv.WS_GetEnumValuesByEnumCategoryId(baseInput, modelproductPrice.EnumCategoryRub.Id, true, out modelproductPrice.EnumValueArrayRub);
            modelproductPrice.EnumValueListRub = modelproductPrice.EnumValueArrayRub.ToList();



            return View("Approv", modelproductPrice);

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }



        public ActionResult Delete(long id, int year, int rub)
        {
            try { 

            baseInput = new BaseInput();

            modelproductPrice = new ProductPriceViewModel();

            long? UserId = null;
            if (User != null && User.Identity.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                if (identity.Ticket.UserData.Length > 0)
                {
                    UserId = Int32.Parse(identity.Ticket.UserData);
                }
            }
            BaseOutput user = srv.WS_GetUserById(baseInput, (long)UserId, true, out modelproductPrice.Admin);
            baseInput.userName = modelproductPrice.Admin.Username;

            modelproductPrice.ProductPrice = new tblProductPrice();
            modelproductPrice.ProductPrice.Id = id;
            modelproductPrice.ProductPrice.IdSpecified = true;

            BaseOutput dpp = srv.WS_DeleteProductPrice(baseInput, modelproductPrice.ProductPrice);

            return RedirectToAction("Approv", "ProductPrice", new { year = year, rub = rub });

            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Error", "Error"));
            }
        }

    }
}
