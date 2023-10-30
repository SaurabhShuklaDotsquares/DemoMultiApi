using EC.API.ViewModels.SiteKey;
using EC.API.ViewModels;
using EC.Data.Models;
using EC.Service;
using EC.Service.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ToDo.WebApi.Models;
using System.Text.Json;
using System.Text;
using Stripe;
using Stripe.Issuing;
using EC.Core.Enums;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using EC.Core;
using Microsoft.EntityFrameworkCore.Internal;
using NPOI.SS.Formula.Functions;
using System.Linq;
using EC.Service.Product;
using EC.Service.Taxs;
using System.IO;
using SixLabors.ImageSharp;
using EC.Service.ReturnRequest;
using EC.Data.Entities;
using Orders = EC.Data.Models.Orders;
using OrderItems = EC.Data.Models.OrderItems;
using System.Drawing;
using EC.Service.Currency_data;
using EC.Service.Payments;
using EC.Service.Shippings;
using System.Threading.Tasks;
using EC.Core.LIBS;
using System.Security.Policy;
using NPOI.SS.Formula.Eval;
using NPOI.POIFS.Crypt.Dsig;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : BaseAPIController
    {
        #region Constructor
        private IOrdersService _ordersService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        private readonly IProductAttributeImageService _productAttributeImageService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IProductService _productService;
        private readonly IproductImagesService _productImagesService;
        private readonly ITaxService _taxService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IReturnItemsService _returnItemsService;
        private ICurrenciesdataService _currenciesdataService;
        private IPaymentsService _paymentsService;
        private IReviewsService _reviewsService;
        private readonly IShippingService _shippingService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IUsersService _usersService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly ICountryService _CountryService;
        private readonly ICategoryService _categoryService;
        public OrdersController(IOrdersService ordersService, ICartService cartService, ICouponService couponService, IProductService productService, IproductImagesService iproductImagesService, ITaxService taxService, IReturnRequestService returnRequestService, IReturnItemsService returnItemsService, IProductAttributeDetailsService productAttributeDetailsService, IProductAttributeImageService productAttributeImageService, ICurrenciesdataService currenciesdataService, IPaymentsService paymentsService, IReviewsService reviewsService, IShippingService shippingService, ITemplateEmailService templateEmailService, IUsersService usersService, IEmailsTemplateService emailSenderService, ICountryService countryService, ICategoryService categoryService)
        {
            _ordersService = ordersService;
            _cartService = cartService;
            _couponService = couponService;
            _productService = productService;
            _productImagesService = iproductImagesService;
            _taxService = taxService;
            _returnRequestService = returnRequestService;
            _returnItemsService = returnItemsService;
            _productAttributeDetailsService = productAttributeDetailsService;
            _productAttributeImageService = productAttributeImageService;
            _currenciesdataService = currenciesdataService;
            _paymentsService = paymentsService;
            _reviewsService = reviewsService;
            _shippingService = shippingService;
            _templateEmailService = templateEmailService;
            _usersService = usersService;
            _emailSenderService = emailSenderService;
            _CountryService = countryService;
            _categoryService= categoryService;
        }
        #endregion

        #region Cancel Order Api
        [Authorize]
        [Route("/order/cancel-order")]
        [HttpPost]
        public IActionResult Cancellorder(Order_Id orderids)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                Orders order = _ordersService.GetByIdorderid(orderids.order_id);
                if (order != null)
                {
                    order.Status = ReturnOrderEnum.Cancelled.ToString();
                    order.UpdatedAt = DateTime.Now;
                    _ordersService.Update(order);

                    // Return cancel order response
                    CancelOrderModel model = new CancelOrderModel();
                    model.id = order.Id;
                    model.order_id = order.OrderId;
                    model.user_id = order.UserId;
                    model.seller_id = order.SellerId;
                    model.firstname = order.Firstname;
                    model.lastname = order.Lastname;
                    model.email = !string.IsNullOrEmpty(order.Email) ? order.Email : order.User.Email;
                    model.mobile = order.Mobile;

                    #region Get Billing Address
                    var billingaddress = JsonSerializer.Deserialize<CancleOrderBillingAddress>(order.BillingAddress);
                    if (billingaddress != null)
                    {
                        CancleOrderBillingAddress billingAddress = new CancleOrderBillingAddress();
                        billingAddress.address1 = billingaddress.address1;
                        billingAddress.address2 = billingaddress.address2;
                        billingAddress.state = billingaddress.state;
                        billingAddress.postal_code = billingaddress.postal_code;
                        billingAddress.city = billingaddress.city;
                        billingAddress.country = billingaddress.country;
                        model.billing_address = billingAddress;
                    }
                    #endregion

                    #region Get Shipping Address
                    var shippingaddress = JsonSerializer.Deserialize<CancleOrderShippingAddress>(order.ShipingAddress);
                    if (shippingaddress != null)
                    {
                        CancleOrderShippingAddress shippingAddress = new CancleOrderShippingAddress();
                        shippingAddress.address1 = shippingaddress.address1;
                        shippingAddress.address2 = shippingaddress.address2;
                        shippingAddress.state = shippingaddress.state;
                        shippingAddress.postal_code = shippingaddress.postal_code;
                        shippingAddress.city = shippingaddress.city;
                        shippingAddress.country = shippingaddress.country;
                        model.shipping_address = shippingAddress;
                    }
                    #endregion

                    model.voucher_code = order.VoucherCode;
                    model.discount_amount = order.DiscountAmount;
                    model.amount = order.Amount;
                    model.shipping_type = order.ShippingType;
                    model.shipping_amount = order.ShippingAmount;

                    #region Get Currency
                    var currencyId = HttpContext.Session.GetString("currencyId") != null ? HttpContext.Session.GetString("currencyId") : SiteKey.DefaultCurrency;
                    var targetCurrenciesData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                    if (targetCurrenciesData != null)
                    {
                        CurrencyDataModels currencyDataModels = new CurrencyDataModels();
                        currencyDataModels.id = targetCurrenciesData.Id;
                        currencyDataModels.is_primary = Convert.ToInt32(targetCurrenciesData.IsPrimary);
                        currencyDataModels.live_rate = targetCurrenciesData.LiveRate;
                        currencyDataModels.converted_rate = targetCurrenciesData.ConvertedRate;

                        if (targetCurrenciesData.Currency != null)
                        {
                            CurrencyModels currencyModels = new CurrencyModels();
                            currencyModels.id = targetCurrenciesData.Currency.Id;
                            currencyModels.iso = targetCurrenciesData.Currency.Iso;
                            currencyModels.name = targetCurrenciesData.Currency.Name;
                            currencyModels.symbol = targetCurrenciesData.Currency.Symbol;
                            currencyModels.symbol_native = targetCurrenciesData.Currency.SymbolNative;
                            currencyDataModels.currency = currencyModels;
                        }
                    }
                    #endregion

                    model.admin_commission = order.AdminCommission;
                    model.seller_commission = order.SellerCommission;
                    model.expected_delivery_date = null;
                    model.status = order.Status;
                    model.message = order.Message;
                    model.transaction_id = order.TransactionId;
                    model.payment_method = order.PaymentMethod;
                    model.transaction_response = order.TransactionResponse;
                    model.created_at = order.CreatedAt;
                    model.updated_at = order.UpdatedAt;
                    model.display_amount = ConvertPrice(order.Amount);
                    model.display_discount_amount = order.DiscountAmount != null && order.DiscountAmount != 0 ? ConvertPrice(order.DiscountAmount.Value) : ConvertPrice(0);
                    decimal shippingAmount = order.ShippingAmount != null && order.ShippingAmount != 0 ? order.ShippingAmount.Value : 0;
                    model.display_shipping_amount = ConvertPrice(shippingAmount);
                    decimal displayTotal = order.Amount - shippingAmount;
                    model.display_total = ConvertPrice(displayTotal);

                    return Ok(new { error = false, data = model, message = "Your Order has been successfully cancelled.", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Not found.", data = "null", code = 400, status = false };
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

        #region Order Product
        [Authorize]
        [Route("/order/product")]
        [HttpPost]
        public IActionResult OrderProduct(Order_products ordrprodut)
        {
            try
            {
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                ///List<cartdata> modela = new List<cartdata>();
                ///
                productsOrder orderproduts = new productsOrder();
                var produtorderdata = _ordersService.GetByproduts(ordrprodut.order_id, ordrprodut.product_id);
                if (produtorderdata != null)
                {
                    orderproduts.id = produtorderdata.Product.Id;
                    orderproduts.title = produtorderdata.Product.Title;

                    ///Get  Review 
                    var reviewdata = _reviewsService.GetAllReviewsList(ordrprodut.order_id, ordrprodut.product_id, id);
                    if (reviewdata != null)
                    {
                        foreach (var item in reviewdata)
                        {
                            reviewsall allreview = new reviewsall();
                            orderproduts.reviews_all.Add(allreview);
                        }
                    }

                    ///Get Product Image
                    var proimgdata = _productImagesService.GetByproductId(produtorderdata.ProductId);
                    if (proimgdata != null)
                    {
                        foreach (var item in proimgdata)
                        {
                            productimage imagedata = new productimage();
                            imagedata.id = item.Id;
                            imagedata.product_id = item.ProductId;
                            imagedata.image_name = item.ImageName;
                            imagedata.image_link = item.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + item.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            orderproduts.product_image.Add(imagedata);
                        }
                    }
                    return Ok(new { error = false, data = orderproduts, messege = "Order product fetch successfully!", state = "order", code = 200, status = true });
                }

                var errorData = new { error = true, message = "Order produt Record Not found.", data = "null", code = 400, status = false };
                return new UnauthorizedResponse(errorData);
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Add New Place Order Api
        [Authorize]
        [Route("/order/place-order")]
        [HttpPost]
        public IActionResult Addplaceaorder(OrdersViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (model.token == null)
                {
                    if (model.payment_method == "online")
                    {
                        var errorData = new { error = true, message = "Please fiil token.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                }
                decimal totalAmount = 0;
                var dataorder = _ordersService.GetBylastrecord() != null ? _ordersService.GetBylastrecord() : null;
                var dt = dataorder != null ? Convert.ToInt32(dataorder.OrderId.Split('-')[0]) + 1 : 1;
                Random random = new Random();
                const string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var builder = new StringBuilder();

                for (var i = 0; i < 7; i++)
                {
                    var c = pool[random.Next(0, pool.Length)];
                    builder.Append(c);
                }
                Orders entitycarts = new Orders();
                DateTime currentTime = DateTime.UtcNow;
                long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
                var randomdata = dt.ToString() + '-' + unixTime.ToString() + '-' + builder;
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var emaildata=_usersService.GetById(id);
                var cartdt = _cartService.GetByIdCart(id);
                
                if (cartdt != null && cartdt.Any())
                {
                    foreach (var item in cartdt)
                    {
                        var cate_getproduct_tbl = _productService.GetById(item.ProductId);
                        var categorydata = _categoryService.GetById(cate_getproduct_tbl.CategoryId);

                        entitycarts.AdminCommission = Convert.ToDouble(categorydata.AdminCommission);
                    }
                }

                List<Order_Id> order_Ids = new List<Order_Id>();
                Order_Id order1 = new Order_Id();
                order1.order_id = randomdata.ToString();
                order_Ids.Add(order1);
                shippingAddress address = new shippingAddress();
                address.address = model.shipping_address.address.ToString();
                address.address2 = model.shipping_address.address2.ToString();
                address.city = model.shipping_address.city.ToString();
                address.state = model.shipping_address.state.ToString();
                address.country = model.shipping_address.country;
                address.postal_code = model.shipping_address.postal_code;
                var jsonString = JsonSerializer.Serialize(address);
                billingAddress b_address = new billingAddress();

                b_address.address = model.billing_address.address.ToString();
                b_address.address2 = model.billing_address.address2.ToString();
                b_address.city = model.billing_address.city.ToString();
                b_address.state = model.billing_address.state.ToString();
                b_address.country = model.billing_address.country;
                b_address.postal_code = model.billing_address.postal_code;
                var bjsonString = JsonSerializer.Serialize(b_address);

                entitycarts.UserId = id;
                entitycarts.OrderId = randomdata;
                entitycarts.Firstname = model.firstname;
                entitycarts.Lastname = model.lastname;
                entitycarts.Mobile = model.mobile.ToString();
                entitycarts.Email = emaildata.Email;
                entitycarts.Amount = model.price;
                entitycarts.PaymentMethod = model.payment_method;
                entitycarts.TransactionId = model.token;
                entitycarts.ShipingAddress = jsonString;
                entitycarts.BillingAddress = bjsonString;
                entitycarts.PaymentMethod = model.payment_method.ToString();
                entitycarts.ShippingType = model.shipping_type;
                entitycarts.ShippingAmount = Convert.ToDecimal(model.shipping_amount);
               
                if (model.payment_method == "cod")
                {
                    entitycarts.Status = "processing";
                }
                else
                {
                    entitycarts.Status = "processing";
                }
                entitycarts.CreatedAt = DateTime.Now;
                entitycarts.UpdatedAt = DateTime.Now;

                if (cartdt != null && cartdt.Any())
                {
                    entitycarts.Amount = Convert.ToDecimal(cartdt.Sum(x => x.FinalValue));
                    var Shipping = _shippingService.GetShippingRates(entitycarts.Amount);
                    entitycarts.ShippingAmount = Shipping != null ? Shipping.ShippingCharge : 0;
                    foreach (var item in cartdt)
                    {
                        entitycarts.VendorId = item.SellerId;
                    }
                }
                // if i get promo code
                if (!string.IsNullOrEmpty(model.promocode))
                {
                    var coupon = _couponService.GetByCoupons(model.promocode);
                    if (coupon != null)
                    {
                        VoucherRedemptions entityVoucherRedemptions = new VoucherRedemptions();
                        entityVoucherRedemptions.RedeemerId = id;
                        entityVoucherRedemptions.VoucherId = coupon.Id;
                        entityVoucherRedemptions.Code = coupon.Code;
                        entityVoucherRedemptions.CreatedAt = DateTime.Now;
                        entityVoucherRedemptions.UpdatedAt = DateTime.Now;
                        if (coupon.Type == "fixed")
                        {
                            entitycarts.DiscountAmount = coupon.MaximumValue;
                            entitycarts.Amount = entitycarts.Amount - entitycarts.DiscountAmount.Value;
                            entitycarts.VoucherCode = coupon.Code;
                        }
                        else
                        {
                            entitycarts.DiscountAmount = (entitycarts.Amount * coupon.MaximumValue) / 100;
                            entitycarts.Amount = entitycarts.Amount - entitycarts.DiscountAmount.Value;
                            entitycarts.VoucherCode = coupon.Code;
                        }
                        _couponService.SaveVaucher(entityVoucherRedemptions);
                    }
                }
                var entity = _ordersService.Save(entitycarts);

                if (entity != null)
                {
                    var cartdata = _cartService.GetByIdCart(id);
                    if (cartdata != null && cartdata.Any())
                    {
                        foreach (var item in cartdata)
                        {
                            OrderItems entityitems = new OrderItems();
                            entityitems.OrderId = entity.Id;
                            entityitems.ProductId = item.ProductId;
                            entityitems.VendorId = item.SellerId;
                            entityitems.VariantId = item.VariantId.ToString() != null ? item.VariantId.ToString() : "";
                            entityitems.VariantSlug = item.VariantSlug != null ? item.VariantSlug : "";
                            entityitems.Quantity = item.Quantity;
                            entityitems.Price = item.FinalValue ?? 0;
                            var enttity = _ordersService.Saveitem(entityitems);

                            // Less completed stock in product stock
                            if ((item.ProductId != null && item.ProductId > 0) && (item.VariantId != null && item.VariantId > 0))
                            {
                                var productAttributeDetail = _productAttributeDetailsService.GetBy_Id(item.VariantId.Value);
                                if (productAttributeDetail != null)
                                {
                                    productAttributeDetail.Stock = productAttributeDetail.Stock - item.Quantity;
                                    _productAttributeDetailsService.Update(productAttributeDetail);
                                }
                            }
                            else
                            {
                                var product = _productService.GetById(item.ProductId);
                                if (product != null)
                                {
                                    product.Stock = product.Stock - item.Quantity;
                                    _productService.Update(product);
                                }
                            }

                            if (enttity != null)
                            {
                                bool isDeleted = _cartService.Delete((item));
                            }
                            else
                            {
                                var errorData = new { error = true, message = "product not shipped", data = "null", code = 400, status = false };
                                return new UnauthorizedResponse(errorData);
                            }
                        }
                    }
                    else
                    {
                        var errorData = new { error = true, message = "user not added to cart.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                }
                if (model.token != null)
                {
                    string Order_Id = "";
                    decimal Total_Price = 0;
                    decimal Dilevery_charge = 0;
                    StripeConfiguration.ApiKey = SiteKey.StripeKeys;
                    var options = new ChargeCreateOptions
                    {
                        Amount = Convert.ToInt16(entitycarts.Amount) * 100,
                        Currency = "usd",
                        Source = model.token,
                        Description = (Order_Id = randomdata, Total_Price = (entitycarts.Amount + Convert.ToDecimal(entitycarts.ShippingAmount)), Dilevery_charge = (Convert.ToDecimal(entitycarts.ShippingAmount))).ToString(),
                    };
                    var service = new ChargeService();
                    var paymentdata = service.Create(options);
                    if (paymentdata.Status.ToLower() == ReturnOrderEnum.Succeeded.GetName().ToLower())
                    {
                        entity.Status = "processing";
                        entity.TransactionId = paymentdata.Id;
                        _ordersService.Update(entity);
                    }
                    #region Payment transaction code
                    if (model.payment_method.ToLower() == "online")
                    {
                        EC.Data.Models.Payment entityPayment = new EC.Data.Models.Payment();
                        entityPayment.OrderId = entity.Id;
                        entityPayment.UserId = id;
                        entityPayment.Status = true;
                        entityPayment.TransactionId = paymentdata.Id;
                        entityPayment.MethodType = "online";
                        entityPayment.PaymentStatus = "Paid";
                        entityPayment.CurrencyCode = "USD";
                        entityPayment.Amount = entitycarts.Amount;
                        entityPayment.CreatedAt = DateTime.Now;
                        entityPayment.UpdatedAt = DateTime.Now;
                        entityPayment = _paymentsService.SavePayment(entityPayment);
                    }
                    #endregion
                    else if (paymentdata.Status == "Canceled")
                    {
                        var data = _ordersService.GetByItemsorderId(Convert.ToInt16(entity.Id));
                        Carts entitycartitems = new Carts();
                        entitycartitems.UserId = id;
                        entitycartitems.ProductId = data.ProductId;
                        entitycartitems.Quantity = data.Quantity;
                        entitycartitems.VariantId = Convert.ToInt32(data.VariantId);
                        entitycartitems.VariantSlug = data.VariantSlug;
                        entitycartitems.FinalValue = data.Price;
                        entitycartitems.CreatedAt = DateTime.Now;
                        entitycartitems.UpdatedAt = DateTime.Now;
                        _cartService.Save(entitycartitems);

                        if (randomdata != null)
                        {
                            var dlt = _ordersService.GetByItemsorderId(Convert.ToInt16(randomdata));
                            if (dlt != null)
                            {
                                bool isDeleted = _ordersService.Delete((Convert.ToInt16(dlt.Id)));
                                if (isDeleted == true)
                                {
                                    var errorData = new { error = true, message = "This product payment faild", data = "null", code = 400, status = false };
                                    return new UnauthorizedResponse(errorData);
                                }
                            }
                        }
                    }
                }

                #region Send Order Mail
                sendEmailVerificationEmail(id, entity.OrderId);
                #endregion
                return Ok(new { error = false, data = order_Ids, message = "Order placed successfully!", code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = ex, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get All Orders By UserId List Api
        [Authorize]
        [Route("/order")]
        [HttpGet]
        public IActionResult GetAllOrder([FromQuery] ToDoSearchSpecs paging)
        {
            try
            {
                OrderList1 orderLists = new OrderList1();
                List<OrderModel> orderListModel = new List<OrderModel>();
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                var orderList = _ordersService.GetOrderByUserId(userId, paging);

                if (orderList != null && orderList.Any())
                {
                    var PageMetadate = new
                    {
                        orderList.CurrentPage,
                        orderList.PazeSize,
                        orderList.TotalPage,
                        orderList.TotalCount,
                        orderList.HasNext,
                        orderList.HasPrev
                    };
                    Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));

                    foreach (var item in orderList)
                    {
                        // Order Data
                        OrderModel orderModel = new OrderModel();
                        orderModel.id = Convert.ToInt32(item.Id);
                        orderModel.order_id = item.OrderId;
                        orderModel.shipping_amount = item.ShippingAmount != 0 && item.ShippingAmount != null ? item.ShippingAmount.Value : 0;
                        orderModel.discount_amount = item.DiscountAmount != 0 && item.DiscountAmount != null ? item.DiscountAmount.Value.ToString() : "0.00";
                        orderModel.amount = item.Amount;
                        orderModel.transaction_id = item.TransactionId;
                        orderModel.transaction_response = item.TransactionResponse;
                        orderModel.status = item.Status;
                        // Calculate order display amount
                        decimal orderDisplayAmount = item.ShippingAmount != null ? item.Amount + item.ShippingAmount.Value : item.Amount;
                        orderModel.display_amount = ConvertPrice(orderDisplayAmount);
                        // Convert order display discount amount
                        decimal orderDisplayDiscountAmount = item.DiscountAmount != null ? item.DiscountAmount.Value : 0;
                        orderModel.display_discount_amount = ConvertPrice(orderDisplayDiscountAmount);
                        // Convert display shipping amount
                        decimal orderShippingAmount = item.ShippingAmount != null ? item.ShippingAmount.Value : 0;
                        orderModel.display_shipping_amount = ConvertPrice(orderShippingAmount);
                        orderModel.display_status = item.Status;
                        orderModel.created_at = item.CreatedAt.Value;
                        orderModel.sum_ammout_return = 0;

                        // Order Item Data
                        if (item.OrderItems != null && item.OrderItems.Any())
                        {
                            foreach (var orderitem in item.OrderItems)
                            {
                                OrderitemModel orderItemModel = new OrderitemModel();
                                orderItemModel.id = Convert.ToInt32(orderitem.Id);
                                orderItemModel.seller_id = orderitem.SellerId != null && orderitem.SellerId != 0 ? orderitem.SellerId.Value : 0;
                                orderItemModel.product_id = orderitem.ProductId;
                                orderItemModel.variant_id = orderitem.VariantId;
                                orderItemModel.variant_slug = orderitem.VariantSlug;
                                orderItemModel.quantity = orderitem.Quantity;
                                orderItemModel.tax = orderitem.Tax;
                                orderItemModel.admin_commission = 0;
                                orderItemModel.is_review = 0;
                                // Calculate order item display price
                                decimal orderitemDisplayPrice = orderitem.Price;
                                orderItemModel.display_price = ConvertPrice(orderitemDisplayPrice);
                                // Calculate order item display total price
                                decimal orderitemDisplayTotalPrice = orderitem.Price;
                                orderItemModel.display_total_price = ConvertPrice(orderitemDisplayTotalPrice);

                                // Product Data
                                var product = _productService.GetById(orderitem.ProductId);
                                if (product != null)
                                {
                                    ProductsModel productModel = new ProductsModel();
                                    productModel.id = product.Id;
                                    productModel.title = product.Title;
                                    productModel.sku = product.Sku;
                                    productModel.slug = product.Slug;
                                    productModel.brand_name = product.BrandName != null && product.BrandName != 0 ? product.BrandName.Value : 0;
                                    productModel.prod_description = product.LongDescription;
                                    productModel.average_rating = product.Rating;
                                    productModel.display_price = product.DiscountedPrice != null && product.DiscountedPrice != 0 ? product.DiscountedPrice.Value : product.Price != null && product.Price != 0 ? product.Price.Value : 0;
                                    productModel.display_discounted_price = product.DiscountedPrice != null && product.DiscountedPrice != 0 ? product.DiscountedPrice.Value : product.Price != null && product.Price != 0 ? product.Price.Value : 0;

                                    // Convert display total price
                                    decimal displayTotal = (product.DiscountedPrice != null && product.DiscountedPrice != 0 ? product.DiscountedPrice.Value : product.Price != null && product.Price != 0 ? product.Price.Value : 0);
                                    orderModel.display_total = ConvertPrice(displayTotal);
                                    decimal totalPrice = product.DiscountedPrice != null && product.DiscountedPrice != 0 ? product.DiscountedPrice.Value : product.Price != null && product.Price != 0 ? product.Price.Value : 0;
                                    orderModel.sum_amount += totalPrice;
                                    orderItemModel.product = productModel;

                                    // Product Image Data
                                    if (product.ProductImages != null && product.ProductImages.Any())
                                    {
                                        foreach (var productImage in product.ProductImages)
                                        {
                                            ProductImageModel productImageModel = new ProductImageModel();
                                            productImageModel.id = productImage.Id;
                                            productImageModel.product_id = productImage.ProductId;
                                            productImageModel.image_name = productImage.ImageName != null ? productImage.ImageName : SiteKey.DefaultImage;
                                            productImageModel.image_link = productImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + productImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                            productModel.product_image.Add(productImageModel);
                                        }
                                    }
                                }

                                orderItemModel.returnrequest = null;
                                orderModel.orderitems.Add(orderItemModel);
                            }
                        }

                        orderListModel.Add(orderModel);

                        orderLists.data = orderListModel;
                        orderLists.current_page = orderList.CurrentPage;
                        orderLists.total_page = orderList.TotalPage;
                        orderLists.page_size = orderList.PazeSize;
                        // orderLists.Add(orderListModel);
                    }
                    return Ok(new { error = false, message = "Orders fetch successfully.", data = orderLists, code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Order List Not Found.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get All Order Items List By Order Id Api
        [Authorize]
        [Route("/order/{order_id}")]
        [HttpGet]
        public IActionResult GetOrderDetailByOrderId(string order_id)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                OrderDetail1 orderDeta = new OrderDetail1();
                OrderDetail orderDetailListModel = new OrderDetail();
                var orderDetail = _ordersService.GetByItemId(order_id, userId);

                if (orderDetail != null)
                {
                    OrderDetail model = new OrderDetail();
                    model.id = Convert.ToInt16(orderDetail.Id);
                    model.user_id = orderDetail.UserId;
                    model.order_id = orderDetail.OrderId;
                    model.firstname = $"{orderDetail.Firstname}";
                    model.lastname = $"{orderDetail.Lastname}";
                    model.mobile = !string.IsNullOrEmpty(orderDetail.Mobile) ? orderDetail.Mobile : string.Empty;
                    model.email = !string.IsNullOrEmpty(orderDetail.Email) ? orderDetail.Email : orderDetail.User != null ? orderDetail.User.Email : string.Empty;

                    #region Get Shipping address
                    if (!string.IsNullOrEmpty(orderDetail.ShipingAddress))
                    {
                        var orderDetailShipping = JsonSerializer.Deserialize<ShippingAddress>(orderDetail.ShipingAddress);
                        model.shipping_address = new ShippingAddress();
                        model.shipping_address.address = orderDetailShipping.address;
                        model.shipping_address.address2 = orderDetailShipping.address2;
                        model.shipping_address.country = orderDetailShipping.country;
                        model.shipping_address.state = orderDetailShipping.state;
                        var countryShipingname = _CountryService.GetById(Convert.ToInt32(orderDetailShipping.country));
                        if (countryShipingname != null)
                        {
                            model.shipping_address.countryName = countryShipingname.Name;
                        }
                        model.shipping_address.city = orderDetailShipping.city;
                        model.shipping_address.postal_code = orderDetailShipping.postal_code;
                    }
                    #endregion

                    #region Get Billing Address
                    if (!string.IsNullOrEmpty(orderDetail.BillingAddress))
                    {
                        var orderDetailBilling = JsonSerializer.Deserialize<BillingAddress>(orderDetail.BillingAddress);
                        model.billing_address = new BillingAddress();
                        model.billing_address.address = orderDetailBilling.address;
                        model.billing_address.address2 = orderDetailBilling.address2;
                        model.billing_address.country = orderDetailBilling.country;
                        model.billing_address.state = orderDetailBilling.state;

                        var countryBillingname = _CountryService.GetById(Convert.ToInt32(orderDetailBilling.country));
                        if (countryBillingname != null)
                        {
                            model.billing_address.countryName = countryBillingname.Name.ToString();
                        }
                        model.billing_address.city = orderDetailBilling.city;
                        model.billing_address.postal_code = orderDetailBilling.postal_code;
                    }
                    #endregion

                    model.discount_amount = orderDetail.DiscountAmount.ToString();
                    model.amount = orderDetail.Amount.ToString();
                    model.shipping_type = orderDetail.ShippingType;
                    model.shipping_amount = orderDetail.ShippingAmount;
                    model.status = orderDetail.Status;
                    model.payment_method = orderDetail.PaymentMethod;
                    model.transaction_response = orderDetail.TransactionResponse;

                    #region Get Currency List
                    var currency = _currenciesdataService.GetCurrencyDataList();
                    if (currency != null && currency.Any())
                    {
                        foreach (var item in currency)
                        {
                            CurrencyViewModel currencyView = new CurrencyViewModel();
                            currencyView.id = Convert.ToInt32(item.Id);
                            currencyView.currency_id = item.CurrencyId;
                            if (item.Currency != null)
                            {
                                CurrencyModel currencyModel = new CurrencyModel();
                                currencyModel.id = item.Currency.Id;
                                currencyModel.iso = item.Currency.Iso;
                                currencyModel.name = item.Currency.Name;
                                currencyModel.symbol = item.Currency.Symbol;
                                currencyModel.symbol_native = item.Currency.SymbolNative;
                                currencyView.currency = currencyModel;
                            }
                            model.currency = currencyView;
                        }
                    }
                    #endregion

                    model.payment_payed_to_vendor = "0";
                    model.created_at = orderDetail.CreatedAt;
                    // Convert order display amount
                    decimal orderDisplayAmount = orderDetail.ShippingAmount != null ? (orderDetail.Amount + orderDetail.ShippingAmount.Value) : orderDetail.Amount;
                    model.display_amount = ConvertPrice(orderDisplayAmount);
                    // Convert order display discount amount
                    decimal orderDisplayDiscountAmount = orderDetail.DiscountAmount != null ? orderDetail.DiscountAmount.Value : 0;
                    model.display_discount_amount = ConvertPrice(orderDisplayDiscountAmount);
                    // Convert display shipping amount
                    decimal orderDisplayShippingAmount = orderDetail.ShippingAmount != null ? orderDetail.ShippingAmount.Value : 0;
                    model.display_shipping_amount = ConvertPrice(orderDisplayShippingAmount);
                    // Convert order display total amount
                    decimal orderDisplayTotal = orderDetail.Amount;
                    model.display_total = ConvertPrice(orderDisplayTotal);
                    model.display_status = orderDetail.Status;
                    model.sum_amount += orderDetail.Amount;
                    model.sum_ammout_return = 0;
                    orderDetailListModel = model;

                    #region Get Order Items
                    OrderitemModel orderitem1 = new OrderitemModel();
                    var orderitemDetail = _ordersService.GetByItemsorderId(Convert.ToInt16(orderDetail.Id));
                    if (orderDetail.OrderItems != null && orderDetail.OrderItems.Any())
                    {
                        foreach (var item in orderDetail.OrderItems)
                        {
                            OrderitemModel1 orderitem = new OrderitemModel1();
                            orderitem.id = Convert.ToInt16(item.Id);
                            orderitem.order_id = Convert.ToString(item.OrderId);
                            orderitem.seller_id = item.SellerId;
                            orderitem.product_id = item.ProductId;
                            orderitem.variant_id = item.VariantId;
                            orderitem.quantity = item.Quantity;
                            // Convert order item price
                            decimal orderitemPrice = item.Price / item.Quantity;
                            orderitem.price = ConvertPriceInDecimal(orderitemPrice);
                            orderitem.tax = item.Tax;
                            orderitem.admin_commission = 0;
                            orderitem.is_review = 0;
                            // Convert order item display price
                            decimal orderitemDisplayPrice = (item.Price / item.Quantity);
                            orderitem.display_price = ConvertPrice(orderitemDisplayPrice);
                            // Convert order item display total price
                            decimal orderitemDisplayTotalPrice = (item.Price / item.Quantity);
                            orderitem.display_total_price = ConvertPrice(orderitemDisplayTotalPrice);

                            decimal taxRateValue = 0;
                            decimal totalAmount = 0;
                            #region Get Products 
                            var product = _productService.GetById(item.ProductId);
                            if (product != null)
                            {
                                ProductDetail productmodel1 = new ProductDetail();
                                productmodel1.id = product.Id;
                                productmodel1.Title = product.Title;
                                productmodel1.sku = product.Sku;
                                productmodel1.slug = product.Slug;
                                productmodel1.brand_name = product.BrandName.ToString();
                                productmodel1.prod_description = product.LongDescription;
                                productmodel1.average_rating = product.Rating;
                                productmodel1.display_price = item.Price / item.Quantity;
                                productmodel1.display_discounted_price = item.Price / item.Quantity;
                                totalAmount = item.Price * item.Quantity;
                                //orderitem.product = productmodel1;

                                #region Product Images
                                var productImages = _productImagesService.GetByproductId(item.ProductId);
                                if (productImages != null && productImages.Any())
                                {
                                    foreach (var productImage in productImages)
                                    {
                                        ProductimageDetail productimages = new ProductimageDetail();
                                        productimages.id = productImage.Id;
                                        productimages.product_id = productImage.ProductId;
                                        productimages.image_name = productImage.ImageName != null ? productImage.ImageName : SiteKey.DefaultImage;
                                        productimages.image_link = productImage.ImageName != null ? SiteKey.ImagePath + "/Uploads/" + productImage.ImageName : SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                                        productmodel1.product_image.Add(productimages);
                                    }
                                }
                                //else
                                //{
                                //    productimages.image_name = SiteKey.DefaultImage;
                                //}
                                #endregion
                                orderitem.product = productmodel1;

                                if (orderDetail.Status == "completed")
                                {
                                    ReturnRequest returndata = new ReturnRequest();
                                    var data = _returnRequestService.GetByOrderId(Convert.ToInt32(orderDetail.Id));
                                    if (data != null)
                                    {
                                        returndata.id = Convert.ToInt32(data.Id);
                                        returndata.order_id = Convert.ToInt32(data.OrderId);
                                        returndata.user_id = data.UserId;
                                        returndata.amount = data.Amount;
                                        returndata.refund_amount = data.RefundAmount;
                                        returndata.bank_account_id = data.BankAccountId;
                                        returndata.message = data.Message;
                                        returndata.return_status = data.Status;
                                        returndata.created_at = data.CreatedAt;
                                        returndata.updated_at = data.UpdatedAt;
                                        orderitem.returnrequest = returndata;
                                    }
                                    else
                                    { 
                                        orderitem.returnrequest = null;
                                    }
                                    //else
                                    //{
                                    //    var errorData = new { error = true, message = "Return Request Record Not found.", data = "null", code = 400, status = false };
                                    //    return new UnauthorizedResponse(errorData);
                                    //}
                                }
                                

                                //orderDetailListModel.product_image.Add(productimages);
                                // Get Tax For TotalAmount
                                var taxRate = _taxService.GetTaxByCategoryId(product.CategoryId);
                                if (taxRate != null)
                                {
                                    taxRateValue = taxRate.Value != 0 ? taxRate.Value.Value : 0;
                                }
                                if (taxRateValue != 0)
                                {
                                    var productTax = totalAmount * taxRateValue / 100;
                                    // model.TotalAmount += totalAmount + productTax;
                                }
                                else
                                {
                                    //model.TotalAmount += totalAmount;
                                }

                            }
                            #endregion
                            model.orderitems.Add(orderitem);
                        }
                        orderDetailListModel = model;
                    }
                    #endregion
                    //if (orderDetail.Status == "completed")
                    //{
                    #region Get Payment
                    Payament paydata = new Payament();
                    var paymentdata = _paymentsService.GetByPaymentsOrderId(Convert.ToInt16(orderDetail.Id));
                    if (paymentdata != null)
                    {
                        paydata.id = Convert.ToInt16(paymentdata.Id);
                        paydata.order_id = Convert.ToInt16(paymentdata.OrderId);
                        paydata.user_id = Convert.ToInt16(paymentdata.UserId);
                        paydata.vendor_id = 0;
                        paydata.status = Convert.ToInt32(paymentdata.Status);
                        paydata.transaction_id = paymentdata.TransactionId;
                        paydata.method_type = paymentdata.MethodType;
                        paydata.payment_status = paymentdata.PaymentStatus;
                        paydata.currency_code = paymentdata.CurrencyCode;
                        paydata.amount = (paymentdata.Amount).ToString();
                        paydata.created_at = paymentdata.CreatedAt;
                        paydata.updated_at = paymentdata.UpdatedAt;
                    }
                    model.payment = paydata;
                    //}
                    #endregion
                    orderDeta.order = orderDetailListModel;
                    return Ok(new { error = false, data = orderDeta, message = "Order Detail Record Fatch Successfully.", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Order Detail Record Not found.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Return Order Api
        [Authorize]
        [Route("/order/return")]
        [HttpPost]
        public IActionResult ReturnOrder(ReturnOrderModel model)
        {
            try
            {
                bool isReturnCancel = false;
                List<OrderDetail> orderDetailListModel = new List<OrderDetail>();
                string Message = string.Empty;
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                decimal totalAmount = 0;

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (model.item == null)
                {
                    var errorData = new { error = true, message = "Items Required.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                if (model.item.Where(x => x == 0).Count() > 0)
                {
                    var errorData = new { error = true, message = "Items Required.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                if (model.order_id <= 0)
                {
                    var errorData = new { error = true, message = "Order Id Required.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }

                var orderDetail = _ordersService.GetById(model.order_id);
                if (orderDetail != null && orderDetail.OrderItems.Any())
                {
                    ReturnRequests entityReturnRequests = new ReturnRequests();
                    entityReturnRequests.OrderId = orderDetail.Id;
                    entityReturnRequests.UserId = userId;

                    entityReturnRequests.CreatedAt = DateTime.Now;

                    foreach (var item in model.item)
                    {
                        var returnItems = _returnItemsService.GetByOrderItemId(item);
                        //if(returnItems != null)
                        //{
                        //    var errorData = new { error = true, message = "Order Item Already Returned.",  code = 400, status = false };
                        //    return new UnauthorizedResponse(errorData);
                        //    //return Ok(new { error = true, message = "Order Item Already Returned.", code = 400, status = false });
                        //}
                        var orderItems = orderDetail.OrderItems.Where(x => x.Id == item).FirstOrDefault();
                        if (orderItems != null)
                        {
                            if (model.return_quantity > orderItems.Quantity)
                            {
                                var error = new { error = true, message = "Quantity should be less than.", code = 400, status = false };
                                return new UnauthorizedResponse(error);
                            }
                            ReturnItems entityReturnItems = new ReturnItems();
                            entityReturnItems.OrderItemId = orderItems.Id;
                            entityReturnItems.ReturnQuantity = model.return_quantity.ToString();
                            if (model.status == "cancel")
                            {
                                isReturnCancel = true;
                                entityReturnRequests.Id = returnItems.RequestId.Value;
                                entityReturnRequests.Status = ReturnOrderEnum.Declined.GetDescription();
                                entityReturnItems.ReturnStatus = ReturnOrderEnum.Declined.GetDescription();
                                Message = "Return request cancelled.";
                                if (returnItems != null)
                                {
                                    _returnRequestService.DeleteReturnItems(returnItems);
                                }
                            }
                            else
                            {
                                entityReturnRequests.Status = ReturnOrderEnum.New.GetDescription();
                                entityReturnItems.ReturnStatus = ReturnOrderEnum.New.GetDescription();
                                Message = "Your order return request generated successfully.";
                            }
                            entityReturnItems.CreatedAt = DateTime.Now;
                            entityReturnRequests.ReturnItems.Add(entityReturnItems);
                            totalAmount += orderItems.Price;
                        }
                    }
                    var responseentity = !isReturnCancel ? _returnRequestService.SaveReturnRequests(entityReturnRequests) : _returnRequestService.UpdateReturnRequests(entityReturnRequests);
                    if (responseentity != null)
                    {
                        responseentity.Amount = totalAmount.ToString();
                        responseentity.RefundAmount = totalAmount.ToString();
                        responseentity.UpdatedAt = DateTime.Now;
                        var response = _returnRequestService.UpdateReturnRequests(responseentity);
                    }
                    return Ok(new { error = false, data = "", message = Message, code = 200, status = true });
                    //var errorData = new { error = true,  message = "Your Order has been return successfully.", code = 200, status = true };
                    //return new UnauthorizedResponse(errorData);
                }
                else
                {
                    var errorData = new { error = true, message = "Order Not Found.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Send Email
        private bool sendEmailVerificationEmail(int id, string orderId)
        {
            bool isSendEmail = false;
            Data.Models.Emails emailTemplate = new Data.Models.Emails();
            var order = _ordersService.GetByIdorderid(orderId);
            if (order != null)
            {
                emailTemplate = _templateEmailService.GetById((int)EmailType.ThankYouForOrder);
                if (emailTemplate != null)
                {
                    string urlToClick = "";
                    var url = "";
                    var clickme = "";
                    var user = _usersService.GetById(id);
                    var Name = user != null ? user.Firstname + ' ' + user.Lastname : string.Empty;
                    url = emailTemplate.Description;
                    string productList = string.Empty;
                    string ShippingAddressInfomation = string.Empty;

                    IDictionary<string, string> d = new Dictionary<string, string>();
                    d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKey.ImagePath + "/Uploads/" + "/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100' style='height:100px; width:100px;'>"));
                    d.Add(new KeyValuePair<string, string>("##UserName##", Name));
                    d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                    d.Add(new KeyValuePair<string, string>("##CurrentDate##", DateTime.Now.ToString()));
                    d.Add(new KeyValuePair<string, string>("##OrderId##", order.OrderId));
                    d.Add(new KeyValuePair<string, string>("##OrderDate##", order.CreatedAt != null ? order.CreatedAt.Value.ToString("dd/MM/yyyy") : order.UpdatedAt.Value.ToString("dd/MM/yyyy")));
                    if (order.OrderItems != null && order.OrderItems.Any())
                    {
                        int i = 0;
                        foreach (var item in order.OrderItems)
                        {
                            var product = _productService.GetById(item.ProductId);
                            var productImage = _productImagesService.GetById(item.ProductId);
                            productList += "<html><head></head><body><div style='border: 5px solid #D3D3D3; height:150px; width:500px;margin:auto;'><div style='height: 150px; width:150px;float:left;'><div><img src='" + SiteKey.ImagePath + "/Uploads/" + productImage.ImageName + "' alt='ProductImage' height='150' width='150' style='height:150px; width:150px;'></div></div><div style='background-color:#F1F1F1; height: 150px; width:350px;float:left;'><div style='padding-left:50px;font-weight:bold;'><table><tr><td>Title :</td><td>" + product.Title + "</td></tr><tr><td>Quantity :</td><td>" + item.Quantity + "</td></tr><tr><td>Amount :</td><td>"+item.Price+"</td></tr><tr><td>Price :</td><td>" + (item.Price*item.Quantity) + "</td></tr></table></div></div></div></body></html>";
                            productList += order.OrderItems.Count() - 1 == i ? string.Empty : "<br>";
                            i++;
                        }
                    }
                    #region Get Shipping address
                    if (!string.IsNullOrEmpty(order.ShipingAddress))
                    {
                        var shippingaddress = JsonSerializer.Deserialize<ShippingAddress>(order.ShipingAddress);
                        ShippingAddressInfomation = "<div style='float:left;'><label'>Shipping Information :</label><br><label>" + shippingaddress.address2 + "</label><br><label>" + shippingaddress.city + "</label><br><label>" + shippingaddress.state + "</label><br><label>" + shippingaddress.postal_code + "</label></div>";
                    }
                    #endregion

                    #region Get Billing Address
                    if (!string.IsNullOrEmpty(order.BillingAddress))
                    {
                        var billingaddress = JsonSerializer.Deserialize<BillingAddress>(order.BillingAddress);
                        ShippingAddressInfomation += "<div style='float:right;'><label'>Billing Information :</label><br><label>" + billingaddress.address2 + "</label><br><label>" + billingaddress.city + "</label><br><label>" + billingaddress.state + "</label><br><label>" + billingaddress.postal_code + "</label></div>";
                    }
                    #endregion
                    decimal shippingAmount = order.ShippingAmount != null ? order.ShippingAmount.Value : 0;
                    decimal totalAmount = order.Amount + shippingAmount;
                    decimal discountAmount = order.DiscountAmount != null && order.DiscountAmount != 0 ? order.DiscountAmount.Value : 0;
                    string amountSection = "<div style='height:50px; width:500px;margin:auto;'><label style='float:left;'>Discount :</label><label style='float:right;'>$" + discountAmount + "</label></div>";
                    amountSection += "<div style='height:50px; width:500px;margin:auto;'><label style='float:left;'>Shipping :</label><label style='float:right;'>$" + shippingAmount + "</label></div>";
                    amountSection += "<div style='height:50px; width:500px;margin:auto;'><label style='float:left;'>Total :</label><label style='float:right;'>$" + totalAmount + "</label></div>";
                    string addressInformation = "<div style='height:150px; width:500px;margin:auto;'> " + ShippingAddressInfomation + "</div>";
                    d.Add(new KeyValuePair<string, string>("##ProductDetails##", productList));
                    d.Add(new KeyValuePair<string, string>("##AmountSection##", amountSection));
                    d.Add(new KeyValuePair<string, string>("##ShippingAddressInfomation##", addressInformation));
                    clickme = url;
                    foreach (KeyValuePair<string, string> ele in d)
                    {
                        clickme = clickme.Replace(ele.Key, ele.Value);
                    }
                    urlToClick = clickme;
                    var subject = emailTemplate.Subject.ToTitleCase();
                    _emailSenderService.SendEmailAsync(user.Email, subject, urlToClick);
                    isSendEmail = true;
                }
            }
            return isSendEmail;
        }
        #endregion

    }
}
