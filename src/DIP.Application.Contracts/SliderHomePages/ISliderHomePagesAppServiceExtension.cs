using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using DIP.Shared;
using DIP.Amenities;
using System.Collections.Generic;

namespace DIP.SliderHomePages
{
    public partial interface ISliderHomePagesAppService
    {
        Task<List<SliderHomePageFrontEnd>> GetListFrontEndAsync(GetSliderHomePagesInput input);
    }
}