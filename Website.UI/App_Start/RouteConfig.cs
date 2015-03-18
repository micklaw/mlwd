﻿using System.Web.Mvc;
using System.Web.Routing;
using MLWD.Umbraco.Mvc.Startup;

namespace Website.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Robots",
                url: "robots.txt",
                defaults: new { controller = "Home", action = "Robots" }
            );

            routes.MapRoute(
                name: "Sitemap",
                url: "sitemap.xml",
                defaults: new { controller = "Home", action = "SiteMap" }
            );

            MLWDRouteConfig.RegisterRoutes(routes);
        }
    }
}