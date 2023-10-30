using EC.Core.Enums;
using EC.Service;
using EC.Web.Areas.Admin.Code;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using EC.Web.Areas.Admin.ViewModels;
using System.Web.Mvc;
using EC.Core;

namespace EC.Web.Areas.Admin.Controllers
{
    [CustomAuthorization(RoleType.Administrator)]
    public class DashboardController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IOrdersService _OrdersService;
        private readonly IUsersService _UsersService;
        private readonly IReviewsService _reviewsService;
        public DashboardController(IOrdersService OrdersService, IUsersService UsersService, IReviewsService reviewsService, IConfiguration configuration)
        {
            _OrdersService = OrdersService;
            _UsersService = UsersService;
            _reviewsService = reviewsService;
            _configuration = configuration;
        }
        #region [Index]
        public async Task<IActionResult> Index(string filterPeriod)
        {
            DashBoardViewModels models = new DashBoardViewModels();
            try
            {
                if (filterPeriod != null)
                {
                    string Alldate = "";
                    string AllAmmount = "";
                    List<string> ListDateString = new List<string>();
                    using (var connection = new SqlConnection(_configuration["ConnectionStrings:ProjectDBConnection"]))
                    {
                        IEnumerable<DashBoardViewModels> AllOrderChartdata = await connection.QueryAsync<DashBoardViewModels>("GetDashboardChartData", new { Day = filterPeriod }, null, 120, commandType: CommandType.StoredProcedure);

                        foreach (var item in AllOrderChartdata)
                        {
                            ListDateString.Add(string.Format("{0:dd-MMM}", item.Date));
                            AllAmmount += item.TotalAmount + ",";
                        }
                        Alldate = Alldate.TrimEnd(',');
                        AllAmmount.TrimEnd(',');
                        ViewBag.Alldate = ListDateString.ToArray();
                        ViewBag.AllAmmount = AllAmmount;
                    }
                    var Total_Order_Complete = _OrdersService.GetAOrdersList(filterPeriod).Count;
                    ViewBag.Total_Order_Complete = Total_Order_Complete;

                    var Total_UsersList_Active = _UsersService.GetAUsersList(filterPeriod).Count;
                    ViewBag.Total_UsersList_Active = Total_UsersList_Active;

                    var Total_Reviews = _reviewsService.GetAReviewsList(filterPeriod).Count;
                    ViewBag.Total_Reviews = Total_Reviews;

                    var Total_Order_Complete_List = _OrdersService.GetAllOrdersList(filterPeriod);
                    ViewBag.Total_Order_Complete_List = Total_Order_Complete_List;

                    var Total_Order_Complete1 = _OrdersService.GetOrdersTotalList(filterPeriod);
                    ViewBag.Total_Order_Complete1 = Total_Order_Complete1;

                    ViewBag.filterPeriod = filterPeriod;
                }
                else
                {
                    string Alldate = "";
                    string AllAmmount = "";
                    List<string> ListDateString = new List<string>();
                    using (var connection = new SqlConnection(_configuration["ConnectionStrings:ProjectDBConnection"]))
                    {
                        IEnumerable<DashBoardViewModels> AllOrderChartdata = await connection.QueryAsync<DashBoardViewModels>("GetDashboardChartData", new { Day = 7 }, null, 120, commandType: CommandType.StoredProcedure);
                        foreach (var item in AllOrderChartdata)
                        {
                            ListDateString.Add(string.Format("{0:dd-MMM}", item.Date));
                            AllAmmount += item.TotalAmount + ",";
                        }
                        Alldate = Alldate.TrimEnd(',');
                        AllAmmount.TrimEnd(',');
                        ViewBag.Alldate = ListDateString.ToArray();
                        ViewBag.AllAmmount = AllAmmount;
                    }
                    filterPeriod = "7";
                    var Total_Order_Complete = _OrdersService.GetAOrdersList(filterPeriod).Count;
                    ViewBag.Total_Order_Complete = Total_Order_Complete;

                    var Total_UsersList_Active = _UsersService.GetAUsersList(filterPeriod).Count;
                    ViewBag.Total_UsersList_Active = Total_UsersList_Active;

                    var Total_Reviews = _reviewsService.GetAReviewsList(filterPeriod).Count;
                    ViewBag.Total_Reviews = Total_Reviews;

                    var Total_Order_Complete_List = _OrdersService.GetAllOrdersList(filterPeriod);
                    ViewBag.Total_Order_Complete_List = Total_Order_Complete_List;

                    var Total_Order_Complete1 = _OrdersService.GetOrdersTotalList(filterPeriod);
                    ViewBag.Total_Order_Complete1 = Total_Order_Complete1;

                    ViewBag.filterPeriod = filterPeriod;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
             return View();
        }
        #endregion
    }
}
