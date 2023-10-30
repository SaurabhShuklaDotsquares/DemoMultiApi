using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class UserViewModels
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        //[Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string mobile { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string email { get; set; }
        public IFormFile profile_pic { get; set; }
        //[Display(Name = "Image")]
        //[Required(ErrorMessage = "Please upload Image")]
        //public string profile_pic { get; set; }
    }
    public class UpdateUserViewModels
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        //[Display(Name = "Image")]
        //[Required(ErrorMessage = "Please upload Image")]
        public string profile_pic { get; set; }
    }
    public class UsersViewModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Fill First Name")]
        [RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid First Name")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Please Fill last Name")]
        [RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid Last Name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string Mobile { get; set; }
        public string Email { get; set; }
    }


    public class UserViewModel
    {
        public int id { get; set; }
        public int? role { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string profile_pic { get; set; }
    }
}
