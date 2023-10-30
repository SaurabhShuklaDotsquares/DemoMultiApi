using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class CartViewModels
    {
        //public int Id { get; set; }
        // public int? UserId { get; set; }
        [Required(ErrorMessage ="Product id required.")]
        [Range(1,int.MaxValue, ErrorMessage = "Product id should be greater than 0")]
        public int product_id { get; set; }
        //public int? SellerId { get; set; }
        [Required(ErrorMessage = "Quantity required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity should be greater than 0")]
        public int quantity { get; set; }
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        public int? variant_id { get; set; }
        public string variant_slug { get; set; }
        //public decimal? FinalValue { get; set; }
    }
    public class cartdata
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public int? variant_id { get; set; }
        public string variant_slug { get; set; }
        public decimal? final_value { get; set; }
        public DateTime? created_at { get; set; }
        public string display_final_price { get; set; }
        public string display_final { get; set; }
        public List<CartImagedata> product_image { get; set; } = new List<CartImagedata>();
        public Productdata product { get; set; } = new Productdata();
        public List<ProductAttributeDetaildata> product_attribute_detail { get; set; } = new List<ProductAttributeDetaildata>();
        

    }
    

    public class CartViewModel
    {
        [Required(ErrorMessage = "Cart id required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Cart id should be greater than 0")]
        public int cart_id { get; set; }
        [Required(ErrorMessage = "Quantity id required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity should be greater than 0")]
        public int quantity { get; set; }
        [Required(ErrorMessage = "Product id required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product id should be greater than 0")]
        public int product_id { get; set; }
        public int variant_id { get; set; }
        //public int Id { get; set; }
        //public int? UserId { get; set; }
        //public int? SellerId { get; set; }
        ////public DateTime? CreatedAt { get; set; }
        ////public DateTime? UpdatedAt { get; set; }
        //public int? VariantId { get; set; }
        //public string VariantSlug { get; set; }
        //public decimal? FinalValue { get; set; }
    }
    public class cartcountdata
    {
        public int countdata { get; set; }
    }
    public class cartId_data
    {
        public int cart_id { get; set; }
    }

    public class CartImagedata
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
    }

    public class Productdata
    {
        public int id { get; set; }
        public object product_type { get; set; }
        public int vendor_id { get; set; }
        public object seller_id { get; set; }
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
        public int status { get; set; }
        public object currency { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
        public List<CartImagedata> product_image { get; set; } = new List<CartImagedata>();

    }

    public class ProductAttributeDetaildata
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string attribute_slug { get; set; }
        public string variant_text { get; set; }
        public decimal? regular_price { get; set; }
        public decimal? price { get; set; }
        public int? stock { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string display_price { get; set; }
        public string display_regular_price { get; set; }
    }

    public class Shippingdata
    {
        public string message { get; set; }
        public Ratedata rate { get; set; } = new Ratedata();
    }
    public class Ratedata
    {
        public decimal shipping_charges { get; set; }
    }

    public class ResponseCartdata
    {
        public List<cartdata> data { get; set; } = new List<cartdata>();
        public int quantity { get; set; }
        public int tax { get; set; }
        public Shippingdata shipping { get; set; } = new Shippingdata();
        public decimal sub_total { get; set; }
        public decimal tax_price { get; set; }
        public decimal? total_price { get; set; }
        public decimal shipping_charges { get; set; }
        public string display_shipping_charges { get; set; }
        public string display_sub_total { get; set; }
        public decimal display_tax_price { get; set; }
        public string display_total_price { get; set; }
    }

    public class CartResponseModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int product_id { get; set; }
        public int? seller_id { get; set; }
        public int quantity { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int? variant_id { get; set; }
        public string variant_slug { get; set; }
        public decimal? final_value { get; set; }
        public decimal tax_amount { get; set; }
    }

}
