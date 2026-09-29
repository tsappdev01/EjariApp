using DIP.EFormServiceSubCategories;

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
    public class EFormServiceSubCategoryObjectMapper : IObjectMapper<EFormServiceSubCategory, EFormServiceSubCategoryFrontEnd>,
        IObjectMapper<List<EFormServiceSubCategory>, List<EFormServiceSubCategoryFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public EFormServiceSubCategoryObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<EFormServiceSubCategoryFrontEnd> Map(List<EFormServiceSubCategory> source)
        {

            var output = new List<EFormServiceSubCategoryFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<EFormServiceSubCategoryFrontEnd> Map(List<EFormServiceSubCategory> source, List<EFormServiceSubCategoryFrontEnd> destination)
        {
            return Map(source);
        }

        public EFormServiceSubCategoryFrontEnd Map(EFormServiceSubCategory source)
        {

            EFormServiceSubCategoryFrontEnd EFormServiceSubCategoryFront = new EFormServiceSubCategoryFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                EFormServiceSubCategoryFront = new EFormServiceSubCategoryFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    EFormServiceId = source.EFormServiceId,
                    File = source.File != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/eformServiceSubCategory/{source.File}" : null,
                };
            }
            else
            {
                EFormServiceSubCategoryFront = new EFormServiceSubCategoryFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                    EFormServiceId = source.EFormServiceId,
                    File = source.File != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/eformServiceSubCategory/{source.File}" : null,
                };

            }

            return EFormServiceSubCategoryFront;
        }

        public EFormServiceSubCategoryFrontEnd Map(EFormServiceSubCategory source, EFormServiceSubCategoryFrontEnd destination)
        {
            return Map(source);
        }
    }
}


