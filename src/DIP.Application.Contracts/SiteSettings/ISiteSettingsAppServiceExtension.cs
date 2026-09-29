using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.SiteSettings
{
    public partial interface ISiteSettingsAppService
    {
        Task<SiteSettingFrontEnd> GetFrontAsync();
    }
}