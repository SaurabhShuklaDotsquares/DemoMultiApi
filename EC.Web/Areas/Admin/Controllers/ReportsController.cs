using EC.DataTable.Search;
using EC.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using EC.Data.Models;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using EC.Web.Models.Others;
using EC.Core.LIBS;
using System.Globalization;
using Newtonsoft.Json;
using static EC.Data.Models.Orders;
using EC.Web.Areas.Admin.ViewModels;
using System.Reflection.Metadata;
using System.Security.Policy;
using Microsoft.AspNetCore.Http.Extensions;
using System.Web;

namespace EC.Web.Areas.Admin.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOrdersService _OrdersService;
        private readonly IUsersService _UsersService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandsService _brandsService;
        public ReportsController(IOrdersService OrdersService, IUsersService UsersService, ICategoryService categoryService, IBrandsService brandsService, IHostingEnvironment hostingEnvironment)
        {
            _OrdersService = OrdersService;
            _UsersService = UsersService;
            _categoryService = categoryService;
            _brandsService = brandsService;
            _hostingEnvironment = hostingEnvironment;
        }
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

        #region [Index]

        [HttpGet]
        public IActionResult Index(string start_date)
        {
            if (start_date != null)
            {
                ViewBag.start_date = start_date;
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);
                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);

                var Total_Order_Complete = _OrdersService.GetOrdersList(S_date, E_date).Count;
                ViewBag.Total_Order_Complete = Total_Order_Complete;

                var Total_Order_NewOrder = _OrdersService.GetProcessingList(S_date, E_date).Count;
                ViewBag.Total_Order_NewOrder = Total_Order_NewOrder;

                var Total_Order_Return = _OrdersService.GetOrdersreturnedList(S_date, E_date).Count;
                ViewBag.Total_Order_Return = Total_Order_Return;

                var Total_Order_cancelled = _OrdersService.GetOrderscancelledList(S_date, E_date).Count;
                ViewBag.Total_Order_cancelled = Total_Order_cancelled;

                var Total_UsersList_Active = _UsersService.GetUsersList(S_date, E_date).Count;
                ViewBag.Total_UsersList_Active = Total_UsersList_Active;

                var Total_UsersList_Inactive = _UsersService.GetUsers_InactiveList(S_date, E_date).Count;
                ViewBag.Total_UsersList_Inactive = Total_UsersList_Inactive;

                var Total_UsersList_category = _categoryService.GetCategorieList(S_date, E_date).Count;
                ViewBag.Total_UsersList_category = Total_UsersList_category;

                var Total_UsersList_brands = _brandsService.GetBrandsList(S_date, E_date).Count;
                ViewBag.Total_UsersList_brands = Total_UsersList_brands;
            }
            else
            {
                var Total_Order_Complete = _OrdersService.GetOrdersList().Count;
                ViewBag.Total_Order_Complete = Total_Order_Complete;

                var Total_Order_NewOrder = _OrdersService.GetProcessingList().Count;
                ViewBag.Total_Order_NewOrder = Total_Order_NewOrder;

                var Total_Order_Return = _OrdersService.GetOrdersreturnedList().Count;
                ViewBag.Total_Order_Return = Total_Order_Return;

                var Total_Order_cancelled = _OrdersService.GetOrderscancelledList().Count;
                ViewBag.Total_Order_cancelled = Total_Order_cancelled;

                var Total_UsersList_Active = _UsersService.GetUsersList().Count;
                ViewBag.Total_UsersList_Active = Total_UsersList_Active;

                var Total_UsersList_Inactive = _UsersService.GetUsers_InactiveList().Count;
                ViewBag.Total_UsersList_Inactive = Total_UsersList_Inactive;

                var Total_UsersList_category = _categoryService.GetCategorieList().Count;
                ViewBag.Total_UsersList_category = Total_UsersList_category;

                var Total_UsersList_brands = _brandsService.GetBrandsList().Count;
                ViewBag.Total_UsersList_brands = Total_UsersList_brands;
            }

            return View();
        }
        #endregion

        #region [Order Excel]
        public async Task<FileResult> ExportCSV(string start_date)
        {
            List<Orders> result = new List<Orders>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);

                result = _OrdersService.GetOrdersList(S_date, E_date).ToList();
            }
            else
            {
                 result = _OrdersService.GetOrdersList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("OrderId");
            dt.Columns.Add("Firstname");
            dt.Columns.Add("Lastname");
            dt.Columns.Add("Mobile");
            dt.Columns.Add("Email");
            dt.Columns.Add("Status");
            dt.Columns.Add("BillingAddress");
            dt.Columns.Add("ShipingAddress");
            dt.Columns.Add("ShippingType");
            dt.Columns.Add("Message");
            dt.Columns.Add("PaymentMethod");
            dt.Columns.Add("TransactionId");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");
            foreach (var res in result)
            {
                string add = "";
                string addShiping = "";
                if (res.BillingAddress != null)
                {
                    string BillingAdd = JsonConvert.SerializeObject(res.BillingAddress.ToString());
                    var jsonResult = JsonConvert.DeserializeObject(BillingAdd).ToString();
                    var re = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult);
                    add = re.address2 + ',' + re.state + ',' + re.postal_code + ',' + re.city + ',' + re.country;
                    add = "\"" + add + "\"";
                }
                if (res.ShipingAddress != null)
                {
                    string ShipingAdd = JsonConvert.SerializeObject(res.ShipingAddress.ToString());
                    var jsonResult1 = JsonConvert.DeserializeObject(ShipingAdd).ToString();
                    var data = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult1);
                    addShiping = data.address2 + ',' + data.state + ',' + data.postal_code + ',' + data.city + ',' + data.country;
                    addShiping = "\"" + addShiping + "\"";
                }
                System.Data.DataRow dr = dt.NewRow();
                dr["OrderId"] = res.OrderId;
                dr["Firstname"] = res.Firstname;
                dr["Lastname"] = res.Lastname;
                dr["Mobile"] = res.Mobile;
                dr["Email"] = res.Email;
                dr["Status"] = res.Status;
                dr["BillingAddress"] = add.ToString();
                dr["ShipingAddress"] = addShiping.ToString();
                dr["ShippingType"] = res.ShippingType;
                dr["Message"] = res.Message;
                dr["PaymentMethod"] = res.PaymentMethod;
                dr["TransactionId"] = res.TransactionId;
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Order Id");
            CsvRow.Add("First Name");
            CsvRow.Add("Last Name");
            CsvRow.Add("Mobile");
            CsvRow.Add("Email");
            CsvRow.Add("Status");
            CsvRow.Add("Billing Address");
            CsvRow.Add("Shipping Address");
            CsvRow.Add("Shipping Type");
            CsvRow.Add("Message");
            CsvRow.Add("Payment Method");
            CsvRow.Add("Transaction Id");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");
            sb.AppendLine(string.Join(delimiter, CsvRow));
            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["OrderId"].ToString());
                    CsvRow.Add(dr["Firstname"].ToString());
                    CsvRow.Add(dr["Lastname"].ToString());
                    CsvRow.Add(dr["Mobile"].ToString());
                    CsvRow.Add(dr["Email"].ToString());
                    CsvRow.Add(dr["Status"].ToString());
                    CsvRow.Add(dr["BillingAddress"].ToString());
                    CsvRow.Add(dr["ShipingAddress"].ToString());
                    CsvRow.Add(dr["ShippingType"].ToString());
                    CsvRow.Add(dr["Message"].ToString());
                    CsvRow.Add(dr["PaymentMethod"].ToString());
                    CsvRow.Add(dr["TransactionId"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [Active Customer Excel]
        public async Task<FileResult> ExportCSVActiveCustomer(string start_date)
        {
            List<Users> result = new List<Users>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                result= _UsersService.GetUsersList(S_date, E_date).ToList();
            }
            else
            {
                result = _UsersService.GetUsersList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Firstname");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");
            dt.Columns.Add("Email");
            dt.Columns.Add("Mobile");
            dt.Columns.Add("IsActive");
            foreach (var res in result)
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["Id"] = res.Id;
                dr["Firstname"] = $"{res.Firstname} {res.Lastname}";
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dr["Email"] = res.Email;
                dr["Mobile"] = res.Mobile;
                dr["IsActive"] = res.IsActive;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Id");
            CsvRow.Add("User Name");
            CsvRow.Add("Email");
            CsvRow.Add("Mobile");
            CsvRow.Add("Status");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");

            sb.AppendLine(string.Join(delimiter, CsvRow));

            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["Id"].ToString());
                    CsvRow.Add(dr["Firstname"].ToString());
                    CsvRow.Add(dr["Email"].ToString());
                    CsvRow.Add(dr["Mobile"].ToString());
                    CsvRow.Add(dr["IsActive"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [InActive Customer Excel]
        public async Task<FileResult> ExportCSVInactiveCustomer(string start_date)
        {
            List<Users> result = new List<Users>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                result = _UsersService.GetUsers_InactiveList(S_date, E_date);
            }
            else
            {
                result = _UsersService.GetUsers_InactiveList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Firstname");
            dt.Columns.Add("Lastname");
            dt.Columns.Add("Mobile");
            dt.Columns.Add("Email");
            dt.Columns.Add("Status");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");

            foreach (var res in result)
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["Id"] = res.Id;
                dr["Firstname"] = res.Firstname;
                dr["Lastname"] = res.Lastname;
                dr["Mobile"] = res.Mobile;
                dr["Email"] = res.Email;
                dr["Status"] = res.IsActive;
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Id");
            CsvRow.Add("First Name");
            CsvRow.Add("Last Name");
            CsvRow.Add("Mobile");
            CsvRow.Add("Email");
            CsvRow.Add("Status");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");

            sb.AppendLine(string.Join(delimiter, CsvRow));

            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["Id"].ToString());
                    CsvRow.Add(dr["Firstname"].ToString());
                    CsvRow.Add(dr["Lastname"].ToString());
                    CsvRow.Add(dr["Mobile"].ToString());
                    CsvRow.Add(dr["Email"].ToString());
                    CsvRow.Add(dr["Status"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [NewOrder Excel]

        public async Task<FileResult> ExportCSVNewOrderOrder(string start_date)
        {
            List<Orders> result = new List<Orders>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                result= _OrdersService.GetProcessingList(S_date, E_date).ToList();
            }
            else
            {
                result = _OrdersService.GetProcessingList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("OrderId");
            dt.Columns.Add("Firstname");
            dt.Columns.Add("Lastname");
            dt.Columns.Add("Mobile");
            dt.Columns.Add("Email");
            dt.Columns.Add("Status");
            dt.Columns.Add("BillingAddress");
            dt.Columns.Add("ShipingAddress");
            dt.Columns.Add("ShippingType");
            dt.Columns.Add("Message");
            dt.Columns.Add("PaymentMethod");
            dt.Columns.Add("TransactionId");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");

            foreach (var res in result)
            {
                string add = "";
                string addShiping = "";
                if (res.BillingAddress != null)
                {
                    string BillingAdd = JsonConvert.SerializeObject(res.BillingAddress.ToString());
                    var jsonResult = JsonConvert.DeserializeObject(BillingAdd).ToString();
                    var re = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult);
                    add = re.address2 + ',' + re.state + ',' + re.postal_code + ',' + re.city + ',' + re.country;
                    add = "\"" + add + "\"";
                }
                if (res.ShipingAddress != null)
                {
                    string ShipingAdd = JsonConvert.SerializeObject(res.ShipingAddress.ToString());
                    var jsonResult1 = JsonConvert.DeserializeObject(ShipingAdd).ToString();
                    var data = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult1);
                    addShiping = data.address2 + ',' + data.state + ',' + data.postal_code + ',' + data.city + ',' + data.country;
                    addShiping = "\"" + addShiping + "\"";
                }

                System.Data.DataRow dr = dt.NewRow();
                dr["Id"] = res.Id;
                dr["OrderId"] = res.OrderId;
                dr["Firstname"] = res.Firstname;
                dr["Lastname"] = res.Lastname;
                dr["Mobile"] = res.Mobile;
                dr["Email"] = res.Email;
                dr["Status"] = res.Status;
                dr["BillingAddress"] = add.ToString();
                dr["ShipingAddress"] = addShiping.ToString();
                dr["ShippingType"] = res.ShippingType;
                dr["Message"] = res.Message;
                dr["PaymentMethod"] = res.PaymentMethod;
                dr["TransactionId"] = res.TransactionId;
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Id");
            CsvRow.Add("Order Id");
            CsvRow.Add("First Name");
            CsvRow.Add("Last Name");
            CsvRow.Add("Mobile");
            CsvRow.Add("Email");
            CsvRow.Add("Status");
            CsvRow.Add("Billing Address");
            CsvRow.Add("Shipping Address");
            CsvRow.Add("Shipping Type");
            CsvRow.Add("Message");
            CsvRow.Add("Payment Method");
            CsvRow.Add("Transaction Id");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");

            sb.AppendLine(string.Join(delimiter, CsvRow));

            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["Id"].ToString());
                    CsvRow.Add(dr["OrderId"].ToString());
                    CsvRow.Add(dr["Firstname"].ToString());
                    CsvRow.Add(dr["Lastname"].ToString());
                    CsvRow.Add(dr["Mobile"].ToString());
                    CsvRow.Add(dr["Email"].ToString());
                    CsvRow.Add(dr["Status"].ToString());
                    CsvRow.Add(dr["BillingAddress"].ToString());
                    CsvRow.Add(dr["ShipingAddress"].ToString());
                    CsvRow.Add(dr["ShippingType"].ToString());
                    CsvRow.Add(dr["Message"].ToString());
                    CsvRow.Add(dr["PaymentMethod"].ToString());
                    CsvRow.Add(dr["TransactionId"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [Cancell Order Excel]

        public async Task<FileResult> ExportCSVTotalCancelledOrder(string start_date)
        {
            List<Orders> result = new List<Orders>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                result= _OrdersService.GetOrderscancelledList(S_date, E_date).ToList();
            }
            else
            {
                result = _OrdersService.GetOrderscancelledList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("OrderId");
            dt.Columns.Add("Firstname");
            dt.Columns.Add("Lastname");
            dt.Columns.Add("Mobile");
            dt.Columns.Add("Email");
            dt.Columns.Add("Status");
            dt.Columns.Add("BillingAddress");
            dt.Columns.Add("ShipingAddress");
            dt.Columns.Add("ShippingType");
            dt.Columns.Add("Message");
            dt.Columns.Add("PaymentMethod");
            dt.Columns.Add("TransactionId");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");

            foreach (var res in result)
            {
                string add = "";
                string addShiping = "";
                if (res.BillingAddress != null)
                {
                    string BillingAdd = JsonConvert.SerializeObject(res.BillingAddress.ToString());
                    var jsonResult = JsonConvert.DeserializeObject(BillingAdd).ToString();
                    var re = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult);
                    add = re.address2 + ',' + re.state + ',' + re.postal_code + ',' + re.city + ',' + re.country;
                    add = "\"" + add + "\"";
                }
                if (res.ShipingAddress != null)
                {
                    string ShipingAdd = JsonConvert.SerializeObject(res.ShipingAddress.ToString());
                    var jsonResult1 = JsonConvert.DeserializeObject(ShipingAdd).ToString();
                    var data = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult1);
                    addShiping = data.address2 + ',' + data.state + ',' + data.postal_code + ',' + data.city + ',' + data.country;
                    addShiping = "\"" + addShiping + "\"";
                }
                System.Data.DataRow dr = dt.NewRow();
                dr["Id"] = res.Id;
                dr["OrderId"] = res.OrderId;
                dr["Firstname"] = res.Firstname;
                dr["Lastname"] = res.Lastname;
                dr["Mobile"] = res.Mobile;
                dr["Email"] = res.Email;
                dr["Status"] = res.Status;
                dr["BillingAddress"] = add.ToString();
                dr["ShipingAddress"] = addShiping.ToString();
                dr["ShippingType"] = res.ShippingType;
                dr["Message"] = res.Message;
                dr["PaymentMethod"] = res.PaymentMethod;
                dr["TransactionId"] = res.TransactionId;
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Id");
            CsvRow.Add("Order Id");
            CsvRow.Add("First Name");
            CsvRow.Add("Last Name");
            CsvRow.Add("Mobile");
            CsvRow.Add("Email");
            CsvRow.Add("Status");
            CsvRow.Add("Billing Address");
            CsvRow.Add("Shipping Address");
            CsvRow.Add("Shipping Type");
            CsvRow.Add("Message");
            CsvRow.Add("Payment Method");
            CsvRow.Add("Transaction Id");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");

            sb.AppendLine(string.Join(delimiter, CsvRow));

            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["Id"].ToString());
                    CsvRow.Add(dr["OrderId"].ToString());
                    CsvRow.Add(dr["Firstname"].ToString());
                    CsvRow.Add(dr["Lastname"].ToString());
                    CsvRow.Add(dr["Mobile"].ToString());
                    CsvRow.Add(dr["Email"].ToString());
                    CsvRow.Add(dr["Status"].ToString());
                    CsvRow.Add(dr["BillingAddress"].ToString());
                    CsvRow.Add(dr["ShipingAddress"].ToString());
                    CsvRow.Add(dr["ShippingType"].ToString());
                    CsvRow.Add(dr["Message"].ToString());
                    CsvRow.Add(dr["PaymentMethod"].ToString());
                    CsvRow.Add(dr["TransactionId"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [category Excel]
        public async Task<FileResult> ExportCSVCategory(string start_date)
        {
            List<Categories> result = new List<Categories>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                result= _categoryService.GetCategorieList(S_date, E_date).ToList();
            }
            else
            {
                result = _categoryService.GetCategorieList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Title");
            dt.Columns.Add("Slug");
            dt.Columns.Add("Image");
            dt.Columns.Add("ApprovalStatus");
            dt.Columns.Add("Banner");
            dt.Columns.Add("Status");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");
            foreach (var res in result)
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["Title"] = res.Title;
                dr["Slug"] = res.Slug;
                dr["Image"] = res.Image;
                dr["ApprovalStatus"] = res.ApprovalStatus;
                dr["Banner"] = res.Banner;
                dr["Status"] = res.Status;
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Title");
            CsvRow.Add("Slug");
            CsvRow.Add("Image");
            CsvRow.Add("Approval Status");
            CsvRow.Add("Banner");
            CsvRow.Add("Status");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");
            sb.AppendLine(string.Join(delimiter, CsvRow));

            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["Title"].ToString());
                    CsvRow.Add(dr["Slug"].ToString());
                    CsvRow.Add(dr["Image"].ToString());
                    CsvRow.Add(dr["ApprovalStatus"].ToString());
                    CsvRow.Add(dr["Banner"].ToString());
                    CsvRow.Add(dr["Status"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [Brand Excel]

        public async Task<FileResult> ExportCSVBrand(string start_date)
        {
            List<Brands> result = new List<Brands>();
            if (start_date != null)
            {
                string start = start_date;
                string str1 = start.Split('-')[0].Trim();
                CultureInfo provider = new CultureInfo("en-US");
                DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                string str2 = start.Split('-')[1].Trim();
                CultureInfo provider2 = new CultureInfo("en-US");
                DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                result=_brandsService.GetBrandsList(S_date, E_date).ToList();
            }
            else
            {
                result = _brandsService.GetBrandsList();
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Title");
            dt.Columns.Add("Slug");
            dt.Columns.Add("Image");
            dt.Columns.Add("IsFeatured");
            dt.Columns.Add("ApprovalStatus");
            dt.Columns.Add("Status");
            dt.Columns.Add("CreatedAt");
            dt.Columns.Add("UpdatedAt");
            foreach (var res in result)
            {
                System.Data.DataRow dr = dt.NewRow();
                dr["Title"] = res.Title;
                dr["Slug"] = res.Slug;
                dr["Image"] = res.Image;
                if (res.IsFeatured==true)
                {
                   dr["IsFeatured"] = "Yes";
                }
                else
                {
                    dr["IsFeatured"] = "No";
                }
                if (res.ApprovalStatus==0)
                {
                    dr["ApprovalStatus"] = "Approved";
                }
                else
                {
                    dr["ApprovalStatus"] = "UnApproved";
                }
                if (res.Status==true)
                {
                    dr["Status"] = "Active";
                }
                else
                {
                    dr["Status"] = "InActive";
                }
                dr["CreatedAt"] = res.CreatedAt;
                dr["UpdatedAt"] = res.UpdatedAt;
                dt.Rows.Add(dr);
            }
            System.Data.DataView view = new System.Data.DataView(dt);
            dt = view.ToTable(true);

            string filename = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();
            List<string> CsvRow = new List<string>();
            CsvRow.Add("Title");
            CsvRow.Add("Slug");
            CsvRow.Add("Image");
            CsvRow.Add("IsFeatured");
            CsvRow.Add("Approval Status");
            CsvRow.Add("Status");
            CsvRow.Add("Created_at");
            CsvRow.Add("Updated_at");

            sb.AppendLine(string.Join(delimiter, CsvRow));

            if (result != null && result.Count() > 0)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    CsvRow.Clear();
                    CsvRow.Add(dr["Title"].ToString());
                    CsvRow.Add(dr["Slug"].ToString());
                    CsvRow.Add(dr["Image"].ToString());
                    CsvRow.Add(dr["IsFeatured"].ToString());
                    CsvRow.Add(dr["ApprovalStatus"].ToString());
                    CsvRow.Add(dr["Status"].ToString());
                    CsvRow.Add(dr["CreatedAt"].ToString());
                    CsvRow.Add(dr["UpdatedAt"].ToString());
                    sb.AppendLine(string.Join(delimiter, CsvRow));
                }
            }
            else
            {
                CsvRow.Clear();
                CsvRow.Add("No records are available.");
            }
            System.IO.File.WriteAllText(filePath, sb.ToString());
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        #endregion

        #region [Returnt Order Excel]
        public async Task<FileResult> ExportCSVReturnedOrder(string start_date)
        {
            try
            {
                List<Orders> result = new List<Orders>();
                if (start_date != null)
                {
                    string start = start_date;
                    string str1 = start.Split('-')[0].Trim();
                    CultureInfo provider = new CultureInfo("en-US");
                    DateTime S_date = DateTime.Parse(str1, provider, DateTimeStyles.AdjustToUniversal);

                    string str2 = start.Split('-')[1].Trim();
                    CultureInfo provider2 = new CultureInfo("en-US");
                    DateTime E_date = DateTime.Parse(str2, provider2, DateTimeStyles.AdjustToUniversal);
                   result= _OrdersService.GetOrdersreturnedList(S_date, E_date).ToList();
                }
                else
                {
                    result = _OrdersService.GetOrdersreturnedList();
                }
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("OrderId");
                dt.Columns.Add("Firstname");
                dt.Columns.Add("Lastname");
                dt.Columns.Add("Mobile");
                dt.Columns.Add("Email");
                dt.Columns.Add("Status");
                dt.Columns.Add("BillingAddress");
                dt.Columns.Add("ShipingAddress");
                dt.Columns.Add("ShippingType");
                dt.Columns.Add("Message");
                dt.Columns.Add("PaymentMethod");
                dt.Columns.Add("TransactionId");
                dt.Columns.Add("CreatedAt");
                dt.Columns.Add("UpdatedAt");
                foreach (var res in result)
                {
                    string add = "";
                    string addShiping = "";
                    if (res.BillingAddress!=null)
                    {
                        string BillingAdd = JsonConvert.SerializeObject(res.BillingAddress.ToString());
                        var jsonResult = JsonConvert.DeserializeObject(BillingAdd).ToString();
                        var re = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult);
                         add = re.address2 + ',' + re.state + ',' + re.postal_code + ',' + re.city + ',' + re.country;
                        add= "\"" + add + "\"";
                    }
                    if (res.ShipingAddress != null)
                    {
                        string ShipingAdd = JsonConvert.SerializeObject(res.ShipingAddress.ToString());
                        var jsonResult1 = JsonConvert.DeserializeObject(ShipingAdd).ToString();
                         var data = JsonConvert.DeserializeObject<ReturnExcelImportViewModel>(jsonResult1);
                         addShiping = data.address2 + ',' + data.state + ',' + data.postal_code + ',' + data.city + ',' + data.country;
                        addShiping = "\"" + addShiping + "\"";
                    }
                    System.Data.DataRow dr = dt.NewRow();
                    dr["Id"] = res.Id;
                    dr["OrderId"] = res.OrderId;
                    dr["Firstname"] = res.Firstname;
                    dr["Lastname"] = res.Lastname;
                    dr["Mobile"] = res.Mobile;
                    dr["Email"] = res.Email;
                    dr["Status"] = res.Status;
                    dr["BillingAddress"] = add.ToString();
                    dr["ShipingAddress"] = addShiping.ToString();
                    dr["ShippingType"] = res.ShippingType;
                    dr["Message"] = res.Message;
                    dr["PaymentMethod"] = res.PaymentMethod;
                    dr["TransactionId"] = res.TransactionId;
                    dr["CreatedAt"] = res.CreatedAt;
                    dr["UpdatedAt"] = res.UpdatedAt;
                    dt.Rows.Add(dr);
                }
                System.Data.DataView view = new System.Data.DataView(dt);
                dt = view.ToTable(true);

                string filename = Guid.NewGuid() + ".csv";
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "csv", filename);
                string delimiter = ",";

                StringBuilder sb = new StringBuilder();
                List<string> CsvRow = new List<string>();
                CsvRow.Add("Id");
                CsvRow.Add("Order Id");
                CsvRow.Add("First Name");
                CsvRow.Add("Last Name");
                CsvRow.Add("Mobile");
                CsvRow.Add("Email");
                CsvRow.Add("Status");
                CsvRow.Add("Billing Address");
                CsvRow.Add("Shipping Address");
                CsvRow.Add("Shipping Type");
                CsvRow.Add("Message");
                CsvRow.Add("Payment Method");
                CsvRow.Add("Transaction Id");
                CsvRow.Add("Created_at");
                CsvRow.Add("Updated_at");
                sb.AppendLine(string.Join(delimiter, CsvRow));

                if (result != null && result.Count() > 0)
                {
                    foreach (System.Data.DataRow dr in dt.Rows)
                    {
                        CsvRow.Clear();
                        CsvRow.Add(dr["Id"].ToString());
                        CsvRow.Add(dr["OrderId"].ToString());
                        CsvRow.Add(dr["Firstname"].ToString());
                        CsvRow.Add(dr["Lastname"].ToString());
                        CsvRow.Add(dr["Mobile"].ToString());
                        CsvRow.Add(dr["Email"].ToString());
                        CsvRow.Add(dr["Status"].ToString());
                        CsvRow.Add(dr["BillingAddress"].ToString());
                        CsvRow.Add(dr["ShipingAddress"].ToString());
                        CsvRow.Add(dr["ShippingType"].ToString());
                        CsvRow.Add(dr["Message"].ToString());
                        CsvRow.Add(dr["PaymentMethod"].ToString());
                        CsvRow.Add(dr["TransactionId"].ToString());
                        CsvRow.Add(dr["CreatedAt"].ToString());
                        CsvRow.Add(dr["UpdatedAt"].ToString());
                        sb.AppendLine(string.Join(delimiter, CsvRow));
                    }
                }
                else
                {
                    CsvRow.Clear();
                    CsvRow.Add("No records are available.");
                }
                System.IO.File.WriteAllText(filePath, sb.ToString());

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

    }
}
