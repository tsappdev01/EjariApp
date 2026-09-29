
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
    public class AmenityFrontEndObjectMapper : IObjectMapper<Amenity, AmenityFrontEnd>,
        IObjectMapper<List<Amenity>, List<AmenityFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public AmenityFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public AmenityFrontEnd Map(Amenity source)
        {
            AmenityFrontEnd amenityFrontEnd = new AmenityFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                amenityFrontEnd = new AmenityFrontEnd()
                {
                    Title = source.TitleEn,
                    Slug = source.Slug,
                    MetaTitle = source.MetaTitleEn,
                    MetaDescription = source.MetaDescriptionEn,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.Image}" : null,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.HeaderImage}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };
            }
            else
            {
                amenityFrontEnd = new AmenityFrontEnd()
                {
                    Title = source.TitleAr,
                    Slug = source.Slug,
                    MetaTitle = source.MetaTitleAr,
                    MetaDescription = source.MetaDescriptionAr,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.Image}" : null,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/amenity/{source.HeaderImage}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };
            }           
            return amenityFrontEnd;
        }

        public AmenityFrontEnd Map(Amenity source, AmenityFrontEnd destination)
        {
            return Map(source);
        }

        public List<AmenityFrontEnd> Map(List<Amenity> source)
        {
            var output = new List<AmenityFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<AmenityFrontEnd> Map(List<Amenity> source, List<AmenityFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

