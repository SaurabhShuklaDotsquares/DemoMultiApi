using EC.Service.Currency_data;
using EC.Service.Taxs;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using EC.Service.Specification;
using EC.API.ViewModels.MultiVendor;
using NuGet.Protocol.Plugins;
using ToDo.WebApi.Models;
using Microsoft.EntityFrameworkCore.Internal;
using EC.API.ViewModels.SiteKey;
using Microsoft.AspNetCore.Authorization;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewVendorController : BaseAPIController
    {
        #region Constructor
        private readonly IReviewsService _reviewsService;
        private readonly IProductService _productService;
        public ReviewVendorController(IReviewsService reviewsService, IProductService productService)
        {
            _reviewsService = reviewsService;
            _productService = productService;
        }
        #endregion

        #region Review List Api
        [Authorize]
        [HttpPost]
        [Route("/vendor/get-all-review")]
        public ActionResult GetReviewList([FromQuery] int? page = 1, string? search = "")
        {
            try
            {
                ReturnReviewListModel returnReviewListModel = new ReturnReviewListModel();
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;
                var productid = _productService.GetByVendorId(userId);

                foreach (var item in productid)
                {
                    var reviewList = _reviewsService.GetReviewListForVendor(page.Value, search, item.Id);
                    //var PageMetadate = new
                    //{
                    //    reviewList.CurrentPage,
                    //    reviewList.PazeSize,
                    //    reviewList.TotalPage,
                    //    reviewList.TotalCount,
                    //    reviewList.HasNext,
                    //    reviewList.HasPrev
                    //};
                    
                    if (reviewList != null && reviewList.Any())
                    {
                        //Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));
                        foreach (var review in reviewList)
                        {
                            ReviewViewModel reviewViewModel = new ReviewViewModel();

                            #region Get Review List
                            reviewViewModel.id = review.Id;
                            reviewViewModel.user_id = review.UserId;
                            reviewViewModel.order_id = Convert.ToInt32(review.OrderId);
                            reviewViewModel.product_id = review.ProductId;
                            reviewViewModel.rating = review.Rating;
                            reviewViewModel.comment = review.Comment;
                            if (review.Status.ToString()=="1")
                            {
                                reviewViewModel.status= "Pending";
                            }
                            else if (review.Status.ToString() == "2")
                            {
                                reviewViewModel.status = "Approved";
                            }
                            else
                            {
                                reviewViewModel.status = "UnApproved";
                            }
                            //reviewViewModel.status = ((review.Status).ToString() == "1") ? "Pending" : "Approved";
                            reviewViewModel.created_at = review.CreatedAt;
                            reviewViewModel.updated_at = review.UpdatedAt;
                            #endregion

                            #region Get UserList
                            if (review.User != null)
                            {
                                UserViewModel userViewModel = new UserViewModel();
                                userViewModel.id = review.User.Id;
                                userViewModel.role = 1;
                                userViewModel.firstname = review.User.Firstname;
                                userViewModel.email = review.User.Email;
                                userViewModel.mobile = review.User.Mobile;
                                userViewModel.lastname = review.User.Lastname;
                                userViewModel.profile_pic = review.User.ProfilePic;
                                userViewModel.state = review.User.State;
                                userViewModel.email_verified_at = review.User.EmailVerifiedAt;
                                userViewModel.isVerified = Convert.ToInt32(review.User.IsVerified);
                                userViewModel.is_admin = review.User.IsAdmin;
                                userViewModel.stripe_customer_id = review.User.StripeCustomerId;
                                userViewModel.stripe_id = review.User.StripeId;
                                userViewModel.created_at = review.User.CreatedAt;
                                userViewModel.updated_at = review.User.UpdatedAt;
                                userViewModel.country = review.User.Country;
                                userViewModel.status = Convert.ToInt32(review.User.IsActive);
                                userViewModel.postal_code = review.User.PostalCode;
                                userViewModel.country_code = review.User.CountryCode;
                                userViewModel.is_guest = false;
                                reviewViewModel.user = userViewModel;
                            }
                            #endregion

                            #region Get Product List
                            if (review.Product != null)
                            {
                                ProductViewModel productViewModel = new ProductViewModel();
                                productViewModel.id = review.Product.Id;
                                productViewModel.product_type = review.Product.ProductType;
                                productViewModel.vendor_id = review.Product.VendorId;
                                productViewModel.seller_id = review.Product.SellerId;
                                productViewModel.category_id = review.Product.CategoryId;
                                productViewModel.title = review.Product.Title;
                                productViewModel.slug = review.Product.Slug;
                                productViewModel.brand_name = review.Product.BrandName;
                                productViewModel.sku = review.Product.Sku;
                                productViewModel.type = review.Product.Type;
                                productViewModel.tax_class = review.Product.TaxClass;
                                productViewModel.reference = review.Product.Reference;
                                productViewModel.features = review.Product.Features;
                                productViewModel.warranty_details = review.Product.WarrantyDetails;
                                productViewModel.customs_commodity_code = review.Product.CustomsCommodityCode;
                                productViewModel.country_of_manufacture = review.Product.CountryOfManufacture;
                                productViewModel.country_of_shipment = review.Product.CountryOfShipment;
                                productViewModel.barcode_type = review.Product.BarcodeType;
                                productViewModel.barcode = review.Product.Barcode;
                                productViewModel.stock = review.Product.Stock;
                                productViewModel.moq = review.Product.Moq;
                                productViewModel.price = review.Product.Price;
                                productViewModel.discounted_price = review.Product.DiscountedPrice;
                                productViewModel.discount_type = review.Product.DiscountType;
                                productViewModel.flash_deal = review.Product.FlashDeal;
                                productViewModel.ready_to_ship = review.Product.ReadyToShip;
                                productViewModel.meta_title = review.Product.MetaTitle;
                                productViewModel.meta_keyword = review.Product.MetaKeyword;
                                productViewModel.meta_description = review.Product.MetaDescription;
                                productViewModel.banner_flag = review.Product.BannerFlag;
                                productViewModel.banner_image = review.Product.BannerImage;
                                productViewModel.banner_link = review.Product.BannerLink;
                                productViewModel.video = review.Product.Video;
                                productViewModel.short_description = review.Product.ShortDescription;
                                productViewModel.long_description = review.Product.LongDescription;
                                productViewModel.rating = review.Product.Rating;
                                productViewModel.is_featured = Convert.ToInt32(review.Product.IsFeatured);
                                productViewModel.is_popular = Convert.ToInt32(review.Product.IsPopular);
                                productViewModel.show_notes = review.Product.ShowNotes;
                                productViewModel.approval_status = review.Product.ApprovalStatus;
                                productViewModel.is_change = review.Product.IsChange;
                                productViewModel.status = Convert.ToInt32(review.Product.Status);
                                productViewModel.currency = null;
                                productViewModel.created_at = review.Product.CreatedAt;
                                productViewModel.updated_at = review.Product.UpdatedAt;
                                productViewModel.prod_description = review.Product.LongDescription;
                                productViewModel.average_rating = Convert.ToInt32(review.Product.Rating);
                                productViewModel.url = null;
                                productViewModel.discounted_price = review.Product.Price;
                                productViewModel.display_discounted_price = review.Product.DiscountedPrice.ToString();
                                if (review.Product.ProductImages != null && review.Product.ProductImages.Any())
                                {
                                    foreach (var productImage in review.Product.ProductImages)
                                    {
                                        ProductImageViewModel productImageViewModel = new ProductImageViewModel();
                                        productImageViewModel.id = productImage.Id;
                                        productImageViewModel.product_id = productImage.ProductId;
                                        productImageViewModel.image_name = productImage.ImageName;
                                        productImageViewModel.image_link = !string.IsNullOrEmpty(productImage.ImageName) ? SiteKey.ImagePath + "/Uploads/" + productImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                        productViewModel.product_image.Add(productImageViewModel);
                                    }
                                }

                                reviewViewModel.product = productViewModel;
                            }
                            #endregion

                            returnReviewListModel.data.Add(reviewViewModel);
                           // returnReviewListModel.current_page = PageMetadate.CurrentPage;
                            returnReviewListModel.current_page = reviewList.CurrentPage;
                            returnReviewListModel.total_page = reviewList.TotalPage;
                            returnReviewListModel.page_size = reviewList.PazeSize;
                        }
                        Message = "Reviews all  fetch successfully!";
                    }
                    else
                    {
                        Message = "Record not found!";
                    }
                }
                return Ok(new { error = false, data = returnReviewListModel, Message = Message, state = "review", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get Review View Api
        [HttpPost]
        [Route("/vendor/get-single-review/{reviewId}")]
        public IActionResult GetReviewView(int reviewId)
        {
            try
            {
                if (reviewId <= 0)
                {
                    return Ok(new { error = false, data = "", Message = "Review id required", state = "review", code = 200, status = true });
                }
                string Message = string.Empty;
                ReturnReviewViewModel returnReviewViewModel = new ReturnReviewViewModel();
                var review = _reviewsService.GetByReviewsId(reviewId);
                if (review != null)
                {
                    #region Get Review List
                    returnReviewViewModel.id = review.Id;
                    returnReviewViewModel.user_id = review.UserId;
                    returnReviewViewModel.order_id = Convert.ToInt32(review.OrderId);
                    returnReviewViewModel.product_id = review.ProductId;
                    returnReviewViewModel.rating = review.Rating;
                    returnReviewViewModel.comment = review.Comment;
                    returnReviewViewModel.status = review.Status.ToString();
                    returnReviewViewModel.created_at = review.CreatedAt;
                    returnReviewViewModel.updated_at = review.UpdatedAt;
                    #endregion

                    #region Get UserList
                    if (review.User != null)
                    {
                        UserReviewViewModel userViewModel = new UserReviewViewModel();
                        userViewModel.id = review.User.Id;
                        userViewModel.firstname = review.User.Firstname;
                        userViewModel.lastname = review.User.Lastname;
                        userViewModel.is_guest = false;
                        returnReviewViewModel.user = userViewModel;
                    }
                    #endregion

                    #region Get Product List
                    if (review.Product != null)
                    {
                        ProductViewModel productViewModel = new ProductViewModel();
                        productViewModel.id = review.Product.Id;
                        productViewModel.product_type = review.Product.ProductType;
                        productViewModel.vendor_id = review.Product.VendorId;
                        productViewModel.seller_id = review.Product.SellerId;
                        productViewModel.category_id = review.Product.CategoryId;
                        productViewModel.title = review.Product.Title;
                        productViewModel.slug = review.Product.Slug;
                        productViewModel.brand_name = review.Product.BrandName;
                        productViewModel.sku = review.Product.Sku;
                        productViewModel.type = review.Product.Type;
                        productViewModel.tax_class = review.Product.TaxClass;
                        productViewModel.reference = review.Product.Reference;
                        productViewModel.features = review.Product.Features;
                        productViewModel.warranty_details = review.Product.WarrantyDetails;
                        productViewModel.customs_commodity_code = review.Product.CustomsCommodityCode;
                        productViewModel.country_of_manufacture = review.Product.CountryOfManufacture;
                        productViewModel.country_of_shipment = review.Product.CountryOfShipment;
                        productViewModel.barcode_type = review.Product.BarcodeType;
                        productViewModel.barcode = review.Product.Barcode;
                        productViewModel.stock = review.Product.Stock;
                        productViewModel.moq = review.Product.Moq;
                        productViewModel.price = review.Product.Price;
                        productViewModel.discounted_price = review.Product.DiscountedPrice;
                        productViewModel.discount_type = review.Product.DiscountType;
                        productViewModel.flash_deal = review.Product.FlashDeal;
                        productViewModel.ready_to_ship = review.Product.ReadyToShip;
                        productViewModel.meta_title = review.Product.MetaTitle;
                        productViewModel.meta_keyword = review.Product.MetaKeyword;
                        productViewModel.meta_description = review.Product.MetaDescription;
                        productViewModel.banner_flag = review.Product.BannerFlag;
                        productViewModel.banner_image = review.Product.BannerImage;
                        productViewModel.banner_link = review.Product.BannerLink;
                        productViewModel.video = review.Product.Video;
                        productViewModel.short_description = review.Product.ShortDescription;
                        productViewModel.long_description = review.Product.LongDescription;
                        productViewModel.rating = review.Product.Rating;
                        productViewModel.is_featured = Convert.ToInt32(review.Product.IsFeatured);
                        productViewModel.is_popular = Convert.ToInt32(review.Product.IsPopular);
                        productViewModel.show_notes = review.Product.ShowNotes;
                        productViewModel.approval_status = review.Product.ApprovalStatus;
                        productViewModel.is_change = review.Product.IsChange;
                        productViewModel.status = Convert.ToInt32(review.Product.Status);
                        productViewModel.currency = null;
                        productViewModel.created_at = review.Product.CreatedAt;
                        productViewModel.updated_at = review.Product.UpdatedAt;
                        productViewModel.prod_description = review.Product.LongDescription;
                        productViewModel.average_rating = Convert.ToInt32(review.Product.Rating);
                        productViewModel.url = null;
                        productViewModel.discounted_price = review.Product.Price;
                        productViewModel.display_discounted_price = review.Product.DiscountedPrice.ToString();
                        if (review.Product.ProductImages != null && review.Product.ProductImages.Any())
                        {
                            foreach (var productImage in review.Product.ProductImages)
                            {
                                ProductImageViewModel productImageViewModel = new ProductImageViewModel();
                                productImageViewModel.id = productImage.Id;
                                productImageViewModel.product_id = productImage.ProductId;
                                productImageViewModel.image_name = productImage.ImageName;
                                productImageViewModel.image_link = productImage.ImageName;
                                productViewModel.product_image.Add(productImageViewModel);
                            }
                        }

                        returnReviewViewModel.product = productViewModel;
                    }
                    #endregion

                    Message = "Review fetch successfully!";
                }
                else
                {
                    Message = "Record not found!";
                }
                return Ok(new { error = false, data = returnReviewViewModel, Message = Message, state = "review", code = 200, status = true });
            }
            catch (Exception Ex) 
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Review Status Update Api
        [HttpPost]
        [Route("/vendor/review-status")]
        public IActionResult ReviewStatusUpdate([FromForm] ReviewStatusUpdateRequestModel model)
        {
            try
            {
                string Message = string.Empty;
                ReturnReviewViewModel returnReviewViewModel = new ReturnReviewViewModel();
                #region Update Review
                var review = _reviewsService.GetByReviewsId(model.id);
                if (review != null)
                {
                    review.Status = Convert.ToByte(model.status);
                    review.Comment = model.comment!=null ? model.comment: review.Comment;
                    var reviewResponse = _reviewsService.UpdateReviewss(review);
                    if (reviewResponse != null)
                    {
                        #region Get Review List
                        returnReviewViewModel.id = reviewResponse.Id;
                        returnReviewViewModel.user_id = reviewResponse.UserId;
                        returnReviewViewModel.order_id = Convert.ToInt32(reviewResponse.OrderId);
                        returnReviewViewModel.product_id = reviewResponse.ProductId;
                        returnReviewViewModel.rating = reviewResponse.Rating;
                        returnReviewViewModel.comment = reviewResponse.Comment;
                       
                        returnReviewViewModel.status = (reviewResponse.Status.ToString()) == "2" ? "Approved" : "Pending";
                        returnReviewViewModel.created_at = reviewResponse.CreatedAt;
                        returnReviewViewModel.updated_at = reviewResponse.UpdatedAt;
                        #endregion

                        #region Get UserList
                        if (review.User != null)
                        {
                            UserReviewViewModel userViewModel = new UserReviewViewModel();
                            userViewModel.id = reviewResponse.User.Id;
                            userViewModel.firstname = reviewResponse.User.Firstname;
                            userViewModel.lastname = reviewResponse.User.Lastname;
                            userViewModel.is_guest = false;
                            returnReviewViewModel.user = userViewModel;
                        }
                        #endregion

                        #region Get Product List
                        if (review.Product != null)
                        {
                            ProductViewModel productViewModel = new ProductViewModel();
                            productViewModel.id = reviewResponse.Product.Id;
                            productViewModel.product_type = reviewResponse.Product.ProductType;
                            productViewModel.vendor_id = reviewResponse.Product.VendorId;
                            productViewModel.seller_id = reviewResponse.Product.SellerId;
                            productViewModel.category_id = reviewResponse.Product.CategoryId;
                            productViewModel.title = reviewResponse.Product.Title;
                            productViewModel.slug = reviewResponse.Product.Slug;
                            productViewModel.brand_name = reviewResponse.Product.BrandName;
                            productViewModel.sku = reviewResponse.Product.Sku;
                            productViewModel.type = reviewResponse.Product.Type;
                            productViewModel.tax_class = reviewResponse.Product.TaxClass;
                            productViewModel.reference = reviewResponse.Product.Reference;
                            productViewModel.features = reviewResponse.Product.Features;
                            productViewModel.warranty_details = reviewResponse.Product.WarrantyDetails;
                            productViewModel.customs_commodity_code = reviewResponse.Product.CustomsCommodityCode;
                            productViewModel.country_of_manufacture = reviewResponse.Product.CountryOfManufacture;
                            productViewModel.country_of_shipment = reviewResponse.Product.CountryOfShipment;
                            productViewModel.barcode_type = reviewResponse.Product.BarcodeType;
                            productViewModel.barcode = reviewResponse.Product.Barcode;
                            productViewModel.stock = reviewResponse.Product.Stock;
                            productViewModel.moq = reviewResponse.Product.Moq;
                            productViewModel.price = reviewResponse.Product.Price;
                            productViewModel.discounted_price = reviewResponse.Product.DiscountedPrice;
                            productViewModel.discount_type = reviewResponse.Product.DiscountType;
                            productViewModel.flash_deal = reviewResponse.Product.FlashDeal;
                            productViewModel.ready_to_ship = reviewResponse.Product.ReadyToShip;
                            productViewModel.meta_title = reviewResponse.Product.MetaTitle;
                            productViewModel.meta_keyword = reviewResponse.Product.MetaKeyword;
                            productViewModel.meta_description = reviewResponse.Product.MetaDescription;
                            productViewModel.banner_flag = reviewResponse.Product.BannerFlag;
                            productViewModel.banner_image = reviewResponse.Product.BannerImage;
                            productViewModel.banner_link = reviewResponse.Product.BannerLink;
                            productViewModel.video = reviewResponse.Product.Video;
                            productViewModel.short_description = reviewResponse.Product.ShortDescription;
                            productViewModel.long_description = reviewResponse.Product.LongDescription;
                            productViewModel.rating = reviewResponse.Product.Rating;
                            productViewModel.is_featured = Convert.ToInt32(reviewResponse.Product.IsFeatured);
                            productViewModel.is_popular = Convert.ToInt32(reviewResponse.Product.IsPopular);
                            productViewModel.show_notes = reviewResponse.Product.ShowNotes;
                            productViewModel.approval_status = reviewResponse.Product.ApprovalStatus;
                            productViewModel.is_change = reviewResponse.Product.IsChange;
                            productViewModel.status = Convert.ToInt32(reviewResponse.Product.Status);
                            productViewModel.currency = null;
                            productViewModel.created_at = reviewResponse.Product.CreatedAt;
                            productViewModel.updated_at = reviewResponse.Product.UpdatedAt;
                            productViewModel.prod_description = reviewResponse.Product.LongDescription;
                            productViewModel.average_rating = Convert.ToInt32(reviewResponse.Product.Rating);
                            productViewModel.url = null;
                            productViewModel.discounted_price = reviewResponse.Product.Price;
                            productViewModel.display_discounted_price = reviewResponse.Product.DiscountedPrice.ToString();
                            if (review.Product.ProductImages != null && reviewResponse.Product.ProductImages.Any())
                            {
                                foreach (var productImage in reviewResponse.Product.ProductImages)
                                {
                                    ProductImageViewModel productImageViewModel = new ProductImageViewModel();
                                    productImageViewModel.id = productImage.Id;
                                    productImageViewModel.product_id = productImage.ProductId;
                                    productImageViewModel.image_name = productImage.ImageName;
                                    productImageViewModel.image_link = productImage.ImageName;
                                    productViewModel.product_image.Add(productImageViewModel);
                                }
                            }

                            returnReviewViewModel.product = productViewModel;
                        }
                        #endregion
                    }
                    Message = "Review Approved successfully.";
                }
                else
                {
                    Message = "Record not found.";
                }
                #endregion
                return Ok(new { error = false, data = returnReviewViewModel, Message = Message, state = "review", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
