using DIP.EFormServiceSubCategories;
using DIP.MajorIndustries;
using DIP.ZoneParagraphs;
using System.Collections.Generic;

namespace DIP.EFormServices
{
    public class EFromServiceWithDetails
    {
        public  List<EFormServiceSubCategory> EFormServiceSubCategories { get; set; }
        public EFormService EFormService { get; set; }   
        
    }
}