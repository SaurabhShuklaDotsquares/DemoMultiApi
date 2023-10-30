using EC.Data.Models;
using EC.DataTable.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service
{
    public interface IReturnItemsService:IDisposable
    {
        PagedListResult<ReturnItems> GetReturnItemsByPage(SearchQuery<ReturnItems> query, out int totalItems);
        ReturnItems GetById(int id);
        List<ReturnItems> GetReturnItemsList();
        ReturnItems UpdateReturnItems(ReturnItems ReturnItems);
        ReturnItems GetByReturnItems(byte ReturnItemsType);
        ReturnItems SaveReturnItems(ReturnItems ReturnItems);
        bool Delete(int id);
        ReturnItems GetByOrderItemId(int orderItemId);
        List<ReturnItems> GetByRequestId(int requestId);
        List<ReturnItems> UpdateReturnItemsCollection(List<ReturnItems> ReturnItem);

    }
}
