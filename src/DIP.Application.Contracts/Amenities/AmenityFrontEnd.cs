using DIP.AmenityParagraphs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.Amenities
{
    public class AmenityFrontEnd
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public  string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public string Image { get; set; }
        public string HeaderImage { get; set; }
        public List<AmenityParagraphFrontEnd> AmenityParagraphFrontEnd { get; set; }
    }
}