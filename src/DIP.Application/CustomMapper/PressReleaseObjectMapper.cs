using DIP.EFormServiceSubCategories;
using DIP.PressReleases;
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
    public class PressReleaseObjectMapper : IObjectMapper<PressRelease, PressReleaseFrontEnd>,
        IObjectMapper<List<PressRelease>, List<PressReleaseFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public PressReleaseObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<PressReleaseFrontEnd> Map(List<PressRelease> source)
        {

            var output = new List<PressReleaseFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<PressReleaseFrontEnd> Map(List<PressRelease> source, List<PressReleaseFrontEnd> destination)
        {
            return Map(source);
        }

        public PressReleaseFrontEnd Map(PressRelease source)
        {

            PressReleaseFrontEnd PressReleaseFront = new PressReleaseFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                PressReleaseFront = new PressReleaseFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    Description = source.DescriptionEn,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pressRelease/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pressRelease/{source.Image}" : null,
                    MetaDescription =source.MetaDescriptionEn,
                    MetaTitle =source.MetaTitleEn,
                    Slug = source.Slug,
                    Summary =source.SummaryEn,
                    IsFeatured = source.IsFeatured,
                    Date = source.Date,


                };
            }
            else
            {
                PressReleaseFront = new PressReleaseFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                    Description = source.DescriptionAr,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pressRelease/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pressRelease/{source.Image}" : null,
                    MetaDescription = source.MetaDescriptionAr,
                    MetaTitle = source.MetaTitleAr,
                    Slug = source.Slug,
                    Summary = source.SummaryAr,
                    IsFeatured = source.IsFeatured,
                    Date = source.Date,
                    

                };

            }

            return PressReleaseFront;
        }

        public PressReleaseFrontEnd Map(PressRelease source, PressReleaseFrontEnd destination)
        {
            return Map(source);
        }
    }
}


