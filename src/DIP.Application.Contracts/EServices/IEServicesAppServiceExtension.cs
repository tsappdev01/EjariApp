using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DIP.EServices
{
    public partial interface IEServicesAppService
    {
        Task<List<EServiceFrontEnd>> GetListFrontEndAsync(GetEServicesInput input);
        Task<ActionResult> CallBackBank([FromForm] IFormCollection form);

    }
}