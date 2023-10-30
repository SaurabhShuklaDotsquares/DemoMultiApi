using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace EC.API.ViewModels
{
    public class UserManagerViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Fill First Name")]
        [RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid First Name")]
        public string Firstname { get; set; }
        //[DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Fill last Name")]
        [RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid Last Name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string Mobile { get; set; }
       
        [Required(ErrorMessage = "Please Fill Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+[^\s]+$)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please fill Password")]

        [RegularExpression("^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[()-_\\|;:'\",!~/@#$%^&+=]).*$", ErrorMessage = "Password must contains at least 8 char(one special character, one uppercase, one lowercase(in any order)).")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //[DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not Match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Please Check Gender")]
        public byte Gender { get; set; }
        
    }

    public class UserManagersViewModel
    {
        //[Required(ErrorMessage = "FirstName Required.")]
        [Required(ErrorMessage = "Please Fill First Name"), MinLength(3), DataType(DataType.Text), MaxLength(50), Display(Name = "First Name")]
        //[RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Please enter a valid First Name")]
        public string Firstname { get; set; }
        //[Required(ErrorMessage = "LastName Required.")]
        //[RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid Last Name")]
        [Required(ErrorMessage = "Please Fill Last Name"), MinLength(3), DataType(DataType.Text), MaxLength(50), Display(Name = "Last Name")]

        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Email Required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Required.")]
        [RegularExpression("^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[()-_\\|;:'\",!~/@#$%^&+=]).*$", ErrorMessage = "Password must contains at least 8 char(one special character, one uppercase, one lowercase(in any order)).")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //[Required(ErrorMessage = "Please Check Gender")]
        //public byte Gender { get; set; }
        //[Display(Name = "Image")]
        //[Required(ErrorMessage = "Please upload Image")]
        //public IFormFile Image { get; set; }
    }
}
