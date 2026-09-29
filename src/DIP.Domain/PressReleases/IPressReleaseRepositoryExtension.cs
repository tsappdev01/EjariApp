using DIP.LastEventss;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.PressReleases
{
    public partial interface IPressReleaseRepository
    {
        Task<PressRelease> GetBySlugAsync(string slug);

    }
}
