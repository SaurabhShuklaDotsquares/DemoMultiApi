using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels.MultiVendor
{
    public class OrderVendorViewModel
    {
      
    }
    public class OrderStatusViewModel
    {
        public string processing { get; set; }
        public string shipped { get; set; }
        public string completed { get; set; }
        public string pending_for_payment { get; set; }
        public string failed { get; set; }
        public string returned { get; set; }
        public string refunded { get; set; }
        public string cancelled { get; set; }
        public string pending { get; set; }
    }

    public class OrderViewModel
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? user_id { get; set; }
        public int vendor_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string voucher_code { get; set; }
        public decimal? discount_amount { get; set; }
        public decimal amount { get; set; }
        public string shipping_type { get; set; }
        public decimal? shipping_amount { get; set; }
        public double? admin_commission { get; set; }
        public double? seller_commission { get; set; }
        public DateTime? expected_delivery_date { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string transaction_id { get; set; }
        public string payment_method { get; set; }
        public string transaction_response { get; set; }
        public int payment_payed_to_vendor { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public decimal display_amount { get; set; }
        public decimal? display_discount_amount { get; set; }
        public decimal? display_shipping_amount { get; set; }
        public decimal display_total { get; set; }
        public string display_status { get; set; }
        public decimal sum_amount { get; set; }
        public decimal sum_ammout_return { get; set; }
        public BillingVendorAddressModel billing_address { get; set; } = new BillingVendorAddressModel();
        public ShippingVendorAddressModel shipping_address { get; set; } = new ShippingVendorAddressModel();
        public CurrencyDataModel currency { get; set; } = new CurrencyDataModel();
        public List<OrderItemModel> orderitems { get; set; } = new List<OrderItemModel>();

    }

    public class OrderViewUpdateModel
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? user_id { get; set; }
        public int? vendor_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string voucher_code { get; set; }
        public decimal? discount_amount { get; set; }
        public decimal amount { get; set; }
        public string shipping_type { get; set; }
        public decimal? shipping_amount { get; set; }
        public double? admin_commission { get; set; }
        public double? seller_commission { get; set; }
        public DateTime? expected_delivery_date { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string transaction_id { get; set; }
        public string payment_method { get; set; }
        public string transaction_response { get; set; }
        public int payment_payed_to_vendor { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public decimal display_amount { get; set; }
        public decimal? display_discount_amount { get; set; }
        public decimal? display_shipping_amount { get; set; }
        public decimal display_total { get; set; }
        public string display_status { get; set; }
        public decimal sum_amount { get; set; }
        public decimal sum_ammout_return { get; set; }
        public BillingVendorAddressModel billing_address { get; set; } = new BillingVendorAddressModel();
        public ShippingVendorAddressModel shipping_address { get; set; } = new ShippingVendorAddressModel();
        public CurrencyDataModel currency { get; set; } = new CurrencyDataModel();
        public UserModel user { get; set; } = new UserModel();

    }

    public class Reurnmainmodel
    {
        public ReturnViewUpdateModel order_data { get; set; } = new ReturnViewUpdateModel();
        public return_item return_item { get; set; } = new return_item();
    }

    public class ReturnViewUpdateModel
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? user_id { get; set; }
        public int? vendor_id { get; set; }
        public string amount { get; set; }
        public string refund_amount { get; set; }
        public string bank_account_id { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string refund_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string sum_amount { get; set; }
        public ReturnOrderViewModel order { get; set; } = new ReturnOrderViewModel();
        public BillingVendorAddressModel billing_address { get; set; } = new BillingVendorAddressModel();
        public ShippingVendorAddressModel shipping_address { get; set; } = new ShippingVendorAddressModel();
        public CurrencyDataModel currency { get; set; } = new CurrencyDataModel();


    }

    public class return_item
    {
        public int id { get; set; }
        public int request_id { get; set; }
        public int order_item_id { get; set; }
        public string return_quantity { get; set; }
        public string return_status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }






    public class UserModel
    {
        public int id { get; set; }
        public int role { get; set; }
        public string firstname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string lastname { get; set; }
        public string profile_pic { get; set; }
        public string state { get; set; }
        public DateTime? email_verified_at { get; set; }
        public int isVerified { get; set; }
        public int is_admin { get; set; }
        public string stripe_customer_id { get; set; }
        public string stripe_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string country { get; set; }
        public int status { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public bool is_guest { get; set; }
    }

    public class BillingAddressModel
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }
    }
    public class BillingVendorAddressModel
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }
        public string countryName { get; set; }
    }

    public class ShippingVendorAddressModel
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }
        public string countryName { get; set; }
    }
    public class ShippingAddressModel
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }
    }

    public class CurrencyDataModel
    {
        public int id { get; set; }
        public int currency_id { get; set; }
        public int is_primary { get; set; }
        public int live_rate { get; set; }
        public int converted_rate { get; set; }
        public CurrencyModel currency { get; set; } = new CurrencyModel();
    }

    public class CurrencyModel
    {
        public int id { get; set; }
        public string iso { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string symbol_native { get; set; }
    }

    public class OrderItemModel
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int? seller_id { get; set; }
        public int? vendor_id { get; set; }
        public int product_id { get; set; }
        public string variant_id { get; set; }
        public string variant_slug { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string tax { get; set; }
        public double? admin_commission { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int is_review { get; set; }
        public decimal display_price { get; set; }
        public decimal display_total_price { get; set; }
        public OrderProductModel product { get; set; } = new OrderProductModel();
    }

    public class OrderListModel
    {
        public List<OrderViewModel> data { get; set; } = new List<OrderViewModel>();
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
    }

    public class OrderListResponseModel
    {
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

    public class OrderProductModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string sku { get; set; }
        public string slug { get; set; }
        public int? brand_name { get; set; }
        public string banner_link { get; set; }
        public string banner_image { get; set; }
        public string prod_description { get; set; }
        public int average_rating { get; set; }
        public string url { get; set; }
        public string display_price { get; set; }
        public string display_discounted_price { get; set; }
    }
    public class TaxModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public int category_id { get; set; }
        public string sub_category_id { get; set; }
        public decimal? value { get; set; }
        public int status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public class ReturnRequestOrderViewModel
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int? user_id { get; set; }
        public decimal amount { get; set; }
        public decimal? refund_amount { get; set; }
        public string bank_account_id { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string refund_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public decimal sum_amount { get; set; }
        public ReturnOrderViewModel order { get; set; } = new ReturnOrderViewModel();
        public UserModel user { get; set; } = new UserModel();
        public List<ReturnItemViewModel> return_items { get; set; } = new List<ReturnItemViewModel>();
    }

    public class ReturnOrderViewModel
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? user_id { get; set; }
        public int vendor_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string voucher_code { get; set; }
        public decimal? discount_amount { get; set; }
        public decimal amount { get; set; }
        public string shipping_type { get; set; }
        public decimal? shipping_amount { get; set; }
        public double? admin_commission { get; set; }
        public double? seller_commission { get; set; }
        public DateTime? expected_delivery_date { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string transaction_id { get; set; }
        public string payment_method { get; set; }
        public string transaction_response { get; set; }
        public int payment_payed_to_vendor { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public decimal display_amount { get; set; }
        public decimal? display_discount_amount { get; set; }
        public decimal? display_shipping_amount { get; set; }
        public decimal display_total { get; set; }
        public string display_status { get; set; }
        public decimal sum_amount { get; set; }
        public decimal sum_ammout_return { get; set; }
        public BillingVendorAddressModel billing_address { get; set; } = new BillingVendorAddressModel();
        public ShippingVendorAddressModel shipping_address { get; set; } = new ShippingVendorAddressModel();
        public CurrencyDataModel currency { get; set; } = new CurrencyDataModel();
    }
    public class ReturnItemViewModel
    {
        public int id { get; set; }
        public int? request_id { get; set; }
        public int? order_item_id { get; set; }
        public int return_quantity { get; set; }
        public string return_status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public OrderItemModel order_item { get; set; } = new OrderItemModel();
    }

    public class ReturnStatusModel
    {
        public string New { get; set; }
        public string accepted { get; set; }
        public string declined { get; set; }
        public string refunded { get; set; }
    }

    public class OrderListReturnModel
    {
        public List<OrderStatusViewModel> orderStatus { get; set; } = new List<OrderStatusViewModel>();
        public OrderListModel orderitems { get; set; } = new OrderListModel();
    }

    public class OrderViewReturnModel
    {
        public OrderStatusViewModel orderStatuses { get; set; } = new OrderStatusViewModel();
        public OrderViewModel order { get; set; } = new OrderViewModel();
        public TaxModel taxClass { get; set; }
    }

    public class OrderUpdateRequestModel
    {
        [Required(ErrorMessage ="Order id required")]
        public int id { get; set; }
        [Required(ErrorMessage = "Order status required")]
        public string status { get; set; }
        public string comment { get; set; }
    }
    public class ReturnUpdateRequestModel
    {
        [Required(ErrorMessage = "Order id required")]
        public int id { get; set; }
        [Required(ErrorMessage = "Order status required")]
        public string status { get; set; }
        public string message { get; set; }
    }
    public class ReturnOrderReturn
    {
        public List<ReturnRequestOrderViewModel> data { get; set; } = new List<ReturnRequestOrderViewModel>();
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
    }

    public class ReturnOrderReturnModel
    {
        public ReturnOrderReturn returnRequests { get; set; } = new ReturnOrderReturn();
        public ReturnStatusModel returnStatus { get; set; } = new ReturnStatusModel();
    }

    public class ReturnOrderShowReturnModel
    {
        public ReturnRequestOrderViewModel returnRequest { get; set; } = new ReturnRequestOrderViewModel();
        public ReturnStatusModel returnStatus { get; set; } = new ReturnStatusModel();
    }

    public class RefundOrderRequestModel
    {
        [Required(ErrorMessage = "Required id")]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be a positive number.")]
        public int id { get; set; }
        [Required(ErrorMessage = "Required amount")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only numeric values are allowed.")]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be a positive number.")]
        public decimal amount { get; set; }
    }
}
