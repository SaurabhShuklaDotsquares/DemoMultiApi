using EC.API.ViewModels;
using EC.Data.Models;
using EC.Service;
using EC.Service.UserAddressBook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EC.API.Controllers.BaseAPIController;
using Stripe;
using System;
using ToDo.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using EC.Service.Specification;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using EC.Data.Entities;
using UserAddress = EC.Data.Models.UserAddress;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserAddressBookController : BaseAPIController
    {
        #region Constructor
        private readonly IUserAdressBookService _userAdressBookService;
        private readonly IUsersService _userService;
        private readonly ICountryService _CountryService;

        public UserAddressBookController(IUserAdressBookService userAdressBookService, IUsersService userService, ICountryService countryService)
        {
            _userAdressBookService = userAdressBookService;
            _userService = userService;
            _CountryService = countryService;
        }
        #endregion

        #region Add Address 

        [Authorize]
        [Route("/user-address/add")]
        [HttpPost]
        public IActionResult AddAddressbook(UserAddressViewModels model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (model != null)
                {
                    var authuser = new AuthUser(User);
                    var id = authuser.Id;
                    UserAddress entityuseraddressbook = new UserAddress();
                    entityuseraddressbook.UserId = id;
                    entityuseraddressbook.Address1 = model.address1;
                    entityuseraddressbook.Address2 = model.address2;
                    entityuseraddressbook.City = model.city;
                    entityuseraddressbook.AddressType = model.AddressType;
                    entityuseraddressbook.PostalCode = Convert.ToInt32(model.postal_code);
                    entityuseraddressbook.State = model.state;
                    entityuseraddressbook.CountryId = Convert.ToInt32(model.country_id);
                    entityuseraddressbook.CreatedAt = DateTime.Now;
                    entityuseraddressbook.UpdatedAt = DateTime.Now;

                    var data1=_userAdressBookService.SaveUserAddressbook(entityuseraddressbook);
                    var username=_userService.GetUserByuserIdName(id).Firstname;
                   
                    UsersAddressModel entity = new UsersAddressModel();
                    if (data1!=null)
                    {
                        entity.address1 = data1.Address1;
                        entity.address2 = data1.Address2;
                        entity.city = data1.City;
                        entity.postal_code = data1.PostalCode.ToString();
                        entity.state = data1.State;
                        entity.country_id = data1.CountryId.ToString();
                        entity.name = username;
                        //entity.mobile = data1.Mobile;
                        entity.address_type = data1.AddressType;
                        entity.user_id = data1.UserId;
                        entity.id= data1.Id;
                    }
                    return Ok(new { error = false, data = entity, message = "Address added successfully.", state= "add address", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "please send valid data", data = "null", code = 400, status = false };
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

        #region Update Address

        [Authorize]
        [Route("/user-address/update")]
        [HttpPost]
        public IActionResult UpdateAddressbooks(UserAddressViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                if (model.user_address_id > 0)
                {
                    bool isIdExist = id != 0;

                    var entity = isIdExist ? _userAdressBookService.GetById(model.user_address_id,id) : new Data.Models.UserAddress();
                    if (model.address1 != null)
                    {
                        entity.Address1 = model.address1;
                    }
                    else
                    {
                        entity.Address1 = entity.Address1;
                        model.address1 = entity.Address1;
                    }
                    if (model.address2 != null)
                    {
                        entity.Address2 = model.address2;
                    }
                    else
                    {
                        entity.Address2 = entity.Address2;
                        model.address2 = entity.Address2;
                    }
                    if (model.country_id > 0)
                    {
                        entity.CountryId = model.country_id;
                    }
                    else
                    {
                        entity.CountryId = entity.CountryId;
                        model.country_id = entity.CountryId;
                    }
                    if (model.state != null)
                    {
                        entity.State = model.state;
                    }
                    else
                    {
                        entity.State = entity.State;
                        model.state = entity.State;
                    }
                    if (model.postal_code > 0)
                    {
                        entity.PostalCode = model.postal_code;
                    }
                    else
                    {
                        entity.PostalCode = entity.PostalCode;
                        model.postal_code = entity.PostalCode;
                    }
                    if (model.city!=null)
                    {
                        entity.City = model.city;
                    }
                    else
                    {
                        entity.City = entity.City;
                        model.city = entity.City;
                    }
                    if (model.address_type != null)
                    {
                        entity.AddressType = model.address_type;
                    }
                    else
                    {
                        entity.AddressType = entity.AddressType;
                        model.address_type = entity.AddressType;
                    }
                    if (model.name != null)
                    {
                        entity.Name = model.name;
                    }
                    else
                    {
                        entity.Name = entity.Name;
                        model.name = entity.Name;
                    }
                    var entity1 = _userAdressBookService.UpdateUserAddressbook(entity);

                    UserUpdateAddressViewModel updatedata= new UserUpdateAddressViewModel();

                    if (entity1 != null)
                    {
                        updatedata.id = entity1.Id;
                        updatedata.user_id = entity1.UserId;
                        updatedata.name = entity1.Name;
                        updatedata.address1 = entity1.Address1;
                        updatedata.address2 = entity1.Address2;
                        updatedata.address_type = entity1.AddressType;
                        updatedata.city = entity1.City;
                        updatedata.country_id = entity1.CountryId.ToString();
                        updatedata.postal_code = entity1.PostalCode.ToString();
                        updatedata.state = entity1.State;
                    }
                    return Ok(new { error = false, data = updatedata, message = "Address Update sucessfully.", state = "Update address",code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Not found record", data = "null", code = 400, status = false };
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

        #region Address List

        [Authorize]
        [Route("/user-address")]
        [HttpGet]
        public IActionResult GetUserAddresslist([FromQuery] ToDoSearchSpecs specs)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var users = _userAdressBookService.GetUserAddressList(id, specs);
                var PageMetadate = new
                {
                    users.CurrentPage,
                    users.PazeSize,
                    users.TotalPage,
                    users.TotalCount,
                    users.HasNext,
                    users.HasPrev
                };
                Response.Headers.Add("X-Pagination", System.Text.Json.JsonSerializer.Serialize(PageMetadate));
                UserAddress2 modeles = new UserAddress2();
                List<UserAddressModel> models = new List<UserAddressModel>();
                if (users != null)
                {
                    foreach (var item in users)
                    {
                        UserAddressModel model = new UserAddressModel();
                        model.id = item.Id;
                        model.address1 = item.Address1;
                        model.address2 = item.Address2;
                        model.city = item.City;
                        model.postal_code = item.PostalCode;
                        model.state = item.State;
                        model.country_id = item.CountryId;
                        //var countryBillingname = _CountryService.GetById(Convert.ToInt32(item.CountryId));
                        //if (countryBillingname != null)
                        //{
                        //    model.country_id = countryBillingname.Name.ToString();
                        //}
                        model.address_type = item.AddressType;
                        model.mobile = item.Mobile;
                        model.name = authuser.Name;
                        models.Add(model);

                    }
                    modeles.data = models;
                    return Ok(new
                    {
                        error = false,
                        currentpage = PageMetadate.CurrentPage,
                        data = modeles,
                        message = "Address fetch successfully.",
                        totalpage = PageMetadate.TotalPage,
                        pagrsize = PageMetadate.PazeSize,
                        state = "address",
                        code = 200,
                        status = true
                    });
                    //return Ok(new { error = false, currentpage = PageMetadate.CurrentPage,totalpage=PageMetadate.TotalCount,pagrsize=PageMetadate.PazeSize, data = models, message = "Address fetch successfully.", state = "address", code = 200, status = true });
                }
                else
                {
                    var errorData = new { error = true, message = "Not found", data = "null", code = 400, status = false };
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

        #region Address Delete

        [Authorize]
        [Route("/user-address/delete/{id:int}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var Id = authuser.Id;
                if (id > 0)
                {
                    var entity = _userAdressBookService.GetById(id, Id);
                    if (entity!=null)
                    {
                        bool isDeleted = _userAdressBookService.Delete(entity.Id);
                        if (isDeleted)
                        {
                            return Ok(new { error = false, message = "Address deleted successfully.", state = "address", code = 200, status = true });
                        }
                        else
                        {
                            var errorData1 = new { error = true, message = "Not found book", data = "null", code = 400, status = false };
                            return new UnauthorizedResponse(errorData1);
                        }

                    }
                    var errorData = new { error = true, message = "Not found book", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);

                }
                else
                {
                    var errorData = new { error = true, message = "Not found book", data = "null", code = 400, status = false };
                    return new UnauthorizedResponse(errorData);
                }
            }
            catch (Exception)
            {
                var errorData = new { error = true, message = "Internal Server Error.", code = 500, status = false };
                return new InternalResponse(errorData);
            }
        }

        #endregion

        #region User Details

        [Authorize]
        [Route("/user-address/detail/{id:int}")]
        [HttpGet]
        public IActionResult GetUserAddressdetails(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var authuser = new AuthUser(User);
                var Id = authuser.Id;
                if (id > 0)
                {
                    var entity = _userAdressBookService.GetById(id, Id);
                    UserAddressModel model = new UserAddressModel();
                    if (entity != null)
                    {
                        model.id = entity.Id;
                        model.address1 = entity.Address1;
                        model.address2 = entity.Address2;
                        model.city = entity.City;
                        model.postal_code = entity.PostalCode;
                        model.state = entity.State;
                        model.country_id = entity.CountryId;
                        model.address_type = entity.AddressType;
                        model.mobile = entity.Mobile;
                        model.name = entity.Name;
                        return Ok(new { error = false, data = model, message = "Address fetch successfully.",
                            state= "address", code = 200, status = true });
                        }
                    else
                    {
                        var errorData = new { error = true, message = "Not found", data = "null", code = 400, status = false };
                        return new UnauthorizedResponse(errorData);
                    }
                }
                else
                {
                    var errorData = new { error = true, message = "Not found", data = "null", code = 400, status = false };
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
