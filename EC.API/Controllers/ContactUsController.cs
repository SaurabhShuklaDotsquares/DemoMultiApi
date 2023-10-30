using EC.API.ViewModels;
using EC.Data.Models;
using EC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using ToDo.WebApi.Models;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactUsController : BaseAPIController
    {
        #region Constructor
        private IContactUsService _contactservice;
        public ContactUsController(IContactUsService contactservice)
        {
            _contactservice = contactservice;
        }
        #endregion

        #region Contact Us Api

        [Route("/contact")]
        [HttpPost]
        public IActionResult Contactus(ContactusViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                ContuctUs entity = new ContuctUs();
                entity.Firstname = model.firstname;
                entity.Lastname = model.lastname;
                entity.Phone = model.phone.ToString();
                entity.Email = model.email;
                entity.Message = model.message;
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;
                entity = _contactservice.Save(entity);
                return Ok(new { error = false, data = string.Empty, Message = "Enquiry has been sent successfully.", code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = ex.Message, code = 400, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Contact Us Enquiry Api
        [Route("/contact-us-enquiry")]
        [HttpPost]
        public IActionResult ContactUsEnquiry(ContactusEnquiryViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                ContuctUs entity = new ContuctUs();
                entity.Firstname = model.firstname;
                entity.Lastname = model.lastname;
                entity.Phone = model.phone.ToString();
                entity.Email = model.email;
                entity.Message = model.message;
                entity.ProductId = Convert.ToInt32(model.product_id);
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;
                entity = _contactservice.Save(entity);
                return Ok(new { error = false, data = string.Empty, Message = "Enquiry has been sent successfully.", code = 200, status = true });
            }
            catch (Exception ex) 
            {
                var errorData = new { error = true, message = ex.Message, code = 400, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
