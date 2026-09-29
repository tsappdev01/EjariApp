
using DIP.ZoneParagraphs;
using System;
using System.Collections.Generic;
using System.Globalization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using DIP.Zones;
using DIP.MajorIndustries;

namespace DIP.CustomMapper
{
    public class ZoneWithDetailsFrontEndObjectMapper : IObjectMapper<ZoneWithDetails, ZoneFrontEnd>,
        IObjectMapper<List<ZoneWithDetails>, List<ZoneFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public ZoneWithDetailsFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public ZoneFrontEnd Map(ZoneWithDetails source)
        {
            ZoneFrontEnd zoneFrontEnd = new ZoneFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                zoneFrontEnd = new ZoneFrontEnd()
                {
                    Title = source.Zone.TitleEn,
                    Slug = source.Zone.Slug,
                    Summary = source.Zone.SummaryEn,
                    Image = source.Zone.Image != null? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.Zone.Image}": null,
                    HeaderImage = source.Zone.HeaderImage != null? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.Zone.HeaderImage}": null,
                    Order = source.Zone.Order,
                    IsFeature = source.Zone.IsFeature,
                    IsActive = source.Zone.IsActive,
                    MetaDescription = source.Zone.MetaDescriptionEn,
                    MetaTitle =source.Zone.MetaTitleEn
                };
            }
            else
            {
                zoneFrontEnd = new ZoneFrontEnd()
                {
                    Title = source.Zone.TitleAR,
                    Slug = source.Zone.Slug,
                    Summary = source.Zone.SummaryAr,
                    Image = source.Zone.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.Zone.Image}" : null,
                    HeaderImage = source.Zone.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/zone/{source.Zone.HeaderImage}" : null,
                    Order = source.Zone.Order,
                    IsFeature = source.Zone.IsFeature,
                    IsActive = source.Zone.IsActive,
                    MetaDescription = source.Zone.MetaDescriptionAr,
                    MetaTitle = source.Zone.MetaTitleAr
                };
            }
            if (!source.ZoneParagraphs.IsNullOrEmpty())
            {
                zoneFrontEnd.ZonParagraphs = _objectMapper.Map<List<ZoneParagraphWithDetails>, List<ZoneParagraphFrontEnd>>(source.ZoneParagraphs);
            }
            if (!source.MajorIndustries.IsNullOrEmpty())
            {
                zoneFrontEnd.MajorIndustries = _objectMapper.Map<List<MajorIndustry>, List<MajorIndustryFrontEnd>>(source.MajorIndustries);
            }
            return zoneFrontEnd;
        }

        public ZoneFrontEnd Map(ZoneWithDetails source, ZoneFrontEnd destination)
        {
            return Map(source);
        }

        public List<ZoneFrontEnd> Map(List<ZoneWithDetails> source)
        {
            var output = new List<ZoneFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<ZoneFrontEnd> Map(List<ZoneWithDetails> source, List<ZoneFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

