using DIP.Categories;

using System;
using System.Collections.Generic;

namespace DIP.SubCategories
{
    public class SubCategoryWithNavigationProperties
    {
        public SubCategory SubCategory { get; set; }

        public Category Category { get; set; }
        

        
    }
}