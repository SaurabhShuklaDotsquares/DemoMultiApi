using EC.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class OrdersViewModel
    {

        public OrdersViewModel()
        {

            OrdersStatusTypeList = new List<SelectListItem>();
        }
        public long Id { get; set; }
        public string OrderId { get; set; }
        public int? UserId { get; set; }
        public int? SellerId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string BillingAddress { get; set; }
        public string ShipingAddress { get; set; }
        public string VoucherCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal Amount { get; set; }
        public string ShippingType { get; set; }
        public decimal? ShippingAmount { get; set; }
        public double? AdminCommission { get; set; }
        public double? SellerCommission { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionResponse { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductId { get; set; }
        public string VariantId { get; set; }
        public string VariantSlug { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Tax { get; set; }
        public List<orderItems> orderItems { get; set; }
        public List<billingAddress> billingAddress { get; set; }
        public List<shippingAddress> shippingAddress { get; set; }
        public string Total { get; set; }
        public string SubTotal { get; set; }
        public string ? GrandTotal { get; set; }
        public string varientText { get; set; }
        
        //public string Comment { get; set; }

        public List<SelectListItem> OrdersStatusTypeList { get; set; }
    }

    public class billingAddress
    {
        public string address { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }

    }
    public class shippingAddress
    {
        public string address2 { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }

    }

    public class orderItems
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Tax { get; set; }
        public string VariantId { get; set; }
        public string VariantSlug { get; set; }
        public decimal Total { get; set; }
        public string productname { get; set; }

    }

}
