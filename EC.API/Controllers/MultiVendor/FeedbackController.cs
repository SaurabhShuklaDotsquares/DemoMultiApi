using EC.API.ViewModels.SiteKey;
using EC.Core.Enums;
using EC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ToDo.WebApi.Models;
using EC.API.ViewModels.MultiVendor;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {

        #region Constructor
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly IUsersService _usersService;
        private readonly IContactUsService _contactUsService;

        public FeedbackController(ITemplateEmailService templateEmailService, IEmailsTemplateService emailSenderService, IUsersService usersService, IContactUsService contactUsService)
        {
            _templateEmailService = templateEmailService;
            _emailSenderService = emailSenderService;
            _usersService = usersService;
            _contactUsService = contactUsService;
        }
        #endregion

        #region Send Feed Back

        [Authorize]
        [Route("/send-feedback")]
        [HttpPost]
        public async Task<IActionResult> feedback(FeedbackViewModels feedback)
        {
            try
            {
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var data = _contactUsService.GetByVendoremail(feedback.email);
                if (data!=null)
                {
                    if (await sendEmailVerificationEmail(id, feedback.email, feedback.subject, feedback.message))
                    {
                      return Ok(new { error = false, data = "null", message = "Enquiry feedback has been sent successfully.", code = 200, status = true });
                    }
                }

                return Ok(new { error = true, data = "null", message = "Data not found", code = 401, status = false });

            }
            catch (System.Exception)
            {

                throw;
            }
        }
       

        #region Send Email
        [HttpPost]
        public async Task<bool> sendEmailVerificationEmail(int id, string email, string subject, string message)
        {
            bool isSendEmail = false;
            for (int i = 0; i < 2; i++)
            {
                Data.Models.Emails emailTemplate = new Data.Models.Emails();
               // var callbackUrl = Url.Action("EmailVerification", "UserVendor", new { Id = id });
               
                emailTemplate = _templateEmailService.GetById((int)EmailType.feedback);

                string urlToClick = "";
                string Subject1 = "";
                var url = "";
                var clickme = "";
                url = emailTemplate.Description;
                Subject1 = emailTemplate.Subject;
                var user1 = _usersService.GetById(id);
                var UserName = user1.Firstname + ' ' + user1.Lastname;
                var Address = user1.UserAddress.ToString();
                var DATE = DateTime.Now.ToString();

                IDictionary<string, string> d = new Dictionary<string, string>();
                d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKey.ImagePath + "/Uploads/" + "/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100' style='height:100px; width:100px;'>"));
                d.Add(new KeyValuePair<string, string>("##UserName##", UserName));
                d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                d.Add(new KeyValuePair<string, string>("##Subject##", subject));
                //d.Add(new KeyValuePair<string, string>("##AppName##", "E-Commerce"));
                d.Add(new KeyValuePair<string, string>("##SupportMail##", "<a href='" + SiteKey.SupportEmail + "' ' target='_blank'>" + SiteKey.SupportEmail + "</a>"));
                d.Add(new KeyValuePair<string, string>("##message##", message));

                clickme = url;
                foreach (KeyValuePair<string, string> ele in d)
                {
                    clickme = clickme.Replace(ele.Key, ele.Value);
                }
                urlToClick = clickme;
                var subject2 = Subject1;
                await _emailSenderService.SendEmailAsync(email, subject2, urlToClick);
                isSendEmail = true;
            }
            return isSendEmail;

        }
        #endregion

        #endregion
    }
}
