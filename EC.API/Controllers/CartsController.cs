using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using EC.API.ViewModels;
using EC.Core.LIBS;
using EC.Service.Product;
using EC.Data.Entities;
using ToDo.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using EC.Data.Models;
using EC.API.ViewModels.SiteKey;
using EC.Service.Specification;
using System.Collections.Generic;
using System.Linq;
using EC.Service.Shippings;
using Stripe;
using EC.Service.Taxs;

namespace EC.API.Controllers
{
    
    [Route("api/[controller]/[action]")]
    [ApiController]   
    public class CartsController : BaseAPIController
    {
        #region Constructor
        private readonly ICartService _cartService;
        private readonly IOptionsService _optionsService;
        private readonly IProductService _productService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        private readonly IProductAttributeImageService _productAttributeImageService;
        private readonly IproductImagesService _productImagesService;
        private readonly IShippingService _shippingService;
        private readonly ITaxService _taxService;
        public CartsController(ICartService cartService, IOptionsService optionsService, IProductService productService, IProductAttributeDetailsService productAttributeDetailsService, IProductAttributeImageService productAttributeImageService, IproductImagesService productImagesService, IShippingService shippingService, ITaxService taxService)
        {
            _cartService = cartService;
            _optionsService = optionsService;
            _productService = productService;
            _productAttributeDetailsService = productAttributeDetailsService;
            _productAttributeImageService = productAttributeImageService;
            _productImagesService = productImagesService;
            _shippingService = shippingService;
            _taxService = taxService;
        }
        #endregion

