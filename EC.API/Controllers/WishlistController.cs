using EC.API.ViewModels;
using EC.API.ViewModels.SiteKey;
using EC.Data.Models;
using EC.Service;
using EC.Service.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using ToDo.WebApi.Models;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WishlistController : BaseAPIController
    {
        #region Constructor
        private readonly IProductService _productService;
        private readonly IWishlistService _iWishlistService;
        public WishlistController(IProductService productService, IWishlistService iWishlistService)
        {
            _productService = productService;
            _iWishlistService = iWishlistService;
        }
        #endregion

        #region Add Remove WhishList Api
        [HttpPost]

        [Route("/wishlist/add-remove-list")]
        [Authorize]
        public IActionResult AddRemovewhitlist(wishlistViewModels model)
        {
            string Message = string.Empty;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                var wishList = _iWishlistService.GetByProductIdAndUserId(model.product_id, userId);
                if (model.Status_type == 1)
                {
                    if (wishList != null)
                    {
                        Message = "This product is already exists.";
                    }
                    else
                    {
                        Wishlists entitywishlist = new Wishlists();
                        entitywishlist.UserId = userId;
                        entitywishlist.ProductId = model.product_id;
                        entitywishlist.CreatedAt = DateTime.Now;
                        var entity = _iWishlistService.Save(entitywishlist);
                        Message = "Product successfully added to wishlist.";
                    }
                }
                else if (model.Status_type == 0)
                {
                    if(wishList != null)
                    {
                        wishList = _iWishlistService.Delete(wishList);
                        Message = "Product successfully removed from wishlist.";
                    }
                    else
                    {
                        var errorData = new { error = true, message = "Not found", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                }
                return Ok(new { error = false, data = "", message = Message, code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get Whish List Api

        [Authorize]
        [Route("/wishlist/list")]
        [HttpPost]
        public IActionResult GetWhishList()
        {
            string Message = string.Empty;
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                List<WhishListModel> wishList = new List<WhishListModel>();
                var whishList = _iWishlistService.GetWishListByUserId(userId);
                if (whishList != null && whishList.Any())
                {
                    foreach (var item in whishList)
                    {
                        WhishListModel model= new WhishListModel();
                        model.id = item.Id;
                        model.product_id = item.ProductId;
                        model.user_id = item.UserId;
                        model.created_at = item.CreatedAt.ToString();
                        model.updated_at= item.UpdatedAt.ToString();

                        if (item.Product != null)
                        {
                            Product product = new Product();
                            product.id = item.Product.Id;
                            product.product_type = item.Product.ProductType;
                            product.vendor_id = item.Product.VendorId??0;
                            product.seller_id = item.Product.SellerId != null ? item.Product.SellerId.Value.ToString() : string.Empty;
                            product.category_id = item.Product.CategoryId;
                            product.title = item.Product.Title;
                            product.slug = item.Product.Slug;
                            product.brand_name = item.Product.BrandName != null ? item.Product.BrandName.Value : 0;
                            product.price = item.Product.DiscountedPrice != null && item.Product.DiscountedPrice != 0 ? item.Product.DiscountedPrice.Value : item.Product.Price.Value;
                            product.sku = item.Product.Sku;
                            product.type = item.Product.Type;
                            product.tax_class = item.Product.TaxClass;
                            product.reference = item.Product.Reference;
                            product.features = item.Product.Features;
                            product.warranty_details = item.Product.WarrantyDetails;
                            product.customs_commodity_code = item.Product.CustomsCommodityCode;
                            product.country_of_manufacture = item.Product.CountryOfManufacture;
                            product.country_of_shipment = item.Product.CountryOfShipment;
                            product.barcode_type = item.Product.BarcodeType;
                            product.barcode = item.Product.Barcode;
                            product.stock = item.Product.Stock;
                            product.moq = item.Product.Moq;
                            product.discounted_price = item.Product.DiscountedPrice;
                            product.flash_deal = item.Product.FlashDeal;
                            product.ready_to_ship = item.Product.ReadyToShip;
                            product.meta_title = item.Product.MetaTitle;
                            product.meta_keyword = item.Product.MetaKeyword;
                            product.meta_description = item.Product.MetaDescription;
                            product.banner_flag = item.Product.BannerFlag;
                            product.banner_image = item.Product.BannerImage;
                            product.banner_link = item.Product.BannerLink;
                            product.video = item.Product.Video;
                            product.short_description = item.Product.ShortDescription;
                            product.long_description = item.Product.LongDescription;
                            product.rating = item.Product.Rating;
                            product.is_featured = item.Product.IsFeatured;
                            product.is_popular = item.Product.IsPopular;
                            product.show_notes = item.Product.ShowNotes;
                            product.approval_status = item.Product.ApprovalStatus;
                            product.is_change = item.Product.IsChange;
                            product.status = item.Product.Status;
                            product.currency = null;
                            product.created_at = item.Product.CreatedAt;
                            product.updated_at = item.Product.UpdatedAt;
                            product.prod_description = item.Product.LongDescription;
                            product.average_rating = 0;
                            product.url = null;
                            // Calculate display price
                            decimal displayPrice = item.Product.Price != null ? item.Product.Price.Value : 0;
                            product.display_price = ConvertPrice(displayPrice);
                            // Calculate display disconted price
                            decimal displayDiscountedPrice = item.Product.DiscountedPrice != null ? item.Product.DiscountedPrice.Value : 0;
                            product.display_discounted_price = ConvertPrice(displayDiscountedPrice);
                            model.product = product;
                        }

                        if(item.Product.ProductImages != null && item.Product.ProductImages.Any())
                        {
                            foreach (var itemImage in item.Product.ProductImages)
                            {
                                ProductImage productImage = new ProductImage();
                                //productImage.id = itemImage.Id;
                                //productImage.product_id = itemImage.ProductId;
                                productImage.image_name= itemImage.ImageName;
                                productImage.image_link = SiteKey.ImagePath + "/Uploads/" + itemImage.ImageName;
                                model.product.product_image.Add(productImage);
                            }
                        }
                        wishList.Add(model);
                    }
                    Message = "Wishlists fetch successfully.";
                }
                else
                {
                    Message = "Record Not Found.";
                }

                return Ok(new { error = false, data = wishList, message = Message, authenticate = true, code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get WishList Count

        [Authorize]
        [Route("/wishlist/count")]
        [HttpPost]
        public IActionResult WishListCount()
        {
            var authuser = new AuthUser(User);
            var userId = authuser.Id;
            string Message = string.Empty;
            int Count = 0;
            try
            {
                var whishListCount = _iWishlistService.GetCount(userId);
                if (whishListCount > 0)
                {
                    Count = whishListCount;
                    Message = "Wish list count fetch successfully.";
                }
                else
                {
                    Message = "Record Not Found.";
                }
                return Ok(new { error = false, data = Count, message = Message, authenticate = true, code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
