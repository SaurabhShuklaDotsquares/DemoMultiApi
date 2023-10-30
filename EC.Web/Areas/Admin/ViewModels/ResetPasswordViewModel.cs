using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DisplayName("New Password")]
        [Required(ErrorMessage = "Please fill New Password")]
        [RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$",
         ErrorMessage = "Password expects atleast 1 Capital letter, 1 small-case letter, 1 digit, 1 special character and the length should be minimum 8 characters.")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm password does not match.")]
        public string ConfirmPassword { get; set; }

        public int Id { get; set; }
    }
    
}
