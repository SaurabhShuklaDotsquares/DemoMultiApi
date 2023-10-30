using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class wishlistViewModels
    {
        [Required(ErrorMessage = "Product id required.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Product id should be greater than 0")]
        public int product_id { get; set; }
        [Required(ErrorMessage = "Status type required.")]
        public int Status_type { get; set; }
    }
    public class wishlist
    {
        public string product_name { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
        public decimal? Price { get; set; }
    }

    public class WhishListModel
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public int product_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Product product { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int stock { get; set; }
        public decimal price { get; set; }
        public string product_type { get; set; }
        public int vendor_id { get; set; }
        public string seller_id { get; set; }
        public int category_id { get; set; }
        public int brand_name { get; set; }
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
        public int moq { get; set; }
        public decimal? discounted_price { get; set; }
        public int flash_deal { get; set; }
        public int ready_to_ship { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public byte banner_flag { get; set; }
        public string banner_image { get; set; }
        public string banner_link { get; set; }
        public string video { get; set; }
        public string short_description { get; set; }
        public string long_description { get; set; }
        public string rating { get; set; }
        public bool is_featured { get; set; }
        public bool is_popular { get; set; }
        public int show_notes { get; set; }
        public int approval_status { get; set; }
        public byte is_change { get; set; }
        public bool? status { get; set; }
        public object currency { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public object url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
        public List<ProductImage> product_image { get; set; } = new List<ProductImage>();
    }
    public class ProductImage
    {
        //public int id { get; set; }
        //public int product_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
    }
}
