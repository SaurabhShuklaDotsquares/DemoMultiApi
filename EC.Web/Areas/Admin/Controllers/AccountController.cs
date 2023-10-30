using Auth0.ManagementApi.Models;
using EC.Core.Enums;
using EC.Core.LIBS;
using EC.Data.Entities;
using EC.Service;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;
        private IUsersService _usersService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly ITemplateEmailService _emailTemplateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
        private IDataProtector _protector;
        private readonly ISettingService _settingService;

        public AccountController(IConfiguration configuration, IUsersService usersService,IDataProtectionProvider provider, ITemplateEmailService emailTemplateEmailService, IEmailsTemplateService emailSenderService, ITemplateEmailService templateEmailService, ISettingService settingService)
        {
            _configuration = configuration;
            _usersService = usersService;
            _emailTemplateEmailService = emailTemplateEmailService;
            var protectorPurpose = "secure username and password";
            _protector = provider.CreateProtector(protectorPurpose);
            _emailSenderService = emailSenderService;
            _templateEmailService = templateEmailService;
            _settingService = settingService;
        }
        public async Task<IActionResult> Index()
        {
            await SignOut();
            var model = new LogInViewModel();
            var userKey = Request.Cookies["UserKey"];
            var userPwd = Request.Cookies["UserValue"];
            if (!string.IsNullOrEmpty(userKey) && !string.IsNullOrEmpty(userPwd))
            {
                string unProtName = string.Empty;
                string unProtPwd = string.Empty;
                try
                {
                    unProtName = _protector.Unprotect(userKey);
                    unProtPwd = _protector.Unprotect(userPwd);
                    if (!string.IsNullOrEmpty(unProtName) && !string.IsNullOrEmpty(unProtPwd))
                    {
                        model.Email = unProtName;
                        model.Password = unProtPwd;
                        if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Email))
                        {
                            model.RememberMe = true;
                            var isRedirect = await loginAsync(model);
                            if (string.IsNullOrEmpty(isRedirect))
                            {
                                return RedirectToAction("index", "dashboard");
                            }
                            else
                            {
                                return View(model);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Response.Cookies.Delete("UserKey");
                    Response.Cookies.Delete("UserValue");
                }
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(LogInViewModel model)
        {
            var validMessage = await loginAsync(model);
            if (validMessage == string.Empty)
                return RedirectToAction("index", "dashboard");

            ShowErrorMessage("Warning!", validMessage);
            return View(model);
        }
        private async Task<string> loginAsync(LogInViewModel model)
        {
            string displayMsg = string.Empty;
            var user = _usersService.GetAdminUserByEmail(model.Email);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        var match = false;
                        match = PasswordEncryption.IsPasswordMatch(user.Password, model.Password, user.SaltKey);
                        if (match)
                        {
                            var entityMAIN_LOGO = _settingService.GetLogoSettingList().Where(x => x.Slug == "MAIN_LOGO").FirstOrDefault().ConfigValue;
                            var entityFAVICON = _settingService.GetLogoSettingList().Where(x => x.Slug == "MAIN_FAVICON").FirstOrDefault().ConfigValue;
                            if (entityMAIN_LOGO != null)
                            {
                                HttpContext.Session.SetString("Main_Logo", entityMAIN_LOGO);
                            }
                            if (entityFAVICON != null)
                            {
                                HttpContext.Session.SetString("Main_FAVICON", entityFAVICON);
                            }
                            var roleId = user.RoleUser.Select(x => x.RoleId).FirstOrDefault();
                            var roleType = ((RoleType)roleId).GetEnumDescription();

                            await CreateAuthenticationTicket(user, roleType, model.RememberMe);

                            var cookieOptions = new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddDays(1),
                                IsEssential = model.RememberMe
                            };
                            if (model.RememberMe)
                            {
                                var protectedName = _protector.Protect(model.Email.Trim());
                                var protectedPassword = _protector.Protect(model.Password);
                                Response.Cookies.Append("UserKey", protectedName, cookieOptions);
                                Response.Cookies.Append("UserValue", protectedPassword, cookieOptions);
                            }
                            else
                            {
                                //Response.Cookies.Delete("UserKey");
                            }
                            displayMsg = string.Empty;
                        }
                        else
                        {
                            displayMsg = "Password is incorrect !";
                        }
                    }
                    else
                    {
                        displayMsg = "Your account is not active. please contact to administrator";
                    }
                }
                else
                {
                    displayMsg = "User name is incorrect !";
                }
            }
            else
            {
                displayMsg = "Invalid login attempt !";
            }
            return displayMsg;
        }
        /// <summary>
        /// Forgets the password.
        /// </summary>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var users = _usersService.GetAdminUserByEmail(model.Email);
                    if (users != null)
                    {
                        users.ForgotPasswordLink = Guid.NewGuid().ToString();
                        users.ForgotPasswordLinkExpired = DateTime.Now.AddDays(30);
                        users.ForgotPasswordLinkUsed = false;
                        await _usersService.UpdateUserAsync(users);
                        if (await sendForgotPasswordEmail(users.Id, model.Email))
                        {
                            ShowSuccessMessage("Success", "Forgot password link has been sent successfully to your email Id.", false);
                        }

                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Email Id doesn't exists.", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error!", "Didn't find any email address to send the reset password link", false);
                }

            }
            catch (Exception ex)
            {
                //await Task.Run(() => TraceError.ReportError(ex, SiteKeys.AdminEmail, "Forgotpass Email Send"));
                ShowErrorMessage("Error!", "Something went wrong with the process", false);
            }

            ModelState.Clear();
            model.Email = string.Empty;
            return View(model);
        }
        public static string EncodeServerName(string serverName)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(serverName));
        }
        public static string DecodeServerName(string encodedServername)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedServername));
        }
        public async Task<bool> sendForgotPasswordEmail(int Id, string email)
        {
            bool isSendEmail = false;
            var ff = System.Web.HttpUtility.UrlEncodeToBytes(Id.ToString());
            var a = EncodeServerName(Id.ToString());
            
           
            var callbackUrl = Url.Action("ResetPasswords", "UserManager", new { Id = a });
            //var data = _templateEmailService.GetEmailsList().Where(x => x.Slug == "forgot-password" && x.Islocked == true).FirstOrDefault();
            var data = _templateEmailService.GetById((int)EmailType.ForgotPassword);

            string Subject1 = "";
            var url = "";
            var clickme = "";
            url = data.Description;
            Subject1 = data.Subject;
            var user1 = _usersService.GetById(Id);
            var Name = user1.Firstname + ' ' + user1.Lastname;
            var Address = user1.UserAddress.ToString();
            var DATE = DateTime.Now.ToString();

            IDictionary<string, string> d = new Dictionary<string, string>();
            d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKeys.Domain + "/uploads/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100'>"));
            d.Add(new KeyValuePair<string, string>("##UserName##", Name));
            d.Add(new KeyValuePair<string, string>("##SupportMail##", "<a class='btn btn-warning' href='" + SiteKeys.SupportEmail + "' ' target='_blank'>" + SiteKeys.SupportEmail + "</a>"));
            d.Add(new KeyValuePair<string, string>("##ResetPasswordLink##", "<a style='display: block; width: 150px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + SiteKeys.Domain + callbackUrl + "' ' target='_blank'>Reset Password</a>"));
            //d.Add(new KeyValuePair<string, string>("%USER_NAME%", Name));
            //d.Add(new KeyValuePair<string, string>("%ADDRESS%", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
            //d.Add(new KeyValuePair<string, string>("%CURRENT_DATE%", DATE));
            //d.Add(new KeyValuePair<string, string>("%APP_NAME%", "E-Comerce"));
            //d.Add(new KeyValuePair<string, string>("%SUPPORT_MAIL%", "support@ecommerceportal.24livehost.com"));

            //var ff1 = System.Web.HttpUtility.UrlEncode(callbackUrl) ;
            clickme = url;
            foreach (KeyValuePair<string, string> ele in d)
            {
                clickme = clickme.Replace(ele.Key, ele.Value);
                //Console.WriteLine("Key = {0}, Value = {1}", ele.Key, ele.Value);
            }
            //var urlToClick = "<a href='" + SiteKeys.Domain + callbackUrl + "' +'" + clickme + "'>Click here to Reset your Password</a>";
            var urlToClick = clickme;
            var subject = Subject1.ToTitleCase();
            await _emailSenderService.SendEmailAsync(email, subject, urlToClick);
            isSendEmail = true;
            return isSendEmail;
        }

        /// <summary>
        /// Represents an event that is raised when the sign-out operation is complete.
        /// </summary>
        /// <returns></returns>
        /// 

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> SignOut()
        {
            //var dd = Request.Cookies["UserValue"];
            //var dd1 = Request.Cookies["UserKey"];
            await signOutFunctionality();
            
            Response.Cookies.Delete("UserKey");
            Response.Cookies.Delete("UserValue");
           //var dd= HttpContext.Request.Cookies["UserValue"];
            //YourCookies.Expires = DateTime.Now.AddDays(-1d);
            ////Session.Abandon();
            //Response.Cookies.c
            return RedirectToAction("index");
        }
        /// <summary>
        /// Remove user logged Info's from Cookies
        /// </summary>
        public async Task signOutFunctionality()
        {
            await RemoveAuthentication();
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddSeconds(1) };
            Response.Cookies.Append("UserSessionCookies", string.Empty, cookieOptions);
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        }
    }
}
