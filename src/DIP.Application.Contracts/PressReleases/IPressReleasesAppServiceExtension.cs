using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;
using DIP.LastEventss;

namespace DIP.PressReleases
{
    public partial interface IPressReleasesAppService
    {
        Task<PressReleaseFrontEnd> GetBySlugAsync(string slug);

        Task<PagedResultDto<PressReleaseFrontEnd>> GetViewListAsync(GetPressReleasesInput input);
        Task<List<PressReleaseFrontEnd>> GetListFrontEndAsync(GetPressReleasesInput input);
    }
}