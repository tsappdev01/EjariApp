using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.InquiryForms
{
    public interface IInquiryFormsAppService : IApplicationService
    {
        Task<PagedResultDto<InquiryFormDto>> GetListAsync(GetInquiryFormsInput input);

        Task<InquiryFormDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<InquiryFormDto> CreateAsync(InquiryFormCreateDto input);

        Task<InquiryFormDto> UpdateAsync(Guid id, InquiryFormUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(InquiryFormExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}