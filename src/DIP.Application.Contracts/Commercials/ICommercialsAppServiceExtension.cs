using DIP.Shared;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using DIP.PressReleases;
using System.Collections.Generic;

namespace DIP.Commercials
{
    public partial interface ICommercialsAppService 
    {
        Task<PagedResultDto<CommercialFrontEnd>> GetViewListAsync(GetCommercialsInputDetails input);
        Task<PagedResultDto<CommercialFrontEnd>> GetViewListByTextAsync(GetCommercialsInputDetails input);
        Task<List<CommercialWithNavigationPropertiesDto>> GetListWithPlotNoAsync(GetCommercialsInput input);

        Task<CommercialDto> UpdateMakaniAsync(Guid id, string input);
    }
}