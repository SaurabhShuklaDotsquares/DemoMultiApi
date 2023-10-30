using EC.API.ViewModels;
using EC.Data.Models;
using EC.Service;
using EC.Service.Shippings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftAntimalwareEngine;
using Microsoft.EntityFrameworkCore.Internal;
using Stripe;
using System;
using System.Collections.Generic;
using ToDo.WebApi.Models;
using static EC.API.Controllers.BaseAPIController;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShippingManagerController : BaseAPIController
    {
        #region Constructor
        private ICartService _cartService;
        private IShippingService _shippingService;
        public ShippingManagerController(ICartService cartService, IShippingService shippingService) 
        {
            this._cartService = cartService;
            this._shippingService = shippingService;    
        }
        #endregion

        #region Get Shipping List with Shipping Cgarges
        [HttpGet]
        public IActionResult GetShippingList()
        {
            try
            {
                ShippingViewModel model = new ShippingViewModel();
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                bool isShipping = false;

                var cartList = _cartService.GetByIdCart(userId);
                var shippingchargesList = _shippingService.GetShippingratesList();

                if (cartList != null && cartList.Any())
                {
                    decimal totalAmount = 0;
                    foreach (var item in cartList)
                    {
                        totalAmount += Convert.ToDecimal(item.FinalValue * item.Quantity);
                    }

                    if (shippingchargesList != null && shippingchargesList.Any())
                    {
                        foreach (var shipping in shippingchargesList)
                        {
                            if (shipping.MinimumOrderAmount < totalAmount && shipping.MaximumOrderAmount > totalAmount)
                            {
                                model.Amount = totalAmount;
                                model.ShippingCharge = shipping.ShippingCharge;
                                model.TotalAmount = totalAmount + shipping.ShippingCharge;
                                isShipping = true;
                            }
                        }
                    }
                    // If total amount does not exist in min and max order amount
                    if (!isShipping)
                    {
                        model.Amount = totalAmount;
                        model.ShippingCharge = 0;
                        model.TotalAmount = totalAmount;
                    }
                }

                return Ok(new { error = false, Data = model, message = "Shipping List Fatch Successfully.", code = 200, status = true });
            }
            catch (Exception Ex)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }
        #endregion
    }
}
