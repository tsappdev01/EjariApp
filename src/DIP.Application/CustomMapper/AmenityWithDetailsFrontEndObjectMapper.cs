
using DIP.Amenities;
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
    public class AmenityWithDetailsFrontEndObjectMapper : IObjectMapper<AmenityWithDetails, AmenityFrontEnd>,
        IObjectMapper<List<AmenityWithDetails>, List<AmenityFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public AmenityWithDetailsFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public AmenityFrontEnd Map(AmenityWithDetails source)
        {
            AmenityFrontEnd amenityFrontEnd = new AmenityFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                amenityFrontEnd = new AmenityFrontEnd()
                {
                    Title = source.Amenity.TitleEn,
                    Slug = source.Amenity.Slug,
                    Order = source.Amenity.Order,
                    IsActive = source.Amenity.IsActive,
                    Image = source.Amenity.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.Amenity.Image}" : null,
                    HeaderImage = source.Amenity.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.Amenity.HeaderImage}" : null,
                    MetaDescription = source.Amenity.MetaDescriptionEn,
                    MetaTitle =source.Amenity.MetaTitleEn

                };
            }
            else
            {
                amenityFrontEnd = new AmenityFrontEnd()
                {
                    Title = source.Amenity.TitleAr,
                    Slug = source.Amenity.Slug,
                    Order = source.Amenity.Order,
                    IsActive = source.Amenity.IsActive,
                    Image = source.Amenity.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.Amenity.Image}" : null,
                    HeaderImage = source.Amenity.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.Amenity.HeaderImage}" : null,
                    MetaDescription = source.Amenity.MetaDescriptionAr,
                    MetaTitle = source.Amenity.MetaTitleAr

                };
            }
            if (!source.AmenityParagraphs.IsNullOrEmpty())
            {
                amenityFrontEnd.AmenityParagraphFrontEnd = _objectMapper.Map<List<AmenityParagraphWithDetails>, List<AmenityParagraphFrontEnd>>(source.AmenityParagraphs);
            }
           
            return amenityFrontEnd;
        }

        public AmenityFrontEnd Map(AmenityWithDetails source, AmenityFrontEnd destination)
        {
            return Map(source);
        }

        public List<AmenityFrontEnd> Map(List<AmenityWithDetails> source)
        {
            var output = new List<AmenityFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<AmenityFrontEnd> Map(List<AmenityWithDetails> source, List<AmenityFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

