namespace DIP.ContactForms
{
    public static class ContactFormConsts
    {
        private const string DefaultSorting = "{0}CreationTime DESC";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ContactForm." : string.Empty);
        }

    }
}