using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EC.API.ViewModels
{
    public class LogInViewModel
    {
        public int user_id { get; set; }
        public int? role { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public bool isGuest { get; set; }
        public string api_token { get; set; }
        public string profile_pic { get; set; }
        public bool rememberme { get; set; }
        public string auth_key { get; set; }
    }
}
