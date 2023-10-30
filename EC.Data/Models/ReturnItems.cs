using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class ReturnItems
    {
        public long Id { get; set; }
        public long? RequestId { get; set; }
        public long? OrderItemId { get; set; }
        public string ReturnQuantity { get; set; }
        public string ReturnStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual OrderItems OrderItem { get; set; }
        public virtual ReturnRequests Request { get; set; }
    }
}
