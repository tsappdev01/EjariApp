namespace DIP.Amenities
{
    public static class AmenityConsts
    {
        private const string DefaultSorting = "{0}Order asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Amenity." : string.Empty);
        }

    }
}