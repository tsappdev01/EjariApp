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
using DIP.DipBranches;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.DipBranches
{

    [Authorize(DIPPermissions.DipBranches.Default)]
    public partial class DipBranchesAppService : ApplicationService, IDipBranchesAppService
    {
        private readonly IDistributedCache<DipBranchExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IDipBranchRepository _dipBranchRepository;
        private readonly DipBranchManager _dipBranchManager;

        public DipBranchesAppService(IDipBranchRepository dipBranchRepository, DipBranchManager dipBranchManager, IDistributedCache<DipBranchExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _dipBranchRepository = dipBranchRepository;
            _dipBranchManager = dipBranchManager;
        }

        public virtual async Task<PagedResultDto<DipBranchDto>> GetListAsync(GetDipBranchesInput input)
        {
            var totalCount = await _dipBranchRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.Phone, input.AlternativePhone, input.Email, input.AlternativeEmail, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _dipBranchRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.Phone, input.AlternativePhone, input.Email, input.AlternativeEmail, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<DipBranchDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<DipBranch>, List<DipBranchDto>>(items)
            };
        }

        public virtual async Task<DipBranchDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<DipBranch, DipBranchDto>(await _dipBranchRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.DipBranches.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _dipBranchRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.DipBranches.Create)]
        public virtual async Task<DipBranchDto> CreateAsync(DipBranchCreateDto input)
        {

            var dipBranch = await _dipBranchManager.CreateAsync(
            input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.Phone, input.AlternativePhone, input.Email, input.AlternativeEmail, input.Order, input.IsActive
            );

            return ObjectMapper.Map<DipBranch, DipBranchDto>(dipBranch);
        }

        [Authorize(DIPPermissions.DipBranches.Edit)]
        public virtual async Task<DipBranchDto> UpdateAsync(Guid id, DipBranchUpdateDto input)
        {

            var dipBranch = await _dipBranchManager.UpdateAsync(
            id,
            input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.Phone, input.AlternativePhone, input.Email, input.AlternativeEmail, input.Order, input.IsActive, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<DipBranch, DipBranchDto>(dipBranch);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DipBranchExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _dipBranchRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.SubTitleEn, input.SubTitleAr, input.Phone, input.AlternativePhone, input.Email, input.AlternativeEmail, input.OrderMin, input.OrderMax, input.IsActive);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<DipBranch>, List<DipBranchExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "DipBranches.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new DipBranchExcelDownloadTokenCacheItem { Token = token },
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