using DIP.TimeLineCategories;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.TimeLines
{
    public class TimeLineWithNavigationPropertiesDto
    {
        public TimeLineDto TimeLine { get; set; }

        public TimeLineCategoryDto TimeLineCategory { get; set; }

    }
}