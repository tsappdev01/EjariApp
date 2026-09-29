using DIP.CCPayment;
using System.Threading.Tasks;

namespace DIP.Interface
{
    public interface IDIPAppsettingService
    {
        Task<CCPaymentConfigValues> GetCardPaymentConfigurations();
    }
}
