using EC.Service;
using EC.Service.Currency;
using EC.Service.Currency_data;
using EC.Service.Specification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Text.Json;
using EC.API.ViewModels.MultiVendor;
using NuGet.Protocol.Plugins;
using EC.Service.Taxs;
using Microsoft.AspNetCore.Authorization;
using ToDo.WebApi.Models;
using EC.Service.ReturnRequest;
using EC.Core.Enums;
using EC.Core;
using EC.Data.Models;
using Stripe;
using EC.Core.Stripe;
using EC.API.ViewModels;
using CurrencyModel = EC.API.ViewModels.MultiVendor.CurrencyModel;
using System.IO;
using EC.API.ViewModels.SiteKey;
using SelectPdf;
using PdfDocument = SelectPdf.PdfDocument;
using PdfPageSize = SelectPdf.PdfPageSize;
using HtmlToPdf = SelectPdf.HtmlToPdf;
using PdfPageOrientation = SelectPdf.PdfPageOrientation;
using System.Text;
using EC.Core.LIBS;
using EC.Service.Payments;
using System.Collections.Generic;
using System.Linq;
using EC.Service.Product;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderVendorController : BaseAPIController
    {
        #region Constructor
        private readonly IOrdersService _ordersService;
        private readonly ICurrenciesdataService _currenciesdataService;
        private readonly IProductService  _productService;
        private readonly ITaxService  _taxService;
        private readonly IUsersService _usersService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ICountryService _CountryService;
        private IPaymentsService _paymentsService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IproductImagesService _productImagesService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly IReturnItemsService _itemreturnService;
        public OrderVendorController(IOrdersService ordersService, ICurrenciesdataService currenciesdataService, IProductService productService, ITaxService taxService, IUsersService usersService, IReturnRequestService returnRequestService, ICountryService countryService,IPaymentsService paymentsService, ITemplateEmailService templateEmailService, IproductImagesService productImagesService, IEmailsTemplateService emailSenderService, IReturnItemsService itemreturnService)
        {
            _ordersService = ordersService;
            _currenciesdataService = currenciesdataService;
            _productService = productService;
            _taxService = taxService;
            _usersService = usersService;
            _returnRequestService = returnRequestService;
            _CountryService = countryService;
            _paymentsService = paymentsService;
            _templateEmailService = templateEmailService;
            _productImagesService = productImagesService;
            _emailSenderService = emailSenderService;
            _itemreturnService = itemreturnService;
        }
        #endregion

        #region Get OrderList Api
        [Authorize]
        [HttpGet]
        [Route("/vendor/order/list")]
        public IActionResult GetOrderList([FromQuery] ToDoSearchSpecs model, [FromQuery] string created_at)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;
                OrderListReturnModel orderListReturnModel = new OrderListReturnModel(); 
                #region Get Order
                var orderList = _ordersService.GetAllOrderForVendor(model, userId, created_at);
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

                    foreach (var order in orderList)
                    {
                        #region Get Order Status
                        OrderStatusViewModel orderStatusViewModel = new OrderStatusViewModel();
                        orderStatusViewModel.processing = "Processing";
                        orderStatusViewModel.shipped = "Shipped";
                        orderStatusViewModel.completed = "Completed";
                        orderStatusViewModel.pending_for_payment = "Pending for Payment";
                        orderStatusViewModel.failed = "Failed";
                        orderStatusViewModel.returned = "Returned";
                        orderStatusViewModel.refunded = "Refunded";
                        orderStatusViewModel.cancelled = "Cancelled";
                        orderStatusViewModel.pending = "Cancelled";
                        orderListReturnModel.orderStatus.Add(orderStatusViewModel);
                        #endregion

                        #region Get Order Data
                        OrderViewModel orderViewModel= new OrderViewModel();
                        orderViewModel.id = Convert.ToInt32(order.Id);
                        orderViewModel.order_id = order.OrderId;
                        orderViewModel.user_id = order.UserId;
                        orderViewModel.vendor_id = (order.VendorId ?? 0);
                        orderViewModel.firstname = order.User.Firstname;
                        orderViewModel.lastname = order.User.Lastname;
                        orderViewModel.email = order.User.Email;
                        orderViewModel.mobile = order.User.Mobile;
                        orderViewModel.voucher_code = order.VoucherCode;
                        orderViewModel.discount_amount = order.DiscountAmount;
                        orderViewModel.amount = order.Amount;
                        orderViewModel.shipping_type = order.ShippingType;
                        orderViewModel.shipping_amount = order.ShippingAmount;
                        orderViewModel.admin_commission = (Convert.ToDouble(order.Amount) * (order.AdminCommission)) / 100;
                        orderViewModel.admin_commission = order.AdminCommission;
                        orderViewModel.seller_commission = order.SellerCommission;
                        orderViewModel.expected_delivery_date = null;
                        orderViewModel.status = order.Status;
                        orderViewModel.message = order.Message;
                        orderViewModel.transaction_id = order.TransactionId;
                        orderViewModel.payment_method = order.PaymentMethod;
                        orderViewModel.transaction_response = order.TransactionResponse;
                        orderViewModel.payment_payed_to_vendor = 0;
                        orderViewModel.created_at = order.CreatedAt;
                        orderViewModel.updated_at = order.UpdatedAt;
                        orderViewModel.discount_amount = order.Amount;
                        orderViewModel.display_discount_amount = order.DiscountAmount;
                        orderViewModel.display_shipping_amount = order.ShippingAmount;
                        orderViewModel.display_total = order.Amount;
                        var commision = order.Amount * Convert.ToDecimal(orderViewModel.admin_commission) / 100;
                        orderViewModel.display_amount = (order.Amount + (order.ShippingAmount ?? 0)+ commision);
                        orderViewModel.display_status = order.Status;
                        orderViewModel.sum_amount = order.Amount;
                        orderViewModel.sum_ammout_return = order.Amount;

                        #region Get Billing address
                        if (!string.IsNullOrEmpty(order.BillingAddress))
                        {
                            BillingVendorAddressModel billingAddressModel = new BillingVendorAddressModel();
                            var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(order.BillingAddress);
                            if (orderDetailBilling != null)
                            {
                                billingAddressModel.address = orderDetailBilling.address;
                                billingAddressModel.address2 = orderDetailBilling.address2;
                                billingAddressModel.state = orderDetailBilling.state;
                                billingAddressModel.postal_code = orderDetailBilling.postal_code;
                                billingAddressModel.city = orderDetailBilling.city;
                                billingAddressModel.country = orderDetailBilling.country;
                                orderViewModel.billing_address = billingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Shipping address
                        if (!string.IsNullOrEmpty(order.BillingAddress))
                        {
                            ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
                            var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(order.ShipingAddress);
                            if (orderDetailShipping != null)
                            {
                                shippingAddressModel.address = orderDetailShipping.address;
                                shippingAddressModel.address2 = orderDetailShipping.address2;
                                shippingAddressModel.state = orderDetailShipping.state;
                                shippingAddressModel.postal_code = orderDetailShipping.postal_code;
                                shippingAddressModel.city = orderDetailShipping.city;
                                shippingAddressModel.country = orderDetailShipping.country;
                                orderViewModel.shipping_address = shippingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Currency
                        string currencyId = HttpContext.Session.GetString("currencyId");
                        var currencyData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                        if (currencyData != null)
                        {
                            CurrencyDataModel currencyDataModel = new CurrencyDataModel();
                            currencyDataModel.id = Convert.ToInt32(currencyData.Id);
                            currencyDataModel.currency_id = currencyData.CurrencyId;
                            currencyDataModel.is_primary = Convert.ToInt32(currencyData.IsPrimary);
                            currencyDataModel.live_rate = Convert.ToInt32(currencyData.LiveRate);
                            currencyDataModel.converted_rate= Convert.ToInt32(currencyData.ConvertedRate);
                            if (currencyData.Currency != null)
                            {
                                CurrencyModel currencyModel = new CurrencyModel();
                                currencyModel.id = currencyData.Currency.Id;
                                currencyModel.iso = currencyData.Currency.Iso;
                                currencyModel.name = currencyData.Currency.Name;
                                currencyModel.symbol = currencyData.Currency.Symbol;
                                currencyModel.symbol_native = currencyData.Currency.SymbolNative;
                                currencyDataModel.currency = currencyModel;

                            }
                            orderViewModel.currency = currencyDataModel;
                        }
                        #endregion

                        #region Get Order Items
                        if (order.OrderItems != null && order.OrderItems.Any())
                        {
                            foreach (var orderItems in order.OrderItems)
                            {
                                OrderItemModel orderItemModel = new OrderItemModel();
                                orderItemModel.id =Convert.ToInt32(orderItems.Id);
                                orderItemModel.order_id = Convert.ToInt32(orderItems.OrderId);
                                orderItemModel.vendor_id = orderItems.VendorId;
                                orderItemModel.seller_id = orderItems.SellerId;
                                orderItemModel.product_id = orderItems.ProductId;
                                orderItemModel.variant_id = orderItems.VariantId;
                                orderItemModel.variant_slug = orderItems.VariantSlug;
                                orderItemModel.quantity = orderItems.Quantity;
                                orderItemModel.price = orderItems.Price;
                                orderItemModel.tax = orderItems.Tax;
                                orderItemModel.admin_commission = 0;
                                orderItemModel.created_at = orderItems.CreatedAt;
                                orderItemModel.updated_at = orderItems.UpdatedAt;
                                orderItemModel.is_review = 0;
                                orderItemModel.display_price = orderItems.Price;
                                orderItemModel.display_total_price = orderItems.Price;
                                orderViewModel.orderitems.Add(orderItemModel);
                            }
                        }
                        #endregion

                        orderListReturnModel.orderitems.data.Add(orderViewModel);
                        orderListReturnModel.orderitems.current_page = PageMetadate.CurrentPage;
                        orderListReturnModel.orderitems.total_page = PageMetadate.TotalPage;
                        orderListReturnModel.orderitems.page_size = PageMetadate.PazeSize;
                        #endregion
                    }
                    Message = "order list fetch successfully";
                    return Ok(new { error = false, data = orderListReturnModel, Message = Message, state = "order", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = false, message = "Order List Not Found.", data = "[]", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                #endregion
                
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get Order View Api
        [Authorize]
        [HttpGet]
        [Route("/vendor/order/show/{orderId}")]
        public IActionResult GetOrderView(int orderId)
        {
            try
            {
                string Message = string.Empty;
                OrderViewReturnModel orderViewReturnModel = new OrderViewReturnModel();
                double data2 = 00;
                var orderDetail = _ordersService.GetById(orderId);
                var paymentstatus = _paymentsService.GetByPaymentsOrderId(Convert.ToInt32(orderDetail.Id));
                if (orderDetail != null)
                {
                    #region Get Order Status
                    OrderStatusViewModel orderStatusViewModel = new OrderStatusViewModel();
                    orderStatusViewModel.processing = "Processing";
                    orderStatusViewModel.shipped = "Shipped";
                    orderStatusViewModel.completed = "Completed";
                    orderStatusViewModel.pending_for_payment = "Pending for Payment";
                    orderStatusViewModel.failed = "Failed";
                    orderStatusViewModel.returned = "Returned";
                    orderStatusViewModel.refunded = "Refunded";
                    orderStatusViewModel.cancelled = "Cancelled";
                    orderStatusViewModel.pending = "Cancelled";
                    orderViewReturnModel.orderStatuses = orderStatusViewModel;
                    #endregion

                    #region Get Order Data
                    OrderViewModel orderViewModel = new OrderViewModel();
                        orderViewModel.id = Convert.ToInt32(orderDetail.Id);
                        orderViewModel.order_id = orderDetail.OrderId;
                        orderViewModel.user_id = orderDetail.UserId;
                        orderViewModel.vendor_id = 0;
                        orderViewModel.firstname = orderDetail.User.Firstname;
                        orderViewModel.lastname = orderDetail.User.Lastname;
                        orderViewModel.email = orderDetail.User.Email;
                        orderViewModel.mobile = orderDetail.User.Mobile;
                        orderViewModel.voucher_code = orderDetail.VoucherCode;
                        orderViewModel.discount_amount = orderDetail.DiscountAmount;
                        orderViewModel.amount = orderDetail.Amount;
                        orderViewModel.shipping_type = orderDetail.ShippingType;
                        orderViewModel.shipping_amount = orderDetail.ShippingAmount;
                        orderViewModel.admin_commission = orderDetail.AdminCommission;
                        orderViewModel.seller_commission = orderDetail.SellerCommission;
                        orderViewModel.expected_delivery_date = null;
                        orderViewModel.status = orderDetail.Status;
                        orderViewModel.message = orderDetail.Message;
                        orderViewModel.transaction_id = orderDetail.TransactionId;
                        orderViewModel.payment_method = orderDetail.PaymentMethod;
                        orderViewModel.transaction_response = paymentstatus.PaymentStatus;
                        orderViewModel.payment_payed_to_vendor = 0;
                        orderViewModel.created_at = orderDetail.CreatedAt;
                        orderViewModel.updated_at = orderDetail.UpdatedAt;
                        orderViewModel.discount_amount = orderDetail.Amount;
                        orderViewModel.display_discount_amount = orderDetail.DiscountAmount;
                        orderViewModel.display_shipping_amount = orderDetail.ShippingAmount;
                        orderViewModel.display_total = (orderDetail.Amount);
                        orderViewModel.display_status = orderDetail.Status;
                        orderViewModel.sum_amount = orderDetail.Amount;
                        orderViewModel.sum_ammout_return = orderDetail.Amount;
                       orderViewModel.admin_commission =(Convert.ToDouble(orderDetail.Amount)*(orderDetail.AdminCommission))/100;
                    orderViewModel.display_amount = (orderDetail.Amount + (orderDetail.ShippingAmount ?? 0) 
                        +Convert.ToDecimal(orderViewModel.admin_commission));

                    //data2 = (Convert.ToDouble(orderDetail.Amount) * (orderDetail.AdminCommission)??0)/100;

                    #region Get Billing address
                    if (!string.IsNullOrEmpty(orderDetail.BillingAddress))
                        {
                            BillingVendorAddressModel billingAddressModel = new BillingVendorAddressModel();
                            var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(orderDetail.BillingAddress);
                            if (orderDetailBilling != null)
                            {
                                billingAddressModel.address = orderDetailBilling.address;
                                billingAddressModel.address2 = orderDetailBilling.address2;
                                billingAddressModel.state = orderDetailBilling.state;
                                billingAddressModel.postal_code = orderDetailBilling.postal_code;
                                billingAddressModel.city = orderDetailBilling.city;
                                billingAddressModel.country = orderDetailBilling.country;
                                orderViewModel.billing_address = billingAddressModel;
                            var countryShipingname = _CountryService.GetById(Convert.ToInt32(orderDetailBilling.country));
                            if (countryShipingname != null)
                            {
                                billingAddressModel.countryName = countryShipingname.Name;
                            }
                        }
                        }
                        #endregion

                        #region Get Shipping address
                        if (!string.IsNullOrEmpty(orderDetail.BillingAddress))
                        {
                            ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
                            var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(orderDetail.ShipingAddress);
                            if (orderDetailShipping != null)
                            {
                                shippingAddressModel.address = orderDetailShipping.address;
                                shippingAddressModel.address2 = orderDetailShipping.address2;
                                shippingAddressModel.state = orderDetailShipping.state;
                                shippingAddressModel.postal_code = orderDetailShipping.postal_code;
                                shippingAddressModel.city = orderDetailShipping.city;
                                shippingAddressModel.country = orderDetailShipping.country;
                                orderViewModel.shipping_address = shippingAddressModel;
                            var countryShipingname = _CountryService.GetById(Convert.ToInt32(orderDetailShipping.country));
                            if (countryShipingname != null)
                            {
                                shippingAddressModel.countryName = countryShipingname.Name;
                            }
                        }
                        }
                        #endregion

                        #region Get Currency
                        string currencyId = HttpContext.Session.GetString("currencyId");
                        var currencyData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                        if (currencyData != null)
                        {
                            CurrencyDataModel currencyDataModel = new CurrencyDataModel();
                            currencyDataModel.id = Convert.ToInt32(currencyData.Id);
                            currencyDataModel.currency_id = currencyData.CurrencyId;
                            currencyDataModel.is_primary = Convert.ToInt32(currencyData.IsPrimary);
                            currencyDataModel.live_rate = Convert.ToInt32(currencyData.LiveRate);
                            currencyDataModel.converted_rate = Convert.ToInt32(currencyData.ConvertedRate);
                            if (currencyData.Currency != null)
                            {
                                CurrencyModel currencyModel = new CurrencyModel();
                                currencyModel.id = currencyData.Currency.Id;
                                currencyModel.iso = currencyData.Currency.Iso;
                                currencyModel.name = currencyData.Currency.Name;
                                currencyModel.symbol = currencyData.Currency.Symbol;
                                currencyModel.symbol_native = currencyData.Currency.SymbolNative;
                                currencyDataModel.currency = currencyModel;

                            }
                            orderViewModel.currency = currencyDataModel;
                    }
                    #endregion

                    #region Get Order Items
                    if (orderDetail.OrderItems != null && orderDetail.OrderItems.Any())
                    {
                        foreach (var orderItems in orderDetail.OrderItems)
                        {
                            OrderItemModel orderItemModel = new OrderItemModel();
                            orderItemModel.id = Convert.ToInt32(orderItems.Id);
                            orderItemModel.order_id = Convert.ToInt32(orderItems.OrderId);
                            orderItemModel.seller_id = orderItems.SellerId;
                            orderItemModel.product_id = orderItems.ProductId;
                            orderItemModel.variant_id = orderItems.VariantId;
                            orderItemModel.variant_slug = orderItems.VariantSlug;
                            orderItemModel.quantity = orderItems.Quantity;
                            orderItemModel.price = (orderItems.Price / orderItems.Quantity);
                            orderItemModel.tax = orderItems.Tax;
                            //orderItemModel.admin_commission = orderItems.Order.AdminCommission;
                            orderItemModel.created_at = orderItems.CreatedAt;
                            orderItemModel.updated_at = orderItems.UpdatedAt;
                            orderItemModel.is_review = 0;
                            orderItemModel.display_price = (orderItems.Price/orderItems.Quantity);
                            orderItemModel.display_total_price = (orderItems.Price);

                            #region Get Product
                            var product = _productService.FindById(orderItems.ProductId);
                            if (product != null)
                            {
                                OrderProductModel orderProductModel = new OrderProductModel();
                                orderProductModel.id = product.Id;
                                orderProductModel.title = product.Title;
                                orderProductModel.sku = product.Sku;
                                orderProductModel.slug = product.Slug;
                                orderProductModel.brand_name = product.BrandName;
                                orderProductModel.banner_link = product.BannerLink;
                                orderProductModel.banner_image = product.BannerImage;
                                orderProductModel.prod_description = product.LongDescription;
                                orderProductModel.average_rating = 0;
                                orderProductModel.url = string.Empty;
                                orderProductModel.display_price = (Convert.ToInt32(product.Price) / orderItems.Quantity).ToString();
                                orderProductModel.display_discounted_price = (Convert.ToInt32(product.DiscountedPrice)).ToString();
                                orderItemModel.product = orderProductModel;

                                #region Get Tax
                                var tax = _taxService.GetTaxByCategoryId(product.CategoryId);
                                if (tax != null)
                                {
                                    TaxModel taxModel = new TaxModel();
                                    taxModel.id = Convert.ToInt32(tax.Id);
                                    taxModel.title = tax.Title;
                                    taxModel.category_id = tax.CategoryId;
                                    taxModel.sub_category_id = tax.SubCategoryId;
                                    taxModel.value = tax.Value;
                                    taxModel.status = Convert.ToInt32(tax.Status);
                                    taxModel.created_at = tax.CreatedAt;
                                    taxModel.updated_at = tax.UpdatedAt;
                                    orderViewReturnModel.taxClass = taxModel;
                                }
                                #endregion
                            }
                            #endregion

                            orderViewModel.orderitems.Add(orderItemModel);
                        }
                    }
                    #endregion

                    orderViewReturnModel.order = orderViewModel;
                    #endregion
                }

                Message = "Settings fetch successfully.";
                return Ok(new { error = false, data = orderViewReturnModel, Message = Message, state = "order", code = 200, status = true });
            }
            catch(Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }   
        }
        #endregion

        #region Get Order Status Update Api
        [Authorize]
        [HttpPost]
        [Route("/vendor/order/status-update")]
        //[Route("/vendor/return-order/status-update")]
        public IActionResult OrdersStatusUpdate([FromForm] OrderUpdateRequestModel requestModel)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;

                #region Order Status Update
                OrderViewUpdateModel orderViewUpdateModel = new OrderViewUpdateModel();
                var order = _ordersService.GetById(requestModel.id);
                if (order != null)
                {
                    order.Status = requestModel.status;
                    order.Message = requestModel.comment;
                    var responseOrder = _ordersService.Update(order);
                    #region Send Order Mail
                    sendEmailVerificationEmail(userId, order.OrderId);
                    #endregion
                    if (responseOrder != null)
                    {
                        
                        orderViewUpdateModel.id = Convert.ToInt32(responseOrder.Id);
                        orderViewUpdateModel.order_id = responseOrder.OrderId;
                        orderViewUpdateModel.user_id = responseOrder.UserId;
                        orderViewUpdateModel.vendor_id = 0;
                        orderViewUpdateModel.firstname = responseOrder.User.Firstname;
                        orderViewUpdateModel.lastname = responseOrder.User.Lastname;
                        orderViewUpdateModel.email = responseOrder.User.Email;
                        orderViewUpdateModel.mobile = responseOrder.User.Mobile;
                        orderViewUpdateModel.voucher_code = responseOrder.VoucherCode;
                        orderViewUpdateModel.discount_amount = responseOrder.DiscountAmount;
                        orderViewUpdateModel.amount = responseOrder.Amount;
                        orderViewUpdateModel.shipping_type = responseOrder.ShippingType;
                        orderViewUpdateModel.shipping_amount = responseOrder.ShippingAmount;
                        orderViewUpdateModel.admin_commission = responseOrder.AdminCommission;
                        orderViewUpdateModel.seller_commission = responseOrder.SellerCommission;
                        orderViewUpdateModel.expected_delivery_date = null;
                        orderViewUpdateModel.status = responseOrder.Status;
                        orderViewUpdateModel.message = responseOrder.Message;
                        orderViewUpdateModel.transaction_id = responseOrder.TransactionId;
                        orderViewUpdateModel.payment_method = responseOrder.PaymentMethod;
                        orderViewUpdateModel.transaction_response = responseOrder.TransactionResponse;
                        orderViewUpdateModel.payment_payed_to_vendor = 0;
                        orderViewUpdateModel.created_at = responseOrder.CreatedAt;
                        orderViewUpdateModel.updated_at = responseOrder.UpdatedAt;
                        orderViewUpdateModel.discount_amount = responseOrder.Amount;
                        orderViewUpdateModel.display_discount_amount = responseOrder.DiscountAmount;
                        orderViewUpdateModel.display_shipping_amount = responseOrder.ShippingAmount;
                        orderViewUpdateModel.display_total = responseOrder.Amount;
                        orderViewUpdateModel.display_status = responseOrder.Status;
                        orderViewUpdateModel.sum_amount = responseOrder.Amount;
                        orderViewUpdateModel.sum_ammout_return = responseOrder.Amount;

                        #region Get Billing Address
                        if (!string.IsNullOrEmpty(responseOrder.BillingAddress))
                        {
                            BillingVendorAddressModel billingAddressModel = new BillingVendorAddressModel();
                            var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(responseOrder.BillingAddress);
                            if (orderDetailBilling != null)
                            {
                                billingAddressModel.address = orderDetailBilling.address;
                                billingAddressModel.address2 = orderDetailBilling.address2;
                                billingAddressModel.state = orderDetailBilling.state;
                                billingAddressModel.postal_code = orderDetailBilling.postal_code;
                                billingAddressModel.city = orderDetailBilling.city;
                                billingAddressModel.country = orderDetailBilling.country;
                                orderViewUpdateModel.billing_address = billingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Shipping Address
                        if (!string.IsNullOrEmpty(responseOrder.BillingAddress))
                        {
                            ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
                            var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(responseOrder.ShipingAddress);
                            if (orderDetailShipping != null)
                            {
                                shippingAddressModel.address = orderDetailShipping.address;
                                shippingAddressModel.address2 = orderDetailShipping.address2;
                                shippingAddressModel.state = orderDetailShipping.state;
                                shippingAddressModel.postal_code = orderDetailShipping.postal_code;
                                shippingAddressModel.city = orderDetailShipping.city;
                                shippingAddressModel.country = orderDetailShipping.country;
                                orderViewUpdateModel.shipping_address = shippingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Currency
                        string currencyId = HttpContext.Session.GetString("currencyId");
                        var currencyData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                        if (currencyData != null)
                        {
                            CurrencyDataModel currencyDataModel = new CurrencyDataModel();
                            currencyDataModel.id = Convert.ToInt32(currencyData.Id);
                            currencyDataModel.currency_id = currencyData.CurrencyId;
                            currencyDataModel.is_primary = Convert.ToInt32(currencyData.IsPrimary);
                            currencyDataModel.live_rate = Convert.ToInt32(currencyData.LiveRate);
                            currencyDataModel.converted_rate = Convert.ToInt32(currencyData.ConvertedRate);
                            if (currencyData.Currency != null)
                            {
                                CurrencyModel currencyModel = new CurrencyModel();
                                currencyModel.id = currencyData.Currency.Id;
                                currencyModel.iso = currencyData.Currency.Iso;
                                currencyModel.name = currencyData.Currency.Name;
                                currencyModel.symbol = currencyData.Currency.Symbol;
                                currencyModel.symbol_native = currencyData.Currency.SymbolNative;
                                currencyDataModel.currency = currencyModel;

                            }
                            orderViewUpdateModel.currency = currencyDataModel;
                        }
                        #endregion

                        #region Get User Detail
                        var userDetail = _usersService.GetById(responseOrder.UserId);
                        if (userDetail != null)
                        {
                            UserModel userModel = new UserModel();
                            userModel.id = userDetail.Id;
                            userModel.role = 0;
                            userModel.firstname = userDetail.Firstname;
                            userModel.email = userDetail.Email;
                            userModel.mobile = userDetail.Mobile;
                            userModel.lastname = userDetail.Lastname;
                            userModel.profile_pic = userDetail.ProfilePic;
                            userModel.state = userDetail.State;
                            userModel.email_verified_at = userDetail.EmailVerifiedAt;
                            userModel.isVerified = Convert.ToInt32(userDetail.IsVerified);
                            userModel.is_admin = Convert.ToInt32(userDetail.IsAdmin);
                            userModel.stripe_customer_id = userDetail.StripeCustomerId;
                            userModel.stripe_id = userDetail.StripeId;
                            userModel.created_at = userDetail.CreatedAt;
                            userModel.updated_at = userDetail.UpdatedAt;
                            userModel.country = userDetail.Country;
                            userModel.status = userDetail.IsActive ? 1 : 0;
                            userModel.postal_code = userDetail.PostalCode;
                            userModel.country_code = userDetail.CountryCode;
                            userModel.is_guest = false;
                            orderViewUpdateModel.user = userModel;
                        }
                        #endregion

                        Message = "Order Updated Successfully.";
                    }
                }
                else
                {
                    Message = "Order does not exist.";
                    return Ok(new { error = false, data = "", Message = Message, state = "order", code = 200, status = true });
                }
                #endregion

                return Ok(new { error = false, data = orderViewUpdateModel, Message = Message, state = "order", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Return order Status Update

        [Authorize]
        [HttpPost]
        [Route("/vendor/return-order/status-update")]
        public IActionResult ReturnStatusUpdate([FromForm] ReturnUpdateRequestModel requestModel)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;

                #region Order Status Update
                Reurnmainmodel order_data = new Reurnmainmodel();
                ReturnRequests request =  _returnRequestService.GetById(requestModel.id);
                if (request != null)
                {
                    request.Status = requestModel.status;
                    request.Message = requestModel.message;
                    var responseOrder = _returnRequestService.UpdateReturnRequests(request);
                    var requestitem = _itemreturnService.GetById(Convert.ToInt16(responseOrder.Id));

                    foreach (var item in request.ReturnItems)
                    {
                        item.ReturnStatus = requestModel.status;
                        item.UpdatedAt = DateTime.Now;
                        var returitemdata = _itemreturnService.UpdateReturnItems(item);
                    }
                    var order = _ordersService.GetById(Convert.ToInt32(responseOrder.OrderId));
                    #region Send Order Mail
                    sendEmailVerificationEmail(userId, order.OrderId);
                    #endregion
                    if (responseOrder != null)
                    {
                        ReturnViewUpdateModel orderdata = new ReturnViewUpdateModel();
                        orderdata.id = Convert.ToInt32(responseOrder.Id);
                        orderdata.order_id = (responseOrder.OrderId).ToString();
                        orderdata.user_id = responseOrder.UserId;
                        orderdata.amount = responseOrder.Amount;
                        orderdata.refund_amount = responseOrder.RefundAmount;
                        orderdata.bank_account_id = responseOrder.BankAccountId;
                        orderdata.message = responseOrder.Message;
                        orderdata.status = responseOrder.Status;
                        orderdata.refund_id = responseOrder.TransactionId;
                        orderdata.created_at = responseOrder.CreatedAt;
                        orderdata.updated_at = responseOrder.UpdatedAt;
                        orderdata.sum_amount = responseOrder.Amount;
                        order_data.order_data = orderdata;

                        ReturnOrderViewModel orderdataall =new ReturnOrderViewModel();

                        orderdataall.id = Convert.ToInt32(order.Id);
                        orderdataall.order_id = order.OrderId;
                        orderdataall.user_id = order.UserId;
                        orderdataall.vendor_id = order.VendorId ??0;
                        orderdataall.firstname = order.User.Firstname;
                        orderdataall.lastname = order.User.Lastname;
                        orderdataall.email = order.User.Email;
                        orderdataall.mobile = order.User.Mobile;
                        orderdataall.voucher_code = order.VoucherCode;
                        orderdataall.discount_amount = order.DiscountAmount;
                        orderdataall.amount = order.Amount;
                        orderdataall.shipping_type = order.ShippingType;
                        orderdataall.shipping_amount = order.ShippingAmount;
                        orderdataall.admin_commission = order.AdminCommission;
                        orderdataall.seller_commission = order.SellerCommission;
                        orderdataall.expected_delivery_date = null;
                        orderdataall.status = order.Status;
                        orderdataall.message = order.Message;
                        orderdataall.transaction_id = order.TransactionId;
                        orderdataall.payment_method = order.PaymentMethod;
                        orderdataall.transaction_response = order.TransactionResponse;
                        orderdataall.payment_payed_to_vendor = 0;
                        orderdataall.created_at = order.CreatedAt;
                        orderdataall.updated_at = order.UpdatedAt;
                        orderdataall.discount_amount = order.Amount;
                        orderdataall.display_discount_amount = order.DiscountAmount;
                        orderdataall.display_shipping_amount = order.ShippingAmount;
                        orderdataall.display_total = order.Amount;
                        orderdataall.display_status = order.Status;
                        orderdataall.sum_amount = order.Amount;
                        orderdataall.sum_ammout_return = order.Amount;

                        #region Get Billing Address
                        if (!string.IsNullOrEmpty(order.BillingAddress))
                        {
                            BillingVendorAddressModel billingAddressModel = new BillingVendorAddressModel();
                            var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(order.BillingAddress);
                            if (orderDetailBilling != null)
                            {
                                billingAddressModel.address = orderDetailBilling.address;
                                billingAddressModel.address2 = orderDetailBilling.address2;
                                billingAddressModel.state = orderDetailBilling.state;
                                billingAddressModel.postal_code = orderDetailBilling.postal_code;
                                billingAddressModel.city = orderDetailBilling.city;
                                billingAddressModel.country = orderDetailBilling.country;
                                orderdata.billing_address = billingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Shipping Address
                        if (!string.IsNullOrEmpty(order.ShipingAddress))
                        {
                            ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
                            var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(order.ShipingAddress);
                            if (orderDetailShipping != null)
                            {
                                shippingAddressModel.address = orderDetailShipping.address;
                                shippingAddressModel.address2 = orderDetailShipping.address2;
                                shippingAddressModel.state = orderDetailShipping.state;
                                shippingAddressModel.postal_code = orderDetailShipping.postal_code;
                                shippingAddressModel.city = orderDetailShipping.city;
                                shippingAddressModel.country = orderDetailShipping.country;
                                orderdata.shipping_address = shippingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Currency
                        string currencyId = HttpContext.Session.GetString("currencyId");
                        var currencyData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                        if (currencyData != null)
                        {
                            CurrencyDataModel currencyDataModel = new CurrencyDataModel();
                            currencyDataModel.id = Convert.ToInt32(currencyData.Id);
                            currencyDataModel.currency_id = currencyData.CurrencyId;
                            currencyDataModel.is_primary = Convert.ToInt32(currencyData.IsPrimary);
                            currencyDataModel.live_rate = Convert.ToInt32(currencyData.LiveRate);
                            currencyDataModel.converted_rate = Convert.ToInt32(currencyData.ConvertedRate);
                            if (currencyData.Currency != null)
                            {
                                CurrencyModel currencyModel = new CurrencyModel();
                                currencyModel.id = currencyData.Currency.Id;
                                currencyModel.iso = currencyData.Currency.Iso;
                                currencyModel.name = currencyData.Currency.Name;
                                currencyModel.symbol = currencyData.Currency.Symbol;
                                currencyModel.symbol_native = currencyData.Currency.SymbolNative;
                                currencyDataModel.currency = currencyModel;

                            }
                            orderdataall.currency = currencyDataModel;
                            //order_data.order_data.order = orderdataall;
                            order_data.order_data.order = orderdataall;
                        }
                        #endregion

                        #region Return items

                        return_item datareturn = new return_item();
                        var Returnrequestitem = _itemreturnService.GetById(Convert.ToInt16(responseOrder.Id));
                        datareturn.id = Convert.ToInt32(Returnrequestitem.Id);
                        datareturn.request_id = Convert.ToInt32(Returnrequestitem.RequestId);
                        datareturn.order_item_id = Convert.ToInt32(Returnrequestitem.OrderItemId);
                        datareturn.return_quantity = Returnrequestitem.ReturnQuantity;
                        datareturn.return_status = Returnrequestitem.ReturnStatus;
                        datareturn.created_at = Returnrequestitem.CreatedAt;
                        datareturn.updated_at = Returnrequestitem.UpdatedAt;
                        order_data.return_item = datareturn;
                        #endregion


                        Message = "Order Updated Successfully.";
                    }
                }
                else
                {
                    Message = "Order does not exist.";
                    return Ok(new { error = false, data = "", Message = Message, state = "order", code = 200, status = true });
                }
                #endregion

                return Ok(new { error = false, data = order_data, Message = Message, state = "order", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion


        #region Get Return Order List Api
        [Authorize]
        [HttpGet]
        [Route("/vendor/return-order/list")]
        public IActionResult GetReturnOrderList([FromQuery] ToDoSearchSpecs model)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;
                ReturnOrderReturnModel returnOrderReturnModel = new ReturnOrderReturnModel();

                #region Get Return Status
                ReturnStatusModel returnStatusModel = new ReturnStatusModel();
                returnStatusModel.New = ReturnOrderEnum.New.GetDescription();
                returnStatusModel.accepted = ReturnOrderEnum.Accepcted.GetDescription();
                returnStatusModel.declined = ReturnOrderEnum.Declined.GetDescription();
                returnStatusModel.refunded = ReturnOrderEnum.Refunded.GetDescription();
                returnOrderReturnModel.returnStatus = returnStatusModel;
                #endregion

                #region Get Return Request
                var returnRequestList = _returnRequestService.GetReturnRequestListForVendor(model, userId);
                if (returnRequestList != null && returnRequestList.Any())
                {
                    var PageMetadate = new
                    {
                        returnRequestList.CurrentPage,
                        returnRequestList.PazeSize,
                        returnRequestList.TotalPage,
                        returnRequestList.TotalCount,
                        returnRequestList.HasNext,
                        returnRequestList.HasPrev
                    };
                    Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));

                    foreach (var returnRequests in returnRequestList)
                    {
                        ReturnRequestOrderViewModel returnRequestOrderViewModel = new ReturnRequestOrderViewModel();
                        #region Get Return Request Data
                        returnRequestOrderViewModel.id = Convert.ToInt32(returnRequests.Id);
                        returnRequestOrderViewModel.order_id = Convert.ToInt32(returnRequests.OrderId);
                        returnRequestOrderViewModel.user_id = returnRequests.UserId;
                        returnRequestOrderViewModel.amount = Convert.ToDecimal(returnRequests.Amount);
                        returnRequestOrderViewModel.refund_amount = Convert.ToDecimal(returnRequests.RefundAmount);
                        returnRequestOrderViewModel.bank_account_id = returnRequests.BankAccountId;
                        returnRequestOrderViewModel.message = returnRequests.Message;
                        returnRequestOrderViewModel.status = returnRequests.Status;
                        returnRequestOrderViewModel.refund_id = returnRequests.TransactionId;
                        returnRequestOrderViewModel.created_at = returnRequests.CreatedAt;
                        returnRequestOrderViewModel.updated_at = returnRequests.UpdatedAt;
                        #endregion

                        #region Get Order Data
                        if (returnRequests.Order != null)
                        {
                            ReturnOrderViewModel orderViewModel = new ReturnOrderViewModel();
                            orderViewModel.id = Convert.ToInt32(returnRequests.Order.Id);
                            orderViewModel.order_id = returnRequests.Order.OrderId;
                            orderViewModel.user_id = returnRequests.Order.UserId;
                            orderViewModel.vendor_id = 0;
                            orderViewModel.firstname = returnRequests.User.Firstname;
                            orderViewModel.lastname = returnRequests.User.Lastname;
                            orderViewModel.email = returnRequests.User.Email;
                            orderViewModel.mobile = returnRequests.User.Mobile;
                            orderViewModel.voucher_code = returnRequests.Order.VoucherCode;
                            orderViewModel.discount_amount = returnRequests.Order.DiscountAmount;
                            orderViewModel.amount = returnRequests.Order.Amount;
                            orderViewModel.shipping_type = returnRequests.Order.ShippingType;
                            orderViewModel.shipping_amount = returnRequests.Order.ShippingAmount;
                            orderViewModel.admin_commission = returnRequests.Order.AdminCommission;
                            orderViewModel.seller_commission = returnRequests.Order.SellerCommission;
                            orderViewModel.expected_delivery_date = null;
                            orderViewModel.status = returnRequests.Order.Status;
                            orderViewModel.message = returnRequests.Order.Message;
                            orderViewModel.transaction_id = returnRequests.Order.TransactionId;
                            orderViewModel.payment_method = returnRequests.Order.PaymentMethod;
                            orderViewModel.transaction_response = returnRequests.Order.TransactionResponse;
                            orderViewModel.payment_payed_to_vendor = 0;
                            orderViewModel.created_at = returnRequests.Order.CreatedAt;
                            orderViewModel.updated_at = returnRequests.Order.UpdatedAt;
                            orderViewModel.discount_amount = returnRequests.Order.Amount;
                            orderViewModel.display_discount_amount = returnRequests.Order.DiscountAmount;
                            orderViewModel.display_shipping_amount = returnRequests.Order.ShippingAmount;
                            orderViewModel.display_total = returnRequests.Order.Amount;
                            orderViewModel.display_status = returnRequests.Order.Status;
                            orderViewModel.sum_amount = returnRequests.Order.Amount;
                            orderViewModel.sum_ammout_return = returnRequests.Order.Amount;

                            #region Get Billing address
                            if (!string.IsNullOrEmpty(returnRequests.Order.BillingAddress))
                            {
                                BillingVendorAddressModel billingAddressModel = new BillingVendorAddressModel();
                                var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(returnRequests.Order.BillingAddress);
                                if (orderDetailBilling != null)
                                {
                                    billingAddressModel.address = orderDetailBilling.address;
                                    billingAddressModel.address2 = orderDetailBilling.address2;
                                    billingAddressModel.state = orderDetailBilling.state;
                                    billingAddressModel.postal_code = orderDetailBilling.postal_code;
                                    billingAddressModel.city = orderDetailBilling.city;
                                    billingAddressModel.country = orderDetailBilling.country;
                                    orderViewModel.billing_address = billingAddressModel;
                                }
                            }
                            #endregion

                            #region Get Shipping address
                            if (!string.IsNullOrEmpty(returnRequests.Order.BillingAddress))
                            {
                                ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
                                var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(returnRequests.Order.ShipingAddress);
                                if (orderDetailShipping != null)
                                {
                                    shippingAddressModel.address = orderDetailShipping.address;
                                    shippingAddressModel.address2 = orderDetailShipping.address2;
                                    shippingAddressModel.state = orderDetailShipping.state;
                                    shippingAddressModel.postal_code = orderDetailShipping.postal_code;
                                    shippingAddressModel.city = orderDetailShipping.city;
                                    shippingAddressModel.country = orderDetailShipping.country;
                                    orderViewModel.shipping_address = shippingAddressModel;
                                }
                            }
                            #endregion

                            #region Get Currency
                            string currencyId = HttpContext.Session.GetString("currencyId");
                            var currencyData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                            if (currencyData != null)
                            {
                                CurrencyDataModel currencyDataModel = new CurrencyDataModel();
                                currencyDataModel.id = Convert.ToInt32(currencyData.Id);
                                currencyDataModel.currency_id = currencyData.CurrencyId;
                                currencyDataModel.is_primary = Convert.ToInt32(currencyData.IsPrimary);
                                currencyDataModel.live_rate = Convert.ToInt32(currencyData.LiveRate);
                                currencyDataModel.converted_rate = Convert.ToInt32(currencyData.ConvertedRate);
                                if (currencyData.Currency != null)
                                {
                                    CurrencyModel currencyModel = new CurrencyModel();
                                    currencyModel.id = currencyData.Currency.Id;
                                    currencyModel.iso = currencyData.Currency.Iso;
                                    currencyModel.name = currencyData.Currency.Name;
                                    currencyModel.symbol = currencyData.Currency.Symbol;
                                    currencyModel.symbol_native = currencyData.Currency.SymbolNative;
                                    currencyDataModel.currency = currencyModel;

                                }
                                orderViewModel.currency = currencyDataModel;
                            }
                            #endregion

                            returnRequestOrderViewModel.order = orderViewModel;
                        }
                        #endregion

                        #region Get User Detail
                        var userDetail = _usersService.GetById(returnRequests.UserId);
                        if (userDetail != null)
                        {
                            UserModel userModel = new UserModel();
                            userModel.id = userDetail.Id;
                            userModel.role = 0;
                            userModel.firstname = userDetail.Firstname;
                            userModel.email = userDetail.Email;
                            userModel.mobile = userDetail.Mobile;
                            userModel.lastname = userDetail.Lastname;
                            userModel.profile_pic = userDetail.ProfilePic;
                            userModel.state = userDetail.State;
                            userModel.email_verified_at = userDetail.EmailVerifiedAt;
                            userModel.isVerified = Convert.ToInt32(userDetail.IsVerified);
                            userModel.is_admin = Convert.ToInt32(userDetail.IsAdmin);
                            userModel.stripe_customer_id = userDetail.StripeCustomerId;
                            userModel.stripe_id = userDetail.StripeId;
                            userModel.created_at = userDetail.CreatedAt;
                            userModel.updated_at = userDetail.UpdatedAt;
                            userModel.country = userDetail.Country;
                            userModel.status = userDetail.IsActive ? 1 : 0;
                            userModel.postal_code = userDetail.PostalCode;
                            userModel.country_code = userDetail.CountryCode;
                            userModel.is_guest = false;
                            returnRequestOrderViewModel.user = userModel;
                        }
                        #endregion

                        #region Get Return Items
                        if (returnRequests.ReturnItems != null && returnRequests.ReturnItems.Any())
                        {
                            foreach (var returnItems in returnRequests.ReturnItems)
                            {
                                ReturnItemViewModel returnItemViewModel = new ReturnItemViewModel();
                                returnItemViewModel.id = Convert.ToInt32(returnItems.Id);
                                returnItemViewModel.request_id = Convert.ToInt32(returnItems.RequestId);
                                returnItemViewModel.order_item_id = Convert.ToInt32(returnItems.OrderItemId);
                                returnItemViewModel.return_quantity = Convert.ToInt32(returnItems.ReturnQuantity);
                                returnItemViewModel.return_status = returnItems.ReturnStatus;
                                returnItemViewModel.created_at = returnItems.CreatedAt;
                                returnItemViewModel.updated_at = returnItems.UpdatedAt;

                                var orderItems = _ordersService.GetByOrderItemId(Convert.ToInt32(returnItems.OrderItemId));
                                if (orderItems != null)
                                {
                                    OrderItemModel orderItemModel = new OrderItemModel();
                                    orderItemModel.id = Convert.ToInt32(orderItems.Id);
                                    orderItemModel.order_id = Convert.ToInt32(orderItems.OrderId);
                                    orderItemModel.seller_id = orderItems.SellerId;
                                    orderItemModel.product_id = orderItems.ProductId;
                                    orderItemModel.variant_id = orderItems.VariantId;
                                    orderItemModel.variant_slug = orderItems.VariantSlug;
                                    orderItemModel.quantity = orderItems.Quantity;
                                    orderItemModel.price = orderItems.Price;
                                    orderItemModel.tax = orderItems.Tax;
                                    orderItemModel.admin_commission = 0;
                                    orderItemModel.created_at = orderItems.CreatedAt;
                                    orderItemModel.updated_at = orderItems.UpdatedAt;
                                    orderItemModel.is_review = 0;
                                    orderItemModel.display_price = orderItems.Price;
                                    orderItemModel.display_total_price = orderItems.Price;

                                    if (orderItems.Product != null)
                                    {
                                        OrderProductModel orderProductModel = new OrderProductModel();
                                        orderProductModel.id = orderItems.Product.Id;
                                        orderProductModel.title = orderItems.Product.Title;
                                        orderProductModel.sku = orderItems.Product.Sku;
                                        orderProductModel.slug = orderItems.Product.Slug;
                                        orderProductModel.brand_name = orderItems.Product.BrandName;
                                        orderProductModel.banner_link = orderItems.Product.BannerLink;
                                        orderProductModel.banner_image = orderItems.Product.BannerImage;
                                        orderProductModel.prod_description = orderItems.Product.LongDescription;
                                        orderProductModel.url = null;
                                        orderProductModel.display_price = orderItems.Product.Price.ToString();
                                        orderProductModel.display_discounted_price = orderItems.Product.DiscountedPrice.ToString();
                                        orderItemModel.product = orderProductModel;
                                    }
                                    returnItemViewModel.order_item = orderItemModel;
                                }
                                returnRequestOrderViewModel.return_items.Add(returnItemViewModel);
                            }
                        }
                        #endregion

                        returnOrderReturnModel.returnRequests.data.Add(returnRequestOrderViewModel);
                        returnOrderReturnModel.returnRequests.current_page = PageMetadate.CurrentPage;
                        returnOrderReturnModel.returnRequests.total_page = PageMetadate.TotalPage;
                        returnOrderReturnModel.returnRequests.page_size = PageMetadate.PazeSize;
                    }
                    Message = "Return Order list fetch successfully";
                }
                else
                {
                    Message = "Record not found";
                }
                #endregion

                return Ok(new { error = false, data = returnOrderReturnModel, Message = Message, state = "order", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Get Return Order show Api
        [Authorize]
        [HttpGet]
        [Route("/vendor/return-order/show/{returnRequestId}")]
        public IActionResult GetReturnOrderShow(int returnRequestId)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;
                ReturnOrderShowReturnModel returnOrderReturnModel = new ReturnOrderShowReturnModel();

                #region Get Return Status
                ReturnStatusModel returnStatusModel = new ReturnStatusModel();
                returnStatusModel.New = ReturnOrderEnum.New.GetDescription();
                returnStatusModel.accepted = ReturnOrderEnum.Accepcted.GetDescription();
                returnStatusModel.declined = ReturnOrderEnum.Declined.GetDescription();
                returnStatusModel.refunded = ReturnOrderEnum.Refunded.GetDescription();
                returnOrderReturnModel.returnStatus = returnStatusModel;
                #endregion

                #region Get Return Request
                var returnRequests = _returnRequestService.GetById(returnRequestId);
                if (returnRequests != null)
                {
                    ReturnRequestOrderViewModel returnRequestOrderViewModel = new ReturnRequestOrderViewModel();
                    #region Get Return Request Data
                    returnRequestOrderViewModel.id = Convert.ToInt32(returnRequests.Id);
                    returnRequestOrderViewModel.order_id = Convert.ToInt32(returnRequests.OrderId);
                    returnRequestOrderViewModel.user_id = returnRequests.UserId;
                    returnRequestOrderViewModel.amount = Convert.ToDecimal(returnRequests.Amount);
                    returnRequestOrderViewModel.refund_amount = Convert.ToDecimal(returnRequests.RefundAmount);
                    returnRequestOrderViewModel.bank_account_id = returnRequests.BankAccountId;
                    returnRequestOrderViewModel.message = returnRequests.Message;
                    returnRequestOrderViewModel.status = returnRequests.Status;
                    returnRequestOrderViewModel.refund_id = returnRequests.TransactionId;
                    returnRequestOrderViewModel.created_at = returnRequests.CreatedAt;
                    returnRequestOrderViewModel.updated_at = returnRequests.UpdatedAt;
                    #endregion

                    #region Get Order Data
                    if (returnRequests.Order != null)
                    {
                        ReturnOrderViewModel orderViewModel = new ReturnOrderViewModel();
                        orderViewModel.id = Convert.ToInt32(returnRequests.Order.Id);
                        orderViewModel.order_id = returnRequests.Order.OrderId;
                        orderViewModel.user_id = returnRequests.Order.UserId;
                        orderViewModel.vendor_id = 0;
                        orderViewModel.firstname = returnRequests.User.Firstname;
                        orderViewModel.lastname = returnRequests.User.Lastname;
                        orderViewModel.email = returnRequests.User.Email;
                        orderViewModel.mobile = returnRequests.User.Mobile;
                        orderViewModel.voucher_code = returnRequests.Order.VoucherCode;
                        orderViewModel.discount_amount = returnRequests.Order.DiscountAmount;
                        orderViewModel.amount = returnRequests.Order.Amount;
                        orderViewModel.shipping_type = returnRequests.Order.ShippingType;
                        orderViewModel.shipping_amount = returnRequests.Order.ShippingAmount;
                        orderViewModel.admin_commission = returnRequests.Order.AdminCommission;
                        orderViewModel.seller_commission = returnRequests.Order.SellerCommission;
                        orderViewModel.expected_delivery_date = null;
                        orderViewModel.status = returnRequests.Order.Status;
                        orderViewModel.message = returnRequests.Order.Message;
                        orderViewModel.transaction_id = returnRequests.Order.TransactionId;
                        orderViewModel.payment_method = returnRequests.Order.PaymentMethod;
                        orderViewModel.transaction_response = returnRequests.Order.TransactionResponse;
                        orderViewModel.payment_payed_to_vendor = 0;
                        orderViewModel.created_at = returnRequests.Order.CreatedAt;
                        orderViewModel.updated_at = returnRequests.Order.UpdatedAt;
                        orderViewModel.discount_amount = returnRequests.Order.Amount;
                        orderViewModel.display_discount_amount = returnRequests.Order.DiscountAmount;
                        orderViewModel.display_shipping_amount = returnRequests.Order.ShippingAmount;
                        orderViewModel.display_total = returnRequests.Order.Amount;
                        orderViewModel.display_status = returnRequests.Order.Status;
                        orderViewModel.sum_amount = returnRequests.Order.Amount;
                        orderViewModel.sum_ammout_return = returnRequests.Order.Amount;

                        #region Get Billing address
                        if (!string.IsNullOrEmpty(returnRequests.Order.BillingAddress))
                        {
                            BillingVendorAddressModel billingAddressModel = new BillingVendorAddressModel();
                            var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(returnRequests.Order.BillingAddress);
                            if (orderDetailBilling != null)
                            {
                                billingAddressModel.address = orderDetailBilling.address;
                                billingAddressModel.address2 = orderDetailBilling.address2;
                                billingAddressModel.state = orderDetailBilling.state;
                                billingAddressModel.postal_code = orderDetailBilling.postal_code;
                                billingAddressModel.city = orderDetailBilling.city;
                                billingAddressModel.country = orderDetailBilling.country;
                                orderViewModel.billing_address = billingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Shipping address
                        if (!string.IsNullOrEmpty(returnRequests.Order.BillingAddress))
                        {
                            ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
                            var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(returnRequests.Order.ShipingAddress);
                            if (orderDetailShipping != null)
                            {
                                shippingAddressModel.address = orderDetailShipping.address;
                                shippingAddressModel.address2 = orderDetailShipping.address2;
                                shippingAddressModel.state = orderDetailShipping.state;
                                shippingAddressModel.postal_code = orderDetailShipping.postal_code;
                                shippingAddressModel.city = orderDetailShipping.city;
                                shippingAddressModel.country = orderDetailShipping.country;
                                orderViewModel.shipping_address = shippingAddressModel;
                            }
                        }
                        #endregion

                        #region Get Currency
                        string currencyId = HttpContext.Session.GetString("currencyId");
                        var currencyData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                        if (currencyData != null)
                        {
                            CurrencyDataModel currencyDataModel = new CurrencyDataModel();
                            currencyDataModel.id = Convert.ToInt32(currencyData.Id);
                            currencyDataModel.currency_id = currencyData.CurrencyId;
                            currencyDataModel.is_primary = Convert.ToInt32(currencyData.IsPrimary);
                            currencyDataModel.live_rate = Convert.ToInt32(currencyData.LiveRate);
                            currencyDataModel.converted_rate = Convert.ToInt32(currencyData.ConvertedRate);
                            if (currencyData.Currency != null)
                            {
                                CurrencyModel currencyModel = new CurrencyModel();
                                currencyModel.id = currencyData.Currency.Id;
                                currencyModel.iso = currencyData.Currency.Iso;
                                currencyModel.name = currencyData.Currency.Name;
                                currencyModel.symbol = currencyData.Currency.Symbol;
                                currencyModel.symbol_native = currencyData.Currency.SymbolNative;
                                currencyDataModel.currency = currencyModel;

                            }
                            orderViewModel.currency = currencyDataModel;
                        }
                        #endregion

                        returnRequestOrderViewModel.order = orderViewModel;
                    }
                    #endregion

                    #region Get User Detail
                    var userDetail = _usersService.GetById(returnRequests.UserId);
                    if (userDetail != null)
                    {
                        UserModel userModel = new UserModel();
                        userModel.id = userDetail.Id;
                        userModel.role = 0;
                        userModel.firstname = userDetail.Firstname;
                        userModel.email = userDetail.Email;
                        userModel.mobile = userDetail.Mobile;
                        userModel.lastname = userDetail.Lastname;
                        userModel.profile_pic = userDetail.ProfilePic;
                        userModel.state = userDetail.State;
                        userModel.email_verified_at = userDetail.EmailVerifiedAt;
                        userModel.isVerified = Convert.ToInt32(userDetail.IsVerified);
                        userModel.is_admin = Convert.ToInt32(userDetail.IsAdmin);
                        userModel.stripe_customer_id = userDetail.StripeCustomerId;
                        userModel.stripe_id = userDetail.StripeId;
                        userModel.created_at = userDetail.CreatedAt;
                        userModel.updated_at = userDetail.UpdatedAt;
                        userModel.country = userDetail.Country;
                        userModel.status = userDetail.IsActive ? 1 : 0;
                        userModel.postal_code = userDetail.PostalCode;
                        userModel.country_code = userDetail.CountryCode;
                        userModel.is_guest = false;
                        returnRequestOrderViewModel.user = userModel;
                    }
                    #endregion

                    #region Get Return Items
                    if (returnRequests.ReturnItems != null && returnRequests.ReturnItems.Any())
                    {
                        foreach (var returnItems in returnRequests.ReturnItems)
                        {
                            ReturnItemViewModel returnItemViewModel = new ReturnItemViewModel();
                            returnItemViewModel.id = Convert.ToInt32(returnItems.Id);
                            returnItemViewModel.request_id = Convert.ToInt32(returnItems.RequestId);
                            returnItemViewModel.order_item_id = Convert.ToInt32(returnItems.OrderItemId);
                            returnItemViewModel.return_quantity = Convert.ToInt32(returnItems.ReturnQuantity);
                            returnItemViewModel.return_status = returnItems.ReturnStatus;
                            returnItemViewModel.created_at = returnItems.CreatedAt;
                            returnItemViewModel.updated_at = returnItems.UpdatedAt;

                            var orderItems = _ordersService.GetByOrderItemId(Convert.ToInt32(returnItems.OrderItemId));
                            if (orderItems != null)
                            {
                                OrderItemModel orderItemModel = new OrderItemModel();
                                orderItemModel.id = Convert.ToInt32(orderItems.Id);
                                orderItemModel.order_id = Convert.ToInt32(orderItems.OrderId);
                                orderItemModel.seller_id = orderItems.SellerId;
                                orderItemModel.product_id = orderItems.ProductId;
                                orderItemModel.variant_id = orderItems.VariantId;
                                orderItemModel.variant_slug = orderItems.VariantSlug;
                                orderItemModel.quantity = orderItems.Quantity;
                                orderItemModel.price = (orderItems.Price / orderItems.Quantity);
                                orderItemModel.tax = orderItems.Tax;
                                orderItemModel.admin_commission = 0;
                                orderItemModel.created_at = orderItems.CreatedAt;
                                orderItemModel.updated_at = orderItems.UpdatedAt;
                                orderItemModel.is_review = 0;
                                orderItemModel.display_price = orderItems.Price;
                                orderItemModel.display_total_price = orderItems.Price;

                                if (orderItems.Product != null)
                                {
                                    OrderProductModel orderProductModel = new OrderProductModel();
                                    orderProductModel.id = orderItems.Product.Id;
                                    orderProductModel.title = orderItems.Product.Title;
                                    orderProductModel.sku = orderItems.Product.Sku;
                                    orderProductModel.slug = orderItems.Product.Slug;
                                    orderProductModel.brand_name = orderItems.Product.BrandName;
                                    orderProductModel.banner_link = orderItems.Product.BannerLink;
                                    orderProductModel.banner_image = orderItems.Product.BannerImage;
                                    orderProductModel.prod_description = orderItems.Product.LongDescription;
                                    orderProductModel.url = null;
                                    orderProductModel.display_price = orderItems.Product.Price.ToString();
                                    orderProductModel.display_discounted_price = orderItems.Product.DiscountedPrice.ToString();
                                    orderItemModel.product = orderProductModel;
                                }
                                returnItemViewModel.order_item = orderItemModel;
                            }
                            returnRequestOrderViewModel.return_items.Add(returnItemViewModel);
                        }
                    }
                    #endregion

                    returnOrderReturnModel.returnRequest = returnRequestOrderViewModel;
                    Message = "Return Order list fetch successfully";
                }
                else
                {
                    Message = "Record not found";
                }
                #endregion
                return Ok(new { error = false, data = returnOrderReturnModel, Message = Message, state = "order", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = Ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Order Refund Api
        [Authorize]
        [HttpPost]
        [Route("/vendor/return-order/refund")]
        public IActionResult RefundOrder([FromForm] RefundOrderRequestModel model)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                string Message = string.Empty;

                var returnRequest = _returnRequestService.GetById(model.id);
                if (returnRequest != null && returnRequest.Order != null)
                {
                    if (returnRequest.Order.PaymentMethod == "cod")
                    {
                        return Ok(new { error = false, data = "", Message = "Not refunded! Order amount paid in cash.", state = "order", code = 400, status = true });
                    }
                    if (string.IsNullOrEmpty(returnRequest.Order.TransactionId))
                    {
                        return Ok(new { error = false, data = "", Message = "Transaction id not available!", state = "order", code = 400, status = true });
                    }

                    var refundPayment = StripeAccount.RefundPayment(returnRequest.Order.TransactionId, returnRequest.Order.Amount);
                    if (refundPayment != null && refundPayment.Status == ReturnOrderEnum.Succeeded.GetDescription())
                    {
                        returnRequest.Status = ReturnOrderEnum.Refunded.GetDescription();
                        returnRequest.TransactionId = refundPayment.Id;
                        if (returnRequest.ReturnItems != null && returnRequest.ReturnItems.Any())
                        {
                            foreach (var item in returnRequest.ReturnItems)
                            {
                                item.ReturnStatus = ReturnOrderEnum.Refunded.GetDescription();
                                returnRequest.ReturnItems.Add(item);
                            }
                        }
                    }
                    var responseRequest = _returnRequestService.UpdateReturnRequests(returnRequest);
                    Message = "Status updated successfully and your amount is refunded";
                    return Ok(new { error = false, data = "", Message = Message, state = "order", code = 200, status = true });
                }
                else
                {
                    return Ok(new { error = false, data = "", Message = "Record not found", state = "order", code = 400, status = true });
                }
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "This order has been already refunded", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Order Invoice Download
        [Route("/download_invoice_by_url/{id}")]
        [HttpGet]
        public IActionResult GeneratePdf(int id)
        {
            string logfilename = SiteKey.UploadImage+"log.txt";
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("before save function");
                using (StreamWriter sw = new StreamWriter(logfilename, true))
                {
                    sw.WriteLineAsync(sb.ToString());
                    sw.Close();
                }
                var invoicedata = _ordersService.GetByItemId_invoice(id);
           
            //ShippingVendorAddressModel shippingAddressModel = new ShippingVendorAddressModel();
            var orderDetailShipping = JsonSerializer.Deserialize<ShippingVendorAddressModel>(invoicedata.ShipingAddress);
            var orderDetailBilling = JsonSerializer.Deserialize<BillingVendorAddressModel>(invoicedata.BillingAddress);


            HtmlToPdf converter = new HtmlToPdf();
            StringBuilder s=new StringBuilder();
            string printDto = "\r\n<html>\r\n<head>\r\n<meta charset=\"utf-8\">\r\n<title>:: DS Newsletter ::</title>\r\n    <style type=\"text/css\">\r\nbody {\r\nmargin-left: 0px;\r\nmargin-top: 0px;\r\nmargin-right: 0px;\r\n            margin-bottom: 0px;\r\n}\r\n</style>\r\n</head>\r\n\r\n<body>\r\n<table width=\"500\" border=\"0\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\">\r\n<tr>\r\n<td height=\"10\"></td>\r\n</tr>\r\n        <tr>\r\n<td><h1><img src=" + SiteKey.ImagePath + "/Uploads/"+ "5_638046463749154848.png" + "></h1></td>\r\n        </tr>\r\n<tr>\r\n<td height=\"10\"></td>\r\n</tr>\r\n<tr>\r\n<td style=\"padding:10px; color:#fff; font-family:Arial, Helvetica, sans-serif; font-size:16px;background-color: #ea3a3c;\">\r\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n\r\n<tr>\r\n<td height=\"5\"></td>\r\n</tr>\r\n<tr>\r\n  <td>Order #" + invoicedata.OrderId + "</td>\r\n</tr>\r\n<tr>\r\n<td height=\"5\"></td>\r\n</tr>\r\n <tr>\r\n<td>Order Date:" + invoicedata.CreatedAt + "</td>\r\n</tr>\r\n\r\n</table>\r\n</td>\r\n</tr>\r\n<tr>\r\n<td style=\"border:1px solid #CCC\">\r\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n<tr style=\"background-color:#edebeb; \">\r\n<td width=\"49%\" height=\"30\" style=\"border-bottom:1px solid #dfdfdf;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Sold to : </strong></td>\r\n<td width=\"51%\" align=\"right\" style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Ship to : </strong></td>\r\n</tr>\r\n<tr>\r\n<td style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">\r\n" + invoicedata.Firstname + "</br>\r\n" + orderDetailBilling.address + "," + orderDetailBilling.address2 + "</br>\r\n    " + orderDetailBilling.state + "," + orderDetailBilling.city + "," + orderDetailBilling.postal_code + ",</br>\r\n" + orderDetailBilling.countryName + "\r\n</td>\r\n <td colspan=\"2\" align=\"right\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">\r\n     " + invoicedata.Firstname + "</br>\r\n" + orderDetailShipping.address2 + "," + orderDetailShipping.address + "</br>\r\n " + orderDetailShipping.state + "," + orderDetailShipping.city + "," + orderDetailShipping.postal_code + ",</br>\r\n" + orderDetailShipping.countryName + "\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n<tr>\r\n<td>&nbsp;</td>\r\n</tr>\r\n<tr>\r\n <td style=\"border:1px solid #CCC\">\r\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                    <tr style=\"background-color:#edebeb; \">\r\n<td width=\"49%\" height=\"30\" style=\"border-bottom:1px solid #dfdfdf;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Payment Method : </strong></td>\r\n                        <td width=\"51%\" align=\"right\" style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><a href=\"#\">Shipping Method : </a></td>\r\n                    </tr>\r\n                    <tr>\r\n                        <td valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">\r\n                            <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n                                <tr>\r\n  <td>" + invoicedata.PaymentMethod + "</td>\r\n</tr>\r\n\r\n</table>\r\n</td>\r\n<td colspan=\"2\" align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">\r\n <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n <tr>\r\n  <td>" + invoicedata.ShippingType + "</td>\r\n </tr>\r\n\r\n<tr>\r\n  <td>&nbsp;</td>\r\n </tr>\r\n<tr>\r\n<td height=\"5\"></td>\r\n  </tr>\r\n<tr>\r\n <td align=\"right\">(Total Shipping Charges :" + invoicedata.ShippingAmount + ")</td>\r\n</tr>\r\n</table>\r\n </td>\r\n</tr>\r\n</table>\r\n            </td>\r\n </tr>\r\n<tr>\r\n<td>&nbsp;</td>\r\n</tr>\r\n</tr>\r\n<tr>\r\n<td style=\"border:1px solid #CCC\">\r\n <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">\r\n     <tr style=\"background-color:#edebeb; \">\r\n<td width=\"49%\" height=\"30\" style=\"border-bottom:1px solid #dfdfdf;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Products</strong></td>\r\n            <td style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>SKU</strong></td>\r\n                        <td style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Price</strong></td>\r\n                        <td style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Qty</strong></td>\r\n                        <td style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Tax</strong></td>\r\n <td style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ;  padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"><strong>Subtotal</strong></td                        <td align=\"right\" style=\"border-left:1px solid #dfdfdf; border-bottom:1px solid #dfdfdf ; padding:5px 10px; font-family:Arial, Helvetica, sans-serif; font-size:16px\"></td>\r\n</tr>";
            s.Append(printDto);
            var ordeitemsdata = _ordersService.GetByItemsOrderId(Convert.ToInt32(invoicedata.Id));
            string data = "";
            foreach (var item in ordeitemsdata)
            {
                data = "<tr>\r\n <td valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">"+item.Product.Title+"</td>\r\n <td valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">"+item.Product.Sku+"</td>\r\n<td valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>"+(item.Price/ item.Quantity) +"</strong></td>\r\n<td valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\">"+item.Quantity+"</td>\r\n<td valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>"+item.Tax+"</strong></td>\r\n                        <td align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>"+(item.Price)+"</strong></td>\r\n </tr>\r\n";
                s.Append(data);
            }                    
            string footer="\r\n\r\n <tr>\r\n<td colspan=\"6\" valign=\"top\">&nbsp;</td>\r\n</tr>\r\n<tr>\r\n                        <td colspan=\"5\" align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>Subtotal : </strong></td>\r\n                        <td align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>"+invoicedata.Amount+"</strong></td>\r\n</tr>\r\n<tr>\r\n<td colspan=\"5\" align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>Shipping &amp; Handling :</strong></td>\r\n                        <td align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>"+invoicedata.ShippingAmount+"</strong></td>\r\n                    </tr>\r\n<tr>\r\n<td colspan=\"5\" align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>Grand Total : </strong></td>\r\n  <td align=\"right\" valign=\"top\" style=\"padding:5px 10px; line-height:20px; font-family:Arial, Helvetica, sans-serif; font-size:14px\"><strong>"+((invoicedata.ShippingAmount)+(invoicedata.Amount))+"</strong></td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n <tr>\r\n</table>\r\n</body>\r\n</html>";
            s.Append(footer);


            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 1024;
            converter.Options.WebPageHeight = 0;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 0;
            converter.Options.WebPageFixedSize = false;
            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.DisplayOnFirstPage = true;
            converter.Footer.DisplayOnOddPages = true;
            converter.Footer.DisplayOnEvenPages = true;
            //converter.Footer.Height = 50;
            PdfHtmlSection footerHtml = new PdfHtmlSection(
             "<div><table style='margin-left: auto;margin-right: auto;'><tr><td><img src='" + SiteKey.ImagePath + "Uploads/1d0178bb-c700-4db0-a4ab-7036f17b00ed_dsfavicon' alt='' style='height: 15px; width: 15px;'></td>" +
             "<td style='font-size:12px;font-family: Arial, Helvetica, sans-serif;'> © Copyright 2023 Dotsquares Technologies (I) Pvt. Ltd. All Rights Reserved.</td></tr></table>" +
             "</div>",
             string.Empty);
            converter.Footer.Add(footerHtml);
            string fileSavePath =SiteKey.UploadImage;
          
            PdfDocument doc = converter.ConvertHtmlString(s.ToString());
               
                // save pdf document
                doc.Save(fileSavePath+ invoicedata.OrderId + "_Invoice.pdf");
                sb.Append("after save function");
                using (StreamWriter sw = new StreamWriter(logfilename, true))
                {
                    sw.WriteLineAsync(sb.ToString());
                    sw.Close();
                }
                // close pdf document
                doc.Close();
            string fullFilePath = SiteKey.ImagePath + "/Uploads/" + invoicedata.OrderId + "_Invoice.pdf";
            return Ok(new { error = false, data = fullFilePath,message = "report fetch successfully!", code = 200, status = true });
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("catch save function");
                using (StreamWriter sw = new StreamWriter(logfilename, true))
                {
                    sw.WriteLineAsync(sb.ToString());
                    sw.Close();
                }
                var errorData = new { error = true, message = ex.Message, code = 401, status = false };
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
                emailTemplate = _templateEmailService.GetById((int)EmailType.OrderStatus);
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
                    d.Add(new KeyValuePair<string, string>("##OrederStatus##", order.Status));
                    d.Add(new KeyValuePair<string, string>("##UserName##", order.Firstname + ' ' + order.Lastname));
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
                            string Image = "";
                            var productImage = _productImagesService.GetById(item.ProductId);
                            if (productImage != null)
                            {
                                 Image = productImage.ImageName;
                            }
                            else
                            {
                                Image = SiteKey.DefaultImage;
                            }
                            productList += "<html><head></head><body><div style='border: 5px solid #D3D3D3; height:150px; width:500px;margin:auto;'><div style='height: 150px; width:150px;float:left;'><div><img src='" + SiteKey.ImagePath + "/Uploads/" + Image + "' alt='ProductImage' height='150' width='150' style='height:150px; width:150px;'></div></div><div style='background-color:#F1F1F1; height: 150px; width:350px;float:left;'><div style='padding-left:50px;font-weight:bold;'><table><tr><td>Title :</td><td>" + product.Title + "</td></tr><tr><td>Quantity :</td><td>" + item.Quantity + "</td></tr><tr><td>Amount :</td><td>" + item.Price + "</td></tr><tr><td>Price :</td><td>" + (item.Price * item.Quantity) + "</td></tr></table></div></div></div></body></html>";
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
                    _emailSenderService.SendEmailAsync(order.Email, subject, urlToClick);
                    isSendEmail = true;
                }
            }
            return isSendEmail;
        }
        #endregion

    }
}
