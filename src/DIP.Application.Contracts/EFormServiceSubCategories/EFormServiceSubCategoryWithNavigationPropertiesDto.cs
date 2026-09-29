using DIP.EFormServices;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace DIP.EFormServiceSubCategories
{
    public class EFormServiceSubCategoryWithNavigationPropertiesDto
    {
        public EFormServiceSubCategoryDto EFormServiceSubCategory { get; set; }

        public EFormServiceDto EFormService { get; set; }

    }
}