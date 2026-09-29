using Volo.Abp.Application.Dtos;
using System;

namespace DIP.EFormServiceSubCategories
{
    public class GetEFormServiceSubCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? File { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? EFormServiceId { get; set; }

        public GetEFormServiceSubCategoriesInput()
        {

        }
    }
}