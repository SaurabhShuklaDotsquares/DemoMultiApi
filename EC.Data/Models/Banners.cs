using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Banners
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int Group { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int Type { get; set; }
        public bool? Status { get; set; }
        public string DeviceType { get; set; }
        public int IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
