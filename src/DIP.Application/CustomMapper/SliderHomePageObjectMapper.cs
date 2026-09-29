using DIP.EFormServiceSubCategories;
using DIP.SliderHomePages;
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
    public class SliderHomePageObjectMapper : IObjectMapper<SliderHomePage, SliderHomePageFrontEnd>,
        IObjectMapper<List<SliderHomePage>, List<SliderHomePageFrontEnd>>,
        ITransientDependency
    {
        private readonly IObjectMapper _objectMapper;

        public SliderHomePageObjectMapper(

           IObjectMapper objectMapper
            )
        {
            _objectMapper = objectMapper;
        }

        public List<SliderHomePageFrontEnd> Map(List<SliderHomePage> source)
        {

            var output = new List<SliderHomePageFrontEnd>();
            foreach (var item in source)
            {
                output.Add(Map(item));
            }

            return output;
        }

        public List<SliderHomePageFrontEnd> Map(List<SliderHomePage> source, List<SliderHomePageFrontEnd> destination)
        {
            return Map(source);
        }

        public SliderHomePageFrontEnd Map(SliderHomePage source)
        {

            SliderHomePageFrontEnd SliderHomePageFront = new SliderHomePageFrontEnd();
            if (CultureInfo.CurrentCulture.Name.StartsWith("en"))
            {
                SliderHomePageFront = new SliderHomePageFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleEn,
                    Order = source.Order,
                    Description = source.DescriptionEn,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/sliderHome/{source.Image}" : null,
                    ButtonTitle =source.ButtonTitleEn,
                    ButtonUrl =source.ButtonUrlEn != null ? source.ButtonUrlEn : source.ButtonUrlAr,
                    YoutubeUrl =source.YoutubeUrl
                };
            }
            else
            {
                SliderHomePageFront = new SliderHomePageFrontEnd()
                {
                    Id = source.Id,
                    IsActive = source.IsActive,
                    Title = source.TitleAr,
                    Order = source.Order,
                    Description = source.DescriptionAr,
                    Image = source.Image != null ? $"{MimeTypes.MimeTypeMap.GetAttachmentPath()}/sliderHome/{source.Image}" : null,
                    ButtonTitle = source.ButtonTitleAr,
                    ButtonUrl = source.ButtonUrlAr != null ? source.ButtonUrlAr : source.ButtonUrlEn,
                    YoutubeUrl = source.YoutubeUrl
                };

            }

            return SliderHomePageFront;
        }

        public SliderHomePageFrontEnd Map(SliderHomePage source, SliderHomePageFrontEnd destination)
        {
            return Map(source);
        }
    }
}


