using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class OrderItems
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int? SellerId { get; set; }
        public int ProductId { get; set; }
        public string VariantId { get; set; }
        public string VariantSlug { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Tax { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
