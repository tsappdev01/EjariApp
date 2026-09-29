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

    public partial class FeedBacksAppService
    {

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<FeedBackDto> CreateAsync(FeedBackCreateDto input)
        {

            var feedBack = await _feedBackManager.CreateAsync(
            input.Subject, input.CompanyName, input.PlotNo, input.PlotCategory, input.ContactPersonName, input.EmailId, input.MobileNumber, input.Department, input.CategoryName, input.Description
            );

            return ObjectMapper.Map<FeedBack, FeedBackDto>(feedBack);
        }

    }
}