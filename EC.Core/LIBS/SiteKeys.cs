using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EC.Core.LIBS
{
    public class SiteKeys
    {
        private static IConfigurationSection _configuration;
        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }
        

        public static string Domain => _configuration["Domain"];
        public static string DomainWithSlash => _configuration["Domain"]+"/";

        public static string AdminEmail => _configuration["AdminEmail"];
        public static string FacebookUrl => _configuration["FacebookUrl"];
        public static string YoutubeUrl => _configuration["YoutubeUrl"];
        public static string InstagramUrl => _configuration["InstagramUrl"];
        public static string FacebookGroupUrl => _configuration["FacebookGroupUrl"];
        public static string StripeKey => _configuration["StripeKey"];
        public static string SMTPHost => _configuration["SMTPHost"];
        public static string SMTPPort => _configuration["SMTPPort"];
        public static string SMTPUserName => _configuration["SMTPUserName"];
        public static string SMTPPassword => _configuration["SMTPPassword"];
        public static string EmailTemplatePath => _configuration["EmailTemplatePath"];
        public static string StripeCurrency => _configuration["Currency"];
        public static string SupportEmail => _configuration["SupportEmail"];

    }
}
