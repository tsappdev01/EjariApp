
using System;
using System.Collections.Generic;
using System.Globalization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using DIP.EFormServices;
using DIP.MajorIndustries;
using DIP.EFormServiceSubCategories;

namespace DIP.CustomMapper
{
    public class EFormServiceWithDetailsFrontEndObjectMapper : IObjectMapper<EFromServiceWithDetails, EFormServiceFrontEnd>,
        IObjectMapper<List<EFromServiceWithDetails>, List<EFormServiceFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public EFormServiceWithDetailsFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }
        
        public EFormServiceFrontEnd Map(EFromServiceWithDetails source)
        {
            EFormServiceFrontEnd EFormServiceFrontEnd = new EFormServiceFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                EFormServiceFrontEnd = new EFormServiceFrontEnd()
                {
                    Title = source.EFormService.TitleEn,                   
                    Order = source.EFormService.Order,
                    IsActive = source.EFormService.IsActive,
                };
            }
            else
            {
                EFormServiceFrontEnd = new EFormServiceFrontEnd()
                {
                    Title = source.EFormService.TitleAr,
                    Order = source.EFormService.Order,
                    IsActive = source.EFormService.IsActive
                };
            }
            if (!source.EFormServiceSubCategories.IsNullOrEmpty())
            {
                EFormServiceFrontEnd.EFormServiceSubCategories = _objectMapper.Map<List<EFormServiceSubCategory>, List<EFormServiceSubCategoryFrontEnd>>(source.EFormServiceSubCategories);
            }
            return EFormServiceFrontEnd;
        }

        public EFormServiceFrontEnd Map(EFromServiceWithDetails source, EFormServiceFrontEnd destination)
        {
            return Map(source);
        }

        public List<EFormServiceFrontEnd> Map(List<EFromServiceWithDetails> source)
        {
            var output = new List<EFormServiceFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<EFormServiceFrontEnd> Map(List<EFromServiceWithDetails>  source, List<EFormServiceFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

