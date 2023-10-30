﻿using EC.API.Configs;
using EC.API.ViewModels.SiteKey;
using EC.API.ViewModels;
using EC.Core.Enums;
using EC.Core.LIBS;
using EC.Data.Models;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using EC.Data.Models;
using ToDo.WebApi.Models;
using System.Threading.Tasks;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : BaseAPIController
    {
        #region Constructor
        private readonly ILogger<AccountController> _logger;
        private IUsersService _usersService;
        private readonly IUserRoleService _userRoleService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly IConfiguration _configuration;
        private readonly JwtConfig _jwtConfigs;

        public AccountController(IUsersService usersService, IUserRoleService userRoleService, ITemplateEmailService templateEmailService, IEmailsTemplateService emailSenderService, IConfiguration configuration, IOptions<JwtConfig> jwtOptionConfig)
        {
            _usersService = usersService;
            _userRoleService = userRoleService;
            _templateEmailService = templateEmailService;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
            _jwtConfigs = jwtOptionConfig.Value;
        }
        #endregion

        #region Bearer Token genrate
        private string GenerateToken(Users user)
        {
            var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Firstname + user.Lastname),
                 new Claim(ClaimTypes.Sid, user.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigs.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtConfigs.Issuer,
                audience: _jwtConfigs.Audience,
                expires: DateTime.Now.AddMinutes(10),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region Login

        [Route("/users/login")]
        [HttpPost]
        public IActionResult Login(UserLoginDetails model)
        {
            try
            {
                string displayMsg = string.Empty;
                //bool error;
                var user = _usersService.GetAdminUserByEmail(model.Email);
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (user != null)
                {
                    if (user.IsVerified ==false)
                    {
                        var errorData = new { error = true, message = "Please verify your email address to login.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                    if (user.IsActive)
                    {
                        var userEntity = PasswordEncryption.IsPasswordMatch(user.Password, model.Password, user.SaltKey);
                        if (userEntity == true)
                        {
                            var roleId = user.RoleUser.Select(x => x.RoleId).FirstOrDefault();
                            var roleType = ((RoleType)roleId).GetEnumDescription();
                            LogInViewModel userDto = new LogInViewModel();
                            //userDto.api_token = GenrateToken(user);
                            userDto.api_token = GenerateToken(user);
                            userDto.email = user.Email;
                            userDto.firstname = user.Firstname;
                            userDto.lastname = user.Lastname;
                            userDto.mobile = user.Mobile;
                            userDto.user_id = user.Id;
                            userDto.role = roleId;
                            userDto.auth_key = null;
                            if (user.ProfilePic != null)
                            {
                                string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + user.ProfilePic;
                                userDto.profile_pic = uploadsFolder;
                            }
                            else
                            {
                                userDto.profile_pic = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            }
                            //return Ok(userDto);
                            return Ok(new { error = false, data = userDto, message = "You have logged in successfully.", code = 200, status = true });
                        }
                        else
                        {
                            var errorData = new { error = true, message = "Password is incorrect !", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                    }
                    else
                    {
                        var errorData = new { error = true, message = "This user not active.", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                }
                else
                {
                    var errorData = new { error = true, message = "Please enter a valid e-mail address.", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception ex)
            {
                string exceptionJson = JsonConvert.SerializeObject(ex, Formatting.Indented);

                // Print the JSON string to the console or log it
                Console.WriteLine(exceptionJson);

                var errorData = new { error = true, message = ex.Message, code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }

        #endregion

        #region Forgot Password
        [Route("/users/forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(Foragte model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                else
                {
                    var users = _usersService.GetAdminUserByEmail(model.email);
                    if (users != null)
                    {
                        users.ForgotPasswordLink = Guid.NewGuid().ToString();
                        users.ForgotPasswordLinkExpired = DateTime.Now.AddDays(30);
                        users.ForgotPasswordLinkUsed = false;
                        await _usersService.UpdateUserAsync(users);
                        if (await sendForgotPasswordEmail(users.Id, model.email))
                        {
                            return Ok(new { error = false, data = string.Empty, message = "We have emailed you password reset link!", state = "forgotPassword", code = 200, status = true });
                        }
                    }
                    else
                    {
                        var errorData1 = new { error = true, message = "Email Id doesn't exists.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData1);
                    }
                    var errorData = new { error = true, message = "Didn't find any email address to send the reset password link.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        public static string EncodeServerName(string serverName)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(serverName));
        }
        [HttpPost]
        public async Task<bool> sendForgotPasswordEmail(int Id, string email)
        {
            bool isSendEmail = false;
            var ff = System.Web.HttpUtility.UrlEncodeToBytes(Id.ToString());
            var a = EncodeServerName(Id.ToString());

            //var callbackUrl = Url.Action("ResetPassword", "Account", new { Id = a,email= email});
            //callbackUrl = callbackUrl.Replace("ResetPassword", "reset-password");
            var callbackUrl = "/reset-password/" + a + "?email=" + email + "";
            var emailTemplate = _templateEmailService.GetById((int)EmailType.ForgotPassword);

            string Subject1 = "";
            var url = "";
            var clickme = "";
            url = emailTemplate.Description;
            Subject1 = emailTemplate.Subject.ToTitleCase();
            var user1 = _usersService.GetById(Id);
            var Name = user1.Firstname + ' ' + user1.Lastname;
            var Address = user1.UserAddress.ToString();
            var DATE = DateTime.Now.ToString();

            IDictionary<string, string> d = new Dictionary<string, string>();
            d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='" + SiteKey.ImagePath + "/uploads/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100' style='height:100px; width:100px;'>"));
            d.Add(new KeyValuePair<string, string>("##UserName##", Name));
            d.Add(new KeyValuePair<string, string>("##SupportMail##", "<a href='" + SiteKey.SupportEmail + "' ' target='_blank'>" + SiteKey.SupportEmail + "</a>"));
            d.Add(new KeyValuePair<string, string>("##ResetPasswordLink##", "<a style='display: block; width: 200px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + SiteKey.FrontedB2BDomain + callbackUrl + "' ' target='_blank'>Reset Password Link</a>"));

            clickme = url;
            foreach (KeyValuePair<string, string> ele in d)
            {
                clickme = clickme.Replace(ele.Key, ele.Value);
            }
            var urlToClick = clickme;
            var subject = Subject1;
            await _emailSenderService.SendEmailAsync(email, subject, urlToClick);
            isSendEmail = true;
            return isSendEmail;
        }
        public static string DecodeServerName(string encodedServername)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedServername));
        }
        #endregion

        #region Reset Password

        [HttpGet("{id}")]
        public IActionResult ResetPassword(string id)
        {
            string Message = string.Empty;
            ResetPasswordModel model = new ResetPasswordModel();
            var ID = Convert.ToInt32(DecodeServerName(id));
            var user = _usersService.GetById(Convert.ToInt32(ID));
            if (user != null)
            {
                model.id = user.Id;
                model.firstname = user.Firstname;
                model.lastname = user.Lastname;
                model.email = user.Email;
                model.mobile = user.Mobile;
                Message = "User fetch successfully.";
            }
            else
            {
                Message = "User Not Found.";
            }
            return Ok(new { error = false, data = model, Message = Message, code = 200, status = true });
        }

        [Route("/users/reset-password")]
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _usersService.GetAdminUserByEmail(model.Email);
                    if (user != null)
                    {
                        user.SaltKey = PasswordEncryption.CreateSaltKey();
                        user.Password = PasswordEncryption.GenerateEncryptedPassword(model.Password, user.SaltKey);
                        _usersService.UpdateUser(user);
                        return Ok(new { error = false, data = string.Empty, Message = "Password has been changed successfully.", state = "resetPassword", code = 200, status = true });
                    }
                    else
                    {
                        return Ok(new { error = true, Message = "Incorrect Old Password.", code = 400, status = false });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { error = true, message = "Internal Server Error.", code = 500, status = false });
            }
            return Ok(new { error = true, Message = "Password has been not changed.", code = 400, status = false });
        }

        #endregion

        #region Change Password Api

        [Route("/users/change-password")]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            string Message = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var authuser = new AuthUser(User);
                    var userId = authuser.Id;
                    var user = _usersService.GetById(userId);
                    if (user != null)
                    {
                        var isOldPasswordExits = PasswordEncryption.IsPasswordMatch(user.Password, model.old_password, user.SaltKey);
                        if (!isOldPasswordExits)
                        {
                            return Ok(new { error = true, message = "Old password incorrect.", authenticate = true, code = 400, status = true });
                        }
                        var isNewPasswordExits = PasswordEncryption.IsPasswordMatch(user.Password, model.new_password, user.SaltKey);
                        if (isNewPasswordExits)
                        {
                            return Ok(new { error = false, message = "It Seems You Have Entered Same Password As Old Password.", authenticate = true, code = 400, status = true });
                        }
                        user.SaltKey = PasswordEncryption.CreateSaltKey();
                        user.Password = PasswordEncryption.GenerateEncryptedPassword(model.new_password, user.SaltKey);
                        _usersService.UpdateUser(user);
                        Message = "Password changed successfully!";
                    }
                    else
                    {
                        Message = "User Not Found.";
                    }
                }
                else
                {
                    Message = "Incorrect Old Password.";
                }
                return Ok(new { error = false, message = Message, state = "changepassword", authenticate = true, code = 200, status = true });
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
