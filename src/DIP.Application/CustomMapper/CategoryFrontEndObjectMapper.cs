
using DIP.Amenities;
using DIP.Categories;
using DIP.Medias;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;


namespace DIP.CustomMapper
{
    public class CategoryFrontEndObjectMapper : IObjectMapper<Category, CategoryFrontEnd>,
        IObjectMapper<List<Category>, List<CategoryFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public CategoryFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public CategoryFrontEnd Map(Category source)
        {
            CategoryFrontEnd categoryFrontEnd = new CategoryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                categoryFrontEnd = new CategoryFrontEnd()
                {
                    Id= source.Id,
                    Title = source.TitleEn,
                    Order = source.Order,
                    IsFeature = source.IsFeature,
                    IsActive = source.IsActive
                };
            }
            else
            {
                categoryFrontEnd = new CategoryFrontEnd()
                {
                    Id = source.Id,
                    Title = source.TitleAr,
                    Order = source.Order,
                    IsFeature = source.IsFeature,
                    IsActive = source.IsActive
                };
            }           
            return categoryFrontEnd;
        }

        public CategoryFrontEnd Map(Category source, CategoryFrontEnd destination)
        {
            return Map(source);
        }

        public List<CategoryFrontEnd> Map(List<Category> source)
        {
            var output = new List<CategoryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<CategoryFrontEnd> Map(List<Category> source, List<CategoryFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

