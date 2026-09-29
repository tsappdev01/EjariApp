namespace DIP.MajorIndustries
{
    public static class MajorIndustryConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MajorIndustry." : string.Empty);
        }

    }
}