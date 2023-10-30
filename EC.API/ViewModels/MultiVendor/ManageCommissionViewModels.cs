using System;
using System.Collections.Generic;

namespace EC.API.ViewModels.MultiVendor
{
    public class ManageCommissionViewModels
    {
        public int id { get; set; }
        public int? parent_id { get; set; }
        public int? seller_id { get; set; }
        public bool is_featured { get; set; }
        public decimal? admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public string banner { get; set; }
        public string meta_title { get; set; }
        public string meta_keyword { get; set; }
        public string meta_description { get; set; }
        public int status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public int? depth { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string image_link { get; set; }
    }

    public class ManageCommission
    {
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
        public List<ManageCommissionViewModels> data { get; set; } = new List<ManageCommissionViewModels>();

    }

    public class ManageExcelExport
    {
        public string OrderID { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string UserEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalTax { get; set; }
        public decimal ReturnAmount { get; set; }
        public string ShippingCharge { get; set; }
        public string TotalEarnings { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
    }
    public class ReturnExcelImportViewModel
    {
        public string address2 { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }

    }
}
