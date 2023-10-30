using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class VendorDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string VatNo { get; set; }
        public string BusinessName { get; set; }
        public decimal? TransactionFee { get; set; }
        public string StripeAccount { get; set; }
        public string StripePublic { get; set; }
        public string StripeSecret { get; set; }
        public int? OrderReturnDays { get; set; }
        public string Reasons { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Users User { get; set; }
    }
}
