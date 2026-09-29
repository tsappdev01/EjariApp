

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Azure;
using Microsoft.AspNetCore.Localization;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using AutoMapper;
using Humanizer.Localisation;
using System.Threading;

namespace DIP.Blazor
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
     
        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var url1 = httpContext.Request.GetDisplayUrl();
            var url2 = httpContext.Request.GetEncodedUrl();
            var path = httpContext.Request.Path.ToString();
            IRequestCultureFeature culture = httpContext.Features.Get<IRequestCultureFeature>();
            if (path != "/" && path != "")
            {
                if (path.StartsWith("/en"))
                {
                    if (culture.RequestCulture.Culture.Name != "en")
                    {
                        httpContext.Response.Cookies.Append(
              CookieRequestCultureProvider.DefaultCookieName,
              CookieRequestCultureProvider.MakeCookieValue(
                  new RequestCulture("en")));
                        CultureInfo.CurrentCulture = new CultureInfo("en");

                        CultureInfo ci = new CultureInfo("en");

                        Thread.CurrentThread.CurrentCulture = ci;
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);
                    }

                }
                else
                {
                    if (path.StartsWith("/ar"))
                    {
                        if (culture.RequestCulture.Culture.Name != "ar")

                        {
                            httpContext.Response.Cookies.Append(
                   CookieRequestCultureProvider.DefaultCookieName,
                   CookieRequestCultureProvider.MakeCookieValue(
                       new RequestCulture("ar")));
                            CultureInfo.CurrentCulture = new CultureInfo("ar");

                            CultureInfo ci = new CultureInfo("ar");

                            Thread.CurrentThread.CurrentCulture = ci;
                            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);

                        }

                    }
                }
            }
            else
            {
                if(path.EndsWith("/ar"))
                {
                    if (culture.RequestCulture.Culture.Name != "ar")
                    {
                        httpContext.Response.Cookies.Append(
             CookieRequestCultureProvider.DefaultCookieName,
             CookieRequestCultureProvider.MakeCookieValue(
                 new RequestCulture("ar")));
                        CultureInfo.CurrentCulture = new CultureInfo("ar");

                        CultureInfo ci = new CultureInfo("ar");

                        Thread.CurrentThread.CurrentCulture = ci;
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);


                    }
                }
                else
                {
                    if(path.Equals("/"))
                    {
                        if (culture.RequestCulture.Culture.Name != "en")
                        {
                            httpContext.Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(
                            new RequestCulture("en")));
                            CultureInfo.CurrentCulture = new CultureInfo("en");

                            CultureInfo ci = new CultureInfo("en");

                            Thread.CurrentThread.CurrentCulture = ci;
                            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(ci.Name);

                        }

                       

                    }
                }
            }

          
            await _next(httpContext);



        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
    }

}
