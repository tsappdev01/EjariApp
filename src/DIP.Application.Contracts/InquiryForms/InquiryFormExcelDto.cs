using System;

namespace DIP.InquiryForms
{
    public class InquiryFormExcelDto
    {
        public string? CompanyName { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Phone { get; set; }
        public string? InquiryType { get; set; }
        public string? TradeLicensePlateOfIssue { get; set; }
        public string? BuyRent { get; set; }
        public double SpaceInSquareFeet { get; set; }
        public string? Comments { get; set; }
    }
}