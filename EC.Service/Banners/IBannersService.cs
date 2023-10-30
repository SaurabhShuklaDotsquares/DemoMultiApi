using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IBannersService:IDisposable
    {
        PagedListResult<Banners> GetBannerByPage(SearchQuery<Banners> query, out int totalItems);
        Banners GetById(int id);
        List<Banners> Getlist();
        Banners UpdateBanner(Banners Pages);
        Banners SaveBanner(Banners Pages);
        bool Delete(int id);
        List<Banners> GetActiveBannerList();
        Banners GetByTitle(string title);
    }
}
