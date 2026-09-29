using DIP.Categories;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.SubCategories
{
    public class SubCategoryWithNavigationPropertiesDto
    {
        public SubCategoryDto SubCategory { get; set; }

        public CategoryDto Category { get; set; }

    }
}