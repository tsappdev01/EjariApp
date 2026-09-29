using DIP.Settings;
using System.Threading.Tasks;

namespace DIP.Settings
{
    public interface IDIPSettingAppService
    {
        Task<GoogleReCaptachDto> GetGoogleReCaptachSettingAsync();
        //Task UpdateGoogleReCaptachSettingAsync(GoogleReCaptachDto input);
        Task<SmtpSettingDto> GetSmtpSettingAsync();
    }
}