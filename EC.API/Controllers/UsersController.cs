using EC.Core.LIBS;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using EC.API.ViewModels;
using EC.API.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using EC.API.ViewModels.SiteKey;
using Microsoft.AspNetCore.Authorization;
using ToDo.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Hosting;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Hosting.Internal;
using NPOI.HPSF;
using static EC.API.Controllers.BaseAPIController;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using EC.Core.Enums;
using System.Text;
using Polly;
using RestSharp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : BaseAPIController
    {
        #region Constructor
        private IUsersService _usersService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IUserRoleService _userRoleService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
       

        public UsersController(IUsersService usersService, IUserRoleService userRoleService, ITemplateEmailService templateEmailService, IEmailsTemplateService emailSenderService, IWebHostEnvironment hostEnvironment)
        {
            _usersService = usersService;
            _userRoleService = userRoleService;
            _templateEmailService = templateEmailService;
            _emailSenderService = emailSenderService;
            webHostEnvironment = hostEnvironment;
        }
        #endregion

        #region Users Register Api
        [Route("/users/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm]UserManagersViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                bool isExists = _usersService.IsEmailExists(model.Email);
                int length = 10; // Desired length of the random string
                string randomString = GenerateRandomString(length);
                static string GenerateRandomString(int length)
                {
                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    Random random = new Random();
                    StringBuilder sb = new StringBuilder(length);

                    for (int i = 0; i < length; i++)
                    {
                        int index = random.Next(chars.Length);
                        sb.Append(chars[index]);
                    }

                    return sb.ToString();
                }
                if (isExists == true)
                {
                    var errorData = new { error = true, message = "This email is already exists, please try another one.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                Data.Models.Users entityusers = new Data.Models.Users();
                //string uniqueFileName = ProcessUploadedFile(model);
                entityusers.Firstname = model.Firstname;
                entityusers.Lastname = model.Lastname;
                entityusers.Email = model.Email;
                entityusers.SaltKey = PasswordEncryption.CreateSaltKey();
                entityusers.Password = PasswordEncryption.GenerateEncryptedPassword(model.Password, entityusers.SaltKey);
                entityusers.Mobile = model.Mobile;
                //entityusers.ProfilePic = uniqueFileName==null?"Image" : uniqueFileName;
                entityusers.IsActive = true;
                entityusers.IsVerified = false;
                entityusers.Verifylink = randomString;

                var entity = _usersService.SaveUser(entityusers);
                if (entity != null)
                {
                    if (await sendEmailVerificationEmail(entity.Id, model.Email, entity.Verifylink))
                    {
                        var userRole = new Data.Models.RoleUser();
                        userRole.UserId = entity.Id;
                        _userRoleService.SaveUserRole(userRole);
                    }
                }

                return Ok(new { error = false, data = string.Empty, message = "You have registered successfully, Please verify your email address.", state = "register", code = 200, status = true });
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Somthing went worng inside Register action:{ex.GetBaseException().Message}");
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Send Email
        [HttpPost]
        public async Task<bool> sendEmailVerificationEmail(int id, string email,string verifylink)
        {
            bool isSendEmail = false;
            
            //HttpContext.Session["session"]= randomString;
            //HttpContext.Session.SetString("OTP", randomString);
            for (int i = 0; i < 2; i++)
            {
                var a = id;
                Data.Models.Emails emailTemplate = new Data.Models.Emails();
                var callbackUrl = "/email/"+"verify/"+a+""+"/"+""+ verifylink + "";
                //var callbackUrl = Url.Action("EmailVerification", "Users", new { Id = id });
                if (i == 0)
                {
                    emailTemplate = _templateEmailService.GetById((int)EmailType.Registration);
                }
                else
                {
                    emailTemplate = _templateEmailService.GetById((int)EmailType.EmailVerification);
                }
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
                d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKey.ImagePath + "/uploads/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100' style='height:100px; width:100px;'>"));
                d.Add(new KeyValuePair<string, string>("##UserName##", UserName));
                d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                d.Add(new KeyValuePair<string, string>("##CurrentDate##", DATE));
                d.Add(new KeyValuePair<string, string>("##AppName##", "E-Commerce"));
                d.Add(new KeyValuePair<string, string>("##SupportMail##", "<a href='" + SiteKey.SupportEmail + "' ' target='_blank'>" + SiteKey.SupportEmail + "</a>"));
                d.Add(new KeyValuePair<string, string>("##VerifyLink##", "<a style='display: block; width: 115px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='"+ SiteKey.FrontedB2BDomain + callbackUrl + "' ' target='_blank'>Verify</a>"));
                d.Add(new KeyValuePair<string, string>("##GoToMyAccount##", "<a style='display: block; width: 200px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + SiteKey.FrontedB2BLogInDomain + "' ' target='_blank'>Go To My Account</a>"));


                clickme = url;
                foreach (KeyValuePair<string, string> ele in d)
                {
                    clickme = clickme.Replace(ele.Key, ele.Value);
                }
                urlToClick = clickme;
                var subject = Subject1;
                await _emailSenderService.SendEmailAsync(email, subject, urlToClick);
                isSendEmail = true;
            }
            return isSendEmail;

        }
        #endregion

        #region EmailVerification
        [Route("/email/verify/{Id}/{code}")]
        [HttpGet]
        public IActionResult EmailVerification(int Id, string code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _usersService.GetById(Id);
                    if (user != null)
                    {
                        DateTime date2 = DateTime.Now;
                        //TimeSpan comparedate = user.CreatedAt ?? DateTime.Now - date2;

                        DateTime bookingcreatedOn = user.CreatedAt ?? DateTime.Now;
                        DateTime excreatedOn = date2;
                        int MinuteDiff = 0;
                        MinuteDiff = Convert.ToInt32(excreatedOn.Subtract(bookingcreatedOn).TotalMinutes);

                        //TimeSpan difference = date2 - bookingcreatedOn;
                        if (MinuteDiff <= 20)
                        {
                            bool match2 = (user.Verifylink == code);
                            if (match2)
                            {
                                if (user.EmailVerifiedAt == null)
                                {
                                    user.EmailVerifiedAt = DateTime.Now;
                                    user.IsVerified = true;
                                    _usersService.UpdateUser(user);
                                    return Ok(new { error = false, Message = "Your Email Account is successfully verified.", code = 200, status = true });
                                }
                                else
                                {
                                    var errorData = new { error = true, message = "Your are already verified.", data = "null", code = 400, status = false };
                                    return new UnauthorizedResponse(errorData);
                                }
                            }
                            var errormsg = new { error = true, message = "Not matched OTP. please try again.", data = "null", code = 401, status = false };
                            return new UnauthorizedResponse(errormsg);
                        }
                        var errormesege = new { error = true, message = "This link expire. please try again. ", data = "null", code = 401, status = false };
                        return new UnauthorizedResponse(errormesege);
                    }
                    var errormessege = new { error = true, message = "User Not found.", data = "null", code = 401, status = false };
                    return new UnauthorizedResponse(errormessege);
                }
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
            return Ok(201);
        }
        #endregion

        #region Update User Profile Api
        [Authorize]
        [Route("/users/profile-update")]
        [HttpPost]
        public IActionResult UpdateUserProfile([FromForm]UserViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                string uniqueFileName = ProcessUploadedFile(model);

                if (id > 0)
                {
                    bool isIdExist = id != 0;

                    var entity = isIdExist ? _usersService.GetById(id) : new Data.Models.Users();
                    if (model.firstname != null && model.firstname != "")
                    {
                        entity.Firstname = model.firstname;
                    }
                    else
                    {
                        entity.Firstname = entity.Firstname;
                    }
                    if (model.lastname != null && model.lastname != "")
                    {
                        entity.Lastname = model.lastname;
                    }
                    else
                    {
                        entity.Lastname = entity.Lastname;
                    }
                    if (model.email != null && model.email!="")
                    {
                        entity.Email = model.email;
                    }
                    else
                    {
                        entity.Email = entity.Email;
                    }
                    if (model.mobile != null && model.mobile != "")
                    {
                        entity.Mobile = model.mobile;
                    }
                    else
                    {
                        entity.Mobile = entity.Mobile;
                    }
                    if (model.profile_pic != null)
                    {
                        entity.ProfilePic = uniqueFileName;
                    }
                    else
                    {
                        entity.ProfilePic = entity.ProfilePic;
                    }
                    var entity1 = _usersService.UpdateUser(entity);
                    UpdateUserViewModels entitydata = new UpdateUserViewModels();
                    if (entity1 != null)
                    {
                        entitydata.firstname = entity1.Firstname;
                        entitydata.lastname = entity1.Lastname;
                        entitydata.email = entity1.Email;
                        entitydata.mobile = entity1.Mobile;
                        entitydata.profile_pic= SiteKey.ImagePath + "/Uploads/" + entity1.ProfilePic;
                    }
                    return Ok(new { error=false,data = entitydata, message = "Your profile has been updated successfully.",
                        state="user update",code = 200, status = true });
                    }
                else
                {
                    var errorData = new { error = true, message = "User Invaild", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Upload File
        private string ProcessUploadedFile(UserViewModels model)
        {
            string uniqueFileName = null;

            if (model.profile_pic != null)
            {
                string uploadsFolder = SiteKey.UploadImage;
                #region  For local path Iamage

                //string uploadsFolder = SiteKey.UploadImage;
                //uploadsFolder = uploadsFolder.Replace("API", "Web") + "\\wwwroot\\Uploads\\";
                #endregion
                #region  server path Iamage
                //string uploadsFolder = "";
                //uploadsFolder = "E:\\testing-admin-ecom-single.24livehost.com\\wwwroot\\Uploads\\";
                #endregion
                //string uploadsFolder = SiteKey.ImagePath + "/Uploads/";

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.profile_pic.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.profile_pic.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        #endregion

        #region Get User Profile Api
        [Authorize]
        [Route("/users/profile-detail")]
        [HttpGet]
        public IActionResult GetUserProfile()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var users = _usersService.GetById(id);
                var users1 = _usersService.GetUserByuserId(id);
                if (users != null)
                {
                    UserViewModel model = new UserViewModel();
                    var roleId = users1.RoleUser.Select(x => x.RoleId).FirstOrDefault();
                    model.id = users.Id;
                    model.role = roleId;
                    model.firstname = users.Firstname;
                    model.lastname = users.Lastname;
                    model.email = users.Email;
                    model.mobile = users.Mobile;
                    //model.profile_pic = users.Mobile;
                    if (users.ProfilePic != null)
                    {
                        string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + users.ProfilePic;
                        model.profile_pic = uploadsFolder;
                    }
                    else
                    {
                        model.profile_pic = SiteKey.ImagePath + "/Uploads/"+SiteKey.DefaultImage;
                    }
                    //model.profile_pic = users.Mobile;
                    return Ok(new { error=false,data = model,message = "User detail fetch successfully.", state="profileDetail" ,code = 200, status = true });
                    //return Ok(model);
                }
                else
                {
                    var errorData = new { error = true, message = "Invalid User", data = "null", state = "profileDetail", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
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
