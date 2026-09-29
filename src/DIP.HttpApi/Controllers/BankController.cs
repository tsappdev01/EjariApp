using Asp.Versioning;
using DIP.EServices;
using DIP.Localization;
using DIP.Shared;
using DIP.Zones;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;
using static DIP.Permissions.DIPPermissions;


namespace DIP.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("bank")]
[Microsoft.AspNetCore.Components.Route("api/app/bank")]
//[IntegrationService]

public class BankController : AbpController, IEServicesAppService
{

    private readonly IEServicesAppService _eServicesAppService;
    private readonly IConfiguration _configuration;
    private readonly IZonesAppService _zonesAppService;
    [Microsoft.AspNetCore.Components.Inject]
    ProtectedLocalStorage LocalStorage { get; set; }

    public BankController(IEServicesAppService eServicesAppService, IConfiguration configuration  , IZonesAppService zonesAppService
)
    {
        _eServicesAppService = eServicesAppService;
        _configuration = configuration;
        _zonesAppService = zonesAppService;

    }

  
    [HttpPost]
    [Route("call-back-bank")]
    [Consumes("multipart/form-data")]

    public async Task<ActionResult> CallBackBank([FromForm] IFormCollection form)
    {
        //await _eServicesAppService.CallBackBank(form);
        var mobileAppLanguage = HttpContext.Request.Headers["Accept-Language"];

        await LocalStorage.SetAsync("FormCallBack", form);
  
        var t = ($"/{CultureInfo.CurrentCulture.Name}/uaepgsgateway/PGResponse");
        return Redirect(t);

    }


    [HttpGet]
    [Route("makani-test")]

    public async Task<string> Makani(string lat , string lon)
    {

      return await _zonesAppService.GetMakaniNumber(lat, lon); 
    }
    [RemoteService(false)]
    public Task<EServiceDto> CreateAsync(EServiceCreateDto input)
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task<EServiceDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(EServiceExcelDownloadDto input)
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task<PagedResultDto<EServiceDto>> GetListAsync(GetEServicesInput input)
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task<List<EServiceFrontEnd>> GetListFrontEndAsync(GetEServicesInput input)
    {
        throw new NotImplementedException();
    }
    [RemoteService(false)]
    public Task<EServiceDto> UpdateAsync(Guid id, EServiceUpdateDto input)
    {
        throw new NotImplementedException();
    }
}
