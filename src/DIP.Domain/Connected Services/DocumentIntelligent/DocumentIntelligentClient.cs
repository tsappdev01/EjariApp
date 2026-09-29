using DIP.DIModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DIP.Connected_Services.DocumentIntelligent
{
    #region Interface
    public interface IDocumentIntelligentClient
    {
        Task<object> UploadFile(byte[] fileBytes, string DocumentModel);
        Task<List<DocumentMatchDTO>> GetDocumentMatchObject();
    }
    #endregion


    public class DocumentIntelligentClient : IDocumentIntelligentClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public DocumentIntelligentClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config.GetSection("DocumentIntelligent");
        }

        public Task<List<DocumentMatchDTO>> GetDocumentMatchObject()
        {
            var _documentMatch = _config["DocumentMatch"] ?? throw new ArgumentNullException("DocumentMatch not be null value.");
            return Task.FromResult(JsonSerializer.Deserialize<List<DocumentMatchDTO>>(_documentMatch) ?? []);
        }

        public async Task<object> UploadFile(byte[] fileBytes, string DocumentModel)
        {
            try
            {

                var diUrl = _config["BaseUrl"] + _config["EndPoints:UploadFile"]?.Replace("{{di-model-id}}", DocumentModel);
                var apiKey = _config["ApiKey"];
                using var request = new HttpRequestMessage(HttpMethod.Post, diUrl);
                request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
                request.Content = new ByteArrayContent(fileBytes);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var requestId = response.Headers.GetValues("apim-request-id");
                if (requestId.Count() > 0)
                {
                    return await GetDIResult(operationId:
                        requestId.FirstOrDefault() ?? throw new ArgumentException("Document Intelligent: Not return orderid."),
                        DocumentModel: DocumentModel);
                }
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                return 500;
            }
        }

        private async Task<object> GetDIResult(string operationId, string DocumentModel)
        {
            var diUrl = _config["BaseUrl"] + _config["EndPoints:Result"]?
                .Replace("{{di-model-id}}", DocumentModel)
                .Replace("{{di-operation-id}}", operationId);
            var apiKey = _config["ApiKey"] ?? throw new NullReferenceException("DI:ApiKey:ApiKey not be null.");

            using var request = new HttpRequestMessage(HttpMethod.Get, diUrl);
            request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var DocResponse = JsonSerializer.Deserialize<DocumentModelDTO>(json);
            if (DocResponse?.status == "running")
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                return await GetDIResult(operationId, DocumentModel);
            }
            return DocResponse ?? new DocumentModelDTO();
        }

    }
}


