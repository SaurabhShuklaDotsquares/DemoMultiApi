using System;
using System.Collections.Generic;

namespace EC.API.ViewModels
{
    public class HomeViewModels
    {
      public List<ProductModels> new_arrival { get; set; } = new List<ProductModels>();
      public List<BannerModels> banners { get; set; } = new List<BannerModels>();
      public List<CategoriesModels> featured_category { get; set; } = new List<CategoriesModels>();
      public List<CategoryModels> category { get; set; } = new List<CategoryModels>();
    }

    public class ProductModels
    {
        public int id { get; set; }
        public string title { get; set; }
        public string category_id { get; set; }
        public bool? Status { get; set; }
        public decimal? price { get; set; }
        public string slug { get; set; }
        public string prod_description { get; set; }
        public decimal average_rating { get; set; }
        //public bool IsFeatured { get; set; }
        //public bool IsPopular { get; set; }
        public decimal? discounted_price { get; set; }
        public string display_discounted_price { get; set; }
        public string display_price { get; set; }
        //public string ProductType { get; set; }
        public List<ProductImagesModel> product_image { get; set; } = new List<ProductImagesModel>();
        
    }
    public class ProductImagesModel
    {
        public int id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
        public int product_id { get; set; }
    }

    public class BannerModels
    {
        public int id { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public string image { get; set; }
    }

    public class FeaturedProductModels
    {
        public int id { get; set; }
        public string product_type { get; set; }
        public int vendor_id { get; set; }
        //public object seller_id { get; set; }
        public int category_id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public int brand_name { get; set; }
        public string sku { get; set; }
        //public object type { get; set; }
        //public object tax_class { get; set; }
        //public object reference { get; set; }
        //public object features { get; set; }
        //public object warranty_details { get; set; }
        //public object customs_commodity_code { get; set; }
        public int country_of_manufacture { get; set; }
        //public object country_of_shipment { get; set; }
        //public object barcode_type { get; set; }
        //public object barcode { get; set; }
        public int stock { get; set; }
        public int moq { get; set; }
        public string price { get; set; }
        public string discounted_price { get; set; }
       // public object discount_type { get; set; }
        public int flash_deal { get; set; }
        public int ready_to_ship { get; set; }
        //public object meta_title { get; set; }
        //public object meta_keyword { get; set; }
        //public object meta_description { get; set; }
        public int banner_flag { get; set; }
        //public object banner_image { get; set; }
       //public object banner_link { get; set; }
        //public object video { get; set; }
        //public object short_description { get; set; }
        public string long_description { get; set; }
        public object rating { get; set; }
        public int is_featured { get; set; }
        public int is_popular { get; set; }
        public int show_notes { get; set; }
        public int approval_status { get; set; }
        public string is_change { get; set; }
        public int status { get; set; }
        //public object currency { get; set; }
        //public DateTime created_at { get; set; }
        //public DateTime updated_at { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        //public decimal display_price { get; set; }
        public string display_discounted_price { get; set; }
        public List<ProductImagesModel> product_image { get; set; } = new List<ProductImagesModel>();
    }

    public class CategoriesModels
    {
        public int id { get; set; }
        //public object parent_id { get; set; }
        //public object seller_id { get; set; }
        public int is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        //public object banner { get; set; }
        //public object meta_title { get; set; }
        //public object meta_keyword { get; set; }
        //public object meta_description { get; set; }
        public int status { get; set; }
        public int approval_status { get; set; }
        public int lft { get; set; }
        public int rgt { get; set; }
        //public object depth { get; set; }
        //public string created_at { get; set; }
        //public string updated_at { get; set; }
        public string image_link { get; set; }

        public List<FeaturedProductModels> featured_products { get; set; } = new List<FeaturedProductModels>();   
    }

    public class CategoryModels
    {
        public int id { get; set; }
        public object parent_id { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public string image_link { get; set; }

        public List<CategoryModels> childs { get; set; } = new List<CategoryModels>();
    }
}
