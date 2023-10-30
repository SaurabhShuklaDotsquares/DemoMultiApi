using EC.API.ViewModels.SiteKey;
using EC.Service.Currency;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EC.API
{
    public class CommonMethod
    {
        private const string ApiBaseUrl = "https://api.exchangerate-api.com/v4/latest/";
        private static readonly HttpClient _httpClient;

        static CommonMethod()
        {
            _httpClient = new HttpClient();
        }

        public static decimal ConvertCurrency(decimal amount, string sourceCurrency, string targetCurrency)
        {
            try
            {
                string apiEndpoint = ApiBaseUrl + sourceCurrency.ToUpper();

                var response = _httpClient.GetAsync(apiEndpoint).Result;
                response.EnsureSuccessStatusCode();

                var content = response.Content.ReadAsStringAsync().Result;
                var rates = JsonConvert.DeserializeObject<ExchangeRatesResponse>(content);

                if (rates.Rates.TryGetValue(targetCurrency.ToUpper(), out decimal targetExchangeRate))
                {
                    decimal convertedAmount = amount * targetExchangeRate;
                    return convertedAmount;
                }
                else
                {
                    throw new Exception($"Exchange rate for {targetCurrency} not available.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Currency conversion failed.", ex);
            }
        }

        private class ExchangeRatesResponse
        {
            public string Base { get; set; }
            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}
