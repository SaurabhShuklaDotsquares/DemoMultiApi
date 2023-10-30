using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace EC.Data.Models
{
    public partial class Currencies
    {
        public Currencies()
        {
            CurrencyData = new HashSet<CurrencyData>();
        }

        public int Id { get; set; }
        public string Iso { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string SymbolNative { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<CurrencyData> CurrencyData { get; set; }
    }
}
