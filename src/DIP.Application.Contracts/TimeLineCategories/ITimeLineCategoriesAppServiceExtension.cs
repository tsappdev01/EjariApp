using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;

namespace DIP.TimeLineCategories
{
    public partial interface ITimeLineCategoriesAppService : IApplicationService
    {
        Task<List<TimeLineCategoryFrontEnd>> GetListWithDetailsFrontEndAsync();
    }
}