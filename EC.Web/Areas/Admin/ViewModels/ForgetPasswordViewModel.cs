using System.ComponentModel.DataAnnotations;
using System;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class ForgetPasswordViewModel
    {

        [Required(ErrorMessage = "*required")]
        [EmailAddress(ErrorMessage = "Please enter a valid e-mail address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        public string ForgetPasswordLink { get; set; }
        public string ForgetPasswordMessage { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string replyName { get; set; }
    }
}
