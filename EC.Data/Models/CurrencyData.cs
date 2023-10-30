﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class CurrencyData
    {
        public long Id { get; set; }
        public int CurrencyId { get; set; }
        public bool IsPrimary { get; set; }
        public string LiveRate { get; set; }
        public string ConvertedRate { get; set; }
        public string Response { get; set; }
        public DateTime? LastRateUpdate { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Currencies Currency { get; set; }
    }
}
