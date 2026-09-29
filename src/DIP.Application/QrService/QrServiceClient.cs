using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DIP.QrService
{
    public class QrServiceClient : IQrServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public QrServiceClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config.GetSection("QrCodeGenerate");
        }

        public async Task<byte[]> ConvertStringToQrImageAsync(string referenceNo)
        {
            var url = $"{_config["BaseUrl"]}?size={_config["Size"]}&data={referenceNo}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
