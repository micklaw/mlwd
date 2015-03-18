﻿using Examine;
using System;
using MLWD.Umbraco.Context;
using MLWD.Umbraco.Umbraco.Services.Container;
using Umbraco.Core;
using umbraco.NodeFactory;

namespace MLWD.Umbraco.Umbraco.Events
{
    public class ExamineEvents : IApplicationEventHandler
    {
        private static object _lockObj = new object();
        private static bool _ran = false; 

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //throw new NotImplementedException();
        }

        void ExamineEventsGatheringNodeData(object sender, IndexingNodeDataEventArgs e)
        {
            var app = new CoreApp<CoreServiceContainer>();

            var node = new Node(e.NodeId);

            if (node.Id > 0)
            {
                var parentId = (node.Parent != null) ? node.Parent.Id : 0;

                e.Fields.Add("SystemParentId", parentId.ToString());

                // NOTE: Examine prepends __Sort_ to a column name
                e.Fields.Add("SystemSortOrder", node.SortOrder.ToString().PadLeft(12, '0'));

                bool exists;
                var publishDate = node.GetProperty("publishDate", out exists);
                if (exists)
                {
                    e.Fields.Add("SystemPublishDate", publishDate.Value);
                }

                exists = false;
                var datePublished = node.GetProperty("datePublished", out exists);
                if (!exists)
                {
                    datePublished = node.GetProperty("published", out exists);
                }
                if (exists)
                {
                    DateTime date;
                    if (DateTime.TryParse(datePublished.Value, out date))
                    {
                        e.Fields.Add("ContentDatePublished", date.ToString("ddMMyyyyHHmmss"));
                    }
                }

                var tags = node.GetProperty("blogTags", out exists);
                if (exists)
                {
                    e.Fields.Add("SystemBlogTags", tags.Value.Replace(",", " "));
                }

                var culture = app.Services.Content.GetCulture(node.Id);

                if (string.IsNullOrWhiteSpace(culture))
                {
                    culture = "en-GB";
                }

                e.Fields.Add("SystemCulture", culture.Replace("-", "").ToLower());
            }
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // lock - taken from: http://our.umbraco.org/wiki/reference/api-cheatsheet/using-iapplicationeventhandler-to-register-events
            if (!_ran)
            {
                lock (_lockObj)
                {
                    if (!_ran)
                    {
                        ExamineManager.Instance.IndexProviderCollection[Constants.Examine.MainExamineIndexProvider].GatheringNodeData += ExamineEventsGatheringNodeData;
                        _ran = true;
                    }
                }
            }
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //throw new NotImplementedException();
        }
    }
}