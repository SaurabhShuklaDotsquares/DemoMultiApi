namespace EC.API.ViewModels.MultiVendor
{
    public class ReportVendorViewModels
    {

    }
    public class ReportManagervendor
    {
        public int total_completed_orders { get; set; }
        public int total_processing_orders { get; set; }
        public int total_returned_orders { get; set; }
        public decimal earnings { get; set; }
    }
}
