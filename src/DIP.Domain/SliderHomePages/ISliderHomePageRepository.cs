using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.SliderHomePages
{
    public interface ISliderHomePageRepository : IRepository<SliderHomePage, Guid>
    {
        Task<List<SliderHomePage>> GetListAsync(
            string? filterText = null,
            string? titleAr = null,
            string? titleEn = null,
            string? descriptionAr = null,
            string? descriptionEn = null,
            string? buttonTitleAr = null,
            string? buttonTitleEn = null,
            string? buttonUrlEn = null,
            string? buttonUrlAr = null,
            string? image = null,
            string? youtubeUrl = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? titleAr = null,
            string? titleEn = null,
            string? descriptionAr = null,
            string? descriptionEn = null,
            string? buttonTitleAr = null,
            string? buttonTitleEn = null,
            string? buttonUrlEn = null,
            string? buttonUrlAr = null,
            string? image = null,
            string? youtubeUrl = null,
            bool? isActive = null,
            int? orderMin = null,
            int? orderMax = null,
            CancellationToken cancellationToken = default);
    }
}