using Volo.Abp.Application.Dtos;
using System;

namespace DIP.InquiryForms
{
    public class GetInquiryFormsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? CompanyName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Phone { get; set; }
        public string? InquiryType { get; set; }
        public string? TradeLicensePlateOfIssue { get; set; }
        public string? BuyRent { get; set; }
        public double? SpaceInSquareFeetMin { get; set; }
        public double? SpaceInSquareFeetMax { get; set; }
        public string? Comments { get; set; }

        public GetInquiryFormsInput()
        {

        }
    }
}