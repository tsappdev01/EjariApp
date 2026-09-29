namespace DIP.SupportedBanks
{
    public static class SupportedBankConsts
    {
        private const string DefaultSorting = "{0}TitleAr asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SupportedBank." : string.Empty);
        }

    }
}