using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using EC.Data.Models;

namespace EC.API.ViewModels
{
    public class ProductViewModel
    {
        //public ProductsViewModel()
        //{
        //    TitleList = new List<SelectListItem>();
        //    CountriesList = new List<SelectListItem>();
        //    AttributeList = new List<SelectListItem>();
        //    OptionValuesList = new List<List<SelectListItem>>();
        //    BrandNameList = new List<SelectListItem>();
        //    ProductTypeList = new List<SelectListItem>();
        //}
        public int Id { get; set; }
        public int? SellerId { get; set; }
        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter product name")]
        public string Title { get; set; }
        public string Category { get; set; }
        public string ApprovalStatus { get; set; }
        public bool? Status { get; set; }
        public int Stock { get; set; }
        public int? Stocks { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "Please select brand")]
        public int? BrandName { get; set; }
        public string BrandNames { get; set; }
        [Required(ErrorMessage = "Please enter sku")]
        public string Sku { get; set; }
        [Required(ErrorMessage = "Please select country")]
        public int CountryOfManufacture { get; set; }
        [Required(ErrorMessage = "Please enter price")]
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        [Required(ErrorMessage = "Please enter description")]
        public string LongDescription { get; set; }
        public string Rating { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPopular { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required(ErrorMessage = "Please select product type")]
        public string ProductType { get; set; }
        [Required(ErrorMessage = "Please enter AvailableStock")]
        public int AvailableStock { get; set; }
        [Required(ErrorMessage = "Please enter ClosingStock")]
        public int StockClose { get; set; }
        public List<IFormFile> MyImage { get; set; }
        public List<SelectListItem> TitleList { get; set; }
        public List<SelectListItem> CountriesList { get; set; }
        public List<SelectListItem> AttributeList { get; set; }
        public List<List<SelectListItem>> OptionValuesList { get; set; }
        public List<SelectListItem> BrandNameList { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
        public string AdditionalRowsJSON { get; set; }
        public List<string> hdnAttributeValuseDetails { get; set; }
        public string hdnMain_RemoveImage { get; set; }
        public string hdnAttribute_RemoveImage { get; set; }
    }
    public class ProductModel1
    {
        public List<ProductModel> data { get; set; } = new List<ProductModel>();
        public List<Breadcrumbs> breadcrumbs { get; set; } = new List<Breadcrumbs>();
        public Range rang { get; set; } = new Range();
    }

    public class ProductModel
    {
        public int id { get; set; }
        public string category_id { get; set; }
        public string brand_name { get; set; }
        public string title { get; set; }
       
        //public string ApprovalStatus { get; set; }
        //public bool? Status { get; set; }
        public decimal? price { get; set; }
        public string slug { get; set; }
        public decimal? discounted_price { get; set; }
        public int inWishlist { get; set; }
        public string prod_description { get; set; }
        public decimal avrage_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
        //public bool IsFeatured { get; set; }
        //public bool IsPopular { get; set; }

        //public string ProductType { get; set; }
        //public int AvailableStock { get; set; }
        //public int StockClose { get; set; }
        public List<product_Images> product_image { get; set; } = new List<product_Images>();
        //public List<ProductModel> data { get; set; } = new List<ProductModel>();
        List<ProductModel> ProductList = new List<ProductModel>();
        public Brand_data brand_data { get; set; } = new Brand_data();
        //public List<Breadcrumbs> breadcrumbs { get; set; } = new List<Breadcrumbs>();
        //public Range rang { get; set; } = new Range();

    }



    public class ProductResponseModel
    {
        public string title { get; set; }
        public string brand_name { get; set; }
        public string category_id { get; set; }
        //public string ApprovalStatus { get; set; }
        //public bool? Status { get; set; }
        public decimal? price { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
        public string prod_description { get; set; }
        public string avrage_rating { get; set; }
        //public bool IsFeatured { get; set; }
        //public bool IsPopular { get; set; }
        public decimal? discounted_price { get; set; }
        //public string ProductType { get; set; }
        //public int AvailableStock { get; set; }
        //public int StockClose { get; set; }
        public List<product_Images> productImages { get; set; } = new List<product_Images>();
        public List<ProductModel> data { get; set; } = new List<ProductModel>();
        List<ProductModel> ProductList = new List<ProductModel>();
        public List<Brand_data> brand_data { get; set; } = new List<Brand_data>();
        public List<Category> ProductCategory { get; set; } = new List<Category>();
        public List<Product_Attribute_Image> ProductAttributeImage { get; set; } = new List<Product_Attribute_Image>();
        public List<Breadcrumbs> breadcrumbs { get; set; } = new List<Breadcrumbs>();
    }



    public class product_Images
    {
        public int id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
        public int product_id { get; set; }
    }
    public class product_list
    {
        public decimal? max_price { get; set; }
        public decimal min_price { get; set; }
        public string sort_by { get; set; }
    }
    public class Brand_data
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
    public class Brand_dataModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int is_featured { get; set; }
        public string approval_status { get; set; }
        public string image { get; set; }
        public int? status { get; set; }
        public string image_link { get; set; }

    }
    public class Product_Details
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
        //public int flash_deal { get; set; }
        //public int ready_to_ship { get; set; }
        //public int banner_flag { get; set; }
        public string long_description { get; set; }
        public int is_featured { get; set; }
        public int is_popular { get; set; }
        public int show_notes { get; set; }
        public int approval_status { get; set; }
        //public int is_change { get; set; }
        public int status { get; set; }
        //public string business_name { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }

        public List<product_Images> product_image { get; set; } = new List<product_Images>();
        public Brand_dataModel brand_data { get; set; }=new Brand_dataModel();
        public BusinessModel business_names { get; set; }=new BusinessModel();
        public List<productAttribute> product_attribute { get; set; } = new List<productAttribute>();
        public List<ProductAttributeDetail> product_attribute_detail { get; set; } = new List<ProductAttributeDetail>();
        public List<ReviewModel> reviews { get; set; } = new List<ReviewModel>();
        public List<Breadcrumbs> breadcrumbs { get; set; } = new List<Breadcrumbs>();
        public List<ProductsLike> productsLike { get; set; } = new List<ProductsLike>();
        public bool isLiked { get; set; }
        public List<Category> categories { get; set; } = new List<Category>();

    }

