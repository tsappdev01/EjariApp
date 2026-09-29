using DIP.Amenities;
using DIP.AmenityParagraphs;
using System;
using System.Collections.Generic;

namespace DIP.Amenities
{
    public class AmenityWithDetails
    {
        public  List<AmenityParagraphWithDetails> AmenityParagraphs { get; set; }
        public Amenity Amenity { get; set; }    
        
    }
}