using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using EC.Data.Models;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class VendorViewModels
    {
        public int Id { get; set; }
        public int id { get; set; }
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Please Fill Business Registration Number")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string VatNo { get; set; }
        [Required(ErrorMessage = "Please Fill Business Name")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string BusinessName { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [DisplayName("First Name")]
        [Required(ErrorMessage = "Please Fill First Name")]
        [RegularExpression(@"^([a-zA-Z0-9]+$)", ErrorMessage = "Please enter a valid First Name")]
        public string Firstname { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "Please Fill Last Name")]
        [RegularExpression(@"^([a-zA-Z0-9]+$)", ErrorMessage = "Please enter a valid Last Name")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Please Fill 9-16 Digit Mobile Number")]
        [RegularExpression(@"^[0-9]\d{0,15}$", ErrorMessage = "Invalid mobile number")]
        [StringLength(16, MinimumLength = 9, ErrorMessage = "Mobile number must be between 9 and 16 digits")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Please Fill Email")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please fill Password")]

        [RegularExpression("^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[()-_\\|;:'\",!~/@#$%^&+=]).*$", ErrorMessage = "Password must contains at least 8 char(one special character, one uppercase, one lowercase(in any order)).")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please Fill Confirm Password")]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password does not Match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string ImageName { get; set; }
        [Required(ErrorMessage = "Please Choose Document.")]
        public List<IFormFile> Image { get; set; }
        public List<IFormFile> Images { get; set; }
        public string Name { get; set; }
        public List<VendorDocuments> vendorDocuments { get; set; } = new List<VendorDocuments>();
    }
    public class VewvendorModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Please fill reasons for decline")]
        public string Reasons { get; set; }
        public string VatNo { get; set; }
        public bool? Status { get; set; }
        public bool? Status1 { get; set; }
        public string ImageName { get; set; }
        public List<VendorDocuments> vendorDocuments { get; set; } = new List<VendorDocuments>();
    }
}
