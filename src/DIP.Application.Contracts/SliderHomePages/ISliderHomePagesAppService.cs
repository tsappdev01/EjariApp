using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.SliderHomePages
{
    public partial interface ISliderHomePagesAppService : IApplicationService
    {
        Task<PagedResultDto<SliderHomePageDto>> GetListAsync(GetSliderHomePagesInput input);

        Task<SliderHomePageDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SliderHomePageDto> CreateAsync(SliderHomePageCreateDto input);

        Task<SliderHomePageDto> UpdateAsync(Guid id, SliderHomePageUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SliderHomePageExcelDownloadDto input);

        Task<DIP.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}