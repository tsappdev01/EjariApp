
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using System.Collections.Generic;
using DIP.SubCategories;
using Volo.Abp.ObjectMapping;
using System.Linq;

namespace DIP.Categories
{
    public partial class CategoriesAppService
    {

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<CategoryFrontEnd>> GetListFrontEndAsync(GetCategoriesInput input)
        {
            var items = await _categoryRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.OrderMin, input.OrderMax, input.IsFeature, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<List<Category>, List<CategoryFrontEnd>>(items);
        }        
        
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<CategorySubCategoryLookUp>> GetCategorySubcategoryLookupAsync()
        {
            var items = await _categoryRepository.GetListAsync(isFeature: true, isActive: true, skipCount: 0, maxResultCount: 1000);
            var itemsSubCateory = await _subCategoryRepository.GetListWithNavigationPropertiesAsync(isFeature: true, isActive: true, skipCount: 0, maxResultCount: 1000);

            var result = new List<CategorySubCategoryLookUp>();
            if (items != null && items.Count > 0)
                result = ObjectMapper.Map<List<Category>, List<CategorySubCategoryLookUp>>(items);
            if (itemsSubCateory != null && itemsSubCateory.Count > 0)
                result.AddRange(ObjectMapper.Map<List<SubCategoryWithNavigationProperties>, List<CategorySubCategoryLookUp>>(itemsSubCateory));
             return result;

        }
    }
}