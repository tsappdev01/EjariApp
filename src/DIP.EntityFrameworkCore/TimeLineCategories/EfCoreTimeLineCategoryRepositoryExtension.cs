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
using DIP.Amenities;
using DIP.AmenityParagraphs;

namespace DIP.TimeLineCategories
{
    public partial class EfCoreTimeLineCategoryRepository
    {
        public async Task<List<TimeLineCategoryWithDetails>> GetListWithDetailsAsync()
        {           
                var dbContext = await GetDbContextAsync();
                return (await GetDbSetAsync()).Where(t=>t.IsActive)
                    .Select(timelineCategory => new TimeLineCategoryWithDetails
                    {
                        TimeLineCategory = timelineCategory,
                        TimeLines = dbContext.TimeLines.Where(b => b.IsActive && b.TimeLineCategoryId == timelineCategory.Id).OrderBy(a => a.Order).ToList()
                    }).OrderBy(t=>t.TimeLineCategory.Order).ToList();
        }
    }
}