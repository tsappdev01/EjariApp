using Slugify;
using System;
using System.Collections.Generic;
using System.Text;

namespace DIP.Helper
{
    public static class Slug
    {
        static SlugHelper helper = new SlugHelper();

        public static string GenerateSlug(string input)
        {
            if(!input.IsNullOrEmpty())
                return helper.GenerateSlug(input);
            return null;
        }
    }
}
