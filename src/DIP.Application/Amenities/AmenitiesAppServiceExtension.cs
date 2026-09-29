
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using DIP.Zones;
using System.Collections.Generic;

namespace DIP.Amenities
{
    public partial class AmenitiesAppService 
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<AmenityFrontEnd> GetWithDetailsFrontEndAsync(string slug)
        {
            return ObjectMapper.Map<AmenityWithDetails, AmenityFrontEnd>(await _amenityRepository.GetWithDetailsAsync(slug));
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<AmenityFrontEnd>> GetListFrontEndAsync(GetAmenitiesInput input)
        {
            var items = await _amenityRepository.GetListAsync(input.FilterText, input.TitleEn, input.TitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaTitleAr, input.MetaDescriptionAr, input.Slug, input.HeaderImage, input.Image, input.OrderMin, input.OrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<List<Amenity>, List<AmenityFrontEnd>>(items);
        }
    }
}