using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;

namespace DIP.ContactForms
{
    public interface IContactFormsAppService : IApplicationService
    {
        Task<PagedResultDto<ContactFormDto>> GetListAsync(GetContactFormsInput input);

        Task<ContactFormDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ContactFormDto> CreateAsync(ContactFormCreateDto input);

        Task<ContactFormDto> UpdateAsync(Guid id, ContactFormUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ContactFormExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}