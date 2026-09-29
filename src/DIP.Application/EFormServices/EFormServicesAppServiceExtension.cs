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
using DIP.EFormServices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using DIP.Zones;

namespace DIP.EFormServices
{

    public partial class EFormServicesAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<EFormServiceFrontEnd>> GetListFrontEndAsync(GetEFormServicesInput input)
        {
            var items = await _eFormServiceRepository.GetListWithDetailsAsync(input.FilterText, input.TitleEn, input.TitleAr, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);


            return ObjectMapper.Map<List<EFromServiceWithDetails>, List<EFormServiceFrontEnd>>(items);

        }
    }
}