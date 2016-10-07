/******************************************************************************
 * Filename: WebConvert.cs
 * Project:  UTC
 * 
 * Description:
 * Web-safe conversion utilities that do not fail (return a default value).
 * 
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace com.unitethiscity.api.Models
{
    /// <summary>
    /// The WebConvert class is a collection of conversion routines designed for use with the web.
    /// </summary>
    public static class WebConvert
    {
        /// <summary>
        /// Attempt to convert the supplied object to an Int32.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static int ToInt32(object input, int defaultValue)
        {
            int ret = defaultValue;

            if (input != null)
            {
                if (!Int32.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a single-precision float.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static float ToSingle(object input, float defaultValue)
        {
            float ret = defaultValue;

            if (input != null)
            {
                if (!Single.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a double-precision float.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static double ToDouble(object input, double defaultValue)
        {
            double ret = defaultValue;

            if (input != null)
            {
                if (!Double.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a decimal.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static decimal ToDecimal(object input, decimal defaultValue)
        {
            decimal ret = defaultValue;

            if (input != null)
            {
                if (!Decimal.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a long.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static long ToLong(object input, long defaultValue)
        {
            long ret = defaultValue;

            if (input != null)
            {
                if (!long.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a String.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static string ToString(object input, string defaultValue)
        {
            string ret = defaultValue;

            if (input != null)
            {
                if (!String.IsNullOrEmpty(input.ToString()))
                {
                    ret = input.ToString();
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a Boolean.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static bool ToBoolean(object input, bool defaultValue)
        {
            bool ret = defaultValue;

            if (input != null)
            {
                if (!Boolean.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a DateTime.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static DateTime ToDateTime(object input, DateTime defaultValue)
        {
            DateTime ret = defaultValue;

            if (input != null)
            {
                if (!DateTime.TryParse(input.ToString(), out ret))
                {
                    ret = defaultValue;
                }
            }

            return ret;
        }

        /// <summary>
        /// Attempt to convert the supplied object to a nullable DateTime.
        /// </summary>
        /// <param name="input">The object to parse.</param>
        /// <param name="defaultValue">The default value to use if the conversion fails.</param>
        /// <returns>The converted value if the conversion passes, otherwise the default value.</returns>
        public static DateTime? ToDateTime(object input, DateTime? defaultValue)
        {
            DateTime? ret = defaultValue;

            if (input != null)
            {
                DateTime dtParse;

                if (DateTime.TryParse(input.ToString(), out dtParse))
                {
                    ret = dtParse;
                }
            }

            return ret;
        }

        /// <summary>
        /// Retrieve the value of a cookie by name
        /// </summary>
        /// <param name="cookieName">Name of the cookie</param>
        /// <returns>The stored value of the named cookie if it exists</returns>
        public static string CookieValue(string cookieName)
        {
            string ret = "";

            HttpCookie cke = HttpContext.Current.Request.Cookies[cookieName];

            if (cke != null)
            {
                ret = cke.Value;
            }

            return ret;
        }

        /// <summary>
        /// Truncate the supplied string to the max length if necessary.
        /// </summary>
        public static string Truncate(string input, int maxLength)
        {
            return (input.Length > maxLength) ? input.Substring(0, maxLength) : input;
        }

        /// <summary>
        /// Truncate the supplied string to the max length if necessary and add an elipsis
        /// </summary>
        public static string Summarize(string input, int maxLength)
        {
            return (input.Length > maxLength) ? input.Substring(0, maxLength) + "..." : input;
        }

        /// <summary>
        /// Preserve carriage returns on the web by converting them to breaks.
        /// </summary>
        /// <param name="input">The string string containing carriage return breaks.</param>
        /// <returns>A string with preserved breaks for the web.</returns>
        public static string PreserveBreaks(string input)
        {
            return input.Replace(Environment.NewLine, "<br />");
        }

        public static string FormatDisplayDate(DateTime dt)
        {
            return String.Format("{0:MMMM d, yyyy}", dt);
        }

        public static string FormatDisplayDate(DateTime? dt)
        {
            return FormatDisplayDate(ToDateTime(dt, DateTime.Now));
        }
    }
}