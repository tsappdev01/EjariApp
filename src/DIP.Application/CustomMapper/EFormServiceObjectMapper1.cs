using DIP.EFormServices;
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
    public class EFormServiceObjectMapper : IObjectMapper<EFormService, EFormServiceFrontEnd>,
        IObjectMapper<List<EFormService>, List<EFormServiceFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public EFormServiceObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<EFormServiceFrontEnd> Map(List<EFormService> source)
        {

            var output = new List<EFormServiceFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<EFormServiceFrontEnd> Map(List<EFormService> source, List<EFormServiceFrontEnd> destination)
        {
            return Map(source);
        }

        public EFormServiceFrontEnd Map(EFormService source)
        {

            EFormServiceFrontEnd EFormServiceFront = new EFormServiceFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                EFormServiceFront = new EFormServiceFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                };
            }
            else
            {
                EFormServiceFront = new EFormServiceFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                };

            }

            return EFormServiceFront;
        }

        public EFormServiceFrontEnd Map(EFormService source, EFormServiceFrontEnd destination)
        {
            return Map(source);
        }
    }
}


