using DIP.CCPayment;
using DIP.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DIP
{
    public class DIPAppsettingService : IDIPAppsettingService
    {
        private readonly IConfiguration _config;
        public DIPAppsettingService(IConfiguration config)
        {
            _config = config.GetSection("CardPayment");
        }

        public Task<CCPaymentConfigValues> GetCardPaymentConfigurations()
        {
            string MerchantId = _config["MerchantId"] ?? throw new NullReferenceException("CardPayment:MerchantId not be null.");
            string AccessCode = _config["AccessCode"] ?? throw new NullReferenceException("CardPayment:AccessCode not be null.");
            string WorkingKey = _config["WorkingKey"] ?? throw new NullReferenceException("CardPayment:WorkingKey not be null.");
            string PaymentUrl = _config["PaymentUrl"] ?? throw new NullReferenceException("CardPayment:PaymentUrl not be null.");
            string RedirectUrl = _config["RedirectUrl"] ?? throw new NullReferenceException("CardPayment:RedirectUrl not be null.");
            string Currency = _config["Currency"] ?? throw new NullReferenceException("CardPayment:Currency not be null.");
            string Language = _config["Language"] ?? throw new NullReferenceException("CardPayment:Currency not be null.");

            return Task.FromResult(new CCPaymentConfigValues
            {
                MerchantId = MerchantId,
                AccessCode = AccessCode,
                WorkingKey = WorkingKey,
                PaymentUrl = PaymentUrl,
                RedirectUrl = RedirectUrl,
                Currency = Currency,
                Language = Language
            });
        }
    }
}
