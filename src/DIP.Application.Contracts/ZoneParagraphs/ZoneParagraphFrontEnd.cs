using DIP.Medias;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphFrontEnd
    {
        public string Title { get; set; }
        public string? SubTitle { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public Guid ZoneId { get; set; }
        public List<MediaFrontEnd> Medias { get; set; }
    }
}