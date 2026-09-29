
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using DIP.EServices;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace DIP.MediaGalleries
{
    public partial class MediaGalleriesAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<MediaGalleryFrontEnd> GetWithDetailsFrontEndAsync(string slug)
        {
            return ObjectMapper.Map<MediaGalleryWithDetails, MediaGalleryFrontEnd>(await _mediaGalleryRepository.GetWithDetailsAsync(slug));
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<MediaGalleryFrontEnd>> GetListFrontEndAsync(GetMediaGalleriesInput input)
        {
            var totalCount = await _mediaGalleryRepository.GetCountAsync(input.FilterText, input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive);
            var items = await _mediaGalleryRepository.GetListAsync(input.FilterText, input.TitleEn, input.Slug, input.TitleAr, input.MetaTitleEn, input.MetaTitleAr, input.MetaDescriptionEn, input.MetaDescriptionAr, input.SummaryEn, input.SummaryAr, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MediaGalleryFrontEnd>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MediaGallery>, List<MediaGalleryFrontEnd>>(items)
            };

        }
    }
}