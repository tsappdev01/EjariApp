using DIP.Medias;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.MediaGalleries
{
    public class MediaGalleryFrontEnd 
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Summary { get; set; }
        public string? HeaderImage { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public List<MediaFrontEnd> Medias { get; set; }
    }
}