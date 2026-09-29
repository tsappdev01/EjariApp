using DIP.Categories;
using DIP.SubCategories;

using System;
using System.Collections.Generic;

namespace DIP.Commercials
{
    public class CommercialWithDetails
    {
        public Commercial Commercial { get; set; }
        public SubCategory SubCategory { get; set; }
        public Category Category { get; set; }
        

        
    }
}