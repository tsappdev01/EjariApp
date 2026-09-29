namespace DIP.DipFacts
{
    public static class DipFactConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "DipFact." : string.Empty);
        }

    }
}