using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class UserAddressViewModels
    {
        public string AddressType { get; set; }
        [Required]
        public string address1 { get; set; }
        [Required]
        public string address2 { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string postal_code { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public string country_id { get; set; }
      
    }
    public class UserAddressViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "user_address_id should be greater than 0")]
        public int user_address_id { get; set; }
        [Required(ErrorMessage = "Address 1 Required.")]
        public string address1 { get; set; }
        [Required(ErrorMessage = "Address 2 Required.")]
        public string address2 { get; set; }
        [Required(ErrorMessage = "City Required.")]
        public string city { get; set; }
        [Required(ErrorMessage = "Postal Code Required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Postal Code should be greater than 0")]
        public int postal_code { get; set; }
        [Required(ErrorMessage = "State Required.")]
        public string state { get; set; }
        [Required(ErrorMessage = "Country Required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Country Id should be greater than 0")]
        public int country_id { get; set; }
        public string address_type { get; set; }
        public string name { get; set; }

    }
    public class UserAddress2
    {
        public List<UserAddressModel> data { get; set; } = new List<UserAddressModel>();
    }
    public class UserAddressModel
    {
        public int id { get; set; }
        //public int user_address_id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string name { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        public string mobile { get; set; }
        public int postal_code { get; set; }
        public string state { get; set; }
        public int country_id { get; set; }

    }

    public class UserAddress1
    {
        public int id { get; set; }
    }

    public class UsersAddressModel
    {
       
        public int user_id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string name { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        //public string mobile { get; set; }
        public string postal_code { get; set; }
        public string state { get; set; }
        public string country_id { get; set; }
        public int id { get; set; }
    }
    public class UserUpdateAddressViewModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        public string name { get; set; }
        public string postal_code { get; set; }
        public string state { get; set; }
        public string country_id { get; set; }

    }

}
