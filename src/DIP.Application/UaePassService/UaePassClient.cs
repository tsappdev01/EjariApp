using DIP.EServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StgDipService;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DIP.UaePassService
{
    public class UaePassClient : IUaePassClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IConfiguration _configApp;
        private readonly ILogger<UaePassClient> _logger;

        public UaePassClient(HttpClient httpClient, IConfiguration config, ILogger<UaePassClient> logger)
        {
            _httpClient = httpClient;
            _config = config.GetSection("UaePass");
            _configApp = config.GetSection("App");
            _logger = logger;

            // Adds a valid User-Agent so the NetScaler WAF doesn't silently block the request resulting in a timeout
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/122.0.0.0");
            }
        }

        public async Task<string> GetNOcDeclarationEnpoint()
        {
            string NocDeclarationEndpoint = _configApp["NocDeclarationEndpoint"] ?? throw new NullReferenceException("App:NocDeclarationEndpoint not be null.");
            return NocDeclarationEndpoint;
        }
        public async Task<string> GetAccessTokenAsync()
        {
            var tokenUrl = _config["BaseUrl"] + _config["Endpoints:Token"];
            var clientId = _config["ClientId"];
            var clientSecret = _config["ClientSecret"];
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            tokenUrl = tokenUrl + "?grant_type=" + _config["GrantType"] + "&scope=" + _config["Scope"];
            using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(json);
            string _accessToken = tokenResponse.GetProperty("access_token").GetString();

            return _accessToken;
        }

        public async Task<UaePassTokenResponse> GetAccessTokenByAuthorizationCodeAsync(string code, string rediractionToken)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(code))
                    throw new ArgumentException("code must be provided", nameof(code));

                string authRedirectUri = _config["authRedirecturi"] ?? throw new NullReferenceException("UAEPass:authRedirecturi not be null.");
                string AccessTokenEndpoint = _config["Endpoints:AccessToken"] ?? throw new NullReferenceException("Endpoints:AccessToken not be null.");

                var tokenUrl = _config["BaseUrl"] + AccessTokenEndpoint.Replace("{{{authRedirecturi}}}", authRedirectUri)
                                       .Replace("{{{Token}}}", Uri.EscapeDataString(rediractionToken))
                                       .Replace("{{{Code}}}", Uri.EscapeDataString(code));

                var clientId = _config["ClientId"] ?? throw new NullReferenceException("UaePass:ClientId not be null.");
                var clientSecret = _config["ClientSecret"] ?? throw new NullReferenceException("UaePass:ClientSecret not be null.");

                // Build Basic credentials: "clientId:clientSecret" -> Base64
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                //_logger.LogInformation(message: $"Build Basic tokenUrl : {tokenUrl}");
                //_logger.LogInformation(message: $"Build Basic credentials : {credentials}");
                using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // Deserialize into the DTO that maps to the API response
                var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<UaePassTokenResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return tokenResponse ?? new UaePassTokenResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UAE Pass callback error during authentication flow.");
                throw;
            }
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("accessToken must be provided", nameof(accessToken));

            var userInfoEndpoint = _config["Endpoints:UserInfo"] ?? throw new NullReferenceException("UaePass:Endpoints:UserInfo not be null.");
            var url = _config["BaseUrl"] + userInfoEndpoint;

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var userInfo = System.Text.Json.JsonSerializer.Deserialize<UserInfoResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return userInfo ?? new UserInfoResponse();
        }

        public async Task<SignProcessResponse> CreateSignProcessAsync(string base64PdfString, string UniqueString, string lang, string FileName, string _accessToken, EOGETCurrentLandlordSignatureDetailsResult CSDetails)
        {
            var url = _config["BaseUrl"] + _config["Endpoints:CreateSignProcess"];

            string processJson = _config["Process"] ?? throw new NullReferenceException("UaePass:Process not be null.");
            string redirectAllowedUrls = _configApp["RedirectAllowedUrls"] ?? throw new NullReferenceException("App:RedirectAllowedUrls not be null.");
            string redirectionUrl = _config["redirectionUrl"] ?? throw new NullReferenceException("UaePass:redirectionUrl not be null.");
            DefaultSignaturePlacementDTO defaultSignaturePlacement =
                _config.GetSection("DefaultSignaturePlacement").Get<DefaultSignaturePlacementDTO>()
                ?? new DefaultSignaturePlacementDTO();
            string uniqId = DateTime.Now.ToString("yyyyMMddHHmmss");
            _logger.LogInformation("CSDetails - CSDetails-EOGETCurrentLandlordSignatureDetailsResult:{CSDetails}", JsonSerializer.Serialize(CSDetails));
            if (CSDetails.CurrentSignerLevel == 1 || CSDetails.CurrentSignerLevel == 0 || CSDetails.SignatureType == "OR")
            {
                defaultSignaturePlacement.PageNumber = CSDetails.IsNewNOC && CSDetails.SignatureType == "OR" ? 10 :
                    CSDetails.IsNewNOC && (CSDetails.NOCFor == 3 || CSDetails.NOCFor == 4) ? 4 : !CSDetails.IsNewNOC ? 2 : 3;
                defaultSignaturePlacement.X = 130;
                defaultSignaturePlacement.Y = CSDetails.IsNewNOC ? 540 : 700;
                defaultSignaturePlacement.Name = "Sign1";
            }
            else
            {
                defaultSignaturePlacement.PageNumber = CSDetails.IsNewNOC && CSDetails.CurrentSignerLevel > 6 && (CSDetails.NOCFor == 3 || CSDetails.NOCFor == 4) ? 5 : CSDetails.IsNewNOC && CSDetails.CurrentSignerLevel > 6 ? 4 :
                                                       CSDetails.IsNewNOC && (CSDetails.NOCFor == 3 || CSDetails.NOCFor == 4) ? 4 : !CSDetails.IsNewNOC ? 2 : 3;
                defaultSignaturePlacement.X = 130;
                defaultSignaturePlacement.Y = CSDetails.IsNewNOC && CSDetails.CurrentSignerLevel > 6 ?
                                                840 - 90 * Math.Abs(6 - CSDetails.CurrentSignerLevel)
                                                : !CSDetails.IsNewNOC ? 700 - 90 * Math.Abs(CSDetails.CurrentSignerLevel - 1) :
                                                540 - 90 * Math.Abs(CSDetails.CurrentSignerLevel - 1);
                defaultSignaturePlacement.Name = "Sign" + CSDetails.CurrentSignerLevel + "_" + uniqId;
            }


            redirectAllowedUrls = redirectAllowedUrls + $"/{lang}" + redirectionUrl;
            processJson = processJson.Replace("{{{{redirectionUrl}}}}", redirectAllowedUrls)
                .Replace("{{{{page-number}}}}", defaultSignaturePlacement.PageNumber.ToString())
                .Replace("{{{{x-axis}}}}", defaultSignaturePlacement.X.ToString())
                .Replace("{{{{y-axis}}}}", defaultSignaturePlacement.Y.ToString())
                .Replace("{{{{height}}}}", defaultSignaturePlacement.Height.ToString())
                .Replace("{{{{width}}}}", defaultSignaturePlacement.Width.ToString())
                .Replace("{{{signature_field_name}}}", defaultSignaturePlacement.Name)
                ;

            //_logger.LogInformation("base64PdfString - base64PdfString:{base64PdfString}", base64PdfString);
            _logger.LogInformation("ProcessJson - Length: {Length}", processJson?.Length ?? 0);
            _logger.LogInformation("ProcessJson content: {ProcessJson}", processJson);

            byte[] fileBytes = Convert.FromBase64String(base64PdfString);
            using var fileStream = new MemoryStream(fileBytes);
            fileStream.Position = 0;
            var fileContent = new StreamContent(fileStream);
            using var form = new MultipartFormDataContent();

            var jsonContent = new StringContent(processJson, Encoding.UTF8, "application/json");
            form.Add(jsonContent, "process");

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "document", fileName: FileName);

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = form
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            request.Content = form;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<SignProcessResponse>(json);
        }

        public async Task<byte[]> FetchSignedDocumentAsync(string documentId, string _accessToken)
        {
            var url = $"{_config["BaseUrl"]}{_config["Endpoints:FetchSignedDocument"]}{documentId}/content";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public string GetuaepassloginRedirectionLink(string token)
        {
            string baseurl = _config["BaseUrl"] ?? throw new NullReferenceException("UaePass:BaseUrl not be null.");
            string loginRedirectionUri = _config["loginRedirectionUri"] ?? throw new NullReferenceException("UaePass:loginRedirectionUri not be null.");
            string clientId = _config["ClientId"] ?? throw new NullReferenceException("UaePass:ClientId not be null.");
            string authRedirecturi = _config["authRedirecturi"] ?? throw new NullReferenceException("UAEPass:authRedirecturi not be null.");

            loginRedirectionUri = baseurl + loginRedirectionUri.Replace("{{{clientId}}}", Uri.EscapeDataString(clientId))
                                                   .Replace("{{{authRedirecturi}}}", authRedirecturi)
                                                   .Replace("{{{Token}}}", Uri.EscapeDataString(token));

            return loginRedirectionUri;
        }

        public string GetuaepasslogoutRedirectionLink(string token)
        {
            string baseurl = _config["BaseUrl"] ?? throw new NullReferenceException("UaePass:BaseUrl not be null.");
            string logoutRedirectionUri = _config["logoutRedirectionUri"] ?? throw new NullReferenceException("UaePass:logoutRedirectionUri not be null.");
            logoutRedirectionUri = baseurl + logoutRedirectionUri.Replace("{{{token}}}", token);
            return logoutRedirectionUri;
        }

        public string GetuaepassLogoutUrlwithLoginRedirectionlink(string link)
        {
            string baseurl = _config["BaseUrl"] ?? throw new NullReferenceException("UaePass:BaseUrl not be null.");
            string logoutRedirectionUri = _config["logoutRedirectionUriV2"] ?? throw new NullReferenceException("UaePass:logoutRedirectionUriV2 not be null.");
            logoutRedirectionUri = baseurl + logoutRedirectionUri.Replace("{{{loginurl}}}", link);
            return logoutRedirectionUri;
        }

        private class DefaultSignaturePlacementDTO
        {
            public int PageNumber { get; set; } = 3;
            public int X { get; set; } = 130;
            public int Y { get; set; } = 560;
            public int Height { get; set; } = 100;
            public int Width { get; set; } = 350;
            public string Name { get; set; } = "Sign1";
        }
    }
}
