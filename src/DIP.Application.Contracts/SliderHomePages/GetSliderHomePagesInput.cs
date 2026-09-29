using Volo.Abp.Application.Dtos;
using System;

namespace DIP.SliderHomePages
{
    public class GetSliderHomePagesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? ButtonTitleAr { get; set; }
        public string? ButtonTitleEn { get; set; }
        public string? ButtonUrlEn { get; set; }
        public string? ButtonUrlAr { get; set; }
        public string? Image { get; set; }
        public string? YoutubeUrl { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }

        public GetSliderHomePagesInput()
        {

        }
    }
}