
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using DIP.EServices;
using System.Collections.Generic;
using DIP.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace DIP.Zones
{
    public partial class ZonesAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<ZoneFrontEnd> GetWithDetailsFrontEndAsync(string slug)
        {
            return ObjectMapper.Map<ZoneWithDetails, ZoneFrontEnd>(await _zoneRepository.GetWithDetailsAsync(slug));
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<ZoneFrontEnd>> GetListFrontEndAsync(GetZonesInput input)
        {
            var items = await _zoneRepository.GetListAsync(filterText: input.FilterText,
                                                         titleEn : input.TitleEn,
                                                          titleAR: input.TitleAR,
                                                         metaTitleEn:  input.MetaTitleEn,
                                                        metaDescriptionEn:   input.MetaDescriptionEn,
                                                       metaTitleAr:    input.MetaTitleAr,
                                                        metaDescriptionAr:   input.MetaDescriptionAr,
                                                       slug:    input.Slug,
                                                        summaryEn:   input.SummaryEn,
                                                          summaryAr: input.SummaryAr,
                                                      image:     input.Image,
                                                      headerImage:     input.HeaderImage,
                                                     orderMin:      input.OrderMin,
                                                     orderMax:      input.OrderMax,
                                                         isFeature:  input.IsFeature,
                                                         isActive:  input.IsActive,
                                                       sorting:    input.Sorting,
                                                       maxResultCount :   input.MaxResultCount,
                                                        skipCount:   input.SkipCount);

            return ObjectMapper.Map<List<Zone>, List<ZoneFrontEnd>>(items);

        }

        public async Task<string> GetMakaniNumber( string lat , string lon)
        {
         return await _zoneManager.getMakaniNumber(lat , lon);
        }
    }
}