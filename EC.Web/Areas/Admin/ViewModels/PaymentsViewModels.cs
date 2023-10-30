using EC.Data.Models;
using System;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class PaymentsViewModels
    {
        public int Id { get; set; }
        //public long? OrderId { get; set; }
        public int? UserId { get; set; }
        public bool? Status { get; set; }
        public string TransactionId { get; set; }
        public string MethodType { get; set; }
        public string PaymentStatus { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string OrderId { get; set; }
        public string Name{ get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        public string Mobile { get; set; }
        public virtual Orders Order { get; set; }
        public virtual Users User { get; set; }
    }
}
