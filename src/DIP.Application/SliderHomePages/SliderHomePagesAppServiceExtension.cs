using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;

namespace DIP.SliderHomePages
{
    public partial class SliderHomePagesAppService
    {

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<SliderHomePageFrontEnd>> GetListFrontEndAsync(GetSliderHomePagesInput input)
        {
            var items = await _sliderHomePageRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.DescriptionAr, input.DescriptionEn, input.ButtonTitleAr, input.ButtonTitleEn, input.ButtonUrlEn, input.ButtonUrlAr, input.Image, input.YoutubeUrl, input.IsActive, input.OrderMin, input.OrderMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<List<SliderHomePage>, List<SliderHomePageFrontEnd>>(items);
        }
    }
}