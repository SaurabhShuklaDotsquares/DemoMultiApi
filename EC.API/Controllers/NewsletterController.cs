using EC.Service.Taxs;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EC.Service.Newsletters;
using EC.API.ViewModels;
using EC.Core.LIBS;
using static EC.API.Controllers.BaseAPIController;
using System.Threading.Tasks;
using System;
using EC.Data.Models;
using EC.API.ViewModels.SiteKey;
using EC.Core.Enums;
using System.Collections.Generic;

namespace EC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : ControllerBase
    {
        #region Constructor
        private readonly INewslettersService _newslettersService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
        public NewsletterController(INewslettersService newslettersService, ITemplateEmailService templateEmailService, IEmailsTemplateService emailSenderService)
        {
            _newslettersService = newslettersService;
            _templateEmailService = templateEmailService;
            _emailSenderService = emailSenderService;
        }
        #endregion

        #region Create News Letter
        [Route("/newsletter/add")]
        [HttpPost]
        public IActionResult Savedata(NewsletterViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                bool isExists = _newslettersService.IsEmailExists(model.email);

                if (isExists == true)
                {
                    var errorData = new { error = true, message = "The email has already been taken.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                NewsLetters entityusers = new NewsLetters();
                entityusers.Email = model.email;
                entityusers.Status = true;
                entityusers.CreatedAt = DateTime.Now;
                var entity = _newslettersService.SaveNewsLetters(entityusers);
                if (entity != null)
                {
                    sendEmailVerificationEmail(entity.Email);
                }
                return Ok(new { error = false, data = string.Empty, message = "Thank you for the subscription.", code = 200, status = true });
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Send Email
        [HttpPost]
        public async Task<bool> sendEmailVerificationEmail(string email)
        {
            bool isSendEmail = false;
            if (!string.IsNullOrEmpty(email))
            {
                Data.Models.Emails emailTemplate = new Data.Models.Emails();
                emailTemplate = _templateEmailService.GetById((int)EmailType.SubscriptionMail);
                if (emailTemplate != null)
                {
                    string urlToClick = string.Empty;
                    var url = string.Empty;
                    var clickme = string.Empty;
                    url = emailTemplate.Description;

                    IDictionary<string, string> d = new Dictionary<string, string>();
                    d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKey.ImagePath + "/Uploads/" + "/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100' style='height:100px; width:100px;'>"));
                    d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));

                    clickme = url;
                    foreach (KeyValuePair<string, string> ele in d)
                    {
                        clickme = clickme.Replace(ele.Key, ele.Value);
                    }
                    urlToClick = clickme;
                    string subject = emailTemplate.Subject.ToTitleCase();
                    await _emailSenderService.SendEmailAsync(email, subject, urlToClick);
                    isSendEmail = true;
                }
            }
            return isSendEmail;
        }
        #endregion

    }
}
