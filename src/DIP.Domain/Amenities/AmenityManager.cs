using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.Amenities
{
    public class AmenityManager : DomainService
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenityManager(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<Amenity> CreateAsync(
        string titleEn, string titleAr, string metaTitleEn, string metaDescriptionEn, string metaTitleAr, string metaDescriptionAr, string slug, string headerImage, string image, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var amenity = new Amenity(
             GuidGenerator.Create(),
             titleEn, titleAr, metaTitleEn, metaDescriptionEn, metaTitleAr, metaDescriptionAr, slug, headerImage, image, order, isActive
             );

            return await _amenityRepository.InsertAsync(amenity);
        }

        public async Task<Amenity> UpdateAsync(
            Guid id,
            string titleEn, string titleAr, string metaTitleEn, string metaDescriptionEn, string metaTitleAr, string metaDescriptionAr, string slug, string headerImage, string image, int order, bool isActive, [CanBeNull] string concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(slug, nameof(slug));

            var amenity = await _amenityRepository.GetAsync(id);

            amenity.TitleEn = titleEn;
            amenity.TitleAr = titleAr;
            amenity.MetaTitleEn = metaTitleEn;
            amenity.MetaDescriptionEn = metaDescriptionEn;
            amenity.MetaTitleAr = metaTitleAr;
            amenity.MetaDescriptionAr = metaDescriptionAr;
            amenity.Slug = slug;
            amenity.HeaderImage = headerImage;
            amenity.Image = image;
            amenity.Order = order;
            amenity.IsActive = isActive;

            amenity.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _amenityRepository.UpdateAsync(amenity);
        }

    }
}