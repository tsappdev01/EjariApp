using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.ContactForms
{
    public class ContactFormManager : DomainService
    {
        private readonly IContactFormRepository _contactFormRepository;

        public ContactFormManager(IContactFormRepository contactFormRepository)
        {
            _contactFormRepository = contactFormRepository;
        }

        public async Task<ContactForm> CreateAsync(
        string fullName, string email, string subject, string message)
        {
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));
            Check.NotNullOrWhiteSpace(email, nameof(email));

            var contactForm = new ContactForm(
             GuidGenerator.Create(),
             fullName, email, subject, message
             );

            return await _contactFormRepository.InsertAsync(contactForm);
        }

        public async Task<ContactForm> UpdateAsync(
            Guid id,
            string fullName, string email, string subject, string message, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(fullName, nameof(fullName));
            Check.NotNullOrWhiteSpace(email, nameof(email));

            var contactForm = await _contactFormRepository.GetAsync(id);

            contactForm.FullName = fullName;
            contactForm.Email = email;
            contactForm.Subject = subject;
            contactForm.Message = message;

            contactForm.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _contactFormRepository.UpdateAsync(contactForm);
        }

    }
}