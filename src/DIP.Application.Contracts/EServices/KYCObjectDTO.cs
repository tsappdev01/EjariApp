using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIP.EServices
{
    public class KYCObjectDTO
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldType { get; set; }
        public decimal ConfidenceScore { get; set; }
    }
}
