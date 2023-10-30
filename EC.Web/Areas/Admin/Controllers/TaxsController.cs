using EC.Service.Product;
using EC.Service.ReturnRequest;
using EC.Service;
using Microsoft.AspNetCore.Mvc;
using EC.Service.Taxs;
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
using Newtonsoft.Json;

namespace EC.Web.Areas.Admin.Controllers
{
    public class TaxsController : BaseController
    {
        #region Constructor
        private readonly ITaxService _TaxService;
        private readonly ICategoryService _categoryService;
        public TaxsController(ITaxService TaxService, ICategoryService categoryService)
        {
            _TaxService = TaxService;
            _categoryService = categoryService;
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
            var query = new SearchQuery<Tax>();
            query.IncludeProperties = "Category";
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Tax, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Tax, string>(q => q.Category.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Tax, decimal?>(q => q.Value, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Tax, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Tax, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Tax> entities = _TaxService.GetTaxByPage(query, out total).Entities;
            foreach (Tax entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),                    
                    entity.Title.ToString(),
                    entity.Category.Title.ToString(),
                    entity.Value.ToString(),
                    entity.Status.ToString(),
                    entity.CreatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [Add/Edit]

        [HttpGet]
        public IActionResult Create(int? id)
        {
            var model = new TaxsViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            var categoriesList = _categoryService.GetCategoriesList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            List<SelectListItem> subcategoryListItems = new List<SelectListItem>();
            var parentTitleList = categoriesList.Where(x => x.ParentId == null).Distinct().ToList();
            foreach (var parentTitle in parentTitleList)
            {
                SelectListItem selectItem = new SelectListItem();
                selectItem.Value = parentTitle.Id.ToString();
                selectItem.Text =  parentTitle.Title.ToString();    
                selectListItems.Add(selectItem);
            }
            model.TitleList = selectListItems;
            if (id.HasValue)
            {
                Tax entity = _TaxService.GetById(id.Value);
                model.Id = entity.Id;
                model.CategoryId = entity.CategoryId;
                model.Title = entity.Title;
                model.Value = entity.Value;
                model.Status = entity.Status;
                model.SubCategoryId = entity.SubCategoryId != null ? entity.SubCategoryId.Split(',').Select(x => Convert.ToInt32(x)).ToArray() : null;

                var subCateGory = _categoryService.GetSubCategory(entity.CategoryId);
                if (subCateGory != null && subCateGory.Any())
                {
                    foreach (var item in subCateGory)
                    {
                        SelectListItem selectItem = new SelectListItem();
                        selectItem.Value = item.Id.ToString();
                        selectItem.Text = item.Title.ToString();
                        //selectItem.Selected = true; 
                        subcategoryListItems.Add(selectItem);
                    }
                }
            }
            model.SubCategoryList = subcategoryListItems;

            return PartialView("_Create", model);
        }
        [HttpPost]
        public IActionResult Create(int? id,TaxsViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;
                    bool subcategoryExist = CheckSubCategoryExist(model.CategoryId, model.SubCategoryId != null ? model.SubCategoryId : null);
                    if (subcategoryExist && !isExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = false, Message= "Tax for this category already exist." });
                    }
                    //var tax = _TaxService.GetTaxByCategoryId(model.CategoryId);
                    //if (tax != null)
                    //{
                    //    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = false, Message = "The title has been already taken." });
                    //}

                    var entity = isExist ? _TaxService.GetById(Convert.ToInt16(model.Id)) : new Tax();
                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.UpdatedAt = model.UpdatedAt;
                    entity.CategoryId = model.CategoryId;
                    entity.SubCategoryId = model.SubCategoryId != null ? string.Join(',', model.SubCategoryId) : null;
                    entity.Title = model.Title;
                    entity.Value = model.Value;
                    entity.Status = model.Status;
                   
                    entity = isExist ? _TaxService.UpdateTax(entity) : _TaxService.SaveTax(entity);
                    ShowSuccessMessage("Success!", $"Tax {(isExist ? "updated" : "created")} successfully", false);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
                }
                else
                {
                    return PartialView("_Create", model);
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
        public IActionResult ActiveTaxStatus(int id)
        {
            Tax entity = _TaxService.GetById(id);
            entity.Status = !entity.Status;
            _TaxService.UpdateTax(entity);
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
                Message = "Are you sure you want to delete this tax information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete  tax Information" },
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
                bool isDeleted = _TaxService.Delete((id));
                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", " tax Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Taxs");
        }
        #endregion [ DELETE ]

        #region [View]

        [HttpGet]
        public IActionResult View(int? id)
        {
            TaxsViewModels model = new TaxsViewModels();
            if (id.HasValue)
            {
                Tax categories = _TaxService.GetById(id.Value);
                if (categories == null)
                {
                    return Redirect404();
                }
                model.Title = categories.Title;
                model.ParentName = _categoryService.GetNameById(categories.CategoryId);
                model.SubCategoryName = categories.SubCategoryId != null ? string.Join(',', _categoryService.GetSubCategoryNameById(categories.SubCategoryId.Split(','))) : string.Empty;
                model.CreatedAt = categories.CreatedAt;
                model.Value = categories.Value;
                model.Status = categories.Status;
            }
            return View(model);
        }
        #endregion

        #region Get SubCategoryList
        [HttpPost]
        public IActionResult GetSubcategory(int CategoryId)
        {
            List<SelectListItem> subcategoryList = new List<SelectListItem>();
            var subCateGory = _categoryService.GetSubCategory(CategoryId);
            if(subCateGory != null && subCateGory.Any())
            {
                foreach (var item in subCateGory)
                {
                    SelectListItem selectItem = new SelectListItem();
                    selectItem.Value = item.Id.ToString();
                    selectItem.Text = item.Title.ToString();
                    subcategoryList.Add(selectItem);
                }
            }
            return Json(subcategoryList);
        }
        #endregion

        #region Check SubcategoryExist
        public bool CheckSubCategoryExist(int categoryId, int[] subcategoryId)
        {
            string subCategory = string.Empty;
            if (subcategoryId != null)
            {
                subCategory = string.Join(",", subcategoryId);
            }
            else
            {
                subCategory = null;
            }
               
            bool categoryExist = false;
            var category = _TaxService.checkCategoryExist(categoryId);
            if (category != null && category.Any())
            {
                foreach (var item in category)
                {
                    if (subCategory != null && item.SubCategoryId != null)
                    {
                        categoryExist = item.SubCategoryId.Contains(subCategory);
                    }
                    else if (item.SubCategoryId == null && subCategory == null)
                    {
                        categoryExist = item.SubCategoryId == subCategory;
                    }
                }
            }
            return categoryExist;
        }
        #endregion
    }
}
