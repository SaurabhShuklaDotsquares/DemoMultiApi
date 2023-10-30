using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels.MultiVendor
{
    public class ProductVendorViewModels
    {
        public Dictionary<string, string> brand { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> countries { get; set; } = new Dictionary<string, string>();
        public List<option_Atribute> attributes { get; set; } = new List<option_Atribute>();
        public List<Category_Multivendor> category { get; set; } = new List<Category_Multivendor>();
    }

    public class option_Atribute
    {
        public int id { get; set; }
        public int? seller_id { get; set; }
        public string header_type { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int sort_order { get; set; }
        public bool status { get; set; }
        public bool? deletable { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }

        public List<Option_valuse> option_values_show { get; set; } = new List<Option_valuse>();
        //public List<Option_valuse> category { get; set; } = new List<Option_valuse>();

    }
    public class Option_valuse
    {
        public int id { get; set; }
        public int option_id { get; set; }
        public string title { get; set; }
        public string hexcode { get; set; }
        public int sort_order { get; set; }
        public string image { get; set; }
        public bool status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public class Category_Multivendor
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public int? seller_id { get; set; }
        public int is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public string banner { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public bool status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public int? depth { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string image_link { get; set; }
        public List<Category_Multivendor_Children> children { get; set; } = new List<Category_Multivendor_Children>();

    }
    public class Category_Multivendor_Children
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public int? seller_id { get; set; }
        public int is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public string banner { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public bool status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public int? depth { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string image_link { get; set; }

    }

    public class Product_Vendor_Model
    {
        public List<Product_vendor_attribute> attribute { get; set; } = new List<Product_vendor_attribute>();
        public  Dictionary<string, string[]> attribute_options { get; set; }
        public int category_id { get; set; }
        public int? brand_name { get; set; }
        public int stock { get; set; }
        [Required, MinLength(3), DataType(DataType.Text), MaxLength(50), Display(Name = "SKU")]
        public string sku { get; set; }
        public string title { get; set; }
        public int country_of_manufacture { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string long_description { get; set; }
        public int is_featured { get; set; }
        public string show_notes { get; set; }
        public int? status { get; set; }
        public int? approval_status { get; set; }
        public IFormFile images { get; set; }
    }
    public class UpdateproductStatus
    {
        public int id { get; set; }
        public string approval_status { get; set; }
    }



    public class Product_Atributevendor_attribute
    {
        public int[] attribute_id { get; set; }
        public string[] attribute_values { get; set; }
    }

    public class Product_vendor_attribute
    {
        public decimal[] regular_price { get; set; }
        public decimal[] price { get; set; }
        public int[] stock { get; set; }
        public string[] id { get; set; }
        public IFormFile images { get; set; }
        public string[] attribute { get; set; }
        public string[] product_attribute_id { get; set; }
    }

    public class ProductVendor_Models
    {
        public int id { get; set; }
        public int category_id { get; set; }
        public int? brand_name { get; set; }
        public int stock { get; set; }
        public string sku { get; set; }
        public string title { get; set; }
        public int country_of_manufacture { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string long_description { get; set; }
        public bool is_featured { get; set; }
        public string show_notes { get; set; }
        public bool? status { get; set; }
        public IFormFile images { get; set; }
        public Dictionary<string, string[]> attribute_options { get; set; }
        public List<Product_attribute> attribute { get; set; } = new List<Product_attribute>();
    }
    public class Product_attribute
    {
        public decimal[] regular_price { get; set; }
        public decimal[] price { get; set; }
        public int[] stock { get; set; }
        public int[] id { get; set; }
        public List<IFormFile>[] images { get; set; }
        public string[] attribute { get; set; }

    }

    public class ProductUpdateVendor_Models
    {
        //public int id { get; set; }
        public int category_id { get; set; }
        public int? brand_name { get; set; }
        public int stock { get; set; }
        public string sku { get; set; }
        public string title { get; set; }
        public int country_of_manufacture { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string long_description { get; set; }
        public bool is_featured { get; set; }
        public string show_notes { get; set; }
        public bool? status { get; set; }
        public IFormFile images { get; set; }
        public List<ProductUpdate_attribute> attribute { get; set; } = new List<ProductUpdate_attribute>();
        public List<ProductUpdate_attribute> attribute_options { get; set; } = new List<ProductUpdate_attribute>();
    }
    public class ProductUpdate_attribute
    {
        public decimal? regular_price { get; set; }
        public decimal? price { get; set; }
        public int? stock { get; set; }
        public int id { get; set; }
        public string attribute { get; set; }
        public string product_attribute_id { get; set; }
    }



    #region Multivendor Product Api Models
    public class productRequestModel
    {
        public string category_slug { get; set; }
        //public  string sort_by { get; set; }
        public decimal max_price { get; set; }
        public decimal min_price { get; set; }
        public string search { get; set; }
        public int page { get; set; } = 1;
        private int _pageSize = 10;
        const int MAX_PAGE_SIZE = 50;
        public int PageSize
        {
            get { return _pageSize; }

            set { _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value; }
        }
    }
   
    public class ProductListModel
    {
        public int id { get; set; }
        public string category_id { get; set; }
        public string brand_name { get; set; }
        public string title { get; set; }
        public decimal? price { get; set; }
        public string slug { get; set; }
        public decimal? discounted_price { get; set; }
        public int inWishlist { get; set; }
        public string prod_description { get; set; }
        public decimal avrage_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
        public List<ProductImageModel> product_image { get; set; } = new List<ProductImageModel>();
        List<ProductModel> ProductList = new List<ProductModel>();
        public BrandDataModel brand_data { get; set; } = new BrandDataModel();
    }
    public class RangeModel
    {
        public decimal max { get; set; }
        public decimal min { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
    }
    public class BreadCrumbsModel
    {
        public string title { get; set; }
        public string slug { get; set; }
        public string type { get; set; }
    }
    public class ProductImageModel
    {
        public int id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
        public int product_id { get; set; }
    }
    public class BrandDataModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int is_featured { get; set; }
        public string approval_status { get; set; }
        public string image { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int? status { get; set; }
        public string image_link { get; set; }

    }
    public class ProductModel
    {
        public int id { get; set; }
        public string product_type { get; set; }
        public int vendor_id { get; set; }
        public int category_id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int? brand_name { get; set; }
        public string sku { get; set; }
        public int? country_of_manufacture { get; set; }
        public int stock { get; set; }
        public int moq { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string long_description { get; set; }
        public int is_featured { get; set; }
        public int is_popular { get; set; }
        public int show_notes { get; set; }
        public int approval_status { get; set; }
        public int status { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
        public List<ProductImageModel> product_image { get; set; } = new List<ProductImageModel>();
        public BrandDataModel brand_data { get; set; }
        public List<ProductAttributeModel> product_attribute { get; set; } = new List<ProductAttributeModel>();
        public List<ProductAttributeDetailModel> product_attribute_detail { get; set; } = new List<ProductAttributeDetailModel>();
    }
    public class ProductAttributeModel
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int attribute_id { get; set; }
        public string attribute_values { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public AttributeDetailModel attribute_detail { get; set; } = new AttributeDetailModel();
        public List<OptionsValueModel> attribute_values_detail { get; set; } = new List<OptionsValueModel>();
        public OptionsModel option { get; set; } = new OptionsModel();
    }
    public class OptionModel
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public List<OptionValueModel> option_values { get; set; } = new List<OptionValueModel>();

    }
    public class OptionValueModel
    {
        public int id { get; set; }
        public int option_id { get; set; }
        public string title { get; set; }
        public string hexcode { get; set; }
        public int sort_order { get; set; }
        public object image { get; set; }
        public int status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
    public class ProductAttributeDetailModel
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string attribute_slug { get; set; }
        public string variant_text { get; set; }
        public decimal? regular_price { get; set; }
        public decimal? price { get; set; }
        public int? stock { get; set; }
        public decimal? display_price { get; set; }
        public decimal? display_regular_price { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<ProductAttributeImageModel> attribute_image { get; set; } = new List<ProductAttributeImageModel>();
    }
    public class ProductAttributeImageModel
    {
        public int id { get; set; }
        public int product_attribute_detail_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }

    }
    public class ReviewModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public class ProductCategoryModel
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public int? seller_id { get; set; }
        public int is_featured { get; set; }
        public decimal? admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public string banner { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public int status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public int? depth { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string image_link { get; set; }
    }
    public class OptionsModel
    {
        public int id { get; set; }
        public int? seller_id { get; set; }
        public string header_type { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int sort_order { get; set; }
        public int status { get; set; }
        public int? deletable { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public List<OptionsValueModel> option_values_show { get; set; } = new List<OptionsValueModel>();
    }

    public class OptionsValueModel
    {
        public int id { get; set; }
        public int option_id { get; set; }
        public string title { get; set; }
        public string hexcode { get; set; }
        public int sort_order { get; set; }
        public string image { get; set; }
        public int status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class AttributeDetailModel
    {
        public int id { get; set; }
        public int? seller_id { get; set; }
        public string header_type { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int sort_order { get; set; }
        public int status { get; set; }
        public int? deletable { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
    public class ProductViewReturnModel
    {
        public ProductModel product { get; set; } = new ProductModel();
        public ProductCategoryModel parentCategories { get; set; } = new ProductCategoryModel();
        public Dictionary<string, string> countries { get; set; } = new Dictionary<string, string>();
        public List<OptionsModel> options { get; set; } = new List<OptionsModel>();
    }

    public class ProductStoreReturnModel
    {
        public int category_id { get; set; }
        public int? brand_name { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public int stock { get; set; }
        public string sku { get; set; }
        public int? country_of_manufacture { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string long_description { get; set; }
        public int is_featured { get; set; }
        public int show_notes { get; set; }
        public int is_popular { get; set; }
        public int approval_status { get; set; }
        public string slug { get; set; }
        public int? vendor_id { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? created_at { get; set; }
        public int id { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
    }

    public class ProductImageStoreReturnModel
    {
        public int product_id { get; set; }
        public string image_name { get; set; }
    }

    public class ProductAttributeDetailUpdateReturnModel
    {
        public decimal regular_price { get; set; }
        public decimal price { get; set; }
        public int stock { get; set; }
        public string id { get; set; }
        public string attribute { get; set; }
        public int product_attribute_id { get; set; }
    }

    public class ProductUpdateReturnModel
    {
        public int category_id { get; set; }
        public int? brand_name { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public int stock { get; set; }
        public string sku { get; set; }
        public int? country_of_manufacture { get; set; }
        public decimal? price { get; set; }
        public decimal? discounted_price { get; set; }
        public string long_description { get; set; }
        public int is_featured { get; set; }
        public int show_notes { get; set; }
        public int? vendor_id { get; set; }
        public int is_popular { get; set; }
        public int approval_status { get; set; }
        public string slug { get; set; }
    }

    public class ProductImageReturnModel
    {

    }

    public class ProductVendorModel
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
        public string status { get; set; }
        public object currency { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
        public CategoryVendorModel category_data { get; set; } = new CategoryVendorModel();
    }
    public class CategoryVendorModel
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public int? seller_id { get; set; }
        public int is_featured { get; set; }
        public decimal? admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public string banner { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public string status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public int? depth { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string image_link { get; set; }
        public string parent_category { get; set; }
    }
    public class ProductListReturnModel
    {
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
        public List<ProductVendorModel> data { get; set; } = new List<ProductVendorModel>();



    }

    #endregion
}
