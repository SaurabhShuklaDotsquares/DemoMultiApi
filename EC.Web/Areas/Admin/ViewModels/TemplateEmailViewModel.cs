using System;
using System.ComponentModel.DataAnnotations;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class TemplateEmailViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 200 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 200 characters.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "*required")]
        public string Description { get; set; }
        public string Status { get; set; }
        public string Slug { get; set; }
        public byte Locked { get; set; }
        public bool? Isdeleted { get; set; }
        public string RememberToken { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
