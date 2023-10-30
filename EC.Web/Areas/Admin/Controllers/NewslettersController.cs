using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service.Currency;
using EC.Service.Currency_data;
using EC.Service.Newsletters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Data.Models;
using System.Linq;
using EC.Core;
using EC.Core.Enums;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;

namespace EC.Web.Areas.Admin.Controllers
{
    
    public class NewslettersController : BaseController
    {
        private readonly INewslettersService _NewslettersService;

        public NewslettersController(INewslettersService NewslettersService)
        {
            _NewslettersService = NewslettersService;
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
            var query = new SearchQuery<NewsLetters>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Email.Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<NewsLetters, string>(q => q.Email, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<NewsLetters, bool?>(q => q.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<NewsLetters, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<NewsLetters, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<NewsLetters> entities = _NewslettersService.GetNewsLettersByPage(query, out total).Entities;
            foreach (var entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Email.ToString(),
                    entity.Status.ToString(),
                    entity.CreatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [ STATUS ]

        /// <summary>
        /// Update Categories Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveNewsLettersStatus(int id)
        {
            //var IsDeleted = false;
            NewsLetters entity = _NewslettersService.GetById(id);
            entity.Status = !entity.Status;
            _NewslettersService.UpdateNewsLetters(entity);
            return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Status updated successfully.", IsSuccess = true });
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
                Message = "Are you sure you want to delete this Newsletter information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Newsletter Information" },
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
                bool isDeleted = _NewslettersService.Delete(id);
                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "Newsletters Information deleted successfully.", false);
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
            return RedirectToAction("Index");
        }

        #endregion [ DELETE ]
    }
}
