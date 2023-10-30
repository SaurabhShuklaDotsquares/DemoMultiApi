using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Service.Currency;
using EC.Service.Currency_data;
using EC.Service.ReturnRequest;
using System.Linq;
using RestSharp;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using EC.Service;
using EC.Data.Entities;
using EC.Service.Product;
using EC.Core.Enums;
using EC.Core;
using Stripe;
using EC.Core.LIBS;
using System.IO;
using System.Globalization;

namespace EC.Web.Areas.Admin.Controllers
{
    public class ReturnRequestController : BaseController
    {
        #region Constructor
        private readonly IReturnRequestService _ReturnRequestService;
        private readonly IReturnItemsService _ReturnItemsService;
        private readonly IOrdersService _OrdersService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        private readonly IEmailsTemplateService _emailsTemplateService;
        private readonly ITemplateEmailService _emailService;
        public ReturnRequestController(IReturnRequestService ReturnRequestService, IOrdersService OrdersService, IProductAttributeDetailsService productAttributeDetailsService, IReturnItemsService ReturnItemsService, IEmailsTemplateService emailsTemplateService, ITemplateEmailService emailService)
        {
            _ReturnRequestService = ReturnRequestService;
            _OrdersService = OrdersService;
            _productAttributeDetailsService = productAttributeDetailsService;
            _ReturnItemsService = ReturnItemsService;
            _emailsTemplateService = emailsTemplateService;
            _emailService = emailService;   
        }
        #endregion

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
            var query = new SearchQuery<ReturnRequests>();
            query.IncludeProperties = "Order,User,ReturnItems";
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower();
                query.AddFilter(q => q.Order.OrderId.Contains(sSearch) || (q.User.Firstname +' '+ q.User.Lastname).Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, DateTime?>(q => q.Order.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, string>(q => q.Order.OrderId, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, string>(q =>q.User.Firstname + q.User.Lastname, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, string>(q =>q.Order.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 8:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, DateTime?>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<ReturnRequests, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<ReturnRequests> entities = _ReturnRequestService.GetReturnRequestsByPage(query, out total).Entities;
            foreach (ReturnRequests entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Order.CreatedAt.ToString(),
                    entity.Order.OrderId.ToString(),
                    entity.User != null ? entity.User.Firstname +' '+ entity.User.Lastname : string.Empty,
                    entity.Status.ToString(),
                    entity.Order.PaymentMethod.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [create]

        [HttpGet]
        public IActionResult Create(int? id)
        {
            var model = new ReturnRequestsViewModels();
            List<SelectListItem> ReturnStatusList = new List<SelectListItem>()
            {
                new SelectListItem { Value= "Processing", Text= "Processing" },
                new SelectListItem { Value = "Shipped", Text = "Shipped" },
                new SelectListItem { Value= "Completed", Text= "Completed" },
                new SelectListItem { Value = "Pending_for_payment", Text = "Pending for Payment" },
                new SelectListItem { Value= "Failed", Text= "Failed" },
                new SelectListItem { Value = "Returned", Text = "Returned" },
                new SelectListItem { Value = "Refunded", Text = "Refunded" },
                new SelectListItem { Value = "Cancelled", Text = "Cancelled" },
                new SelectListItem { Value = "Pending", Text = "Pending" },
                new SelectListItem { Value = "New", Text = "New" },
                new SelectListItem { Value = "Accepted", Text = "Accepted" },
                new SelectListItem { Value = "Declined", Text = "Declined" }
            };
            model.ReturnStatusTypeList = ReturnStatusList;

            if (id.HasValue)
            {
                ReturnRequests entity = _ReturnRequestService.GetById(id.Value);
                model.Id = entity.Id;
                model.CreatedAt = entity.CreatedAt?.ToString("dd-MMM-yyyy");
                model.Status = entity.Status;
                model.CustomerName = entity.User.Firstname + entity.User.Lastname;
                model.Email = entity.User.Email;
                model.Price = Convert.ToDecimal(entity.Amount);
                model.OrderId = entity.Order.OrderId;
                model.orderItem = new List<orderItem>();
                var returnItems = _ReturnItemsService.GetByRequestId(id.Value);
                if(returnItems != null && returnItems.Any())
                {
                    foreach (var item in returnItems)
                    {
                        orderItem tempp = new orderItem();
                        tempp.ProductId = item.OrderItem.ProductId;
                        tempp.Quantity = item.OrderItem.Quantity;
                        tempp.VariantId = item.OrderItem.VariantId;
                        tempp.VariantSlug = item.OrderItem.VariantSlug;
                        tempp.Tax = item.OrderItem.Tax;
                        tempp.Price = Convert.ToDecimal(entity.Order.Amount);
                        model.Total = Convert.ToDecimal(entity.Order.Amount) * item.OrderItem.Quantity;
                        var entity2 = _OrdersService.GetByItemsId(tempp.ProductId);
                        tempp.productname = entity2.Product.Title;
                        model.orderItem.Add(tempp);
                        IEnumerable<Data.Models.ProductAttributeDetails> productAttributeDetailsList = _productAttributeDetailsService.GetById(item.OrderItem.ProductId);
                        foreach (var item2 in productAttributeDetailsList)
                        {
                            model.varientText = item2.VariantText;
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(int? id, ReturnRequestsViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string Message = string.Empty, refundPaymentTransactionId = string.Empty;
                    bool isExist = id.HasValue && id.Value != 0;
                    bool isRefunded = true;
                    var entityReturnRequest = isExist ? _ReturnRequestService.GetById(Convert.ToInt32(model.Id)) : new ReturnRequests();

                    if (entityReturnRequest.Order != null && entityReturnRequest.Order.TransactionId == string.Empty)
                    {
                        Message = "Transaction Id Is Not Valid.";
                        ShowErrorMessage("Error!", Message, false);
                        return RedirectToAction("Index");
                    }

                    if (entityReturnRequest != null)
                    {
                        entityReturnRequest.CreatedAt = isExist ? entityReturnRequest.CreatedAt : DateTime.Now;
                        entityReturnRequest.UpdatedAt = DateTime.Now;
                        isRefunded = entityReturnRequest.Status == ReturnOrderEnum.Refunded.GetDescription();
                    }

                    if(entityReturnRequest.ReturnItems != null && entityReturnRequest.ReturnItems.Any())
                    {
                        foreach (var item in entityReturnRequest.ReturnItems)
                        {
                            item.CreatedAt = isExist ? item.CreatedAt : DateTime.Now;
                            item.UpdatedAt = DateTime.Now;
                            entityReturnRequest.ReturnItems.Add(item);
                        }
                    }
                    #region Refund Payment Process
                    if (model.Status == ReturnOrderEnum.Accepcted.GetDescription() && isExist && !isRefunded)
                    {
                        StripeConfiguration.ApiKey = SiteKeys.StripeKey;

                        var options = new RefundCreateOptions
                        {
                            Charge = entityReturnRequest.Order.TransactionId,
                            Amount = (long)Convert.ToDouble(entityReturnRequest.RefundAmount) * 100
                        };
                        var service = new RefundService();
                        var refundPayment = service.Create(options);
                        if (refundPayment.Status == ReturnOrderEnum.Succeeded.GetDescription().ToLower())
                        {
                            entityReturnRequest.Status = ReturnOrderEnum.Refunded.GetDescription();
                            entityReturnRequest.TransactionId = refundPayment.Id;
                            refundPaymentTransactionId = refundPayment.Id;
                            if (entityReturnRequest.ReturnItems != null && entityReturnRequest.ReturnItems.Any())
                            {
                                foreach (var item in entityReturnRequest.ReturnItems)
                                {
                                    item.ReturnStatus = ReturnOrderEnum.Refunded.GetDescription();
                                    entityReturnRequest.ReturnItems.Add(item);
                                }
                            }
                        }
                        Message = "Payment Refunded Successfully.";
                    }
                    else
                    {
                        Message = $"Status {(isExist ? "Updated" : "Created")} Successfully.";
                    }
                    #endregion

                    #region Refund Payment Email Send
                    if (!isRefunded && !string.IsNullOrEmpty(refundPaymentTransactionId))
                    {
                        if (entityReturnRequest != null && entityReturnRequest.User != null)
                        {
                            var emailTemplate = _emailService.GetById((int)EmailType.Refunded);
                            if(emailTemplate != null)
                            {
                                string messageBody = emailTemplate.Description;
                                string Email = entityReturnRequest.User.Email;
                                string Subject = "Regarding Refund";
                                messageBody = messageBody.Replace("##FirstName##", entityReturnRequest.User.Firstname).Replace("##LastName##", entityReturnRequest.User.Lastname).Replace("##Email##", Email).Replace("##Amount##", entityReturnRequest.RefundAmount);
                                var response = _emailsTemplateService.SendEmailAsync(Email, Subject, messageBody);
                            }
                        }
                    }
                    #endregion
                    entityReturnRequest = isExist ? _ReturnRequestService.UpdateReturnRequests(entityReturnRequest) : _ReturnRequestService.SaveReturnRequests(entityReturnRequest);
                    var entityReturnItems = _ReturnItemsService.UpdateReturnItemsCollection(entityReturnRequest.ReturnItems.ToList());

                    ShowSuccessMessage("Success!", Message, false);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("Index");
        }

        #endregion

    }
}
