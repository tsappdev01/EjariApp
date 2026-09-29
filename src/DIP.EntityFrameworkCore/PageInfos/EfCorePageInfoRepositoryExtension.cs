using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using DIP.EntityFrameworkCore;

namespace DIP.PageInfos
{
    public partial class EfCorePageInfoRepository
    {
        public async Task<PageInfoWithDetails> GetBySlugAsync(string slug)
        {
            var dbContext = await GetDbContextAsync();
            return (await GetDbSetAsync()).Where(a => a.Slug == slug)
                        .Select(a => new PageInfoWithDetails()
                        {
                            PageInfo = a,
                            PageInfoSections = dbContext.PageInfoSections.Where(p => p.IsActive && p.PageInfoId == a.Id).OrderBy(p=>p.Order).ToList()
                        }).FirstOrDefault();
        }
    }
}