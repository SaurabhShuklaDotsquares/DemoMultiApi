using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.ViewModels
{
    public class NewViewModel
    {
        public string Name { get; set; }
        public IFormFile VarientImage { get; set; }
        public string VarientImageName { get; set; }
    }
}
