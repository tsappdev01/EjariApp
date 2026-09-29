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
using DIP.FeedBacks;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.FeedBacks
{

    [Authorize(DIPPermissions.FeedBacks.Default)]
    public partial class FeedBacksAppService : ApplicationService, IFeedBacksAppService
    {
        private readonly IDistributedCache<FeedBackExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IFeedBackRepository _feedBackRepository;
        private readonly FeedBackManager _feedBackManager;

        public FeedBacksAppService(IFeedBackRepository feedBackRepository, FeedBackManager feedBackManager, IDistributedCache<FeedBackExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _feedBackRepository = feedBackRepository;
            _feedBackManager = feedBackManager;
        }

        public virtual async Task<PagedResultDto<FeedBackDto>> GetListAsync(GetFeedBacksInput input)
        {
            var totalCount = await _feedBackRepository.GetCountAsync(input.FilterText, input.Subject, input.CompanyName, input.PlotNo, input.PlotCategory, input.ContactPersonName, input.EmailId, input.MobileNumber, input.Department, input.CategoryName, input.Description);
            var items = await _feedBackRepository.GetListAsync(input.FilterText, input.Subject, input.CompanyName, input.PlotNo, input.PlotCategory, input.ContactPersonName, input.EmailId, input.MobileNumber, input.Department, input.CategoryName, input.Description, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<FeedBackDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<FeedBack>, List<FeedBackDto>>(items)
            };
        }

        public virtual async Task<FeedBackDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<FeedBack, FeedBackDto>(await _feedBackRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.FeedBacks.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _feedBackRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.FeedBacks.Edit)]
        public virtual async Task<FeedBackDto> UpdateAsync(Guid id, FeedBackUpdateDto input)
        {

            var feedBack = await _feedBackManager.UpdateAsync(
            id,
            input.Subject, input.CompanyName, input.PlotNo, input.PlotCategory, input.ContactPersonName, input.EmailId, input.MobileNumber, input.Department, input.CategoryName, input.Description, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<FeedBack, FeedBackDto>(feedBack);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(FeedBackExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _feedBackRepository.GetListAsync(input.FilterText, input.Subject, input.CompanyName, input.PlotNo, input.PlotCategory, input.ContactPersonName, input.EmailId, input.MobileNumber, input.Department, input.CategoryName, input.Description);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<FeedBack>, List<FeedBackExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "FeedBacks.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new FeedBackExcelDownloadTokenCacheItem { Token = token },
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