using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Service;
using EC.Service.Shippings;
using System.Linq;
using EC.Web.Areas.Admin.ViewModels;
using EC.Core.Enums;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;
using EC.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Auth0.ManagementApi.Models.AttackProtection;

namespace EC.Web.Areas.Admin.Controllers
{
    public class ShippingController : BaseController
    {
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        public ShippingController(IShippingService shippingService, ICountryService countryService, IStateService stateService)
        {
            _shippingService = shippingService;
            _countryService = countryService;
            _stateService = stateService;
        }
        #region [Index]
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
            var query = new SearchQuery<ShippingCharges>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.MinimumOrderAmount.ToString().Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<ShippingCharges, decimal>(q => q.MinimumOrderAmount, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<ShippingCharges, decimal>(q => q.MaximumOrderAmount, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<ShippingCharges, decimal?>(q => q.ShippingCharge, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<ShippingCharges, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Descending : SortDirection.Ascending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<ShippingCharges> entities = _shippingService.GetShippingchargesByPage(query, out total).Entities;

            foreach (ShippingCharges entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.MinimumOrderAmount.ToString(),
                    entity.MaximumOrderAmount.ToString(),
                    entity.ShippingCharge.ToString(),
                    entity.Status.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [Create]
        [HttpGet]
        public IActionResult Create(int? id)
        {
            var model = new ShippingViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            if (id.HasValue)
            {
                ShippingCharges entity = _shippingService.GetById(id.Value);
                model.Id = entity.Id;
                model.ShippingCharge = entity.ShippingCharge.ToString();
                model.MaximumOrderAmount= entity.MaximumOrderAmount.ToString();
                model.MinimumOrderAmount=entity.MinimumOrderAmount.ToString();
                model.Status = entity.Status;
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(int? id, ShippingViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool range = _shippingService.GetShippingratesrangeList(Convert.ToDecimal(model.MinimumOrderAmount),Convert.ToDecimal(model.MaximumOrderAmount));
                    if (!range)
                    {
                        ShowErrorMessage("Error", "This range already exist, please try another range.", false);
                        return RedirectToAction("Create");
                    }
                    bool isExist = id.HasValue && id.Value != 0;
                    var entity = isExist ? _shippingService.GetById(model.Id) : new ShippingCharges();
                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.MinimumOrderAmount = Convert.ToDecimal(model.MinimumOrderAmount);
                    entity.MaximumOrderAmount = Convert.ToDecimal(model.MaximumOrderAmount);
                    entity.ShippingCharge = Convert.ToDecimal(model.ShippingCharge);
                    entity.Status = model.Status;
                    entity.UpdatedAt = model.UpdatedAt;
                    entity = isExist ? _shippingService.UpdateShippincharge(entity) : _shippingService.SaveShippincharge(entity);
                        ShowSuccessMessage("Success!", $"Shipping {(isExist ? "updated" : "created")} successfully", false);
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

        #region[View]
        [HttpGet]
        public IActionResult View(int? id)
        {
            ShippingViewModel model = new ShippingViewModel();
            if (id.HasValue)
            {
                ShippingCharges shippincharge = _shippingService.GetById(id.Value);
                if (shippincharge == null)
                {
                    return Redirect404();
                }
               model.MinimumOrderAmount=shippincharge.MinimumOrderAmount.ToString();
               model.MaximumOrderAmount=shippincharge.MaximumOrderAmount.ToString();
               model.ShippingCharge = shippincharge.ShippingCharge.ToString();
               model.Status = shippincharge.Status;
            }
            return View(model);
        }

        #endregion

        #region [ STATUS ]

        /// <summary>
        /// Update Categories Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveshippingStatus(int id)
        {
            ShippingCharges entity = _shippingService.GetById(id);
            entity.Status = !entity.Status;
            _shippingService.UpdateShippincharge(entity);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
        }

        #endregion [ STATUS ]

        #region [ DELETE ]
        /// <summary>
        /// Show Confirmation Box For Delete Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Delete Confirmation Box </returns>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure you want to delete this shipping information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete shipping Information" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        /// <summary>
        /// Delete Record From DB(IsDeleted)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FC"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult Delete(int id, IFormCollection FC)
        {
            try
            {
                bool isDeleted = _shippingService.Delete((id));
                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Shipping Information deleted successfully.", false);
                }
                else
                {
                    ShowErrorMessage("Error!", "Error occurred, Please try again.", false);
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ShowErrorMessage("Error!", message, false);
            }
            return RedirectToAction("Index", "Shipping");
        }
        #endregion [ DELETE ]
    }
}
