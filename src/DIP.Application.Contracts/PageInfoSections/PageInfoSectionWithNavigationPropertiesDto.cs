using DIP.PageInfos;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.PageInfoSections
{
    public class PageInfoSectionWithNavigationPropertiesDto
    {
        public PageInfoSectionDto PageInfoSection { get; set; }

        public PageInfoDto PageInfo { get; set; }

    }
}