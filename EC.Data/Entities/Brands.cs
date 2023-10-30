using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class Brands
    {
        public int Id { get; set; }
        public bool IsFeatured { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public byte ApprovalStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
