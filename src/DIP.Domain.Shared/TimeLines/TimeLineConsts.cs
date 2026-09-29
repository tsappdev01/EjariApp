namespace DIP.TimeLines
{
    public static class TimeLineConsts
    {
        private const string DefaultSorting = "{0}TitleEn asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "TimeLine." : string.Empty);
        }

    }
}