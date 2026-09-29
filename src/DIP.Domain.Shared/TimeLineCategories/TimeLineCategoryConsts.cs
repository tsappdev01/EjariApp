namespace DIP.TimeLineCategories
{
    public static class TimeLineCategoryConsts
    {
        private const string DefaultSorting = "{0}TitleEn asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "TimeLineCategory." : string.Empty);
        }

    }
}