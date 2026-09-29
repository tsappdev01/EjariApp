using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using DIP.Permissions;
using DIP.PressReleases;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using DIP.LastEventss;

namespace DIP.PressReleases
{

    public partial class PressReleasesAppService 
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<PressReleaseFrontEnd>> GetViewListAsync(GetPressReleasesInput input)
        {
            var totalCount = await _pressReleaseRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.DateMin, input.DateMax, input.IsActive);
            var items = await _pressReleaseRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.DateMin, input.DateMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PressReleaseFrontEnd>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PressRelease>, List<PressReleaseFrontEnd>>(items)
            };
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<PressReleaseFrontEnd>> GetListFrontEndAsync(GetPressReleasesInput input)
        {
            var items = await _pressReleaseRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.DateMin, input.DateMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);
            return ObjectMapper.Map<List<PressRelease>, List<PressReleaseFrontEnd>>(items);
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PressReleaseFrontEnd> GetBySlugAsync(string slug)
        {
            var items = await _pressReleaseRepository.GetBySlugAsync(slug);

            return ObjectMapper.Map<PressRelease, PressReleaseFrontEnd>(items);

        }
    }
}