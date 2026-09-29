using Asp.Versioning;
using DIP.EServices;
using DIP.Localization;
using DIP.MakaniNumber;
using DIP.Shared;
using DIP.Zones;
using Microsoft.AspNetCore.Components.Routing;
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
[ControllerName("MakaniNumber")]
[Route("api/app/makani-number")]
//[IntegrationService]

public class MakaniNumberController : AbpController, IMakaniNumberAppService
{

    private readonly IMakaniNumberAppService _makaniNumberAppService;

   

    public MakaniNumberController(IMakaniNumberAppService makaniNumberAppService)
    {
        _makaniNumberAppService = makaniNumberAppService;
    }


    [HttpPost]
    [Route("get-makani-number")]

    public  Task<MakaniNumberPropertyDto> GetMakaniNumberAsync(string plotNumber)
    {
        return _makaniNumberAppService.GetMakaniNumberAsync(plotNumber);
    }

   
}
