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
using DIP.SupportedBanks;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.SupportedBanks
{

    [Authorize(DIPPermissions.SupportedBanks.Default)]
    public partial class SupportedBanksAppService : ApplicationService, ISupportedBanksAppService
    {
        private readonly IDistributedCache<SupportedBankExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ISupportedBankRepository _supportedBankRepository;
        private readonly SupportedBankManager _supportedBankManager;

        public SupportedBanksAppService(ISupportedBankRepository supportedBankRepository, SupportedBankManager supportedBankManager, IDistributedCache<SupportedBankExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _supportedBankRepository = supportedBankRepository;
            _supportedBankManager = supportedBankManager;
        }

        public virtual async Task<PagedResultDto<SupportedBankDto>> GetListAsync(GetSupportedBanksInput input)
        {
            var totalCount = await _supportedBankRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.IsActive, input.OrderMin, input.OrderMax);
            var items = await _supportedBankRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.IsActive, input.OrderMin, input.OrderMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SupportedBankDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SupportedBank>, List<SupportedBankDto>>(items)
            };
        }

        public virtual async Task<SupportedBankDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SupportedBank, SupportedBankDto>(await _supportedBankRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.SupportedBanks.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _supportedBankRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.SupportedBanks.Create)]
        public virtual async Task<SupportedBankDto> CreateAsync(SupportedBankCreateDto input)
        {

            var supportedBank = await _supportedBankManager.CreateAsync(
            input.TitleAr, input.TitleEn, input.IsActive, input.Order
            );

            return ObjectMapper.Map<SupportedBank, SupportedBankDto>(supportedBank);
        }

        [Authorize(DIPPermissions.SupportedBanks.Edit)]
        public virtual async Task<SupportedBankDto> UpdateAsync(Guid id, SupportedBankUpdateDto input)
        {

            var supportedBank = await _supportedBankManager.UpdateAsync(
            id,
            input.TitleAr, input.TitleEn, input.IsActive, input.Order, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SupportedBank, SupportedBankDto>(supportedBank);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SupportedBankExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _supportedBankRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.IsActive, input.OrderMin, input.OrderMax);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<SupportedBank>, List<SupportedBankExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "SupportedBanks.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new SupportedBankExcelDownloadTokenCacheItem { Token = token },
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