using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.SupportedBanks
{
    public class SupportedBankManager : DomainService
    {
        private readonly ISupportedBankRepository _supportedBankRepository;

        public SupportedBankManager(ISupportedBankRepository supportedBankRepository)
        {
            _supportedBankRepository = supportedBankRepository;
        }

        public async Task<SupportedBank> CreateAsync(
        string titleAr, string titleEn, bool isActive, int order)
        {

            var supportedBank = new SupportedBank(
             GuidGenerator.Create(),
             titleAr, titleEn, isActive, order
             );

            return await _supportedBankRepository.InsertAsync(supportedBank);
        }

        public async Task<SupportedBank> UpdateAsync(
            Guid id,
            string titleAr, string titleEn, bool isActive, int order, [CanBeNull] string concurrencyStamp = null
        )
        {

            var supportedBank = await _supportedBankRepository.GetAsync(id);

            supportedBank.TitleAr = titleAr;
            supportedBank.TitleEn = titleEn;
            supportedBank.IsActive = isActive;
            supportedBank.Order = order;

            supportedBank.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _supportedBankRepository.UpdateAsync(supportedBank);
        }

    }
}