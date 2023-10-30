using EC.Service.Payments;
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
using EC.Core;

namespace EC.Web.Areas.Admin.Controllers
{
    public class ReviewsController : BaseController
    {
        private readonly IReviewsService _reviewsService;
        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
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
            var query = new SearchQuery<Reviews>();
            query.IncludeProperties = "Order,Product,User";
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => (q.User.Firstname + ' ' + q.User.Lastname).Contains(sSearch));
            }
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Reviews, string>(q => q.User.Firstname+q.User.Lastname, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Reviews, string>(q => q.Product.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Reviews, string>(q => q.Rating.ToString(), sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Reviews, string>(q => q.Comment, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Reviews, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Reviews, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Reviews> entities = _reviewsService.GetReviewsByPage(query, out total).Entities;
            foreach (Reviews entity in entities)
            {
                var status = "";
                if (entity.Status == 1)
                {
                    status = "Pending";
                }
                else if (entity.Status == 2)
                {
                    status = "Approved";
                }
                else if (entity.Status == 3)
                {
                    status = "UnApproved";
                }
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.User != null ? entity.User.Firstname +' '+ entity.User.Lastname: "",
                    entity.Product!= null ? entity.Product.Title : "",
                    entity.Rating.ToString(),
                    entity.Comment != null ? entity.Comment.ToString() : string.Empty,
                    entity.CreatedAt.ToString(),
                    status,
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion

        #region [View]
        [HttpGet]
        public IActionResult ViewReviews(int? id)
        {
            ReviewsViewModel model = new ReviewsViewModel();
            if (id.HasValue)
            {
                var data = _reviewsService.GetByReviewsId(id.Value);
                if (data == null)
                {
                    return Redirect404();
                }
                model.Name = data.User.Firstname + ' ' + data.User.Lastname;
                model.Productname = data.Product.Title;
                model.Rating = data.Rating;
                model.Comment = data.Comment;
                model.CreatedAt = data.CreatedAt;
                //model.Status = data.Status;
                if (data.Status == 2)
                {
                    model.StatusView = "Approved";
                }
                else if (data.Status == 3)
                {
                    model.StatusView = "UnApproved";
                }
                else
                {
                    model.StatusView = "Pending";
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Approve(int id)
        {
            Reviews entity = _reviewsService.GetByReviewsId(id);
            entity.Status = 2;
            _reviewsService.UpdateReviewss(entity);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Reject(int id)
        {
            Reviews entity = _reviewsService.GetByReviewsId(id);
            entity.Status = 3;
            _reviewsService.UpdateReviewss(entity);
            return RedirectToAction("Index");
        }
        #endregion

    }
}
