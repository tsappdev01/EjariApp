using Volo.Abp.Application.Dtos;
using System;

namespace DIP.LastEventss
{
    public class GetLastEventssInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public string? MetaTitleAr { get; set; }
        public string? MetaTitleEn { get; set; }
        public string? MetaDescriptionEn { get; set; }
        public string? MetaDescriptionAr { get; set; }
        public bool? IsFeatured { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
        public string? HeaderImage { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? SummaryEn { get; set; }
        public string? SummaryAr { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public DateTime? StartDateMin { get; set; }
        public DateTime? StartDateMax { get; set; }
        public DateTime? EndDateMin { get; set; }
        public DateTime? EndDateMax { get; set; }
        public bool? IsActive { get; set; }
        public string? LocationAr { get; set; }
        public string? LocationEn { get; set; }

        public GetLastEventssInput()
        {

        }
    }
}