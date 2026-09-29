using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;

namespace DIP.LastEventss
{
    public partial interface ILastEventssAppService
    {
        Task<LastEventsFrontEnd> GetBySlugAsync(string slug);
        Task<PagedResultDto<LastEventsFrontEnd>> GetViewListAsync(GetLastEventssInput input);
        Task<List<LastEventsFrontEnd>> GetListFrontEndAsync(GetLastEventssInput input);
    }
}