using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Users
    {
        public Users()
        {
            Carts = new HashSet<Carts>();
            Orders = new HashSet<Orders>();
            Payment = new HashSet<Payment>();
            ReturnRequests = new HashSet<ReturnRequests>();
            Reviews = new HashSet<Reviews>();
            RoleUser = new HashSet<RoleUser>();
            UserAddress = new HashSet<UserAddress>();
            VendorDetails = new HashSet<VendorDetails>();
            VendorDocuments = new HashSet<VendorDocuments>();
            Wishlists = new HashSet<Wishlists>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Lastname { get; set; }
        public string ProfilePic { get; set; }
        public string State { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public byte? IsAdmin { get; set; }
        public string Password { get; set; }
        public string RememberToken { get; set; }
        public string StripeCustomerId { get; set; }
        public string StripeId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string SaltKey { get; set; }
        public bool IsActive { get; set; }
        public byte Gender { get; set; }
        public string ForgotPasswordLink { get; set; }
        public DateTime? ForgotPasswordLinkExpired { get; set; }
        public bool? ForgotPasswordLinkUsed { get; set; }
        public bool? IsVerified { get; set; }
        public string Verifylink { get; set; }

        public virtual ICollection<Carts> Carts { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Payment> Payment { get; set; }
        public virtual ICollection<ReturnRequests> ReturnRequests { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
        public virtual ICollection<RoleUser> RoleUser { get; set; }
        public virtual ICollection<UserAddress> UserAddress { get; set; }
        public virtual ICollection<VendorDetails> VendorDetails { get; set; }
        public virtual ICollection<VendorDocuments> VendorDocuments { get; set; }
        public virtual ICollection<Wishlists> Wishlists { get; set; }
    }
}
