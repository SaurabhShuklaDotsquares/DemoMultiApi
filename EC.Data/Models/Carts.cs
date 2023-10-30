using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Carts
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int ProductId { get; set; }
        public int? SellerId { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? VariantId { get; set; }
        public string VariantSlug { get; set; }
        public decimal? FinalValue { get; set; }

        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
        public virtual ProductAttributeDetails Variant { get; set; }
    }
}
