using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using OneDirect.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetCookie(this HttpContext httpContext, string key, string value, CookieOptions options = null)
        {
            if (httpContext == null || string.IsNullOrEmpty(value))
                return;
            if (httpContext.Request == null || httpContext.Response == null)
                return;

            if (httpContext.Request.Cookies.ContainsKey(key))
            {
                httpContext.Response.Cookies.Delete(key, new CookieOptions() { Expires = DateTimeOffset.MinValue });
            }
            value = Utilities.EncryptText(value);
            if (options == null)
            {
                httpContext.Response.Cookies.Append(key, value);
            }
            else
            {
                options.Path = "/";
                httpContext.Response.Cookies.Append(key, value, options);
            }
        }

        public static void SetCookie(this HttpContext httpContext, string key, string value, int? expireTime, CookieExpiryIn? expiryIn)
        {
            if (httpContext == null || string.IsNullOrEmpty(value))
                return;
            if (httpContext.Request == null || httpContext.Response == null)
                return;

            if (httpContext.Request.Cookies.ContainsKey(key))
            {
                httpContext.Response.Cookies.Delete(key, new CookieOptions() { Expires = DateTimeOffset.MinValue });
            }
            value = Utilities.EncryptText(value);
            if (expireTime.HasValue && expiryIn.HasValue)
            {
                var options = new CookieOptions() { Path = "/" };
                if (expiryIn.HasValue)
                {
                    switch (expiryIn.Value)
                    {
                        case CookieExpiryIn.Seconds:
                            options.Expires = DateTime.UtcNow.AddSeconds(expireTime.Value);
                            break;
                        case CookieExpiryIn.Minitues:
                            options.Expires = DateTime.UtcNow.AddMinutes(expireTime.Value);
                            break;
                        case CookieExpiryIn.Hours:
                            options.Expires = DateTime.UtcNow.AddHours(expireTime.Value);
                            break;
                        case CookieExpiryIn.Days:
                            options.Expires = DateTime.UtcNow.AddDays(expireTime.Value);
                            break;
                        case CookieExpiryIn.Months:
                            options.Expires = DateTime.UtcNow.AddMonths(expireTime.Value);
                            break;
                        case CookieExpiryIn.Years:
                            options.Expires = DateTime.UtcNow.AddYears(expireTime.Value);
                            break;
                    }
                }
                else
                {
                    options.Expires = DateTime.UtcNow.AddDays(expireTime.Value);
                }
                httpContext.Response.Cookies.Append(key, value, options);
            }
            else
            {
                httpContext.Response.Cookies.Append(key, value);
            }
        }

        public static void RemoveCookie(this HttpContext httpContext, string key)
        {
            if (httpContext == null || string.IsNullOrEmpty(key))
                return;
            if (httpContext.Request == null)
                return;
            if (httpContext.Request.Cookies.ContainsKey(key))
                httpContext.Response.Cookies.Delete(key);
        }

        public static string GetCookie(this HttpContext httpContext, string key)
        {
            if (httpContext == null || string.IsNullOrEmpty(key))
                return "";
            if (httpContext.Request == null)
                return "";

            string cookieValue = string.Empty;
            if (httpContext.Request.Cookies.ContainsKey(key))
            {
                cookieValue = httpContext.Request.Cookies[key];
                if (!string.IsNullOrEmpty(cookieValue))
                    cookieValue = Utilities.DecryptText(cookieValue);
            }
            return cookieValue;
        }

        public static string GetClientIP(this HttpContext httpContext)
        {
            string ip = string.Empty;
            try
            {
                ip = httpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("ERROR: {0}", ex.Message));
            }

            return ip;
        }
    }
}
