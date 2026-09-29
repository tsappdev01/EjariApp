using DIP.EFormServiceSubCategories;
using DIP.SiteSettings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace DIP.CustomMapper
{
    public class SiteSettingObjectMapper : IObjectMapper<SiteSetting, SiteSettingFrontEnd>,
        IObjectMapper<List<SiteSetting>, List<SiteSettingFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public SiteSettingObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<SiteSettingFrontEnd> Map(List<SiteSetting> source)
        {

            var output = new List<SiteSettingFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<SiteSettingFrontEnd> Map(List<SiteSetting> source, List<SiteSettingFrontEnd> destination)
        {
            return Map(source);
        }

        public SiteSettingFrontEnd Map(SiteSetting source)
        {

            SiteSettingFrontEnd SiteSettingFront = new SiteSettingFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                SiteSettingFront = new SiteSettingFrontEnd()
                {
                    Id = source.Id,
                    FaceBookLink = source.FaceBookLink,
                    DipEmail = source.DipEmail,
                    Emergency1Phone = source.Emergency1Phone,
                    Emergency2Phone = source.Emergency2Phone,
                    FireDepartmentPhone = source.FireDepartmentPhone,
                    InstagramLink = source.InstagramLink,
                    LinkedinLink = source.LinkedinLink,
                     OfficeLocation = source.OfficeLocationEn,
                     Phone = source.Phone,
                    POBox = source.POBox,
                    PolicePost = source.PolicePost,
                    SiteLink = source.SiteLink,
                    TwitterLink = source.TwitterLink,
                    YouTubeLink = source.YouTubeLink,
                    WorkDays= source.WorkDays,
                    WorkHours= source.WorkHours,
                    RamadanWorkDays= source.RamadanWorkDays,
                    RamadanWorkHours= source.RamadanWorkHours,
                    FridayWorkHours= source.FridayWorkHours,
                    ClosedDay1= source.ClosedDay1,
                    ClosedDay2= source.ClosedDay2,
                    PbLocation = source.PbLocation,
                    
                };
            }
            else
            {
                SiteSettingFront = new SiteSettingFrontEnd()
                {
                    Id = source.Id,
                    FaceBookLink = source.FaceBookLink,
                    DipEmail = source.DipEmail,
                    Emergency1Phone = source.Emergency1Phone,
                    Emergency2Phone = source.Emergency2Phone,
                    FireDepartmentPhone = source.FireDepartmentPhone,
                    InstagramLink = source.InstagramLink,
                    LinkedinLink = source.LinkedinLink,
                    OfficeLocation = source.OfficeLocationAr,
                    Phone =source.Phone,
                    POBox = source.POBox,
                    PolicePost =source.PolicePost,
                    SiteLink = source.SiteLink,
                    TwitterLink = source.TwitterLink,
                    YouTubeLink = source.YouTubeLink,
                    WorkDays = source.WorkDays,
                    WorkHours = source.WorkHours,
                    RamadanWorkDays = source.RamadanWorkDays,
                    RamadanWorkHours = source.RamadanWorkHours,
                    FridayWorkHours = source.FridayWorkHours,
                    ClosedDay1 = source.ClosedDay1,
                    ClosedDay2 = source.ClosedDay2,
                    PbLocation = source.PbLocation,
                };

            }

            return SiteSettingFront;
        }

        public SiteSettingFrontEnd Map(SiteSetting source, SiteSettingFrontEnd destination)
        {
            return Map(source);
        }
    }
}


