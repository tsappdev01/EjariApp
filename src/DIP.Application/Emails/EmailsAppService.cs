using DIP.Shared;
using DIP.MediaGalleries;
using DIP.AmenityParagraphs;
using DIP.ZoneParagraphs;
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
using DIP.Medias;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using DIP.Shared;
using Volo.Abp.BlobStoring;
using DIP.Emails;
using Volo.Abp.Emailing;
using Volo.Abp.Emailing.Smtp;
using DLD.DSGP.Email;
using Microsoft.Extensions.Configuration;
using DIP.SiteSettings;

namespace DIP.Medias
{
    public partial class EmailsAppService : ApplicationService, IEmailsAppService
    {
        // private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ISmtpEmailSender _emailSender;
        private readonly DipEmailManager _dipEmailManager;
        private readonly ISiteSettingsAppService _siteSettingsAppService;
        public EmailsAppService(IConfiguration configuration, ISmtpEmailSender emailSender, ISiteSettingsAppService siteSettingsAppService, DipEmailManager dipEmailManager)
        {
            _configuration = configuration;
            _emailSender = emailSender;
            _siteSettingsAppService= siteSettingsAppService;
            _dipEmailManager = dipEmailManager;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            GetSiteSettingsInput getSiteSettingsInput = new GetSiteSettingsInput();
            getSiteSettingsInput.SkipCount = 0;
            getSiteSettingsInput.MaxResultCount = 1;

            List<SiteSettingDto> siteSettingDtos = (await _siteSettingsAppService.GetListAsync(getSiteSettingsInput)).Items.ToList();
            SiteSettingDto siteSettingDto = new SiteSettingDto();
            if (!siteSettingDtos.IsNullOrEmpty())
                siteSettingDto = siteSettingDtos.FirstOrDefault();

            var bodyHtml = await _dipEmailManager.GenerateEmailAsync(_configuration["App:SelfUrl"], body, siteSettingDto.FaceBookLink, siteSettingDto.TwitterLink, siteSettingDto.InstagramLink,
                                                                        siteSettingDto.LinkedinLink, siteSettingDto.YouTubeLink, siteSettingDto.POBox, siteSettingDto.Phone);
            await _emailSender.SendAsync(to, subject, bodyHtml, true);
        }
    }
}