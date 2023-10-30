//using EC.Data.Models;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class Users
    {
        public Users()
        {
            //Carts = new HashSet<Carts>();
            Orders = new HashSet<Orders>();
            Payment = new HashSet<Payment>();
            Reviews = new HashSet<Reviews>();
            RoleUser = new HashSet<RoleUser>();
            UserAddress = new HashSet<UserAddress>();
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
        //public virtual ICollection<Carts> Carts { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Payment> Payment { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
        public virtual ICollection<RoleUser> RoleUser { get; set; }
        public virtual ICollection<UserAddress> UserAddress { get; set; }
    }
}
