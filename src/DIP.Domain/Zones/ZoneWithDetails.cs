using DIP.MajorIndustries;
using DIP.ZoneParagraphs;
using System.Collections.Generic;

namespace DIP.Zones
{
    public class ZoneWithDetails
    {
        public  List<ZoneParagraphWithDetails> ZoneParagraphs { get; set; }
        public  List<MajorIndustry> MajorIndustries { get; set; }
        public Zone Zone { get; set; }    
        
    }
}