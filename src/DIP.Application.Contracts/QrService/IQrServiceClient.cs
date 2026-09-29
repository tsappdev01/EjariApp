using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.QrService
{
    public interface IQrServiceClient
    {
        Task<byte[]> ConvertStringToQrImageAsync(string referenceNo);
    }
}
