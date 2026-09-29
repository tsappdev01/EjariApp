using DIP.Medias;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphFrontEnd
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string? ButtonUrl { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid AmenityId { get; set; }
        public List<MediaFrontEnd> Medias { get; set; }

        public AmenityParagraphFrontEnd()
        { 
            Title = string.Empty;
            Description = string.Empty;
            SubTitle = string.Empty;
        }
    }
}