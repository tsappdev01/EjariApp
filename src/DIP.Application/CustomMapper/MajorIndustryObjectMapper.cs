using DIP.EFormServiceSubCategories;
using DIP.MajorIndustries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using static System.Net.Mime.MediaTypeNames;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace DIP.CustomMapper
{
    public class MajorIndustryObjectMapper : IObjectMapper<MajorIndustry, MajorIndustryFrontEnd>,
        IObjectMapper<List<MajorIndustry>, List<MajorIndustryFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public MajorIndustryObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<MajorIndustryFrontEnd> Map(List<MajorIndustry> source)
        {

            var output = new List<MajorIndustryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<MajorIndustryFrontEnd> Map(List<MajorIndustry> source, List<MajorIndustryFrontEnd> destination)
        {
            return Map(source);
        }

        public MajorIndustryFrontEnd Map(MajorIndustry source)
        {

            MajorIndustryFrontEnd MajorIndustryFront = new MajorIndustryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                MajorIndustryFront = new MajorIndustryFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/majorIndustry/{source.Image}" : null,

                    ZoneId = source.ZoneId,
                 
                    
                    
                };
            }
            else
            {
                MajorIndustryFront = new MajorIndustryFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,

                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/majorIndustry/{source.Image}" : null,
                    ZoneId = source.ZoneId,


                };

            }

            return MajorIndustryFront;
        }

        public MajorIndustryFrontEnd Map(MajorIndustry source, MajorIndustryFrontEnd destination)
        {
            return Map(source);
        }
    }
}


