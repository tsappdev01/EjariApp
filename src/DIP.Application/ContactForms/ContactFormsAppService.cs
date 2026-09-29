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
using DIP.ContactForms;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.ContactForms
{

    [Authorize(DIPPermissions.ContactForms.Default)]
    public partial class ContactFormsAppService : ApplicationService, IContactFormsAppService
    {
        private readonly IDistributedCache<ContactFormExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly IContactFormRepository _contactFormRepository;
        private readonly ContactFormManager _contactFormManager;

        public ContactFormsAppService(IContactFormRepository contactFormRepository, ContactFormManager contactFormManager, IDistributedCache<ContactFormExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _contactFormRepository = contactFormRepository;
            _contactFormManager = contactFormManager;
        }

        public virtual async Task<PagedResultDto<ContactFormDto>> GetListAsync(GetContactFormsInput input)
        {
            var totalCount = await _contactFormRepository.GetCountAsync(input.FilterText, input.FullName, input.Email, input.Subject, input.Message);
            var items = await _contactFormRepository.GetListAsync(input.FilterText, input.FullName, input.Email, input.Subject, input.Message, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ContactFormDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ContactForm>, List<ContactFormDto>>(items)
            };
        }

        public virtual async Task<ContactFormDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ContactForm, ContactFormDto>(await _contactFormRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.ContactForms.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _contactFormRepository.DeleteAsync(id);
        }


        [Authorize(DIPPermissions.ContactForms.Edit)]
        public virtual async Task<ContactFormDto> UpdateAsync(Guid id, ContactFormUpdateDto input)
        {

            var contactForm = await _contactFormManager.UpdateAsync(
            id,
            input.FullName, input.Email, input.Subject, input.Message, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<ContactForm, ContactFormDto>(contactForm);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ContactFormExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _contactFormRepository.GetListAsync(input.FilterText, input.FullName, input.Email, input.Subject, input.Message);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<ContactForm>, List<ContactFormExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ContactForms.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ContactFormExcelDownloadTokenCacheItem { Token = token },
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