namespace DIP.PageInfos
{
    public static class PageInfoConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PageInfo." : string.Empty);
        }

    }
}