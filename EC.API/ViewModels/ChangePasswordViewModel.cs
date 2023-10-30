using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$",ErrorMessage = "Password expects atleast 1 Capital letter, 1 small-case letter, 1 digit, 1 special character and the length should be minimum 8 characters.")]
        public string old_password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string new_password { get; set; }
    }
}
