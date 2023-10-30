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
using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.Controllers
{
    [CustomAuthorization(RoleType.Administrator)]
    public class BrandsController : BaseController
    {
        #region [ Service Injection ]
        /// <summary>
        /// Inject Required Services
        /// </summary>
        /// 
        private readonly IBrandsService _brandsService;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public BrandsController(IBrandsService brandsService, IWebHostEnvironment hostEnvironment, IProductService productService)
        {
            _brandsService = brandsService;
            webHostEnvironment = hostEnvironment;
            _productService = productService;
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
        /// Get & Set Brands record into DataTable With Pagination
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>return Brands record DataTable With Pagination</returns>
        [HttpPost]
        public ActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();
            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Brands>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch));
            }
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Brands, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Brands, string>(q => q.Slug, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Brands, bool>(q => q.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Brands, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Brands, DateTime?>(q => q.CreatedAt,SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Brands> entities = _brandsService.Get(query, out total).Entities;
            foreach (Brands entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title,
                    entity.Slug,
                    entity.Image,
                    entity.CreatedAt.ToString(),
                    entity.Status.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion [ INDEX ]

        #region [ ADD / EDIT ]
        /// <summary>
        /// Get & Set Value into BrandsViewModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            var model = new BrandsViewModel();
            if (id.HasValue)
            {
                Brands entity = _brandsService.GetById(id.Value);
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.Status = entity.Status;
                model.IsFeatured = entity.IsFeatured;
                model.Image = entity.Image;
            }
            return PartialView("_CreateEdit", model);
        }

        /// <summary>
        /// Insert or Update BrandsViewModel Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult CreateEdit(int? id, BrandsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    bool isExist = id.HasValue && id.Value != 0;
                    var brand = _brandsService.GetByTitle(model.Title.Trim());
                    if (brand != null && !isExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "The title has been already taken.", IsSuccess = false });
                    }
                    var entity = isExist ? _brandsService.GetById(model.Id) : new Brands();
                    string uniqueFileName = ProcessUploadedFile(model);
                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.Title = model.Title;
                    entity.Slug = model.Title.ToLower();
                    entity.Status = model.Status;
                    entity.IsFeatured = model.IsFeatured;
                    entity.Image = model.BrandPicture != null ? uniqueFileName : model.Image;
                    entity = isExist ? _brandsService.Update(entity) : _brandsService.Save(entity);
                    ShowSuccessMessage("Success!", $"Brand {(isExist ? "updated" : "created")} successfully", false);
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("Index");
        }
        private string ProcessUploadedFile(BrandsViewModel model)
        {
            string uniqueFileName = null;

            if (model.BrandPicture != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BrandPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.BrandPicture.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        public IActionResult View(int? id)
        {
            BrandsViewModel model = new BrandsViewModel();
            if (id.HasValue)
            {
                Brands brands = _brandsService.GetById(id.Value);
                if (brands == null)
                {
                    return Redirect404();
                }
                model.Title = brands.Title;
                model.Slug = brands.Slug;
                model.CreatedAt = brands.CreatedAt;
                model.UpdatedAt = brands.UpdatedAt;
                model.IsFeatured = brands.IsFeatured;
                model.Status = brands.Status;
                model.Image = brands.Image;
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
                Message = "Are you sure you want to delete this Brand information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Brand Information" },
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
                bool isDeleted = _brandsService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Brand Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Brands");
        }
        #endregion [ DELETE ]

        #region [ STATUS ]

        /// <summary>
        /// Update Brand Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveBrandStatus(int id)
        {
            var product = _productService.GetByBrandId(id);
            if (product != null && product.Any())
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Cannot update status, there are some products listed for this brand.", IsSuccess = false });
            }
            else
            {
                Brands entity = _brandsService.GetById(id);
                if (entity != null)
                {
                    entity.Status = !entity.Status;
                    _brandsService.Update(entity);
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
