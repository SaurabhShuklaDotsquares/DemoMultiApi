using System;
using System.ComponentModel.DataAnnotations;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class PagesViewModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter page title")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string Title { get; set; }
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string SubTitle { get; set; }
        public string Slug { get; set; }
        // [Required(ErrorMessage = "Please enter page short description!")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "Please enter page description")]
        public string Description { get; set; }
        public string Banner { get; set; }
        [Required(ErrorMessage = "Please enter page meta title")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string MetaTitle { get; set; }
        [Required(ErrorMessage = "Please enter page meta keyword")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string MetaKeyword { get; set; }
        [Required(ErrorMessage = "Please enter page meta description")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Input must be between 5 and 100 characters.")]
        public string MetaDescription { get; set; }
        public string Position { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
