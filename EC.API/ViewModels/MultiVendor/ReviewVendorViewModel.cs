using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels.MultiVendor
{
    public class ReviewVendorViewModel
    {
    }

    public class ReviewViewModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public string status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public UserViewModel user { get; set; } = new UserViewModel();
        public ProductViewModel product { get; set; } = new ProductViewModel();
    }
    public class UserViewModel
    {
        public int id { get; set; }
        public int role { get; set; }
        public string firstname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string lastname { get; set; }
        public string profile_pic { get; set; }
        public string state { get; set; }
        public DateTime? email_verified_at { get; set; }
        public int? isVerified { get; set; }
        public int? is_admin { get; set; }
        public string stripe_customer_id { get; set; }
        public string stripe_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string country { get; set; }
        public int status { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public bool is_guest { get; set; }
    }

    public class ProductViewModel
    {
        public int id { get; set; }
        public string product_type { get; set; }
        public int? vendor_id { get; set; }
        public int? seller_id { get; set; }
        public int category_id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int? brand_name { get; set; }
        public string sku { get; set; }
        public string type { get; set; }
        public string tax_class { get; set; }
        public string reference { get; set; }
        public string features { get; set; }
        public string warranty_details { get; set; }
        public string customs_commodity_code { get; set; }
        public int? country_of_manufacture { get; set; }
        public string country_of_shipment { get; set; }
        public string barcode_type { get; set; }
        public string barcode { get; set; }
        public int stock { get; set; }
        public int moq { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string discount_type { get; set; }
        public int flash_deal { get; set; }
        public int ready_to_ship { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public int banner_flag { get; set; }
        public string banner_image { get; set; }
        public string banner_link { get; set; }
        public string video { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }
        public string rating { get; set; }
        public int is_featured { get; set; }
        public int is_popular { get; set; }
        public int show_notes { get; set; }
        public int approval_status { get; set; }
        public int is_change { get; set; }
        public int? status { get; set; }
        public string currency { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }

        public List<ProductImageViewModel> product_image { get; set; } = new List<ProductImageViewModel>();
    }

    public class ProductImageViewModel
    {
        public int id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
        public int product_id { get; set; }
    }
    public class UserReviewViewModel
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public bool is_guest { get; set; }
    }
    public class ReviewStatusUpdateRequestModel
    {
        [Required(ErrorMessage = "Order id required")]
        public int id { get; set; }
        [Required(ErrorMessage = "Order status required")]
        public int status { get; set; }
        public string comment { get; set; }
    }

    public class ReturnReviewListModel
    {
        public List<ReviewViewModel> data { get; set; } = new List<ReviewViewModel>();
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
    }

    public class ReturnReviewViewModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public string status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public UserReviewViewModel user { get; set; } = new UserReviewViewModel();
        public ProductViewModel product { get; set; } = new ProductViewModel();
    }

}
