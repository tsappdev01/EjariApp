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
using DIP.LastEventss;

namespace DIP.PressReleases
{
    public partial class EfCorePressReleaseRepository 
    {
        public async Task<PressRelease> GetBySlugAsync(string slug)
        {
            return (await GetDbSetAsync()).Where(a => a.Slug == slug)
                        .FirstOrDefault();
        }

    }
}