using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.InquiryForms
{
    public class InquiryFormManager : DomainService
    {
        private readonly IInquiryFormRepository _inquiryFormRepository;

        public InquiryFormManager(IInquiryFormRepository inquiryFormRepository)
        {
            _inquiryFormRepository = inquiryFormRepository;
        }

        public async Task<InquiryForm> CreateAsync(
        string companyName, string name, string email, string mobile, string fax, string phone, string inquiryType, string tradeLicensePlateOfIssue, string buyRent, double spaceInSquareFeet, string comments)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(mobile, nameof(mobile));

            var inquiryForm = new InquiryForm(
             GuidGenerator.Create(),
             companyName, name, email, mobile, fax, phone, inquiryType, tradeLicensePlateOfIssue, buyRent, spaceInSquareFeet, comments
             );

            return await _inquiryFormRepository.InsertAsync(inquiryForm);
        }

        public async Task<InquiryForm> UpdateAsync(
            Guid id,
            string companyName, string name, string email, string mobile, string fax, string phone, string inquiryType, string tradeLicensePlateOfIssue, string buyRent, double spaceInSquareFeet, string comments, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(mobile, nameof(mobile));

            var inquiryForm = await _inquiryFormRepository.GetAsync(id);

            inquiryForm.CompanyName = companyName;
            inquiryForm.Name = name;
            inquiryForm.Email = email;
            inquiryForm.Mobile = mobile;
            inquiryForm.Fax = fax;
            inquiryForm.Phone = phone;
            inquiryForm.InquiryType = inquiryType;
            inquiryForm.TradeLicensePlateOfIssue = tradeLicensePlateOfIssue;
            inquiryForm.BuyRent = buyRent;
            inquiryForm.SpaceInSquareFeet = spaceInSquareFeet;
            inquiryForm.Comments = comments;

            inquiryForm.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _inquiryFormRepository.UpdateAsync(inquiryForm);
        }

    }
}