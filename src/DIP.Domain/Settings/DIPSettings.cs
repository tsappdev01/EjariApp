namespace DIP.Settings;

public static class DIPSettings
{
    private const string Prefix = "DIP";
    private const string PrefixAbp = "Abp";
    //Add your own setting names here. Example:
    //public const string MySetting1 = Prefix + ".MySetting1";
    public static class GoogleReCaptcha
    {
        public const string SiteKey = Prefix + ".GoogleReCaptcha.SiteKey";
        public const string SecretKey = Prefix + ".GoogleReCaptcha.SecretKey";
    }

    public static class Smtp
    {
        public const string Host = PrefixAbp + ".Mailing.Smtp" + ".Host";
        public const string Port = PrefixAbp + ".Mailing.Smtp" + ".Port";
        public const string UserName = PrefixAbp + ".Mailing.Smtp" + ".UserName";
        public const string Password = PrefixAbp + ".Mailing.Smtp" + ".Password";
        public const string Domain = PrefixAbp + ".Mailing.Smtp" + ".Domain";
        public const string EnableSsl = PrefixAbp + ".Mailing.Smtp" + ".EnableSsl";
        public const string UseDefaultCredentials = PrefixAbp + ".Mailing.Smtp" + ".UseDefaultCredentials";
        public const string DefaultFromAddress = PrefixAbp + ".Mailing" + ".DefaultFromAddress";
        public const string DefaultFromDisplayName = PrefixAbp + ".Mailing" + ".DefaultFromDisplayName";
    }
}
