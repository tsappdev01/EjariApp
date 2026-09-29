
using DIP.AmenityParagraphs;
using DIP.Medias;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;


namespace DIP.CustomMapper
{
    public class AmenityParagraphFrontEndObjectMapper : IObjectMapper<AmenityParagraphWithDetails, AmenityParagraphFrontEnd>,
        IObjectMapper<List<AmenityParagraphWithDetails>, List<AmenityParagraphFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public AmenityParagraphFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public AmenityParagraphFrontEnd Map(AmenityParagraphWithDetails source)
        {
            AmenityParagraphFrontEnd amenityParagraphFrontEnd = new AmenityParagraphFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                amenityParagraphFrontEnd = new AmenityParagraphFrontEnd()
                {
                    Title = source.AmenityParagraph.TitleEn,
                    SubTitle = source.AmenityParagraph.SubTitleEn,
                    Description = source.AmenityParagraph.DescriptionEn,
                    ButtonUrl = source.AmenityParagraph.ButtonUrl,
                    Order = source.AmenityParagraph.Order,
                    IsActive = source.AmenityParagraph.IsActive,
                    
                };
            }
            else
            {
                amenityParagraphFrontEnd = new AmenityParagraphFrontEnd()
                {
                    Title = source.AmenityParagraph.TitleAr,
                    SubTitle = source.AmenityParagraph.SubTitleAr,
                    Description = source.AmenityParagraph.DescriptionAr,
                    ButtonUrl = source.AmenityParagraph.ButtonUrl,
                    Order = source.AmenityParagraph.Order,
                    IsActive = source.AmenityParagraph.IsActive
                    
                };
            }
            if (!source.Medias.IsNullOrEmpty())
            {
                amenityParagraphFrontEnd.Medias = _objectMapper.Map<List<Media>, List<MediaFrontEnd>>(source.Medias);
            }
           
            return amenityParagraphFrontEnd;
        }

        public AmenityParagraphFrontEnd Map(AmenityParagraphWithDetails source, AmenityParagraphFrontEnd destination)
        {
            return Map(source);
        }

        public List<AmenityParagraphFrontEnd> Map(List<AmenityParagraphWithDetails> source)
        {
            var output = new List<AmenityParagraphFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<AmenityParagraphFrontEnd> Map(List<AmenityParagraphWithDetails> source, List<AmenityParagraphFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

