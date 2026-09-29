using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DIP.Data;

/* This is used if database provider does't define
 * IDIPDbSchemaMigrator implementation.
 */
public class NullDIPDbSchemaMigrator : IDIPDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
