using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.SiteSettings
{
    public class SiteSettingManager : DomainService
    {
        private readonly ISiteSettingRepository _siteSettingRepository;

        public SiteSettingManager(ISiteSettingRepository siteSettingRepository)
        {
            _siteSettingRepository = siteSettingRepository;
        }

        public async Task<SiteSetting> CreateAsync(
        string faceBookLink, string twitterLink, string instagramLink, string youTubeLink, string linkedinLink, string fireDepartmentPhone, string emergency1Phone, string emergency2Phone, string policePost, string dipEmail, string phone, string pOBox, string officeLocationEn, string officeLocationAr, string siteLink, string pbLocation, string workDays, string workHours, string ramadanWorkDays, string ramadanWorkHours, string fridayWorkHours, string closedDay1, string closedDay2)
        {

            var siteSetting = new SiteSetting(
             GuidGenerator.Create(),
             faceBookLink, twitterLink, instagramLink, youTubeLink, linkedinLink, fireDepartmentPhone, emergency1Phone, emergency2Phone, policePost, dipEmail, phone, pOBox, officeLocationEn, officeLocationAr, siteLink, pbLocation, workDays, workHours, ramadanWorkDays, ramadanWorkHours, fridayWorkHours, closedDay1, closedDay2
             );

            return await _siteSettingRepository.InsertAsync(siteSetting);
        }

        public async Task<SiteSetting> UpdateAsync(
            Guid id,
            string faceBookLink, string twitterLink, string instagramLink, string youTubeLink, string linkedinLink, string fireDepartmentPhone, string emergency1Phone, string emergency2Phone, string policePost, string dipEmail, string phone, string pOBox, string officeLocationEn, string officeLocationAr, string siteLink, string pbLocation, string workDays, string workHours, string ramadanWorkDays, string ramadanWorkHours, string fridayWorkHours, string closedDay1, string closedDay2, [CanBeNull] string concurrencyStamp = null
        )
        {

            var siteSetting = await _siteSettingRepository.GetAsync(id);

            siteSetting.FaceBookLink = faceBookLink;
            siteSetting.TwitterLink = twitterLink;
            siteSetting.InstagramLink = instagramLink;
            siteSetting.YouTubeLink = youTubeLink;
            siteSetting.LinkedinLink = linkedinLink;
            siteSetting.FireDepartmentPhone = fireDepartmentPhone;
            siteSetting.Emergency1Phone = emergency1Phone;
            siteSetting.Emergency2Phone = emergency2Phone;
            siteSetting.PolicePost = policePost;
            siteSetting.DipEmail = dipEmail;
            siteSetting.Phone = phone;
            siteSetting.POBox = pOBox;
            siteSetting.OfficeLocationEn = officeLocationEn;
            siteSetting.OfficeLocationAr = officeLocationAr;
            siteSetting.SiteLink = siteLink;
            siteSetting.PbLocation = pbLocation;
            siteSetting.WorkDays = workDays;
            siteSetting.WorkHours = workHours;
            siteSetting.RamadanWorkDays = ramadanWorkDays;
            siteSetting.RamadanWorkHours = ramadanWorkHours;
            siteSetting.FridayWorkHours = fridayWorkHours;
            siteSetting.ClosedDay1 = closedDay1;
            siteSetting.ClosedDay2 = closedDay2;

            siteSetting.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _siteSettingRepository.UpdateAsync(siteSetting);
        }

    }
}