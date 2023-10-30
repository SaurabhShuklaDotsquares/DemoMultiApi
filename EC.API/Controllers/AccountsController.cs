using EC.API.Configs;
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

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : BaseAPIController
    {
        private IUsersService _usersService;
        private readonly IUserRoleService _userRoleService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly IConfiguration _configuration;
        private readonly JwtConfig _jwtConfigs;



        public AccountsController(IUsersService usersService, IUserRoleService userRoleService, ITemplateEmailService templateEmailService, IEmailsTemplateService emailSenderService, IConfiguration configuration, IOptions<JwtConfig> jwtOptionConfig)
        {
            _usersService = usersService;
            _userRoleService = userRoleService;
            _templateEmailService = templateEmailService;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
            _jwtConfigs = jwtOptionConfig.Value;

        }


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


        [Route("/HelloData")]
        [HttpGet]
        public IActionResult HelloData()
        {
            try
            {
                return Ok($"{"WelCome"}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        #region Login

        [Route("/users1/login")]
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
                    if (user.EmailVerifiedAt == null)
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

    }
}
