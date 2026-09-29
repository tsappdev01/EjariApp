using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using System.Collections.Generic;

namespace DIP.DipFacts
{
    public partial interface IDipFactsAppService
    {
        Task<List<DipFactFrontEnd>> GetListFrontEndAsync(GetDipFactsInput input);
    }
}