namespace DIP.PressReleases
{
    public static class PressReleaseConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PressRelease." : string.Empty);
        }

    }
}