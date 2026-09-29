using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace DIP.ContactForms
{
    public class ContactForm : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string FullName { get; set; }

        [NotNull]
        public virtual string Email { get; set; }

        [CanBeNull]
        public virtual string? Subject { get; set; }

        [CanBeNull]
        public virtual string? Message { get; set; }

        public ContactForm()
        {

        }

        public ContactForm(Guid id, string fullName, string email, string subject, string message)
        {

            Id = id;
            Check.NotNull(fullName, nameof(fullName));
            Check.NotNull(email, nameof(email));
            FullName = fullName;
            Email = email;
            Subject = subject;
            Message = message;
        }

    }
}