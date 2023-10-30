using EC.Service;
using EC.Service.Specification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Collections.Generic;
using System;
using ToDo.WebApi.Models;
using EC.API.ViewModels;
using EC.API.ViewModels.SiteKey;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BannersController : BaseAPIController
    {
        #region Constructor
        private IBannersService _bannerservice;
        public BannersController(IBannersService bannerservice)
        {
            _bannerservice= bannerservice;
        }
        #endregion

        #region Banner List

        [Route("/get_banners")]
        [HttpGet]
        public IActionResult Bannerlist()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                List<BannersViewModels> model = new List<BannersViewModels>();
                var bannerdata = _bannerservice.Getlist();
                if (bannerdata != null)
                {
                    foreach (var item in bannerdata)
                    {
                        BannersViewModels model1 = new BannersViewModels();
                        model1.Id = item.Id;
                        model1.Title = item.Title;
                        model1.type = item.Type;
                       string uploadsFolder = SiteKey.ImagePath+"/Uploads/"+item.Image;
                        model1.Image = uploadsFolder;// + "/Uploads/" + item.Image;
                        model.Add(model1);
                    }
                    return Ok(new {error=false,Data = model, message = "Banners fetch successfully!", state = "banners", code = 200, status = true });
                }
                var errorData = new { error = true, message = "This banner not avilable.!", data = "null", code = 400, status = false };
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
