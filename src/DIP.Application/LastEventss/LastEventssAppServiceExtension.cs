using System.Collections.Generic;
using System.Threading.Tasks;
using DIP.PageInfos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace DIP.LastEventss
{


    public partial class LastEventssAppService
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PagedResultDto<LastEventsFrontEnd>> GetViewListAsync(GetLastEventssInput input)
        {
            var totalCount = await _lastEventsRepository.GetCountAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.IsActive, input.LocationAr, input.LocationEn);
            var items = await _lastEventsRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.IsActive, input.LocationAr, input.LocationEn, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<LastEventsFrontEnd>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<LastEvents>, List<LastEventsFrontEnd>>(items)
            };
        }

        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<LastEventsFrontEnd>> GetListFrontEndAsync(GetLastEventssInput input)
        {
            var items = await _lastEventsRepository.GetListAsync(input.FilterText, input.TitleAr, input.TitleEn, input.MetaTitleAr, input.MetaTitleEn, input.MetaDescriptionEn, input.MetaDescriptionAr, input.IsFeatured, input.Slug, input.Image, input.HeaderImage, input.DescriptionAr, input.DescriptionEn, input.SummaryEn, input.SummaryAr, input.OrderMin, input.OrderMax, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.IsActive, input.LocationAr, input.LocationEn, input.Sorting, input.MaxResultCount, input.SkipCount);

            return ObjectMapper.Map<List<LastEvents>, List<LastEventsFrontEnd>>(items);

        }
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<LastEventsFrontEnd> GetBySlugAsync(string slug)
        {
            var items = await _lastEventsRepository.GetBySlugAsync(slug);

            return ObjectMapper.Map<LastEvents, LastEventsFrontEnd>(items);

        }
    }
}