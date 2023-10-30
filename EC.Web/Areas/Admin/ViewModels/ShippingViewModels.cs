using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class ShippingViewModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please fill minimum order amount")]
        public string MinimumOrderAmount { get; set; }
        [Required(ErrorMessage = "Please fill maximum order amount")]
        public string MaximumOrderAmount { get; set; }
        [Required(ErrorMessage = "Please fill shipping charge")]
        public string ShippingCharge { get; set; }
        public bool Status { get; set; }
        public long? SupersetId { get; set; }
        public string CountryCode { get; set; }
        public string Sortname { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public long region_id { get; set; }
        public decimal? WeightFrom { get; set; }
        public decimal? WeightTo { get; set; }
        public int? ZipFrom { get; set; }
        public int? ZipTo { get; set; }
        public int? Zip { get; set; }
        public string ZipCode { get; set; }
        public decimal? Price { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> StatesList { get; set; }
    }

    public class ShippingViewModel
    {
        public string MinimumOrderAmount { get; set; }
        public string MaximumOrderAmount { get; set; }
        public string ShippingCharge { get; set; }
        public bool Status { get; set; }
    }
}
