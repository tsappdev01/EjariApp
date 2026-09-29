using DIP.Shared;
using DIP.MediaGalleries;
using DIP.AmenityParagraphs;
using DIP.ZoneParagraphs;
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
using DIP.Medias;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using Volo.Abp.BlobStoring;

namespace DIP.Medias
{

    [Authorize(DIPPermissions.Medias.Default)]
    public partial class MediasAppService : ApplicationService, IMediasAppService
    {
        private readonly IDistributedCache<MediaExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IMediaRepository _mediaRepository;
        private readonly MediaManager _mediaManager;
        private IBlobContainer<MediaContainer> _mediaContainer;
        private readonly IRepository<ZoneParagraph, Guid> _zoneParagraphRepository;
        private readonly IRepository<AmenityParagraph, Guid> _amenityParagraphRepository;
        private readonly IRepository<MediaGallery, Guid> _mediaGalleryRepository;

        public MediasAppService(IMediaRepository mediaRepository, MediaManager mediaManager, IBlobContainer<MediaContainer> mediaContainer, IDistributedCache<MediaExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<ZoneParagraph, Guid> zoneParagraphRepository, IRepository<AmenityParagraph, Guid> amenityParagraphRepository, IRepository<MediaGallery, Guid> mediaGalleryRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _mediaRepository = mediaRepository;
            _mediaManager = mediaManager;
            _mediaContainer = mediaContainer;
            _zoneParagraphRepository = zoneParagraphRepository;
            _amenityParagraphRepository = amenityParagraphRepository;
            _mediaGalleryRepository = mediaGalleryRepository;
        }

        public virtual async Task<PagedResultDto<MediaWithNavigationPropertiesDto>> GetListAsync(GetMediasInput input)
        {
            var totalCount = await _mediaRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.ZoneParagraphId, input.AmenityParagraphId, input.MediaGalleryId);
            var items = await _mediaRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.File, input.OrderMin, input.OrderMax, input.IsActive, input.ZoneParagraphId, input.AmenityParagraphId, input.MediaGalleryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MediaWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MediaWithNavigationProperties>, List<MediaWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<MediaWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<MediaWithNavigationProperties, MediaWithNavigationPropertiesDto>
                (await _mediaRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<MediaDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Media, MediaDto>(await _mediaRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetZoneParagraphLookupAsync(LookupRequestDto input)
        {
            var query = (await _zoneParagraphRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<ZoneParagraph>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ZoneParagraph>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetAmenityParagraphLookupAsync(LookupRequestDto input)
        {
            var query = (await _amenityParagraphRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<AmenityParagraph>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<AmenityParagraph>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMediaGalleryLookupAsync(LookupRequestDto input)
        {
            var query = (await _mediaGalleryRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.TitleEn != null &&
                         x.TitleEn.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<MediaGallery>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MediaGallery>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(DIPPermissions.Medias.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _mediaRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.Medias.Create)]
        public virtual async Task<MediaDto> CreateAsync(MediaCreateDto input)
        {

            var media = await _mediaManager.CreateAsync(
            input.ZoneParagraphId, input.AmenityParagraphId, input.MediaGalleryId, input.TitleEn, input.TitleAr, input.File, input.Order, input.IsActive
            );

            return ObjectMapper.Map<Media, MediaDto>(media);
        }

        [Authorize(DIPPermissions.Medias.Edit)]
        public virtual async Task<MediaDto> UpdateAsync(Guid id, MediaUpdateDto input)
        {

            var media = await _mediaManager.UpdateAsync(
            id,
            input.ZoneParagraphId, input.AmenityParagraphId, input.MediaGalleryId, input.TitleEn, input.TitleAr, input.File, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Media, MediaDto>(media);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MediaExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var medias = await _mediaRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.File, input.OrderMin, input.OrderMax, input.IsActive);
            var items = medias.Select(item => new
            {
                TitleEn = item.Media.TitleEn,
                TitleAr = item.Media.TitleAr,
                File = item.Media.File,
                Order = item.Media.Order,
                IsActive = item.Media.IsActive,

                ZoneParagraph = item.ZoneParagraph?.TitleEn,
                AmenityParagraph = item.AmenityParagraph?.TitleEn,
                MediaGallery = item.MediaGallery?.TitleEn,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Medias.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new MediaExcelDownloadTokenCacheItem { Token = token },
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