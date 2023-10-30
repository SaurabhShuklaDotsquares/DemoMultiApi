using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.Serialization;
using SixLabors.ImageSharp.Metadata;

namespace EC.API.ViewModels
{
    //public class OrdersViewModels
    //{
    //    public OrdersViewModels()
    //    {

    //        // OrdersStatusTypeList = new List<SelectListItem>();
    //    }
    //    //public long Id { get; set; }
    //    public string OrderId { get; set; }
    //    public int? UserId { get; set; }
    //    public int? SellerId { get; set; }
    //    public string Firstname { get; set; }
    //    public string Lastname { get; set; }
    //    public string Email { get; set; }
    //    public string Mobile { get; set; }
    //    public string BillingAddress { get; set; }
    //    public string ShipingAddress { get; set; }
    //    public string VoucherCode { get; set; }
    //    public decimal? DiscountAmount { get; set; }
    //    public decimal Amount { get; set; }
    //    public string ShippingType { get; set; }
    //    public decimal? ShippingAmount { get; set; }
    //    public double? AdminCommission { get; set; }
    //    public double? SellerCommission { get; set; }
    //    public DateTime? ExpectedDeliveryDate { get; set; }
    //    public string Status { get; set; }
    //    public string Message { get; set; }
    //    public string TransactionId { get; set; }
    //    public string PaymentMethod { get; set; }
    //    public string TransactionResponse { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //    public int ProductId { get; set; }
    //    public string VariantId { get; set; }
    //    public string VariantSlug { get; set; }
    //    public int Quantity { get; set; }
    //    public decimal Price { get; set; }
    //    public string Tax { get; set; }
    //    public List<orderItems> orderItems { get; set; }
    //    public List<billingAddress> billingAddress { get; set; }
    //    public List<shippingAddress> shippingAddress { get; set; }
    //    public decimal Total { get; set; }
    //    public decimal? GrandTotal { get; set; }
    //    public string varientText { get; set; }
    //    public List<SelectListItem> OrdersStatusTypeList { get; set; }
    //}
    public enum paymentmethod
    {
        cod,
        online,
    }
    public class shipping_address
    {
        [Required]
        public string address { get; set; }
        [Required]
        public string address2 { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Postal code should be greater than 0")]
        public int postal_code { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Country id should be greater than 0")]
        public int country { get; set; }

    }
    public class Billing_address
    {
        [Required]
        public string address { get; set; }
        public string address2 { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public int postal_code { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public int country { get; set; }

    }

    public class OrdersViewModel
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public string shipping_addres { get; set; }
        public string shipping_address2 { get; set; }
        public string shipping_type { get; set; }
        public string payment_method { get; set; }
        public int shipping_amount { get; set; }
        public decimal price { get; set; }
        public string token { get; set; }
        public shipping_address shipping_address { get; set; }
        public Billing_address billing_address { get; set; }
        public string promocode { get; set; }
    }
    //public class OrdersViewModel
    //{
    //    [Required(ErrorMessage = "Please Fill First Name")]
    //    public string firstname { get; set; }
    //    [Required(ErrorMessage = "Please Fill last Name")]
    //    public string lastname { get; set; }
    //    [RegularExpression(@"^[0-9]{9,16}$", ErrorMessage = "Mobile no must be between 9-16 digit.")]
    //    public int mobile { get; set; }
    //    [Required(ErrorMessage = "Please fill address")]
    //    public string shipping_address1 { get; set; }
    //    public string shipping_address2 { get; set; }
    //    [Required(ErrorMessage = "Please fill state")]
    //    public string shipping_state { get; set; }
    //    [Required(ErrorMessage = "Please fill postal_code")]
    //    public int shipping_postal_code { get; set; }
    //    [Required(ErrorMessage = "Please fill city")]
    //    public string shipping_city { get; set; }
    //    [Required(ErrorMessage = "Please fill country")]
    //    public string shipping_country { get; set; }
    //    [Required(ErrorMessage = "Please fill shippingtype")]
    //    public string shipping_type { get; set; }
    //    [Required(ErrorMessage = "Please fill paymentmethod")]
    //    public string payment_method { get; set; }
    //    public string? shipping_amount { get; set; }
    //    //public string price { get; set; }
    //    public string token { get; set; }
    //}
    public class billingAddress
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }

    }
    public class shippingAddress
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }
        //public string PaymentMethod { get; set; }
    }
    public class orderItems
    {
        public int Id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string tax { get; set; }
        public string variantId { get; set; }
        public string variantSlug { get; set; }
        public decimal total { get; set; }
        public string productname { get; set; }
    }


    public class OrderList1
    {
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
        public List<OrderModel> data { get; set; } = new List<OrderModel>();

    }


    public class OrderList
    {
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
        public OrderModel data { get; set; }
    }

    public class PagingModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class OrderDetail1
    {
        public OrderDetail order { get; set; }
    }
    public class OrderDetail
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? user_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public BillingAddress billing_address { get; set; }
        //public DateTime OrderDate { get; set; }
        // public string OrderStatus { get; set; }
        //public decimal ShippingAmount { get; set; }
        //public decimal TotalAmount { get; set; }
        public string? discount_amount { get; set; }
        public string amount { get; set; }
        public string shipping_type { get; set; }
        public decimal? shipping_amount { get; set; }
        public string status { get; set; }
        public string payment_method { get; set; }
        //public string business_name { get; set; }
        public string transaction_response { get; set; }
        public CurrencyViewModel currency { get; set; } = new CurrencyViewModel();
        public string payment_payed_to_vendor { get; set; }
        public DateTime? created_at { get; set; }
        public string? display_amount { get; set; }
        public string display_discount_amount { get; set; }
        public string? display_shipping_amount { get; set; }
        public string display_total { get; set; }
        public string display_status { get; set; }
        public decimal sum_amount { get; set; }
        public decimal sum_ammout_return { get; set; }
        public List<OrderitemModel1> orderitems { get; set; } = new List<OrderitemModel1>();
        public Payament payment { get; set; } = new Payament();

    }
    public class Payament
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int user_id { get; set; }
        public int vendor_id { get; set; }
        public int status { get; set; }
        public string transaction_id { get; set; }
        public string method_type { get; set; }
        public string payment_status { get; set; }
        public string currency_code { get; set; }
        public string? amount { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    public class ShippingAddress
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }
        public string countryName { get; set; }
        //public string PaymentMethod { get; set; }
    }

    public class BillingAddress
    {
        public string address { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }

        public string countryName { get; set; }
        //public string PaymentMethod { get; set; }
    }

    public class ProductimageDetail
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
        //public decimal Price { get; set; }
    }
    public class ProductDetail
    {
        public string Title { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
        public string sku { get; set; }
        public string brand_name { get; set; }
        public string prod_description { get; set; }
        public string average_rating { get; set; }
        public decimal display_price { get; set; }
        public decimal display_discounted_price { get; set; }
        public List<ProductimageDetail> product_image { get; set; } = new List<ProductimageDetail>();
    }

    public class ReturnOrderModel
    {
        [Required(ErrorMessage = "Items Requird.")]
        public int[] item { get; set; }
        [Required(ErrorMessage = "Order Id Requird.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order should be greater than 0")]
        public int order_id { get; set; }
        [Required(ErrorMessage = "Quantity required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity should be greater than 0")]
        public int return_quantity { get; set; }
        public string status { get; set; }
    }
    public class Orders_Id
    {
        public List<Order_Id> order_Ids { get; set; } = new List<Order_Id>();
    }
    public class Order_Id
    {
        public string order_id { get; set; }
    }
    public class Order_products
    {
        public int product_id { get; set; }
        public int order_id { get; set; }
    }

    public class productsOrder
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<reviewsall> reviews_all { get; set; } = new List<reviewsall>();
        public List<productimage> product_image { get; set; } = new List<productimage>();


    }
    public class productimage
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
    }
    public class reviewsall
    {

    }

    public class OrderModel
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public decimal shipping_amount { get; set; }
        public string discount_amount { get; set; }
        public decimal amount { get; set; }
        public string transaction_id { get; set; }
        public string transaction_response { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public string display_amount { get; set; }
        public string display_discount_amount { get; set; }
        public string display_shipping_amount { get; set; }
        public string display_total { get; set; }
        public string display_status { get; set; }
        public decimal sum_amount { get; set; }
        public int sum_ammout_return { get; set; }
        public List<OrderitemModel> orderitems { get; set; } = new List<OrderitemModel>();
    }
    public class OrderitemModel
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? seller_id { get; set; }
        public int product_id { get; set; }
        public string variant_id { get; set; }

        public string variant_slug { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string tax { get; set; }
        public int admin_commission { get; set; }
        public int is_review { get; set; }
        public string display_price { get; set; }
        public string display_total_price { get; set; }
        public ProductsModel product { get; set; } = new ProductsModel();

        public string returnrequest { get; set; }
    }

    public class OrderitemModel1
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public int? seller_id { get; set; }
        public int product_id { get; set; }
        public string variant_id { get; set; }

        public string variant_slug { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string tax { get; set; }
        public int admin_commission { get; set; }
        public int is_review { get; set; }
        public string display_price { get; set; }
        public string display_total_price { get; set; }
        public ProductDetail product { get; set; } = new ProductDetail();
        public product_attribute_details product_attribute_detail { get; set; } = new product_attribute_details();
        public ReturnRequest returnrequest { get; set; } = new ReturnRequest();
    }
    public class ReturnRequest
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int? user_id { get; set; }
        public string amount { get; set; }
        public string refund_amount { get; set; }
        public string bank_account_id { get; set; }
        public string message { get; set; }
        public string return_status { get; set; }
        public string refund_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string sum_amount { get; set; }
    }

    public class product_attribute_details
    {

    }
    public class ProductsModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string sku { get; set; }
        public string slug { get; set; }
        public int brand_name { get; set; }
        public string prod_description { get; set; }
        public string average_rating { get; set; }
        public string url { get; set; }
        public decimal display_price { get; set; }
        public decimal display_discounted_price { get; set; }
        public List<ProductImageModel> product_image { get; set; } = new List<ProductImageModel>();

    }

    public class Products_Model
    {
        public int id { get; set; }
        public string title { get; set; }
        public string sku { get; set; }
        public string slug { get; set; }
        public int brand_name { get; set; }
        public string prod_description { get; set; }
        public string average_rating { get; set; }
        public string url { get; set; }
        public decimal display_price { get; set; }
        public decimal display_discounted_price { get; set; }
        public List<ProductImageModel> product_image { get; set; } = new List<ProductImageModel>();

    }

    public class ProductImageModel
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string image_name { get; set; }
        public string image_link { get; set; }
    }

    public class CancelOrderModel
    {
        public long id { get; set; }
        public string order_id { get; set; }
        public int? user_id { get; set; }
        public int? seller_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public CancleOrderBillingAddress billing_address { get; set; }
        public CancleOrderShippingAddress shipping_address { get; set; }
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
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string display_amount { get; set; }
        public string display_discount_amount { get; set; }
        public string display_shipping_amount { get; set; }
        public string display_total { get; set; }
    }

    public class CancleOrderBillingAddress
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }
    }

    public class CancleOrderShippingAddress
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public int postal_code { get; set; }
        public string city { get; set; }
        public int country { get; set; }
    }

    public class CurrencyDataModels
    {
        public long id { get; set; }
        public int currency_id { get; set; }
        public int is_primary { get; set; }
        public string live_rate { get; set; }
        public string converted_rate { get; set; }
        public CurrencyModels currency { get; set; }
    }

    public class CurrencyModels
    {
        public int id { get; set; }
        public string iso { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string symbol_native { get; set; }
    }

}