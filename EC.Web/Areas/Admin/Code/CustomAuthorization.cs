using EC.Core.Enums;
using EC.Core.LIBS;
using EC.Web.Areas.Admin.Code.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;

namespace EC.Web.Areas.Admin.Code
{
    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {
        public HttpContextAccessor accessor;
        //private int[] permission;
        //private int permissionId;
        //private byte[] roleTypes;
        private RoleType[] roleTypes;
        //public CustomAuthorization(params AppPermissions[] permission)
        //{
        //    this.permission = Array.ConvertAll(permission, value => (int)value);
        //}

        public CustomAuthorization(params RoleType[] roleTypes)
        {
            this.roleTypes = roleTypes;
        }

        protected virtual CustomPrincipal CurrentUser
        {
           get { return new CustomPrincipal(ContextProvider.HttpContext.User); }
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (CurrentUser == null || !CurrentUser.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/admin/");
            }
            if (CurrentUser.IsAuthenticated && !CurrentUser.Role.Equals(RoleType.Administrator.GetName()))
            {
                ReturnAccessDenied(filterContext);
            }
            #region Not in User For now

            //if (CurrentUser != null && CurrentUser.IsAuthenticated)
            //{
            //    //if (permission.Length > 1)
            //    //{
            //    //    if (isNumeric)
            //    //    {
            //    //        int lastIndex = permission[permission.Length - 1];
            //    //        permissionId = lastIndex;
            //    //        // permission = permission.Intersect(new int[] { lastItem }).ToArray();

            //    //    }
            //    //    else
            //    //    {
            //    //        permissionId = permission[0];

            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    permissionId = permission[0];
            //    //}
            //    if (CurrentUser.Role != StaticRole.Administrator.GetDescription())
            //    {

            //        if (!CurrentUser.HasPermission(permissionId))
            //        {
            //            ReturnAccessDenied(filterContext);
            //        }

            //    }
            //}
            //else
            //{
            //    filterContext.Result = new RedirectResult("~/admin");
            //}
            #endregion
        }

        private void ReturnAccessDenied(AuthorizationFilterContext filterContext)
        {
            //var isAjax = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            //if (isAjax)
            //{
            //    filterContext.Result = new RedirectToActionResult("accessDenied", "error");
            //    //    controller = "error",
            //    //    action = "accessDenied"
            //    //}));
            //}
            //else
            //{
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "error",
                    action = "accessDenied"
                }));
           // }
        }

    }
    public class CustomActionAuthorization : ActionFilterAttribute
    {
        protected virtual CustomPrincipal CurrentUser
        {
            get { return new CustomPrincipal(ContextProvider.HttpContext.User); }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext != null)
            {
                if (!CurrentUser.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("~/account/login");
                }
            }
        }
    }
}
