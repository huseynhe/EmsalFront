using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emsal.UI.Controllers
{
    public class OtherController : Controller
    {
        //
        // GET: /Other/

        public ActionResult legislation()
        {
            return View();
        }

        public ActionResult activity()
        {
            return View();
        }

    }
}
