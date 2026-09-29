using Volo.Abp.Application.Dtos;
using System;

namespace DIP.EFormServices
{
    public class GetEFormServicesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }

        public GetEFormServicesInput()
        {

        }
    }
}