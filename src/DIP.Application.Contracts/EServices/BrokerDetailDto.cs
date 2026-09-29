using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.EServices
{
    public class BrokerDetailDto
    {
        public int ID {  get; set; }
        public string BrokerName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
