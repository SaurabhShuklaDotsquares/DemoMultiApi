using EC.Core;
using EC.Core.Enums;
using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using EC.Web.Areas.Admin.Code;
using EC.Web.Areas.Admin.ViewModels;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EC.Web.Areas.Admin.Controllers
{
    [CustomAuthorization(RoleType.Administrator)]
    public class CategoryController : BaseController
    {
        #region [ Service Injection ]
        /// <summary>
        /// Inject Required Services
        /// </summary>
        /// 
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CategoryController(ICategoryService categoryService, IWebHostEnvironment hostEnvironment, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
            webHostEnvironment = hostEnvironment;
        }

        #endregion [ Service Injection ]

        #region [ INDEX ]
        /// <summary>
        /// Navigate & Start From This Index View
        /// </summary>
        /// <returns>return to Index View</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get & Set Categories record into DataTable With Pagination
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>return Categories record DataTable With Pagination</returns>
        [HttpPost]
        public ActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Categories>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            query.AddFilter(q => q.IsDeleted==false);
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, int?>(q => q.ParentId, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, string>(q => q.Slug, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, bool>(q => q.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Categories> entities = _categoryService.Get(query, out total).Entities;
            
            foreach (Categories entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.ParentId != null ? _categoryService.GetNameById(entity.ParentId) + " > " + entity.Title : "N/A",
                    entity.Title,
                    entity.Slug,
                    entity.Image,
                    entity.CreatedAt.ToString(),
                    entity.Status.ToString(),
                    entity.AdminCommission != null ?  entity.AdminCommission + "%" : "0.00%"
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion [ INDEX ]

        #region [ ADD / EDIT ]
        /// <summary>
        /// Get & Set Value into CategoryViewModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            var model = new CategoryViewModel
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            var categoriesList = _categoryService.GetCategoriesList();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            var parentTitleList = categoriesList.Where(x => x.ParentId == null).Distinct();
            foreach (var parentTitle in parentTitleList)
            {
                foreach (var childTitle in categoriesList.Where(x => x.ParentId == parentTitle.Id || x.Id == parentTitle.Id).OrderBy(x => x.Id))
                {
                    SelectListItem selectItem = new SelectListItem();
                    selectItem.Value = childTitle.Id.ToString();
                    selectItem.Text = childTitle.ParentId != null ? "-" + childTitle.Title.ToString() : childTitle.Title;
                    selectListItems.Add(selectItem);
                }
            }
            model.TitleList = selectListItems;

            if (id.HasValue)
            {
                Categories entity = _categoryService.GetById(id.Value);
                model.Id = entity.Id;
                model.ParentId = entity.ParentId;
                model.Title = entity.Title;
                model.Status = entity.Status;
                model.IsFeatured = entity.IsFeatured;
                model.Image = entity.Image;
                model.hdnUploadImage = entity.Image;
                model.Commission = entity.AdminCommission != 0 && entity.AdminCommission != null ? entity.AdminCommission.Value : 0;
                //model.CategoryPicture = entity.Image;
            }
            return PartialView("_CreateEdit", model);
        }

        /// <summary>
        /// Insert or Update CategoryViewModel Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult CreateEdit(int? id, CategoryViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var category = _categoryService.GetByTitle(model.Title.Trim());
                    if (category != null && !id.HasValue)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "The title has been already taken.", IsSuccess = false });
                    }

                    bool isExist = id.HasValue && id.Value != 0;
                    var entity = isExist ? _categoryService.GetById(model.Id.Value) : new Categories();

                    string uniqueFileName = ProcessUploadedFile(model);
                    entity.CreatedAt = isExist ? entity.CreatedAt : model.CreatedAt;
                    entity.UpdatedAt = model.UpdatedAt;
                    entity.ParentId = model.ParentId;
                    //var data=_categoryService
                    entity.Title = model.Title;
                    entity.Slug = model.Title+'-'+2;
                    entity.Status = model.Status;
                    entity.IsFeatured = model.IsFeatured;
                    entity.IsDeleted = false;
                    entity.AdminCommission = model.Commission;
                    entity.Image = model.CategoryPicture != null ? uniqueFileName : model.Image;
                    entity = isExist ? _categoryService.Update(entity) : _categoryService.Save(entity);
                    ShowSuccessMessage("Success!", $"Category {(isExist ? "updated" : "created")} successfully", false);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("Index");
        }
        private string ProcessUploadedFile(CategoryViewModel model)
        {
            string uniqueFileName = null;
            if (model.CategoryPicture != null)
            {
                var profileExten = new[] { ".jpg", ".png", ".jpeg" };
                var ext = Path.GetExtension(model.CategoryPicture.FileName).ToLower();
                if (!profileExten.Contains(ext))
                {
                    ModelState.AddModelError("", $"image not valid, Please choose jpg,png,jpeg format");
                    return CreateModelStateErrors().ToString();
                }
                else
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CategoryPicture.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.CategoryPicture.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public IActionResult View(int? id)
         {
            CategoryViewModel model = new CategoryViewModel();
                if (id.HasValue)
                {
                    Categories categories = _categoryService.GetById(id.Value);
                    if(categories == null)
                    {
                        return Redirect404();
                    }
                    model.Title = categories.Title;
                    model.Slug = categories.Slug;
                    model.ParentName = categories.ParentId != null ? _categoryService.GetNameById(categories.ParentId) + " > " + categories.Title : "N/A";
                    model.CreatedAt = categories.CreatedAt;
                    model.UpdatedAt = categories.UpdatedAt;
                    model.IsFeatured = categories.IsFeatured;
                    model.Status = categories.Status;
                    model.Image = categories.Image;
                    model.Commission = categories.AdminCommission != null ? categories.AdminCommission : 0;
                }
                return PartialView("_View", model);
        }
        #endregion [ ADD / EDIT ]

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
                Message = "Are you sure you want to delete this Category information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Category Information" },
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
                bool isDeleted = _categoryService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Category Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Category");
        }
        #endregion [ DELETE ]

        #region [ STATUS ]

        /// <summary>
        /// Update Categories Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveCategoryStatus(int id)
        {
            var product = _productService.GetByCategoryId(id);
            if (product != null && product.Any())
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Cannot update status, there are some products listed for this category.", IsSuccess = false });
            }
            else
            {
                Categories entity = _categoryService.GetById(id);
                if (entity != null)
                {
                    entity.Status = !entity.Status;
                    _categoryService.Update(entity);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Some error occurred.", IsSuccess = false });
                }
            }
        }

        #endregion [ STATUS ]
    }
}
