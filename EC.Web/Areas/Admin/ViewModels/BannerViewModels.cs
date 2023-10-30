using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class BannerViewModels
    {
        public BannerViewModels()
        {
            this.GetDeviceType = new List<SelectListItem>();
            this.GetPositionType = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "*required")]

        public int Group { get; set; }
        public string Link { get; set; }
        [Required(ErrorMessage = "Please fill Title")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 200 characters.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Pease fill Sub title")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 200 characters.")]
        public string Subtitle { get; set; }
        [Required(ErrorMessage = "*required")]

        public int Type { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = "*required")]

        public string DeviceType { get; set; }
        public int IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Display(Name = "Image")]
        public IFormFile BannerPicture { get; set; }
        public string Typ { get; set; }
        public string Stats { get; set; }
        public List<SelectListItem> GetDeviceType { get; set; }
        public List<SelectListItem> GetPositionType { get; set; }
    }
}
