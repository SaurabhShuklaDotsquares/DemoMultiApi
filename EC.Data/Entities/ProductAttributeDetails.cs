using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class ProductAttributeDetails
    {
        public ProductAttributeDetails()
        {
            ProductAttributeImages = new HashSet<ProductAttributeImages>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string AttributeSlug { get; set; }
        public string VariantText { get; set; }
        public decimal? RegularPrice { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? StockClose { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Products Product { get; set; }
        public virtual ICollection<ProductAttributeImages> ProductAttributeImages { get; set; }
    }
}
