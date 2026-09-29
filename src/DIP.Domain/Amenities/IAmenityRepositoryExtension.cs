using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DIP.Amenities
{
    public partial interface IAmenityRepository 
    {
        Task<AmenityWithDetails> GetWithDetailsAsync(string slug, CancellationToken cancellationToken = default);
    }
}