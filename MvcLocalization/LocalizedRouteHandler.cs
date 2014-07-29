using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcLocalization
{
    public class LocalizedRouteHandler : MvcRouteHandler
    {
        protected override System.Web.IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
            var urlLocale = requestContext.RouteData.Values["culture"] as string;
            var cultureName = urlLocale ?? "";

            var cookieLocale = requestContext.HttpContext.Request.Cookies["locale"];
            if (cookieLocale != null)
            {
                // if request contains locale cookie, we need to put higher priority than url locale
                // user might click the link from somewhere but he/she already set different locale
                if (!cookieLocale.Value.Equals(urlLocale, StringComparison.OrdinalIgnoreCase))
                {
                    // if cookie locale and url cookie are different,
                    // we should redirect with cookie locale
                    var routeValues = requestContext.RouteData.Values;
                    routeValues["culture"] = cookieLocale.Value;

                    var queryString = requestContext.HttpContext.Request.QueryString;
                    foreach (var key in queryString.AllKeys)
                    {
                        if (!routeValues.ContainsKey(key))
                        {
                            routeValues.Add(key, queryString[key]);
                        }
                    }

                    return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
                }
                else
                {
                    cultureName = cookieLocale.Value;
                }
            }

            if (cultureName == "")
            {
                return GetDefaultLocaleRedirectHandler(requestContext);
            }

            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                // if CultureInfo.GetCultureInfo throws exception
                // we should redirect with default locale
                return GetDefaultLocaleRedirectHandler(requestContext);
            }

            if (cookieLocale == null)
            {
                requestContext.HttpContext.Response.AppendCookie(new HttpCookie("locale", cultureName));
            }
            return base.GetHttpHandler(requestContext);
        }

        private static IHttpHandler GetDefaultLocaleRedirectHandler(RequestContext requestContext)
        {
            var uiCulture = CultureInfo.CurrentUICulture;
            var routeValues = requestContext.RouteData.Values;
            routeValues["culture"] = uiCulture.Name;
            return new RedirectHandler(new UrlHelper(requestContext).RouteUrl(routeValues));
        }
    }
}