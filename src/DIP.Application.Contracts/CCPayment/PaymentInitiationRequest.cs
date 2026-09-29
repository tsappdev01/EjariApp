using System;
using System.ComponentModel.DataAnnotations;

namespace DIP.CCPayment
{
    public class PaymentInitiationRequest
    {
        [Required]
        [StringLength(30)]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? BillingName { get; set; }
        public string? BillingEmail { get; set; }
        public string? BillingTel { get; set; }
        public string? BillingNotes { get; set; } = string.Empty;
    }
}
