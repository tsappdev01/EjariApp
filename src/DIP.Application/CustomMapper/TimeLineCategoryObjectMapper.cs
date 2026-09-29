using DIP.EFormServiceSubCategories;
using DIP.TimeLineCategories;
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
    public class TimeLineCategoryObjectMapper : IObjectMapper<TimeLineCategoryWithDetails, TimeLineCategoryFrontEnd>,
        IObjectMapper<List<TimeLineCategoryWithDetails>, List<TimeLineCategoryFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public TimeLineCategoryObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<TimeLineCategoryFrontEnd> Map(List<TimeLineCategoryWithDetails> source)
        {

            var output = new List<TimeLineCategoryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<TimeLineCategoryFrontEnd> Map(List<TimeLineCategoryWithDetails> source, List<TimeLineCategoryFrontEnd> destination)
        {
            return Map(source);
        }

        public TimeLineCategoryFrontEnd Map(TimeLineCategoryWithDetails source)
        {

            TimeLineCategoryFrontEnd timeLineCategoryFront = new TimeLineCategoryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                timeLineCategoryFront = new TimeLineCategoryFrontEnd()
                {
                    Title = source.TimeLineCategory.TitleEn,
                    Order = source.TimeLineCategory.Order,
                    IsActive = source.TimeLineCategory.IsActive,
                    Id = source.TimeLineCategory.Id
                };
            }
            else
            {
                timeLineCategoryFront = new TimeLineCategoryFrontEnd()
                {
                    Title = source.TimeLineCategory.TitleAr,
                    Order = source.TimeLineCategory.Order,
                    IsActive = source.TimeLineCategory.IsActive   ,
                    Id = source.TimeLineCategory.Id

                };
            }

            if (!source.TimeLines.IsNullOrEmpty())
                timeLineCategoryFront.TimeLineFrontEndList = _objectMapper.Map<List<TimeLine>, List<TimeLineFrontEnd>>(source.TimeLines);

            return timeLineCategoryFront;
        }

        public TimeLineCategoryFrontEnd Map(TimeLineCategoryWithDetails source, TimeLineCategoryFrontEnd destination)
        {
            return Map(source);
        }
    }
}


