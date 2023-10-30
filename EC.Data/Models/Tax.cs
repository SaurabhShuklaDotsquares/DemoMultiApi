using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Tax
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public decimal? Value { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string SubCategoryId { get; set; }

        public virtual Categories Category { get; set; }
    }
}