        #region Add To Cart Api
        [Authorize]
        [Route("/cart/add")]
        [HttpPost]
        public IActionResult AddCart(CartViewModels model)
        {
            try
            {
                CartResponseModel responseModel= new CartResponseModel();
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                decimal taxRate = 0;
                // Product Validation Check
                var product = _productService.GetById(model.product_id);
                if (product != null)
                {

                    // Product varient validation check
                    var varient = product.ProductAttributeDetails != null && product.ProductAttributeDetails.Any() ? product.ProductAttributeDetails.Where(x => x.Id == model.variant_id).FirstOrDefault() : null;
                    if (varient != null)
                    {
                        if (varient.Stock == 0)
                        {
                            var errorData = new { error = true, message = "Product quantity not available.", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        else if (varient.Stock < model.quantity)
                        {
                            var errorData = new { error = true, message = "Product quantity should be less than.", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                    }
                    else
                    {
                        if (product.Stock == 0)
                        {
                            var errorData = new { error = true, messagesg = "Product quantity not available.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        else if (product.Stock < model.quantity)
                        {
                            var errorData = new { error = true, messagesg = "Product quantity should be less than.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                    }
                    int categoryId = product.Category.ParentId != 0 && product.Category.ParentId > 0 ? product.Category.ParentId.Value : product.Category.Id;
                    var tax = _taxService.GetTaxByCategoryId(categoryId);
                    taxRate = tax != null ? tax.Value.Value : 0;
                    
                }
                else
                {
                    var errorData = new { error = true, messagesg = "Product does not exists, please try another one.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }

                // Vendor Validation Check
                var vendor = _productAttributeDetailsService.GetBy_Id(Convert.ToInt32(model.variant_id));
                if (vendor != null)
                {
                    if (vendor.Stock == 0)
                    {
                        var errorData = new { error = true, messagesg = "Vendor quantity not available.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                    else if (vendor.Stock < model.quantity)
                    {
                        var errorData = new { error = true, messagesg = "Vendor quantity should be less than.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                }
                var carts = new Carts();
                if (model.product_id !=0 && model.variant_id != 0 && model.variant_id != null && model.quantity !=0)
                {
                    carts = _cartService.GetByUserIdAndProductIdAndVarientId(userId, model.product_id, model.variant_id.Value);
                }
                else
                {
                    carts = _cartService.GetByUserIdAndProductId(userId, model.product_id);
                }
                
                if (carts != null)
                {
                    carts.Quantity = carts.Quantity + model.quantity;
                    carts.UpdatedAt = DateTime.Now;
                    if (model.variant_id != 0 && model.variant_id != null && vendor != null)
                    {
                        if (vendor.Stock < carts.Quantity)
                        {
                            var errorData = new { error = true, messagesg = "Desired quantity is less then actual quantity, Please adjust the quantity.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        decimal price = (vendor.Price != 0 && vendor.Price != null ? vendor.Price.Value : vendor.RegularPrice.Value);
                        decimal priceWithTax = taxRate !=0 ? (price + (taxRate * price) / 100) : price;
                        var finalValue = model.quantity * priceWithTax;
                        carts.FinalValue = carts.FinalValue + finalValue;
                    }
                    else
                    {
                        if (product.Stock < carts.Quantity)
                        {
                            var errorData = new { error = true, messagesg = "Desired quantity is less then actual quantity, Please adjust the quantity.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        decimal price = (product.DiscountedPrice != 0 && product.DiscountedPrice != null ? product.DiscountedPrice.Value : product.Price.Value);
                        decimal priceWithTax = taxRate != 0 ? (price + (taxRate * price) / 100) : price;
                        var finalValue = model.quantity * priceWithTax;
                        carts.FinalValue = carts.FinalValue + finalValue;
                    }
                    carts = _cartService.Update(carts);
                }
                else
                {
                    Carts entity = new Carts();
                    entity.UserId = userId;
                    entity.SellerId = product.VendorId;
                    entity.ProductId = model.product_id;
                    entity.Quantity = model.quantity;
                    entity.CreatedAt = DateTime.Now;
                    entity.VariantId = model.variant_id != 0 && model.variant_id != null ? model.variant_id : null;
                    entity.VariantSlug = model.variant_slug != string.Empty ? model.variant_slug : null;
                    if (model.variant_id != 0 && model.variant_id != null && vendor != null)
                    {
                        decimal price = (vendor.Price != 0 && vendor.Price != null ? vendor.Price.Value : vendor.RegularPrice.Value);
                        decimal priceWithTax = taxRate != 0 ? (price + (taxRate * price) / 100) : price;
                        var finalValue = model.quantity * priceWithTax;
                        entity.FinalValue = finalValue;
                    }
                    else
                    {
                        decimal price = (product.DiscountedPrice != 0 && product.DiscountedPrice != null ? product.DiscountedPrice.Value : product.Price.Value);
                        decimal priceWithTax = taxRate != 0 ? (price + (taxRate * price) / 100) : price;
                        var finalValue = model.quantity * priceWithTax;
                        entity.FinalValue = finalValue;
                    }
                    entity = _cartService.Save(entity);
                    if (entity != null)
                    {
                        responseModel.id = entity.Id;
                        responseModel.user_id = userId;
                        responseModel.product_id = entity.ProductId;
                        responseModel.seller_id = entity.SellerId;
                        responseModel.quantity = entity.Quantity;
                        responseModel.created_at = entity.CreatedAt;
                        responseModel.updated_at = entity.UpdatedAt;
                        responseModel.variant_id = entity.VariantId;
                        responseModel.variant_slug = entity.VariantSlug;
                        responseModel.final_value = entity.FinalValue;
                        responseModel.tax_amount = taxRate;
                    }
                }

                return Ok(new { error = false, data = responseModel, message = "Added successfully!", code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Update Cart api
        [Authorize]
        [Route("/cart/update")]
        [HttpPost]
        public IActionResult updateCart(CartViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var ID = authuser.Id;

                // Product Validation check
                var product = _productService.GetById(model.product_id);
                if (product != null)
                {
                    // Product varient validation check
                    var varient = product.ProductAttributeDetails != null && product.ProductAttributeDetails.Any() ? product.ProductAttributeDetails.Where(x => x.Id == model.variant_id).FirstOrDefault() : null;
                    if (varient != null)
                    {
                        if (varient.Stock == 0)
                        {
                            var errorData = new { error = true, message = "Product quantity not available.", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        else if (varient.Stock < model.quantity)
                        {
                            var errorData = new { error = true, message = "Product quantity should be less than.", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                    }
                    if (product.Stock == 0)
                    {
                        var errorData = new { error = true, message = "Product quantity not available.", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                    else if (product.Stock < model.quantity)
                    {
                        var errorData = new { error = true, message = "Product quantity should be less than.", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }

                    var carts = _cartService.GetByCartIdAndProductId(model.cart_id, model.product_id);
                    if (carts != null)
                    {
                        varient = carts.VariantId != 0 && carts.VariantId != null ? product.ProductAttributeDetails.Where(x => x.Id == carts.VariantId).FirstOrDefault() : null;
                        if (varient != null && varient.Stock < model.quantity)
                        {
                            var errorData = new { error = true, messagesg = "Desired quantity is less then actual quantity, Please adjust the quantity.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        if (product.Stock < model.quantity)
                        {
                            var errorData = new { error = true, messagesg = "Desired quantity is less then actual quantity, Please adjust the quantity.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        carts.UpdatedAt = DateTime.Now;
                        //carts.SellerId = carts.SellerId;
                        carts.FinalValue = model.quantity * (carts.FinalValue / carts.Quantity);
                        carts.Quantity = model.quantity;
                        //carts.FinalValue = carts.FinalValue + finalValue;
                        carts = _cartService.Update(carts);
                    }
                    else
                    {
                        return NotFound();
                    }
                    return Ok(new { error = false, data = "", message = "Cart updated successfully!", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, data = "null", message = "Product does not exists, please try another one.", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                    //return Ok(new { error = false, data = "null", message = "Product does not exists, please try another one.", code = 400, status = false });
                }
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Delete Cart Api
        //[Authorize]
        [Route("/cart/remove")]
        [HttpPost]
        public IActionResult Deletecart(cartId_data cart)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                Carts carts = _cartService.GetById(cart.cart_id);
                if (carts != null)
                {
                    bool isDeleted = _cartService.Delete(carts);
                    return Ok(new { error = false, data = "", message = "Cart item removed successfully!", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Not found cart", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get All CartList Api
        [Authorize]
        [Route("/list_cart")]
        [HttpGet]
        public IActionResult GetAllcartlists()
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                decimal shippingCharge = 0;
                decimal TotalPrice = 0;
                ResponseCartdata responseCartList = new ResponseCartdata();
                var cartsList = _cartService.GetCartsByUserId(userId);
                if (cartsList != null && cartsList.Any())
                {
                    foreach (var item in cartsList)
                    {
                        cartdata cart = new cartdata();
                        cart.id = item.Id;
                        cart.product_id = item.ProductId;
                        cart.quantity = item.Quantity;
                        cart.variant_id = item.VariantId;
                        cart.variant_slug = item.VariantSlug;
                        cart.final_value = item.FinalValue;
                        cart.created_at = item.CreatedAt;

                        if (item.Product != null && item.Product.ProductImages != null && item.Product.ProductImages.Any())
                        {
                            foreach (var itemProductImage in item.Product.ProductImages)
                            {
                                CartImagedata cartImage = new CartImagedata();
                                cartImage.id = itemProductImage.Id;
                                cartImage.product_id = itemProductImage.ProductId;
                                cartImage.image_name = itemProductImage.ImageName != null ? itemProductImage.ImageName : SiteKey.DefaultImage;
                                cartImage.image_link = itemProductImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + itemProductImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                cart.product_image.Add(cartImage);
                            }
                        }
                        // Calculate cart display final price
                        decimal cartDisplayFinalPrice = item.FinalValue.Value / item.Quantity;
                        cart.display_final_price = ConvertPrice(cartDisplayFinalPrice);
                        // Calculate cart display final
                        decimal cartDisplayFinal = item.FinalValue.Value;
                        cart.display_final = ConvertPrice(cartDisplayFinal);

                        #region Get Product Data
                        if (item.Product != null)
                        {
                            var varient = item.VariantId != 0 && item.VariantId != null ? _productAttributeDetailsService.GetBy_Id(item.VariantId.Value) : null;
                            decimal taxRate = 0;
                            if (item.Product.Category != null)
                            {
                                int categoryId = item.Product.Category.ParentId != 0 && item.Product.Category.ParentId > 0 ? item.Product.Category.ParentId.Value : item.Product.Category.Id;
                                var tax = _taxService.GetTaxByCategoryId(categoryId);
                                taxRate = tax != null ? tax.Value.Value : 0;
                            }
                            //decimal amount = Convert.ToDecimal(item.Quantity * item.Product.Price);
                            TotalPrice += item.FinalValue.Value;
                            //responseCartList.display_sub_total += "$" + amount;    // quantity * produc prce
                            responseCartList.display_tax_price = 0;
                            //responseCartList.display_total_price = "$" + (amount + shippingCharge);  // quantity * produc prce + shipping
                            responseCartList.quantity += item.Quantity;

                            Productdata product = new Productdata();
                            product.id = item.Product.Id;    
                            product.product_type = item.Product.ProductType;
                            product.vendor_id = 0;
                            product.seller_id = item.Product.SellerId;
                            product.category_id = item.Product.CategoryId;
                            product.title = item.Product.Title;
                            product.slug = item.Product.Slug;
                            product.brand_name = item.Product.BrandName;
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
                            product.stock = varient != null ? varient.Stock.Value : item.Product.Stock;
                            product.moq = item.Product.Moq;
                            product.price = item.Product.DiscountedPrice != null && item.Product.DiscountedPrice != 0 ? item.Product.DiscountedPrice : item.Product.Price;
                            product.discounted_price = item.Product.DiscountedPrice != null && item.Product.DiscountedPrice != 0 ? item.Product.DiscountedPrice : item.Product.Price;
                            product.discount_type = item.Product.DiscountType;
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
                            product.is_featured = Convert.ToInt32(item.Product.IsFeatured);
                            product.is_popular = Convert.ToInt32(item.Product.IsPopular);
                            product.show_notes = item.Product.ShowNotes;
                            product.approval_status = item.Product.ApprovalStatus;
                            product.is_change = item.Product.IsChange;
                            product.status = Convert.ToInt32(item.Product.Status);
                            product.currency = null;
                            product.created_at = item.Product.CreatedAt;
                            product.updated_at = item.Product.UpdatedAt;
                            product.prod_description = item.Product.LongDescription;
                            product.average_rating = Convert.ToInt32(item.Product.Rating);
                            product.url = null;
                            // Calculate product display price
                            decimal displayPrice = item.Product.DiscountedPrice != null && item.Product.DiscountedPrice != 0 ? taxRate != 0 ? (item.Product.DiscountedPrice.Value + (taxRate * item.Product.DiscountedPrice.Value) / 100) : item.Product.DiscountedPrice.Value : taxRate != 0 ? (item.Product.Price.Value + (taxRate * item.Product.Price.Value) / 100) : item.Product.Price.Value;
                            product.display_price = ConvertPrice(displayPrice);
                            // Calculate product display discounted price
                            decimal displaydiscountedPrice = item.Product.DiscountedPrice != null && item.Product.DiscountedPrice != 0 ? taxRate != 0 ? (item.Product.DiscountedPrice.Value + (taxRate * item.Product.DiscountedPrice.Value) / 100) : item.Product.DiscountedPrice.Value : taxRate != 0 ? (item.Product.Price.Value + (taxRate * item.Product.Price.Value) / 100) : item.Product.Price.Value;
                            product.display_discounted_price = ConvertPrice(displaydiscountedPrice);

                            if (item.Product != null && item.Product.ProductImages != null && item.Product.ProductImages.Any())
                            {
                                foreach (var itemProductImage in item.Product.ProductImages)
                                {
                                    CartImagedata cartImage = new CartImagedata();
                                    cartImage.id = itemProductImage.Id;
                                    cartImage.product_id = itemProductImage.ProductId;
                                    cartImage.image_name = itemProductImage.ImageName != null ? itemProductImage.ImageName : SiteKey.DefaultImage;
                                    cartImage.image_link = itemProductImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + itemProductImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                    product.product_image.Add(cartImage);
                                }
                            }
                            cart.product = product;
                        }
                        #endregion

                        #region Get Product Attribute Detail
                        if (item.Product != null && item.Product.ProductAttributeDetails != null && item.Product.ProductAttributeDetails.Any())
                        {
                            foreach (var itemProductAttributeDetail in item.Product.ProductAttributeDetails)
                            {
                                ProductAttributeDetaildata productAttributeDetail = new ProductAttributeDetaildata();
                                productAttributeDetail.id = itemProductAttributeDetail.Id;
                                productAttributeDetail.product_id = itemProductAttributeDetail.ProductId;
                                productAttributeDetail.attribute_slug = itemProductAttributeDetail.AttributeSlug;
                                productAttributeDetail.variant_text = itemProductAttributeDetail.VariantText;
                                productAttributeDetail.regular_price = itemProductAttributeDetail.RegularPrice;
                                productAttributeDetail.price = itemProductAttributeDetail.Price;
                                productAttributeDetail.stock = itemProductAttributeDetail.Stock;
                                productAttributeDetail.created_at = itemProductAttributeDetail.CreatedAt;
                                productAttributeDetail.updated_at = itemProductAttributeDetail.UpdatedAt;
                                // Calculate product attribute detail display price
                                decimal productAttributeDisplayPrice = itemProductAttributeDetail.RegularPrice != null && itemProductAttributeDetail.RegularPrice != 0 ? itemProductAttributeDetail.RegularPrice.Value : itemProductAttributeDetail.Price.Value;
                                productAttributeDetail.display_price = ConvertPrice(productAttributeDisplayPrice);
                                // Calculte product attribute detail display regular price
                                decimal productAttributeDisplayRegularPrice = itemProductAttributeDetail.RegularPrice != null && itemProductAttributeDetail.RegularPrice != 0 ? itemProductAttributeDetail.RegularPrice.Value : itemProductAttributeDetail.Price.Value;
                                productAttributeDetail.display_regular_price = ConvertPrice(productAttributeDisplayRegularPrice); 
                                cart.product_attribute_detail.Add(productAttributeDetail);
                            }
                        }
                        #endregion

                        #region Get Shipping Data
                        var shippingAmount = _shippingService.GetShippingRates(TotalPrice);
                        shippingCharge = shippingAmount != null ? shippingAmount.ShippingCharge : 0;
                        responseCartList.sub_total = TotalPrice;
                        responseCartList.tax_price = 0;
                        responseCartList.total_price = TotalPrice + shippingCharge;   // quantity * produc prce + shipping
                        responseCartList.shipping_charges = shippingCharge;
                        responseCartList.display_shipping_charges = ConvertPrice(shippingCharge);

                        Shippingdata shipping = new Shippingdata();
                        shipping.message = "SUCCESS";

                        Ratedata rate = new Ratedata();
                        rate.shipping_charges = shippingCharge;
                        shipping.rate = rate;
                        responseCartList.shipping = shipping;
                        // Calculate subtotal
                        responseCartList.display_sub_total = ConvertPrice(responseCartList.sub_total);
                        // Calculte display subtotal
                        responseCartList.display_total_price = ConvertPrice(responseCartList.total_price.Value);
                        //cart.display_final = TotalPrice.ToString();
                        //cart.display_final_price = cart.display_final + shipping;
                        #endregion

                        responseCartList.data.Add(cart);
                    }

                    return Ok(new { error = false, data = responseCartList, message = "Orders fetch successfully.",  state = "order", code = 200, status = true });
                }
                else
                {
                    return Ok(new { error = false, data = responseCartList, message = "Orders fetch successfully.", state = "order", code = 200, status = true });
                }
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Cart Count Api
        [Authorize]
        [Route("/cart/count")]
        [HttpPost]
        public IActionResult CartCount()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var Id = authuser.Id;
                var count = _cartService.Count(Id);

                return Ok(new { error = false, data = count, message = "Cart count fetch successfully.", code = 200, status = true });
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
