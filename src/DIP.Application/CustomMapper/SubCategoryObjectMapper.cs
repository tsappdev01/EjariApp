using DIP.EFormServiceSubCategories;
using DIP.SubCategories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace DIP.CustomMapper
{
    public class SubCategoryObjectMapper : IObjectMapper<SubCategory, SubCategoryFrontEnd>,
        IObjectMapper<List<SubCategory>, List<SubCategoryFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public SubCategoryObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<SubCategoryFrontEnd> Map(List<SubCategory> source)
        {

            var output = new List<SubCategoryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<SubCategoryFrontEnd> Map(List<SubCategory> source, List<SubCategoryFrontEnd> destination)
        {
            return Map(source);
        }

        public SubCategoryFrontEnd Map(SubCategory source)
        {

            SubCategoryFrontEnd SubCategoryFront = new SubCategoryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                SubCategoryFront = new SubCategoryFrontEnd()
                {
                    Id = source.Id,                   
                    Title = source.TitleEn,
                    Order = source.Order,
                    IsFeature= source.IsFeature,
                    IsActive = source.IsActive,
                    CategoryId = source.CategoryId,
                };
            }
            else
            {
                SubCategoryFront = new SubCategoryFrontEnd()
                {
                    Id = source.Id,
                    Title = source.TitleAr,
                    Order = source.Order,
                    IsFeature = source.IsFeature,
                    IsActive = source.IsActive,
                    CategoryId = source.CategoryId,
                };

            }

            return SubCategoryFront;
        }

        public SubCategoryFrontEnd Map(SubCategory source, SubCategoryFrontEnd destination)
        {
            return Map(source);
        }
    }
}


