namespace DIP.EFormServiceSubCategories
{
    public static class EFormServiceSubCategoryConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "EFormServiceSubCategory." : string.Empty);
        }

    }
}