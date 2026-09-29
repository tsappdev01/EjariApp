namespace DIP.InquiryForms
{
    public static class InquiryFormConsts
    {
        private const string DefaultSorting = "{0}CreationTime DESC";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "InquiryForm." : string.Empty);
        }

    }
}