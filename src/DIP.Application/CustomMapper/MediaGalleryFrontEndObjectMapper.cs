
using DIP.ZoneParagraphs;
using DIP.Medias;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using DIP.MediaGalleries;

namespace DIP.CustomMapper
{
    public class MediaGalleryFrontEndObjectMapper : IObjectMapper<MediaGallery, MediaGalleryFrontEnd>, IObjectMapper<List<MediaGallery>, List<MediaGalleryFrontEnd>>, IObjectMapper<MediaGalleryWithDetails, MediaGalleryFrontEnd>,
        IObjectMapper<List<MediaGalleryWithDetails>, List<MediaGalleryFrontEnd>>, ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public MediaGalleryFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        /// <summary>
        /// Media Gallery Mapper
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MediaGalleryFrontEnd Map(MediaGallery source)
        {
            MediaGalleryFrontEnd mediaGalleryFrontEnd = new MediaGalleryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                mediaGalleryFrontEnd = new MediaGalleryFrontEnd()
                {
                    Title = source.TitleEn,
                    Slug = source.Slug,
                    MetaTitle = source.MetaTitleEn,
                    MetaDescription = source.MetaDescriptionEn,
                    Summary = source.SummaryEn,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.Image}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };
            }
            else
            {
                mediaGalleryFrontEnd = new MediaGalleryFrontEnd()
                {
                    Title = source.TitleAr,
                    Slug = source.Slug,
                    MetaTitle = source.MetaTitleAr,
                    MetaDescription = source.MetaDescriptionAr,
                    Summary = source.SummaryAr,
                    HeaderImage = source.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.HeaderImage}" : null,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.Image}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };
            }

            return mediaGalleryFrontEnd;
        }

        public MediaGalleryFrontEnd Map(MediaGallery source, MediaGalleryFrontEnd destination)
        {
            return Map(source);
        }

        public List<MediaGalleryFrontEnd> Map(List<MediaGallery> source)
        {
            var output = new List<MediaGalleryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<MediaGalleryFrontEnd> Map(List<MediaGallery> source, List<MediaGalleryFrontEnd> destination)
        {
            return Map(source);
        }

        /// <summary>
        /// Media Gallery With Details Mapper
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public MediaGalleryFrontEnd Map(MediaGalleryWithDetails source)
        {
            MediaGalleryFrontEnd mediaGalleryFrontEnd = new MediaGalleryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                mediaGalleryFrontEnd = new MediaGalleryFrontEnd()
                {
                    Title = source.MediaGallery.TitleEn,
                    Slug = source.MediaGallery.Slug,
                    MetaTitle = source.MediaGallery.MetaTitleEn,
                    MetaDescription= source.MediaGallery.MetaDescriptionEn,
                    Summary = source.MediaGallery.SummaryEn,
                    HeaderImage = source.MediaGallery.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.MediaGallery.HeaderImage}" : null,
                    Image = source.MediaGallery.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.MediaGallery.Image}" : null,
                    Order = source.MediaGallery.Order,
                    IsActive = source.MediaGallery.IsActive
                };
            }
            else
            {
                mediaGalleryFrontEnd = new MediaGalleryFrontEnd()
                {
                    Title = source.MediaGallery.TitleAr,
                    Slug = source.MediaGallery.Slug,
                    MetaTitle = source.MediaGallery.MetaTitleAr,
                    MetaDescription = source.MediaGallery.MetaDescriptionAr,
                    Summary = source.MediaGallery.SummaryAr,
                    HeaderImage = source.MediaGallery.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.MediaGallery.HeaderImage}" : null,
                    Image = source.MediaGallery.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/mediaGallery/{source.MediaGallery.Image}" : null,
                    Order = source.MediaGallery.Order,
                    IsActive = source.MediaGallery.IsActive
                };
            }
            if (!source.Medias.IsNullOrEmpty())
            {
                mediaGalleryFrontEnd.Medias = _objectMapper.Map<List<Media>, List<MediaFrontEnd>>(source.Medias);
            }
           
            return mediaGalleryFrontEnd;
        }

        public MediaGalleryFrontEnd Map(MediaGalleryWithDetails source, MediaGalleryFrontEnd destination)
        {
            return Map(source);
        }

        public List<MediaGalleryFrontEnd> Map(List<MediaGalleryWithDetails> source)
        {
            var output = new List<MediaGalleryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }


        public List<MediaGalleryFrontEnd> Map(List<MediaGalleryWithDetails> source, List<MediaGalleryFrontEnd> destination)
        {
            return Map(source);
        }


    }
}

