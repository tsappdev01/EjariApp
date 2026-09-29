using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DIP.Zones
{
    public partial interface IZonesAppService 
    {
        Task<ZoneFrontEnd> GetWithDetailsFrontEndAsync(string slug);
        Task<List<ZoneFrontEnd>> GetListFrontEndAsync(GetZonesInput input);

        Task<string> GetMakaniNumber(string lat , string lon);
    }
}