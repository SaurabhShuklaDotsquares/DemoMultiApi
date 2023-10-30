using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class Payment
    {
        public int Id { get; set; }
        public long? OrderId { get; set; }
        public int? UserId { get; set; }
        public bool? Status { get; set; }
        public string TransactionId { get; set; }
        public string MethodType { get; set; }
        public string PaymentStatus { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Users User { get; set; }
    }
}
