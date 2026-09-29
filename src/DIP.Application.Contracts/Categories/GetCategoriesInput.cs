using Volo.Abp.Application.Dtos;
using System;

namespace DIP.Categories
{
    public class GetCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsFeature { get; set; }
        public bool? IsActive { get; set; }

        public GetCategoriesInput()
        {

        }
    }
}