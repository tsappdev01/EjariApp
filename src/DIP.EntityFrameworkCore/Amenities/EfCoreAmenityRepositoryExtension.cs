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
using DIP.AmenityParagraphs;

namespace DIP.Amenities
{
    public partial class EfCoreAmenityRepository 
    {
        public async Task<AmenityWithDetails> GetWithDetailsAsync(string slug, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();
            return (await GetDbSetAsync()).Where(a => a.IsActive && a.Slug == slug)
                .Select(amenity => new AmenityWithDetails
                {
                    Amenity = amenity,                   
                    AmenityParagraphs = dbContext.AmenityParagraphs.Where(b => b.IsActive && b.AmenityId == amenity.Id)
                                        .Select(amenityParagraph =>new AmenityParagraphWithDetails
                                        {
                                            AmenityParagraph = amenityParagraph,
                                            Medias = dbContext.Medias.Where(b => b.IsActive && b.AmenityParagraphId == amenityParagraph.Id).OrderBy(m=>m.Order).ToList()
                                        }).OrderBy(a=>a.AmenityParagraph.Order).ToList()            
                }).FirstOrDefault();
        }
    }
}