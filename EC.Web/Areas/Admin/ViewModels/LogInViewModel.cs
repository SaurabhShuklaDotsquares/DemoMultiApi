using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class LogInViewModel
    {
        [Required(ErrorMessage = "*required")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "*required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string MailLogovalue { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
