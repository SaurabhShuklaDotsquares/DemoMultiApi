using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class BrandsViewModel
    {
        public int Id { get; set; }
        public bool IsFeatured { get; set; }
        // [StringLength(500, MinimumLength = 0)]
        //[StringLength(500, MinimumLength = 0, ErrorMessage = "Input must be between 0 and 500 characters.")]
        [Required(ErrorMessage = "Pease fill Title")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public byte ApprovalStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Image")]
        public IFormFile BrandPicture { get; set; }
       
    }
}
