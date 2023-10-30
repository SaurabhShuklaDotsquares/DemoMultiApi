using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service.AllPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Data.Models;
using System.Linq;
using EC.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using EC.Web.Areas.Admin.ViewModels;
using EC.Core.Enums;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EC.Web.Areas.Admin.Controllers
{
    public class PagesController : BaseController
    {
        private readonly IPagesService _pagesService;
        public PagesController(IPagesService pagesService)
        {
            _pagesService = pagesService;
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
            var query = new SearchQuery<Pages>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Title.Contains(sSearch) || q.Slug.Contains(sSearch));
            }
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Pages, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Pages, string>(q => q.Slug, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Pages, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Pages, DateTime?>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Pages, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Pages> entities = _pagesService.GetPagesByPage(query, out total).Entities;

            foreach (Pages entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title,
                    entity.Slug,
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString(),
                    entity.Status.ToString(),

                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [Add/Edit]

        [HttpGet]
        public IActionResult CreatePages(int? id)
        {
            var model = new PagesViewModels
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            if (id.HasValue)
            {
                Pages entity = _pagesService.GetById(Convert.ToInt16(id.Value));
                model.Id = Convert.ToInt16(entity.Id);
                model.Title = entity.Title;
                model.SubTitle = entity.SubTitle;
                model.MetaTitle = entity.MetaTitle;
                model.MetaKeyword = entity.MetaKeyword;
                model.MetaDescription = entity.MetaDescription;
                model.ShortDescription = entity.ShortDescription;
                model.MetaDescription = entity.MetaDescription;
                model.Description = entity.Description;
                model.Status = Convert.ToBoolean(entity.Status);
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult CreatePages(int ? id, PagesViewModels pages)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;
                    var page = _pagesService.GetByTitle(pages.Title.Trim());
                    if (page != null && !isExist)
                    {
                        ShowErrorMessage("!Error", "The title has been already taken.", false);
                        return RedirectToAction("Index", "Pages");
                    }

                    var obj1 = _pagesService.GetById(pages.Id);
                    isExist = obj1 != null ? true : false;
                    var obj = _pagesService.GetById(pages.Id);
                    isExist = obj != null ? true : false;
                    Pages PagesInfo = isExist ? obj : new Pages();
                    PagesInfo.Title = pages.Title;
                    PagesInfo.Slug = pages.Title+'1';
                    PagesInfo.SubTitle = pages.SubTitle;
                    PagesInfo.MetaTitle = pages.MetaTitle;
                    PagesInfo.MetaTitle = pages.MetaTitle;
                    PagesInfo.MetaKeyword = pages.MetaKeyword;
                    PagesInfo.ShortDescription = pages.ShortDescription;
                    PagesInfo.MetaDescription = pages.MetaDescription;
                    PagesInfo.Description = pages.Description;
                    PagesInfo.Status = pages.Status;
                    if (isExist)
                    {
                        PagesInfo.CreatedAt = pages.CreatedAt;
                        PagesInfo.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        PagesInfo.CreatedAt = DateTime.Now;
                        PagesInfo.UpdatedAt = DateTime.Now;
                    }
                    PagesInfo = isExist ? _pagesService.UpdatePages(obj) : _pagesService.SavePages(PagesInfo);
                    ShowSuccessMessage("Success", isExist ? $"Updated Successfully " : $"Create Successfully ", false);
                    return RedirectToAction("Index", "Pages");
                }
                return RedirectToAction("Index", "Pages");
            }
            catch (Exception ex)
            {
                return View(ex);
            }
            return CreateModelStateErrors();
        }

        #endregion

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
                Message = "Are you sure you want to delete this Products information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Products Information" },
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
                bool isDeleted = _pagesService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Products Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Pages");
        }

        #endregion [ DELETE ]

        #region [View]

        [HttpGet]
        public IActionResult ViewPages(int? id)
        {
            PagesViewModels model = new PagesViewModels();
            if (id.HasValue)
            {
                var data = _pagesService.GetById(id.Value);
                if (data == null)
                {
                    return Redirect404();
                }
                model.Title = data.Title;
                model.SubTitle = data.SubTitle;
                model.MetaTitle = data.MetaTitle;
                model.Slug = data.Slug;
                model.MetaTitle = data.MetaTitle;
                model.MetaKeyword = data.MetaKeyword;
                model.ShortDescription = data.ShortDescription;
                model.MetaDescription = data.MetaDescription;
                model.Description = data.Description;
                model.Status = data.Status;
                model.CreatedAt = data.CreatedAt;
                model.UpdatedAt = data.UpdatedAt;
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
        public IActionResult ActivePagesStatus(int id)
        {
            //var IsDeleted = false;
            Pages entity = _pagesService.GetById(id);
            entity.Status = !entity.Status;
            _pagesService.UpdatePages(entity);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
        }
        #endregion
    }
}
