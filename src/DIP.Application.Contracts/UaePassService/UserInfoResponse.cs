using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DIP.UaePassService
{
    public class UserInfoResponse
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("fullnameAR")]
        public string FullnameAR { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; }

        [JsonPropertyName("lastnameEN")]
        public string LastnameEN { get; set; }

        [JsonPropertyName("fullnameEN")]
        public string FullnameEN { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("lastnameAR")]
        public string LastnameAR { get; set; }

        [JsonPropertyName("idn")]
        public string Idn { get; set; }

        [JsonPropertyName("nationalityEN")]
        public string NationalityEN { get; set; }

        [JsonPropertyName("firstnameEN")]
        public string FirstnameEN { get; set; }

        [JsonPropertyName("userType")]
        public string UserType { get; set; }

        [JsonPropertyName("nationalityAR")]
        public string NationalityAR { get; set; }

        [JsonPropertyName("firstnameAR")]
        public string FirstnameAR { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}