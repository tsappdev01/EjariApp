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
using DIP.SiteSettings;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;

namespace DIP.SiteSettings
{
       
    public partial class SiteSettingsAppService
    {

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<SiteSettingFrontEnd> GetFrontAsync()
        {

            GetSiteSettingsInput input = new GetSiteSettingsInput();
            input.SkipCount = 0;
            input.MaxResultCount = 1;
            var items = await _siteSettingRepository.GetListAsync(input.FilterText, input.FaceBookLink, input.TwitterLink, input.InstagramLink, input.YouTubeLink, input.LinkedinLink, input.FireDepartmentPhone, input.Emergency1Phone, input.Emergency2Phone, input.PolicePost, input.DipEmail, input.Phone, input.POBox, input.OfficeLocationEn, input.OfficeLocationAr, input.SiteLink, input.PbLocation, input.WorkDays, input.WorkHours, input.RamadanWorkDays, input.RamadanWorkHours, input.FridayWorkHours, input.ClosedDay1, input.ClosedDay2, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<SiteSetting, SiteSettingFrontEnd>(items.FirstOrDefault());
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<SiteSettingDto>> GetListAsync(GetSiteSettingsInput input)
        {
            var totalCount = await _siteSettingRepository.GetCountAsync(input.FilterText, input.FaceBookLink, input.TwitterLink, input.InstagramLink, input.YouTubeLink, input.LinkedinLink, input.FireDepartmentPhone, input.Emergency1Phone, input.Emergency2Phone, input.PolicePost, input.DipEmail, input.Phone, input.POBox, input.OfficeLocationEn, input.OfficeLocationAr, input.SiteLink, input.PbLocation, input.WorkDays, input.WorkHours, input.RamadanWorkDays, input.RamadanWorkHours, input.FridayWorkHours, input.ClosedDay1, input.ClosedDay2);
            var items = await _siteSettingRepository.GetListAsync(input.FilterText, input.FaceBookLink, input.TwitterLink, input.InstagramLink, input.YouTubeLink, input.LinkedinLink, input.FireDepartmentPhone, input.Emergency1Phone, input.Emergency2Phone, input.PolicePost, input.DipEmail, input.Phone, input.POBox, input.OfficeLocationEn, input.OfficeLocationAr, input.SiteLink, input.PbLocation, input.WorkDays, input.WorkHours, input.RamadanWorkDays, input.RamadanWorkHours, input.FridayWorkHours, input.ClosedDay1, input.ClosedDay2, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SiteSettingDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SiteSetting>, List<SiteSettingDto>>(items)
            };
        }


    }
}