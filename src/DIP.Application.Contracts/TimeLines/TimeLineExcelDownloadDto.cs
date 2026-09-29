using Volo.Abp.Application.Dtos;
using System;

namespace DIP.TimeLines
{
    public class TimeLineExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Image { get; set; }
        public DateTime? TimeLineDateMin { get; set; }
        public DateTime? TimeLineDateMax { get; set; }
        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? TimeLineCategoryId { get; set; }

        public TimeLineExcelDownloadDto()
        {

        }
    }
}