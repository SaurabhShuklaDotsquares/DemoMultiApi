using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class CampaignsService : ICampaignsService
    {
        private readonly IRepository<Campaigns> _repoCampaigns;
        public CampaignsService(IRepository<Campaigns> repoCampaigns)
        {
            _repoCampaigns = repoCampaigns;
        }
        public Campaigns GetByCampaignItems(byte CampaignItemsType)
        {
            return _repoCampaigns.Query().Get().FirstOrDefault();
        }
        public Campaigns GetById(int id)
        {
            return _repoCampaigns.Query().Filter(x => x.Id == id).AsNoTracking().Get().FirstOrDefault();
        }
        public Campaigns GetByTitle(string title)
        {
            return _repoCampaigns.Query().Filter(x => x.Title == title).AsNoTracking().Get().FirstOrDefault();
        }
        public PagedListResult<Campaigns> GetCampaignItemsByPage(SearchQuery<Campaigns> query, out int totalItems)
        {
            return _repoCampaigns.Search(query, out totalItems);
        }
        public List<Campaigns> GetCampaignItemsList()
        {
            return _repoCampaigns.Query().Get().ToList();
        }
        public Campaigns SaveCampaignItems(Campaigns CampaignItems)
        {
            _repoCampaigns.Insert(CampaignItems);
            return CampaignItems;
        }
        public Campaigns UpdateCampaignItems(Campaigns CampaignItems)
        {
            _repoCampaigns.Update(CampaignItems);
            return CampaignItems;
        }
        public bool Delete(int id)
        {
            Campaigns pages = _repoCampaigns.FindById(id);
            _repoCampaigns.Delete(pages);
            return true;
        }
        public void Dispose()
        {
            if (_repoCampaigns != null)
            {
                _repoCampaigns.Dispose();
            }
        }
    }
}
