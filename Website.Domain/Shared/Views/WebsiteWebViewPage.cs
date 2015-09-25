﻿using System;
using System.Web.Mvc;

namespace Website.Domain.Shared.Views
{
    public abstract class WebsiteWebViewPage<T> : WebViewPage<T>
    {
        private readonly Lazy<WebsiteApp> _lazyApp = new Lazy<WebsiteApp>(() => new WebsiteApp());

        protected WebsiteApp DomainApp
        {
            get
            {
                return _lazyApp.Value;
            }
        }

        public TimeSpan Subtract(DateTime date)
        {
            return DateTime.Now.Subtract(date);
        }

        public string Experience
        {
            get { return Math.Floor((double)Subtract(new DateTime(2005, 1, 1)).Days / 365) + "+"; }
        }

        public int Age
        {
            get { return (int)Math.Floor((double)Subtract(new DateTime(1981, 5, 7)).Days / 365); }
        }
    }
}
