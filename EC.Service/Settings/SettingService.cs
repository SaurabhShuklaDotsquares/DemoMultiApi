using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace EC.Service
{
    public class SettingService : ISettingService
    {
        private readonly IRepository<Settings> _repoSettings;
        public SettingService(IRepository<Settings> repoSettings)
        {
            _repoSettings = repoSettings;
        }
        public Settings GetByFaviconSetting(byte faviconSettingType)
        {
            return _repoSettings.Query().Get().FirstOrDefault();
        }
        public Settings GetById(int id)
        {
            return _repoSettings.Query().Filter(x => x.Id == id).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdHOST(string entityHOST)
        {
            return _repoSettings.Query().Filter(x => x.Id == 9).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdNAME(string entityNAME)
        {
            return _repoSettings.Query().Filter(x => x.Id == 10).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdPASSWORD(string entityPASSWORD)
        {
            return _repoSettings.Query().Filter(x => x.Id == 11).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdPORT(string entityPORT)
        {
            return _repoSettings.Query().Filter(x => x.Id == 12).AsNoTracking().Get().FirstOrDefault();
        }
        public List<Settings> GetFaviconSettingList()
        {
            return _repoSettings.Query().Filter(x=>x.Slug == "SMTP_EMAIL_HOST" || x.Slug == "SMTP_USERNAME" || x.Slug == "SMTP_PASSWORD" || x.Slug == "SMTP_PORT").Get().ToList();
        }
        public List<Settings> GetLogoSettingList()
        {
            return _repoSettings.Query().Filter(x=>x.Slug == "MAIN_LOGO" || x.Slug == "MAIN_FAVICON").Get().ToList();
        }
        public List<Settings> GetSocialMediaSettingList()
        {
            return _repoSettings.Query().Filter(x=>x.Slug == "facebook" || x.Slug == "twitter" || x.Slug == "linkedin" || x.Slug == "youtube" || x.Slug == "google_plus").Get().ToList();
        }
        public PagedListResult<Settings> GetFaviconSettingsByPage(SearchQuery<Settings> query, out int totalItems)
        {
            return _repoSettings.Search(query, out totalItems);
        }
        public Settings SaveFaviconSetting(Settings faviconSetting)
        {
            _repoSettings.Insert(faviconSetting);
            return faviconSetting;
        }
        public Settings UpdateFaviconSetting(Settings faviconSetting)
        {
            _repoSettings.UpdateWithoutAttach(faviconSetting);
            return faviconSetting;
        }
        public Settings GetByIdfacebook(string entityfacebook)
        {
            return _repoSettings.Query().Filter(x => x.Id == 4).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdyoutube(string entityyoutube)
        {
            return _repoSettings.Query().Filter(x => x.Id == 7).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdlinkdin(string entitylinkdin)
        {
            return _repoSettings.Query().Filter(x => x.Id == 6).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdtwitter(string entitytwitter)
        {
            return _repoSettings.Query().Filter(x => x.Id == 5).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetByIdgoogle(string entitygoogle)
        {
            return _repoSettings.Query().Filter(x => x.Id == 8).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetById_Main_Logo(string entityLogo)
        {
            return _repoSettings.Query().Filter(x => x.Id == 1).AsNoTracking().Get().FirstOrDefault();
        }
        public Settings GetById_Main_Favicon(string entityFavicon)
        {
            return _repoSettings.Query().Filter(x => x.Id == 2).AsNoTracking().Get().FirstOrDefault();
        }
        public List<Settings> GetSettingListByManager(string[] manager)
        {
            return _repoSettings.Query().Filter(x => manager.Contains(x.Manager)).Get().ToList();
        }
        

        public bool Delete(int id)
        {
            Settings pages = _repoSettings.FindById(id);
            _repoSettings.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            if (_repoSettings != null)
            {
                _repoSettings.Dispose();
            }
        }
    }
}
