using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface ICampaignsService:IDisposable
    {
        PagedListResult<Campaigns> GetCampaignItemsByPage(SearchQuery<Campaigns> query, out int totalItems);
        Campaigns GetById(int id);
        List<Campaigns> GetCampaignItemsList();
        Campaigns UpdateCampaignItems(Campaigns CampaignItems);
        Campaigns GetByCampaignItems(byte CampaignItemsType);
        Campaigns SaveCampaignItems(Campaigns CampaignItems);
        bool Delete(int id);
        Campaigns GetByTitle(string title);
    }
}
