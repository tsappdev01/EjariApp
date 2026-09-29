using DIP.EFormServiceSubCategories;
using DIP.SupportedBanks;
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
    public class SupportedBankObjectMapper : IObjectMapper<SupportedBank, SupportedBankFrontEnd>,
        IObjectMapper<List<SupportedBank>, List<SupportedBankFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public SupportedBankObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<SupportedBankFrontEnd> Map(List<SupportedBank> source)
        {

            var output = new List<SupportedBankFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<SupportedBankFrontEnd> Map(List<SupportedBank> source, List<SupportedBankFrontEnd> destination)
        {
            return Map(source);
        }

        public SupportedBankFrontEnd Map(SupportedBank source)
        {

            SupportedBankFrontEnd SupportedBankFront = new SupportedBankFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                SupportedBankFront = new SupportedBankFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                   



                };
            }
            else
            {
                SupportedBankFront = new SupportedBankFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
               


                };

            }

            return SupportedBankFront;
        }

        public SupportedBankFrontEnd Map(SupportedBank source, SupportedBankFrontEnd destination)
        {
            return Map(source);
        }
    }
}


