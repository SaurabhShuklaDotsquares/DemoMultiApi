using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class OptionValues
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public string Title { get; set; }
        public string Hexcode { get; set; }
        public int SortOrder { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Options Option { get; set; }
    }
}
