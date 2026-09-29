using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.DipBranches
{
    public class DipBranchManager : DomainService
    {
        private readonly IDipBranchRepository _dipBranchRepository;

        public DipBranchManager(IDipBranchRepository dipBranchRepository)
        {
            _dipBranchRepository = dipBranchRepository;
        }

        public async Task<DipBranch> CreateAsync(
        string titleEn, string titleAr, string subTitleEn, string subTitleAr, string phone, string alternativePhone, string email, string alternativeEmail, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var dipBranch = new DipBranch(
             GuidGenerator.Create(),
             titleEn, titleAr, subTitleEn, subTitleAr, phone, alternativePhone, email, alternativeEmail, order, isActive
             );

            return await _dipBranchRepository.InsertAsync(dipBranch);
        }

        public async Task<DipBranch> UpdateAsync(
            Guid id,
            string titleEn, string titleAr, string subTitleEn, string subTitleAr, string phone, string alternativePhone, string email, string alternativeEmail, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));

            var dipBranch = await _dipBranchRepository.GetAsync(id);

            dipBranch.TitleEn = titleEn;
            dipBranch.TitleAr = titleAr;
            dipBranch.SubTitleEn = subTitleEn;
            dipBranch.SubTitleAr = subTitleAr;
            dipBranch.Phone = phone;
            dipBranch.AlternativePhone = alternativePhone;
            dipBranch.Email = email;
            dipBranch.AlternativeEmail = alternativeEmail;
            dipBranch.Order = order;
            dipBranch.IsActive = isActive;

            dipBranch.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _dipBranchRepository.UpdateAsync(dipBranch);
        }

    }
}