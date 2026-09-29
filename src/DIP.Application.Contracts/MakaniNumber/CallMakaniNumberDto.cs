using System;
using System.Collections.Generic;
using System.Text;

namespace DIP.MakaniNumber
{
    public class BUILDING
    {
        public string BldIf { get; set; }
        public string BldgNameEn { get; set; }
        public string BldgNameAr { get; set; }
        public string ParcelId { get; set; }
        public string BldgCat { get; set; }
        public string EmirateE { get; set; }
        public string EmirateA { get; set; }
        public string SHAPE { get; set; }
    }

    public class MAKANI
    {
        public string Makani { get; set; }
        public string ParcelId { get; set; }
        public string BldgNameAr { get; set; }
        public string BldgNameEn { get; set; }
        public string EntNameA { get; set; }
        public string EntNameE { get; set; }
        public string AddressE { get; set; }
        public string EntType { get; set; }
        public string AddressA { get; set; }
        public string EmirateE { get; set; }
        public string CommEn { get; set; }
        public string EmirateA { get; set; }
        public string CommAr { get; set; }
        public string SHAPE { get; set; }
    }

    public class PARCEL
    {
        public string ParcelId { get; set; }
        public string SHAPE { get; set; }
    }

    public class MakaniNumberPropertyDto
    {
        public List<PARCEL> PARCEL { get; set; }
        public List<BUILDING> BUILDINGS { get; set; }
        public List<MAKANI> MAKANI { get; set; }
        public bool IsExp { get; set; }
        public string ExpMessage { get; set; }
    }


}
