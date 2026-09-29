using DIP.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace DIP;

[DependsOn(
    typeof(DIPEntityFrameworkCoreTestModule)
    )]
public class DIPDomainTestModule : AbpModule
{

}
