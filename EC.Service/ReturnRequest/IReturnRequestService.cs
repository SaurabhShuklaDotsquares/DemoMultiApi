using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Service.ReturnRequest
{
    public interface IReturnRequestService:IDisposable
    {
        PagedListResult<ReturnRequests> GetReturnRequestsByPage(SearchQuery<ReturnRequests> query, out int totalItems);
        ReturnRequests GetById(int id);
        List<ReturnRequests> GetReturnRequestsList();
        ReturnRequests UpdateReturnRequests(ReturnRequests ReturnRequests);
        List<ReturnRequests> GetVendorReturnOrdersList(DateTime? S_date, DateTime? E_date, int id);
        ReturnRequests GetByReturnRequests(byte ReturnRequestsType);
        ReturnRequests SaveReturnRequests(ReturnRequests ReturnRequests);
        bool Delete(int id);
        ReturnRequests GetByOrderId(int id);
        ReturnItems DeleteReturnItems(ReturnItems returnItems);
        PageList<ReturnRequests> GetReturnRequestListForVendor(ToDoSearchSpecs specs, int? userId);
    }
}
