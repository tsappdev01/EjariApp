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

namespace DIP.PressReleases
{

    [Authorize(DIPPermissions.PressReleases.Default)]
    public partial class PressReleasesAppService : ApplicationService, IPressReleasesAppService
    {
        private readonly IDistributedCache<PressReleaseExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IPressReleaseRepository _pressReleaseRepository;
        private readonly PressReleaseManager _pressReleaseManager;

        public PressReleasesAppService(IPressReleaseRepository pressReleaseRepository, PressReleaseManager pressReleaseManager, IDistributedCache<PressReleaseExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _pressReleaseRepository = pressReleaseRepository;
            _pressReleaseManager = pressReleaseManager;
        }

        public virtual async Task<PagedResultDto<PressReleaseDto>> GetListAsync(GetPressReleasesInput input)
        {
            var totalCount = await _pressReleaseRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.DateMin, input.DateMax, input.IsActive);
            var items = await _pressReleaseRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.DateMin, input.DateMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PressReleaseDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<PressRelease>, List<PressReleaseDto>>(items)
            };
        }

        public virtual async Task<PressReleaseDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<PressRelease, PressReleaseDto>(await _pressReleaseRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.PressReleases.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _pressReleaseRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.PressReleases.Create)]
        public virtual async Task<PressReleaseDto> CreateAsync(PressReleaseCreateDto input)
        {

            var pressRelease = await _pressReleaseManager.CreateAsync(
            input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.Order, input.Date, input.IsActive
            );

            return ObjectMapper.Map<PressRelease, PressReleaseDto>(pressRelease);
        }

        [Authorize(DIPPermissions.PressReleases.Edit)]
        public virtual async Task<PressReleaseDto> UpdateAsync(Guid id, PressReleaseUpdateDto input)
        {

            var pressRelease = await _pressReleaseManager.UpdateAsync(
            id,
            input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.Order, input.Date, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<PressRelease, PressReleaseDto>(pressRelease);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PressReleaseExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _pressReleaseRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.DateMin, input.DateMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<PressRelease>, List<PressReleaseExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "PressReleases.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new PressReleaseExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}