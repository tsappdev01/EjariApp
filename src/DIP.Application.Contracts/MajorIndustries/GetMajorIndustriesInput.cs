using Volo.Abp.Application.Dtos;
using System;

namespace DIP.MajorIndustries
{
    public class GetMajorIndustriesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? Image { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ZoneId { get; set; }

        public GetMajorIndustriesInput()
        {

        }
    }
}