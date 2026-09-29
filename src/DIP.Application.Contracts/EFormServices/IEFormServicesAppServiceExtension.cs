using System.Threading.Tasks;
using System.Collections.Generic;

namespace DIP.EFormServices
{
    public partial interface IEFormServicesAppService
    {
        Task<List<EFormServiceFrontEnd>> GetListFrontEndAsync(GetEFormServicesInput input);
    }
}