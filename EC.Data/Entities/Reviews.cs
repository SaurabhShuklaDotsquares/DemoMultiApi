using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Entities
{
    public partial class Reviews
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public long? OrderId { get; set; }
        public int ProductId { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
        public byte Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
    }
}