    public class BusinessModel
    {
        public string business_name { get; set; }
        public int user_id { get; set; }
    }

    public class Option
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }

    }
    public class OptionModel
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public List<OptionValueModel> option_values { get; set; } = new List<OptionValueModel>();

    }

    public class OptionValue
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
    public class OptionValueModel
    {
        public int id { get; set; }
        public int option_id { get; set; }
        public string title { get; set; }
        public string hexcode { get; set; }
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
    public class CategoryList
    {
        public List<Category> categoryProps { get; set; } = new List<Category>();
    }
    public class ProductAttributeDetail
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string attribute_slug { get; set; }
        public string variant_text { get; set; }
        public decimal? regular_price { get; set; }
        public decimal price { get; set; }
        public int? stock { get; set; }
        public decimal? display_price { get; set; }
        public decimal? display_regular_price { get; set; }
        public List<Product_Attribute_Image> images { get; set; } = new List<Product_Attribute_Image>();
        //public List<Breadcrumbs> breadcrumbs { get; set; } = new List<Breadcrumbs>();
        //public List<ProductsLike> productsLike { get; set; } = new List<ProductsLike>();
        ////public List<Categorie> categoriesapi { get; set; } = new List<Categorie>();
        //public List<Product_Attribute_Image> ProductAttribute_Image { get; set; } = new List<Product_Attribute_Image>();


    }
    public class Product_Attribute_Image
    {
        public int Id { get; set; }
        public int ProductAttributeDetailId { get; set; }
        public string Image { get; set; }
        public string Image_link { get; set; }

    }
    public class Breadcrumbs
    {
        public string title { get; set; }
        public string slug { get; set; }
        public string type { get; set; }
    }
    public class ProductsLike
    {
        public int id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int? brand_name { get; set; }
        public string sku { get; set; }
        public int stock { get; set; }
        public decimal? price { get; set; }
        public string long_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public decimal? display_price { get; set; }
        public decimal? display_discounted_price { get; set; }
        public List<product_Images> product_image { get; set; } = new List<product_Images>();

    }
    public class Category
    {
        public int id { get; set; }
        public bool is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public bool status { get; set; }
        public byte approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public string image_link { get; set; }
        public List<CategoryChildren> children { get; set; } = new List<CategoryChildren>();
        //public int? ParentId { get; set; }
        //public int? SellerId { get; set; }
        //public string Banner { get; set; }
        //public string MetaTitle { get; set; }
        //public string MetaKeyword { get; set; }
        //public string MetaDescription { get; set; }
        //public int? Depth { get; set; }
        //public DateTime? CreatedAt { get; set; }
        //public DateTime? UpdatedAt { get; set; }

    }
    public class CategoryChildren
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public bool is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public bool status { get; set; }
        public byte approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public string image_link { get; set; }

    }
    public class productAttribute
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int attribute_id { get; set; }
        public string attribute_values { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        //public Option attribute_detail { get; set; } = new Option();
        //public List<OptionValue> attribute_values_detail { get; set; } = new List<OptionValue>();
        public OptionModel option { get; set; } = new OptionModel();
    }
    public class slugdata
    {
        [Required (ErrorMessage ="Required Slug.")]
        public string slug { get; set; }
    }
    public class Range
    {
        public decimal max { get; set; }
        public decimal min { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
    }
    public class productlist
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
   

    public class PRoductPrice
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string attribute_slug { get; set; }
        public string variant_text { get; set; }
        public decimal? regular_price { get; set; }
        public decimal? price { get; set; }
        public int? stock { get; set; }
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        public string discounted_show { get; set; }
        public string regular_show { get; set; }
        public string display_price { get; set; }
        public string display_regular_price { get; set; }
        public List<ProductPriceAttributeImage> attribute_image { get; set; } = new List<ProductPriceAttributeImage>();
    }
    public class ProductPriceAttributeImage
    {
        public int id { get; set; }
        public int product_attribute_detail_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }

    }
    public class DataModel
    {
       public Product_Details data { get; set; }
    }

    public class ProductPriceRequest
    {
        [Required(ErrorMessage = "Product id required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product id should be greater than 0")]
        public int product_id { get; set; }
        [Required(ErrorMessage = "Attribute required.")]
        public string[] attribute { get; set; }
    }
}
