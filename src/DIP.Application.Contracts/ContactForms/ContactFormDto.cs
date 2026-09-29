using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace DIP.ContactForms
{
    public class ContactFormDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}