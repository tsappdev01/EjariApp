using DIP.EFormServiceSubCategories;
using DIP.Medias;
using DIP.TimeLines;
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
    public class MediaFrontEndObjectMapper : IObjectMapper<Media, MediaFrontEnd>,
        IObjectMapper<List<Media>, List<MediaFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public MediaFrontEndObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<MediaFrontEnd> Map(List<Media> source)
        {

            var output = new List<MediaFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<MediaFrontEnd> Map(List<Media> source, List<MediaFrontEnd> destination)
        {
            return Map(source);
        }

        public MediaFrontEnd Map(Media source)
        {

            MediaFrontEnd mediaFrontEnd = new MediaFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                mediaFrontEnd = new MediaFrontEnd()
                {
                    Title = source.TitleEn,
                    File = source.File != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/media/{source.File}" : null,
                    Order = source.Order,  
                    IsActive = source.IsActive
                };
            }
            else
            {
                mediaFrontEnd = new MediaFrontEnd()
                {
                    Title = source.TitleAr,
                    File = source.File != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/media/{source.File}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };

            }

            return mediaFrontEnd;
        }

        public MediaFrontEnd Map(Media source, MediaFrontEnd destination)
        {
            return Map(source);
        }
    }
}


