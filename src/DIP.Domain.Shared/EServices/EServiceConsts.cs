namespace DIP.EServices
{
    public static class EServiceConsts
    {
        private const string DefaultSorting = "{0}TitleEn asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "EService." : string.Empty);
        }

    }
}