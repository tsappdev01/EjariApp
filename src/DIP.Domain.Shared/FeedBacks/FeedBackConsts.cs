namespace DIP.FeedBacks
{
    public static class FeedBackConsts
    {
        private const string DefaultSorting = "{0}CreationTime DESC";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "FeedBack." : string.Empty);
        }

    }
}