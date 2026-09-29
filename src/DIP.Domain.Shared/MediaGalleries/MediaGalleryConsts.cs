namespace DIP.MediaGalleries
{
    public static class MediaGalleryConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "MediaGallery." : string.Empty);
        }

    }
}