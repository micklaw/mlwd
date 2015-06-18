﻿using System.Linq;
using System.Web;
using System.Web.Http;
using Umbraco.Core.Models.PublishedContent;
using Yomego.Umbraco.Collections;
using Yomego.Umbraco.Mvc.Model;
using Yomego.Umbraco.Umbraco.Model;
using Yomego.Umbraco.Umbraco.Services.Search.Models;

namespace Yomego.Umbraco.Mvc.Controllers.CMS.Api
{
    public class DocumentApiController : BaseApiController
    {
        #region helpers

        private string[] GetTypesFromString(string typesDelimited)
        {
            if (!string.IsNullOrWhiteSpace(typesDelimited))
            {
                var items = typesDelimited.Split('|');

                return items.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToArray();
            }

            return null;
        }

        private TypedCriteria<Criteria> BuildSearchCriteria(SearchParams searchParams)
        {
            var criteria = Criteria.WithKeywords(searchParams.Keywords)
                                   .AndTypes(GetTypesFromString(searchParams.Types))
                                   .AndCulture(searchParams.Culture);

            if (searchParams.Descending)
            {
                criteria.OrderBy(searchParams.Order, true);
            }
            else
            {
                criteria.OrderBy(searchParams.Order);
            }

            if (searchParams.ParentId.HasValue)
            {
                criteria.AndParentId(searchParams.ParentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Custom))
            {
                var query = HttpUtility.ParseQueryString(searchParams.Custom);

                if (query.HasKeys())
                {
                    foreach (var item in query.AllKeys)
                    {
                        criteria.AddSearchItem(item, query[item]);
                    }
                }
            }

            return criteria;
        }

        private TypedCriteria<Criteria> BuildSearchCriteriaWithPaging(SearchParams searchParams, int? page = null, int? pageSize = null)
        {
            var criteria = BuildSearchCriteria(searchParams);

            int p = FixPage(page);

            criteria.AndPaging(p, pageSize ?? 10);

            return criteria;
        }

        #endregion helpers

        [HttpGet]
        public object Search([FromUri]SearchParams search, int? page = null, int? pageSize = null)
        {
            var criteria = BuildSearchCriteriaWithPaging(search, page, pageSize);

            var content = (App.Services.Content.Get<Content>(criteria) as PagedList<Content>) ?? new PagedList<Content>();

            var response = new ApiResultsModel
            {
                results = content,
                resultCount = content.Count,
                totalCount = content.TotalCount,
                page = content.PageIndex,
                pageSize = content.PageSize,
                hasPrevious = content.IsPreviousPage,
                hasNext = content.IsNextPage
            };

            return response;
        }

        [HttpGet]
        public PublishedContentModel First([FromUri]SearchParams search)
        {
            var criteria = BuildSearchCriteria(search);

            return App.Services.Content.First<PublishedContentModel>(criteria);
        }

        [HttpGet]
        public object Get(int id)
        {
            return App.Services.Content.Get(id);
        }

        [HttpGet]
        public object Get(string url)
        {
            return App.Services.Content.Get(url);
        }
    }
}
