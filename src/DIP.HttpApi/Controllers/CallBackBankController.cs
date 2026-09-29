using Asp.Versioning;
using DeviceDetectorNET.Cache;
using DIP.EServices;
using DIP.Localization;
using DIP.Shared;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
[Route("uaepgsgateway")]

//[IntegrationService]

public class CallBackBankController : AbpController
{

    private readonly IEServicesAppService _eServicesAppService;
    private readonly IConfiguration _configuration;

   
    private readonly ILogger<CallBackBankController> _logger;
    private readonly IDistributedCache _cache;
    public CallBackBankController(IEServicesAppService eServicesAppService,
        IConfiguration configuration ,
        ILogger<CallBackBankController> logger,
IDistributedCache cache  )
    {
        _eServicesAppService = eServicesAppService;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;
    }

  
    [HttpPost]
    [Route("PGResponse")]
    [IgnoreAntiforgeryToken]
    

    public async Task<ActionResult> CallBackBank([FromForm] IFormCollection form)
    {

        if (form != null)
        {
            _logger.LogError("CallBackBank" + JsonConvert.SerializeObject(form));
        }
        else
        {
            _logger.LogError("CallBackBank is Null");

        }
        var mobileAppLanguage = HttpContext.Request.Headers["Accept-Language"];

        var formDict = form.Keys.ToDictionary(k => k, k => form[k].ToString());

        var dto = new BankCallbackFormDto
        {
            FormFields = formDict
        };

        // Generate unique reference key
        var reference = Guid.NewGuid().ToString();

        // Save to distributed cache with expiry
        await _cache.SetStringAsync(reference, JsonConvert.SerializeObject(dto),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        // Redirect to Blazor page with reference

        var t = ($"/{CultureInfo.CurrentCulture.Name}/uaepgsgateway/PGResponse?ref={reference}");
        return Redirect(t);

    }



    public class BankCallbackFormDto
    {
        public Dictionary<string, string> FormFields { get; set; }
    }
}
