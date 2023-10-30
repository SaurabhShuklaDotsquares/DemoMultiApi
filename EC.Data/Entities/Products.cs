using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class Products
    {
        public Products()
        {
            OrderItems = new HashSet<OrderItems>();
            ProductAttributeDetails = new HashSet<ProductAttributeDetails>();
            ProductAttributes = new HashSet<ProductAttributes>();
            ProductImages = new HashSet<ProductImages>();
            Reviews = new HashSet<Reviews>();
        }

        public int Id { get; set; }
        public int? SellerId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int? BrandName { get; set; }
        public string Sku { get; set; }
        public string Type { get; set; }
        public string TaxClass { get; set; }
        public string Reference { get; set; }
        public string Features { get; set; }
        public string WarrantyDetails { get; set; }
        public string CustomsCommodityCode { get; set; }
        public int? CountryOfManufacture { get; set; }
        public string CountryOfShipment { get; set; }
        public string BarcodeType { get; set; }
        public string Barcode { get; set; }
        public int Stock { get; set; }
        public int Moq { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string DiscountType { get; set; }
        public int FlashDeal { get; set; }
        public int ReadyToShip { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public byte BannerFlag { get; set; }
        public string BannerImage { get; set; }
        public string BannerLink { get; set; }
        public string Video { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Rating { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPopular { get; set; }
        public int ShowNotes { get; set; }
        public int ApprovalStatus { get; set; }
        public byte IsChange { get; set; }
        public bool? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public int? StockClose { get; set; }
        public string ProductType { get; set; }

        public virtual Categories Category { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
        public virtual ICollection<ProductAttributeDetails> ProductAttributeDetails { get; set; }
        public virtual ICollection<ProductAttributes> ProductAttributes { get; set; }
        public virtual ICollection<ProductImages> ProductImages { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
