using Volo.Abp.Application.Dtos;
using System;

namespace DIP.Commercials
{
    public class CommercialExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? PlotNo { get; set; }
        public string? ActivityEn { get; set; }
        public string? ActivityAr { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? MakaniNo { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public Guid? SubCategoryId { get; set; }

        public CommercialExcelDownloadDto()
        {

        }
    }
}