using EC.Core.Enums;
using EC.Core.LIBS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;

namespace EC.Web.Areas.Admin.Code.Security
{

    public class CustomPrincipal
    {
        private readonly ClaimsPrincipal claimsPrincipal;

        public CustomPrincipal(ClaimsPrincipal principal)
        {
            claimsPrincipal = principal;
            this.IsAuthenticated = claimsPrincipal==null?false: claimsPrincipal.Identity.IsAuthenticated;

            if (this.IsAuthenticated)
            {
                Role = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value;
                if (!Role.Equals("user"))
                {
                    Id = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value);
                    Firstname = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(Firstname))?.Value;
                    Lastname = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(Lastname))?.Value;
                    Gender = int.Parse(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(Gender))?.Value);
                    Email = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Email)?.Value;

                    //_imageName = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == nameof(ImageName))?.Value;
                    //if (!string.IsNullOrEmpty(_imageName))
                    //    _imageName = "/Upload/Users/" + _imageName;
                    //else
                    //    _imageName = "/images/no-user.jpg";
                    //FullName = $"{FirstName} {LastName}".Trim();
                    //if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "userPermissions")?.Value != null)
                    //{
                    //    if (claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "userPermissions")?.Value != null)
                    //        Permissions = JsonConvert.DeserializeObject<int[]>(claimsPrincipal.Claims.FirstOrDefault(u => u.Type == "userPermissions")?.Value).ToArray();
                    //}
                }
                //else { IsAuthenticated = false; }
            }
        }

        public bool IsAuthenticated { get; private set; }
        public int Id { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string FullName { get; private set; }
        public int Gender { get; private set; }
        public string Email { get; private set; }

        public string Role { get; private set; }
        public int[] Roles { get; set; }
        public int[] Permissions { get; set; }
        private string _imageName;

        public string ImageName
        {
            get { return _imageName; }
            set
            {
                UpdateClaim(nameof(ImageName), value.ToString());
                _imageName = value;
            }
        }
        public bool IsInRole(RoleType roleType)
        {
            return Roles.Contains((int)roleType);
        }

        //public bool HasPermission(int[] permission)
        //{
        //    var hasPermission = false;
        //    bool isEmpty = Permissions.All(x => x == default(int));
        //    if (!isEmpty)
        //    {
        //         hasPermission= Permissions.Intersect(permission).Any();
        //    }
        //    return hasPermission;
        //}
        public bool HasPermission(int permissionId)
        {
            var hasPermission = false;
            //bool isEmpty = Permissions.All(x => x == default(int));
            //if (!isEmpty)
            //{
            hasPermission = Permissions.Contains(permissionId);
            //}
            return hasPermission;
        }

        private void UpdateClaim(string key, string value)
        {
            var claims = claimsPrincipal.Claims.ToList();
            if (claims.Any())
            {
                var pmClaim = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == key);
                if (pmClaim != null)
                {
                    claims.Remove(pmClaim);
                    claims.Add(new Claim(key, value));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };
            ContextProvider.HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                   new ClaimsPrincipal(claimsIdentity),
                   authProperties
                 ).Wait();
        }


    }

   

}
