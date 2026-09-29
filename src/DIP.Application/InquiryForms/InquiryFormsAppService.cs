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
using DIP.InquiryForms;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.InquiryForms
{

    [Authorize(DIPPermissions.InquiryForms.Default)]
    public partial class InquiryFormsAppService : ApplicationService, IInquiryFormsAppService
    {
        private readonly IDistributedCache<InquiryFormExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IInquiryFormRepository _inquiryFormRepository;
        private readonly InquiryFormManager _inquiryFormManager;

        public InquiryFormsAppService(IInquiryFormRepository inquiryFormRepository, InquiryFormManager inquiryFormManager, IDistributedCache<InquiryFormExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _inquiryFormRepository = inquiryFormRepository;
            _inquiryFormManager = inquiryFormManager;
        }

        public virtual async Task<PagedResultDto<InquiryFormDto>> GetListAsync(GetInquiryFormsInput input)
        {
            var totalCount = await _inquiryFormRepository.GetCountAsync(input.FilterText, input.CompanyName, input.Name, input.Email, input.Mobile, input.Fax, input.Phone, input.InquiryType, input.TradeLicensePlateOfIssue, input.BuyRent, input.SpaceInSquareFeetMin, input.SpaceInSquareFeetMax, input.Comments);
            var items = await _inquiryFormRepository.GetListAsync(input.FilterText, input.CompanyName, input.Name, input.Email, input.Mobile, input.Fax, input.Phone, input.InquiryType, input.TradeLicensePlateOfIssue, input.BuyRent, input.SpaceInSquareFeetMin, input.SpaceInSquareFeetMax, input.Comments, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<InquiryFormDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<InquiryForm>, List<InquiryFormDto>>(items)
            };
        }

        public virtual async Task<InquiryFormDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<InquiryForm, InquiryFormDto>(await _inquiryFormRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.InquiryForms.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _inquiryFormRepository.DeleteAsync(id);
        }

        //[Authorize(DIPPermissions.InquiryForms.Create)]
        //public virtual async Task<InquiryFormDto> CreateAsync(InquiryFormCreateDto input)
        //{

        //    var inquiryForm = await _inquiryFormManager.CreateAsync(
        //    input.CompanyName, input.Name, input.Email, input.Mobile, input.Fax, input.Phone, input.InquiryType, input.TradeLicensePlateOfIssue, input.BuyRent, input.SpaceInSquareFeet, input.Comments
        //    );

        //    return ObjectMapper.Map<InquiryForm, InquiryFormDto>(inquiryForm);
        //}

        [Authorize(DIPPermissions.InquiryForms.Edit)]
        public virtual async Task<InquiryFormDto> UpdateAsync(Guid id, InquiryFormUpdateDto input)
        {

            var inquiryForm = await _inquiryFormManager.UpdateAsync(
            id,
            input.CompanyName, input.Name, input.Email, input.Mobile, input.Fax, input.Phone, input.InquiryType, input.TradeLicensePlateOfIssue, input.BuyRent, input.SpaceInSquareFeet, input.Comments, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<InquiryForm, InquiryFormDto>(inquiryForm);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(InquiryFormExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _inquiryFormRepository.GetListAsync(input.FilterText, input.CompanyName, input.Name, input.Email, input.Mobile, input.Fax, input.Phone, input.InquiryType, input.TradeLicensePlateOfIssue, input.BuyRent, input.SpaceInSquareFeetMin, input.SpaceInSquareFeetMax, input.Comments);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<InquiryForm>, List<InquiryFormExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "InquiryForms.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new InquiryFormExcelDownloadTokenCacheItem { Token = token },
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