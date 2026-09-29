using Volo.Abp.Modularity;

namespace DIP;

[DependsOn(
    typeof(DIPApplicationModule),
    typeof(DIPDomainTestModule)
    )]
public class DIPApplicationTestModule : AbpModule
{

}
