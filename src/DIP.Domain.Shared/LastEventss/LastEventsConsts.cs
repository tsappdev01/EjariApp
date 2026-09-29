namespace DIP.LastEventss
{
    public static class LastEventsConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "LastEvents." : string.Empty);
        }

    }
}