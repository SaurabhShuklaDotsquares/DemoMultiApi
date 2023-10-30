using EC.Service.Product;
using EC.Service.Taxs;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Stripe;
using EC.API.ViewModels;
using System.Collections.Generic;
using EC.API.ViewModels.SiteKey;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Asn1.Cms;
using NPOI.SS.Formula.Functions;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeAPIController : BaseAPIController
    {
        #region Constructor
        private readonly ICategoryService _categoryService;
        private readonly IBrandsService _brandsService;
        private readonly IProductService _productService;
        private readonly IBannersService _bannersService;
        private readonly IReviewsService _reviewsService;
        public HomeAPIController(ICategoryService categoryService, IBrandsService brandsService, IProductService productService, IBannersService bannersService, IReviewsService reviewsService)
        {
            _categoryService = categoryService;
            _brandsService = brandsService;
            _productService = productService;
            _bannersService= bannersService;    
            _reviewsService = reviewsService;
        }
        #endregion

        #region Home Api List
        [Route("/home")]
        [HttpGet]
        public IActionResult home()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                List<HomeViewModels> list = new List<HomeViewModels>();
                HomeViewModels model= new HomeViewModels();
                
                #region Get Products as new_arrival List

                var products = _productService.GetAllProdutList();
                if (products != null && products.Any())
                {
                    foreach (var item in products)
                    {
                        decimal totalRating = 0;
                        var reviews = _reviewsService.GetByproductidReviews(item.Id);
                        if (reviews != null && reviews.Any()) 
                        {
                            foreach (var itemreview in reviews)
                            {
                                totalRating += itemreview.Rating;
                            }
                            totalRating = totalRating / reviews.Count;
                        }
                        else
                        {
                            totalRating = 0;
                        }
                        ProductModels product = new ProductModels();
                        product.id = item.Id;
                        product.category_id = item.CategoryId.ToString();
                        product.title = item.Title;
                        product.price = item.Price;
                        product.discounted_price = item.DiscountedPrice;
                        product.slug = item.Slug;
                        product.prod_description = item.LongDescription;
                        product.average_rating = totalRating;
                        //product.display_price = "$" + item.Price;
                        //product.display_discounted_price = "$" + item.DiscountedPrice;
                        product.display_price = ConvertPrice(item.Price.Value);
                        decimal displayDiscountedPrice = item.DiscountedPrice != null && item.DiscountedPrice != 0 ? item.DiscountedPrice.Value : item.Price.Value;
                        product.display_discounted_price = ConvertPrice(displayDiscountedPrice);

                        foreach (var product_image in item.ProductImages)
                        {
                            ProductImagesModel productImages = new ProductImagesModel();
                            productImages.id = product_image.Id;
                            productImages.product_id = product_image.ProductId;
                            productImages.image_name = product_image.ImageName;
                            if (product_image.ImageName != null)
                            {
                                string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + product_image.ImageName;
                                productImages.image_link = uploadsFolder;
                            }
                            else
                            {
                                productImages.image_link = SiteKey.DefaultImage;
                            }
                            product.product_image.Add(productImages);
                        }
                        model.new_arrival.Add(product);
                    }
                }
                #endregion

                #region  Get Banners List

                var banners = _bannersService.GetActiveBannerList();
                if (banners != null && banners.Any())
                {
                    foreach (var item in banners)
                    {
                        BannerModels banner = new BannerModels();
                        banner.id = item.Id;
                        banner.title = item.Title;
                        banner.type = item.Type;
                        banner.image = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.DefaultImage;
                        model.banners.Add(banner);
                    }
                }
                #endregion

                #region Get Categories as featured_category List

                var categories = _categoryService.GetFeaturedCategoriesList();
                if (categories != null && categories.Any())
                {
                    foreach (var item in categories)
                    {
                        CategoriesModels category = new CategoriesModels();
                        category.id = item.Id;
                        //category.parent_id = item.ParentId;
                        //category.seller_id = item.SellerId;
                        category.is_featured = Convert.ToInt32(item.IsFeatured);
                        category.admin_commission = 0;
                        category.title = item.Title;
                        category.slug = item.Slug;
                        category.image = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.DefaultImage;
                        //category.banner = item.Banner;
                        //category.meta_title = item.MetaTitle;
                        //category.meta_keyword = item.MetaKeyword;
                        //category.meta_description = item.MetaDescription;
                        category.status = Convert.ToInt32(item.Status);
                        category.approval_status = item.ApprovalStatus;
                        category.lft = item.Lft !=null ? item.Lft.Value : 0;
                        category.rgt = item.Rgt != null ? item.Rgt.Value : 0;
                        //category.depth = item.Depth;
                        //category.created_at = item.CreatedAt != null ? item.CreatedAt.Value.ToString() : string.Empty;
                        //category.updated_at = item.UpdatedAt != null ? item.UpdatedAt.Value.ToString() : string.Empty;
                        category.image_link = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.DefaultImage;

                        // Get featured_products
                        var featuredProducts = _productService.GetByCategoryId(item.Id);
                        if (featuredProducts != null && featuredProducts.Any())
                        {
                            foreach (var itemfeaturedproducts in featuredProducts)
                            {
                                FeaturedProductModels featuredproduct = new FeaturedProductModels();
                                featuredproduct.id = itemfeaturedproducts.Id;
                                featuredproduct.product_type = itemfeaturedproducts.ProductType;
                                featuredproduct.vendor_id = 1;
                                //featuredproduct.seller_id = itemfeaturedproducts.SellerId;
                                featuredproduct.category_id = itemfeaturedproducts.CategoryId;
                                featuredproduct.title = itemfeaturedproducts.Title;
                                featuredproduct.slug = itemfeaturedproducts.Slug;
                                featuredproduct.brand_name = itemfeaturedproducts.BrandName.Value;
                                featuredproduct.sku = itemfeaturedproducts.Sku;
                                //featuredproduct.type = itemfeaturedproducts.Type;
                                //featuredproduct.tax_class = itemfeaturedproducts.TaxClass;
                                //featuredproduct.reference = itemfeaturedproducts.Reference;
                                //featuredproduct.features = itemfeaturedproducts.Features;
                                //featuredproduct.warranty_details = itemfeaturedproducts.WarrantyDetails;
                                //featuredproduct.customs_commodity_code = itemfeaturedproducts.CustomsCommodityCode;
                                featuredproduct.country_of_manufacture = itemfeaturedproducts.CountryOfManufacture.Value;
                                //featuredproduct.country_of_shipment = itemfeaturedproducts.CountryOfShipment;
                                //featuredproduct.barcode_type = itemfeaturedproducts.BarcodeType;
                                //featuredproduct.barcode = itemfeaturedproducts.Barcode;
                                featuredproduct.stock = itemfeaturedproducts.Stock;
                                featuredproduct.moq = itemfeaturedproducts.Moq;
                                featuredproduct.price = ConvertPrice(itemfeaturedproducts.Price.Value);
                                featuredproduct.discounted_price = ConvertPrice(itemfeaturedproducts.DiscountedPrice.Value);
                                //featuredproduct.discount_type = itemfeaturedproducts.DiscountType;
                                featuredproduct.flash_deal = itemfeaturedproducts.FlashDeal;
                                featuredproduct.ready_to_ship = itemfeaturedproducts.ReadyToShip;
                                //featuredproduct.meta_title = itemfeaturedproducts.MetaTitle;
                                //featuredproduct.meta_keyword = itemfeaturedproducts.MetaKeyword;
                                //featuredproduct.meta_description = itemfeaturedproducts.MetaDescription;
                                featuredproduct.banner_flag = itemfeaturedproducts.BannerFlag;
                                //featuredproduct.banner_image = itemfeaturedproducts.BannerImage;
                                //featuredproduct.banner_link = itemfeaturedproducts.BannerLink;
                                //featuredproduct.video = itemfeaturedproducts.Video;
                                //featuredproduct.short_description = itemfeaturedproducts.ShortDescription;
                                featuredproduct.long_description = itemfeaturedproducts.LongDescription;
                                featuredproduct.rating = itemfeaturedproducts.Rating;
                                featuredproduct.is_featured = Convert.ToInt32(itemfeaturedproducts.IsFeatured);
                                featuredproduct.is_popular = Convert.ToInt32(itemfeaturedproducts.IsPopular);
                                featuredproduct.show_notes = itemfeaturedproducts.ShowNotes;
                                featuredproduct.approval_status = itemfeaturedproducts.ApprovalStatus;
                                featuredproduct.is_change = itemfeaturedproducts.IsChange.ToString();
                                featuredproduct.status = Convert.ToInt32(itemfeaturedproducts.Status);
                                //featuredproduct.currency = null;
                                //featuredproduct.created_at = itemfeaturedproducts.CreatedAt;
                                //featuredproduct.updated_at = itemfeaturedproducts.UpdatedAt;
                                featuredproduct.prod_description = itemfeaturedproducts.LongDescription;
                                featuredproduct.average_rating = 0;
                                featuredproduct.url = null;
                                decimal displayDiscountedPrice = itemfeaturedproducts.DiscountedPrice != null && itemfeaturedproducts.DiscountedPrice != 0 ? itemfeaturedproducts.DiscountedPrice.Value : itemfeaturedproducts.Price.Value;
                                featuredproduct.display_discounted_price = ConvertPrice(displayDiscountedPrice);

                                // Get Featured Product Images
                                foreach (var product_image in itemfeaturedproducts.ProductImages)
                                {
                                    ProductImagesModel productImages = new ProductImagesModel();
                                    productImages.id = product_image.Id;
                                    productImages.product_id = product_image.ProductId;
                                    productImages.image_name = product_image.ImageName;
                                    productImages.image_link = productImages.image_name != null ? SiteKey.ImagePath + "/Uploads/" + product_image.ImageName : SiteKey.DefaultImage;
                                    featuredproduct.product_image.Add(productImages);
                                }
                                category.featured_products.Add(featuredproduct);
                            }
                        }
                        model.featured_category.Add(category);
                    }
                }
                #endregion

                #region Get Category List With Child

                var allCategories = _categoryService.GetAllCategories();
                if(allCategories != null && allCategories.Any())
                {
                    foreach (var item in allCategories)
                    {
                        CategoryModels parent = new CategoryModels();
                        parent.id = item.Id;
                        parent.parent_id = item.ParentId;
                        parent.title = item.Title;
                        parent.slug = item.Slug;
                        parent.image = item.Image;
                        parent.image_link = item.Image != null ? SiteKey.ImagePath + "/Uploads/" + item.Image : SiteKey.DefaultImage;

                        var childCategories = _categoryService.GetChildByCategoryId(item.Id);
                        if (childCategories != null && childCategories.Any())
                        {
                            foreach (var childitem in childCategories)
                            {
                                CategoryModels child = new CategoryModels();
                                child.id = childitem.Id;
                                child.parent_id = childitem.ParentId;
                                child.title = childitem.Title;
                                child.slug = childitem.Slug;
                                child.image = childitem.Image;
                                child.image_link = childitem.Image != null ? SiteKey.ImagePath + "/Uploads/" + childitem.Image : SiteKey.DefaultImage;
                                parent.childs.Add(child);
                            }
                        }
                        model.category.Add(parent);
                    }
                }
                #endregion

                return Ok(new { error = false, data= model, Message = "Categories fetch successfully.", state = "category", code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
