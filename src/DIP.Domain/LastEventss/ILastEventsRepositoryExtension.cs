using DIP.PageInfos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.LastEventss
{
    public partial interface ILastEventsRepository
    {
        Task<LastEvents> GetBySlugAsync(string slug);
    }
}