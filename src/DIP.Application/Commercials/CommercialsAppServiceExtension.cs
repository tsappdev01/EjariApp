using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using DIP.PressReleases;
using DIP.Permissions;
using System;

namespace DIP.Commercials
{

    public partial class CommercialsAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<CommercialFrontEnd>> GetViewListAsync(GetCommercialsInputDetails input)
        {
            var totalCount = await _commercialRepository.GetCountDetailsAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.IsActive, input.OrderMin, input.OrderMax, input.SubCategoryIds, input.StartWithLetter);
            var items = await _commercialRepository.GetListWithDetailsAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.IsActive, input.OrderMin, input.OrderMax, input.SubCategoryIds, input.StartWithLetter, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CommercialFrontEnd>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CommercialWithDetails>, List<CommercialFrontEnd>>(items)
            };
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<CommercialFrontEnd>> GetViewListByTextAsync(GetCommercialsInputDetails input)
        {
            var totalCount = await _commercialRepository.GetCountDetailsByTextAsync(input.FilterText);
            var items = await _commercialRepository.GetListWithDetailsByTextAsync(input.FilterText, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CommercialFrontEnd>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CommercialWithDetails>, List<CommercialFrontEnd>>(items)
            };
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<CommercialWithNavigationPropertiesDto>> GetListWithPlotNoAsync(GetCommercialsInput input)
        {
          //  var totalCount = await _commercialRepository.GetCountAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.OrderMin, input.OrderMax, input.SubCategoryId);
            var items = await _commercialRepository.GetListWithPlotNoWithNavigationPropertiesAsync(input.FilterText, input.TitleEn, input.TitleAr, input.PlotNo, input.ActivityEn, input.ActivityAr, input.Phone, input.Fax, input.MakaniNo, input.IsActive, input.OrderMin, input.OrderMax, input.SubCategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);
            return ObjectMapper.Map<List<CommercialWithNavigationProperties>, List<CommercialWithNavigationPropertiesDto>>(items);
        }

        [AllowAnonymous]
        public virtual async Task<CommercialDto> UpdateMakaniAsync(Guid id, string input)
        {
            var commercial = await _commercialManager.UpdateMakaniAsync(
            id, input
            );

            return ObjectMapper.Map<Commercial, CommercialDto>(commercial);
        }
    }
}