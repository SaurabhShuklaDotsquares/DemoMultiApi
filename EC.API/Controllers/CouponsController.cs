using EC.API.ViewModels;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System;
using ToDo.WebApi.Models;
using Stripe;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;
using EC.Data.Models;
using EC.Core.Enums;
using EC.Core;
using static EC.API.Controllers.BaseAPIController;
using EC.Service.Shippings;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CouponsController : BaseAPIController
    {
        #region Constructor
        private readonly ICouponService _couponService;
        private IOrdersService _ordersService;
        private readonly ICartService _cartService;
        private readonly IShippingService _shippingService;
        public CouponsController(ICouponService couponService, ICartService cartService, IOrdersService ordersService, IShippingService shippingService)
        {
            _couponService = couponService;
            _cartService = cartService;
            _ordersService = ordersService;
            _shippingService = shippingService;
        }
        #endregion

        #region Apply Cart Coupon
        [HttpPost]
        [Route("/coupon/apply-code")]
        [Authorize]
        public IActionResult ApplyCoupon(Coupon_code code) 
        {
            try
            {
                CouponViewModel model = new CouponViewModel();
                ResponseCartdata responseCartList = new ResponseCartdata();
                var authuser = new AuthUser(User);
                var Id = authuser.Id;
                var currentdate = DateTime.Now.Date;
                decimal totalAmount = 0;
                
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(code.coupon_code))
                {
                    var coupon = _couponService.GetByCoupons(code.coupon_code);
                    if (coupon == null)
                    {
                        var errorData = new { error = true, message = "Invalid coupon code", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                    var cart = _cartService.GetByIdCart(Id);
                    if (cart == null)
                    {
                        var errorData = new { error = true, message = "Not have any cart value.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                    var couponexpiry = _couponService.CheckCouponExpiry(currentdate, code.coupon_code);
                    if (couponexpiry == null)
                    {
                        var errorData = new { error = true, message = "Coupon has been expired.", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                    else
                    {

                        if (!coupon.IsActive)
                        {
                            var errorData = new { error = true, message = "Coupon has been inactive.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        foreach (var item in cart)
                        {
                            totalAmount += item.FinalValue.Value;
                        }

                        if (coupon.MaximumValue != 0 && coupon.Amount >= totalAmount)
                        {
                            var errorData = new { error = true, message = "Minimum cart value should be " + coupon.Amount, data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        var couponUsageCount = _couponService.CheckVoucherUse(Id).Count;
                        if (coupon.MaximumUsage != 0) { }
                        if (coupon.MaximumUsage <= couponUsageCount && coupon.MaximumUsage != 0 && couponUsageCount != 0)
                        {
                            var errorData = new { error = true, message = "Coupon maximum usage limit is reached.", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData);
                        }
                        else
                        {
                            model.display_total_amount = '$' + totalAmount.ToString();
                            var shippingAmount = _shippingService.GetShippingRates(totalAmount);
                            decimal shipingvalue = shippingAmount.ShippingCharge;
                            model.display_discount = '$' + (coupon.Type == (string)CouponType.Percentage.GetDescription() ? Convert.ToDecimal(totalAmount * coupon.MaximumValue / 100) : Convert.ToDecimal(coupon.MaximumValue)).ToString();
                            model.code = coupon.Code;
                            model.dispay_total = '$' + ((totalAmount - (coupon.Type == (string)CouponType.Percentage.GetDescription() ? Convert.ToDecimal(totalAmount * coupon.MaximumValue / 100) : Convert.ToDecimal(coupon.MaximumValue))) + shipingvalue).ToString();
                            return Ok(new { error = false, data = model, messege = "Coupon Applied Successfully.", code = 200, status = true });

                        }
                    }
                }
                else
                {
                    var errorData = new { error = true, message = "Coupon code required.", data = "null", code = 400, status = false };
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
    }
}

