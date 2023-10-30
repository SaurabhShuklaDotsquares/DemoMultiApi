using Microsoft.AspNetCore.Http;
using System;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class SettingViewModels
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SMTP_EMAIL_HOST { get; set; }
        public string SMTP_USERNAME { get; set; }
        public string SMTP_PASSWORD { get; set; }
        public string SMTP_PORT { get; set; }
        public string Slug { get; set; }
        public string SMTP_EMAIL_HOSTSlug { get; set; }
        public string SMTP_USERNAMESlug { get; set; }
        public string SMTP_PASSWORDSlug { get; set; }
        public string SMTP_PORTSlug { get; set; }
        public string ConfigValue { get; set; }
        public string Manager { get; set; }
        public string FieldType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string facebookSlug { get; set; }
        public string YoutubeSlug { get; set; }
        public string LinkdinSlug { get; set; }
        public string twitterSlug { get; set; }
        public string google_plusSlug { get; set; }

        public string facebookConfigValue { get; set; }
        public string YoutubeConfigValue { get; set; }
        public string LinkdinConfigValue { get; set; }
        public string twitterConfigValue { get; set; }
        public string google_plusConfigValue { get; set; }

        public string MAIN_LOGOSlug { get; set; }
        public string MAIN_FAVICONSlug { get; set; }
        public string MAIN_LOGOConfigValue { get; set; }
        public string MAIN_FAVICONConfigValue { get; set; }
        public IFormFile MAIN_FAVICONConfigValue1 { get; set; }
        public IFormFile MAIN_LOGOConfigValue1 { get; set; }
    }
}
