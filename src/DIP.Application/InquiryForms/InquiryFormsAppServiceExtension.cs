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

    
    public partial class InquiryFormsAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<InquiryFormDto> CreateAsync(InquiryFormCreateDto input)
        {

            var inquiryForm = await _inquiryFormManager.CreateAsync(
            input.CompanyName, input.Name, input.Email, input.Mobile, input.Fax, input.Phone, input.InquiryType, input.TradeLicensePlateOfIssue, input.BuyRent, input.SpaceInSquareFeet, input.Comments
            );

            return ObjectMapper.Map<InquiryForm, InquiryFormDto>(inquiryForm);
        }
    }
}