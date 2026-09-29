using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.SliderHomePages
{
    public class SliderHomePageFrontEnd : FullAuditedEntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ButtonTitle { get; set; }
        public string? ButtonUrl { get; set; }
        public string? Image { get; set; }
        public string? YoutubeUrl { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
}