using EC.Data.Models;
using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using EC.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
using EC.Web.Areas.Admin.ViewModels;

namespace EC.Web.Areas.Admin.Controllers
{
    public class ContactUsController : BaseController
    {
        private readonly IContactUsService _ContactUsService;
        public ContactUsController(IContactUsService ContactUsService)
        {
            _ContactUsService=ContactUsService;
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
            var query = new SearchQuery<ContuctUs>();
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Email.Contains(sSearch));
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<ContuctUs, string>(q => q.Firstname.ToString(), sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<ContuctUs, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<ContuctUs, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<ContuctUs> entities = _ContactUsService.Get(query, out total).Entities;

            foreach (ContuctUs entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Firstname.ToString() +' '+ entity.Lastname.ToString(),
                    entity.Email.ToString(),
                    entity.CreatedAt.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #endregion

        #region [View]

        [HttpGet]
        public IActionResult View(int? id)
        {
            ContactUsViewModels model1 = new ContactUsViewModels();
            if (id.HasValue)
            {
                var data = _ContactUsService.GetById(id.Value);
                if (data == null)
                {
                    return Redirect404();
                }
                model1.Name = data.Firstname +' '+ data.Lastname;
                model1.Phone = data.Phone;
                model1.Email = data.Email;
                model1.Message = data.Message;
            }
            return View(model1);
        }
        #endregion
    }
}
