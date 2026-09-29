using DIP.Amenities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.TimeLineCategories
{
    public partial interface ITimeLineCategoryRepository
    {
        Task<List<TimeLineCategoryWithDetails>> GetListWithDetailsAsync();
    }
}