using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Service
{
    public class ReturnItemsService : IReturnItemsService
    {
        private readonly IRepository<ReturnItems> _repoReturnItems;
        public ReturnItemsService(IRepository<ReturnItems> repoReturnItems)
        {
            _repoReturnItems = repoReturnItems;
        }
        public ReturnItems GetById(int id)
        {
            return _repoReturnItems.Query().Filter(x => x.RequestId == id).AsNoTracking().Get().FirstOrDefault();
        }
        public ReturnItems GetByReturnItems(byte ReturnItemsType)
        {
            return _repoReturnItems.Query().Get().FirstOrDefault();
        }
        public PagedListResult<ReturnItems> GetReturnItemsByPage(SearchQuery<ReturnItems> query, out int totalItems)
        {
            return _repoReturnItems.Search(query, out totalItems);
        }
        public List<ReturnItems> GetReturnItemsList()
        {
            return _repoReturnItems.Query().Get().ToList();
        }
        public ReturnItems SaveReturnItems(ReturnItems ReturnItem)
        {
            _repoReturnItems.Insert(ReturnItem);
            return ReturnItem;
        }
        public ReturnItems UpdateReturnItems(ReturnItems ReturnItem)
        {
            _repoReturnItems.Update(ReturnItem);
            return ReturnItem;
        }
        public List<ReturnItems> UpdateReturnItemsCollection(List<ReturnItems> ReturnItem)
        {
            _repoReturnItems.UpdateCollection(ReturnItem);
            return ReturnItem;
        }
        public bool Delete(int id)
        {
            ReturnItems pages = _repoReturnItems.FindById(id);
            _repoReturnItems.Delete(pages);
            return true;
        }
        public ReturnItems GetByOrderItemId(int orderItemId)
        {
            return _repoReturnItems.Query().Filter(x => x.OrderItemId == orderItemId).Get().FirstOrDefault();
        }
        public List<ReturnItems> GetByRequestId(int requestId)
        {
            return _repoReturnItems.Query().Filter(x => x.RequestId == requestId).Include(x=> x.OrderItem).Get().ToList();
        }
        public void Dispose()
        {
            if (_repoReturnItems != null)
            {
                _repoReturnItems.Dispose();
            }
        }
    }
}
