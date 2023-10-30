using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Options
    {
        public Options()
        {
            OptionValues = new HashSet<OptionValues>();
            ProductAttributes = new HashSet<ProductAttributes>();
        }

        public int Id { get; set; }
        public int? SellerId { get; set; }
        public string HeaderType { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public bool? Deletable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<OptionValues> OptionValues { get; set; }
        public virtual ICollection<ProductAttributes> ProductAttributes { get; set; }
    }
}
