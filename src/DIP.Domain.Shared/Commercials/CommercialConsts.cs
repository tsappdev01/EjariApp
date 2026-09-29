namespace DIP.Commercials
{
    public static class CommercialConsts
    {
        private const string DefaultSorting = "{0}SubCategoryId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Commercial." : string.Empty);
        }

    }
}