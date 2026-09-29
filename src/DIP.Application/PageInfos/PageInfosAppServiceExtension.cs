using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;

namespace DIP.PageInfos
{

    public partial class PageInfosAppService 
    {
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<PageInfoFrontEnd> GetBySlugAsync(string slug)
        {
            return ObjectMapper.Map<PageInfoWithDetails, PageInfoFrontEnd>(await _pageInfoRepository.GetBySlugAsync(slug));
        }
        
        [RemoteService(false)]
        [AllowAnonymous]
        public virtual async Task<List<PageInfoHeaderFrontEnd>> GetPageInfoLookupBySlugsAsync(List<string> slugs)
        {
            var query = (await _pageInfoRepository.GetQueryableAsync())
                .WhereIf(!slugs.IsNullOrEmpty(),
                    x => x.Slug != null &&
                         slugs.Contains(x.Slug))
                .Where(x=>x.IsActive)
                .OrderBy(x=>x.Order);

            var lookupData = await query.PageBy(0, 12).ToDynamicListAsync<PageInfo>();
            return ObjectMapper.Map<List<PageInfo>, List<PageInfoHeaderFrontEnd>>(lookupData);
        }

    }
}