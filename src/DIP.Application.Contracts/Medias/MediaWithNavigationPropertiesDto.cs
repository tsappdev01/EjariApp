using DIP.ZoneParagraphs;
using DIP.AmenityParagraphs;
using DIP.MediaGalleries;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.Medias
{
    public class MediaWithNavigationPropertiesDto
    {
        public MediaDto Media { get; set; }

        public ZoneParagraphDto ZoneParagraph { get; set; }
        public AmenityParagraphDto AmenityParagraph { get; set; }
        public MediaGalleryDto MediaGallery { get; set; }

    }
}