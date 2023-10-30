using EC.API.ViewModels;
using EC.API.ViewModels.MultiVendor;
using EC.API.ViewModels.SiteKey;
using EC.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.POIFS.Crypt.Dsig;
using Stripe;
using System;
using ToDo.WebApi.Models;
using static EC.API.Controllers.BaseAPIController;

namespace EC.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StripeController : ControllerBase
    {

        #region Constructor
        private readonly IUsersService _usersService;
        public StripeController(IUsersService usersService)
        {
            _usersService=usersService;
        }
        #endregion

        #region Strip Token

        [Route("/stripe-token")]
        [HttpPost]
        public IActionResult Stripe([FromForm]StripeViewModels stripe)
        {
            

            if (stripe != null)
            {

                StripeConfiguration.ApiKey = "sk_test_l2rWyISuQPr9T3YVUkAnWiL9";
                //StripeConfiguration.ApiKey = "sk_test_51NiaHNKzaZ8bViv1kgukX2zWA3sGVbJlrbl8jhtwypwBXtBf1jKDF5WzvbgZc7jndORwM9CwzyQOkWJu8aipZRCv00tMRS7tj6";

                var tokenOptions = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = stripe.card_number,
                        ExpYear = stripe.exp_year.ToString(),
                        ExpMonth = stripe.exp_month.ToString(),
                        Cvc = stripe.cvc,
                    },
                };

                var tokenService = new TokenService();
                Token stripeToken = tokenService.Create(tokenOptions);

                Object jsonObject = JObject.Parse(stripeToken.RawJObject.ToString());

                StripeObject data = JsonConvert.DeserializeObject<StripeObject>(jsonObject.ToString());

                return Ok(new { error = false, data, message= "Card token fetch successfully!", code = 200, status = true });
            }
            else
            {
                var errorData = new { error = true, message = "Record Not Found.", code = 400, status = false };
                return new UnauthorizedResponse(errorData);
            }
        }
        #endregion

        #region  Genrat Vendor Strip Account
        [Authorize]
        [HttpPost]
        [Route("/vendor/stripe/create-stripe-account")]
        public IActionResult StripeAccountCreate(StripecreateModel stripe)
        {

            if (stripe != null)
            {
                var authuser = new AuthUser(User);
                var id = authuser.Id;
                var data = _usersService.GetById(id);
                StripeConfiguration.ApiKey = "sk_test_l2rWyISuQPr9T3YVUkAnWiL9";
                #region Accoucnt create
                var options = new AccountCreateOptions
                {
                    Type = "express",
                    Country = "US",
                    Email = data.Email,
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions
                        {
                            Requested = true,
                        },
                        Transfers = new AccountCapabilitiesTransfersOptions
                        {
                            Requested = true,
                        },
                    },
                };
                var service = new AccountService();
                var account = service.Create(options);
                #endregion

                #region link create
                var option = new AccountLinkCreateOptions
                {
                    Account = account.Id,
                    RefreshUrl = stripe.redirect_url,
                    ReturnUrl = stripe.redirect_url,
                    Type = "account_onboarding",
                };
                var services = new AccountLinkService();
                var link=services.Create(option);
                #endregion
                return Ok(new { error = false, data = link, message = "payment link get successfully!", code = 200, status = true });
            }
            else
            {
                var errorData = new { error = true, message = "Record Not Found.", code = 400, status = false };
                return new UnauthorizedResponse(errorData);
            }
        }

        #endregion

        #region Update Stripe Account

        [Authorize]
        [HttpPost]
        [Route("/vendor/stripe/update-stripe-key")]
        public IActionResult UpdatestripeId(stripekey key_id)
        {
            try
            {
                var authuser = new AuthUser(User);
                var userId = authuser.Id;
                bool isIdExist = userId != 0;
                var entity = isIdExist ? _usersService.GetById(userId) : new Data.Models.Users();
                if (key_id.stripe_id != "")
                {
                    entity.StripeId = key_id.stripe_id;
                }
                var stripedata = _usersService.UpdateUser(entity);
                Vendor_stripe_Details Data = new Vendor_stripe_Details();

                Data.id= stripedata.Id;
                Data.role= stripedata.Id;
                Data.firstname= stripedata.Firstname;
                Data.email= stripedata.Email;
                Data.mobile= stripedata.Mobile;
                Data.lastname= stripedata.Lastname;
                Data.profile_pic= stripedata.ProfilePic;
                Data.state= stripedata.State;
                Data.email_verified_at = stripedata.EmailVerifiedAt;
                Data.isVerified = Convert.ToInt32(stripedata.IsVerified);
                Data.is_admin = Convert.ToInt32(stripedata.IsAdmin);
                Data.stripe_customer_id = stripedata.StripeCustomerId;
                Data.stripe_id = stripedata.StripeId;
                Data.created_at = stripedata.CreatedAt;
                Data.updated_at = stripedata.UpdatedAt;
                Data.country = stripedata.Country;
                Data.status = stripedata.IsActive;
                Data.postal_code = stripedata.PostalCode;
                Data.country_code = stripedata.CountryCode;
                Data.is_guest = false;
                return Ok(new { error = false, data = Data, message = "Your stripe account verified successfully", code = 200, status = true });
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
