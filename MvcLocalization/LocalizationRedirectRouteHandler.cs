using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcLocalization
{
    public class LocalizationRedirectRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var routeValues = requestContext.RouteData.Values;

            var cookieLocale = requestContext.HttpContext.Request.Cookies["locale"];
            if (cookieLocale != null)
            {
                routeValues["culture"] = cookieLocale.Value;
            }
            else
            {
                var uiCulture = CultureInfo.CurrentUICulture;
                routeValues["culture"] = uiCulture.Name;
            }

            var queryString = requestContext.HttpContext.Request.QueryString;
            foreach (var key in queryString.AllKeys)
            {
                if (!routeValues.ContainsKey(key))
                {
                    routeValues.Add(key, queryString[key]);
                }
            }

            var redirectUrl = new UrlHelper(requestContext).RouteUrl(routeValues);
            return new RedirectHandler(redirectUrl);
        }
    }
}