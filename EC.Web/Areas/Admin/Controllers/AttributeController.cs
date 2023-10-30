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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Web.Areas.Admin.Controllers
{
    [CustomAuthorization(RoleType.Administrator)]
    public class AttributeController : BaseController
    {
        #region [ SERVICE INJECTION ]
        /// <summary>
        /// Inject Required Services
        /// </summary>
        /// 
        private readonly IOptionsService _OptionsService;
        private readonly IOptionValuesService _optionValuesService;
        private readonly IProductService _productService;
        public AttributeController(IOptionsService OptionsService, IOptionValuesService optionValuesService, IProductService productService)
        {
            _OptionsService = OptionsService;
            _optionValuesService = optionValuesService;
            _productService = productService;
        }
        #endregion [ SERVICE INJECTION ]

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
        /// Get & Set Options record into DataTable With Pagination
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>return Options record DataTable With Pagination</returns>
        [HttpPost]
        public ActionResult Index(EC.DataTable.DataTables.DataTable dataTable)
        {
            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Options>();
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
                    query.AddSortCriteria(new ExpressionSortCriteria<Options, string>(q => q.Title, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Options, int>(q => q.SortOrder, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Options, bool>(q => q.Status, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Options, DateTime?>(q => q.CreatedAt, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Options, DateTime?>(q => q.CreatedAt, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;
            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Options> entities = _OptionsService.Get(query, out total).Entities;

            foreach (Options entity in entities)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    entity.Id.ToString(),
                    count.ToString(),
                    entity.Title,
                    entity.SortOrder.ToString(),
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
        /// Get & Set Value into AttributeViewModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            var model = new AttributeViewModel
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true
            };
            if (id.HasValue)
            {
                Options entity = _OptionsService.GetOptionsById(id.Value);
                var OptionValuesList = new List<OptionValuesViewModel>();
                foreach (var item in entity.OptionValues)
                {
                    OptionValuesViewModel optionValues = new OptionValuesViewModel();
                    optionValues.id = item.Id;
                    optionValues.title = item.Title;
                    optionValues.sortorder = item.SortOrder;
                    OptionValuesList.Add(optionValues);
                }
                model.OptionValuesList = OptionValuesList;
                model.Id = entity.Id;
                model.Title = entity.Title;
                model.SortOrder = entity.SortOrder;
                model.Status = entity.Status;
            }
            return PartialView("_CreateEdit", model);
        }

        /// <summary>
        /// Insert or Update AttributeViewModel Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        [HttpPost]
        public IActionResult CreateEdit(int? id, AttributeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExist = id.HasValue && id.Value != 0;
                    var attribute = _OptionsService.GetByTitle(model.Title.Trim());
                    var entity = isExist ? _OptionsService.GetById(model.Id) : new Options();
                    if (attribute != null && !isExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "The title has been already taken.", IsSuccess = false });
                    }

                    if (isExist && entity.Status != model.Status)
                    {
                        var product = _productService.GetByAttributeId(id.Value);
                        if (product != null && product.Any())
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Cannot update status, there are some products listed for this attribute.", IsSuccess = false });
                        }
                    }
                    entity.CreatedAt = isExist ? entity.CreatedAt : model.CreatedAt;
                    entity.UpdatedAt = model.UpdatedAt;
                    entity.Title = model.Title;
                    entity.Status = model.Status;
                    entity.SortOrder = model.SortOrder;
                    if (model.TitleOrderValue != null)
                    {
                        entity = isExist ? _OptionsService.Update(entity) : _OptionsService.Save(entity);
                        _optionValuesService.DeleteByOptionValueId(id.Value);
                        int count = 0;
                        var optionValues = new OptionValues();
                        foreach (string titleValue in model.TitleOrderValue)
                        {
                            if (count == 0)
                            {
                                optionValues = new OptionValues();
                                optionValues.Title = titleValue;
                                count++;
                            }
                            else
                            {
                                optionValues.SortOrder = Convert.ToInt32(titleValue);
                                optionValues.OptionId = entity.Id;
                                optionValues.Status = model.Status;
                                optionValues.CreatedAt = model.CreatedAt;
                                optionValues.UpdatedAt = isExist ? model.UpdatedAt : entity.UpdatedAt;
                                _optionValuesService.Save(optionValues);
                                count = 0;
                            }
                        }
                        ShowSuccessMessage("Success!", $"Attributes {(isExist ? "updated" : "created")} successfully", false);
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Please select Attribute Values !", IsSuccess = false });
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index"), IsSuccess = true });
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult View(int? id)
        {
            AttributeViewModel model = new AttributeViewModel();
            if (id.HasValue)
            {
                Options Options = _OptionsService.GetById(id.Value);
                if(Options == null)
                {
                    return Redirect404();
                }
                model.Title = Options.Title;
                model.Status = Options.Status;
                model.Status = Options.Status;
                model.CreatedAt = Options.CreatedAt;
                model.UpdatedAt = Options.UpdatedAt;

                List<OptionValues> valuesList = _optionValuesService.GetOptionValuesById(id);
                var OptionValuesList = new List<OptionValuesViewModel>();

                foreach (var item in valuesList)
                {
                    OptionValuesViewModel optionValues = new OptionValuesViewModel();
                    optionValues.id = item.Id;
                    optionValues.title = item.Title;
                    optionValues.sortorder = item.SortOrder;
                    OptionValuesList.Add(optionValues);
                }
                model.OptionValuesList = OptionValuesList;
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
                Message = "Are you sure you want to delete this Attribute information?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Attribute Information" },
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
                var productAttribute = _productService.GetByAttrId(id);
                if (productAttribute != null)
                {
                    ShowErrorMessage("Error!", "Cannot delete record, there are some products listed for this attribute.", false);
                    return RedirectToAction("Index", "Attribute");
                }
                bool isDeletedOptionValues = _optionValuesService.DeleteByOptionValueId(id);
                bool isDeleted = false;
                if (isDeletedOptionValues == true)
                {
                    isDeleted = _OptionsService.Delete(id);
                    if (isDeleted)
                    {
                        ShowSuccessMessage("Success!", "Attribute Information deleted successfully.", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Error occurred, Please try again.", false);
                    }
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
            return RedirectToAction("Index", "Attribute");
        }

        #endregion [ DELETE ]

        #region [ STATUS ]

        /// <summary>
        /// Update Attributes Record Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        [HttpGet]
        public IActionResult ActiveAttributeStatus(int id)
        {
            var product = _productService.GetByAttributeId(id);
            if (product != null && product.Any())
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { Data = "Cannot update status, there are some products listed for this attribute.", IsSuccess = false });
            }
            else
            {
                List<OptionValues> valuesList = _optionValuesService.GetOptionValuesById(id);
                if (valuesList != null && valuesList.Any())
                {
                    foreach (var item in valuesList)
                    {
                        item.Status = !item.Status;
                        _optionValuesService.Update(item);
                    }
                    Options entity = _OptionsService.GetById(id);
                    entity.Status = !entity.Status;
                    _OptionsService.Update(entity);
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
