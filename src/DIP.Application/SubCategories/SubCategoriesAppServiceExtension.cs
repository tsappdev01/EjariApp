using Microsoft.AspNetCore.Authorization;
using DIP.Permissions;
using DIP.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using System.Linq;

namespace DIP.SubCategories
{
    public partial class SubCategoriesAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<SubCategoryFrontEnd>> GetListFrontEndAsync(GetSubCategoriesInput input)
        {
            var items = await _subCategoryRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleAr, input.TitleEn, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive, input.CategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            if (items.Any())
            {
                return ObjectMapper.Map<List<SubCategory>, List<SubCategoryFrontEnd>>(items.Select(s => s.SubCategory).ToList());
            }
            return new List<SubCategoryFrontEnd>();
        }
    }
}