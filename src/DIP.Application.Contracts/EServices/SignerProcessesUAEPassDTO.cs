using DIP.UaePassService;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DIP.EServices
{

    public class DeclarationCacheDTO
    {
        public string EncryptedRefNo { get; set; }
        public SignProcessResponse signProcessResponse { get; set; }
        public UserInfoResponse UserInfo { get; set; }
    }
    public class UAEpassLoginCacheDTO
    {
        public string EncryptedRefNo { get; set; }
    }
    public class SignProcessResponse
    {
        [JsonPropertyName("process_type")]
        public string ProcessType { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("self")]
        public string Self { get; set; }

        [JsonPropertyName("tasks")]
        public SignProcessTasks Tasks { get; set; }

        [JsonPropertyName("documents")]
        public List<SignProcessDocument> Documents { get; set; }
    }

    public class SignProcessTasks
    {
        [JsonPropertyName("pending")]
        public List<SignProcessPendingTask> Pending { get; set; }
    }

    public class SignProcessPendingTask
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class SignProcessDocument
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
