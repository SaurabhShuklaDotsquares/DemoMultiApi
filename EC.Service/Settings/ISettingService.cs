using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ISettingService:IDisposable
    {
        PagedListResult<Settings> GetFaviconSettingsByPage(SearchQuery<Settings> query, out int totalItems);
        Settings GetById(int id);
        Settings GetByIdHOST(string entityHOST);
        Settings GetByIdNAME(string entityNAME);
        Settings GetByIdPASSWORD(string entityPASSWORD);
        Settings GetByIdPORT(string entityPORT);
        List<Settings> GetSocialMediaSettingList();
        List<Settings> GetFaviconSettingList();
        List<Settings> GetLogoSettingList();
        Settings UpdateFaviconSetting(Settings faviconSetting);
        Settings GetByFaviconSetting(byte faviconSettingType);
        Settings SaveFaviconSetting(Settings faviconSetting);
        Settings GetByIdfacebook(string entityfacebook);
        Settings GetByIdyoutube(string entityyoutube);
        Settings GetByIdlinkdin(string entitylinkdin);
        Settings GetByIdtwitter(string entitytwitter);
        Settings GetByIdgoogle(string entitygoogle);
        Settings GetById_Main_Logo(string entityLogo);
        Settings GetById_Main_Favicon(string entityFavicon);
        bool Delete(int id);
        List<Settings> GetSettingListByManager(string[] manager);
    }
}
