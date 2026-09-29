using DIP.MediaGalleries;
using DIP.AmenityParagraphs;
using DIP.ZoneParagraphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using DIP.EntityFrameworkCore;

namespace DIP.Medias
{
    public class EfCoreMediaRepository : EfCoreRepository<DIPDbContext, Media, Guid>, IMediaRepository
    {
        public EfCoreMediaRepository(IDbContextProvider<DIPDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public async Task<MediaWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(media => new MediaWithNavigationProperties
                {
                    Media = media,
                    ZoneParagraph = dbContext.Set<ZoneParagraph>().FirstOrDefault(c => c.Id == media.ZoneParagraphId),
                    AmenityParagraph = dbContext.Set<AmenityParagraph>().FirstOrDefault(c => c.Id == media.AmenityParagraphId),
                    MediaGallery = dbContext.Set<MediaGallery>().FirstOrDefault(c => c.Id == media.MediaGalleryId)
                }).FirstOrDefault();
        }

        public async Task<List<MediaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneParagraphId = null,
            Guid? amenityParagraphId = null,
            Guid? mediaGalleryId = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, file, orderMin, orderMax, isActive, zoneParagraphId, amenityParagraphId, mediaGalleryId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MediaConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<MediaWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from media in (await GetDbSetAsync())
                   join zoneParagraph in (await GetDbContextAsync()).Set<ZoneParagraph>() on media.ZoneParagraphId equals zoneParagraph.Id into zoneParagraphs
                   from zoneParagraph in zoneParagraphs.DefaultIfEmpty()
                   join amenityParagraph in (await GetDbContextAsync()).Set<AmenityParagraph>() on media.AmenityParagraphId equals amenityParagraph.Id into amenityParagraphs
                   from amenityParagraph in amenityParagraphs.DefaultIfEmpty()
                   join mediaGallery in (await GetDbContextAsync()).Set<MediaGallery>() on media.MediaGalleryId equals mediaGallery.Id into mediaGalleries
                   from mediaGallery in mediaGalleries.DefaultIfEmpty()
                   select new MediaWithNavigationProperties
                   {
                       Media = media,
                       ZoneParagraph = zoneParagraph,
                       AmenityParagraph = amenityParagraph,
                       MediaGallery = mediaGallery
                   };
        }

        protected virtual IQueryable<MediaWithNavigationProperties> ApplyFilter(
            IQueryable<MediaWithNavigationProperties> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneParagraphId = null,
            Guid? amenityParagraphId = null,
            Guid? mediaGalleryId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Media.TitleEn.Contains(filterText) || e.Media.TitleAr.Contains(filterText) || e.Media.File.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.Media.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.Media.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.Media.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Media.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Media.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.Media.IsActive == isActive)
                    .WhereIf(zoneParagraphId != null && zoneParagraphId != Guid.Empty, e => e.ZoneParagraph != null && e.ZoneParagraph.Id == zoneParagraphId)
                    .WhereIf(amenityParagraphId != null && amenityParagraphId != Guid.Empty, e => e.AmenityParagraph != null && e.AmenityParagraph.Id == amenityParagraphId)
                    .WhereIf(mediaGalleryId != null && mediaGalleryId != Guid.Empty, e => e.MediaGallery != null && e.MediaGallery.Id == mediaGalleryId);
        }

        public async Task<List<Media>> GetListAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, titleEn, titleAr, file, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MediaConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(
            string filterText = null,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? zoneParagraphId = null,
            Guid? amenityParagraphId = null,
            Guid? mediaGalleryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, titleEn, titleAr, file, orderMin, orderMax, isActive, zoneParagraphId, amenityParagraphId, mediaGalleryId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Media> ApplyFilter(
            IQueryable<Media> query,
            string filterText,
            string titleEn = null,
            string titleAr = null,
            string file = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.TitleEn.Contains(filterText) || e.TitleAr.Contains(filterText) || e.File.Contains(filterText))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleEn), e => e.TitleEn.Contains(titleEn))
                    .WhereIf(!string.IsNullOrWhiteSpace(titleAr), e => e.TitleAr.Contains(titleAr))
                    .WhereIf(!string.IsNullOrWhiteSpace(file), e => e.File.Contains(file))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}