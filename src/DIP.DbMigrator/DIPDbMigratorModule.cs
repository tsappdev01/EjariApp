using DIP.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DIP.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DIPEntityFrameworkCoreModule),
    typeof(DIPApplicationContractsModule)
)]
public class DIPDbMigratorModule : AbpModule
{

}
