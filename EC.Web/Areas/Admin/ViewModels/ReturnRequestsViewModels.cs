using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class ReturnRequestsViewModels
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public int? UserId { get; set; }
        public int? SellerId { get; set; }
        public string CustomerName { get; set; }
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
        public string? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductId { get; set; }
        public string VariantId { get; set; }
        public string VariantSlug { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Tax { get; set; }
        public List<billingAddress> billingAddress { get; set; }
        public List<shippingAddress> shippingAddress { get; set; }
        public decimal Total { get; set; }
        public decimal? GrandTotal { get; set; }
        public string varientText { get; set; }
        public List<orderItem> orderItem { get; set; }
        public List<SelectListItem> ReturnStatusTypeList { get; set; }
    }
    public class orderItem
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
