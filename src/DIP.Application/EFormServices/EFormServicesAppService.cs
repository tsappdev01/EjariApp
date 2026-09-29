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
using DIP.EFormServices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.EFormServices
{

    [Authorize(DIPPermissions.EFormServices.Default)]
    public partial class EFormServicesAppService : ApplicationService, IEFormServicesAppService
    {
        private readonly IDistributedCache<EFormServiceExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IEFormServiceRepository _eFormServiceRepository;
        private readonly EFormServiceManager _eFormServiceManager;

        public EFormServicesAppService(IEFormServiceRepository eFormServiceRepository, EFormServiceManager eFormServiceManager, IDistributedCache<EFormServiceExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _eFormServiceRepository = eFormServiceRepository;
            _eFormServiceManager = eFormServiceManager;
        }

        public virtual async Task<PagedResultDto<EFormServiceDto>> GetListAsync(GetEFormServicesInput input)
        {
            var totalCount = await _eFormServiceRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _eFormServiceRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<EFormServiceDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EFormService>, List<EFormServiceDto>>(items)
            };
        }

        public virtual async Task<EFormServiceDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<EFormService, EFormServiceDto>(await _eFormServiceRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.EFormServices.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _eFormServiceRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.EFormServices.Create)]
        public virtual async Task<EFormServiceDto> CreateAsync(EFormServiceCreateDto input)
        {

            var eFormService = await _eFormServiceManager.CreateAsync(
            input.TitleEn, input.TitleAr, input.Order, input.IsActive
            );

            return ObjectMapper.Map<EFormService, EFormServiceDto>(eFormService);
        }

        [Authorize(DIPPermissions.EFormServices.Edit)]
        public virtual async Task<EFormServiceDto> UpdateAsync(Guid id, EFormServiceUpdateDto input)
        {

            var eFormService = await _eFormServiceManager.UpdateAsync(
            id,
            input.TitleEn, input.TitleAr, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<EFormService, EFormServiceDto>(eFormService);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(EFormServiceExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _eFormServiceRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<EFormService>, List<EFormServiceExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "EFormServices.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new EFormServiceExcelDownloadTokenCacheItem { Token = token },
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