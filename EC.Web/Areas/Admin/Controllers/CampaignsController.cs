using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using EC.Data.Models;
using System.Linq;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Auth0.ManagementApi.Models;
using MvcContrib;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nancy.Json;
using EC.Core.Enums;
using EC.Web.Models.Others;
using static Dapper.SqlMapper;
using EC.Data.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using EC.Core.LIBS;
using System.Text;
using EC.Core;
using static System.Net.WebRequestMethods;
using System.Diagnostics;
//using System.Web.Helpers;

namespace EC.Web.Areas.Admin.Controllers
{
    public class CampaignsController : BaseController
    {
        #region Constructor
        private readonly ICampaignsService _campaignsService;
        private readonly IUsersService _usersService;
        private readonly ITemplateEmailService _templateEmailService;
        private readonly IEmailsTemplateService _emailSenderService;
        public CampaignsController(ICampaignsService campaignsService, IUsersService usersService, ITemplateEmailService templateEmailService, IEmailsTemplateService emailSenderService)
        {
            _campaignsService = campaignsService;
            _usersService = usersService;
            _templateEmailService = templateEmailService;
            _emailSenderService = emailSenderService;
        }
        #endregion


        #region [Index]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();
            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Campaigns>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch) || q.Template.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, string>(q => q.Template, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    //query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, string>(q => q.Progress, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, bool>(q => q.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, string>(q => q.Failed, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 8:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, DateTime?>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Campaigns, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Campaigns> entities = _campaignsService.GetCampaignItemsByPage(query, out total).Entities;
            List<string> userdata = new List<string>();
            foreach (Campaigns entity in entities)
            {
                userdata.Add(entity.Users);
                userdata.Add(entity.Progress);
                var UProgressLength= entity.Progress != null ? entity.Progress.Split(',').Length : 0;
                var Udata= entity.Users != null ? entity.Users.Split(',').Length : 0;
                var perc = UProgressLength > 0 && Udata > 0 ? (UProgressLength / Udata) * 100 : 0;
                var status = entity.Progress = (Udata > UProgressLength) ? "Pending" : "Completed";
                var progrees=0.0;
                var faild = (entity.Failed == null || entity.Failed.Length < 0) ? 0 : entity.Failed.Split(',').Length;
     
                if (entity.Progress != null)
                {
                    if (Udata > 0)
                    {
                        var pro = 0;
                        if (entity.Progress != null)
                        {
                            var prodata = entity.Progress.Split(',').Length;

                            pro = prodata;
                        }
                        var dd = ((Convert.ToDouble(pro)) + (Convert.ToDouble(faild)));
                       
                        progrees = ((Convert.ToDouble(pro)) / (Convert.ToDouble(dd))) * 100;
                    }
                }
                else
                {
                    progrees = 0;
                }
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title.ToString(),
                    entity.Template.ToString(),
                    String.Format("{0:0.##}", progrees),
                    status,
                    faild.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString(),
                });
                count++;
            }
            
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion

        #region [Create]
        [HttpGet]
        public IActionResult Create(int? id)
        {
            var model = new CampaignsViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            var categoriesList = _templateEmailService.GetEmailsList().Where(x=> !x.Islocked).ToList();

            List<SelectListItem> selectListItems = new List<SelectListItem>();
            foreach (var parentTitle in categoriesList)
            {
                    SelectListItem selectItem = new SelectListItem();
                    selectItem.Text = parentTitle.Name.ToString();
                    selectListItems.Add(selectItem);
            }
            model.TemplateList = selectListItems;

            if (id.HasValue)
            {
                Campaigns entity = _campaignsService.GetById(id.Value);
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.Template = entity.Template;
                model.Jsondata = entity.Users;
                }
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(int? id, CampaignsViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(string.IsNullOrEmpty(model.Jsondata))
                    {
                        ShowErrorMessage("Error!", "Select at least one user.", false);
                        return RedirectToAction("Create");
                    }
                    bool isIdExist = id.HasValue && id.Value != 0;
                    var campaign = _campaignsService.GetByTitle(model.Title.Trim());
                    if (campaign != null && !isIdExist)
                    {
                        ShowErrorMessage("Error!", "The title has been already taken.", false);
                        return RedirectToAction("Create");
                    }
                    var entity = isIdExist ? _campaignsService.GetById(model.Id) : new Campaigns();
                    entity.Title = model.Title;
                    entity.Template = model.Template;
                    entity.Users=model.Jsondata;
                    entity.Progress=  entity.Progress;    
                    entity.CreatedAt= DateTime.Now;
                    entity.UpdatedAt= DateTime.Now;
                    entity.Status= true;
                    entity = isIdExist ? _campaignsService.UpdateCampaignItems(entity) : _campaignsService.SaveCampaignItems(entity);
                    
                    ShowSuccessMessage("Success!", $"Template {(isIdExist ? "updated" : "created")} successfully", false);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region[Campaign_Users]
        [HttpGet]
        public IActionResult IndexUsers(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();
            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Data.Models.Users>();
            query.AddFilter(q => q.IsActive == true);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower();
                query.AddFilter(q => q.Email.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            //var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            //var sortDirection = Request.Form["sSortDir_0"];
            //switch (sortColumnIndex)
            //{
            //    case 2:
            //        query.AddSortCriteria(new ExpressionSortCriteria<Data.Models.Users, string>(q => q.Email, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
            //        break;

            //    default:
            //        query.AddSortCriteria(new ExpressionSortCriteria<Data.Models.Users, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
            //        break;
            //}
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Data.Models.Users> entities = _usersService.Get(query, out total).Entities.GroupBy(x=> x.Email).Select(x=> x.FirstOrDefault()).ToList();
            //int i = 0;
            foreach (Data.Models.Users entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    string.Empty,
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.IsActive ? "SUBSCRIBER" : entity.Firstname + ' ' + entity.Lastname,
                    entity.Email
                   });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion

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
                Message = "Are you sure you want to delete this Campaign information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Campaign Information" },
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
                bool isDeleted = _campaignsService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Campaign Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Campaigns");
        }

        #endregion [ DELETE ]

        #region[View Camaign]

        [HttpGet]
        public IActionResult View(int? id)
        {
            CampaignsViewModels model = new CampaignsViewModels();
            if (id.HasValue)
            {
                Campaigns campaigns = _campaignsService.GetById(id.Value);
                if (campaigns != null)
                {
                    var successUsersEmail = campaigns.Users != null ? campaigns.Users.Split(',').Length : 0;
                    //var status = campaigns.Progress == null ? "Pending" : "Completed";
                    var faild = campaigns.Failed == null ? 0 : campaigns.Failed.Split(',').Length;
                    var progrees = 0.0;

                    var pro = 0;
                    if (campaigns.Progress != null)
                    {
                        //var json = JsonConvert.SerializeObject(campaigns.Users.Split(','));
                        // var json1 = JsonConvert.SerializeObject(campaigns.Progress.Split('\\'));
                        //var intersect = json.Intersect(json1);
                        if (successUsersEmail > 0)
                        {
                            if (campaigns.Progress != null)
                            {
                                var successProgressEmail = campaigns.Progress.Split(',').Length;
                                if (successProgressEmail > 0)
                                {
                                    pro = successProgressEmail;
                                }
                                else
                                {
                                    pro = (successUsersEmail - successProgressEmail);
                                }
                            }
                            //var dd = ((Convert.ToDouble(pro)) + (Convert.ToDouble(faild)));
                            progrees = successUsersEmail < pro ? 0.0 : ((Convert.ToDouble(pro)) / (Convert.ToDouble(successUsersEmail))) * 100;
                        }
                    }
                    else
                    {
                        progrees = 0;
                    }
                    model.Id = campaigns.Id;
                    model.Title = campaigns.Title;
                    model.Template = campaigns.Template;
                    model.TotalUsers = successUsersEmail;
                    model.Progress = String.Format("{0:0.##}", progrees) + '%';
                    model.showStatus = campaigns.Users != null && campaigns.Progress != null ? campaigns.Users.Split(',').Length == campaigns.Progress.Split(',').Length ? "Completed" : "Processing" : "Pending"; 
                    model.Success = pro;
                    model.Failed = faild.ToString();
                    model.CreatedAt = campaigns.CreatedAt;
                    model.UpdatedAt = campaigns.UpdatedAt;
                }
                else
                {
                    return Redirect404();
                }
            }
            return View(model);
        }
        #endregion

        #region[Email Pending]
        [HttpGet]
        public async Task<IActionResult> Pending(int Id)
        {
            try
            {
                Campaigns model=new Campaigns();
                if (ModelState.IsValid)
                {
                    var campaigns = _campaignsService.GetById(Id);
                    if (campaigns.Users != null)
                    {
                        bool isSend = campaigns.Users != null && campaigns.Progress != null ? campaigns.Users.Split(',').Length == campaigns.Progress.Split(',').Length ? true : false : false;
                        if (isSend)
                        {
                            ShowErrorMessage("Error!", "The email has already been sent successfully.", false);
                            return RedirectToAction("View", new { id = Id });
                        }
                        if (await sendCampaignsPenddingEmail(campaigns.Id, campaigns.Users, campaigns.Template))
                        {
                            if (Task.CompletedTask.IsCompleted == true)
                            {
                                ShowSuccessMessage("Success!", "Email has been sent successfully to your email Id.", false);
                            }
                        }
                        ShowSuccessMessage("Success!", "Email has been sent successfully to your email Id.", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Email Id doesn't exists.", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error!", "Didn't find any email address to send", false);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", "Something went wrong with the process", false);
            }
            return RedirectToAction("View", new { id = Id });
          
        }

        public static string EncodeServerName(string serverName)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(serverName));
        }

        public  async Task<bool>  sendCampaignsPenddingEmail(int Id, string email,string tamplate)
        {
            bool isSendEmail = false;
            Campaigns model = new Campaigns();
            List<string> emails1 = new List<string>();
            List<string> emails = new List<string>();

            string Smail = string.Empty;
            string Fmail = string.Empty;
            string Progressmail = string.Empty;
            string faildsmail = string.Empty;
            string subject = string.Empty;
            var url = string.Empty;
            var clickme = "";
            var Name = string.Empty;
            var Address = string.Empty;

            var campaigns = _campaignsService.GetById(Id);
            var FUser = _campaignsService.GetById(Id);
            emails = email.TrimStart('[').TrimEnd(']').Replace("\\","").Split(',').ToList();
            Data.Models.Emails data = new Data.Models.Emails();
            for (int i = 0; i < emails.Count; i++)
            {
                var userEmail = emails[i].Replace('"', ' ').TrimStart('"').TrimEnd('"').Trim();
                //var eml1 = emails[i].TrimStart('"').TrimEnd('"').Trim();
                var users = _usersService.GetAdminUserByEmailpending(userEmail);
                if (users != null)
                {
                    data = _templateEmailService.GetById((int)EmailType.Ecommercecampaign);
                    if (data != null)
                    {
                        url = data.Description;
                        subject = data.Subject.ToTitleCase();
                    }
                    var user = _usersService.GetById(users.Id);
                    if (user != null)
                    {
                        Name = user.Firstname + ' ' + user.Lastname;
                        Address = user.UserAddress.ToString();
                    }

                    IDictionary<string, string> d = new Dictionary<string, string>();
                    d.Add(new KeyValuePair<string, string>("##UserName##", Name));
                    d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                    d.Add(new KeyValuePair<string, string>("##CurrentDate##", DateTime.Now.ToString()));
                    d.Add(new KeyValuePair<string, string>("##AppName##", "E-Commerce"));
                    d.Add(new KeyValuePair<string, string>("##Url##", "<a href='" + SiteKeys.Domain + "'>" + SiteKeys.Domain + "</a>"));
                    d.Add(new KeyValuePair<string, string>("##Title##", subject));
                    d.Add(new KeyValuePair<string, string>("##SupportMail##", "support@ecommerceportal.24livehost.com"));

                    clickme = url;
                    foreach (KeyValuePair<string, string> ele in d)
                    {
                        clickme = clickme.Replace(ele.Key, ele.Value);
                    }
                    var urlToClick = clickme;
                    await _emailSenderService.SendEmailAsync(userEmail, subject, urlToClick);
                    if (Task.CompletedTask.IsCanceled == true)
                    {
                        Fmail = Fmail + "\"" + userEmail + "\",";
                        faildsmail = Fmail.TrimEnd(',');
                        FUser.Id = Id;
                        FUser.Failed = faildsmail;
                        _campaignsService.UpdateCampaignItems(FUser);
                    }
                    if (Task.CompletedTask.IsCompleted == true)
                    {
                        Smail = Smail + "\"" + userEmail + "\",";
                        Progressmail = Smail.TrimEnd(',');
                        campaigns.Id = Id;
                        campaigns.Progress = Progressmail; 
                        _campaignsService.UpdateCampaignItems(campaigns);
                        isSendEmail = true;
                    }
                }
            }
            //if (Task.CompletedTask.IsCompleted)
            //{
            //    Progressmail = "[" + Progressmail + "]";
            //    campaigns.Id = Id;
            //    campaigns.Progress = Progressmail;
            //    _campaignsService.UpdateCampaignItems(campaigns);
            //    isSendEmail = true;
            //}
            //else
            //{
            //    faildsmail = "[" + faildsmail + "]";
            //    FUser.Id = Id;
            //    FUser.Failed = faildsmail;
            //    _campaignsService.UpdateCampaignItems(FUser);
            //}
           
            return isSendEmail;
        }
        #endregion

        #region[Eamil Faild]
        [HttpGet]
        public async Task<IActionResult> FaildMail(int Id)
        {
            try
            {
                Campaigns model = new Campaigns();
                if (ModelState.IsValid)
                {
                    var users = _campaignsService.GetById(Id);
                    if (users != null)
                    {
                        if (users.Failed != null)
                        {
                            if (await sendCampaignsFaildEmail(users.Id, users.Failed, users.Template))
                            {
                                if (Task.CompletedTask.IsCompleted == true)
                                {
                                    ShowSuccessMessage("Success", "Email has been sent successfully to your email Id.", false);
                                }
                            }
                            ShowSuccessMessage("Success", "Email has been sent successfully to your email Id.", false);
                        }
                        else
                        {
                            ShowErrorMessage("Error!", "The email has already been sent successfully.", false);
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Email Id doesn't exists.", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error!", "Didn't find any email address to send", false);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", "Something went wrong with the process", false);
            }
            return RedirectToAction("View", new { id = Id });
        }
        public async Task<bool> sendCampaignsFaildEmail(int Id, string email, string tamlate)
        {
            bool isSendEmail = false;
            Campaigns model = new Campaigns();
            List<string> emails1 = new List<string>();
            List<string> emails = new List<string>();

            string Smail = string.Empty;
            string Fmail = string.Empty;
            string Progressmail = string.Empty;
            string faildsmail = string.Empty;
            string subject = string.Empty;
            var url = string.Empty;
            var clickme = string.Empty;
            var Name = string.Empty;
            var Address = string.Empty; 

            var users1 = _campaignsService.GetById(Id);
            var FUser = _campaignsService.GetById(Id);
            emails = email.TrimStart('[').TrimEnd(']').Replace("\\", "").Split(',').ToList();
            Data.Models.Emails data = new Data.Models.Emails();
            for (int i = 0; i < emails.Count; i++)
            {
                var eml = emails[i].TrimStart('"').TrimEnd('"').Trim();
                var users = _usersService.GetAdminUserByEmailpending(eml).Id;
                data = _templateEmailService.GetById((int)EmailType.Ecommercecampaign);
               if(data != null)
               {
                    url = data.Description;
                    subject = data.Subject.ToTitleCase();
               }
               
                var user = _usersService.GetById(users);
                if(user != null)
                {
                    Name = user.Firstname + ' ' + user.Lastname;
                    Address = user.UserAddress.ToString();
                }
                var DATE = DateTime.Now.ToString();

                IDictionary<string, string> d = new Dictionary<string, string>();
                d.Add(new KeyValuePair<string, string>("##UserName##", Name));
                d.Add(new KeyValuePair<string, string>("##Address##", "J3, Jhalana Institutional Area, Jhalana Dungri, Jaipur, Rajasthan 302004"));
                d.Add(new KeyValuePair<string, string>("##CurrentDate##", DateTime.Now.ToString()));
                d.Add(new KeyValuePair<string, string>("##AppName##", "E-Commerce"));
                d.Add(new KeyValuePair<string, string>("##Url##", "<a href='" + SiteKeys.Domain + "'>" + SiteKeys.Domain + "</a>"));
                d.Add(new KeyValuePair<string, string>("##Title##", subject));
                d.Add(new KeyValuePair<string, string>("##SupportMail##", "support@ecommerceportal.24livehost.com"));

                clickme = url;
                foreach (KeyValuePair<string, string> ele in d)
                {
                    clickme = clickme.Replace(ele.Key, ele.Value);
                }
                var urlToClick = clickme;
                await _emailSenderService.SendEmailAsync(eml, subject, urlToClick);
                if (Task.CompletedTask.IsCanceled == true)
                {
                    Fmail = Fmail + "\"" + eml + "\",";
                    faildsmail = Fmail.TrimEnd(',');
                }
                if (Task.CompletedTask.IsCompleted == true)
                {
                    Smail = Smail + "\"" + eml + "\",";
                    Progressmail = Smail.TrimEnd(',');
                }
            }
            if (Task.CompletedTask.IsCompleted)
            {
                Progressmail = "[" + Progressmail + "]";
                users1.Id = Id;
                users1.Progress = Progressmail;
                _campaignsService.UpdateCampaignItems(users1);
            }
            else
            {
                faildsmail = "[" + faildsmail + "]";
                FUser.Id = Id;
                FUser.Failed = faildsmail;
                _campaignsService.UpdateCampaignItems(FUser);
            }
            return isSendEmail;
        }
        #endregion

    }
}
