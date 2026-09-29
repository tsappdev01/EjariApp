
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DIP.Amenities
{
    public partial interface IAmenitiesAppService 
    {
        Task<AmenityFrontEnd> GetWithDetailsFrontEndAsync(string slug);
        Task<List<AmenityFrontEnd>> GetListFrontEndAsync(GetAmenitiesInput input);
    }
}