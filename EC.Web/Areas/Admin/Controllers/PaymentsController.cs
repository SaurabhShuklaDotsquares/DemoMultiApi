using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service.Payments;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using EC.Web.Areas.Admin.ViewModels;
using EC.Core;
using EC.Service;

namespace EC.Web.Areas.Admin.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly IPaymentsService _paymentsService;
        private IOrdersService _ordersService;
        public PaymentsController(IPaymentsService paymentsService, IOrdersService ordersService)
        {
            _paymentsService = paymentsService;
            _ordersService = ordersService;
        }
        #region [Index]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Payment>();
            query.IncludeProperties = "Order";
            
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Order.OrderId.Contains(sSearch) || q.TransactionId.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            //query.IncludeProperties = "Orders";
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, string>(q => q.Order.OrderId, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, decimal ?>(q => q.Amount, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, string>(q => q.PaymentStatus, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, string>(q => q.TransactionId, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 8:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, DateTime?>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Payment, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Payment> entities = _paymentsService.GetByPayments(query, out total).Entities;
            var PaymentLists = (from pro in entities
                                   select new PaymentsViewModels
                                   {
                                       Id = pro.Id,
                                       OrderId = pro.Order.OrderId.ToString(),
                                       Amount = pro.Amount + pro.Order.ShippingAmount,
                                       TransactionId = pro.TransactionId,
                                       PaymentStatus = pro.PaymentStatus,
                                       Status = pro.Status,
                                       CreatedAt = pro.CreatedAt,
                                       UpdatedAt = pro.UpdatedAt

                                   }).ToList();
            //var orderDetail = _ordersService.GetByIdorderid(pro.);


            foreach (var entity in PaymentLists)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.OrderId.ToString(),
                    entity.Amount.ToString(),
                    entity.TransactionId.ToString(),
                    entity.PaymentStatus.ToString(),
                    entity.Status.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString()
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

      #endregion

        #region [View]

        [HttpGet]
        public IActionResult View(int? id)
        {
            PaymentsViewModels model1 = new PaymentsViewModels();
            if (id.HasValue)
            {
                var data = _paymentsService.GetByPaymentsId(id.Value);
                if (data == null)
                {
                    return Redirect404();
                }
                model1.Name = data.Order.Firstname +' '+ data.Order.Lastname;
                model1.Mobile = data.Order.Mobile;
                model1.Email = data.Order.Email != string.Empty && data.Order.Email != null ? data.Order.Email : data.User.Email;
                model1.OrderId = data.Order.OrderId;
                model1.TransactionId = data.TransactionId;
                model1.PaymentMethod = data.Order.PaymentMethod;
                model1.PaymentStatus = data.PaymentStatus;
                model1.CurrencyCode = data.CurrencyCode;
                model1.Amount = data.Amount;
            }

            return View(model1);
        }
         #endregion

        #region [ STATUS ]

        /// <summary>
        /// Update Categories Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActivePaymentStatus(int id)
        {
            Payment entity = _paymentsService.GetByPaymentsId(id);
            entity.Status = !entity.Status;
            _paymentsService.UpdatePayment(entity);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
        }
        #endregion

    }
}
