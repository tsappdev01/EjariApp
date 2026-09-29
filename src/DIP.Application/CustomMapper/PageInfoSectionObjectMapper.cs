using DIP.EFormServiceSubCategories;
using DIP.PageInfos;
using DIP.PageInfoSections;
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
    public class PageInfoSectionObjectMapper : IObjectMapper<PageInfoSection, PageInfoSectionFrontEnd>,
        IObjectMapper<List<PageInfoSection>, List<PageInfoSectionFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public PageInfoSectionObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<PageInfoSectionFrontEnd> Map(List<PageInfoSection> source)
        {

            var output = new List<PageInfoSectionFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<PageInfoSectionFrontEnd> Map(List<PageInfoSection> source, List<PageInfoSectionFrontEnd> destination)
        {
            return Map(source);
        }

        public PageInfoSectionFrontEnd Map(PageInfoSection source)
        {

            PageInfoSectionFrontEnd PageInfoFront = new PageInfoSectionFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                PageInfoFront = new PageInfoSectionFrontEnd()
                {
                    Title = source.TitleEn,
                    SubTitle = source.SubTitleEn,
                    Summary = source.SummaryEn,
                    Description = source.DescriptionEn,
                    PageSectionMedia = source.PageSectionMedia != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pageInfoSection/{source.PageSectionMedia}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };
            }
            else
            {
                PageInfoFront = new PageInfoSectionFrontEnd()
                {
                    Title = source.TitleAr,
                    SubTitle = source.SubTitleAr,
                    Summary = source.SummaryAr,
                    Description = source.DescriptionAr,
                    PageSectionMedia = source.PageSectionMedia != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pageInfoSection/{source.PageSectionMedia}" : null,
                    Order = source.Order,
                    IsActive = source.IsActive
                };

            }

            return PageInfoFront;
        }

        public PageInfoSectionFrontEnd Map(PageInfoSection source, PageInfoSectionFrontEnd destination)
        {
            return Map(source);
        }
    }
}


