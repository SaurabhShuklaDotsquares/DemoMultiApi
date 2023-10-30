using Auth0.ManagementApi.Models;
using EC.Core;
using EC.Core.Enums;
using EC.Core.LIBS;
using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using EC.Web.Areas.Admin.Code;
using EC.Web.Areas.Admin.ViewModels;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.Controllers
{
    //[OfflineActionFilter]
    //[CustomAuthorization(RoleType.Administrator)]
    public class UserManagerController : BaseController
    {
        #region [ Service Injection ]
        /// <summary>
        /// Inject Required Services
        /// </summary>
        /// 
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;
        private readonly IUserRoleService _userRoleService;
        private readonly IEmailsTemplateService _emailSenderService;
        public UserManagerController(IUsersService usersService, IUserRoleService userRoleService, IEmailsTemplateService emailSenderService, IConfiguration configuration, ITemplateEmailService templateEmailService)
        {
            _usersService = usersService;
            _userRoleService = userRoleService;
            _emailSenderService = emailSenderService;
            _configuration = configuration;
            _templateEmailService = templateEmailService;
        }
        #endregion [ Service Injection ]

        #region [ INDEX ]
        /// <summary>
        /// Navigate & Start From This Index View
        /// </summary>
        /// <returns>return to Index View</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get & Set Users record into DataTable With Pagination
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>return Users record DataTable With Pagination</returns>
        [HttpPost]
        public ActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Users>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Firstname.Contains(sSearch) || q.Lastname.Contains(sSearch) || q.Email.Contains(sSearch));
            }
            query.AddFilter(q => q.IsAdmin == 0);
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, string>(q => q.Firstname, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, string>(q => q.Lastname, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, string>(q => q.Mobile, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 8:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, string>(q => q.Email, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Users, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Users> entities = _usersService.Get(query, out total).Entities;

            foreach (Users entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Firstname,
                    entity.Lastname,
                    entity.Mobile,
                    entity.Email,
                    entity.IsVerified ==true ? "<span style='width: 100%; text - align: center' class='badge badge-success'>Verified</span>":"<span style='width: 100%; text - align: center' class='badge badge-danger'>Not Verified</span>",
                    //entity.EmailVerifiedAt != null && entity.IsVerified == true ? "<span style='width: 100%; text - align: center' class='badge badge-success'>Verified</span>":"<span style='width: 100%; text - align: center' class='badge badge-danger'>Not Verified</span>",
                    entity.CreatedAt.ToString(),
                    entity.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion [ INDEX ]

        //public IActionResult CreateEdit2(int? id)
        //{
        //    var model = new UserManagerViewModel();

        //    if (id.HasValue)
        //    {
        //        Users users = _usersService.GetById(id);

        //        model.Id = users.Id;
        //        model.Firstname = users.Firstname;
        //        model.Lastname = users.Lastname;
               
        //    }

        //    return PartialView("_CreateEditDemo", model);
        //}

        #region [ ADD / EDIT ]
        /// <summary>
        /// Get & Set Value into UserManagerViewModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            var model = new UserManagerViewModel();

            if (id.HasValue)
            {
                Users users = _usersService.GetById(id);

                model.Id = users.Id;
                model.Firstname = users.Firstname;
                model.Lastname = users.Lastname;
                model.Mobile = users.Mobile;
                model.Email = users.Email;
                //model.Gender = users.Gender;
                model.Password = users.Password;
                model.ConfirmPassword = users.Password;
            }
            return PartialView("_CreateEdit", model);
        }

        [HttpPost]
        public JsonResult AjaxMethod(string email)
        {
            bool isValid = _usersService.IsEmailExists(email);
            return Json(isValid);
        }

        /// <summary>
        /// Insert or Update UserManagerViewModel Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEdit(int? id, UserManagerViewModel model)
        {
            try
            {
                if (id.HasValue && id > 0)
                {
                    ModelState.Remove("Password");
                }
                if(!ModelState.IsValid)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "test", IsSuccess = false });
                }
                bool isIdExist = id.HasValue && id.Value != 0;

                if (!isIdExist)
                {
                    bool isExists = _usersService.IsEmailExists(model.Email);

                    if (isExists)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = false, Data = "This email is already exists, please try another one."});
                    }
                }
                var entity = isIdExist ? _usersService.GetById(model.Id) : new Users();

                entity.SaltKey = isIdExist ? entity.SaltKey : PasswordEncryption.CreateSaltKey();
                entity.Password = isIdExist ? entity.Password : PasswordEncryption.GenerateEncryptedPassword(model.Password, entity.SaltKey);
                entity.Firstname = model.Firstname;
                entity.Lastname = model.Lastname;
                entity.Mobile = model.Mobile;
                entity.Email = isIdExist ? entity.Email : model.Email;
                entity.IsActive = true;
                //entity.IsVerified = isIdExist ? entity.IsVerified : false;
                entity.IsVerified = true;
                //entity.Gender = model.Gender;

                if (!isIdExist)
                {
                    entity = _usersService.SaveUser(entity);
                    if (entity!=null)
                    {
                        var userRole = new RoleUser();
                        userRole.UserId = entity.Id;
                        _userRoleService.SaveUserRole(userRole);
                        ShowSuccessMessage("Success", "User successfully created.", false);
                    }
                    //if (await SendEmailVerificationEmail(entity.Id, model.Email))
                    //{
                    //    var userRole = new RoleUser();
                    //    userRole.UserId = entity.Id;
                    //    _userRoleService.SaveUserRole(userRole);
                    //    ShowSuccessMessage("Success", "Email verification link has been sent successfully to Users email Id and User successfully created.", false);
                    //}
                }
                else
                {
                    entity = _usersService.UpdateUser(entity);
                    ShowSuccessMessage("Success!", "User successfully updated", false);
                }
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true});

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = ex.Message, IsSuccess = false });
            }
        }

        #endregion [ ADD / EDIT ]

        #region [ SendEmailVerificationEmail ]
        /// <summary>
        /// get user id and email
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns>send a link to user for email verification</returns>
        public async Task<bool> SendEmailVerificationEmail(int id, string email)
        {
            bool isSendEmail = false;
            var user = _usersService.GetById(id);
            if(user != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    Emails emailTemplate = new Emails();
                    string urlToClick = string.Empty;
                    string Subject = string.Empty;
                    var description = string.Empty;
                    var clickme = string.Empty;
                    var callbackUrl = Url.Action("EmailVerification", "UserEmailVerication", new { Id = id });
                    if (i == 0)
                    {
                        emailTemplate = _templateEmailService.GetById((int)EmailType.Registration);
                    }
                    else
                    {
                        emailTemplate = _templateEmailService.GetById((int)EmailType.EmailVerification);
                    }

                    description = emailTemplate.Description;
                    Subject = Extensions.ToTitleCase(emailTemplate.Subject);
                    var UserName = user.Firstname + ' ' + user.Lastname;
                    var Address = user.UserAddress.ToString();
                    var DATE = DateTime.Now.ToString();
                    var verifyUrl = SiteKeys.Domain + callbackUrl;
                    IDictionary<string, string> d = new Dictionary<string, string>();
                    d.Add(new KeyValuePair<string, string>("##CompanyLogo##", "<img src='"+SiteKeys.Domain+"/uploads/dotsquaresemaillogo.webp' alt='Logo' height='100' width='100'>"));
                    d.Add(new KeyValuePair<string, string>("##UserName##", UserName));
                    d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                    d.Add(new KeyValuePair<string, string>("##CurrentDate##", DATE));
                    d.Add(new KeyValuePair<string, string>("##AppName##", "E-Commerce"));
                    d.Add(new KeyValuePair<string, string>("##SupportMail##", "<a href='" + SiteKeys.SupportEmail + "' ' target='_blank'>" + SiteKeys.SupportEmail + "</a>"));
                    d.Add(new KeyValuePair<string, string>("##VerifyLink##", "<a style='display: block; width: 115px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + verifyUrl + "' ' target='_blank'>Verify</a>"));
                    d.Add(new KeyValuePair<string, string>("##GoToMyAccount##", "<a style='display: block; width: 200px; margin: auto; height: 25px; background: #ffc107; padding: 10px; text-align: center; border-radius: 5px; color: black; font-weight: bold; line-height: 25px; text-decoration:none; border-color: #ffc107;' href='" + SiteKeys.Domain + "' ' target='_blank'>Go To My Account</a>"));

                    clickme = description;
                    foreach (KeyValuePair<string, string> ele in d)
                    {
                        clickme = clickme.Replace(ele.Key, ele.Value);
                    }
                    urlToClick = clickme;
                    await _emailSenderService.SendEmailAsync(email, Subject, urlToClick);
                    isSendEmail = true;
                }
            }
            return isSendEmail;
         }
        #endregion [ SendEmailVerificationEmail ]

        #region [ DELETE ]
        /// <summary>
        /// Show Confirmation Box For Delete Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Delete Confirmation Box </returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure you want to delete this user information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete User Information" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        /// <summary>
        /// Delete Record From DB(IsDeleted)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FC"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult Delete(int id, IFormCollection FC)
        {
            try
            {
                bool isDeletedUserRole = _userRoleService.DeleteUserole(id);

                if (isDeletedUserRole == true)
                {
                    bool isDeletedUser = _usersService.DeleteUser(id);
                    if (isDeletedUser)
                    {
                        ShowSuccessMessage("Success!", "User Information deleted successfully.", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Error occurred, Please try again.", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error!", "Error occurred, Please try again.", false);
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ShowErrorMessage("Error!", message, false);
            }
            return RedirectToAction("Index");
        }

        #endregion [ DELETE ]

        #region [ ResetPassword ]
        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            return PartialView("_ResetPassword",new ResetPasswordViewModel { Id = id });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _usersService.GetById(model.Id);
                    if (user != null)
                    {
                        user.SaltKey = PasswordEncryption.CreateSaltKey();
                        user.Password = PasswordEncryption.GenerateEncryptedPassword(model.Password, user.SaltKey);
                        _usersService.UpdateUser(user);
                        ShowSuccessMessage("Success", "Password has been changed successfully", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error", "Incorrect Old Password", false);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
            //return RedirectToAction("index", "usermanager");
        }

        #endregion [ ChangePassword ]

        #region [ ChangeStatus ]

        /// <summary>
        /// Update Users Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveUsersStatus(int id)
        {
            Users entity = _usersService.GetById(id);

            entity.IsActive = !entity.IsActive;
            _usersService.UpdateUser(entity);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
        }


        #endregion [ ChangeStatus ]

        #region [View]

        [HttpGet]
        public IActionResult View(int? id)
        {
            UserManagerViewModel model = new UserManagerViewModel();

            if (id.HasValue)
            {

                Users Options = _usersService.GetById(id.Value);

                if (Options == null)
                {
                    return Redirect404();
                }
                model.Name = Options.Firstname +' '+ Options.Lastname;
                model.Email = Options.Email;
                model.Mobile = Options.Mobile;
            }
            return PartialView("_View", model);
        }

        #endregion
        public static string DecodeServerName(string encodedServername)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedServername));
        }

        [HttpGet]
        public IActionResult ResetPasswords(string id)
        {
            var a = DecodeServerName(id);
            var ID = Convert.ToInt32(a);
            return View(new ResetPasswordViewModel { Id = ID });
        }

        [HttpPost]
        public IActionResult ResetPasswords(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _usersService.GetById(model.Id);
                    if (user != null)
                    {
                        user.SaltKey = PasswordEncryption.CreateSaltKey();
                        user.Password = PasswordEncryption.GenerateEncryptedPassword(model.Password, user.SaltKey);
                        _usersService.UpdateUser(user);
                        ShowSuccessMessage("Success", "Password has been changed successfully", false);
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        ShowErrorMessage("Error", "Incorrect Old Password", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error", "Some error occurred.", false);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }

            return RedirectToAction("ResetPasswords", "usermanager");
        }
    }
}
