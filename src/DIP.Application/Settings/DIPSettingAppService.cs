using DIP.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Security.Encryption;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

namespace DIP.Settings
{
    [RemoteService(IsEnabled = false)]

    public class DIPSettingAppService : ApplicationService, IDIPSettingAppService
    {
        private readonly ISettingProvider _settingProvider;
        private readonly ISettingManager _settingManager;

        protected IStringEncryptionService _stringEncryptionService { get; }
        public DIPSettingAppService(
            ISettingProvider settingProvider,
            ISettingManager settingManager,
            IStringEncryptionService stringEncryptionService
            )
        {
            _settingProvider = settingProvider;
            _settingManager = settingManager;
            _stringEncryptionService = stringEncryptionService;
        }


        [AllowAnonymous]
        public Task<GoogleReCaptachDto> GetGoogleReCaptachSettingAsync()
        {
            return GoogleReCaptachSettingAsync();
        }

        [AllowAnonymous]
        public Task<SmtpSettingDto> GetSmtpSettingAsync()
        {
            return SmtpSettingAsync();
        }
        //[AllowAnonymous]
        //public async Task UpdateGoogleReCaptachSettingAsync(GoogleReCaptachDto input)
        //{
        //    await _settingManager.SetGlobalAsync(DIPSettings.GoogleReCaptcha.SiteKey, input.SiteKey);
        //    await _settingManager.SetGlobalAsync(DIPSettings.GoogleReCaptcha.SecretKey, input.SecretKey);
        //}


        [AllowAnonymous]
        public async Task<GoogleReCaptachDto> GoogleReCaptachSettingAsync()
        {
            var googleReCaptachDto = new GoogleReCaptachDto()
            {
                SiteKey = await _settingProvider.GetOrNullAsync(DIPSettings.GoogleReCaptcha.SiteKey),
                SecretKey = await _settingProvider.GetOrNullAsync(DIPSettings.GoogleReCaptcha.SecretKey)
            };
            return googleReCaptachDto;
        }

        [AllowAnonymous]
        public async Task<SmtpSettingDto> SmtpSettingAsync()
        {

            string password = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.Password);

            password = !password.IsNullOrWhiteSpace() ? _stringEncryptionService.Decrypt(password) : "";
            var smtpSettingDto = new SmtpSettingDto()
            {
                Host = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.Host),
                Port = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.Port),
                UserName = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.UserName),
                Password = password,
                Domain = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.Domain),
                EnableSsl = await _settingProvider.GetAsync<bool>(DIPSettings.Smtp.EnableSsl),
                UseDefaultCredentials = await _settingProvider.GetAsync<bool>(DIPSettings.Smtp.UseDefaultCredentials),
                DefaultFromAddress = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.DefaultFromAddress),
                DefaultFromDisplayName = await _settingProvider.GetOrNullAsync(DIPSettings.Smtp.DefaultFromDisplayName),
            };
            return smtpSettingDto;
        }

    }

}