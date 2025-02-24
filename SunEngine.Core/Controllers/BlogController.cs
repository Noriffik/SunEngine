using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SunEngine.Core.Cache.CacheModels;
using SunEngine.Core.Cache.Services;
using SunEngine.Core.Configuration.Options;
using SunEngine.Core.Presenters;
using SunEngine.Core.Security;
using SunEngine.Core.Utils.PagedList;

namespace SunEngine.Core.Controllers
{
    /// <summary>
    /// Get blog posts controller
    /// </summary>
    public class BlogController : BaseController
    {
        protected readonly OperationKeysContainer OperationKeys;

        protected readonly BlogOptions blogOptions;
        protected readonly ICategoriesCache categoriesCache;
        protected readonly IAuthorizationService authorizationService;
        protected readonly IBlogPresenter blogPresenter;


        public BlogController(IOptions<BlogOptions> blogOptions,
            IAuthorizationService authorizationService,
            ICategoriesCache categoriesCache,
            OperationKeysContainer operationKeysContainer,
            IBlogPresenter blogPresenter,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            OperationKeys = operationKeysContainer;

            this.blogOptions = blogOptions.Value;
            this.authorizationService = authorizationService;
            this.categoriesCache = categoriesCache;
            this.blogPresenter = blogPresenter;
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetPosts(string categoryName, int page = 1, bool showDeleted = false)
        {
            var category = categoriesCache.GetCategory(categoryName);

            if (category == null)
                return BadRequest();

            if (!authorizationService.HasAccess(User.Roles, category, OperationKeys.MaterialAndCommentsRead))
                return Unauthorized();

            
            var options = new MaterialsShowOptions
            {
                CategoryId = category.Id, 
                Page = page, 
                PageSize = blogOptions.PostsPageSize
            };

            if (authorizationService.HasAccess(User.Roles, category, OperationKeys.MaterialHide))
                options.ShowHidden = true;
            
            if (showDeleted && authorizationService.HasAccess(User.Roles, category, OperationKeys.MaterialDeleteAny))
                options.ShowDeleted = true;
            
            async Task<IPagedList<PostView>> LoadDataAsync()
            {
                return await blogPresenter.GetPostsAsync(options);
            }

            return await CacheContentAsync(category, category.Id, LoadDataAsync, page);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetPostsFromMultiCategories(string categoriesNames,
            int page = 1, int? pageSize = null)
        {
            var materialsCategoriesDic = categoriesCache.GetAllCategoriesIncludeSub(categoriesNames);

            IList<CategoryCached> categoriesList = authorizationService.GetAllowedCategories(User.Roles,
                materialsCategoriesDic.Values, OperationKeys.MaterialAndCommentsRead);

            if (categoriesList.Count == 0)
                return BadRequest("No categories to show");

            var options = new MaterialsMultiCatShowOptions()
            {
                CategoriesIds = categoriesList.Select(x => x.Id),
                Page = page, 
                PageSize = pageSize ?? blogOptions.PostsPageSize
            };

            var rez = await blogPresenter.GetPostsFromMultiCategoriesAsync(options);

            return Json(rez);

            /*async Task<IPagedList<PostViewModel>> LoadDataAsync()
            {
               return await blogPresenter.GetPostsFromMultiCategoriesAsync(categoriesIds, page, blogOptions.PostsPageSize);
            }

            var blogCategory = categoriesCache.GetCategory(categoriesNames);
            return await CacheContentAsync(blogCategory, categoriesIds, LoadDataAsync);*/
        }
    }
}
