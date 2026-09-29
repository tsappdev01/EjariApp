using DIP.SubCategories;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.Commercials
{
    public class CommercialWithNavigationPropertiesDto
    {
        public CommercialDto Commercial { get; set; }

        public SubCategoryDto SubCategory { get; set; }

    }
}