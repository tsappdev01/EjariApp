using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DIP.PageInfos
{
    public class PageInfoCreateDto
    {
        [Required]
        public string TitleAr { get; set; }
        [Required]
        public string TitleEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaDescriptionAr { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? PageInfoArticleTilteEn { get; set; }
        public string? PageInfoArticleTilteAr { get; set; }
        public string? PageInfoArticleSubtitleEn { get; set; }
        public string? PageInfoArticleSubtitleAr { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? SummaryEn { get; set; }
        public string? SummaryAr { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}