﻿using System.Web.Mvc;
using Website.Domain.Blog.DocTypes;
using Website.Domain.Blog.ViewModels;
using Website.Domain.Shared.Search;

namespace Website.Domain.Blog.Controllers
{
    public class BlogController : BaseBlogController
    {
        public ActionResult Index(int? p, string c, string t, string k)
        {
            var content = Node as BlogListing;

            var searchCrieria = SearchCriteria.WithBlogCategory(c)
                                              .AndBlogTag(t)
                                              .AndBlogKeyword(k)
                                              .AndPaging(FixPage(p), content.BlogPageCount)
                                              .OrderBy("ContentDatePublished", true);

            var model = new BlogViewModel
            {
                Content = content,
                Blogs = App.Services.Content.Get<BlogDetails>(searchCrieria)
            };

            return View(model);
        }

        public ActionResult Details()
        {
            var model = new BlogDetailsViewModel()
            {
                Content = Node as BlogDetails
            };

            return View(model);
        }
    }
}
