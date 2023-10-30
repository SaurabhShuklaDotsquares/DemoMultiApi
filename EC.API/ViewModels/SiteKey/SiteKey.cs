using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace EC.API.ViewModels.SiteKey
{
    public class SiteKey
    {
        private static IConfigurationSection _configuration;
        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }


        public static string Domains => _configuration["Domains"];
        public static string Domain => _configuration["Domain"];
        public static string DomainWithSlash => _configuration["Domains"] + "/";

        public static string AdminEmail => _configuration["AdminEmail"];
        public static string FacebookUrl => _configuration["FacebookUrl"];
        public static string YoutubeUrl => _configuration["YoutubeUrl"];
        public static string InstagramUrl => _configuration["InstagramUrl"];
        public static string FacebookGroupUrl => _configuration["FacebookGroupUrl"];
        public static string DefaultImage => _configuration["DefaultImage"];
        public static string ImagePath => _configuration["ImagePath"];
        public static string Currency => _configuration["Currency"];
        public static string StripeKeys => _configuration["StripeKey"];
        public static string SupportEmail => _configuration["SupportEmail"];
        public static string BisinessName => _configuration["BisinessName"];
        public static string FrontedDomain => _configuration["FrontedDomain"];
        public static string FrontedB2BDomain => _configuration["FrontedB2BDomain"];
        //public static string FrontedVerifyDomain => _configuration["FrontedVerifyDomain"];
        public static string FrontedLogInDomain => _configuration["FrontedLogInDomain"];
        public static string FrontedB2BLogInDomain => _configuration["FrontedB2BLogInDomain"];
        public static string UploadImage => _configuration["UploadImage"];
        public static string UploadExel => _configuration["UploadExel"];
        public static string CurrencySymbol => _configuration["currencySymbol"];
        public static string CurrencyIso => _configuration["currencyIso"];
        public static string DefaultCurrency => _configuration["defaultCurrency"];
        
        //public static string BasePath => AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("\\bin")); 
        //public static string BasePath => AppContext.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("\\bin"));
        //public static string BasePath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

    }
}
