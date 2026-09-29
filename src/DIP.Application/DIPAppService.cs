using DIP.Localization;
using Volo.Abp.Application.Services;

namespace DIP;

/* Inherit your application services from this class.
 */
public abstract class DIPAppService : ApplicationService
{
    protected DIPAppService()
    {
        LocalizationResource = typeof(DIPResource);
    }
}
