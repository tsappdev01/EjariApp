
using DIP.ZoneParagraphs;
using System;
using System.Collections.Generic;
using System.Globalization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using DIP.Zones;

namespace DIP.CustomMapper
{
    public class ZoneFrontEndObjectMapper : IObjectMapper<Zone, ZoneFrontEnd>,
        IObjectMapper<List<Zone>, List<ZoneFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public ZoneFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public ZoneFrontEnd Map(Zone source)
        {
            ZoneFrontEnd zoneFrontEnd = new ZoneFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                zoneFrontEnd = new ZoneFrontEnd()
                {
                    Title = source.TitleEn,                    
                    Slug = source.Slug,
                    Summary = source.SummaryEn,
                    MetaTitle = source.MetaTitleEn,
                    MetaDescription = source.MetaDescriptionEn,
                    Image = source.Image != null? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.Image}": null,
                    HeaderImage = source.HeaderImage != null? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.HeaderImage}": null,
                    Order = source.Order,
                    IsFeature = source.IsFeature,
                    IsActive = source.IsActive
                };
            }
            else
            {
                zoneFrontEnd = new ZoneFrontEnd()
                {
                    Title = source.TitleAR,
                    Slug = source.Slug,
                    Summary = source.SummaryAr,
                    MetaTitle = source.MetaTitleAr,
                    MetaDescription = source.MetaDescriptionAr,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.Image}" : null,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.HeaderImage}" : null,
                    Order = source.Order,
                    IsFeature = source.IsFeature,
                    IsActive = source.IsActive
                };
            }
           
            return zoneFrontEnd;
        }

        public ZoneFrontEnd Map(Zone source, ZoneFrontEnd destination)
        {
            return Map(source);
        }

        public List<ZoneFrontEnd> Map(List<Zone> source)
        {
            var output = new List<ZoneFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<ZoneFrontEnd> Map(List<Zone> source, List<ZoneFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

