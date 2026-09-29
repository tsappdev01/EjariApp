using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.FeedBacks
{
    public class FeedBackManager : DomainService
    {
        private readonly IFeedBackRepository _feedBackRepository;

        public FeedBackManager(IFeedBackRepository feedBackRepository)
        {
            _feedBackRepository = feedBackRepository;
        }

        public async Task<FeedBack> CreateAsync(
        string subject, string companyName, string plotNo, string plotCategory, string contactPersonName, string emailId, string mobileNumber, string department, string categoryName, string description)
        {
            Check.NotNullOrWhiteSpace(subject, nameof(subject));
            Check.NotNullOrWhiteSpace(companyName, nameof(companyName));
            Check.NotNullOrWhiteSpace(plotNo, nameof(plotNo));
            Check.NotNullOrWhiteSpace(plotCategory, nameof(plotCategory));
            Check.NotNullOrWhiteSpace(contactPersonName, nameof(contactPersonName));
            Check.NotNullOrWhiteSpace(emailId, nameof(emailId));
            Check.NotNullOrWhiteSpace(mobileNumber, nameof(mobileNumber));
            Check.NotNullOrWhiteSpace(department, nameof(department));
            Check.NotNullOrWhiteSpace(categoryName, nameof(categoryName));
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var feedBack = new FeedBack(
             GuidGenerator.Create(),
             subject, companyName, plotNo, plotCategory, contactPersonName, emailId, mobileNumber, department, categoryName, description
             );

            return await _feedBackRepository.InsertAsync(feedBack);
        }

        public async Task<FeedBack> UpdateAsync(
            Guid id,
            string subject, string companyName, string plotNo, string plotCategory, string contactPersonName, string emailId, string mobileNumber, string department, string categoryName, string description, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(subject, nameof(subject));
            Check.NotNullOrWhiteSpace(companyName, nameof(companyName));
            Check.NotNullOrWhiteSpace(plotNo, nameof(plotNo));
            Check.NotNullOrWhiteSpace(plotCategory, nameof(plotCategory));
            Check.NotNullOrWhiteSpace(contactPersonName, nameof(contactPersonName));
            Check.NotNullOrWhiteSpace(emailId, nameof(emailId));
            Check.NotNullOrWhiteSpace(mobileNumber, nameof(mobileNumber));
            Check.NotNullOrWhiteSpace(department, nameof(department));
            Check.NotNullOrWhiteSpace(categoryName, nameof(categoryName));
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var feedBack = await _feedBackRepository.GetAsync(id);

            feedBack.Subject = subject;
            feedBack.CompanyName = companyName;
            feedBack.PlotNo = plotNo;
            feedBack.PlotCategory = plotCategory;
            feedBack.ContactPersonName = contactPersonName;
            feedBack.EmailId = emailId;
            feedBack.MobileNumber = mobileNumber;
            feedBack.Department = department;
            feedBack.CategoryName = categoryName;
            feedBack.Description = description;

            feedBack.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _feedBackRepository.UpdateAsync(feedBack);
        }

    }
}