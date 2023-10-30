using EC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System;
using ToDo.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using EC.API.ViewModels.MultiVendor;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EC.API.Controllers.MultiVendor
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardVendorController : BaseAPIController
    {
        #region Constructor
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOrdersService _OrdersService;
        private readonly IUsersService _UsersService;
        private readonly IReviewsService _reviewsService;
        private readonly IProductService _productService;
        private readonly IContactUsService _contactUsService;
        public DashboardVendorController(IOrdersService OrdersService, IUsersService UsersService, IReviewsService reviewsService, IHostingEnvironment hostingEnvironment, IProductService productService, IContactUsService contactUsService)
        {
            _OrdersService = OrdersService;
            _UsersService = UsersService;
            _reviewsService = reviewsService;
            _hostingEnvironment = hostingEnvironment;
            _productService = productService;
            _contactUsService = contactUsService;
        }
        #endregion

        #region Vendor Dashboard

        [Authorize]
        [Route("/vendor/dashboard")]
        [HttpGet]
        public IActionResult dashboard(string from_date, string to_date)
        {
            try
            {
                dashboardvendor Vendordata = new dashboardvendor();
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var reviewcount = 0;
                if (from_date != null && to_date != null)
                {
                    var dataproduct = _productService.FindByvendorproductId(id);
                    var dataId = dataproduct[0].Id;
                    DateTime S_date = Convert.ToDateTime(from_date);
                    DateTime E_date = Convert.ToDateTime(to_date);

                    var Total_Order_Complete = _OrdersService.GetVendorOrdersList(S_date, E_date, id).Count;
                    Vendordata.order = Total_Order_Complete;

                    var productid = _productService.GetByVendorId(id);
                    foreach (var item in productid)
                    {
                        //var reviewList = _reviewsService.GetReviewListForVendor(item.Id);

                        var Total_Reviews = _reviewsService.GetVendorRviewList(S_date, E_date, item.Id).Count;
                        reviewcount += Total_Reviews;
                    }
                    if (reviewcount > 0)
                    {
                        Vendordata.reviews = reviewcount;
                    }
                    else
                    {
                        Vendordata.reviews = 0;
                    }
                    var Total_Active_Products = _productService.GetVendorproductList(S_date, E_date, id).Count;
                    Vendordata.total_products = Total_Active_Products;

                    var Total_Order_Sum = _OrdersService.GetVendorOrdersTotalList(S_date, E_date, id);
                    Vendordata.total_sale = Total_Order_Sum;

                   
                    if (dataproduct.Count>0)
                    {
                        var Total_Contact_Query = _contactUsService.GetVendorContactList(S_date, E_date, dataproduct.FirstOrDefault().Id).Count;
                        Vendordata.contact_query = Total_Contact_Query;
                    }
                    else
                    {
                        Vendordata.contact_query = 0;
                    }
                }
                return Ok(new { error = false, data = Vendordata, message = "Dashboard list fetch successfully!", code = 200, state = "dashboard", status = true });
            }
            catch (System.Exception msg)
            {
                var errorData = new { error = true, message = msg, code = 401, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
