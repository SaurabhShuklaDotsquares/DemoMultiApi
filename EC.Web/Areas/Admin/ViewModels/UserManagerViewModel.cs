using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class UserManagerViewModel
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Fill First Name")]
        [RegularExpression(@"^([a-zA-Z0-9]+$)", ErrorMessage = "Please enter a valid First Name")]
        public string Firstname { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Fill last Name")]
        [RegularExpression(@"^([a-zA-Z0-9]+$)", ErrorMessage = "Please enter a valid Last Name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Please Fill Email")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid e-mail address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please fill Password")]
        
        [RegularExpression("^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[()-_\\|;:'\",!~/@#$%^&+=]).*$", ErrorMessage = "Password must contains at least 8 char(one special character, one uppercase, one lowercase(in any order)).")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please fill confirm password")]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not Match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        //[Required(ErrorMessage = "Please Check Gender")]
        //public byte Gender { get; set; }
        public string Name { get; set; }
    }
}
