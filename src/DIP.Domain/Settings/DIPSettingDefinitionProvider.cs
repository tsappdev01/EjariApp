using Volo.Abp.Settings;

namespace DIP.Settings;

public class DIPSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DIPSettings.MySetting1));
        context.Add(new SettingDefinition(DIPSettings.GoogleReCaptcha.SiteKey, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.GoogleReCaptcha.SecretKey, isVisibleToClients: false));

        context.Add(new SettingDefinition(DIPSettings.Smtp.Host, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.Port, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.UserName, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.Password, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.Domain, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.EnableSsl, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.UseDefaultCredentials, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.DefaultFromAddress, isVisibleToClients: false));
        context.Add(new SettingDefinition(DIPSettings.Smtp.DefaultFromDisplayName, isVisibleToClients: false));
    }
}
