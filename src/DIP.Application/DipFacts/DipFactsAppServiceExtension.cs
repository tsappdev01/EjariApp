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
using DIP.Zones;

namespace DIP.DipFacts
{
    public partial class DipFactsAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<DipFactFrontEnd>> GetListFrontEndAsync(GetDipFactsInput input)
        {
            var items = await _dipFactRepository.GetListAsync(input.FilterText, input.Image, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<List<DipFact>, List<DipFactFrontEnd>>(items);

        }
    }
}