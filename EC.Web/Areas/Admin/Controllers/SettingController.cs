using EC.Data.Models;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Service.Shippings;
using EC.Service;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace EC.Web.Areas.Admin.Controllers
{
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SettingController(ISettingService settingService, IWebHostEnvironment webHostEnvironment)
        {
            _settingService = settingService;
            this.webHostEnvironment = webHostEnvironment;
        }
        #region [Index]
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
            var query = new SearchQuery<Settings>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Settings, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Settings, string>(q => q.Slug, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Settings, string>(q => q.ConfigValue, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Settings, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Descending : SortDirection.Ascending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Settings> entities = _settingService.GetFaviconSettingsByPage(query, out total).Entities;

            foreach (Settings entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title.ToString(),
                    entity.Slug.ToString(),
                    entity.ConfigValue.ToString(),
                    entity.CreatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [Create]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new SettingViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            var entity = _settingService.GetLogoSettingList();
            model.MAIN_LOGOSlug = entity[0].Slug;
            model.MAIN_FAVICONSlug = entity[1].Slug;
            model.MAIN_LOGOConfigValue = entity[0].ConfigValue;
            model.MAIN_FAVICONConfigValue = entity[1].ConfigValue;
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(int? id, SettingViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueLOGOFileName = ProcessUploadedFile(model);
                    string uniqueFileName = ProcessUploadedFAVICONFile(model);

                    var entityHOST = _settingService.GetLogoSettingList().Where(x => x.Slug == "MAIN_LOGO").FirstOrDefault().Id;
                    bool isExist = 9 != 0;
                    var entityNAME = _settingService.GetLogoSettingList().Where(x => x.Slug == "MAIN_FAVICON").FirstOrDefault().Id;

                    if (entityHOST == 1)
                    {
                        var entity = isExist ? _settingService.GetById_Main_Logo("MAIN_LOGO") : new Settings();
                        entity.Slug = model.MAIN_LOGOSlug;
                        entity.ConfigValue = uniqueLOGOFileName != null && uniqueLOGOFileName != string.Empty ? uniqueLOGOFileName : entity.ConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                        if (entity.ConfigValue != null)
                        {
                            HttpContext.Session.SetString("Main_Logo", entity.ConfigValue);
                        }
                    }
                    if (entityNAME == 2)
                    {
                        var entity = isExist ? _settingService.GetById_Main_Favicon("MAIN_FAVICON") : new Settings();
                        entity.Slug = model.MAIN_FAVICONSlug;
                        entity.ConfigValue = uniqueFileName != null && uniqueFileName != string.Empty ? uniqueFileName : entity.ConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                        if (entity.ConfigValue!=null)
                        {
                            HttpContext.Session.SetString("Main_FAVICON", entity.ConfigValue);
                        }
                    }
                    ShowSuccessMessage("Success!", $"Logo {(isExist ? "updated" : "created")} successfully", false);
                    return RedirectToAction("Create");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("Create");
        }
        private string ProcessUploadedFile(SettingViewModels model)
        {
            string uniqueLOGOFileName = null;

            if (model.MAIN_LOGOConfigValue1 != null)
            {
                
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueLOGOFileName = Guid.NewGuid().ToString() + "_" + model.MAIN_LOGOConfigValue1.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueLOGOFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.MAIN_LOGOConfigValue1.CopyTo(fileStream);
                }
            }
            return uniqueLOGOFileName;
        }
        private string ProcessUploadedFAVICONFile(SettingViewModels model)
        {
            string uniqueFileName = null;

            if (model.MAIN_FAVICONConfigValue1 != null)
            {

                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.MAIN_FAVICONConfigValue1.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.MAIN_FAVICONConfigValue1.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        #endregion

        #region[View]
        [HttpGet]
        public IActionResult View(int? id)
        {
            SettingViewModels model = new SettingViewModels();
            if (id.HasValue)
            {
                Settings setting = _settingService.GetById(id.Value);
                if (setting == null)
                {
                    return Redirect404();
                }
                model.Title = setting.Title;
                model.Manager = setting.Manager;
                model.Slug = setting.Slug;
                model.ConfigValue = setting.ConfigValue;
                model.FieldType = setting.FieldType;
                model.CreatedAt = setting.CreatedAt;
                model.UpdatedAt = setting.UpdatedAt;
            }
            return View(model);
        }

        #endregion

        #region [Edit General Setting]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var model = new SettingViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            if (id.HasValue)
            {
                Settings entity = _settingService.GetById(id.Value);
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.Slug = entity.Slug;
                model.ConfigValue = entity.ConfigValue;
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(int? id, SettingViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                        bool isExist = id.HasValue && id.Value != 0;
                        var entity = isExist ? _settingService.GetById(model.Id) : new Settings();
                        entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                        entity.UpdatedAt = model.UpdatedAt;
                        entity.Title = model.Title;
                        entity.Slug = model.Slug;
                        entity.ConfigValue = model.ConfigValue;

                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                        ShowSuccessMessage("Success!", $"Setting {(isExist ? "updated" : "created")} successfully.", false);
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

        #region [Edit SMTP Detail]
        [HttpGet]
        public IActionResult EditSMTP()
        {
            var model = new SettingViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

               var entity = _settingService.GetFaviconSettingList();
               model.SMTP_EMAIL_HOSTSlug = entity[0].Slug;
               model.SMTP_USERNAMESlug = entity[1].Slug;
               model.SMTP_PASSWORDSlug = entity[2].Slug;
               model.SMTP_PORTSlug = entity[3].Slug;
               model.SMTP_EMAIL_HOST = entity[0].ConfigValue;
               model.SMTP_USERNAME = entity[1].ConfigValue;
               model.SMTP_PASSWORD = entity[2].ConfigValue;
               model.SMTP_PORT = entity[3].ConfigValue;

            return View(model);
        }
        [HttpPost]
        public IActionResult EditSMTP(int? id, SettingViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entityHOST = _settingService.GetFaviconSettingList().Where(x=>x.Slug== "SMTP_EMAIL_HOST").FirstOrDefault().Id;
                    bool isExist = 9 != 0;
                    var entityNAME = _settingService.GetFaviconSettingList().Where(x => x.Slug == "SMTP_USERNAME").FirstOrDefault().Id;
                    var entityPASSWORD = _settingService.GetFaviconSettingList().Where(x => x.Slug == "SMTP_PASSWORD").FirstOrDefault().Id;
                    var entityPORT = _settingService.GetFaviconSettingList().Where(x => x.Slug == "SMTP_PORT").FirstOrDefault().Id;
                    
                    if (entityHOST == 9)
                    {
                        var entity = isExist ? _settingService.GetByIdHOST("SMTP_EMAIL_HOST") : new Settings();
                        entity.Slug = model.SMTP_EMAIL_HOSTSlug;
                        entity.ConfigValue = model.SMTP_EMAIL_HOST;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entityNAME == 10)
                    {
                        var entity = isExist ? _settingService.GetByIdNAME("SMTP_USERNAME") : new Settings();
                        entity.Slug = model.SMTP_USERNAMESlug;
                        entity.ConfigValue = model.SMTP_USERNAME;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entityPASSWORD == 11)
                    {
                        var entity = isExist ? _settingService.GetByIdPASSWORD("SMTP_PASSWORD") : new Settings();
                        entity.Slug = model.SMTP_PASSWORDSlug;
                        entity.ConfigValue = model.SMTP_PASSWORD;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entityPORT == 12)
                    {
                        var entity = isExist ? _settingService.GetByIdPORT("SMTP_PORT") : new Settings();
                        entity.Slug = model.SMTP_PORTSlug;
                        entity.ConfigValue = model.SMTP_PORT;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    ShowSuccessMessage("Success!", $"SMTPSetting {(isExist ? "updated" : "created")} successfully", false);
                    return RedirectToAction("EditSMTP");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("EditSMTP");
        }

        #endregion

        #region[Edit Social Media]

        [HttpGet]
        public IActionResult EditSocialMedia()
        {
            var model = new SettingViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            var entity = _settingService.GetSocialMediaSettingList();
            model.facebookSlug = entity[0].Slug;
            model.twitterSlug = entity[1].Slug;
            model.LinkdinSlug = entity[2].Slug;
            model.YoutubeSlug = entity[3].Slug;
            model.google_plusSlug = entity[4].Slug;
            model.facebookConfigValue = entity[0].ConfigValue;
            model.twitterConfigValue = entity[1].ConfigValue;
            model.LinkdinConfigValue = entity[2].ConfigValue;
            model.YoutubeConfigValue = entity[3].ConfigValue;
            model.google_plusConfigValue = entity[4].ConfigValue;

            return View(model);
        }
        [HttpPost]
        public IActionResult EditSocialMedia(int? id, SettingViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var entitybook = _settingService.GetSocialMediaSettingList().Where(x => x.Slug == "facebook").FirstOrDefault().Id;
                    bool isExist = 4 != 0;
                    var entitytwitter = _settingService.GetSocialMediaSettingList().Where(x => x.Slug == "twitter").FirstOrDefault().Id;
                    var entitylinkedin = _settingService.GetSocialMediaSettingList().Where(x => x.Slug == "linkedin").FirstOrDefault().Id;
                    var entityyoutube = _settingService.GetSocialMediaSettingList().Where(x => x.Slug == "youtube").FirstOrDefault().Id;
                    var entitygoogle = _settingService.GetSocialMediaSettingList().Where(x => x.Slug == "google_plus").FirstOrDefault().Id;

                    if (entitybook == 4)
                    {
                        var entity = isExist ? _settingService.GetByIdfacebook("facebook") : new Settings();
                        entity.Slug = model.facebookSlug;
                        entity.ConfigValue = model.facebookConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entitytwitter == 5)
                    {
                        var entity = isExist ? _settingService.GetByIdtwitter("twitter") : new Settings();
                        entity.Slug = model.twitterSlug;
                        entity.ConfigValue = model.twitterConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entitylinkedin == 6)
                    {
                        var entity = isExist ? _settingService.GetByIdlinkdin("linkedin") : new Settings();
                        entity.Slug = model.LinkdinSlug;
                        entity.ConfigValue = model.LinkdinConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entityyoutube == 7)
                    {
                        var entity = isExist ? _settingService.GetByIdyoutube("youtube") : new Settings();
                        entity.Slug = model.YoutubeSlug;
                        entity.ConfigValue = model.YoutubeConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    if (entitygoogle == 8)
                    {
                        var entity = isExist ? _settingService.GetByIdgoogle("google_plus") : new Settings();
                        entity.Slug = model.google_plusSlug;
                        entity.ConfigValue = model.google_plusConfigValue;
                        entity = isExist ? _settingService.UpdateFaviconSetting(entity) : _settingService.SaveFaviconSetting(entity);
                    }
                    ShowSuccessMessage("Success!", $"SocialMedia {(isExist ? "updated" : "created")} successfully", false);
                    return RedirectToAction("EditSocialMedia");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("EditSocialMedia");
        }

        #endregion

    }
}
