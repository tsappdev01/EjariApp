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
using DIP.MediaGalleries;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.MediaGalleries
{

    [Authorize(DIPPermissions.MediaGalleries.Default)]
    public partial class MediaGalleriesAppService : ApplicationService, IMediaGalleriesAppService
    {
        private readonly IDistributedCache<MediaGalleryExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IMediaGalleryRepository _mediaGalleryRepository;
        private readonly MediaGalleryManager _mediaGalleryManager;

        public MediaGalleriesAppService(IMediaGalleryRepository mediaGalleryRepository, MediaGalleryManager mediaGalleryManager, IDistributedCache<MediaGalleryExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _mediaGalleryRepository = mediaGalleryRepository;
            _mediaGalleryManager = mediaGalleryManager;
        }

        public virtual async Task<PagedResultDto<MediaGalleryDto>> GetListAsync(GetMediaGalleriesInput input)
        {
            var totalCount = await _mediaGalleryRepository.GetCountAsync(input.FilterText, input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _mediaGalleryRepository.GetListAsync(input.FilterText, input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MediaGalleryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MediaGallery>, List<MediaGalleryDto>>(items)
            };
        }

        public virtual async Task<MediaGalleryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<MediaGallery, MediaGalleryDto>(await _mediaGalleryRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.MediaGalleries.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _mediaGalleryRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.MediaGalleries.Create)]
        public virtual async Task<MediaGalleryDto> CreateAsync(MediaGalleryCreateDto input)
        {

            var mediaGallery = await _mediaGalleryManager.CreateAsync(
            input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.Order, input.IsActive
            );

            return ObjectMapper.Map<MediaGallery, MediaGalleryDto>(mediaGallery);
        }

        [Authorize(DIPPermissions.MediaGalleries.Edit)]
        public virtual async Task<MediaGalleryDto> UpdateAsync(Guid id, MediaGalleryUpdateDto input)
        {

            var mediaGallery = await _mediaGalleryManager.UpdateAsync(
            id,
            input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<MediaGallery, MediaGalleryDto>(mediaGallery);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaGalleryExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _mediaGalleryRepository.GetListAsync(input.FilterText, input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<MediaGallery>, List<MediaGalleryExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "MediaGalleries.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new MediaGalleryExcelDownloadTokenCacheItem { Token = token },
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