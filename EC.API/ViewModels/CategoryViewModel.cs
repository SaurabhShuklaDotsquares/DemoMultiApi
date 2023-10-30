using System;
using System.Collections.Generic;

namespace EC.API.ViewModels
{
    public class CategoryParentViewModel
    {
        public int id { get; set; }
        public int is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public int status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string image_link { get; set; }
        public List<CategoryChildViewModel> children { get; set; } = new List<CategoryChildViewModel>();
    }

    public class CategoryChildViewModel
    {
        public int id { get; set; }
        public int parent_id { get; set; }
        public int is_featured { get; set; }
        public int admin_commission { get; set; }
        public string title { get; set; }
        public string slug { get; set; }
        public string image { get; set; }
        public int status { get; set; }
        public int approval_status { get; set; }
        public int? lft { get; set; }
        public int? rgt { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string image_link { get; set; }
    }
}
