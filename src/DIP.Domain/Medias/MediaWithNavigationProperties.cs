using DIP.ZoneParagraphs;
using DIP.AmenityParagraphs;
using DIP.MediaGalleries;

using System;
using System.Collections.Generic;

namespace DIP.Medias
{
    public class MediaWithNavigationProperties
    {
        public Media Media { get; set; }

        public ZoneParagraph ZoneParagraph { get; set; }
        public AmenityParagraph AmenityParagraph { get; set; }
        public MediaGallery MediaGallery { get; set; }
        

        
    }
}