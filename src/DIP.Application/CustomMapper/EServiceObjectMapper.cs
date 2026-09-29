using DIP.EFormServiceSubCategories;
using DIP.EServices;
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
    public class EServiceObjectMapper : IObjectMapper<EService, EServiceFrontEnd>,
        IObjectMapper<List<EService>, List<EServiceFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public EServiceObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<EServiceFrontEnd> Map(List<EService> source)
        {

            var output = new List<EServiceFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<EServiceFrontEnd> Map(List<EService> source, List<EServiceFrontEnd> destination)
        {
            return Map(source);
        }

        public EServiceFrontEnd Map(EService source)
        {

            EServiceFrontEnd EServiceFront = new EServiceFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                EServiceFront = new EServiceFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    Description = source.DescriptionEn,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/eservice/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/eservice/{source.Image}" : null,
                    MetaDescription =source.MetaDescriptionEn,
                    MetaTitle =source.MetaTitleEn,
                    Slug = source.Slug,
                    
                };
            }
            else
            {
                EServiceFront = new EServiceFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                    Description = source.DescriptionAr,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/eservice/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/eservice/{source.Image}" : null,

                    MetaDescription = source.MetaDescriptionAr,
                    MetaTitle = source.MetaTitleAr,
                    Slug = source.Slug,

                };

            }

            return EServiceFront;
        }

        public EServiceFrontEnd Map(EService source, EServiceFrontEnd destination)
        {
            return Map(source);
        }
    }
}


