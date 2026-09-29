using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DIP.Blazor;

[Dependency(ReplaceServices = true)]
public class DIPBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "DIP";
}
