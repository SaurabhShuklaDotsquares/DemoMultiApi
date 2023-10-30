using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

public class ContextProvider
{
    private static IHttpContextAccessor _httpContextAccessor;
    private static IWebHostEnvironment _hostingEnvironment;

    public static void Configure(
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment hostingEnvironment)
    {
        _httpContextAccessor = httpContextAccessor;
        _hostingEnvironment = hostingEnvironment;
    }

    public static HttpContext HttpContext
    {
        get
        {
            return _httpContextAccessor.HttpContext;
        }
    }

    public static Uri AbsoluteUri
    {
        get
        {
            var request = _httpContextAccessor.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            return uriBuilder.Uri;
        }
    }

    public static IWebHostEnvironment HostEnvironment
    {
        get
        {
            return _hostingEnvironment;
        }
    }

}