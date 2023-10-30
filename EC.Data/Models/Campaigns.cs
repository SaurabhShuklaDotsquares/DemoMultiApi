using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Campaigns
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Template { get; set; }
        public string Users { get; set; }
        public string Progress { get; set; }
        public string Failed { get; set; }
        public string GroupId { get; set; }
        public string ErrorLog { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
