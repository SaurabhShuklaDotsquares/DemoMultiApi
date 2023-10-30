using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PredicateBuilderExt = EC.Core.PredicateBuilder;

namespace EC.Service.ReturnRequest
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IRepository<ReturnRequests> _repoReturnRequests;
        private readonly IRepository<ReturnItems> _repoReturnItems;
        public ReturnRequestService(IRepository<ReturnRequests> repoReturnRequests, IRepository<ReturnItems> repoReturnItems)
        {
            _repoReturnRequests = repoReturnRequests;
            _repoReturnItems = repoReturnItems;
        }
        public ReturnRequests GetById(int id)
        {
            return _repoReturnRequests.Query().Include(x=>x.User).Include(x => x.Order).Include(x=>x.Order.OrderItems).Include(x=>x.ReturnItems).Filter(x => x.Id == id).AsNoTracking().Get().FirstOrDefault();
        }
        public ReturnRequests GetByReturnRequests(byte ReturnRequestsType)
        {
            return _repoReturnRequests.Query().Get().FirstOrDefault();
        }
        public PagedListResult<ReturnRequests> GetReturnRequestsByPage(SearchQuery<ReturnRequests> query, out int totalItems)
        {
            return _repoReturnRequests.Search(query, out totalItems);
        }
        public List<ReturnRequests> GetReturnRequestsList()
        {
            return _repoReturnRequests.Query().Get().ToList();
        }
        public ReturnRequests SaveReturnRequests(ReturnRequests ReturnRequests)
        {
            _repoReturnRequests.Insert(ReturnRequests);
            return ReturnRequests;
        }
        public List<ReturnRequests> GetVendorReturnOrdersList(DateTime? S_date, DateTime? E_date, int id)
        {
            var returneddata = _repoReturnRequests.Query().Get().Where(x => x.UserId == id && x.CreatedAt >= S_date && x.UpdatedAt.Value <= E_date).ToList();
            return returneddata;
        }
        public ReturnRequests UpdateReturnRequests(ReturnRequests ReturnRequests)
        {
            _repoReturnRequests.UpdateUnchangedEntity(ReturnRequests);
            return ReturnRequests;
        }
        public bool Delete(int id)
        {
            ReturnRequests pages = _repoReturnRequests.FindById(id);
            _repoReturnRequests.Delete(pages);
            return true;
        }
        public ReturnRequests GetByOrderId(int id)
        {
            return _repoReturnRequests.Query().Include(x => x.User).Include(x => x.Order.OrderItems).Include(x => x.ReturnItems).Filter(x => x.OrderId == id).AsNoTracking().Get().OrderByDescending(x => x.CreatedAt).FirstOrDefault();
        }
        public ReturnItems DeleteReturnItems(ReturnItems returnItems)
        {
            _repoReturnItems.Delete(returnItems);
            return returnItems;
        }

        public PageList<ReturnRequests> GetReturnRequestListForVendor(ToDoSearchSpecs specs, int? userId)
        {
            var predicate = PredicateBuilderExt.True<ReturnRequests>();
            //predicate = predicate.And(e => e. == userId);
            
            var returnRequestList = _repoReturnRequests
                .Query()
                .Filter(predicate)
                .Include(x => x.ReturnItems)
                .Include(x => x.Order)
                .Include(x => x.Order.OrderItems)
                .Include(x => x.User)
                .Get()
                .OrderByDescending(x => x.Id)
                .ToList();

            return PageList<ReturnRequests>.ToPageList(returnRequestList, specs.page, specs.PageSize);
            //returnRequestList = SortHelper<ReturnRequests>.ApplySort(returnRequestList, specs.OrderBy).ToList();
            //return PageList<ReturnRequests>.ToPageList(returnRequestList, specs.PageNumber, specs.PageSize);
        }
        public void Dispose()
        {
            if (_repoReturnRequests != null)
            {
                _repoReturnRequests.Dispose();
            }
        }
    }
}
