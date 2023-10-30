using EC.Service.Product;
using EC.Service.Vendor;
using EC.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EC.API.ViewModels.SiteKey;
using EC.Core.LIBS;
using EC.Data.Models;
using static EC.API.Controllers.BaseAPIController;
using System.IO;
using System.Threading.Tasks;
using System;
using EC.Core.Enums;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ToDo.WebApi.Models;
using EC.API.ViewModels.MultiVendor;
using System.Text;
using EC.API.ViewModels;
using NPOI.Util;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserVendorController : BaseAPIController
    {
        #region [Constructor]
        private readonly IVendorService _vendorService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;
        private readonly IUserRoleService _userRoleService;
        private readonly IEmailsTemplateService _emailSenderService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandsService _brandsService;
        private readonly IProductAttributeImageService _productAttributeImageService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        public UserVendorController(IVendorService vendorService, IUsersService usersService, IUserRoleService userRoleService, IEmailsTemplateService emailSenderService, IConfiguration configuration, ITemplateEmailService templateEmailService, IWebHostEnvironment hostEnvironment, IProductService productService, ICategoryService categoryService, IBrandsService brandsService, IProductAttributeImageService productAttributeImageService, IProductAttributeDetailsService productAttributeDetailsService)
        {
            _vendorService = vendorService;
            _usersService = usersService;
            _userRoleService = userRoleService;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
            _templateEmailService = templateEmailService;
            webHostEnvironment = hostEnvironment; ;
            _productService = productService;
            _categoryService = categoryService;
            _brandsService = brandsService;
            _productAttributeImageService = productAttributeImageService;
            _productAttributeDetailsService = productAttributeDetailsService;
        }
        #endregion

        #region Vendor Register
        [Route("/vendor/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UserVendorViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                bool isExists = _usersService.IsEmailExists(model.email);

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
                bool isVatNoValid = _vendorService.IsVatNoExists(model.vat_no);
                if (isVatNoValid == true)
                {
                    var errorData = new { error = true, message = "The vat no has already been taken.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }

                //bool isbusinessValid = _vendorService.IsbusinessNameExists(model.business_name);
                //if (isbusinessValid == true)
                //{
                //    var errorData = new { error = true, message = "The business name has already been taken.", data = "null", code = 400, status = false };
                //    return new UnauthorizedResponse(errorData);
                //}

                Users entityusers = new Users();
                entityusers.Firstname = model.firstname;
                entityusers.Lastname = model.lastname;
                entityusers.Email = model.email;

                entityusers.SaltKey = PasswordEncryption.CreateSaltKey();
                entityusers.Password = PasswordEncryption.GenerateEncryptedPassword(model.password, entityusers.SaltKey);
                entityusers.Mobile = model.mobile;
                //entityusers.ProfilePic = uniqueFileName==null?"Image" : uniqueFileName;
                entityusers.IsActive = true;
                entityusers.IsVerified = false;
                entityusers.Verifylink = randomString;
                entityusers.CreatedAt = DateTime.Now;
                entityusers.UpdatedAt = null;
                var entity = _usersService.SaveVendorUser(entityusers);

                VendorDetails entityvendor = new VendorDetails();
                entityvendor.UserId = entity.Id;
                entityvendor.VatNo = model.vat_no;
                entityvendor.BusinessName = model.business_name;
                entityvendor.CreatedAt = DateTime.Now;
                entityvendor.UpdatedAt = null;
                entityvendor.Status = true;
                _vendorService.SaveVendor(entityvendor);
                string uniqueFileName = ProcessUploadedFile(model);

                VendorDocuments entityvendorDocuments = new VendorDocuments();
                entityvendorDocuments.UserId = entity.Id;
                entityvendorDocuments.ImageName = uniqueFileName;
                entityvendorDocuments.CreatedAt = DateTime.Now;
                entityvendorDocuments.UpdatedAt = null;
                _vendorService.SaveVendorDocuments(entityvendorDocuments);

                if (entity != null)
                {
                    if (await sendEmailVerificationEmail(entity.Id, model.email, entity.Verifylink))
                    {
                        var userRole = new RoleUser();
                        userRole.UserId = entity.Id;
                        _userRoleService.SaveVendorRole(userRole);
                    }
                }
                return Ok(new { error = false, data = string.Empty, message = "You have registered successfully, Please verify your email address.", state = "register", code = 200, status = true });
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }

        private string ProcessUploadedFile(UserVendorViewModels model)
        {
            string uniqueFileName = null;

            if (model.images != null)
            {
                string uploadsFolder = SiteKey.UploadImage;
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.images.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.images.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        #endregion

        #region Send Email
        [HttpPost]
        public async Task<bool> sendEmailVerificationEmail(int id, string email, string verifylink)
        {
            bool isSendEmail = false;
            for (int i = 0; i < 2; i++)
            {
                var a = id;
                Data.Models.Emails emailTemplate = new Data.Models.Emails();
                var callbackUrl = "/email/" + "verify/" + a + "" + "/" + "" + verifylink + "";
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
                d.Add(new KeyValuePair<string, string>("##VerifyLink##", "<a style='display: block; width: 115px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + SiteKey.FrontedB2BDomain + callbackUrl + "' ' target='_blank'>Verify</a>"));
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
        [Route("/vendoremail/verify/{userid}/{code}")]
        [HttpGet]
        public IActionResult VendorEmailVerification(int userid, string code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _usersService.GetById(userid);
                    if (user != null)
                    {
                        DateTime date2 = DateTime.Now;
                        //TimeSpan comparedate = user.CreatedAt ?? DateTime.Now - date2;

                        DateTime bookingcreatedOn = user.CreatedAt ?? DateTime.Now;
                        DateTime excreatedOn = date2;
                        int MinuteDiff = 0;
                        MinuteDiff = Convert.ToInt32(excreatedOn.Subtract(bookingcreatedOn).TotalMinutes);

                        //TimeSpan difference = date2 - bookingcreatedOn;
                        if (MinuteDiff <= 5)
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

        #region Get Vendor Profile Api
        [Authorize]
        [Route("/vendor/profile-detail")]
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
                    Vendor_Details model = new Vendor_Details();
                    var roleId = users1.RoleUser.Select(x => x.RoleId).FirstOrDefault();
                    model.id = users.Id;
                    model.role = roleId;
                    model.firstname = users.Firstname;
                    model.lastname = users.Lastname;
                    model.email = users.Email;
                    model.mobile = users.Mobile;
                    model.state = users.State;
                    model.postal_code = users.PostalCode;
                    model.country_code = users.CountryCode;
                    model.country = users.Country;
                    model.stripe_id = users.StripeId;
                    model.stripe_account = users.StripeId!=null && users.StripeId.Count()>0 ? model.stripe_account=true: model.stripe_account=false;
                    model.created_at = users.CreatedAt;
                    model.updated_at = users.UpdatedAt;
                    if (users.ProfilePic != null)
                    {
                        string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + users.ProfilePic;
                        model.profile_pic = uploadsFolder;
                    }
                    else
                    {
                        model.profile_pic = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                    }
                    var vendordetails = _vendorService.GetVendorById(users.Id);
                    if (vendordetails != null)
                    {
                        model.business_name = vendordetails.BusinessName;
                        model.vat_no = vendordetails.VatNo;
                    }
                    var vendordocuments = _vendorService.GetVendorDocumentsDetail(users.Id);
                    if (vendordocuments != null)
                    {
                        VendoDocuments documentsdata = new VendoDocuments();
                        documentsdata.id = vendordocuments.Id;
                        documentsdata.user_id = vendordocuments.UserId;
                        if (vendordocuments.ImageName != null)
                        {
                            string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + vendordocuments.ImageName;
                            documentsdata.images = uploadsFolder;
                        }
                        else
                        {
                            documentsdata.images = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        }
                        model.business_document.Add(documentsdata);
                    }
                    return Ok(new { error = false, data = model, message = "User detail fetch successfully.", state = "profileDetail", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Invalid Vendor", data = "null", state = "profileDetail", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);

            }
        }
        #endregion

        #region Update Vendor Profile Api
        [Authorize]
        [Route("/vendor/profile-update")]
        [HttpPost]
        public IActionResult UpdateUserProfile([FromForm] UpdateVendor model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                if (id > 0)
                {
                    bool isIdExist = id != 0;
                    var entity = isIdExist ? _usersService.GetById(id) : new Data.Models.Users();

                    entity.Firstname = string.IsNullOrEmpty(model.firstname) ? entity.Firstname : model.firstname;
                    entity.Lastname = string.IsNullOrEmpty(model.lastname) ? entity.Lastname : model.lastname;
                    entity.Email = string.IsNullOrEmpty(model.email) ? entity.Email : model.email;
                    entity.Mobile = string.IsNullOrEmpty(model.mobile) ? entity.Mobile : model.mobile;
                    entity.Country = string.IsNullOrEmpty(model.country) ? entity.Country : model.country;
                    
                    var entity1 = _usersService.UpdateUser(entity);
                    var vendordetails = isIdExist ? _vendorService.GetVendorById(entity1.Id) : new Data.Models.VendorDetails();

                    if (vendordetails != null)
                    {
                        vendordetails.BusinessName = string.IsNullOrEmpty(model.business_name)? vendordetails.BusinessName: model.business_name;
                        vendordetails.VatNo = string.IsNullOrEmpty(model.vat_no) ? vendordetails.VatNo: model.vat_no;

                        _vendorService.UpdateVendor(vendordetails);
                    }
                    var vendordocuments = isIdExist ? _vendorService.GetByUserIdVendorDocuments(entity1.Id) : new Data.Models.VendorDocuments();
                    if (vendordocuments != null)
                    {
                        if (model.images!=null)
                        {
                                string uploadsFolder = SiteKey.UploadImage;
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.images.FileName;
                                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    model.images.CopyTo(fileStream);
                                }
                                //string uploadsFolder = SiteKey.UploadImage + uniqueFileName;
                                vendordocuments.ImageName = uniqueFileName;
                        }
                        else
                        {
                            vendordocuments.ImageName = vendordocuments.ImageName;
                        }
                        _vendorService.UpdateVendorDocuments(vendordocuments);
                    }
                    var users1 = _usersService.GetUserByuserId(id);
                    Vendor_Details modeldetails = new Vendor_Details();
                    if (entity1 != null)
                    {
                        var roleId = users1.RoleUser.Select(x => x.RoleId).FirstOrDefault();
                        modeldetails.id = entity1.Id;
                        modeldetails.role = roleId;
                        modeldetails.firstname = entity1.Firstname;
                        modeldetails.lastname = entity1.Lastname;
                        modeldetails.email = entity1.Email;
                        modeldetails.mobile = entity1.Mobile;
                        modeldetails.state = entity1.State;
                        modeldetails.postal_code = entity1.PostalCode;
                        modeldetails.country_code = entity1.CountryCode;
                        modeldetails.country = entity1.Country;
                        if (entity1.ProfilePic != null)
                        {
                            string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + entity1.ProfilePic;
                            modeldetails.profile_pic = uploadsFolder;
                        }
                        else
                        {
                            modeldetails.profile_pic = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        }
                        var vendordetail = _vendorService.GetVendorById(entity1.Id);
                        if (vendordetail != null)
                        {
                            modeldetails.business_name = vendordetail.BusinessName;
                            modeldetails.vat_no = vendordetail.VatNo;
                        }
                        var vendordocument = _vendorService.GetVendorDocumentsDetail(entity1.Id);
                        if (vendordocument != null)
                        {
                            VendoDocuments documentsdata = new VendoDocuments();
                            documentsdata.id = vendordocument.Id;
                            documentsdata.user_id = vendordocument.UserId;
                            if (vendordocument.ImageName != null)
                            {
                                string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + vendordocument.ImageName;
                                documentsdata.images = uploadsFolder;
                            }
                            else
                            {
                                documentsdata.images = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                            }
                            modeldetails = modeldetails;
                            modeldetails.business_document.Add(documentsdata);

                        }
                    }
                    return Ok(new
                    {
                        error = false,
                        data = modeldetails,
                        message = "Your profile has been updated successfully.",
                        state = "user update",
                        code = 200,
                        status = true
                    });
                }
                else
                {
                    var errorData = new { error = true, message = "User Invaild", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }






        //private string ProcessUploadedFile(UpdateVendor model)
        //{
        //    string uniqueFileName = null;

        //    if (model.images.Count>0)
        //    {
        //        string uploadsFolder = SiteKey.UploadImage;
        //        for (int i = 0; i < model.images.Count; i++)
        //        {
        //            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.images[i].FileName;
        //            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                model.images.CopyTo(fileStream);
        //            }
        //        }
        //    }
        //    return uniqueFileName;
        //}

        #endregion

    }
}
