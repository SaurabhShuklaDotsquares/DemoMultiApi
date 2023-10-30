namespace EC.API.ViewModels.MultiVendor
{
    public class DashboardVendorViewModels
    {
        public string from_date { get; set; }
        public string to_date { get; set; }
    }
    public class dashboardvendor
    {
        public decimal total_sale { get; set; }
        public int order { get; set; }
        public int total_products { get; set; }
        public int reviews { get; set; }
        public int contact_query { get; set; }
    }
}
