using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace EC.API.ViewModels.MultiVendor
{
    public class UserVendorViewModels
    {
        [Required(ErrorMessage = "Please Fill Business Registration Number")]
        public string vat_no { get; set; }
        [Required(ErrorMessage = "Please Fill Business Name")]
        public string business_name { get; set; }
        //[Required(ErrorMessage = "Please Fill First Name")]
        [Required(ErrorMessage = "Please Fill First Name"), MinLength(3), DataType(DataType.Text), MaxLength(50), Display(Name = "First Name")]
        //[RegularExpression(@"^([a-zA-Z0-9]+$)", ErrorMessage = "Please enter a valid First Name")]
        public string firstname { get; set; }
        //[Required(ErrorMessage = "Please Fill last Name")]
        [Required(ErrorMessage = "Please Fill last Name"), MinLength(3), DataType(DataType.Text), MaxLength(50), Display(Name = "last Name")]
        //[RegularExpression(@"^([a-zA-Z0-9]+$)", ErrorMessage = "Please enter a valid Last Name")]
        public string lastname { get; set; }
        [Required(ErrorMessage = "Please fill 9-16 digit Mobile Number")]
        public string mobile { get; set; }
        [Required(ErrorMessage = "Please Fill Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string email { get; set; }
        [Required(ErrorMessage = "Please fill Password")]

        [RegularExpression("^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[()-_\\|;:'\",!~/@#$%^&+=]).*$", ErrorMessage = "Password must contains at least 8 char(one special character, one uppercase, one lowercase(in any order)).")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required(ErrorMessage = "Please Choose Image")]
        public IFormFile images { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }


    public class UpdateVendor
    {
        //public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string email { get; set; }
        public string vat_no { get; set; }
        public string business_name { get; set; }
        public string country { get; set; }
        public IFormFile images { get; set; }
        //public IFormFile Images { get; set; }

    }

    public class LoginVendorModel
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
        public string state { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public string business_name { get; set; }
        public string stripe_id { get; set; }
        public bool stripe_account { get; set; }
        public string vat_no { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }


    public class Vendor_Details
    {
        public int id { get; set; }
        public int? role { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public bool is_subscription { get; set; }
        public bool stripe_account { get; set; }
        public string vat_no { get; set; }
        public string business_name { get; set; }
        public string profile_pic { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string stripe_id { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public List<VendoDocuments> business_document { get; set; } = new List<VendoDocuments>() { };
    }


    public class stripekey
    {
        public string stripe_id { get; set; }
    }

    public class Vendor_stripe_Details
    {
        public int id { get; set; }
        public int? role { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public bool is_subscription { get; set; }
        public bool status { get; set; }
        public bool is_guest { get; set; }
        public int isVerified { get; set; }
        public int is_admin { get; set; }
        
        public string stripe_customer_id { get; set; }
        public string stripe_id { get; set; }
        
        public string profile_pic { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? email_verified_at { get; set; }
        public DateTime? updated_at { get; set; }
        
    }


    public class VendoDocuments
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public string images { get; set; }

    }

    public class VendorUserUpdateProfile
    {
        public IFormFile profile_pic { get; set; }
    }

}
