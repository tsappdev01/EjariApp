namespace DIP.SliderHomePages
{
    public static class SliderHomePageConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "SliderHomePage." : string.Empty);
        }

    }
}