using DIP.EFormServiceSubCategories;
using DIP.TimeLines;
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
    public class TimeLineObjectMapper : IObjectMapper<TimeLine, TimeLineFrontEnd>,
        IObjectMapper<List<TimeLine>, List<TimeLineFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public TimeLineObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<TimeLineFrontEnd> Map(List<TimeLine> source)
        {

            var output = new List<TimeLineFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<TimeLineFrontEnd> Map(List<TimeLine> source, List<TimeLineFrontEnd> destination)
        {
            return Map(source);
        }

        public TimeLineFrontEnd Map(TimeLine source)
        {

            TimeLineFrontEnd TimeLineFront = new TimeLineFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                TimeLineFront = new TimeLineFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    Description = source.DescriptionEn,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/timeLine/{source.Image}" : null,
                    TimeLineDate = source.TimeLineDate
                };
            }
            else
            {
                TimeLineFront = new TimeLineFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                    Description = source.DescriptionAr,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/timeLine/{source.Image}" : null,
                    TimeLineDate = source.TimeLineDate

                };

            }

            return TimeLineFront;
        }

        public TimeLineFrontEnd Map(TimeLine source, TimeLineFrontEnd destination)
        {
            return Map(source);
        }
    }
}


