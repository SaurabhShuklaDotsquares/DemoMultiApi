using EC.DataTable.Extension;
using EC.DataTable.Search;
using EC.DataTable.Sort;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using EC.Data.Models;
using System.Linq;
using EC.Service;
using EC.Service.Currency;
using EC.Web.Areas.Admin.ViewModels;
using EC.Core.Enums;
using EC.Web.Models.Others;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using EC.Service.Currency_data;
using System.Security.Cryptography.X509Certificates;
using MathNet.Numerics.Distributions;
using EC.Core;

namespace EC.Web.Areas.Admin.Controllers
{
    public class CurrencyController : BaseController
    {
        private readonly ICurrencyService _CurrencyService;
        private readonly ICurrenciesdataService _CurrenciesdataService;
        public CurrencyController(ICurrencyService currencyService, ICurrenciesdataService CurrenciesdataService)
        {
            _CurrencyService=currencyService;
            _CurrenciesdataService=CurrenciesdataService;
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
            var query = new SearchQuery<CurrencyData>();
            query.IncludeProperties = "Currency";
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.ToLower().Trim();
                query.AddFilter(q => q.Currency.Iso.ToLower().Contains(sSearch));
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<CurrencyData, string>(q => q.Currency.Iso, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<CurrencyData, DateTime?>(q => q.LastRateUpdate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<CurrencyData, string>(q => q.LiveRate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<CurrencyData, string>(q => q.ConvertedRate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<CurrencyData, DateTime?>(q => q.LastRateUpdate, SortDirection.Descending ));
                    break;

            }
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<CurrencyData> entities = _CurrenciesdataService.GetCurrencyDataByPage(query, out total).Entities;

            var CurrenciesLists = (from Curr in entities
                                   select new CurrenciesViewModels
                                   {
                                       Id = Convert.ToInt16(Curr.Id),
                                       CurrencyId = Curr.CurrencyId,
                                       Title = Curr.Currency.Iso +" (" + Curr.Currency.Name + ")",
                                       LastRateUpdate = Curr.LastRateUpdate,
                                       LiveRate = Curr.LiveRate,
                                       ConvertedRate =Curr.ConvertedRate,
                                   }).ToList();

            foreach (CurrenciesViewModels entity in CurrenciesLists)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title,
                    entity.LastRateUpdate.ToString(),                   
                    entity.LiveRate.ToString(),
                    entity.ConvertedRate.ToString(),
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
            CurrenciesViewModels models= new CurrenciesViewModels();
            var data= _CurrencyService.GetCurrenciessList().ToList();
            var CurrenciesLists = (from Curr in data
                                   select new CurrenciesViewModels
                                   {
                                       Id = Curr.Id,
                                       CurrencyId = Curr.CurrencyData.Count == 0 ? 0 : (Curr.CurrencyData.FirstOrDefault().CurrencyId),
                                       Title = Curr.Iso + " (" + Curr.Name + ")",
                                   }).Where(x => x.Id != x.CurrencyId).ToList();
            
            models.GetCurrencyType = CurrenciesLists.Select(x => new SelectListItem
            {
                Text = x.Title
            }).ToList();
            return PartialView("_Create", models);
        }
        [HttpPost]
        public IActionResult Create(int ? Id, CurrenciesViewModels model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = Id.HasValue && Id.Value != 0;
                    CurrenciesViewModels data = new CurrenciesViewModels();
                    var targetCurrency = model.Iso.Split('(')[0].Trim();
                    var id = _CurrencyService.GetCurrenciessList().Where(x => x.Iso == targetCurrency).FirstOrDefault().Id;
                    string URLString = "https://v6.exchangerate-api.com/v6/4a23355fd72fabbe8081725a/latest/" + targetCurrency;
                    using (var webClient = new System.Net.WebClient())
                    {
                        var json = webClient.DownloadString(URLString);
                        var Test = JsonConvert.DeserializeObject<Root>(json);

                        var Converted_Rate = Test.conversion_rates.USD;//Converted
                        var Live_Rate = (1 / (float)(Test.conversion_rates.USD));  //LiveRate

                        var entity = isExist ? _CurrenciesdataService.GetById(model.Id) : new CurrencyData();
                        entity.CurrencyId = id;
                        entity.Status = true;
                        entity.LiveRate = Live_Rate.ToString();
                        entity.ConvertedRate = Converted_Rate.ToString();
                        entity.LastRateUpdate = DateTime.Now;
                        entity.CreatedAt = DateTime.Now;
                        entity.UpdatedAt = DateTime.Now;
                        entity = isExist ? _CurrenciesdataService.UpdateCurrencyData(entity) : _CurrenciesdataService.SaveCurrencyData(entity);
                        ShowSuccessMessage("Success!", $"Currencies data {(isExist ? "updated" : "created")} successfully", false);
                        return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
                        //return View("index", "Currency");
                        //return RedirectToAction("Index");
                        //return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Currency");
            //return RedirectToAction("Index");
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
                Message = "Are you sure you want to delete this currency information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete currency Information" },
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
        public IActionResult Delete(long id, IFormCollection FC)
        {
            try
            {
                bool isDeleted = _CurrenciesdataService.Delete(Convert.ToInt16(id));
                if (isDeleted)
                {
                    ShowSuccessMessage("Success!", "currency Information deleted successfully.", false);
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
            return RedirectToAction("Index", "Currency");
        }

        #endregion [ DELETE ]
    }
}
