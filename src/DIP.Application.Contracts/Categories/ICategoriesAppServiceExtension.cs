using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using DIP.Amenities;
using System.Collections.Generic;

namespace DIP.Categories
{
    public partial interface ICategoriesAppService
    {
        Task<List<CategoryFrontEnd>> GetListFrontEndAsync(GetCategoriesInput input);

        Task<List<CategorySubCategoryLookUp>> GetCategorySubcategoryLookupAsync();


    }
}