using DIP.Amenities;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.AmenityParagraphs
{
    public class AmenityParagraphWithNavigationPropertiesDto
    {
        public AmenityParagraphDto AmenityParagraph { get; set; }

        public AmenityDto Amenity { get; set; }

    }
}