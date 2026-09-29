using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DIP.MakaniNumber
{
    public interface IMakaniNumberAppService
    {
        Task<MakaniNumberPropertyDto> GetMakaniNumberAsync(string plotNumber);

    }
}
