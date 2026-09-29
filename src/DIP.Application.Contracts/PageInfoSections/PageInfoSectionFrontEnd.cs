using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.PageInfoSections
{
    public class PageInfoSectionFrontEnd
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public string? PageSectionMedia { get; set; }
        public string? YoutubeUrl { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid PageInfoId { get; set; }
    }
}