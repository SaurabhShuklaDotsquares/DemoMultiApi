using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Shippingrates
    {
        public int Id { get; set; }
        public long? SupersetId { get; set; }
        public string CountryCode { get; set; }
        public long RegionId { get; set; }
        public decimal? WeightFrom { get; set; }
        public decimal? WeightTo { get; set; }
        public int? ZipFrom { get; set; }
        public int? ZipTo { get; set; }
        public int? Zip { get; set; }
        public decimal? Price { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
