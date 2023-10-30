using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EC.Service.Specification
{
    public class ToDoSearchSpecs: GenricSearchSpaces
    {
        public string Search { get; set; }
    }
}
