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
using DIP.SliderHomePages;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.SliderHomePages
{

    [Authorize(DIPPermissions.SliderHomePages.Default)]
    public partial class SliderHomePagesAppService : ApplicationService, ISliderHomePagesAppService
    {
        private readonly IDistributedCache<SliderHomePageExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ISliderHomePageRepository _sliderHomePageRepository;
        private readonly SliderHomePageManager _sliderHomePageManager;

        public SliderHomePagesAppService(ISliderHomePageRepository sliderHomePageRepository, SliderHomePageManager sliderHomePageManager, IDistributedCache<SliderHomePageExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _sliderHomePageRepository = sliderHomePageRepository;
            _sliderHomePageManager = sliderHomePageManager;
        }

        public virtual async Task<PagedResultDto<SliderHomePageDto>> GetListAsync(GetSliderHomePagesInput input)
        {
            var totalCount = await _sliderHomePageRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.ButtonTitleAr, input.ButtonTitleEn, input.ButtonUrlEn, input.ButtonUrlAr, input.Image, input.YoutubeUrl, input.IsActive, input.OrderMin, input.OrderMax);
            var items = await _sliderHomePageRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.ButtonTitleAr, input.ButtonTitleEn, input.ButtonUrlEn, input.ButtonUrlAr, input.Image, input.YoutubeUrl, input.IsActive, input.OrderMin, input.OrderMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SliderHomePageDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SliderHomePage>, List<SliderHomePageDto>>(items)
            };
        }

        public virtual async Task<SliderHomePageDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SliderHomePage, SliderHomePageDto>(await _sliderHomePageRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.SliderHomePages.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _sliderHomePageRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.SliderHomePages.Create)]
        public virtual async Task<SliderHomePageDto> CreateAsync(SliderHomePageCreateDto input)
        {

            var sliderHomePage = await _sliderHomePageManager.CreateAsync(
            input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.IsActive, input.Order, input.ButtonTitleAr, input.ButtonTitleEn, input.ButtonUrlEn, input.ButtonUrlAr, input.Image, input.YoutubeUrl
            );

            return ObjectMapper.Map<SliderHomePage, SliderHomePageDto>(sliderHomePage);
        }

        [Authorize(DIPPermissions.SliderHomePages.Edit)]
        public virtual async Task<SliderHomePageDto> UpdateAsync(Guid id, SliderHomePageUpdateDto input)
        {

            var sliderHomePage = await _sliderHomePageManager.UpdateAsync(
            id,
            input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.IsActive, input.Order, input.ButtonTitleAr, input.ButtonTitleEn, input.ButtonUrlEn, input.ButtonUrlAr, input.Image, input.YoutubeUrl, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SliderHomePage, SliderHomePageDto>(sliderHomePage);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SliderHomePageExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _sliderHomePageRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.ButtonTitleAr, input.ButtonTitleEn, input.ButtonUrlEn, input.ButtonUrlAr, input.Image, input.YoutubeUrl, input.IsActive, input.OrderMin, input.OrderMax);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<SliderHomePage>, List<SliderHomePageExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "SliderHomePages.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DIP.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new SliderHomePageExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DIP.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}