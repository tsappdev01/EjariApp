using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.SiteSettings
{
    public interface ISiteSettingRepository : IRepository<SiteSetting, Guid>
    {
        Task<List<SiteSetting>> GetListAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}