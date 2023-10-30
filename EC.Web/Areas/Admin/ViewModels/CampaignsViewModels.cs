using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class CampaignsViewModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "*required")]
        public string Template { get; set; }
        public string Users { get; set; }
        public string Progress { get; set; }
        public int TotalUsers { get; set; }
        public int Success { get; set; }
        public string Failed { get; set; }
        public string GroupId { get; set; }
        public string ErrorLog { get; set; }
        public string Jsondata { get; set; }
        public bool Status { get; set; }
        public string showStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SelectListItem> TemplateList { get; set; }
    }
}
