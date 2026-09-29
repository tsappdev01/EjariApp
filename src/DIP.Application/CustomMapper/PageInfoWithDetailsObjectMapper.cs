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
    public class PageInfoObjectMapper : IObjectMapper<PageInfoWithDetails, PageInfoFrontEnd>,
        IObjectMapper<List<PageInfoWithDetails>, List<PageInfoFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public PageInfoObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<PageInfoFrontEnd> Map(List<PageInfoWithDetails> source)
        {

            var output = new List<PageInfoFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<PageInfoFrontEnd> Map(List<PageInfoWithDetails> source, List<PageInfoFrontEnd> destination)
        {
            return Map(source);
        }

        public PageInfoFrontEnd Map(PageInfoWithDetails source)
        {

            PageInfoFrontEnd PageInfoFront = new PageInfoFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                PageInfoFront = new PageInfoFrontEnd()
                {
                    Id = source.PageInfo.Id,
                    IsActive = source.PageInfo.IsActive,
                    Title = source.PageInfo.TitleEn,
                    Order = source.PageInfo.Order,
                    PageInfoArticleTilte = source.PageInfo.PageInfoArticleTilteEn,
                    PageInfoArticleSubtitle = source.PageInfo.PageInfoArticleSubtitleEn,
                    Description = source.PageInfo.DescriptionEn,
                    HeaderImage = source.PageInfo.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pageInfo/{source.PageInfo.HeaderImage}" : null,
                    Image = source.PageInfo.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pageInfo/{source.PageInfo.Image}" : null,
                    MetaDescription =source.PageInfo.MetaDescriptionEn,
                    MetaTitle =source.PageInfo.MetaTitleEn,
                    Slug = source.PageInfo.Slug,
                    Summary =source.PageInfo.SummaryEn,
                    YouTubeUrl = source.PageInfo.YouTubeUrl,               
                    
                    
                };
            }
            else
            {
                PageInfoFront = new PageInfoFrontEnd()
                {
                    Id = source.PageInfo.Id,
                    IsActive = source.PageInfo.IsActive,
                    Title = source.PageInfo.TitleAr,
                    Order = source.PageInfo.Order,
                    PageInfoArticleTilte = source.PageInfo.PageInfoArticleTilteAr,
                    PageInfoArticleSubtitle = source.PageInfo.PageInfoArticleSubtitleAr,
                    Description = source.PageInfo.DescriptionAr,                   
                    HeaderImage = source.PageInfo.HeaderImage != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pageInfo/{source.PageInfo.HeaderImage}" : null,
                    Image = source.PageInfo.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/pageInfo/{source.PageInfo.Image}" : null,
                    MetaDescription = source.PageInfo.MetaDescriptionAr,
                    MetaTitle = source.PageInfo.MetaTitleAr,
                    Slug = source.PageInfo.Slug,
                    Summary = source.PageInfo.SummaryAr,
                    YouTubeUrl = source.PageInfo.YouTubeUrl,
                };

            }
            PageInfoFront.PageInfoSections = _objectMapper.Map<List<PageInfoSection>, List<PageInfoSectionFrontEnd>>(source.PageInfoSections);
            return PageInfoFront;
        }

        public PageInfoFrontEnd Map(PageInfoWithDetails source, PageInfoFrontEnd destination)
        {
            return Map(source);
        }
    }
}


