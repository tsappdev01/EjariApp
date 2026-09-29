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
using DIP.Amenities;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.Amenities
{

    [Authorize(DIPPermissions.Amenities.Default)]
    public partial class AmenitiesAppService : ApplicationService, IAmenitiesAppService
    {
        private readonly IDistributedCache<AmenityExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IAmenityRepository _amenityRepository;
        private readonly AmenityManager _amenityManager;

        public AmenitiesAppService(IAmenityRepository amenityRepository, AmenityManager amenityManager, IDistributedCache<AmenityExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _amenityRepository = amenityRepository;
            _amenityManager = amenityManager;
        }

        public virtual async Task<PagedResultDto<AmenityDto>> GetListAsync(GetAmenitiesInput input)
        {
            var totalCount = await _amenityRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _amenityRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<AmenityDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Amenity>, List<AmenityDto>>(items)
            };
        }

        public virtual async Task<AmenityDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Amenity, AmenityDto>(await _amenityRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.Amenities.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _amenityRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.Amenities.Create)]
        public virtual async Task<AmenityDto> CreateAsync(AmenityCreateDto input)
        {

            var amenity = await _amenityManager.CreateAsync(
            input.TitleEn, input.TitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.HeaderImage, input.Image, input.Order, input.IsActive
            );

            return ObjectMapper.Map<Amenity, AmenityDto>(amenity);
        }

        [Authorize(DIPPermissions.Amenities.Edit)]
        public virtual async Task<AmenityDto> UpdateAsync(Guid id, AmenityUpdateDto input)
        {

            var amenity = await _amenityManager.UpdateAsync(
            id,
            input.TitleEn, input.TitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.HeaderImage, input.Image, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Amenity, AmenityDto>(amenity);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AmenityExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _amenityRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Amenity>, List<AmenityExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Amenities.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new AmenityExcelDownloadTokenCacheItem { Token = token },
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