using DIP.CCPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.Interface
{
    public interface ICCPaymentHelpers
    {
        Task<CCPaymentConstant> BuildCcPaymentFormDataAsync(PaymentInitiationRequest request);
        Task<CCPaymentResponse> ParseEncryptedResponseAsync(string encryptedResponse);
    }
}
