using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class AttributeViewModel
    {
        public AttributeViewModel()
        {
            OptionValuesList = new List<OptionValuesViewModel>();
        }
        public int Id { get; set; }
        public int? SellerId { get; set; }
        public string HeaderType { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Please Fill Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please select SortOrder")]
        public int SortOrder { get; set; }
        public bool Status { get; set; }
        public bool? Deletable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<string> TitleOrderValue { get; set; }
        public List<OptionValuesViewModel> OptionValuesList { get; set; }
    }
    public class OptionValuesViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please Fill Title")]
        public string title { get; set; }
        [Required(ErrorMessage = "Please select SortOrder")]
        public int  sortorder { get; set; }
    }


}
