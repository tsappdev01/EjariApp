namespace DIP.SiteSettings
{
    public static class SiteSettingConsts
    {
        private const string DefaultSorting = "{0}FaceBookLink asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SiteSetting." : string.Empty);
        }

    }
}