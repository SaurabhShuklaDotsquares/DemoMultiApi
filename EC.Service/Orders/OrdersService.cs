using EC.API.Helper;
using EC.Data.Models;
using EC.DataTable.Search;
using EC.Repo;
using EC.Service.Helpers;
using EC.Service.Specification;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using PredicateBuilderExt = EC.Core.PredicateBuilder;

namespace EC.Service
{
    public class OrdersService : IOrdersService
    {
        private readonly IRepository<Orders> _repoOrders;
        private readonly IRepository<OrderItems> _repoOrderItems;
        public OrdersService(IRepository<Orders> repoOrders, IRepository<OrderItems> repoOrderItems)
        {
            _repoOrders = repoOrders;
            _repoOrderItems = repoOrderItems;
        }
        public PagedListResult<Orders> Get(SearchQuery<Orders> query, out int totalItems)
        {
            return _repoOrders.Search(query, out totalItems);
        }
        public Orders GetById(int id)
        {
            return _repoOrders.Query().Filter(x => x.Id == id).Include(x => x.OrderItems).Include(x=> x.User).Get().FirstOrDefault();
        }
        public OrderItems GetByOrderItemId(int id)
        {
            return _repoOrderItems.Query().Filter(x => x.Id == id).Include(x=>x.Product).Get().FirstOrDefault();
        }
        public Orders GetBylastrecord()
        {
            var x= _repoOrders.Query().Get().OrderByDescending(x => x.Id).FirstOrDefault();
           
            return x;
        }
        public Orders GetByIdorderid(string order_id)
        {
            return _repoOrders.Query().Filter(x => x.OrderId == order_id).Include(x => x.OrderItems).Include(x=> x.User).Get().FirstOrDefault();
        }
        public Orders Save(Orders entity)
        {
            _repoOrders.Insert(entity);
            return entity;
        }
        public List<Orders> GetOrdersList()
        {
            return _repoOrders.Query().Get().Where(x => x.Status == "completed").ToList();
        }
        public List<Orders> GetAOrdersList(string filterPeriod)
        {
            if (filterPeriod == "15")
            {
                return _repoOrders.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-15)).ToList();
            }
            else if (filterPeriod == "30")
            {
                return _repoOrders.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-30)).ToList();
            }
            else
            {
                return _repoOrders.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-7)).ToList();
            }
        }
        public bool IsproductIdExists(int orderid)
        {
            bool isExist = _repoOrders.Query().Filter(x => x.Id.Equals(orderid)).Get().FirstOrDefault() != null;
            return isExist;
        }
        public decimal GetVendorOrdersTotalList(DateTime? S_date, DateTime? E_date, int id)
        {
            var finalcommissiondata = 0.0;
            var ShippingAmmount = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Value.Date >= S_date.Value.Date && x.CreatedAt.Value.Date <= E_date.Value.Date && x.Status == "completed").Select(x => x.ShippingAmount).Sum();

            var ListingAmmountdat = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Value.Date >= S_date.Value.Date && x.CreatedAt.Value.Date <= E_date.Value.Date && x.Status == "completed").ToList();
            foreach (var item in ListingAmmountdat)
            {
                decimal finalcommission = (item.Amount * Convert.ToDecimal(item.AdminCommission)/ 100);
                finalcommissiondata += Convert.ToDouble(finalcommission);
            }

            //var admincommissionAmmount = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Value.Date >= S_date.Value.Date && x.CreatedAt.Value.Date <= E_date.Value.Date && x.Status == "completed").Select(x => x.AdminCommission).Sum();

            var TotalAmmount = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Value.Date >= S_date.Value.Date && x.CreatedAt.Value.Date <= E_date.Value.Date && x.Status == "completed").Select(x => x.Amount).Sum();
            decimal commisiinget = Convert.ToDecimal(finalcommissiondata);
            decimal totalEarning = (((TotalAmmount) - commisiinget) - (ShippingAmmount??0));

            return totalEarning;
        }
        public List<Orders> GetAllOrdersList(string filterPeriod)
        {
            if (filterPeriod == "15")
            {
                return _repoOrders.Query().Include(x => x.User).Filter(x=>x.User.IsActive==true).Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-15)).ToList();
            }
            else if (filterPeriod == "30")
            {
                return _repoOrders.Query().Include(x => x.User).Filter(x => x.User.IsActive == true).Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-30)).ToList();
            }
            else
            {
                return _repoOrders.Query().Include(x => x.User).Filter(x => x.User.IsActive == true).Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-7)).ToList();
            }
        }
        public decimal GetOrdersTotalList(string filterPeriod)
        {
            //return _repoOrders.Query().Get().Select(x=>x.Amount).Sum();
            if (filterPeriod == "15")
            {
                return _repoOrders.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-15)).Select(x => x.Amount).Sum();
            }
            else if (filterPeriod == "30")
            {
                return _repoOrders.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-30)).Select(x => x.Amount).Sum();
            }
            else
            {
                return _repoOrders.Query().Get().Where(x => x.CreatedAt > DateTime.Now.AddDays(-7)).Select(x => x.Amount).Sum();
            }

        }
        public List<Orders> GetOrdersList(DateTime S_date, DateTime E_date)
        {
            var dd= _repoOrders.Query().Get().Where(x=>x.CreatedAt.Value.Date >= S_date.Date && x.CreatedAt.Value.Date <= E_date.Date && x.Status == "completed").ToList();
            return dd;
        }
        public List<Orders> GetOrdersVendorList(DateTime S_date, DateTime E_date, int id)
        {
            var dd= _repoOrders.Query().Filter(x=>x.VendorId==id).Include(x => x.OrderItems).Include(x => x.ReturnRequests).Get().Where(x=>x.CreatedAt.Value.Date >= S_date.Date && x.CreatedAt.Value.Date <= E_date.Date && x.Status == "completed").ToList();
            return dd;
        }
        
        public List<Orders> GetProcessingList()
        {
            return _repoOrders.Query().Get().Where(x=>x.Status == "processing").ToList();
        }
        public List<Orders> GetProcessingList(DateTime? S_date, DateTime? E_date)
        {
            return _repoOrders.Query().Get().Where(x => x.CreatedAt >= S_date && x.CreatedAt.Value.Date <= E_date && x.Status == "processing").ToList();
        }
        public List<Orders> GetVendorProcessingOrdersList(DateTime? S_date, DateTime? E_date, int id)
        {
            var processingdata = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt >= S_date && x.CreatedAt.Value <= E_date && x.Status == "processing").ToList();
            return processingdata;
        }
        public List<Orders> GetOrdersreturnedList()
        {
            return _repoOrders.Query().Get().Where(x => x.Status == "returned").ToList();
        }
        public List<Orders> GetVendorOrdersList(DateTime? S_date, DateTime? E_date, int id)
        {
            //var completeddata = _repoOrders.Query().Get().Where(x => x.UserId == id && x.CreatedAt.Value >= S_date && x.UpdatedAt.Value <= E_date && x.Status == "completed").ToList();
            //return completeddata;
            var completeddata = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Value.Date >= S_date && x.CreatedAt.Value.Date <= E_date && x.Status == "completed").ToList();
            return completeddata;
        }
        public List<Orders> GetReturnVendorOrdersList(DateTime? S_date, DateTime? E_date, int id)
        {
            //var completeddata = _repoOrders.Query().Get().Where(x => x.UserId == id && x.CreatedAt.Value >= S_date && x.UpdatedAt.Value <= E_date && x.Status == "completed").ToList();
            //return completeddata;
            var completeddata = _repoOrders.Query().Get().Where(x => x.VendorId == id && x.CreatedAt.Value.Date >= S_date && x.CreatedAt.Value.Date <= E_date && x.Status == "returned").ToList();
            return completeddata;
        }
        public List<Orders> GetOrdersreturnedList(DateTime? S_date, DateTime? E_date)
        {
            return _repoOrders.Query().Get().Where(x => x.CreatedAt.Value.Date >= S_date && x.CreatedAt.Value.Date <= E_date && x.Status == "returned").ToList();
        }
        public List<Orders> GetOrderscancelledList()
        {
            return _repoOrders.Query().Get().Where(x => x.Status == "cancelled").ToList();
        }
        public List<Orders> GetOrderscancelledList(DateTime? S_date, DateTime? E_date)
        {
            return _repoOrders.Query().Get().Where(x => x.CreatedAt >= S_date && x.CreatedAt.Value.Date <= E_date && x.Status == "cancelled").ToList();
        }
        public Orders Update(Orders entity)
        {
            _repoOrders.Update(entity);
            return entity;
        }
        public OrderItems GetByItemsId(int id)
        {
            return _repoOrderItems.Query().Filter(x => x.ProductId == id).Include(p=>p.Product).Get().FirstOrDefault();
        }
        public List<OrderItems> GetByItemsOrderId(int id)
        {
            return _repoOrderItems.Query().Filter(x => x.OrderId == id).Include(p=>p.Product).Get().ToList();
        }
        public OrderItems GetByItemsorderId(int id)
        {
            return _repoOrderItems.Query().Filter(x => x.OrderId == id).Include(p=>p.Product).Get().FirstOrDefault();
        }
        public Orders GetByorderIdBaseproduct(int userid,int id)
        {
            return _repoOrders.Query().Filter(x => x.OrderId == id.ToString() && x.UserId==userid).Include(p=>p.OrderItems).Get().FirstOrDefault();
        }
        public OrderItems Saveitem(OrderItems entity)
        {
            _repoOrderItems.Insert(entity);
            return entity;
        }
        public bool Delete(int id)
        {
            Orders items = _repoOrders.FindById(id);
            _repoOrderItems.Delete(items);
            return true;
        }
        public PageList<Orders> GetOrderByUserId(int userId, ToDoSearchSpecs specs)
        {
            var orderList = _repoOrders.Query()
                .Filter(x => x.UserId == userId)
                .Include(x=> x.OrderItems)
                .Get().OrderByDescending(x => x.Id)
                .ToList();
            //if (!string.IsNullOrWhiteSpace(specs.Search))
            //{
            //    orderList = orderList.Where(t => t.UserId.ToString().ToLower().Contains(specs.Search.ToString().ToLower())).ToList();
            //}
            return PageList<Orders>.ToPageList(orderList, specs.page, specs.PageSize);
            //orderList = SortHelper<Orders>.ApplySort(orderList, specs.OrderBy).ToList();
            //return PageList<Orders>.ToPageList(orderList, specs.PageNumber, specs.PageSize);
        }
        public OrderItems GetByproduts(int order_id, int product_id)
        {
            return _repoOrderItems.Query().Filter(x => x.OrderId == Convert.ToInt16(order_id) && x.ProductId == product_id).Include(x => x.Product).Get().FirstOrDefault();
        }
        public Orders GetByItemId(string order_id, int userId)
        {
            return _repoOrders.Query().Filter(x => x.OrderId == (order_id) && x.UserId == userId).Include(x => x.OrderItems).Include(x => x.User).Get().FirstOrDefault();
        }
        public Orders GetByItemId_invoice(int order_id)
        {
            return _repoOrders.Query().Filter(x => x.Id == order_id).Include(x => x.OrderItems).Include(x => x.User).Get().FirstOrDefault();
        }

        public PageList<Orders> GetAllOrderForVendor(ToDoSearchSpecs specs, int? userId, string createdAt)
        {
            var predicate = PredicateBuilderExt.True<Orders>();
            predicate = predicate.And(e => e.VendorId == userId);
            if (!string.IsNullOrEmpty(createdAt))
            {
                predicate = predicate.And(e => e.CreatedAt.Value.Date == Convert.ToDateTime(createdAt).Date);
            }

            var orderList = _repoOrders
                .Query()
                .Filter(predicate)
                .Include(x => x.OrderItems)
                .Include(x => x.User)
                .Get()
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            return PageList<Orders>.ToPageList(orderList, specs.page, specs.PageSize);
            //orderList = SortHelper<Orders>.ApplySort(orderList, specs.OrderBy).ToList();
            //return PageList<Orders>.ToPageList(orderList, specs.PageNumber, specs.PageSize);
        }

        public void Dispose()
        {
            if (_repoOrders != null)
            {
                _repoOrders.Dispose();
            }
        }
    }
}
