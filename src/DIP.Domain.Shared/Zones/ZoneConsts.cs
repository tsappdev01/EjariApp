namespace DIP.Zones
{
    public static class ZoneConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Zone." : string.Empty);
        }

    }
}