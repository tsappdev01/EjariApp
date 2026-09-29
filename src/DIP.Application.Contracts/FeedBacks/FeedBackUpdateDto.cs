using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace DIP.FeedBacks
{
    public class FeedBackUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string PlotNo { get; set; }
        [Required]
        public string PlotCategory { get; set; }
        [Required]
        public string ContactPersonName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public string Description { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}