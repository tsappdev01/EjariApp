using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace DIP.LastEventss
{
    public partial class EfCoreLastEventsRepository 
    {
        public async Task<LastEvents> GetBySlugAsync(string slug)
        {
            return (await GetDbSetAsync()).Where(a => a.Slug == slug)
                        .FirstOrDefault();
        }
    }
}