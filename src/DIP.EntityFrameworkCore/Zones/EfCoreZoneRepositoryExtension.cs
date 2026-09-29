using DIP.Amenities;
using DIP.MajorIndustries;
using DIP.ZoneParagraphs;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace DIP.Zones
{
    public partial class EfCoreZoneRepository 
    {
        public async Task<ZoneWithDetails> GetWithDetailsAsync(string slug, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return (await GetDbSetAsync()).Where(a => a.IsActive && a.Slug == slug)
                .Select(zone => new ZoneWithDetails
                {
                    Zone = zone,
                    ZoneParagraphs = dbContext.ZoneParagraphs.Where(b => b.IsActive && b.ZoneId == zone.Id)
                                        .Select(zoneParagraph => new ZoneParagraphWithDetails
                                        {
                                            ZoneParagraph = zoneParagraph,
                                            Medias = dbContext.Medias.Where(b => b.IsActive && b.ZoneParagraphId == zoneParagraph.Id).OrderBy(m => m.Order).ToList()
                                        }).OrderBy(a => a.ZoneParagraph.Order).ToList(),
                    MajorIndustries = dbContext.MajorIndustries.Where(b => b.IsActive && b.ZoneId == zone.Id)
                                        .OrderBy(a => a.Order).ToList()
                }).FirstOrDefault();
        }
    }
}