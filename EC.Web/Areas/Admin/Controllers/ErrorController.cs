using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ErrorController(IHttpContextAccessor _httpContextAccessor)
        {
            //_logger = logger;
            httpContextAccessor = _httpContextAccessor;
        }
        [Route("access-denied")]
        public IActionResult accessDenied()
        {
            Response.StatusCode = 403;
            return View("accessdenied");
        }

        //[Route("/error/404")]
        public IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("originalPath"))
            {
                originalPath = HttpContext.Items["originalPath"] as string;
            }
            return View("pagenotfound");
        }
    }
}
