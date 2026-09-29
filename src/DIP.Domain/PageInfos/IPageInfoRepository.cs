using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.PageInfos
{
    public partial interface IPageInfoRepository : IRepository<PageInfo, Guid>
    {
        Task<List<PageInfo>> GetListAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string youTubeUrl = null,
            string pageInfoArticleTilteEn = null,
            string pageInfoArticleTilteAr = null,
            string pageInfoArticleSubtitleEn = null,
            string pageInfoArticleSubtitleAr = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filterText = null,
            string titleAr = null,
            string titleEn = null,
            string metaTitleAr = null,
            string metaTitleEn = null,
            string metaDescriptionEn = null,
            string metaDescriptionAr = null,
            string slug = null,
            string image = null,
            string headerImage = null,
            string youTubeUrl = null,
            string pageInfoArticleTilteEn = null,
            string pageInfoArticleTilteAr = null,
            string pageInfoArticleSubtitleEn = null,
            string pageInfoArticleSubtitleAr = null,
            string descriptionAr = null,
            string descriptionEn = null,
            string summaryEn = null,
            string summaryAr = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}