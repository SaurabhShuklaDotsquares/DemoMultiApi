using EC.Core.Enums;
using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using EC.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using System.Linq;
using EC.Core;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;

namespace EC.Web.Areas.Admin.Controllers
{
    public class EmailController : BaseController
    {
        private readonly ITemplateEmailService _templateEmailService;
        public EmailController(ITemplateEmailService templateEmailService)
        {
            _templateEmailService = templateEmailService;
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
            var query = new SearchQuery<Emails>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Name.Contains(sSearch));
            }
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Emails, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Emails, string>(q => q.Subject, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Emails, string>(q => q.Slug, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Emails, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Emails, DateTime?>(q => q.UpdatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Emails, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Emails> entities = _templateEmailService.GetEmailsByPage(query, out total).Entities;
            foreach (Emails entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Name,
                    entity.Subject,
                    entity.Slug,
                    entity.CreatedAt.ToString(),
                    entity.UpdatedAt.ToString(),
                    entity.Islocked.ToString().ToLower()
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion

        #region [ ADD / EDIT ]
        /// <summary>
        /// Get & Set Value into TemplateEmailViewModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            var model = new TemplateEmailViewModel
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            
            if (id.HasValue)
            {
                Emails entity = _templateEmailService.GetById(id.Value);
                model.Id = entity.Id;
                model.Name = entity.Name;
                model.Subject = entity.Subject;
                model.Description = entity.Description;
                model.Slug = entity.Slug;
                model.CreatedAt = entity.CreatedAt;
                model.UpdatedAt = entity.UpdatedAt;
            }
            return View(model);
        }

        /// <summary>
        /// Insert or Update TemplateEmailViewModel Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult CreateEdit(int? id, TemplateEmailViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;

                    var entity = isExist ? _templateEmailService.GetById(model.Id) : new Emails();

                    entity.CreatedAt = isExist ? entity.CreatedAt : DateTime.Now;
                    entity.UpdatedAt = DateTime.Now;
                    entity.Name = model.Name;
                    entity.Subject = model.Subject.ToLower();
                    entity.Slug = model.Name.ToLower().Replace(" ", "-");
                    entity.Description = model.Description;

                    entity = isExist ? _templateEmailService.UpdateEmails(entity) : _templateEmailService.SaveEmails(entity);
                    ShowSuccessMessage("Success!", $"Email {(isExist ? "updated" : "created")} successfully", false);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }



        #endregion [ ADD / EDIT ]

        #region [View]

        [HttpGet]
        public IActionResult View(int? id)
        {
            TemplateEmailViewModel model = new TemplateEmailViewModel();
            if (id.HasValue)
            {
                Emails Options = _templateEmailService.GetById(id.Value);
                if (Options == null)
                {
                    return Redirect404();
                }
                model.Name = Options.Name;
                model.Subject = Options.Subject;
                model.Description= Options.Description;
            }
            return View(model);
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
                Message = "Are you sure you want to delete this Email information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Email Information" },
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
                bool isDeleted = _templateEmailService.Delete(id);

                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Email Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Email");
        }
        #endregion [ DELETE ]
    }
}
