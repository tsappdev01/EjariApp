using System.Text.Json.Serialization;

namespace DIP.Blazor.Pages.Site.NocForm
{
    /// <summary>Result returned by the HTTP-based NOC document upload (uploadNocDocument JS helper).</summary>
    public class NocUploadResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("attachmentName")]
        public string AttachmentName { get; set; }
    }
}
