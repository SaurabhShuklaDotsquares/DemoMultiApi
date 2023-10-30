using EC.API.ViewModels;
using EC.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;

namespace EC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : BaseAPIController
    {
        #region Constructor
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }
        #endregion

        #region Get CountryList

        [Route("/users/country-list")]
        [HttpGet]
        public IActionResult GetCountries()
        {
            try
            {
                string Message = string.Empty;
                List<CountryViewModel> countryList = new List<CountryViewModel>();  
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var countries = _countryService.GetCountries();
                if (countries != null && countries.Any())
                {
                    foreach (var item in countries)
                    {
                        CountryViewModel model = new CountryViewModel();
                        model.id= item.Id;
                        model.sortname = item.Sortname;
                        model.name = item.Name;
                        model.phonecode = item.Phonecode;
                        countryList.Add(model);
                    }
                    Message = "Country fetch successfully.";
                }
                else
                {
                    Message = "Record Not Found.";
                }

                return Ok(new { error = false, data = countryList, Message = Message, authenticate = true, state = "Country List", code = 200, status = true });
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
