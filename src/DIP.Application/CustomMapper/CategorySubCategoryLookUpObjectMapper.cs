
using DIP.Amenities;
using DIP.Categories;
using DIP.Medias;
using DIP.SubCategories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;


namespace DIP.CustomMapper
{
    public class CategorySubCategoryLookUpObjectMapper : IObjectMapper<Category, CategorySubCategoryLookUp>,
        IObjectMapper<List<Category>, List<CategorySubCategoryLookUp>>, IObjectMapper<SubCategoryWithNavigationProperties, CategorySubCategoryLookUp>,
        IObjectMapper<List<SubCategoryWithNavigationProperties>, List<CategorySubCategoryLookUp>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public CategorySubCategoryLookUpObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public CategorySubCategoryLookUp Map(Category source)
        {
            CategorySubCategoryLookUp categorySubCategoryLookUp = new CategorySubCategoryLookUp();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                categorySubCategoryLookUp = new CategorySubCategoryLookUp()
                {
                    Id= source.Id,
                    Title = source.TitleEn,
                    Order = source.Order,
                    IsCategory = true
                };
            }
            else
            {
                categorySubCategoryLookUp = new CategorySubCategoryLookUp()
                {
                    Id = source.Id,
                    Title = source.TitleAr,
                    Order = source.Order,
                    IsCategory = true
                };
            }           
            return categorySubCategoryLookUp;
        }

        public CategorySubCategoryLookUp Map(Category source, CategorySubCategoryLookUp destination)
        {
            return Map(source);
        }

        public List<CategorySubCategoryLookUp> Map(List<Category> source)
        {
            var output = new List<CategorySubCategoryLookUp>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<CategorySubCategoryLookUp> Map(List<Category> source, List<CategorySubCategoryLookUp> destination)
        {
            return Map(source);
        }

        public CategorySubCategoryLookUp Map(SubCategoryWithNavigationProperties source)
        {
            CategorySubCategoryLookUp categorySubCategoryLookUp = new CategorySubCategoryLookUp();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                categorySubCategoryLookUp = new CategorySubCategoryLookUp()
                {
                    Id = source.SubCategory.Id,
                    Title = source.SubCategory.TitleEn,
                    ParentTitle = source.Category.TitleEn,
                    Order = source.SubCategory.Order,
                    IsCategory = false
                };
            }
            else
            {
                categorySubCategoryLookUp = new CategorySubCategoryLookUp()
                {
                    Id = source.SubCategory.Id,
                    Title = source.SubCategory.TitleAr,
                    ParentTitle = source.Category.TitleAr,
                    Order = source.SubCategory.Order,
                    IsCategory = false
                };
            }
            return categorySubCategoryLookUp;
        }

        public CategorySubCategoryLookUp Map(SubCategoryWithNavigationProperties source, CategorySubCategoryLookUp destination)
        {
            return Map(source);
        }

        public List<CategorySubCategoryLookUp> Map(List<SubCategoryWithNavigationProperties> source)
        {
            var output = new List<CategorySubCategoryLookUp>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<CategorySubCategoryLookUp> Map(List<SubCategoryWithNavigationProperties> source, List<CategorySubCategoryLookUp> destination)
        {
            return Map(source);
        }
    }
}

