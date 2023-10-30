using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.API.ViewModels
{
    public class ReviewsViewModel
    {
        public int product_id { get; set; }
        public string order_id { get; set; }
        [Required]
        public byte rating { get; set; }
        public string comment { get; set; }
       
    }


    public class ReviewsreturnViewModel
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public int product_id { get; set; }
        public string order_id { get; set; }
        [Required]
        public byte rating { get; set; }
        public string comment { get; set; }
    }
    public class reviwdata
    {
        public int current_page { get; set; }
        public int total_page { get; set; }
        public int page_size { get; set; }
        public List<review> data { get; set; } = new List<review>();
    }

    public class review
    {
        public string comment { get; set; }
        public Order order { get; set; } = new Order();
        public P_roduct product { get; set; } = new P_roduct();
    }
    public class Order 
    {
        public string order_id { get; set; }
    }
    public class P_roduct
    {
        public string title { get; set; }
    }
}
