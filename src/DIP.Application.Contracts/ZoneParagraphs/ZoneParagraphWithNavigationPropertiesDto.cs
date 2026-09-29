using DIP.Zones;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.ZoneParagraphs
{
    public class ZoneParagraphWithNavigationPropertiesDto
    {
        public ZoneParagraphDto ZoneParagraph { get; set; }

        public ZoneDto Zone { get; set; }

    }
}