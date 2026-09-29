using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using DIP.Categories;
using System.Collections.Generic;

namespace DIP.SubCategories
{
    public partial interface ISubCategoriesAppService
    {
        Task<List<SubCategoryFrontEnd>> GetListFrontEndAsync(GetSubCategoriesInput input);
    }
}