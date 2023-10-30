using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Service;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using EC.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using EC.Core.LIBS;
using EC.Service.Product;

namespace EC.Web.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {

        private readonly IOrdersService _OrdersService;
        private readonly ICountryService _CountryService;
        private readonly IProductAttributeDetailsService _productAttributeDetailsService;
        public OrderController(IOrdersService OrdersService, IProductAttributeDetailsService productAttributeDetailsService, ICountryService CountryService)
        {
            _OrdersService = OrdersService;
            _CountryService= CountryService;
            _productAttributeDetailsService = productAttributeDetailsService;

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
            var query = new SearchQuery<Orders>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.OrderId.Contains(sSearch));
            }
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Orders, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Orders, string>(q => q.OrderId, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Orders, Decimal>(q => q.Amount, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Orders, string>(q => q.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Orders, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Orders> entities = _OrdersService.Get(query, out total).Entities;
            foreach (Orders entity in entities)
            {
                var data = entity.Amount + (entity.ShippingAmount);
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.OrderId,
                    '$'+data.ToString(),
                    entity.Status.ToString(),
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
            var model = new OrdersViewModel();
            decimal totalAmount = 0;
            List<SelectListItem> OrderstatusList = new List<SelectListItem>()
            {
                new SelectListItem { Value= "processing", Text= "Processing" },
                new SelectListItem { Value = "shipped", Text = "Shipped" },
                new SelectListItem { Value= "completed", Text= "Completed" },
                new SelectListItem { Value = "pending_for_payment", Text = "Pending for Payment" },
                new SelectListItem { Value= "failed", Text= "Failed" },
                new SelectListItem { Value = "returned", Text = "Returned" },
                new SelectListItem { Value = "refunded", Text = "Refunded" },
                new SelectListItem { Value = "cancelled", Text = "Cancelled" },
                new SelectListItem { Value = "pending", Text = "Pending" }
            };
            model.OrdersStatusTypeList = OrderstatusList;

            if (id.HasValue)
            {
                Orders entity = _OrdersService.GetById(id.Value);
                model.Id = entity.Id;
                model.CreatedAt = entity.CreatedAt;
                model.Status = OrderstatusList.Where(x => x.Value == entity.Status).FirstOrDefault().Text != null ? OrderstatusList.Where(x => x.Value == entity.Status).FirstOrDefault().Text : entity.Status;
                model.Firstname = entity.Firstname + " " + entity.Lastname;
                model.Email = entity.Email != null && entity.Email != string.Empty ? entity.Email : entity.User.Email;
                model.Price = entity.Amount;
                model.OrderId = entity.OrderId;
                model.Mobile = entity.Mobile;
        
                var shiping = JsonConvert.DeserializeObject<shippingAddress>(entity.ShipingAddress);
                model.shippingAddress = new List<shippingAddress>();
                shippingAddress shippingdata = new shippingAddress();
                shippingdata.address2 = shiping.address2;
                shippingdata.state = shiping.state;
                shippingdata.postal_code = shiping.postal_code;
                shippingdata.city = shiping.city;
                var countryname = _CountryService.GetById(Convert.ToInt32(shiping.country));
                if (countryname!=null)
                {
                    shippingdata.country = countryname.Name.ToString();
                }
                model.shippingAddress.Add(shippingdata);

                if (entity.BillingAddress != null)
                {
                    var dt = JsonConvert.DeserializeObject<billingAddress>(entity.BillingAddress);
                    model.billingAddress = new List<billingAddress>();
                    billingAddress data = new billingAddress();
                    data.address = !string.IsNullOrEmpty(dt.address) ? dt.address : shiping.address2;
                    data.state = !string.IsNullOrEmpty(dt.state) ? dt.state : shiping.state;
                    data.postal_code = !string.IsNullOrEmpty(dt.postal_code) ? dt.postal_code : shiping.postal_code;
                    data.city = !string.IsNullOrEmpty(dt.city) ? dt.city : shiping.city;
                    var countrybillingname = _CountryService.GetById(Convert.ToInt32(dt.country));
                    if (countrybillingname != null)
                    {
                        data.country = countrybillingname.Name.ToString();
                    }
                    // data.country = !string.IsNullOrEmpty(dt.country) ? dt.country : shiping.country;
                    model.billingAddress.Add(data);
                }
                else
                {
                    model.billingAddress = new List<billingAddress>();
                    billingAddress data = new billingAddress();
                    data.address = shiping.address2;
                    data.state = shiping.state;
                    data.postal_code = shiping.postal_code;
                    data.city = shiping.city;
                    data.country = shiping.country;
                    model.billingAddress.Add(data);
                }

                
                model.Message = entity.Message;
                
                model.orderItems = new List<orderItems>();
                foreach (var item in entity.OrderItems)
                {
                    orderItems tempp = new orderItems();
                    tempp.ProductId = item.ProductId;
                    tempp.Quantity = item.Quantity;
                    tempp.VariantId = item.VariantId;
                    tempp.VariantSlug = item.VariantSlug;
                    tempp.Tax = item.Tax;
                    int quantity = item.Quantity != 0 ? item.Quantity : 0;
                    tempp.Price = entity.Amount / quantity;
                    tempp.Total = Convert.ToDecimal(entity.Amount);
                    totalAmount  += Convert.ToDecimal(entity.Amount);
                    OrderItems entity2 = _OrdersService.GetByItemsId(tempp.ProductId);
                    tempp.productname = entity2.Product.Title;
                    model.orderItems.Add(tempp);
                    IEnumerable<ProductAttributeDetails> productAttributeDetailsList = _productAttributeDetailsService.GetById(item.ProductId);
                    
                    foreach (var item2 in productAttributeDetailsList)
                    {
                        model.varientText = item2.VariantText;
                    }
                }
                model.SubTotal = totalAmount.ToString();
                decimal shipping = entity.ShippingAmount != 0 && entity.ShippingAmount != null ? entity.ShippingAmount.Value : 0;
                model.ShippingAmount = shipping;
                model.GrandTotal = Convert.ToString(totalAmount + shipping);
            }
                return View(model);
        }

        [HttpPost]
        public IActionResult Create(int? id, OrdersViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;
                    var entity = isExist ? _OrdersService.GetById(Convert.ToInt32(model.Id)) : new Orders();
                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.Status = model.Status;
                    entity.Message = model.Message;
                    entity = isExist ? _OrdersService.Update(entity) : _OrdersService.Save(entity);
                    ShowSuccessMessage("Success!", $"Order {(isExist ? "updated" : "created")} successfully", false);
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
