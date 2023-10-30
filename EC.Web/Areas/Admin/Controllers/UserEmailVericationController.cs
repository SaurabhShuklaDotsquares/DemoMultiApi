using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EC.Service;
using Microsoft.AspNetCore.Mvc;

namespace EC.Web.Areas.Admin.Controllers
{
    public class UserEmailVericationController : BaseController
    {
        #region [ Service Injection ]
        /// <summary>
        /// Inject Required Services
        /// </summary>
        /// 

        private readonly IUsersService _usersService;
        public UserEmailVericationController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        #endregion [ Service Injection ]

        #region [ EmailVerification ]
        /// <summary>
        /// get id from users email link
        /// </summary>
        /// <param name="id"></param>
        /// <returns>update EmailVerifiedAt in DB</returns>
        public IActionResult EmailVerification(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _usersService.GetById(id);
                    if (user != null)
                    {
                        if (user.EmailVerifiedAt == null && user.IsVerified == false)
                        {
                            user.EmailVerifiedAt = DateTime.Now;
                            user.IsVerified = true;
                            _usersService.UpdateUser(user);
                            ViewBag.SuccessMessage = "Your Email Account is successfully verified.";
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Your are already verified.";
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "User does not exist.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        #endregion [ EmailVerification ]
    }
}
