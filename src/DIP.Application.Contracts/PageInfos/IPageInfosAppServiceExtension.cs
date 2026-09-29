using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;

namespace DIP.PageInfos
{
    public partial interface IPageInfosAppService
    {

        Task<PageInfoFrontEnd> GetBySlugAsync(string slug);

        Task<List<PageInfoHeaderFrontEnd>> GetPageInfoLookupBySlugsAsync(List<string> slugs);
    }
}