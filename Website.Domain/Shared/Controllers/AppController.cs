﻿using System.Collections.Generic;
using System.Web;
using Website.Domain.Shared.DocTypes;
using Website.Domain.Shared.Models;
using Website.Domain.Sitemap.ActionResults;
using Website.Domain.Sitemap.Models.Interfaces;
using Yomego.Umbraco;
using Yomego.Umbraco.Mvc.Controllers.App;

namespace Website.Domain.Shared.Controllers
{
    public class AppController : UmbracoController<WebsiteApp>
    {
        protected XmlSitemapResult Sitemap(IList<ISitemapItem> sitemap)
        {
            return new XmlSitemapResult(sitemap);
        }

        private void CheckForSEOContent()
        {
            var page = Node as Page;

            if (page != null)
            {
                ViewBag.PageTitle = page.MetaPageTitle;

                if (string.IsNullOrWhiteSpace(page.MetaPageTitle))
                {
                    ViewBag.PageTitle = page.Name;
                }

                ViewBag.MetaDescription = page.MetaDescription;
            }
        }

        private void WriteCanonical()
        {
            var node = Node as Page;

            if (node != null)
            {
                ViewBag.Canonical = App.Context.DomainUrl + node.Url;
            }
        }

        private void WriteOg()
        {
            var node = Node as Page;

            if (node != null && node.OgUseThis)
            {
                var model = new OgModel
                {
                    Title = node.OgTitle,
                    Description = node.OgDescription,
                    PageUrl = App.Context.DomainUrl + node.Url
                };

                if (node.OgImage != null)
                {
                    model.ImageUrl = App.Context.DomainUrl + HttpUtility.UrlDecode(node.OgImage.GetCrop(300, 300));
                }

                ViewBag.OgModel = model;
            }
        }

        protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            WriteCanonical();
            CheckForSEOContent();
            WriteOg();

            base.OnActionExecuting(filterContext);
        }
    }
}
