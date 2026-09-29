using DIP.Zones;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.MajorIndustries
{
    public class MajorIndustryWithNavigationPropertiesDto
    {
        public MajorIndustryDto MajorIndustry { get; set; }

        public ZoneDto Zone { get; set; }

    }
}