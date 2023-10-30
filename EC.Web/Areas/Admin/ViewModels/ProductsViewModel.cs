using EC.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class ProductsViewModel : ProductAttributesViewModel
    {
        public ProductsViewModel()
        {
            TitleList = new List<SelectListItem>();
            CountriesList = new List<SelectListItem>();
            AttributeList = new List<SelectListItem>();
            OptionValuesList = new List<List<SelectListItem>>();
            BrandNameList = new List<SelectListItem>();
            ProductTypeList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public int? SellerId { get; set; }
        [Required(ErrorMessage = "Please select category")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter product name")]
        //[MaxLength(10)]
        public string Title { get; set; }
        public string Category { get; set; }
        public string ParentCategory { get; set; }
        public string ApprovalStatus { get; set; }
        public bool? Status { get; set; }
        
        public int Stock { get; set; }
        public int ? Stocks { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "Please select brand")]
        public int  BrandName { get; set; }
        public string BrandNames { get; set; }
        [Required(ErrorMessage = "Please enter sku")]
        public string Sku { get; set; }
        [Required(ErrorMessage = "Please select country")]
        public int CountryOfManufacture { get; set; }
        [Required(ErrorMessage = "Please enter price")]
        
        public decimal? Price { get; set; }
       
        public decimal? DiscountedPrice { get; set; }
        //public decimal? Discounted_Price { get; set; }
        [Required(ErrorMessage = "Please enter description")]
        //[RegularExpression(@"^([a-zA-Z]+$)", ErrorMessage = "Please enter a valid description")]
        public string LongDescription { get; set; }
        public string Rating { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsPopular { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required(ErrorMessage ="Please select product type")]
        public string ProductType { get; set; }
        [Required(ErrorMessage = "Please enter AvailableStock")]
        public int AvailableStock { get; set; }
        [Required(ErrorMessage = "Please enter ClosingStock")]
        public int StockClose { get; set; }
        //[Required(ErrorMessage = "Please Upload Image")]
        public List<IFormFile> MyImage { get; set; }


        public List<SelectListItem> TitleList { get; set; }
        public List<SelectListItem> CountriesList { get; set; }
        public List<SelectListItem> AttributeList { get; set; }
        public List<List<SelectListItem>> OptionValuesList { get; set; }
        public List<SelectListItem> BrandNameList { get; set; }
        public List<SelectListItem> ProductTypeList { get; set; }
        public List<ProductsViewModel> list { get; set; }
        // public List<ProductAttributeDetails> RegularPriceList { get; set; }
        public string AdditionalRowsJSON { get; set; }
        public TempList[] AdditionalRows { get; set; }
        public List<TempList> AdditionalRows_New { get; set; }
        public List<AttributeValuse> AttributeValuse { get; set; }
        public List<AttributeValuseDetails> AttributeValuseDetails { get; set; }
       
        public List<string> hdnAttributeValuseDetails { get; set; }

        public string hdnMain_RemoveImage { get; set; }
        //public string hdnMain_RemoveImage { get; set; }
        public string hdnAttribute_RemoveImage { get; set; }
    }
    public class AttributeValuseImage
    {
        public int Id { get; set; }
        public int ProductAttributeDetailId { get; set; }
        public string ImageName { get; set; }
    }
    public class AttributeValuseDetails
    {
        public int varient_id { get; set; }

        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please Enter Price")]
        public decimal? Price { get; set; }
        public decimal ? RegularPrice { get; set; }
         public int? Stock { get; set; }
        public string VarientText { get; set; }
    }

    public class ProdutsMainImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Image_Main { get; set; }
    }
    public class TempTest
    {
        public TempList[] AdditionalRows { get; set; }

        public ProductsViewModel ProductsViewModel { get; set; }
    }

    public class ProductAttributesViewModel
    {
        public int ProductAttributeId { get; set; }
        public int ProductId { get; set; }
        //public int AttributeId { get; set; }
        public List<int> AttributeIds { get; set; }
        public string AttributeValues { get; set; }
        public string AttributeImages { get; set; }
        public List<IFormFile> hdnMultipleImage { get; set; }
        public string DocumentPath { get; set; }
        public decimal? RegularPrice { get; set; }
        public List<string> RegularPriceList { get; set; }
        public List<string> DiscountPriceList { get; set; }
        public List<string> Discounted_Price { get; set; }
        public List<ProductAttributeDetails> ProductAttributeDetails_list { get; set; }
        public List<ProdutsMainImage> ProductMainDetails_list { get; set; }
        public List<string> StockPriceList { get; set; }
        public List<string> AttributeValue { get; set; }
        public List<AttributeValuseImage> AttributeViewImage { get; set; }
        public List<AttributeValuseImage> EditAttributeImage { get; set; }
        public List<ProdutsMainImage> ProdutsMain_Image { get; set; }
        //public List<AttributeValuseImage> EditAttributeImage { get; set; }
        public List<AttributeImageRemove> AttributeImageRemoveStore { get; set; }


    }
    public class TempList
    {
        public string Name { get; set; }
        public IFormFile VarientImage { get; set; }
        public string VarientImageName { get; set; }
    }
    public class AttributeValuse
    {
        public int AttributeId { get; set; }
        //public int[] AttributeValue { get; set; }
        public List<int> AttributeValue { get; set; }

    }
    public class AttributeImageRemove
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductAttributeDetailId { get; set; }
        public string AttributeImages { get; set; }
        
    }

}
