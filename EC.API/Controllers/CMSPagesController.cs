using EC.API.ViewModels.SiteKey;
using EC.API.ViewModels;
using EC.Service;
using EC.Service.AllPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EC.API.Controllers.BaseAPIController;
using System.Collections.Generic;
using System;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CMSPagesController : BaseAPIController
    {
        #region Constructor
        private readonly IPagesService _pagesService;
       
        public CMSPagesController(IPagesService pagesService)
        {
            _pagesService = pagesService;
          
        }
        #endregion

        #region Get Cms Page Data List
        [Route("/users/page-all")]
        [HttpGet]
        public IActionResult PageAll()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                List<CmsPageViewModels> model = new List<CmsPageViewModels>();
                var cmspagesdata = _pagesService.Getlist();
                if (cmspagesdata != null)
                {
                    foreach (var item in cmspagesdata)
                    {
                        CmsPageViewModels model1 = new CmsPageViewModels();
                        model1.title = item.Title;
                        model1.slug = item.Slug;
                        model.Add(model1);
                    }
                    return Ok(new { error = false, data = model, message = "CMS fetched successfully.",code = 200, status = true });
                }
                var errorData = new { error = true, message = "This CMS not avilable.!", data = "null", code = 400, status = false };
                return new UnauthorizedResponse(errorData);
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
