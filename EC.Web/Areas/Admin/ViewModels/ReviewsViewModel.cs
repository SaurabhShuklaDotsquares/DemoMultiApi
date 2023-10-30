using System;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class ReviewsViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //public long? OrderId { get; set; }
        public int ProductId { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
        public byte Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string OrderId { get; set; }
        public string Name { get; set; }
        public string Productname { get; set; }
        public string StatusView { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        public string Mobile { get; set; }
    }
}
