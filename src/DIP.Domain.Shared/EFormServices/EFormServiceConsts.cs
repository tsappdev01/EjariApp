namespace DIP.EFormServices
{
    public static class EFormServiceConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "EFormService." : string.Empty);
        }

    }
}