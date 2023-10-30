using EC.API.ViewModels;
using EC.Data.Models;
using EC.Service;
using EC.Service.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using Stripe;
using System;
using System.Collections.Generic;
using ToDo.WebApi.Models;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewsController : BaseAPIController
    {

        #region [Constructor]
        private readonly IReviewsService _reviewsService;
        private readonly IProductService _productService;
        private readonly IOrdersService _OrdersService;
        private readonly IUsersService _usersService;
        public ReviewsController(IReviewsService reviewsService, IProductService productService, IOrdersService ordersService, IUsersService usersService)
        {
            _reviewsService = reviewsService;
            _productService = productService;
            _OrdersService = ordersService;
            _usersService = usersService;
        }
        #endregion

        #region [Add Review]
        [Authorize]
        [Route("/order/review")]
        [HttpPost]
        public IActionResult AddReviews(ReviewsViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var reviewdata = _reviewsService.IsReviewsExists(model.product_id, model.order_id,id);

                if (reviewdata == false)
                {
                    Reviews entitycarts = new Reviews();
                    entitycarts.UserId = id;
                    entitycarts.ProductId = model.product_id;
                    entitycarts.OrderId = Convert.ToInt16(model.order_id);
                    entitycarts.Comment= model.comment;
                    entitycarts.Rating= model.rating;
                    entitycarts.Status = 1;
                    entitycarts.CreatedAt= DateTime.Now;
                    var entity = _reviewsService.AddReviewss(entitycarts);

                    ReviewsreturnViewModel returnupdatedata = new ReviewsreturnViewModel();
                    if (entity!=null)
                    {
                        returnupdatedata.id = entity.Id;
                        returnupdatedata.user_id = entity.UserId;
                        returnupdatedata.product_id = entity.ProductId;
                        returnupdatedata.order_id = entity.OrderId.ToString();
                        returnupdatedata.comment = entity.Comment;
                        returnupdatedata.rating = entity.Rating;

                        return Ok(new { error = false, data = returnupdatedata, Message = "Review added successfully and pending for moderation.", code = 200, status = true });
                    }

                    var errorData = new { error = true, message = "Review added successfully.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
                else
                {
                    var errorData = new { error = true, message = "A review already posted for this order.", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region [Review List]
        [Route("/review-productid")]
        [HttpGet]
        public IActionResult reviewList(int productid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (productid!=0)
                {
                    List<ReviewsViewModel> model = new List<ReviewsViewModel>();
                    var fatchdata = _reviewsService.GetByproductidReviews(productid);
                    if (fatchdata.Count>0) 
                    {
                        foreach (var item in fatchdata)
                        {
                            ReviewsViewModel model1 = new ReviewsViewModel();
                            model1.product_id=item.ProductId;
                            model1.order_id = item.OrderId.ToString();
                            model1.rating = item.Rating;
                            model1.comment = item.Comment;
                            //model1.Status = item.Status;
                            //model1.CreatedAt = item.CreatedAt;
                            //model1.UpdatedAt = item.UpdatedAt;
                            model.Add(model1);
                        }
                        return Ok(new {error=false,Data = model, message = "Review fetch successfully!", code = 200, status = true });
                    }

                }
                var errorData = new { error = true, message = "this productid behave not avilable reviews.", data = "null", code = 400, status = false };
                return new UnauthorizedResponse(errorData);
            }
            catch (Exception)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion

        #region [Get My Review]
        [Authorize]
        [Route("/review")]
        [HttpPost]
        public IActionResult myreview(GenricSearchSpaces specs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                reviwdata reviewData=new reviwdata();
                List<review> model = new List<review>();
                var fatchdata = _reviewsService.GetByuseridReviews(id, specs);
                var PageMetadate = new
                {
                    fatchdata.CurrentPage,
                    fatchdata.PazeSize,
                    fatchdata.TotalPage,
                    fatchdata.TotalCount,
                    fatchdata.HasNext,
                    fatchdata.HasPrev
                };
                Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));
                if (fatchdata != null && fatchdata.Any())
                 {
                     foreach (var item in fatchdata)
                     {
                        Order orderdata = new Order();
                        P_roduct productdata = new P_roduct();
                        var Odata=_OrdersService.GetById(Convert.ToInt32(item.OrderId));
                        var Pdata = _productService.GetById(item.ProductId);
                        review model1 = new review();
                        orderdata.order_id = Odata.OrderId;
                        //model1.Rating = item.Rating;
                        model1.comment = item.Comment;
                        productdata.title = Pdata.Title;
                        model.Add(model1);
                        model1.product = productdata;
                        model1.order= orderdata;
                        //model1.orderList.Add(orderdata);
                        //model1.productList.Add(productdata);
                        reviewData.data = model;
                        reviewData.current_page = PageMetadate.CurrentPage;
                        reviewData.total_page = PageMetadate.TotalPage;
                        reviewData.page_size = PageMetadate.PazeSize;
                    }
                        return Ok(new { error = false,data = reviewData, message = "Review fetch successfully!", code = 200, status = true });
                 }
                return Ok(new { error = false, data = "", message = "Review fetch successfully!", code = 200, status = true });
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
