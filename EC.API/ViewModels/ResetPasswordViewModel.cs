using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EC.API.ViewModels
{
    public class ResetPasswordViewModel
    {
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        [Required (ErrorMessage ="email required.")]
        public string Email { get; set; }
        [DisplayName("New Password")]
        [Required(ErrorMessage = "new password required.")]
        [RegularExpression(@"(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$",
         ErrorMessage = "Password expects atleast 1 Capital letter, 1 small-case letter, 1 digit, 1 special character and the length should be minimum 8 characters.")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "confirm password required.")]
        [Compare("Password", ErrorMessage = "Confirm password Does not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class ResetPasswordResponseViewModel
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Fill First Name")]
        [RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid First Name")]
        public string Firstname { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Fill last Name")]
        [RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid Last Name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please Fill Email")]
        //[Required, MinLength(1), DataType(DataType.EmailAddress), EmailAddress, MaxLength(50), Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }

    }

    public class ResetPasswordModel
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
    }
}
