using DIP.MajorIndustries;
using DIP.ZoneParagraphs;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DIP.Zones
{
    public class ZoneFrontEnd
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? Summary { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public int Order { get; set; }
        public bool IsFeature { get; set; }
        public bool IsActive { get; set; }

        public List<ZoneParagraphFrontEnd> ZonParagraphs { get; set; }
        public List<MajorIndustryFrontEnd> MajorIndustries { get; set; }
    }
}