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
using DIP.EServices;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using DIP.PressReleases;
using DotLiquid.Util;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DIP.EServices
{

    public partial class EServicesAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<EServiceFrontEnd>> GetListFrontEndAsync(GetEServicesInput input)
        {
            var items = await _eServiceRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.Slug, input.DescriptionEn, input.DescriptionAr, input.Image, input.HeaderImage, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<List<EService>, List<EServiceFrontEnd>>(items);
           
        }
        [AllowAnonymous]
        public virtual async Task<ActionResult> CallBackBank([FromForm] IFormCollection form)
        {
            return null;

        }


    }
}