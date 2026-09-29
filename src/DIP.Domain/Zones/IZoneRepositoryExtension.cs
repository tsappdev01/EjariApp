using DIP.Amenities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DIP.Zones
{
    public partial interface IZoneRepository 
    {
        Task<ZoneWithDetails> GetWithDetailsAsync(string slug, CancellationToken cancellationToken = default);
    }
}