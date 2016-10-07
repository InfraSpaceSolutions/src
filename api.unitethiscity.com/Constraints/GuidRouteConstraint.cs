/******************************************************************************
 * Filename: GuidRouteConstraint.cs
 * Project:  UTC WebAPI
 * 
 * Description:
 * Routing support for parsing the access token from the URI.  The access
 * token is a Guid
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace com.unitethiscity.api.Constraints
{
    public class GuidRouteConstraint : IHttpRouteConstraint
    {
        private const string _format = "D";
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (values[parameterName] != RouteParameter.Optional)
            {
                object value;
                values.TryGetValue(parameterName, out value);
                string input = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
                Guid guidValue;
                return Guid.TryParseExact(input, _format, out guidValue);
            }
            return true;
        }
    }
}