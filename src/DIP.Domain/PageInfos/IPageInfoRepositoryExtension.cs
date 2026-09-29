using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.PageInfos
{
    public partial interface IPageInfoRepository
    {
        Task<PageInfoWithDetails> GetBySlugAsync(string slug);
    }
}