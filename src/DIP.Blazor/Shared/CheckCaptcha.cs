using DIP.Settings;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DIP.Blazor.Shared
{
    public class CheckCaptcha
    {
        private HttpClient HttpClient { get; set; }
        public async Task<bool> CheckCaptchaResponse(Captcha captcha, GoogleReCaptachDto googleReCaptachDto)
        {
            try
            {
                string reCaptchaResponse = "";
                reCaptchaResponse = await captcha.GetResponseAsync();
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "secret", googleReCaptachDto.SecretKey},
                    { "response", reCaptchaResponse}
                });

                HttpClient = new HttpClient();
                var response = await HttpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

                if (response.IsSuccessStatusCode)
                {
                    // var jsonString = await response.Content.Rea();
                    var verificationResponse = await response.Content.ReadFromJsonAsync<CaptchaVerificationResponse>();
                    if (verificationResponse != null)
                        return verificationResponse.Success;
                    return false;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
