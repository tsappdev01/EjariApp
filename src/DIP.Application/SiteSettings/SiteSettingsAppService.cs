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

    [Authorize(DIPPermissions.SiteSettings.Default)]
    public partial class SiteSettingsAppService : ApplicationService, ISiteSettingsAppService
    {
        private readonly IDistributedCache<SiteSettingExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly SiteSettingManager _siteSettingManager;

        public SiteSettingsAppService(ISiteSettingRepository siteSettingRepository, SiteSettingManager siteSettingManager, IDistributedCache<SiteSettingExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _siteSettingRepository = siteSettingRepository;
            _siteSettingManager = siteSettingManager;
        }

        public virtual async Task<SiteSettingDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<SiteSetting, SiteSettingDto>(await _siteSettingRepository.GetAsync(id));
        }

        [Authorize(DIPPermissions.SiteSettings.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _siteSettingRepository.DeleteAsync(id);
        }

        [Authorize(DIPPermissions.SiteSettings.Create)]
        public virtual async Task<SiteSettingDto> CreateAsync(SiteSettingCreateDto input)
        {

            var siteSetting = await _siteSettingManager.CreateAsync(
            input.FaceBookLink, input.TwitterLink, input.InstagramLink, input.YouTubeLink, input.LinkedinLink, input.FireDepartmentPhone, input.Emergency1Phone, input.Emergency2Phone, input.PolicePost, input.DipEmail, input.Phone, input.POBox, input.OfficeLocationEn, input.OfficeLocationAr, input.SiteLink, input.PbLocation, input.WorkDays, input.WorkHours, input.RamadanWorkDays, input.RamadanWorkHours, input.FridayWorkHours, input.ClosedDay1, input.ClosedDay2
            );

            return ObjectMapper.Map<SiteSetting, SiteSettingDto>(siteSetting);
        }

        [Authorize(DIPPermissions.SiteSettings.Edit)]
        public virtual async Task<SiteSettingDto> UpdateAsync(Guid id, SiteSettingUpdateDto input)
        {

            var siteSetting = await _siteSettingManager.UpdateAsync(
            id,
            input.FaceBookLink, input.TwitterLink, input.InstagramLink, input.YouTubeLink, input.LinkedinLink, input.FireDepartmentPhone, input.Emergency1Phone, input.Emergency2Phone, input.PolicePost, input.DipEmail, input.Phone, input.POBox, input.OfficeLocationEn, input.OfficeLocationAr, input.SiteLink, input.PbLocation, input.WorkDays, input.WorkHours, input.RamadanWorkDays, input.RamadanWorkHours, input.FridayWorkHours, input.ClosedDay1, input.ClosedDay2, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<SiteSetting, SiteSettingDto>(siteSetting);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SiteSettingExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _siteSettingRepository.GetListAsync(input.FilterText, input.FaceBookLink, input.TwitterLink, input.InstagramLink, input.YouTubeLink, input.LinkedinLink, input.FireDepartmentPhone, input.Emergency1Phone, input.Emergency2Phone, input.PolicePost, input.DipEmail, input.Phone, input.POBox, input.OfficeLocationEn, input.OfficeLocationAr, input.SiteLink, input.PbLocation, input.WorkDays, input.WorkHours, input.RamadanWorkDays, input.RamadanWorkHours, input.FridayWorkHours, input.ClosedDay1, input.ClosedDay2);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<SiteSetting>, List<SiteSettingExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "SiteSettings.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new SiteSettingExcelDownloadTokenCacheItem { Token = token },
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