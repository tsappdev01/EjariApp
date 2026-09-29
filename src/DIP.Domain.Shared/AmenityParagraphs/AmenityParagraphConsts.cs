namespace DIP.AmenityParagraphs
{
    public static class AmenityParagraphConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "AmenityParagraph." : string.Empty);
        }

    }
}