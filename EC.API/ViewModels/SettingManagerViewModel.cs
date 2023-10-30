using System.Collections.Generic;

namespace EC.API.ViewModels
{
    public class SettingManagerViewModel
    {
        public Dictionary<string, string> general { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> theme_images { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> social { get; set; } = new Dictionary<string, string>();
        public List<CurrencyViewModel> currency { get; set; } = new List<CurrencyViewModel>();
    }

    public class CurrencyViewModel
    {
        public int id { get; set; }
        public int currency_id { get; set; }
        public CurrencyModel currency { get; set; }
    }
    public class CurrencyModel
    {
        public int id { get; set; }
        public string iso { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string symbol_native { get; set; }
    }
}
