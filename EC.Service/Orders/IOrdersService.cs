using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Service.Specification;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EC.Service
{
    public  interface IOrdersService:IDisposable
    {
        public PagedListResult<Orders> Get(SearchQuery<Orders> query, out int totalItems);
        List<Orders> GetOrdersList();
        List<Orders> GetAOrdersList(string filterPeriod);
        List<Orders> GetAllOrdersList(string filterPeriod);
        public decimal GetOrdersTotalList(string filterPeriod);
        List<Orders> GetOrdersList(DateTime  S_date, DateTime E_date);
        List<Orders> GetOrdersVendorList(DateTime S_date, DateTime E_date, int id);
        List<Orders> GetProcessingList();
        List<Orders> GetProcessingList(DateTime? S_date, DateTime? E_date);
        List<Orders> GetOrdersreturnedList();
        List<Orders> GetVendorProcessingOrdersList(DateTime? S_date, DateTime? E_date, int id);
        public decimal GetVendorOrdersTotalList(DateTime? S_date, DateTime? E_date, int id);
        List<Orders> GetOrdersreturnedList(DateTime? S_date, DateTime? E_date);
        List<Orders> GetVendorOrdersList(DateTime? S_date, DateTime? E_date, int id);
        List<Orders> GetReturnVendorOrdersList(DateTime? S_date, DateTime? E_date, int id);
        List<Orders> GetOrderscancelledList();
        List<Orders> GetOrderscancelledList(DateTime? S_date, DateTime? E_date);
        public bool IsproductIdExists(int orderid);
        Orders GetById(int id);
        Orders GetBylastrecord();
        Orders GetByIdorderid(string order_id);
        Orders Save(Orders entity);
        Orders Update(Orders entity);
        OrderItems GetByItemsId(int id);
        OrderItems GetByItemsorderId(int id);
        List<OrderItems> GetByItemsOrderId(int id);
        Orders GetByorderIdBaseproduct(int userid, int id);
        OrderItems Saveitem(OrderItems entity);
        bool Delete(int id);
        //KeyValuePair<int, List<Orders>> GetOrderByUserId(int userId, int pageIndex, int pageSize);
        PageList<Orders> GetOrderByUserId(int userId, ToDoSearchSpecs specs);
        OrderItems GetByproduts(int order_id, int product_id);
        Orders GetByItemId(string order_id, int userId);
        Orders GetByItemId_invoice(int order_id);
        PageList<Orders> GetAllOrderForVendor(ToDoSearchSpecs specs, int? userId, string createdAt);
        OrderItems GetByOrderItemId(int id);

    }
}
