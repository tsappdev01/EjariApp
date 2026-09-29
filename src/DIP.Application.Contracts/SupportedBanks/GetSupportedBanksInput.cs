using Volo.Abp.Application.Dtos;
using System;

namespace DIP.SupportedBanks
{
    public class GetSupportedBanksInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? TitleAr { get; set; }
        public string? TitleEn { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }

        public GetSupportedBanksInput()
        {

        }
    }
}