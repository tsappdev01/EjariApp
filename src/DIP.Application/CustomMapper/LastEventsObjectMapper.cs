using DIP.EFormServiceSubCategories;
using DIP.LastEventss;
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
    public class LastEventsObjectMapper : IObjectMapper<LastEvents, LastEventsFrontEnd>,
        IObjectMapper<List<LastEvents>, List<LastEventsFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public LastEventsObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<LastEventsFrontEnd> Map(List<LastEvents> source)
        {

            var output = new List<LastEventsFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<LastEventsFrontEnd> Map(List<LastEvents> source, List<LastEventsFrontEnd> destination)
        {
            return Map(source);
        }

        public LastEventsFrontEnd Map(LastEvents source)
        {

            LastEventsFrontEnd LastEventsFront = new LastEventsFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                LastEventsFront = new LastEventsFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    Description = source.DescriptionEn,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/lastEvent/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/lastEvent/{source.Image}" : null,
                    MetaDescription =source.MetaDescriptionEn,
                    MetaTitle =source.MetaTitleEn,
                    Slug = source.Slug,
                    EndDate = source.EndDate,
                    IsFeatured = source.IsFeatured,
                    Location = source.LocationEn,
                    StartDate = source.StartDate,
                    Summary =source.SummaryEn,
                    
                    
                };
            }
            else
            {
                LastEventsFront = new LastEventsFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                    Description = source.DescriptionAr,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/lastEvent/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/lastEvent/{source.Image}" : null,

                    MetaDescription = source.MetaDescriptionAr,
                    MetaTitle = source.MetaTitleAr,
                    Slug = source.Slug,
                   
                    EndDate = source.EndDate,
                    IsFeatured = source.IsFeatured,
                    Location = source.LocationAr,
                    StartDate = source.StartDate,
                    Summary = source.SummaryAr,

                };

            }

            return LastEventsFront;
        }

        public LastEventsFrontEnd Map(LastEvents source, LastEventsFrontEnd destination)
        {
            return Map(source);
        }
    }
}


