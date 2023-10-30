using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class OrdersBkp
    {
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
    }
}
