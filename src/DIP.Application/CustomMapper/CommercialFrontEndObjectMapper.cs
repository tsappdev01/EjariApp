
using DIP.Amenities;
using DIP.AmenityParagraphs;
using DIP.Commercials;
using DIP.Medias;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;


namespace DIP.CustomMapper
{
    public class CommercialFrontEndObjectMapper : IObjectMapper<CommercialWithDetails, CommercialFrontEnd>,
        IObjectMapper<List<CommercialWithDetails>, List<CommercialFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;
        public CommercialFrontEndObjectMapper(IObjectMapper objectMapper)
        {
            _objectMapper = objectMapper;
        }

        public CommercialFrontEnd Map(CommercialWithDetails source)
        {
            CommercialFrontEnd commerciaFrontEnd = new CommercialFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                commerciaFrontEnd = new CommercialFrontEnd()
                {
                    Title = source.Commercial.TitleEn,
                    PlotNo = source.Commercial.PlotNo,
                    Activity = source.Commercial.ActivityEn,
                    Phone = source.Commercial.Phone,
                    Fax = source.Commercial.Fax,
                    MakaniNo = source.Commercial.MakaniNo,
                    Category = source.Category != null? source.Category.TitleEn : null,
                    Order = source.Commercial.Order,
                    IsActive = source.Commercial.IsActive
                };
            }
            else
            {
                commerciaFrontEnd = new CommercialFrontEnd()
                {
                    Title = source.Commercial.TitleAr,
                    PlotNo = source.Commercial.PlotNo,
                    Activity = source.Commercial.ActivityAr,
                    Phone = source.Commercial.Phone,
                    Fax = source.Commercial.Fax,
                    MakaniNo = source.Commercial.MakaniNo,
                    Category = source.Category != null ? source.Category.TitleAr : null,
                    Order = source.Commercial.Order,
                    IsActive = source.Commercial.IsActive
                };
            }           
            return commerciaFrontEnd;
        }

        public CommercialFrontEnd Map(CommercialWithDetails source, CommercialFrontEnd destination)
        {
            return Map(source);
        }

        public List<CommercialFrontEnd> Map(List<CommercialWithDetails> source)
        {
            var output = new List<CommercialFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<CommercialFrontEnd> Map(List<CommercialWithDetails> source, List<CommercialFrontEnd> destination)
        {
            return Map(source);
        }
    }
}

