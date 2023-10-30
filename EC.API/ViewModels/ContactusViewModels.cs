using System;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class ContactusViewModels
    {
        [Required(ErrorMessage = "FirstName Required")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "LastName Required")]
        public string lastname { get; set; } = string.Empty;

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        [Required(ErrorMessage = "Email Required")]
        public string email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone Required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string phone { get; set; }
        [Required(ErrorMessage = "Message Required")]
        public string message { get; set; }

    }

    public class ContactusEnquiryViewModels
    {
        [Required(ErrorMessage = "FirstName Required")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "LastName Required")]
        public string lastname { get; set; } = string.Empty;

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        [Required(ErrorMessage = "Email Required")]
        public string email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone Required")]
        //[RegularExpression(@"^\(?([0-8]{3})\)?[-. ]?([0-8]{3})[-. ]?([0-8]{4})$", ErrorMessage = "Not a valid phone number")]
        public string phone { get; set; }
        [Required(ErrorMessage = "Message Required")]
        public string message { get; set; }
        //[Range(1, string., ErrorMessage = "Productid should be greater than 0")]
        public string product_id { get; set; }
    }
}
