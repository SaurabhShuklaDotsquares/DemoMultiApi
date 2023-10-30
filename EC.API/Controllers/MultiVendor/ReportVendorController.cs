using EC.Service.ReturnRequest;
using EC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System;
using ToDo.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using EC.API.ViewModels.MultiVendor;
using EC.Core.LIBS;
using EC.API.ViewModels.SiteKey;
using EC.API.ViewModels;
using EC.Service.Specification;
using Stripe;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using EC.Data.Models;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using EC.Data.Entities;
using Orders = EC.Data.Models.Orders;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportVendorController : BaseAPIController
    {
        #region Constructor
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOrdersService _OrdersService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly ICategoryService _categoryService;
        public ReportVendorController(IOrdersService OrdersService, IHostingEnvironment hostingEnvironment, IReturnRequestService returnRequestService, ICategoryService categoryService)
        {
            _OrdersService = OrdersService;
            _hostingEnvironment = hostingEnvironment;
            _returnRequestService = returnRequestService;
            _categoryService = categoryService;
        }
        #endregion

        #region Excel related common
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        #endregion

        #region Vendor report

        [Authorize]
        [Route("/vendor/report")]
        [HttpGet]
        public IActionResult ManageReport(string from_date, string to_date)
        {
            try
            {
                ReportManagervendor reportVendordata = new ReportManagervendor();
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                if (from_date != null && to_date != null)
                {
                    //string str1 = from_date;
                    CultureInfo provider = new CultureInfo("en-US");
                    //DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);
                    DateTime S_date = Convert.ToDateTime(from_date);
                    //string str2 = to_date;
                    CultureInfo provider2 = new CultureInfo("en-US");
                    //DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                    DateTime E_date = Convert.ToDateTime(to_date);
                    //var dataproduct = _productService.FindByvendorproductId(id);
                    //var dataId = dataproduct[0].Id;

                    var Total_Order_Complete = _OrdersService.GetVendorOrdersList(S_date, E_date, id).Count;
                    reportVendordata.total_completed_orders = Total_Order_Complete;

                    var Total_Order_Processing = _OrdersService.GetVendorProcessingOrdersList(S_date, E_date, id).Count;
                    reportVendordata.total_processing_orders = Total_Order_Processing;

                    var Total_Order_returned = _OrdersService.GetReturnVendorOrdersList(S_date, E_date, id).Count;
                    reportVendordata.total_returned_orders = Total_Order_returned;

                    var Total_Order_Sum = _OrdersService.GetVendorOrdersTotalList(S_date, E_date, id);
                    reportVendordata.earnings = Total_Order_Sum;
                }
                return Ok(new { error = false, data = reportVendordata, message = "report fetch successfully!", code = 200, status = true });
            }
            catch (System.Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region Excel Genrate
        [Route("/vendor/report-earnings")]
        [Authorize]
        [HttpGet]
        public IActionResult ManageEraning(string from_date, string to_date)
        {
            try
            {
                List<Orders> result = new List<Orders>();
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                DateTime S_date = Convert.ToDateTime(from_date);
                DateTime E_date = Convert.ToDateTime(to_date);
                result = _OrdersService.GetOrdersVendorList(S_date, E_date, id).ToList();
                System.Data.DataTable dt = new System.Data.DataTable();
                
                string AttachementName = string.Empty;
                string filename = Guid.NewGuid() + ".csv";
                
                StreamWriter httpStream = new StreamWriter(new FileStream(SiteKey.UploadExel + filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None));
                    httpStream.WriteLine(string.Join(",", "Order Id", "Payment Method", "Status", "User Email", "Total Amount", "Total Tax", "Shipping Charge", "Return Amount", "Total Earnings", "Address", "Order Date"));
                    foreach (var item in result)
                    {
                    string add = "";
                    string addShiping = "";
                    decimal commisiinget = (item.Amount * Convert.ToDecimal(item.AdminCommission) / 100);
                    if (item.ShipingAddress != null)
                      {
                          string ShipingAdd = JsonConvert.SerializeObject(item.ShipingAddress.ToString());
                          var jsonResult1 = JsonConvert.DeserializeObject(ShipingAdd).ToString();
                          var data = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult1);
                          addShiping = data.address2 + ',' + data.state + ',' + data.postal_code + ',' + data.city + ',' + data.country;
                          addShiping = "\"" + addShiping + "\"";
                      }

                        httpStream.WriteLine(string.Join(",", item.OrderId, item.PaymentMethod, item.Status, item.Email, item.Amount, (Convert.ToInt32(item.OrderItems.FirstOrDefault().Tax) * (item.OrderItems.FirstOrDefault().Quantity)), item.ShippingAmount, (item.ReturnRequests.FirstOrDefault() != null ? item.ReturnRequests.FirstOrDefault().RefundAmount:"0"),((item
                            .Amount - commisiinget) - item.ShippingAmount), addShiping.ToString(),item.CreatedAt));
                    }
                    string fullFilePath = SiteKey.ImagePath + "/storage" + "/export/" + filename;
                    httpStream.Flush();
                    httpStream.Close();
                    httpStream.Dispose();
                return Ok(new { error = false, data = fullFilePath, message = "report fetch successfully!", code = 200, status = true });

                }
                catch (Exception msg)
                {
                   var errorData = new { error = true, message = msg, code = 401, status = false };
                   return new InternalResponse(errorData);
                }
           
        }

        #endregion

        #region Vendor commission

        [Authorize]
        [Route("/category-list/commission")]
        [HttpGet]
        public IActionResult ManageCommission([FromQuery] ToDoSearchSpecs paging)
        {
            try
            {
                ManageCommission ManageCommissions = new ManageCommission();
                List<ManageCommissionViewModels> manageCommissionListModel = new List<ManageCommissionViewModels>();
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                var ManageList = _categoryService.GetManageCommissionList(paging);

                if (ManageList != null && ManageList.Any())
                {
                    var PageMetadate = new
                    {
                        ManageList.CurrentPage,
                        ManageList.PazeSize,
                        ManageList.TotalPage,
                        ManageList.TotalCount,
                        ManageList.HasNext,
                        ManageList.HasPrev
                    };
                    Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));

                    foreach (var item in ManageList)
                    {
                        ManageCommissionViewModels commissiondata=new ManageCommissionViewModels();

                        commissiondata.id = item.Id;
                        commissiondata.parent_id = item.ParentId;
                        commissiondata.seller_id = item.SellerId;
                        commissiondata.is_featured = item.IsFeatured;
                        commissiondata.admin_commission = item.AdminCommission;
                        commissiondata.title = item.Title;
                        commissiondata.slug = item.Slug;
                        if (commissiondata.image != null)
                        {
                            //string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + commissiondata.image;
                            commissiondata.image = item.Image;
                        }
                        else
                        {
                            commissiondata.image = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        }
                        commissiondata.banner = item.Banner;
                        commissiondata.meta_title = item.MetaTitle;
                        commissiondata.meta_keyword = item.MetaKeyword;
                        commissiondata.meta_description = item.MetaDescription;
                        if (item.Status==true)
                        {
                            commissiondata.status = 1;
                        }
                        else
                        {
                            commissiondata.status = 0;
                        }
                        commissiondata.approval_status = item.ApprovalStatus;
                        commissiondata.lft = item.Lft;
                        commissiondata.rgt = item.Rgt;
                        commissiondata.depth = item.Depth;
                        commissiondata.created_at = item.CreatedAt;
                        commissiondata.updated_at = item.UpdatedAt;
                        if (commissiondata.image != null)
                        {
                            string uploadsFolder = SiteKey.ImagePath + "/Uploads/" + item.Image;
                            commissiondata.image_link = uploadsFolder;
                        }
                        else
                        {
                            commissiondata.image_link = SiteKey.ImagePath + "/Uploads/" + SiteKey.DefaultImage;
                        }
                        manageCommissionListModel.Add(commissiondata);
                    }
                    ManageCommissions.data= manageCommissionListModel;
                    ManageCommissions.current_page = ManageList.CurrentPage;
                    ManageCommissions.page_size = ManageList.PazeSize;
                    ManageCommissions.total_page = ManageList.TotalPage;
                    return Ok(new { error = false, data = ManageCommissions, message = "Category commission fetch successfully.", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Category commission List Not Found.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }

        #endregion
    }
}
