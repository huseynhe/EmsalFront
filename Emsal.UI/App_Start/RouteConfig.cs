using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Emsal.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
      name: "ASCSpecial",
      url: "ASCSpecialSummary/Index",
      defaults: new { controller = "ASCSpecialSummary", action = "Index", id = UrlParameter.Optional }
  );
            routes.MapRoute(
  name: "KTNSpecial",
  url: "KTNSpecialSummary/Index",
  defaults: new { controller = "KTNSpecialSummary", action = "Index", id = UrlParameter.Optional }
);
            routes.MapRoute(
          name: "AsanXidmetSpecial",
          url: "AsanXidmetSpecialSummary/Index",
          defaults: new { controller = "AsanXidmetSpecialSummary", action = "Index", id = UrlParameter.Optional }
      );
            routes.MapRoute(
             name: "GovernmentOrganisationSpecial",
             url: "GovernmentOrganisationSpecialSummary/Index",
             defaults: new { controller = "GovernmentOrganisationSpecialSummary", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
             name: "Special",
             url: "SpecialSummary/Index",
             defaults: new { controller = "SpecialSummary", action = "Index", id = UrlParameter.Optional }
         );

            routes.MapRoute(
        name: "OfferProduction",
        url: "OfferProduction/Index",
        defaults: new { controller = "OfferProduction", action = "Index", id = UrlParameter.Optional }
    );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}