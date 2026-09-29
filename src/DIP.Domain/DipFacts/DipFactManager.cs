using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.DipFacts
{
    public class DipFactManager : DomainService
    {
        private readonly IDipFactRepository _dipFactRepository;

        public DipFactManager(IDipFactRepository dipFactRepository)
        {
            _dipFactRepository = dipFactRepository;
        }

        public async Task<DipFact> CreateAsync(
        string image, string titleAr, string titleEn, string descriptionAr, string descriptionEn, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(descriptionAr, nameof(descriptionAr));
            Check.NotNullOrWhiteSpace(descriptionEn, nameof(descriptionEn));

            var dipFact = new DipFact(
             GuidGenerator.Create(),
             image, titleAr, titleEn, descriptionAr, descriptionEn, order, isActive
             );

            return await _dipFactRepository.InsertAsync(dipFact);
        }

        public async Task<DipFact> UpdateAsync(
            Guid id,
            string image, string titleAr, string titleEn, string descriptionAr, string descriptionEn, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(descriptionAr, nameof(descriptionAr));
            Check.NotNullOrWhiteSpace(descriptionEn, nameof(descriptionEn));

            var dipFact = await _dipFactRepository.GetAsync(id);

            dipFact.Image = image;
            dipFact.TitleAr = titleAr;
            dipFact.TitleEn = titleEn;
            dipFact.DescriptionAr = descriptionAr;
            dipFact.DescriptionEn = descriptionEn;
            dipFact.Order = order;
            dipFact.IsActive = isActive;

            dipFact.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _dipFactRepository.UpdateAsync(dipFact);
        }

    }
}