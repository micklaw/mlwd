﻿using System.Web;
using MLWD.Umbraco.Mvc.Controllers.App;
using MLWD.Umbraco.Mvc.Model;
using MLWD.Umbraco.Mvc.Model.Content;

namespace Website.Domain.Shared.Controllers
{
    public class AppController : BaseCMSController
    {
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
            if (Node != null)
            {
                var node = Node as Page;

                if (node != null)
                {
                    ViewBag.Canonical = App.Context.DomainUrl + node.Url;
                }
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
