using DIP.DipFacts;
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
    public class DipFactObjectMapper : IObjectMapper<DipFact, DipFactFrontEnd>,
        IObjectMapper<List<DipFact>, List<DipFactFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public DipFactObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<DipFactFrontEnd> Map(List<DipFact> source)
        {

            var output = new List<DipFactFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<DipFactFrontEnd> Map(List<DipFact> source, List<DipFactFrontEnd> destination)
        {
            return Map(source);
        }

        public DipFactFrontEnd Map(DipFact source)
        {

            DipFactFrontEnd DipFactFront = new DipFactFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                DipFactFront = new DipFactFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/dipFact/{source.Image}" : null,
                    Title = source.TitleEn,
                    Description = source.DescriptionEn,
                    Order = source.Order,
                };
            }
            else
            {
                DipFactFront = new DipFactFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/dipFact/{source.Image}" : null,
                    Title = source.TitleAr,
                    Description = source.DescriptionAr,
                    Order = source.Order,
                };

            }

            return DipFactFront;
        }

        public DipFactFrontEnd Map(DipFact source, DipFactFrontEnd destination)
        {
            return Map(source);
        }
    }
}


