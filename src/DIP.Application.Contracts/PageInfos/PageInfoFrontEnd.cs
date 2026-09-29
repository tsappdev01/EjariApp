using DIP.PageInfoSections;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.PageInfos
{
    public class PageInfoFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? PageInfoArticleTilte { get; set; }
        public string? PageInfoArticleSubtitle { get; set; }
        public string? Description { get; set; }
        public string? Summary { get; set; }
        public int Order { get; set; }
        public List<PageInfoSectionFrontEnd> PageInfoSections { get; set; }
        public bool IsActive { get; set; }
    }
}