using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class ReturnRequestsBkp
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int? UserId { get; set; }
        public string Amount { get; set; }
        public string RefundAmount { get; set; }
        public string BankAccountId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string TransactionId { get; set; }
    }
}
