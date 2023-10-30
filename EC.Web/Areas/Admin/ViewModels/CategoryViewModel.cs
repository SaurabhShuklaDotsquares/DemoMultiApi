using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            TitleList = new List<SelectListItem>();
        }
        public int? Id { get; set; }
        [DisplayName("Parent")]
        public int? ParentId { get; set; }
        //public int? SellerId { get; set; }
        [DisplayName("Featured")]
        public bool IsFeatured { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage ="Please select title")]
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ParentName{ get; set; }
        public string hdnUploadImage { get; set; }

        [Display(Name = "Image")]
        public IFormFile CategoryPicture { get; set; }
        [Required(ErrorMessage = "Please fill commission")]
        [Range(0, 99.99, ErrorMessage = "The admin commission must be between 0 to 99.99.")]
        public decimal? Commission { get; set; }

        public List<SelectListItem> TitleList { get; set; }
    }
}
