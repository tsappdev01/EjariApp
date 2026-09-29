using System.Threading.Tasks;

namespace DIP.Data;

public interface IDIPDbSchemaMigrator
{
    Task MigrateAsync();
}
