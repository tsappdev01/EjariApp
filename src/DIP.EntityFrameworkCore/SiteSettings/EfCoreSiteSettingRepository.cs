using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using DIP.EntityFrameworkCore;

namespace DIP.SiteSettings
{
    public class EfCoreSiteSettingRepository : EfCoreRepository<DIPDbContext, SiteSetting, Guid>, ISiteSettingRepository
    {
        public EfCoreSiteSettingRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<List<SiteSetting>> GetListAsync(
            string filterText = null,
            string faceBookLink = null,
            string twitterLink = null,
            string instagramLink = null,
            string youTubeLink = null,
            string linkedinLink = null,
            string fireDepartmentPhone = null,
            string emergency1Phone = null,
            string emergency2Phone = null,
            string policePost = null,
            string dipEmail = null,
            string phone = null,
            string pOBox = null,
            string officeLocationEn = null,
            string officeLocationAr = null,
            string siteLink = null,
            string pbLocation = null,
            string workDays = null,
            string workHours = null,
            string ramadanWorkDays = null,
            string ramadanWorkHours = null,
            string fridayWorkHours = null,
            string closedDay1 = null,
            string closedDay2 = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, faceBookLink, twitterLink, instagramLink, youTubeLink, linkedinLink, fireDepartmentPhone, emergency1Phone, emergency2Phone, policePost, dipEmail, phone, pOBox, officeLocationEn, officeLocationAr, siteLink, pbLocation, workDays, workHours, ramadanWorkDays, ramadanWorkHours, fridayWorkHours, closedDay1, closedDay2);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SiteSettingConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string faceBookLink = null,
            string twitterLink = null,
            string instagramLink = null,
            string youTubeLink = null,
            string linkedinLink = null,
            string fireDepartmentPhone = null,
            string emergency1Phone = null,
            string emergency2Phone = null,
            string policePost = null,
            string dipEmail = null,
            string phone = null,
            string pOBox = null,
            string officeLocationEn = null,
            string officeLocationAr = null,
            string siteLink = null,
            string pbLocation = null,
            string workDays = null,
            string workHours = null,
            string ramadanWorkDays = null,
            string ramadanWorkHours = null,
            string fridayWorkHours = null,
            string closedDay1 = null,
            string closedDay2 = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, faceBookLink, twitterLink, instagramLink, youTubeLink, linkedinLink, fireDepartmentPhone, emergency1Phone, emergency2Phone, policePost, dipEmail, phone, pOBox, officeLocationEn, officeLocationAr, siteLink, pbLocation, workDays, workHours, ramadanWorkDays, ramadanWorkHours, fridayWorkHours, closedDay1, closedDay2);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SiteSetting> ApplyFilter(
            IQueryable<SiteSetting> query,
            string filterText,
            string faceBookLink = null,
            string twitterLink = null,
            string instagramLink = null,
            string youTubeLink = null,
            string linkedinLink = null,
            string fireDepartmentPhone = null,
            string emergency1Phone = null,
            string emergency2Phone = null,
            string policePost = null,
            string dipEmail = null,
            string phone = null,
            string pOBox = null,
            string officeLocationEn = null,
            string officeLocationAr = null,
            string siteLink = null,
            string pbLocation = null,
            string workDays = null,
            string workHours = null,
            string ramadanWorkDays = null,
            string ramadanWorkHours = null,
            string fridayWorkHours = null,
            string closedDay1 = null,
            string closedDay2 = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FaceBookLink.Contains(filterText) || e.TwitterLink.Contains(filterText) || e.InstagramLink.Contains(filterText) || e.YouTubeLink.Contains(filterText) || e.LinkedinLink.Contains(filterText) || e.FireDepartmentPhone.Contains(filterText) || e.Emergency1Phone.Contains(filterText) || e.Emergency2Phone.Contains(filterText) || e.PolicePost.Contains(filterText) || e.DipEmail.Contains(filterText) || e.Phone.Contains(filterText) || e.POBox.Contains(filterText) || e.OfficeLocationEn.Contains(filterText) || e.OfficeLocationAr.Contains(filterText) || e.SiteLink.Contains(filterText) || e.PbLocation.Contains(filterText) || e.WorkDays.Contains(filterText) || e.WorkHours.Contains(filterText) || e.RamadanWorkDays.Contains(filterText) || e.RamadanWorkHours.Contains(filterText) || e.FridayWorkHours.Contains(filterText) || e.ClosedDay1.Contains(filterText) || e.ClosedDay2.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(faceBookLink), e => e.FaceBookLink.Contains(faceBookLink))
                    .WhereIf(!string.IsNullOrWhiteSpace(twitterLink), e => e.TwitterLink.Contains(twitterLink))
                    .WhereIf(!string.IsNullOrWhiteSpace(instagramLink), e => e.InstagramLink.Contains(instagramLink))
                    .WhereIf(!string.IsNullOrWhiteSpace(youTubeLink), e => e.YouTubeLink.Contains(youTubeLink))
                    .WhereIf(!string.IsNullOrWhiteSpace(linkedinLink), e => e.LinkedinLink.Contains(linkedinLink))
                    .WhereIf(!string.IsNullOrWhiteSpace(fireDepartmentPhone), e => e.FireDepartmentPhone.Contains(fireDepartmentPhone))
                    .WhereIf(!string.IsNullOrWhiteSpace(emergency1Phone), e => e.Emergency1Phone.Contains(emergency1Phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(emergency2Phone), e => e.Emergency2Phone.Contains(emergency2Phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(policePost), e => e.PolicePost.Contains(policePost))
                    .WhereIf(!string.IsNullOrWhiteSpace(dipEmail), e => e.DipEmail.Contains(dipEmail))
                    .WhereIf(!string.IsNullOrWhiteSpace(phone), e => e.Phone.Contains(phone))
                    .WhereIf(!string.IsNullOrWhiteSpace(pOBox), e => e.POBox.Contains(pOBox))
                    .WhereIf(!string.IsNullOrWhiteSpace(officeLocationEn), e => e.OfficeLocationEn.Contains(officeLocationEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(officeLocationAr), e => e.OfficeLocationAr.Contains(officeLocationAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(siteLink), e => e.SiteLink.Contains(siteLink))
                    .WhereIf(!string.IsNullOrWhiteSpace(pbLocation), e => e.PbLocation.Contains(pbLocation))
                    .WhereIf(!string.IsNullOrWhiteSpace(workDays), e => e.WorkDays.Contains(workDays))
                    .WhereIf(!string.IsNullOrWhiteSpace(workHours), e => e.WorkHours.Contains(workHours))
                    .WhereIf(!string.IsNullOrWhiteSpace(ramadanWorkDays), e => e.RamadanWorkDays.Contains(ramadanWorkDays))
                    .WhereIf(!string.IsNullOrWhiteSpace(ramadanWorkHours), e => e.RamadanWorkHours.Contains(ramadanWorkHours))
                    .WhereIf(!string.IsNullOrWhiteSpace(fridayWorkHours), e => e.FridayWorkHours.Contains(fridayWorkHours))
                    .WhereIf(!string.IsNullOrWhiteSpace(closedDay1), e => e.ClosedDay1.Contains(closedDay1))
                    .WhereIf(!string.IsNullOrWhiteSpace(closedDay2), e => e.ClosedDay2.Contains(closedDay2));
        }
    }
}