using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DIP.MakaniNumber
{
  

    public class InputJson
    {
        public string featureclass_id { get; set; }
        public string dgis_id { get; set; }
        public string userid { get; set; }
        public string sessionid { get; set; }
    }

    public class CallMakaniNumberDto
    {
        [JsonPropertyName("InputJson")]
        public InputJson InputJson { get; set; }


        [JsonPropertyName("Token")]

        public string Token { get; set; }

        [JsonPropertyName("Remarks")]
        public string Remarks { get; set; }

    }


}
