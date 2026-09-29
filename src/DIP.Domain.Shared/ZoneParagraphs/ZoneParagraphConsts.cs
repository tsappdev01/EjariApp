namespace DIP.ZoneParagraphs
{
    public static class ZoneParagraphConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ZoneParagraph." : string.Empty);
        }

    }
}