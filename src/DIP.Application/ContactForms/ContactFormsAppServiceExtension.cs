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

    public partial class ContactFormsAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<ContactFormDto> CreateAsync(ContactFormCreateDto input)
        {

            var contactForm = await _contactFormManager.CreateAsync(
            input.FullName, input.Email, input.Subject, input.Message
            );

            return ObjectMapper.Map<ContactForm, ContactFormDto>(contactForm);
        }
    }
}