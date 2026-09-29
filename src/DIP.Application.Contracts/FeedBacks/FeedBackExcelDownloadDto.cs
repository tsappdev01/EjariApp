using Volo.Abp.Application.Dtos;
using System;

namespace DIP.FeedBacks
{
    public class FeedBackExcelDownloadDto
    {
        public string DownloadToken { get; set; }

        public string? FilterText { get; set; }

        public string? Subject { get; set; }
        public string? CompanyName { get; set; }
        public string? PlotNo { get; set; }
        public string? PlotCategory { get; set; }
        public string? ContactPersonName { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNumber { get; set; }
        public string? Department { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }

        public FeedBackExcelDownloadDto()
        {

        }
    }
}