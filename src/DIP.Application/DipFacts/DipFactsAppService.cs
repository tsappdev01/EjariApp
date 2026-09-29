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
using DIP.DipFacts;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.DipFacts
{

    [Authorize(DIPPermissions.DipFacts.Default)]
    public partial class DipFactsAppService : ApplicationService, IDipFactsAppService
    {
        private readonly IDistributedCache<DipFactExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IDipFactRepository _dipFactRepository;
        private readonly DipFactManager _dipFactManager;

        public DipFactsAppService(IDipFactRepository dipFactRepository, DipFactManager dipFactManager, IDistributedCache<DipFactExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _dipFactRepository = dipFactRepository;
            _dipFactManager = dipFactManager;
        }

        public virtual async Task<PagedResultDto<DipFactDto>> GetListAsync(GetDipFactsInput input)
        {
            var totalCount = await _dipFactRepository.GetCountAsync(input.FilterText, input.Image, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _dipFactRepository.GetListAsync(input.FilterText, input.Image, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<DipFactDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<DipFact>, List<DipFactDto>>(items)
            };
        }

        public virtual async Task<DipFactDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<DipFact, DipFactDto>(await _dipFactRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.DipFacts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _dipFactRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.DipFacts.Create)]
        public virtual async Task<DipFactDto> CreateAsync(DipFactCreateDto input)
        {

            var dipFact = await _dipFactManager.CreateAsync(
            input.Image, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.Order, input.IsActive
            );

            return ObjectMapper.Map<DipFact, DipFactDto>(dipFact);
        }

        [Authorize(DIPPermissions.DipFacts.Edit)]
        public virtual async Task<DipFactDto> UpdateAsync(Guid id, DipFactUpdateDto input)
        {

            var dipFact = await _dipFactManager.UpdateAsync(
            id,
            input.Image, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<DipFact, DipFactDto>(dipFact);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DipFactExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _dipFactRepository.GetListAsync(input.FilterText, input.Image, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<DipFact>, List<DipFactExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "DipFacts.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new DipFactExcelDownloadTokenCacheItem { Token = token },
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