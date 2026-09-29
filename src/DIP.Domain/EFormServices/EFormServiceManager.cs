using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.EFormServices
{
    public class EFormServiceManager : DomainService
    {
        private readonly IEFormServiceRepository _eFormServiceRepository;

        public EFormServiceManager(IEFormServiceRepository eFormServiceRepository)
        {
            _eFormServiceRepository = eFormServiceRepository;
        }

        public async Task<EFormService> CreateAsync(
        string titleEn, string titleAr, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var eFormService = new EFormService(
             GuidGenerator.Create(),
             titleEn, titleAr, order, isActive
             );

            return await _eFormServiceRepository.InsertAsync(eFormService);
        }

        public async Task<EFormService> UpdateAsync(
            Guid id,
            string titleEn, string titleAr, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));

            var eFormService = await _eFormServiceRepository.GetAsync(id);

            eFormService.TitleEn = titleEn;
            eFormService.TitleAr = titleAr;
            eFormService.Order = order;
            eFormService.IsActive = isActive;

            eFormService.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _eFormServiceRepository.UpdateAsync(eFormService);
        }

    }
}