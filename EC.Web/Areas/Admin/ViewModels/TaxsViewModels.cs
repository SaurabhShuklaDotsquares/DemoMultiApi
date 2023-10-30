using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class TaxsViewModels
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "*required")]
        public string Title { get; set; }
        public string ParentName { get; set; }
        [Required(ErrorMessage = "*required")]
        public int CategoryId { get; set; }
        public int[] SubCategoryId { get; set; }
        [Required(ErrorMessage = "*required")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a value less than or equal to 100.")]
        public decimal? Value { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<SelectListItem> TitleList { get; set; }
        public List<SelectListItem> SubCategoryList { get; set; }
        public string hdnSubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}
