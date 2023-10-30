using EC.Service.Product;
using EC.Service.Shippings;
using EC.Service.Taxs;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ToDo.WebApi.Models;
using EC.Service.Currency;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencyController : BaseAPIController
    {
        #region Constructor
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrencyController(ICurrencyService currencyService, IHttpContextAccessor httpContextAccessor)
        {
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Set Currency
        [Route("/setcurrency/{currencyId}")]
        [HttpGet]
        public IActionResult SetCurrency(int currencyId)
        {
            try
            {
                var currencies = _currencyService.GetById(currencyId);
                if (currencies != null)
                {
                    HttpContext.Session.SetString("Symbol", currencies.Symbol);
                    HttpContext.Session.SetString("Iso", currencies.Iso);
                }
                return Ok(new { error = false, data = "", message = "Cart count fetch successfully.", code = 200, status = true });
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
