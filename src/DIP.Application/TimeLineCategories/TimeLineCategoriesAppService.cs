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
using DIP.TimeLineCategories;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using DIP.Amenities;

namespace DIP.TimeLineCategories
{

    public partial class TimeLineCategoriesAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<TimeLineCategoryFrontEnd>> GetListWithDetailsFrontEndAsync()
        {
            return ObjectMapper.Map<List<TimeLineCategoryWithDetails>, List<TimeLineCategoryFrontEnd>>(await _timeLineCategoryRepository.GetListWithDetailsAsync());
        }
    }
}