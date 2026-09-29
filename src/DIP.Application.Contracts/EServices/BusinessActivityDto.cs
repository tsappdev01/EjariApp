using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.EServices
{
    public class BusinessActivityDto
    {
        public int MasterActivityId { get; set; }
        public string MasterActivity { get; set; } = string.Empty;
    }

    public class MaterialClassificationDto
    {
        public int id { get; set; }
        public string MaterialClassification { get; set; } = string.Empty;

    }
}
