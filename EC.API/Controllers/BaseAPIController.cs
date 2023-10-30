using EC.API.Configs;
using EC.API.ViewModels.SiteKey;
using EC.Data.Entities;
using EC.Service;
using EC.Service.Currency;
using EC.Service.Currency_data;
using EC.Service.Taxs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace EC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase
    {
        #region Status Code

        public class UnauthorizedResponse : ObjectResult
        {
            public UnauthorizedResponse(object value) : base(value)
            {
                StatusCode = 400; // Set the HTTP status code to 401 (Unauthorized)
            }
        }
        public class InternalResponse : ObjectResult
        {
            public InternalResponse(object value) : base(value)
            {
                StatusCode = 500; // Set the HTTP status code to 401 (Unauthorized)
            }
        }
        #endregion

        #region Converting Price

        [HttpGet]
        public string ConvertPrice(decimal amount)
        {
            IServiceProvider serviceProvider = HttpContext.RequestServices;
            ICurrenciesdataService _currenciesdataService = serviceProvider.GetService<ICurrenciesdataService>();
            ICurrencyService _currencyService = serviceProvider.GetService<ICurrencyService>();

            string returnAmount = string.Empty;
            var currencyId = HttpContext.Session.GetString("currencyId") != null ? HttpContext.Session.GetString("currencyId") : null;
            if (currencyId != null)
            {
                var targetCurrenciesData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                if (targetCurrenciesData != null)
                {
                    decimal convertedAmount = CommonMethod.ConvertCurrency(amount, SiteKey.CurrencyIso, targetCurrenciesData.Currency.Iso);
                    returnAmount = targetCurrenciesData.Currency.SymbolNative + string.Format("{0:0.00}", convertedAmount);
                }
                else
                {
                    returnAmount = SiteKey.CurrencySymbol + string.Format("{0:0.00}", amount);
                }
            }
            else
            {
                var currencyData = _currencyService.GetById(Convert.ToInt32(SiteKey.DefaultCurrency));
                if (currencyData != null)
                {
                    decimal convertedAmount = CommonMethod.ConvertCurrency(amount, SiteKey.CurrencyIso, currencyData.Iso);
                    returnAmount = currencyData.SymbolNative + string.Format("{0:0.00}", convertedAmount);
                }
                else
                {
                    returnAmount = SiteKey.CurrencySymbol + string.Format("{0:0.00}", amount);
                }
            }
            return returnAmount;
        }

        protected decimal ConvertPriceInDecimal(decimal amount) 
        {
            IServiceProvider serviceProvider = HttpContext.RequestServices;
            ICurrenciesdataService _currenciesdataService = serviceProvider.GetService<ICurrenciesdataService>();
            ICurrencyService _currencyService = serviceProvider.GetService<ICurrencyService>();

            decimal returnAmount = 0;
            var currencyId = HttpContext.Session.GetString("currencyId") != null ? HttpContext.Session.GetString("currencyId") : null;
            if (currencyId != null)
            {
                var targetCurrenciesData = _currenciesdataService.GetById(Convert.ToInt32(currencyId));
                if (targetCurrenciesData != null)
                {
                    decimal convertedAmount = CommonMethod.ConvertCurrency(amount, SiteKey.CurrencyIso, targetCurrenciesData.Currency.Iso);
                    returnAmount = convertedAmount;
                }
                else
                {
                    returnAmount = amount;
                }
            }
            else
            {
                var currencyData = _currencyService.GetById(Convert.ToInt32(currencyId));
                if (currencyData != null)
                {
                    decimal convertedAmount = CommonMethod.ConvertCurrency(amount, SiteKey.CurrencyIso, currencyData.Iso);
                    returnAmount = convertedAmount;
                }
                else
                {
                    returnAmount = amount;
                }
            }
            return returnAmount;
        }

        #endregion

        #region Genrat Slug

        protected string GenerateUniqueSlug(string slugProduct)
        {
            string slug = slugProduct.ToLower();  // Convert to lowercase

            // Remove special characters and replace spaces with dashes
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");

            // Append a unique identifier if the slug already exists
            string uniqueSlug = slug;
            int counter = 1;
            while (SlugExists(uniqueSlug))
            {
                uniqueSlug = $"{slug}-{counter}";
                counter++;
            }

            return uniqueSlug;
        }

        protected bool SlugExists(string slugProduct)
        {
            IServiceProvider serviceProvider = HttpContext.RequestServices;
            IProductService _productService = serviceProvider.GetService<IProductService>();

            var product = _productService.GetBySlug(slugProduct);
            if (product != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Tax 
        protected decimal PriceWithTax(decimal amount, int categoryId)
        {
            decimal returnAmount = amount != 0 && amount != null ? amount : 0;
            IServiceProvider serviceProvider = HttpContext.RequestServices;
            ITaxService _taxService = serviceProvider.GetService<ITaxService>();
            var tax = _taxService.GetTaxByCategoryId(categoryId);
            if (tax != null)
            {
                returnAmount = tax.Value.Value != 0 ? returnAmount + tax.Value.Value : returnAmount;
            }

            return returnAmount;
        }

        #endregion

    }
}
