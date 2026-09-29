
using DIP.ZoneParagraphs;
using DIP.Medias;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;


namespace DIP.CustomMapper
{
    public class ZoneParagraphFrontEndObjectMapper : IObjectMapper<ZoneParagraphWithDetails, ZoneParagraphFrontEnd>,
        IObjectMapper<List<ZoneParagraphWithDetails>, List<ZoneParagraphFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public ZoneParagraphFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public ZoneParagraphFrontEnd Map(ZoneParagraphWithDetails source)
        {
            ZoneParagraphFrontEnd zoneParagraphFrontEnd = new ZoneParagraphFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                zoneParagraphFrontEnd = new ZoneParagraphFrontEnd()
                {
                    Title = source.ZoneParagraph.TitleEn,
                    SubTitle = source.ZoneParagraph.SubTilteEn,
                    Description = source.ZoneParagraph.DescriptionEn,
                    Order = source.ZoneParagraph.Order,
                    IsActive = source.ZoneParagraph.IsActive
                };
            }
            else
            {
                zoneParagraphFrontEnd = new ZoneParagraphFrontEnd()
                {
                    Title = source.ZoneParagraph.TitleAr,
                    SubTitle = source.ZoneParagraph.SubTitleAr,
                    Description = source.ZoneParagraph.DescriptionAr,
                    Order = source.ZoneParagraph.Order,
                    IsActive = source.ZoneParagraph.IsActive
                };
            }
            if (!source.Medias.IsNullOrEmpty())
            {
                zoneParagraphFrontEnd.Medias = _objectMapper.Map<List<Media>, List<MediaFrontEnd>>(source.Medias);
            }
           
            return zoneParagraphFrontEnd;
        }

        public ZoneParagraphFrontEnd Map(ZoneParagraphWithDetails source, ZoneParagraphFrontEnd destination)
        {
            return Map(source);
        }

        public List<ZoneParagraphFrontEnd> Map(List<ZoneParagraphWithDetails> source)
        {
            var output = new List<ZoneParagraphFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<ZoneParagraphFrontEnd> Map(List<ZoneParagraphWithDetails> source, List<ZoneParagraphFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

