using DIP.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DIP.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DIPController : AbpControllerBase
{
    protected DIPController()
    {
        LocalizationResource = typeof(DIPResource);
    }
}
