using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace DIP.SliderHomePages
{
    public class SliderHomePageManager : DomainService
    {
        private readonly ISliderHomePageRepository _sliderHomePageRepository;

        public SliderHomePageManager(ISliderHomePageRepository sliderHomePageRepository)
        {
            _sliderHomePageRepository = sliderHomePageRepository;
        }

        public virtual async Task<SliderHomePage> CreateAsync(
        string titleAr, string titleEn, string descriptionAr, string descriptionEn, bool isActive, int order, string? buttonTitleAr = null, string? buttonTitleEn = null, string? buttonUrlEn = null, string? buttonUrlAr = null, string? image = null, string? youtubeUrl = null)
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(descriptionAr, nameof(descriptionAr));
            Check.NotNullOrWhiteSpace(descriptionEn, nameof(descriptionEn));

            var sliderHomePage = new SliderHomePage(
             GuidGenerator.Create(),
             titleAr, titleEn, descriptionAr, descriptionEn, isActive, order, buttonTitleAr, buttonTitleEn, buttonUrlEn, buttonUrlAr, image, youtubeUrl
             );

            return await _sliderHomePageRepository.InsertAsync(sliderHomePage);
        }

        public virtual async Task<SliderHomePage> UpdateAsync(
            Guid id,
            string titleAr, string titleEn, string descriptionAr, string descriptionEn, bool isActive, int order, string? buttonTitleAr = null, string? buttonTitleEn = null, string? buttonUrlEn = null, string? buttonUrlAr = null, string? image = null, string? youtubeUrl = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(titleAr, nameof(titleAr));
            Check.NotNullOrWhiteSpace(titleEn, nameof(titleEn));
            Check.NotNullOrWhiteSpace(descriptionAr, nameof(descriptionAr));
            Check.NotNullOrWhiteSpace(descriptionEn, nameof(descriptionEn));

            var sliderHomePage = await _sliderHomePageRepository.GetAsync(id);

            sliderHomePage.TitleAr = titleAr;
            sliderHomePage.TitleEn = titleEn;
            sliderHomePage.DescriptionAr = descriptionAr;
            sliderHomePage.DescriptionEn = descriptionEn;
            sliderHomePage.IsActive = isActive;
            sliderHomePage.Order = order;
            sliderHomePage.ButtonTitleAr = buttonTitleAr;
            sliderHomePage.ButtonTitleEn = buttonTitleEn;
            sliderHomePage.ButtonUrlEn = buttonUrlEn;
            sliderHomePage.ButtonUrlAr = buttonUrlAr;
            sliderHomePage.Image = image;
            sliderHomePage.YoutubeUrl = youtubeUrl;

            sliderHomePage.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _sliderHomePageRepository.UpdateAsync(sliderHomePage);
        }

    }
}