using EC.Service.Product;
using EC.Service.ReturnRequest;
using EC.Service;
using Microsoft.AspNetCore.Mvc;
using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using System.Collections.Generic;
using System;
using System.Linq;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using EC.Core;
using EC.Core.Enums;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EC.Web.Areas.Admin.Controllers
{
    public class CouponsController : BaseController
    {
        private readonly ICouponService _couponService;
        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
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
            var query = new SearchQuery<Coupons>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Code.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Coupons, string>(q => q.Code, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Coupons, string>(q => q.Type, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Coupons, int?>(q => q.MaximumValue, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Coupons, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Coupons, DateTime?>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Coupons, DateTime?>(q => q.CreatedAt,  SortDirection.Descending ));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Coupons> entities = _couponService.GetCouponsByPage(query, out total).Entities;

                foreach (Coupons entity in entities)
                {
                var value = entity.MaximumValue.ToString();
                if (entity.Type.ToLower() =="fixed")
                {
                     
                }
                else
                {
                    value = value + '%';
                }
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Code.ToString(),
                    entity.Type.ToString(),
                    value,
                    entity.IsActive.ToString(),
                    entity.CreatedAt.ToString(),
                    entity.StartDate.Value.ToShortDateString()+'-'+entity.EndDate.Value.ToShortDateString(),
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
            CouponsViewModels model = new CouponsViewModels();
            List<SelectListItem> CouponsTypeList = new List<SelectListItem>()
            {
                new SelectListItem { Value= "fixed", Text= "Fixed" },
                new SelectListItem { Value = "percentage", Text = "Percentage"},
            };
            model.CouponsTypeList = CouponsTypeList;
            if (id.HasValue)
            {
                Coupons coupons = _couponService.GetById(id.Value);
                if(coupons != null)
                {
                    model.Id = coupons.Id;
                    model.CreatedAt = coupons.CreatedAt;
                    model.Code = coupons.Code;
                    model.MaximumValue = coupons.MaximumValue.ToString();
                    model.Type = coupons.Type;
                    model.Amount = coupons.Amount;
                    model.UpdatedAt = coupons.UpdatedAt;
                    model.MaximumUsageValue = coupons.MaximumUsage;
                    model.StartDate = coupons.StartDate.Value.ToString("dd/MM/yyyy");
                    model.EndDate = coupons.EndDate.Value.ToString("dd/MM/yyyy");
                }
            }
                return View(model);
        }

        [HttpPost]
        public IActionResult Create(int? id, CouponsViewModels model)
        {
            try
            {
                if (model.Amount == null || model.MaximumUsageValue == null)
                {
                    ModelState.Remove("Amount");
                    ModelState.Remove("MaximumUsageValue");
                }
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;
                    var entity = isExist ? _couponService.GetById(Convert.ToInt32(model.Id)) : new Coupons();
                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.Code = model.Code;
                    entity.Slug = model.Type;
                    entity.MaximumValue = Convert.ToInt32(model.MaximumValue);
                    //entity.MaximumValue = Convert.ToInt32(model.MaximumValue);
                    entity.Type = model.Type;
                    entity.StartDate = Convert.ToDateTime(model.StartDate);
                    entity.EndDate = Convert.ToDateTime(model.EndDate);
                    entity.Amount = model.Amount != null ? model.Amount : 0;
                    entity.MaximumUsage = model.MaximumUsageValue != null ? model.MaximumUsageValue : 0;
                    entity.IsActive = true;
                    entity = isExist ? _couponService.UpdateCoupons(entity) : _couponService.SaveCoupons(entity);
                    ShowSuccessMessage("Success!", $"Coupon {(isExist ? "updated" : "created")} successfully", false);
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

        #region [ STATUS ]

        /// <summary>
        /// Update Categories Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveCouponStatus(int id)
        {
            Coupons entity = _couponService.GetById(id);
            entity.IsActive = !entity.IsActive;
            _couponService.UpdateCoupons(entity);
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
                Message = "Are you sure you want to delete this coupon information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete coupon Information" },
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
                bool isDeleted = _couponService.Delete((id));
                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Coupon Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Coupons");
        }
        #endregion [ DELETE ]

        #region [View]
        [HttpGet]
        public IActionResult View(int? id)
        {
            CouponsViewModels model = new CouponsViewModels();
            if (id.HasValue)
            {
                Coupons coupon = _couponService.GetById(id.Value);
                if (coupon == null)
                {
                    return Redirect404();
                }
                model.Code = coupon.Code;
                model.Type = coupon.Type;
                if (coupon.Type=="fixed")
                {
                    model.MaximumValue1 = coupon.MaximumValue.ToString();
                }
                else
                {
                    model.MaximumValue1 = coupon.MaximumValue.ToString() +' '+ '%';
                }
                model.MaximumValue = coupon.Amount.ToString();
                model.MaximumUsage = coupon.MaximumUsage;
                model.DateLimits = "Valid From"+" " + coupon.StartDate.Value.ToShortDateString() + " to " + coupon.EndDate.Value.ToShortDateString();
            }
            return View(model);
        }
        #endregion

    }
}
