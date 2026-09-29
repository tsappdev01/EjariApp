using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.FeedBacks
{
    public interface IFeedBacksAppService : IApplicationService
    {
        Task<PagedResultDto<FeedBackDto>> GetListAsync(GetFeedBacksInput input);

        Task<FeedBackDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<FeedBackDto> CreateAsync(FeedBackCreateDto input);

        Task<FeedBackDto> UpdateAsync(Guid id, FeedBackUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(FeedBackExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}