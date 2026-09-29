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
using DIP.LastEventss;
using Volo.Abp.ObjectMapping;

namespace DIP.SupportedBanks
{

    public partial class SupportedBanksAppService 
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<SupportedBankFrontEnd>> GetListFrontEndAsync(GetSupportedBanksInput input)
        {
            var items = await _supportedBankRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.IsActive, input.OrderMin, input.OrderMax, input.Sorting, input.MaxResultCount, input.SkipCount);
            return ObjectMapper.Map<List<SupportedBank>, List<SupportedBankFrontEnd>>(items);

        
        }
    }
}