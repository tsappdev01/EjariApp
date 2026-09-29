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
using DIP.EServices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace DIP.EServices
{

    [Authorize(DIPPermissions.EServices.Default)]
    public partial class EServicesAppService : ApplicationService, IEServicesAppService
    {
        private readonly IDistributedCache<EServiceExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IEServiceRepository _eServiceRepository;
        private readonly EServiceManager _eServiceManager;

        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly HttpContent _httpContent;
        //private readonly IHttpContextFactory _httpContext;


        public EServicesAppService(IEServiceRepository eServiceRepository, EServiceManager eServiceManager, IDistributedCache<EServiceExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _eServiceRepository = eServiceRepository;
            _eServiceManager = eServiceManager;
            //_httpContextAccessor = httpContextAccessor;
            //_httpContent = httpContent;
            //_httpContext = httpContext;
        }

        public virtual async Task<PagedResultDto<EServiceDto>> GetListAsync(GetEServicesInput input)
        {
            var totalCount = await _eServiceRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Slug, input.DescriptionEn, input.DescriptionAr, input.Image, input.HeaderImage, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _eServiceRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Slug, input.DescriptionEn, input.DescriptionAr, input.Image, input.HeaderImage, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<EServiceDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EService>, List<EServiceDto>>(items)
            };
        }

        public virtual async Task<EServiceDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<EService, EServiceDto>(await _eServiceRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.EServices.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _eServiceRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.EServices.Create)]
        public virtual async Task<EServiceDto> CreateAsync(EServiceCreateDto input)
        {

            var eService = await _eServiceManager.CreateAsync(
            input.TitleEn, input.TitleAr, input.Slug, input.DescriptionEn, input.DescriptionAr, input.Image, input.HeaderImage, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Order, input.IsActive
            );

            return ObjectMapper.Map<EService, EServiceDto>(eService);
        }

        [Authorize(DIPPermissions.EServices.Edit)]
        public virtual async Task<EServiceDto> UpdateAsync(Guid id, EServiceUpdateDto input)
        {

            var eService = await _eServiceManager.UpdateAsync(
            id,
            input.TitleEn, input.TitleAr, input.Slug, input.DescriptionEn, input.DescriptionAr, input.Image, input.HeaderImage, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<EService, EServiceDto>(eService);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(EServiceExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _eServiceRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Slug, input.DescriptionEn, input.DescriptionAr, input.Image, input.HeaderImage, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<EService>, List<EServiceExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "EServices.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new EServiceExcelDownloadTokenCacheItem { Token = token },
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