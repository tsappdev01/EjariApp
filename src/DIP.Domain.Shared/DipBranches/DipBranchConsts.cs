namespace DIP.DipBranches
{
    public static class DipBranchConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "DipBranch." : string.Empty);
        }

    }
}