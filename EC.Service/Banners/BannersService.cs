using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class BannersService : IBannersService
    {
        private readonly IRepository<Banners> _repoBanners;
        public BannersService(IRepository<Banners> repoBanners)
        {
            _repoBanners=repoBanners;
        }
        public PagedListResult<Banners> GetBannerByPage(SearchQuery<Banners> query, out int totalItems)
        {
            return _repoBanners.Search(query, out totalItems);
        }
        public Banners GetById(int id)
        {
            return _repoBanners.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public Banners GetByTitle(string title)
        {
            return _repoBanners.Query().Filter(x => x.Title == title).Get().FirstOrDefault();
        }
        public List<Banners> Getlist()
        {
            return _repoBanners.Query().Get().ToList();
        }

        public List<Banners> GetActiveBannerList()
        {
            return _repoBanners.Query().Filter(x=> x.Status == true).Get().ToList();
        }
        public Banners UpdateBanner(Banners Pages)
        {
            _repoBanners.Update(Pages);
            return Pages;
        }
        public Banners SaveBanner(Banners Pages)
        {
            _repoBanners.Insert(Pages);
            return Pages;
        }
        public bool Delete(int id)
        {
            Banners pages = _repoBanners.FindById(id);
            _repoBanners.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            if (_repoBanners!=null)
            {
                _repoBanners.Dispose();
            }
        }
    }
}
