using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.EServices
{
    public class EServiceManager : DomainService
    {
        private readonly IEServiceRepository _eServiceRepository;

        public EServiceManager(IEServiceRepository eServiceRepository)
        {
            _eServiceRepository = eServiceRepository;
        }

        public async Task<EService> CreateAsync(
        string titleEn, string titleAr, string slug, string descriptionEn, string descriptionAr, string image, string headerImage, string metaTitleEn, string metaTitleAr, string metaDescriptionEn, string metaDescriptionAr, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var eService = new EService(
             GuidGenerator.Create(),
             titleEn, titleAr, slug, descriptionEn, descriptionAr, image, headerImage, metaTitleEn, metaTitleAr, metaDescriptionEn, metaDescriptionAr, order, isActive
             );

            return await _eServiceRepository.InsertAsync(eService);
        }

        public async Task<EService> UpdateAsync(
            Guid id,
            string titleEn, string titleAr, string slug, string descriptionEn, string descriptionAr, string image, string headerImage, string metaTitleEn, string metaTitleAr, string metaDescriptionEn, string metaDescriptionAr, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var eService = await _eServiceRepository.GetAsync(id);

            eService.TitleEn = titleEn;
            eService.TitleAr = titleAr;
            eService.Slug = slug;
            eService.DescriptionEn = descriptionEn;
            eService.DescriptionAr = descriptionAr;
            eService.Image = image;
            eService.HeaderImage = headerImage;
            eService.MetaTitleEn = metaTitleEn;
            eService.MetaTitleAr = metaTitleAr;
            eService.MetaDescriptionEn = metaDescriptionEn;
            eService.MetaDescriptionAr = metaDescriptionAr;
            eService.Order = order;
            eService.IsActive = isActive;

            eService.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _eServiceRepository.UpdateAsync(eService);
        }

    }
}