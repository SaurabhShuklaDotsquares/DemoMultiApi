using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class CouponsViewModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
        
        public string Slug { get; set; }
        [Required(ErrorMessage = "*required")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Please enter coupon value amount.")]
        public string MaximumValue { get; set; }
        public string MaximumValue1 { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value less than to 9999999999.")]
        [Required(ErrorMessage = "This field is required.")]
        public decimal? Amount { get; set; }
        [Required(ErrorMessage = "Please select the start date")]
        public string StartDate { get; set; }
        [Required(ErrorMessage = "Please select the end date")]
        public string EndDate { get; set; }
        public string DateLimits { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public int? MaximumUsage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value less than or equal to 9999999999.")]
        public int? MaximumUsageValue { get; set; }
        public List<SelectListItem> CouponsTypeList { get; set; }
    }
}
